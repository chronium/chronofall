using System.Numerics;

namespace ChronoFall.CharacterExperiment.SimpleMesh.Tests;

public sealed class SelectedSocketTests
{
    private const float MatrixTolerance = 1e-5f;

    [Fact]
    public void SelectedRigResolvesSemanticSocketsFromAnimatedPose()
    {
        SkeletalCharacterAsset asset = LoadAsset();
        SkeletonDefinition skeleton = asset.Mesh.Skin.Skeleton;
        Assert.True(skeleton.TryGetJointIndex("hand_r", out int handIndex));
        Assert.True(skeleton.TryGetJointIndex("spine_03", out int spineIndex));

        var backOffset = new JointTransform(
            new Vector3(0.0f, 0.12f, -0.08f),
            Quaternion.CreateFromAxisAngle(Vector3.UnitY, 0.25f),
            Vector3.One);
        var socketSet = new SkeletonSocketSet(
            skeleton,
            [
                new SkeletonSocketDefinition("primary-hand", handIndex, JointTransform.Identity),
                new SkeletonSocketDefinition("back", spineIndex, backOffset),
            ]);
        AnimationClip attack = Assert.Single(asset.Animations, clip => clip.Name == "Sword_Attack");

        SkeletonGlobalPose startGlobal = EvaluateGlobal(attack, 0.0f);
        SkeletonGlobalPose actionGlobal = EvaluateGlobal(attack, 0.75f);
        SkeletonSocketPose startSockets = SkeletonSocketEvaluator.EvaluateModelSpace(socketSet, startGlobal);
        SkeletonSocketPose actionSockets = SkeletonSocketEvaluator.EvaluateModelSpace(socketSet, actionGlobal);

        Assert.True(actionSockets.TryGetModelTransform("primary-hand", out Matrix4x4 handSocket));
        Assert.True(actionSockets.TryGetModelTransform("back", out Matrix4x4 backSocket));
        AssertMatrix(actionGlobal.GlobalTransforms[handIndex], handSocket);
        AssertMatrix(backOffset.ToMatrix() * actionGlobal.GlobalTransforms[spineIndex], backSocket);
        Assert.All(actionSockets.ModelTransforms, AssertFinite);

        Vector3 startHand = GetTranslation(startSockets.ModelTransforms[0]);
        Vector3 actionHand = GetTranslation(handSocket);
        Assert.True(
            Vector3.Distance(startHand, actionHand) > 0.01f,
            $"Expected the animated primary-hand socket to move, but the distance was {Vector3.Distance(startHand, actionHand)}.");
    }

    private static SkeletonGlobalPose EvaluateGlobal(AnimationClip clip, float sampleTime) =>
        SkeletonPoseEvaluator.EvaluateGlobal(
            AnimationSampler.Sample(clip, sampleTime, AnimationPlaybackMode.Clamp));

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

    private static void AssertMatrix(Matrix4x4 expected, Matrix4x4 actual)
    {
        AssertVector(new Vector4(expected.M11, expected.M12, expected.M13, expected.M14), new Vector4(actual.M11, actual.M12, actual.M13, actual.M14));
        AssertVector(new Vector4(expected.M21, expected.M22, expected.M23, expected.M24), new Vector4(actual.M21, actual.M22, actual.M23, actual.M24));
        AssertVector(new Vector4(expected.M31, expected.M32, expected.M33, expected.M34), new Vector4(actual.M31, actual.M32, actual.M33, actual.M34));
        AssertVector(new Vector4(expected.M41, expected.M42, expected.M43, expected.M44), new Vector4(actual.M41, actual.M42, actual.M43, actual.M44));
    }

    private static void AssertVector(Vector4 expected, Vector4 actual)
    {
        Assert.InRange(actual.X, expected.X - MatrixTolerance, expected.X + MatrixTolerance);
        Assert.InRange(actual.Y, expected.Y - MatrixTolerance, expected.Y + MatrixTolerance);
        Assert.InRange(actual.Z, expected.Z - MatrixTolerance, expected.Z + MatrixTolerance);
        Assert.InRange(actual.W, expected.W - MatrixTolerance, expected.W + MatrixTolerance);
    }

    private static void AssertFinite(Matrix4x4 matrix)
    {
        float[] values =
        [
            matrix.M11, matrix.M12, matrix.M13, matrix.M14,
            matrix.M21, matrix.M22, matrix.M23, matrix.M24,
            matrix.M31, matrix.M32, matrix.M33, matrix.M34,
            matrix.M41, matrix.M42, matrix.M43, matrix.M44,
        ];
        Assert.All(values, value => Assert.True(float.IsFinite(value)));
    }
}
