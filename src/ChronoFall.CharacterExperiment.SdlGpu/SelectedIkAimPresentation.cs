using System.Numerics;

namespace ChronoFall.CharacterExperiment.SdlGpu;

internal sealed record SelectedIkAimPose(
    SkeletonPose Pose,
    AimOffset? AimOffset,
    float EndEffectorError);

internal sealed class SelectedIkAimPresentation
{
    internal const float TargetShiftReachFraction = 0.15f;
    internal const float AimYawRadians = 20.0f * MathF.PI / 180.0f;
    internal const float AimPitchRadians = 10.0f * MathF.PI / 180.0f;
    internal const float MaximumYawRadians = 25.0f * MathF.PI / 180.0f;
    internal const float MaximumPitchRadians = 15.0f * MathF.PI / 180.0f;

    private readonly SkeletonDefinition skeleton;
    private readonly int aimJointIndex;
    private readonly int primaryHandJointIndex;
    private readonly int chainRootIndex;
    private readonly int chainMiddleIndex;
    private readonly int chainEndIndex;
    private readonly TwoBoneIkChain chain;
    private readonly SkeletonSocketSet primarySocketSet;
    private readonly AttachmentReferencePointSet aimReferencePointSet;
    private readonly AimOffsetLimits aimLimits = new(MaximumYawRadians, MaximumPitchRadians);

    internal SelectedIkAimPresentation(SkeletonDefinition skeleton)
    {
        ArgumentNullException.ThrowIfNull(skeleton);
        this.skeleton = skeleton;
        aimJointIndex = FindJointIndex(skeleton, "spine_03");
        primaryHandJointIndex = FindJointIndex(skeleton, "hand_r");
        chainRootIndex = FindJointIndex(skeleton, "upperarm_l");
        chainMiddleIndex = FindJointIndex(skeleton, "lowerarm_l");
        chainEndIndex = FindJointIndex(skeleton, "hand_l");
        chain = new TwoBoneIkChain(skeleton, chainRootIndex, chainMiddleIndex, chainEndIndex);
        primarySocketSet = new SkeletonSocketSet(
            skeleton,
            [new SkeletonSocketDefinition("primary-hand", primaryHandJointIndex, JointTransform.Identity)]);
        aimReferencePointSet = new AttachmentReferencePointSet([
            new AttachmentReferencePointDefinition(
                "aim",
                AttachmentReferencePointRole.Aim,
                JointTransform.Identity),
        ]);
    }

    internal SelectedIkAimPose Apply(
        SkeletonPose sourcePose,
        bool applyAim,
        bool applyIk)
    {
        ArgumentNullException.ThrowIfNull(sourcePose);
        if (!ReferenceEquals(sourcePose.Skeleton, skeleton))
            throw new ArgumentException("The selected IK/Aim proof requires its configured skeleton.", nameof(sourcePose));

        SkeletonPose pose = sourcePose;
        AimOffset? aimOffset = null;
        if (applyAim)
        {
            ProbeFrames probe = CreateProbeFrames(pose);
            aimOffset = AimOffsetEvaluator.EvaluateModelSpace(
                probe.AimReferenceModelTransform,
                OffsetAimDirection(probe.AimReferenceModelTransform),
                aimLimits);
            pose = AimOffsetApplier.ApplyModelSpace(pose, aimJointIndex, aimOffset.Value, 1.0f);
        }

        float endEffectorError = 0.0f;
        if (applyIk)
        {
            ProbeFrames probe = CreateProbeFrames(pose);
            Matrix4x4 target = probe.GripPlacement.OffHandTargetModelTransform!.Value;
            pose = TwoBoneIkSolver.ApplyModelSpace(
                pose,
                chain,
                target,
                probe.PoleModelPosition,
                1.0f);
            SkeletonGlobalPose solvedGlobal = SkeletonPoseEvaluator.EvaluateGlobal(pose);
            endEffectorError = Vector3.Distance(
                GetPosition(target),
                GetPosition(solvedGlobal.GlobalTransforms[chainEndIndex]));
        }

        return new SelectedIkAimPose(pose, aimOffset, endEffectorError);
    }

    private ProbeFrames CreateProbeFrames(SkeletonPose pose)
    {
        SkeletonGlobalPose globalPose = SkeletonPoseEvaluator.EvaluateGlobal(pose);
        SkeletonSocketPose sockets = SkeletonSocketEvaluator.EvaluateModelSpace(primarySocketSet, globalPose);
        if (!sockets.TryGetModelTransform("primary-hand", out Matrix4x4 primarySocket))
            throw new InvalidOperationException("The selected primary-hand socket did not resolve.");

        Vector3 rootPosition = GetPosition(globalPose.GlobalTransforms[chainRootIndex]);
        Vector3 middlePosition = GetPosition(globalPose.GlobalTransforms[chainMiddleIndex]);
        Vector3 endPosition = GetPosition(globalPose.GlobalTransforms[chainEndIndex]);
        float reach =
            Vector3.Distance(rootPosition, middlePosition) +
            Vector3.Distance(middlePosition, endPosition);
        if (!Matrix4x4.Invert(primarySocket, out Matrix4x4 inversePrimarySocket))
            throw new InvalidOperationException("The selected primary-hand socket was not invertible.");
        Matrix4x4 endRelativeToWeapon =
            globalPose.GlobalTransforms[chainEndIndex] * inversePrimarySocket;
        if (!Matrix4x4.Decompose(
                endRelativeToWeapon,
                out _,
                out Quaternion localRotation,
                out Vector3 localTranslation))
        {
            throw new InvalidOperationException("The selected off-hand frame was not decomposable in weapon space.");
        }

        Vector3 towardPrimary = localTranslation.LengthSquared() > 1e-8f
            ? -Vector3.Normalize(localTranslation)
            : Vector3.UnitZ;
        var grip = new WeaponGripDefinition(
            JointTransform.Identity,
            new JointTransform(
                localTranslation + towardPrimary * (reach * TargetShiftReachFraction),
                localRotation,
                Vector3.One));
        WeaponGripPlacement placement = WeaponGripEvaluator.EvaluateModelSpace(grip, primarySocket);
        AttachmentReferencePointPose referencePoints = AttachmentReferencePointEvaluator.EvaluateModelSpace(
            aimReferencePointSet,
            placement.WeaponModelTransform);
        if (!referencePoints.TryGetModelTransform("aim", out Matrix4x4 aimReference))
            throw new InvalidOperationException("The selected Aim reference point did not resolve.");

        return new ProbeFrames(placement, aimReference, middlePosition);
    }

    private static Vector3 OffsetAimDirection(Matrix4x4 aimReferenceModelTransform)
    {
        if (!Matrix4x4.Decompose(aimReferenceModelTransform, out _, out Quaternion rotation, out _))
            throw new InvalidOperationException("The selected Aim reference was not decomposable.");
        float cosinePitch = MathF.Cos(AimPitchRadians);
        var localDirection = new Vector3(
            MathF.Sin(AimYawRadians) * cosinePitch,
            MathF.Sin(AimPitchRadians),
            MathF.Cos(AimYawRadians) * cosinePitch);
        return Vector3.Normalize(Vector3.Transform(localDirection, rotation));
    }

    private static int FindJointIndex(SkeletonDefinition skeleton, string name)
    {
        if (skeleton.TryGetJointIndex(name, out int index))
            return index;
        throw new InvalidOperationException($"Required selected-skeleton joint '{name}' was not found.");
    }

    private static Vector3 GetPosition(Matrix4x4 transform) =>
        new(transform.M41, transform.M42, transform.M43);

    private sealed record ProbeFrames(
        WeaponGripPlacement GripPlacement,
        Matrix4x4 AimReferenceModelTransform,
        Vector3 PoleModelPosition);
}
