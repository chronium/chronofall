namespace ChronoFall.CharacterPresentation;

public static class SkeletonPoseLayerer
{
    public static SkeletonPose Apply(
        SkeletonPose basePose,
        SkeletonPose layerPose,
        SkeletonJointMask mask,
        float amount)
    {
        ArgumentNullException.ThrowIfNull(basePose);
        ArgumentNullException.ThrowIfNull(layerPose);
        ArgumentNullException.ThrowIfNull(mask);
        if (!float.IsFinite(amount) || amount < 0.0f || amount > 1.0f)
            throw new ArgumentOutOfRangeException(nameof(amount), "Layer amount must be finite and between zero and one.");
        if (!ReferenceEquals(basePose.Skeleton, layerPose.Skeleton))
            throw new ArgumentException("Base and layer poses must use the same skeleton instance.", nameof(layerPose));
        if (!ReferenceEquals(basePose.Skeleton, mask.Skeleton))
            throw new ArgumentException("The joint mask must use the same skeleton instance as the poses.", nameof(mask));

        var transforms = new JointTransform[basePose.Skeleton.JointCount];
        for (int index = 0; index < transforms.Length; index++)
        {
            transforms[index] = mask[index]
                ? AnimationInterpolationMath.InterpolateTransform(
                    basePose.LocalTransforms[index],
                    layerPose.LocalTransforms[index],
                    amount,
                    nameof(layerPose))
                : basePose.LocalTransforms[index];
        }

        return new SkeletonPose(basePose.Skeleton, transforms);
    }
}
