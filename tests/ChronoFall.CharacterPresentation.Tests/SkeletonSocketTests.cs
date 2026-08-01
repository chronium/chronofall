using System.Numerics;

namespace ChronoFall.CharacterPresentation.Tests;

public sealed class SkeletonSocketTests
{
    [Fact]
    public void SetCopiesDefinitionsAndUsesOrdinalNames()
    {
        SkeletonDefinition skeleton = CreateSkeleton();
        var source = new[]
        {
            new SkeletonSocketDefinition("Grip", 2, JointTransform.Identity),
            new SkeletonSocketDefinition("grip", 1, JointTransform.Identity),
        };

        var set = new SkeletonSocketSet(skeleton, source);
        source[0] = new SkeletonSocketDefinition("replacement", 0, JointTransform.Identity);

        Assert.Same(skeleton, set.Skeleton);
        Assert.Equal(2, set.SocketCount);
        Assert.Equal("Grip", set.Sockets[0].Name);
        Assert.True(set.TryGetSocketIndex("Grip", out int upperIndex));
        Assert.Equal(0, upperIndex);
        Assert.True(set.TryGetSocketIndex("grip", out int lowerIndex));
        Assert.Equal(1, lowerIndex);
        Assert.False(set.TryGetSocketIndex("missing", out _));
        Assert.Throws<ArgumentException>(() => set.TryGetSocketIndex("", out _));
    }

    [Fact]
    public void SetAllowsNoSockets()
    {
        var set = new SkeletonSocketSet(CreateSkeleton(), []);

        Assert.Empty(set.Sockets);
        Assert.Equal(0, set.SocketCount);
        Assert.False(set.TryGetSocketIndex("missing", out _));
    }

    [Fact]
    public void DefinitionsAndSetRejectInvalidData()
    {
        SkeletonDefinition skeleton = CreateSkeleton();

        Assert.Throws<ArgumentException>(() =>
            new SkeletonSocketDefinition("", 0, JointTransform.Identity));
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new SkeletonSocketDefinition("socket", -1, JointTransform.Identity));
        Assert.Throws<ArgumentException>(() =>
            new SkeletonSocketDefinition("socket", 0, default));
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new SkeletonSocketSet(
                skeleton,
                [new SkeletonSocketDefinition("socket", skeleton.JointCount, JointTransform.Identity)]));
        Assert.Throws<ArgumentException>(() =>
            new SkeletonSocketSet(
                skeleton,
                [
                    new SkeletonSocketDefinition("socket", 0, JointTransform.Identity),
                    new SkeletonSocketDefinition("socket", 1, JointTransform.Identity),
                ]));
        Assert.Throws<ArgumentException>(() =>
            new SkeletonSocketSet(skeleton, [null!]));
    }

    [Fact]
    public void EvaluatorComposesSocketLocalBeforePosedJointGlobal()
    {
        SkeletonDefinition skeleton = CreateSkeleton();
        var pose = new SkeletonPose(
            skeleton,
            [
                new JointTransform(new Vector3(2.0f, 0.0f, 0.0f), Quaternion.Identity, Vector3.One),
                new JointTransform(Vector3.UnitY, Quaternion.CreateFromAxisAngle(Vector3.UnitZ, MathF.PI / 2.0f), Vector3.One),
                new JointTransform(Vector3.UnitX, Quaternion.Identity, Vector3.One),
            ]);
        SkeletonGlobalPose globalPose = SkeletonPoseEvaluator.EvaluateGlobal(pose);
        var offset = new JointTransform(
            new Vector3(0.0f, 0.0f, 2.0f),
            Quaternion.CreateFromAxisAngle(Vector3.UnitY, MathF.PI / 4.0f),
            new Vector3(0.5f));
        var set = new SkeletonSocketSet(
            skeleton,
            [new SkeletonSocketDefinition("primary", 2, offset)]);

        SkeletonSocketPose sockets = SkeletonSocketEvaluator.EvaluateModelSpace(set, globalPose);
        Matrix4x4 expected = offset.ToMatrix() * globalPose.GlobalTransforms[2];

        Assert.Same(set, sockets.SocketSet);
        Assert.Single(sockets.ModelTransforms);
        Assert.Equal(expected, sockets.ModelTransforms[0]);
        Assert.True(sockets.TryGetModelTransform("primary", out Matrix4x4 resolved));
        Assert.Equal(expected, resolved);
        Assert.False(sockets.TryGetModelTransform("missing", out Matrix4x4 missing));
        Assert.Equal(default, missing);
    }

    [Fact]
    public void EvaluatorRequiresTheSameSkeletonInstance()
    {
        SkeletonDefinition skeleton = CreateSkeleton();
        SkeletonDefinition differentSkeleton = CreateSkeleton();
        var set = new SkeletonSocketSet(
            skeleton,
            [new SkeletonSocketDefinition("primary", 0, JointTransform.Identity)]);
        SkeletonGlobalPose globalPose = SkeletonPoseEvaluator.EvaluateGlobal(differentSkeleton.CreateBindPose());

        Assert.Throws<ArgumentException>(() =>
            SkeletonSocketEvaluator.EvaluateModelSpace(set, globalPose));
    }

    [Fact]
    public void SocketPoseCopiesAndValidatesMatrices()
    {
        var set = new SkeletonSocketSet(
            CreateSkeleton(),
            [new SkeletonSocketDefinition("primary", 0, JointTransform.Identity)]);
        Matrix4x4 expected = Matrix4x4.CreateTranslation(1.0f, 2.0f, 3.0f);
        Matrix4x4[] source = [expected];

        var pose = new SkeletonSocketPose(set, source);
        source[0] = Matrix4x4.Identity;

        Assert.Equal(expected, pose.ModelTransforms[0]);
        Assert.Throws<ArgumentException>(() => new SkeletonSocketPose(set, []));
        Assert.Throws<ArgumentException>(() =>
            new SkeletonSocketPose(
                set,
                [new Matrix4x4(float.NaN, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1)]));
    }

    private static SkeletonDefinition CreateSkeleton() => new(
        [
            new SkeletonJoint("root", -1, JointTransform.Identity),
            new SkeletonJoint("spine", 0, JointTransform.Identity),
            new SkeletonJoint("hand", 1, JointTransform.Identity),
        ]);
}
