using System.Numerics;

namespace ChronoFall.CharacterExperiment.SimpleMesh.Tests;

public sealed class SelectedSamplingTests
{
    private const float MatrixTolerance = 1e-4f;
    private const float FixtureTolerance = 1e-5f;

    [Fact]
    public void SelectedBindPoseProducesIdentitySkinningPalette()
    {
        SkeletalCharacterAsset asset = LoadAsset();

        SkeletonGlobalPose globalPose = SkeletonPoseEvaluator.EvaluateGlobal(
            asset.Mesh.Skin.Skeleton.CreateBindPose());
        SkinningPalette palette = SkeletonPoseEvaluator.CreateSkinningPalette(asset.Mesh.Skin, globalPose);

        Assert.Equal(65, palette.JointMatrices.Count);
        Assert.All(palette.JointMatrices, matrix => AssertMatrix(Matrix4x4.Identity, matrix, MatrixTolerance));
    }

    [Fact]
    public void SelectedClipsProduceDeterministicTimestampFixtures()
    {
        SkeletalCharacterAsset asset = LoadAsset();

        AssertFixture(
            asset,
            "Idle_Loop",
            1.25f,
            new JointTransform(
                new Vector3(-0.0024382225f, 0.08654535f, 0.8771455f),
                new Quaternion(0.7561785f, -0.088324025f, -0.07522113f, 0.64399904f),
                new Vector3(1.0f, 0.99999994f, 1.0f)),
            new Vector3(0.34055054f, 0.89902925f, 0.010652959f));
        AssertFixture(
            asset,
            "Walk_Loop",
            0.5f,
            new JointTransform(
                new Vector3(0.039067686f, 0.050006263f, 0.9064422f),
                new Quaternion(0.7902818f, -0.015413179f, 0.007286191f, 0.6125064f),
                new Vector3(1.0f, 0.99999994f, 1.0f)),
            new Vector3(0.34742188f, 0.9731468f, 0.14868025f));
        AssertFixture(
            asset,
            "Sword_Attack",
            0.75f,
            new JointTransform(
                new Vector3(-0.039926544f, 0.07067248f, 0.54745847f),
                new Quaternion(0.728423f, 0.31676242f, 0.20947431f, 0.57024735f),
                new Vector3(1.0f, 1.0f, 0.99999994f)),
            new Vector3(-0.05730962f, 1.2709479f, -0.5285032f));
    }

    private static void AssertFixture(
        SkeletalCharacterAsset asset,
        string clipName,
        float sampleTime,
        JointTransform expectedPelvis,
        Vector3 expectedHandGlobalTranslation)
    {
        AnimationClip clip = Assert.Single(asset.Animations, candidate => candidate.Name == clipName);
        Assert.Equal("root", clip.Skeleton.Joints[0].Name);
        Assert.Equal("pelvis", clip.Skeleton.Joints[1].Name);
        Assert.Equal("hand_l", clip.Skeleton.Joints[10].Name);

        SkeletonPose pose = AnimationSampler.Sample(clip, sampleTime, AnimationPlaybackMode.Clamp);
        SkeletonGlobalPose global = SkeletonPoseEvaluator.EvaluateGlobal(pose);
        AssertTransform(
            new JointTransform(
                Vector3.Zero,
                new Quaternion(-0.7071068f, 0.0f, 0.0f, 0.7071068f),
                Vector3.One),
            pose.LocalTransforms[0]);
        AssertTransform(expectedPelvis, pose.LocalTransforms[1]);
        AssertVector(
            expectedHandGlobalTranslation,
            GetTranslation(global.GlobalTransforms[10]),
            FixtureTolerance);

        SkeletonPose first = AnimationSampler.Sample(clip, 0.0f, AnimationPlaybackMode.Loop);
        SkeletonPose boundary = AnimationSampler.Sample(clip, clip.Duration, AnimationPlaybackMode.Loop);
        for (int index = 0; index < clip.Skeleton.JointCount; index++)
            AssertTransform(first.LocalTransforms[index], boundary.LocalTransforms[index]);
    }

    private static SkeletalCharacterAsset LoadAsset()
    {
        string path = Path.Combine(
            RepositoryPaths.Root,
            "assets",
            "Quaternius",
            "Universal Animation Library[Standard]",
            "Unreal-Godot",
            "UAL1_Standard.glb");
        return SimpleMeshSkeletalAssetLoader.LoadFromFile(path);
    }

    private static Vector3 GetTranslation(Matrix4x4 matrix) =>
        new(matrix.M41, matrix.M42, matrix.M43);

    private static void AssertTransform(JointTransform expected, JointTransform actual)
    {
        AssertVector(expected.Translation, actual.Translation, FixtureTolerance);
        AssertVector(expected.Scale, actual.Scale, FixtureTolerance);
        float rotationDot = MathF.Abs(Quaternion.Dot(expected.Rotation, actual.Rotation));
        Assert.InRange(rotationDot, 1.0f - FixtureTolerance, 1.0f + FixtureTolerance);
    }

    private static void AssertMatrix(Matrix4x4 expected, Matrix4x4 actual, float tolerance)
    {
        AssertVector(new Vector4(expected.M11, expected.M12, expected.M13, expected.M14), new Vector4(actual.M11, actual.M12, actual.M13, actual.M14), tolerance);
        AssertVector(new Vector4(expected.M21, expected.M22, expected.M23, expected.M24), new Vector4(actual.M21, actual.M22, actual.M23, actual.M24), tolerance);
        AssertVector(new Vector4(expected.M31, expected.M32, expected.M33, expected.M34), new Vector4(actual.M31, actual.M32, actual.M33, actual.M34), tolerance);
        AssertVector(new Vector4(expected.M41, expected.M42, expected.M43, expected.M44), new Vector4(actual.M41, actual.M42, actual.M43, actual.M44), tolerance);
    }

    private static void AssertVector(Vector3 expected, Vector3 actual, float tolerance)
    {
        Assert.InRange(actual.X, expected.X - tolerance, expected.X + tolerance);
        Assert.InRange(actual.Y, expected.Y - tolerance, expected.Y + tolerance);
        Assert.InRange(actual.Z, expected.Z - tolerance, expected.Z + tolerance);
    }

    private static void AssertVector(Vector4 expected, Vector4 actual, float tolerance)
    {
        Assert.InRange(actual.X, expected.X - tolerance, expected.X + tolerance);
        Assert.InRange(actual.Y, expected.Y - tolerance, expected.Y + tolerance);
        Assert.InRange(actual.Z, expected.Z - tolerance, expected.Z + tolerance);
        Assert.InRange(actual.W, expected.W - tolerance, expected.W + tolerance);
    }
}
