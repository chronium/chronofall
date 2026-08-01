using System.Numerics;

namespace ChronoFall.CharacterPresentation.Tests;

public sealed class SkeletonPoseLayererTests
{
    [Fact]
    public void ApplyLayersOnlyIncludedLocalTransforms()
    {
        SkeletonDefinition skeleton = CreateSkeleton();
        var basePose = new SkeletonPose(skeleton, [
            JointTransform.Identity,
            new JointTransform(Vector3.UnitY, Quaternion.Identity, Vector3.One),
            new JointTransform(Vector3.UnitX, Quaternion.Identity, Vector3.One),
        ]);
        Quaternion layerRotation = Quaternion.CreateFromAxisAngle(Vector3.UnitZ, MathF.PI * 0.5f);
        var layerPose = new SkeletonPose(skeleton, [
            new JointTransform(Vector3.One, layerRotation, new Vector3(3.0f)),
            new JointTransform(new Vector3(5.0f), layerRotation, new Vector3(2.0f)),
            new JointTransform(new Vector3(9.0f), layerRotation, new Vector3(4.0f)),
        ]);
        var mask = new SkeletonJointMask(skeleton, [false, true, false]);

        SkeletonPose result = SkeletonPoseLayerer.Apply(basePose, layerPose, mask, 0.25f);

        Assert.Same(skeleton, result.Skeleton);
        Assert.Equal(basePose.LocalTransforms[0], result.LocalTransforms[0]);
        Assert.Equal(new Vector3(1.25f, 2.0f, 1.25f), result.LocalTransforms[1].Translation);
        Assert.Equal(new Vector3(1.25f), result.LocalTransforms[1].Scale);
        AssertRotationEquivalent(
            Quaternion.CreateFromAxisAngle(Vector3.UnitZ, MathF.PI * 0.125f),
            result.LocalTransforms[1].Rotation);
        Assert.Equal(basePose.LocalTransforms[2], result.LocalTransforms[2]);
    }

    [Fact]
    public void ApplyPreservesExactEndpointsInIndependentPoses()
    {
        SkeletonDefinition skeleton = CreateSkeleton();
        SkeletonPose basePose = skeleton.CreateBindPose();
        var layerPose = new SkeletonPose(skeleton, [
            new JointTransform(Vector3.UnitX, Quaternion.Identity, Vector3.One),
            new JointTransform(Vector3.UnitY, Quaternion.Identity, Vector3.One),
            new JointTransform(Vector3.UnitZ, Quaternion.Identity, Vector3.One),
        ]);
        var mask = new SkeletonJointMask(skeleton, [false, true, true]);

        SkeletonPose start = SkeletonPoseLayerer.Apply(basePose, layerPose, mask, 0.0f);
        SkeletonPose end = SkeletonPoseLayerer.Apply(basePose, layerPose, mask, 1.0f);

        Assert.NotSame(basePose, start);
        Assert.NotSame(layerPose, end);
        Assert.Equal(basePose.LocalTransforms, start.LocalTransforms);
        Assert.Equal(basePose.LocalTransforms[0], end.LocalTransforms[0]);
        Assert.Equal(layerPose.LocalTransforms[1], end.LocalTransforms[1]);
        Assert.Equal(layerPose.LocalTransforms[2], end.LocalTransforms[2]);
    }

    [Fact]
    public void ApplyUsesShortestPathQuaternionInterpolation()
    {
        SkeletonDefinition skeleton = CreateSkeleton();
        Quaternion rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitY, 0.75f);
        var basePose = new SkeletonPose(skeleton, [
            new JointTransform(Vector3.Zero, rotation, Vector3.One),
            JointTransform.Identity,
            JointTransform.Identity,
        ]);
        var layerPose = new SkeletonPose(skeleton, [
            new JointTransform(
                Vector3.Zero,
                new Quaternion(-rotation.X, -rotation.Y, -rotation.Z, -rotation.W),
                Vector3.One),
            JointTransform.Identity,
            JointTransform.Identity,
        ]);
        var mask = new SkeletonJointMask(skeleton, [true, false, false]);

        Quaternion result = SkeletonPoseLayerer.Apply(basePose, layerPose, mask, 0.5f)
            .LocalTransforms[0]
            .Rotation;

        AssertRotationEquivalent(rotation, result);
        Assert.InRange(result.Length(), 0.99999f, 1.00001f);
    }

    [Theory]
    [InlineData(float.NaN)]
    [InlineData(float.PositiveInfinity)]
    [InlineData(-0.01f)]
    [InlineData(1.01f)]
    public void ApplyRejectsInvalidAmounts(float amount)
    {
        SkeletonDefinition skeleton = CreateSkeleton();
        SkeletonPose pose = skeleton.CreateBindPose();
        SkeletonJointMask mask = SkeletonJointMask.CreateSubtree(skeleton, 0);

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            SkeletonPoseLayerer.Apply(pose, pose, mask, amount));
    }

    [Fact]
    public void ApplyRejectsDifferentPoseSkeletons()
    {
        SkeletonDefinition skeleton = CreateSkeleton();
        SkeletonPose basePose = skeleton.CreateBindPose();
        SkeletonPose layerPose = CreateSkeleton().CreateBindPose();
        SkeletonJointMask mask = SkeletonJointMask.CreateSubtree(skeleton, 0);

        Assert.Throws<ArgumentException>(() =>
            SkeletonPoseLayerer.Apply(basePose, layerPose, mask, 0.5f));
    }

    [Fact]
    public void ApplyRejectsDifferentMaskSkeleton()
    {
        SkeletonDefinition skeleton = CreateSkeleton();
        SkeletonPose pose = skeleton.CreateBindPose();
        SkeletonJointMask mask = SkeletonJointMask.CreateSubtree(CreateSkeleton(), 0);

        Assert.Throws<ArgumentException>(() =>
            SkeletonPoseLayerer.Apply(pose, pose, mask, 0.5f));
    }

    private static SkeletonDefinition CreateSkeleton() => new([
        new SkeletonJoint("root", -1, JointTransform.Identity),
        new SkeletonJoint("spine", 0, JointTransform.Identity),
        new SkeletonJoint("hand", 1, JointTransform.Identity),
    ]);

    private static void AssertRotationEquivalent(Quaternion expected, Quaternion actual)
    {
        float dot = MathF.Abs(Quaternion.Dot(expected, actual));
        Assert.InRange(dot, 0.99999f, 1.00001f);
    }
}
