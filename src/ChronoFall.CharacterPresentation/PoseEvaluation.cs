using System.Numerics;

namespace ChronoFall.CharacterPresentation;

public sealed class SkeletonGlobalPose
{
    public SkeletonGlobalPose(
        SkeletonDefinition skeleton,
        IEnumerable<Matrix4x4> globalTransforms)
    {
        ArgumentNullException.ThrowIfNull(skeleton);
        ArgumentNullException.ThrowIfNull(globalTransforms);

        Matrix4x4[] copy = globalTransforms.ToArray();
        if (copy.Length != skeleton.JointCount)
        {
            throw new ArgumentException(
                $"Expected {skeleton.JointCount} global transforms, but received {copy.Length}.",
                nameof(globalTransforms));
        }

        for (int index = 0; index < copy.Length; index++)
            DataValidation.RequireFinite(copy[index], nameof(globalTransforms), $"Global transform {index}");

        Skeleton = skeleton;
        GlobalTransforms = Array.AsReadOnly(copy);
    }

    public SkeletonDefinition Skeleton { get; }

    public IReadOnlyList<Matrix4x4> GlobalTransforms { get; }
}

public static class SkeletonPoseEvaluator
{
    public static SkeletonGlobalPose EvaluateGlobal(SkeletonPose pose)
    {
        ArgumentNullException.ThrowIfNull(pose);

        var globalTransforms = new Matrix4x4[pose.Skeleton.JointCount];
        for (int index = 0; index < globalTransforms.Length; index++)
        {
            Matrix4x4 local = pose.LocalTransforms[index].ToMatrix();
            int parentIndex = pose.Skeleton.Joints[index].ParentIndex;
            globalTransforms[index] = parentIndex < 0
                ? local
                : local * globalTransforms[parentIndex];
        }

        return new SkeletonGlobalPose(pose.Skeleton, globalTransforms);
    }

    public static SkinningPalette CreateSkinningPalette(
        SkinDefinition skin,
        SkeletonGlobalPose globalPose)
    {
        ArgumentNullException.ThrowIfNull(skin);
        ArgumentNullException.ThrowIfNull(globalPose);
        if (!ReferenceEquals(skin.Skeleton, globalPose.Skeleton))
            throw new ArgumentException("The skin and global pose must use the same skeleton instance.", nameof(globalPose));

        var matrices = new Matrix4x4[skin.Skeleton.JointCount];
        for (int index = 0; index < matrices.Length; index++)
            matrices[index] = skin.InverseBindMatrices[index] * globalPose.GlobalTransforms[index];

        return new SkinningPalette(skin, matrices);
    }
}
