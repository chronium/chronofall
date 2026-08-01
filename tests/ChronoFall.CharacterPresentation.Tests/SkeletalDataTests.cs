using System.Numerics;

namespace ChronoFall.CharacterPresentation.Tests;

public sealed class SkeletalDataTests
{
    [Fact]
    public void SkeletonRequiresOneParentFirstRootAndUniqueNames()
    {
        SkeletonDefinition skeleton = CreateSkeleton();

        Assert.Equal(2, skeleton.JointCount);
        Assert.Equal(-1, skeleton.Joints[0].ParentIndex);
        Assert.Equal(0, skeleton.Joints[1].ParentIndex);
        Assert.True(skeleton.TryGetJointIndex("child", out int childIndex));
        Assert.Equal(1, childIndex);

        Assert.Throws<ArgumentException>(() => new SkeletonDefinition([
            Joint("root", -1),
            Joint("child", -1),
        ]));
        Assert.Throws<ArgumentException>(() => new SkeletonDefinition([
            Joint("root", -1),
            Joint("child", 1),
        ]));
        Assert.Throws<ArgumentException>(() => new SkeletonDefinition([
            Joint("root", -1),
            Joint("root", 0),
        ]));
    }

    [Fact]
    public void SkeletonAndPoseDefensivelyCopyInputs()
    {
        SkeletonJoint[] joints = [Joint("root", -1), Joint("child", 0)];
        var skeleton = new SkeletonDefinition(joints);
        joints[1] = Joint("changed", 0);

        JointTransform[] transforms = [JointTransform.Identity, JointTransform.Identity];
        var pose = new SkeletonPose(skeleton, transforms);
        transforms[1] = new JointTransform(Vector3.One, Quaternion.Identity, Vector3.One);

        Assert.Equal("child", skeleton.Joints[1].Name);
        Assert.Equal(Vector3.Zero, pose.LocalTransforms[1].Translation);
        Assert.Equal(2, skeleton.CreateBindPose().LocalTransforms.Count);
    }

    [Fact]
    public void SkinPoseAndPaletteRequireOneFiniteEntryPerJoint()
    {
        SkeletonDefinition skeleton = CreateSkeleton();
        var skin = new SkinDefinition(skeleton, [Matrix4x4.Identity, Matrix4x4.Identity]);
        var palette = new SkinningPalette(skin, [Matrix4x4.Identity, Matrix4x4.Identity]);

        Assert.Equal(2, skin.InverseBindMatrices.Count);
        Assert.Equal(2, palette.JointMatrices.Count);
        Assert.Throws<ArgumentException>(() => new SkinDefinition(skeleton, [Matrix4x4.Identity]));
        Assert.Throws<ArgumentException>(() => new SkeletonPose(skeleton, [JointTransform.Identity]));
        Assert.Throws<ArgumentException>(() => new SkinningPalette(skin, [Matrix4x4.Identity]));

        Matrix4x4 invalid = Matrix4x4.Identity;
        invalid.M22 = float.NaN;
        Assert.Throws<ArgumentException>(() => new SkinDefinition(skeleton, [Matrix4x4.Identity, invalid]));
        Assert.Throws<ArgumentException>(() => new SkinningPalette(skin, [Matrix4x4.Identity, invalid]));
    }

    [Fact]
    public void FourLaneInfluencesAllowExporterNoiseAndValidateJointRange()
    {
        SkeletonDefinition skeleton = CreateSkeleton();
        var influences = new SkinInfluences(
            new JointIndices4(0, 1, 0, 0),
            new Vector4(0.50000006f, 0.49999997f, 0.0f, 0.0f));

        influences.ValidateForSkeleton(skeleton);

        Assert.Throws<ArgumentException>(() => new SkinInfluences(
            new JointIndices4(0, 1, 0, 0),
            new Vector4(0.4f, 0.4f, 0.0f, 0.0f)));
        Assert.Throws<ArgumentException>(() => new SkinInfluences(
            new JointIndices4(0, 1, 0, 0),
            new Vector4(1.1f, -0.1f, 0.0f, 0.0f)));

        var outOfRange = new SkinInfluences(new JointIndices4(0, 2, 0, 0), Vector4.UnitX);
        Assert.Throws<ArgumentOutOfRangeException>(() => outOfRange.ValidateForSkeleton(skeleton));
        Assert.Throws<ArgumentException>(() => default(SkinInfluences).ValidateForSkeleton(skeleton));
    }

    private static SkeletonDefinition CreateSkeleton() =>
        new([Joint("root", -1), Joint("child", 0)]);

    private static SkeletonJoint Joint(string name, int parentIndex) =>
        new(name, parentIndex, JointTransform.Identity);
}
