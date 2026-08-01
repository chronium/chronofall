namespace ChronoFall.CharacterPresentation;

public static class SkeletonPoseBlender
{
    public static SkeletonPose Blend(
        SkeletonPose source,
        SkeletonPose destination,
        float amount)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(destination);
        if (!float.IsFinite(amount) || amount < 0.0f || amount > 1.0f)
            throw new ArgumentOutOfRangeException(nameof(amount), "Blend amount must be finite and between zero and one.");
        if (!ReferenceEquals(source.Skeleton, destination.Skeleton))
            throw new ArgumentException("Source and destination poses must use the same skeleton instance.", nameof(destination));

        var transforms = new JointTransform[source.Skeleton.JointCount];
        for (int index = 0; index < transforms.Length; index++)
        {
            transforms[index] = AnimationInterpolationMath.InterpolateTransform(
                source.LocalTransforms[index],
                destination.LocalTransforms[index],
                amount,
                nameof(destination));
        }

        return new SkeletonPose(source.Skeleton, transforms);
    }
}
