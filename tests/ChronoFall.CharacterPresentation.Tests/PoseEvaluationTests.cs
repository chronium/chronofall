using System.Numerics;

namespace ChronoFall.CharacterPresentation.Tests;

public sealed class PoseEvaluationTests
{
    [Fact]
    public void EvaluateGlobalUsesParentFirstRowVectorComposition()
    {
        var skeleton = new SkeletonDefinition([
            new SkeletonJoint("root", -1, JointTransform.Identity),
            new SkeletonJoint("child", 0, JointTransform.Identity),
            new SkeletonJoint("grandchild", 1, JointTransform.Identity),
        ]);
        var pose = new SkeletonPose(skeleton, [
            new JointTransform(
                new Vector3(10.0f, 0.0f, 0.0f),
                Quaternion.CreateFromAxisAngle(Vector3.UnitZ, MathF.PI * 0.5f),
                Vector3.One),
            new JointTransform(new Vector3(2.0f, 0.0f, 0.0f), Quaternion.Identity, Vector3.One),
            new JointTransform(new Vector3(0.0f, 3.0f, 0.0f), Quaternion.Identity, Vector3.One),
        ]);

        SkeletonGlobalPose global = SkeletonPoseEvaluator.EvaluateGlobal(pose);

        Assert.Same(skeleton, global.Skeleton);
        AssertVector(new Vector3(10.0f, 2.0f, 0.0f), Vector3.Transform(Vector3.Zero, global.GlobalTransforms[1]));
        AssertVector(new Vector3(7.0f, 2.0f, 0.0f), Vector3.Transform(Vector3.Zero, global.GlobalTransforms[2]));
    }

    [Fact]
    public void BindPoseAndInverseBindsProduceIdentityPalette()
    {
        var skeleton = new SkeletonDefinition([
            new SkeletonJoint(
                "root",
                -1,
                new JointTransform(new Vector3(2.0f, 0.0f, 0.0f), Quaternion.Identity, Vector3.One)),
            new SkeletonJoint(
                "child",
                0,
                new JointTransform(new Vector3(0.0f, 3.0f, 0.0f), Quaternion.Identity, Vector3.One)),
        ]);
        SkeletonGlobalPose bindGlobal = SkeletonPoseEvaluator.EvaluateGlobal(skeleton.CreateBindPose());
        var skin = new SkinDefinition(skeleton, bindGlobal.GlobalTransforms.Select(Invert));

        SkinningPalette palette = SkeletonPoseEvaluator.CreateSkinningPalette(skin, bindGlobal);

        Assert.All(palette.JointMatrices, matrix => AssertMatrix(Matrix4x4.Identity, matrix));
    }

    [Fact]
    public void GlobalPoseDefensivelyCopiesAndPaletteRejectsDifferentSkeletonInstance()
    {
        var skeleton = new SkeletonDefinition([new SkeletonJoint("root", -1, JointTransform.Identity)]);
        Matrix4x4[] transforms = [Matrix4x4.Identity];
        var global = new SkeletonGlobalPose(skeleton, transforms);
        transforms[0] = Matrix4x4.CreateTranslation(Vector3.One);

        var differentSkeleton = new SkeletonDefinition([new SkeletonJoint("root", -1, JointTransform.Identity)]);
        var skin = new SkinDefinition(differentSkeleton, [Matrix4x4.Identity]);
        Matrix4x4 invalid = Matrix4x4.Identity;
        invalid.M11 = float.NaN;

        AssertMatrix(Matrix4x4.Identity, global.GlobalTransforms[0]);
        Assert.Throws<ArgumentException>(() => SkeletonPoseEvaluator.CreateSkinningPalette(skin, global));
        Assert.Throws<ArgumentException>(() => new SkeletonGlobalPose(skeleton, []));
        Assert.Throws<ArgumentException>(() => new SkeletonGlobalPose(skeleton, [invalid]));
    }

    private static Matrix4x4 Invert(Matrix4x4 matrix)
    {
        Assert.True(Matrix4x4.Invert(matrix, out Matrix4x4 inverse));
        return inverse;
    }

    private static void AssertMatrix(Matrix4x4 expected, Matrix4x4 actual)
    {
        AssertVector(new Vector4(expected.M11, expected.M12, expected.M13, expected.M14), new Vector4(actual.M11, actual.M12, actual.M13, actual.M14));
        AssertVector(new Vector4(expected.M21, expected.M22, expected.M23, expected.M24), new Vector4(actual.M21, actual.M22, actual.M23, actual.M24));
        AssertVector(new Vector4(expected.M31, expected.M32, expected.M33, expected.M34), new Vector4(actual.M31, actual.M32, actual.M33, actual.M34));
        AssertVector(new Vector4(expected.M41, expected.M42, expected.M43, expected.M44), new Vector4(actual.M41, actual.M42, actual.M43, actual.M44));
    }

    private static void AssertVector(Vector3 expected, Vector3 actual)
    {
        Assert.Equal(expected.X, actual.X, 5);
        Assert.Equal(expected.Y, actual.Y, 5);
        Assert.Equal(expected.Z, actual.Z, 5);
    }

    private static void AssertVector(Vector4 expected, Vector4 actual)
    {
        Assert.Equal(expected.X, actual.X, 5);
        Assert.Equal(expected.Y, actual.Y, 5);
        Assert.Equal(expected.Z, actual.Z, 5);
        Assert.Equal(expected.W, actual.W, 5);
    }
}
