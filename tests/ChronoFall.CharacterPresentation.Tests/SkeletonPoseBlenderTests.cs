using System.Numerics;

namespace ChronoFall.CharacterPresentation.Tests;

public sealed class SkeletonPoseBlenderTests
{
    [Fact]
    public void BlendInterpolatesLocalTrsAndPreservesSkeletonIdentity()
    {
        SkeletonDefinition skeleton = CreateSkeleton();
        var source = new SkeletonPose(skeleton, [
            new JointTransform(Vector3.Zero, Quaternion.Identity, Vector3.One),
            new JointTransform(Vector3.UnitY, Quaternion.Identity, Vector3.One),
        ]);
        Quaternion destinationRotation = Quaternion.CreateFromAxisAngle(Vector3.UnitY, MathF.PI * 0.5f);
        var destination = new SkeletonPose(skeleton, [
            new JointTransform(new Vector3(2.0f, 4.0f, 6.0f), destinationRotation, new Vector3(3.0f)),
            new JointTransform(new Vector3(4.0f, 3.0f, 2.0f), Quaternion.Identity, new Vector3(2.0f)),
        ]);

        SkeletonPose result = SkeletonPoseBlender.Blend(source, destination, 0.25f);

        Assert.Same(skeleton, result.Skeleton);
        Assert.Equal(new Vector3(0.5f, 1.0f, 1.5f), result.LocalTransforms[0].Translation);
        Assert.Equal(new Vector3(1.5f), result.LocalTransforms[0].Scale);
        AssertRotationEquivalent(
            Quaternion.CreateFromAxisAngle(Vector3.UnitY, MathF.PI * 0.125f),
            result.LocalTransforms[0].Rotation);
        Assert.Equal(new Vector3(1.0f, 1.5f, 0.5f), result.LocalTransforms[1].Translation);
    }

    [Fact]
    public void EndpointsReturnEquivalentIndependentPoses()
    {
        SkeletonDefinition skeleton = CreateSkeleton();
        SkeletonPose source = skeleton.CreateBindPose();
        var destination = new SkeletonPose(skeleton, [
            new JointTransform(Vector3.UnitX, Quaternion.Identity, Vector3.One),
            new JointTransform(Vector3.UnitY * 2.0f, Quaternion.Identity, Vector3.One),
        ]);

        SkeletonPose start = SkeletonPoseBlender.Blend(source, destination, 0.0f);
        SkeletonPose end = SkeletonPoseBlender.Blend(source, destination, 1.0f);

        Assert.NotSame(source, start);
        Assert.NotSame(destination, end);
        Assert.Equal(source.LocalTransforms, start.LocalTransforms);
        Assert.Equal(destination.LocalTransforms, end.LocalTransforms);
    }

    [Fact]
    public void BlendUsesNormalizedShortestPathQuaternionInterpolation()
    {
        SkeletonDefinition skeleton = new([
            new SkeletonJoint("root", -1, JointTransform.Identity),
        ]);
        Quaternion rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitZ, 0.75f);
        var source = new SkeletonPose(skeleton, [
            new JointTransform(Vector3.Zero, rotation, Vector3.One),
        ]);
        var destination = new SkeletonPose(skeleton, [
            new JointTransform(
                Vector3.Zero,
                new Quaternion(-rotation.X, -rotation.Y, -rotation.Z, -rotation.W),
                Vector3.One),
        ]);

        Quaternion result = SkeletonPoseBlender.Blend(source, destination, 0.5f).LocalTransforms[0].Rotation;

        AssertRotationEquivalent(rotation, result);
        Assert.InRange(result.Length(), 0.99999f, 1.00001f);
    }

    [Theory]
    [InlineData(float.NaN)]
    [InlineData(float.PositiveInfinity)]
    [InlineData(-0.01f)]
    [InlineData(1.01f)]
    public void BlendRejectsInvalidAmounts(float amount)
    {
        SkeletonDefinition skeleton = CreateSkeleton();
        SkeletonPose pose = skeleton.CreateBindPose();

        Assert.Throws<ArgumentOutOfRangeException>(() => SkeletonPoseBlender.Blend(pose, pose, amount));
    }

    [Fact]
    public void BlendRejectsDifferentSkeletonInstances()
    {
        SkeletonPose source = CreateSkeleton().CreateBindPose();
        SkeletonPose destination = CreateSkeleton().CreateBindPose();

        ArgumentException exception = Assert.Throws<ArgumentException>(() =>
            SkeletonPoseBlender.Blend(source, destination, 0.5f));

        Assert.Contains("same skeleton instance", exception.Message, StringComparison.Ordinal);
    }

    private static SkeletonDefinition CreateSkeleton() => new([
        new SkeletonJoint("root", -1, JointTransform.Identity),
        new SkeletonJoint("child", 0, new JointTransform(Vector3.UnitY, Quaternion.Identity, Vector3.One)),
    ]);

    private static void AssertRotationEquivalent(Quaternion expected, Quaternion actual)
    {
        float dot = MathF.Abs(Quaternion.Dot(expected, actual));
        Assert.InRange(dot, 0.99999f, 1.00001f);
    }
}
