using System.Numerics;

namespace ChronoFall.CharacterPresentation;

public readonly record struct AimOffsetLimits
{
    public AimOffsetLimits(float maximumYawRadians, float maximumPitchRadians)
    {
        if (!float.IsFinite(maximumYawRadians) || maximumYawRadians < 0.0f || maximumYawRadians > MathF.PI)
        {
            throw new ArgumentOutOfRangeException(
                nameof(maximumYawRadians),
                "Maximum yaw must be finite and between zero and pi radians.");
        }
        if (!float.IsFinite(maximumPitchRadians) || maximumPitchRadians < 0.0f || maximumPitchRadians > MathF.PI * 0.5f)
        {
            throw new ArgumentOutOfRangeException(
                nameof(maximumPitchRadians),
                "Maximum pitch must be finite and between zero and pi/2 radians.");
        }

        MaximumYawRadians = maximumYawRadians;
        MaximumPitchRadians = maximumPitchRadians;
    }

    public float MaximumYawRadians { get; }

    public float MaximumPitchRadians { get; }

    internal void Validate(string parameterName)
    {
        _ = new AimOffsetLimits(MaximumYawRadians, MaximumPitchRadians);
    }
}

public readonly record struct AimOffset
{
    internal AimOffset(
        float yawRadians,
        float pitchRadians,
        Quaternion modelRotationDelta,
        bool wasClamped)
    {
        YawRadians = yawRadians;
        PitchRadians = pitchRadians;
        ModelRotationDelta = DataValidation.NormalizeRotation(modelRotationDelta, nameof(modelRotationDelta));
        WasClamped = wasClamped;
    }

    public float YawRadians { get; }

    public float PitchRadians { get; }

    public Quaternion ModelRotationDelta { get; }

    public bool WasClamped { get; }

    internal void Validate(string parameterName)
    {
        if (!float.IsFinite(YawRadians) || !float.IsFinite(PitchRadians))
            throw new ArgumentException("Aim offset angles must be finite.", parameterName);
        DataValidation.NormalizeRotation(ModelRotationDelta, parameterName);
    }
}

public static class AimOffsetEvaluator
{
    public static AimOffset EvaluateModelSpace(
        Matrix4x4 aimReferenceModelTransform,
        Vector3 desiredAimDirectionModel,
        AimOffsetLimits limits)
    {
        limits.Validate(nameof(limits));
        Quaternion aimRotation = PresentationTransformMath.ExtractNearRigidRotation(
            aimReferenceModelTransform,
            nameof(aimReferenceModelTransform),
            "Aim reference model transform");
        Vector3 desiredDirection = PresentationTransformMath.NormalizeDirection(
            desiredAimDirectionModel,
            nameof(desiredAimDirectionModel),
            "Desired aim direction");

        Matrix4x4 aimRotationMatrix = Matrix4x4.CreateFromQuaternion(aimRotation);
        if (!Matrix4x4.Invert(aimRotationMatrix, out Matrix4x4 inverseAimRotation))
            throw new InvalidOperationException("The validated Aim rotation could not be inverted.");
        Vector3 localDirection = Vector3.Normalize(
            Vector3.TransformNormal(desiredDirection, inverseAimRotation));
        float requestedYaw = MathF.Atan2(localDirection.X, localDirection.Z);
        float requestedPitch = MathF.Atan2(
            localDirection.Y,
            MathF.Sqrt(localDirection.X * localDirection.X + localDirection.Z * localDirection.Z));
        float yaw = Math.Clamp(requestedYaw, -limits.MaximumYawRadians, limits.MaximumYawRadians);
        float pitch = Math.Clamp(requestedPitch, -limits.MaximumPitchRadians, limits.MaximumPitchRadians);

        float cosinePitch = MathF.Cos(pitch);
        var desiredLocalDirection = new Vector3(
            MathF.Sin(yaw) * cosinePitch,
            MathF.Sin(pitch),
            MathF.Cos(yaw) * cosinePitch);
        Vector3 currentForward = Vector3.TransformNormal(Vector3.UnitZ, aimRotationMatrix);
        Vector3 clampedDirection = Vector3.TransformNormal(desiredLocalDirection, aimRotationMatrix);
        Quaternion delta = PresentationTransformMath.CreateShortestArcRotation(
            currentForward,
            clampedDirection,
            nameof(desiredAimDirectionModel));
        bool wasClamped = yaw != requestedYaw || pitch != requestedPitch;
        return new AimOffset(yaw, pitch, delta, wasClamped);
    }
}

public static class AimOffsetApplier
{
    public static SkeletonPose ApplyModelSpace(
        SkeletonPose sourcePose,
        int jointIndex,
        AimOffset offset,
        float amount)
    {
        ArgumentNullException.ThrowIfNull(sourcePose);
        if (jointIndex < 0 || jointIndex >= sourcePose.Skeleton.JointCount)
        {
            throw new ArgumentOutOfRangeException(
                nameof(jointIndex),
                $"Joint index must be between zero and {sourcePose.Skeleton.JointCount - 1}.");
        }
        offset.Validate(nameof(offset));
        if (!float.IsFinite(amount) || amount < 0.0f || amount > 1.0f)
            throw new ArgumentOutOfRangeException(nameof(amount), "Aim amount must be finite and between zero and one.");

        SkeletonGlobalPose globalPose = SkeletonPoseEvaluator.EvaluateGlobal(sourcePose);
        Quaternion currentModelRotation = PresentationTransformMath.ExtractNearRigidRotation(
            globalPose.GlobalTransforms[jointIndex],
            nameof(sourcePose),
            $"Aim joint {jointIndex} model transform");
        Quaternion appliedDelta = AnimationInterpolationMath.InterpolateRotation(
            Quaternion.Identity,
            offset.ModelRotationDelta,
            amount,
            nameof(amount));
        Quaternion desiredModelRotation = PresentationTransformMath.AppendModelRotation(
            currentModelRotation,
            appliedDelta,
            nameof(offset));

        int parentIndex = sourcePose.Skeleton.Joints[jointIndex].ParentIndex;
        Quaternion? parentRotation = parentIndex < 0
            ? null
            : PresentationTransformMath.ExtractNearRigidRotation(
                globalPose.GlobalTransforms[parentIndex],
                nameof(sourcePose),
                $"Parent of aim joint {jointIndex} model transform");
        Quaternion localRotation = PresentationTransformMath.ConvertModelToLocalRotation(
            desiredModelRotation,
            parentRotation,
            nameof(offset));

        var transforms = sourcePose.LocalTransforms.ToArray();
        JointTransform current = transforms[jointIndex];
        transforms[jointIndex] = new JointTransform(current.Translation, localRotation, current.Scale);
        return new SkeletonPose(sourcePose.Skeleton, transforms);
    }
}
