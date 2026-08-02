using System.Numerics;

namespace ChronoFall.CharacterPresentation;

public sealed class TwoBoneIkChain
{
    public TwoBoneIkChain(
        SkeletonDefinition skeleton,
        int rootJointIndex,
        int middleJointIndex,
        int endJointIndex)
    {
        ArgumentNullException.ThrowIfNull(skeleton);
        ValidateJointIndex(skeleton, rootJointIndex, nameof(rootJointIndex));
        ValidateJointIndex(skeleton, middleJointIndex, nameof(middleJointIndex));
        ValidateJointIndex(skeleton, endJointIndex, nameof(endJointIndex));
        if (skeleton.Joints[middleJointIndex].ParentIndex != rootJointIndex)
        {
            throw new ArgumentException(
                "The middle joint must be a direct child of the root joint.",
                nameof(middleJointIndex));
        }
        if (skeleton.Joints[endJointIndex].ParentIndex != middleJointIndex)
        {
            throw new ArgumentException(
                "The end joint must be a direct child of the middle joint.",
                nameof(endJointIndex));
        }

        Skeleton = skeleton;
        RootJointIndex = rootJointIndex;
        MiddleJointIndex = middleJointIndex;
        EndJointIndex = endJointIndex;
    }

    public SkeletonDefinition Skeleton { get; }

    public int RootJointIndex { get; }

    public int MiddleJointIndex { get; }

    public int EndJointIndex { get; }

    private static void ValidateJointIndex(
        SkeletonDefinition skeleton,
        int jointIndex,
        string parameterName)
    {
        if (jointIndex < 0 || jointIndex >= skeleton.JointCount)
        {
            throw new ArgumentOutOfRangeException(
                parameterName,
                $"Joint index must be between zero and {skeleton.JointCount - 1}.");
        }
    }
}

public static class TwoBoneIkSolver
{
    public static SkeletonPose ApplyModelSpace(
        SkeletonPose sourcePose,
        TwoBoneIkChain chain,
        Matrix4x4 targetModelTransform,
        Vector3 poleModelPosition,
        float amount)
    {
        ArgumentNullException.ThrowIfNull(sourcePose);
        ArgumentNullException.ThrowIfNull(chain);
        if (!ReferenceEquals(sourcePose.Skeleton, chain.Skeleton))
            throw new ArgumentException("The pose and IK chain must use the same skeleton instance.", nameof(chain));
        if (!float.IsFinite(amount) || amount < 0.0f || amount > 1.0f)
            throw new ArgumentOutOfRangeException(nameof(amount), "IK amount must be finite and between zero and one.");
        if (!DataValidation.IsFinite(poleModelPosition))
            throw new ArgumentException("Pole position must contain only finite values.", nameof(poleModelPosition));

        Quaternion targetRotation = PresentationTransformMath.ExtractNearRigidRotation(
            targetModelTransform,
            nameof(targetModelTransform),
            "IK target model transform");
        Vector3 requestedTarget = PresentationTransformMath.ExtractTranslation(targetModelTransform);
        SkeletonGlobalPose sourceGlobal = SkeletonPoseEvaluator.EvaluateGlobal(sourcePose);
        ChainGeometry geometry = ReadGeometry(sourceGlobal, chain);
        Vector3 resolvedTarget = ResolveReachableTarget(geometry, requestedTarget);
        Vector3 bendDirection = ResolveBendDirection(geometry, resolvedTarget, poleModelPosition);
        Vector3 desiredMiddle = ResolveMiddlePosition(geometry, resolvedTarget, bendDirection);

        SkeletonPose rootAdjusted = RotateJointToward(
            sourcePose,
            chain.RootJointIndex,
            geometry.MiddlePosition - geometry.RootPosition,
            desiredMiddle - geometry.RootPosition);
        SkeletonGlobalPose rootAdjustedGlobal = SkeletonPoseEvaluator.EvaluateGlobal(rootAdjusted);
        Vector3 adjustedMiddle = GetPosition(rootAdjustedGlobal, chain.MiddleJointIndex);
        Vector3 adjustedEnd = GetPosition(rootAdjustedGlobal, chain.EndJointIndex);
        SkeletonPose middleAdjusted = RotateJointToward(
            rootAdjusted,
            chain.MiddleJointIndex,
            adjustedEnd - adjustedMiddle,
            resolvedTarget - adjustedMiddle);
        SkeletonPose solved = SetJointModelRotation(middleAdjusted, chain.EndJointIndex, targetRotation);

        var output = sourcePose.LocalTransforms.ToArray();
        foreach (int jointIndex in new[] { chain.RootJointIndex, chain.MiddleJointIndex, chain.EndJointIndex })
        {
            JointTransform source = sourcePose.LocalTransforms[jointIndex];
            JointTransform destination = solved.LocalTransforms[jointIndex];
            output[jointIndex] = new JointTransform(
                source.Translation,
                AnimationInterpolationMath.InterpolateRotation(
                    source.Rotation,
                    destination.Rotation,
                    amount,
                    nameof(amount)),
                source.Scale);
        }

        return new SkeletonPose(sourcePose.Skeleton, output);
    }

    private static ChainGeometry ReadGeometry(
        SkeletonGlobalPose globalPose,
        TwoBoneIkChain chain)
    {
        Matrix4x4 rootTransform = globalPose.GlobalTransforms[chain.RootJointIndex];
        Matrix4x4 middleTransform = globalPose.GlobalTransforms[chain.MiddleJointIndex];
        Matrix4x4 endTransform = globalPose.GlobalTransforms[chain.EndJointIndex];
        PresentationTransformMath.ExtractNearRigidRotation(rootTransform, nameof(globalPose), "IK root model transform");
        PresentationTransformMath.ExtractNearRigidRotation(middleTransform, nameof(globalPose), "IK middle model transform");
        PresentationTransformMath.ExtractNearRigidRotation(endTransform, nameof(globalPose), "IK end model transform");

        Vector3 root = PresentationTransformMath.ExtractTranslation(rootTransform);
        Vector3 middle = PresentationTransformMath.ExtractTranslation(middleTransform);
        Vector3 end = PresentationTransformMath.ExtractTranslation(endTransform);
        float firstLength = Vector3.Distance(root, middle);
        float secondLength = Vector3.Distance(middle, end);
        if (firstLength <= PresentationTransformMath.DirectionEpsilon ||
            secondLength <= PresentationTransformMath.DirectionEpsilon)
        {
            throw new ArgumentException("IK chain segments must have non-zero model-space length.", nameof(globalPose));
        }

        return new ChainGeometry(root, middle, end, firstLength, secondLength);
    }

    private static Vector3 ResolveReachableTarget(
        ChainGeometry geometry,
        Vector3 requestedTarget)
    {
        Vector3 requestedDirection = requestedTarget - geometry.RootPosition;
        float requestedDistance = requestedDirection.Length();
        Vector3 currentDirection = geometry.EndPosition - geometry.RootPosition;
        if (currentDirection.LengthSquared() <=
            PresentationTransformMath.DirectionEpsilon * PresentationTransformMath.DirectionEpsilon)
        {
            currentDirection = geometry.MiddlePosition - geometry.RootPosition;
        }
        Vector3 direction = requestedDistance > PresentationTransformMath.DirectionEpsilon
            ? requestedDirection / requestedDistance
            : PresentationTransformMath.NormalizeDirection(
                currentDirection,
                nameof(requestedTarget),
                "Current IK direction");
        float minimumReach = MathF.Abs(geometry.FirstLength - geometry.SecondLength);
        float maximumReach = geometry.FirstLength + geometry.SecondLength;
        float resolvedDistance = Math.Clamp(requestedDistance, minimumReach, maximumReach);
        return geometry.RootPosition + direction * resolvedDistance;
    }

    private static Vector3 ResolveBendDirection(
        ChainGeometry geometry,
        Vector3 resolvedTarget,
        Vector3 poleModelPosition)
    {
        Vector3 targetDirection = resolvedTarget - geometry.RootPosition;
        if (targetDirection.LengthSquared() <= PresentationTransformMath.DirectionEpsilon * PresentationTransformMath.DirectionEpsilon)
            targetDirection = geometry.EndPosition - geometry.RootPosition;
        if (targetDirection.LengthSquared() <= PresentationTransformMath.DirectionEpsilon * PresentationTransformMath.DirectionEpsilon)
            targetDirection = geometry.MiddlePosition - geometry.RootPosition;
        targetDirection = Vector3.Normalize(targetDirection);

        Vector3 bend = Reject(poleModelPosition - geometry.RootPosition, targetDirection);
        if (bend.LengthSquared() <= PresentationTransformMath.DirectionEpsilon * PresentationTransformMath.DirectionEpsilon)
            bend = Reject(geometry.MiddlePosition - geometry.RootPosition, targetDirection);
        if (bend.LengthSquared() <= PresentationTransformMath.DirectionEpsilon * PresentationTransformMath.DirectionEpsilon)
            bend = PresentationTransformMath.CreateDeterministicPerpendicular(targetDirection);
        return Vector3.Normalize(bend);
    }

    private static Vector3 ResolveMiddlePosition(
        ChainGeometry geometry,
        Vector3 resolvedTarget,
        Vector3 bendDirection)
    {
        Vector3 targetOffset = resolvedTarget - geometry.RootPosition;
        float distance = targetOffset.Length();
        Vector3 targetDirection;
        if (distance <= PresentationTransformMath.DirectionEpsilon)
        {
            Vector3 currentDirection = geometry.EndPosition - geometry.RootPosition;
            if (currentDirection.LengthSquared() <=
                PresentationTransformMath.DirectionEpsilon * PresentationTransformMath.DirectionEpsilon)
            {
                currentDirection = geometry.MiddlePosition - geometry.RootPosition;
            }
            targetDirection = PresentationTransformMath.NormalizeDirection(
                currentDirection,
                nameof(resolvedTarget),
                "Current IK direction");
            return geometry.RootPosition + bendDirection * geometry.FirstLength;
        }

        targetDirection = targetOffset / distance;
        float along =
            (geometry.FirstLength * geometry.FirstLength -
             geometry.SecondLength * geometry.SecondLength +
             distance * distance) /
            (2.0f * distance);
        float perpendicularSquared = MathF.Max(
            0.0f,
            geometry.FirstLength * geometry.FirstLength - along * along);
        return geometry.RootPosition +
               targetDirection * along +
               bendDirection * MathF.Sqrt(perpendicularSquared);
    }

    private static SkeletonPose RotateJointToward(
        SkeletonPose pose,
        int jointIndex,
        Vector3 currentDirection,
        Vector3 desiredDirection)
    {
        Quaternion delta = PresentationTransformMath.CreateShortestArcRotation(
            currentDirection,
            desiredDirection,
            nameof(desiredDirection));
        SkeletonGlobalPose globalPose = SkeletonPoseEvaluator.EvaluateGlobal(pose);
        Quaternion currentModelRotation = PresentationTransformMath.ExtractNearRigidRotation(
            globalPose.GlobalTransforms[jointIndex],
            nameof(pose),
            $"Joint {jointIndex} model transform");
        Quaternion desiredModelRotation = PresentationTransformMath.AppendModelRotation(
            currentModelRotation,
            delta,
            nameof(desiredDirection));
        return SetJointModelRotation(pose, jointIndex, desiredModelRotation);
    }

    private static SkeletonPose SetJointModelRotation(
        SkeletonPose pose,
        int jointIndex,
        Quaternion desiredModelRotation)
    {
        SkeletonGlobalPose globalPose = SkeletonPoseEvaluator.EvaluateGlobal(pose);
        int parentIndex = pose.Skeleton.Joints[jointIndex].ParentIndex;
        Quaternion? parentRotation = parentIndex < 0
            ? null
            : PresentationTransformMath.ExtractNearRigidRotation(
                globalPose.GlobalTransforms[parentIndex],
                nameof(pose),
                $"Parent of joint {jointIndex} model transform");
        Quaternion localRotation = PresentationTransformMath.ConvertModelToLocalRotation(
            desiredModelRotation,
            parentRotation,
            nameof(desiredModelRotation));

        JointTransform current = pose.LocalTransforms[jointIndex];
        var transforms = pose.LocalTransforms.ToArray();
        transforms[jointIndex] = new JointTransform(current.Translation, localRotation, current.Scale);
        return new SkeletonPose(pose.Skeleton, transforms);
    }

    private static Vector3 Reject(Vector3 vector, Vector3 direction) =>
        vector - direction * Vector3.Dot(vector, direction);

    private static Vector3 GetPosition(SkeletonGlobalPose pose, int jointIndex) =>
        PresentationTransformMath.ExtractTranslation(pose.GlobalTransforms[jointIndex]);

    private readonly record struct ChainGeometry(
        Vector3 RootPosition,
        Vector3 MiddlePosition,
        Vector3 EndPosition,
        float FirstLength,
        float SecondLength);
}
