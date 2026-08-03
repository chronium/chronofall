namespace ChronoFall.CharacterExperiment.SdlGpu;

internal static class ExactSkeletonAnimationRebinder
{
    internal static AnimationClip Rebind(
        SkinDefinition sourceSkin,
        AnimationClip sourceClip,
        SkinDefinition targetSkin)
    {
        ArgumentNullException.ThrowIfNull(sourceSkin);
        ArgumentNullException.ThrowIfNull(sourceClip);
        ArgumentNullException.ThrowIfNull(targetSkin);
        if (!ReferenceEquals(sourceClip.Skeleton, sourceSkin.Skeleton))
            throw new ArgumentException("The source clip must use the source skin skeleton.", nameof(sourceClip));

        SkeletonDefinition source = sourceSkin.Skeleton;
        SkeletonDefinition target = targetSkin.Skeleton;
        if (source.JointCount != target.JointCount)
        {
            throw new ArgumentException(
                $"Exact skeleton rebind requires the same joint count, but source has {source.JointCount} and target has {target.JointCount}.",
                nameof(targetSkin));
        }

        for (int index = 0; index < source.JointCount; index++)
        {
            SkeletonJoint sourceJoint = source.Joints[index];
            SkeletonJoint targetJoint = target.Joints[index];
            if (!string.Equals(sourceJoint.Name, targetJoint.Name, StringComparison.Ordinal) ||
                sourceJoint.ParentIndex != targetJoint.ParentIndex ||
                sourceJoint.LocalBindTransform != targetJoint.LocalBindTransform ||
                sourceSkin.InverseBindMatrices[index] != targetSkin.InverseBindMatrices[index])
            {
                throw new ArgumentException(
                    $"Exact skeleton rebind mismatch at joint {index} ('{sourceJoint.Name}'/'{targetJoint.Name}').",
                    nameof(targetSkin));
            }
        }

        return new AnimationClip(sourceClip.Name, target, sourceClip.Tracks);
    }
}
