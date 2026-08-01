using System.Numerics;
using ChronoFall.CharacterExperiment.SimpleMesh;

namespace ChronoFall.CharacterExperiment.SdlGpu.Tests;

public sealed class CharacterHarnessDataTests
{
    [Fact]
    public void SelectedWalkSampleProducesFiniteDistinctPalette()
    {
        SkeletalCharacterAsset asset = LoadSelectedAsset();
        AnimationClip animation = Assert.Single(
            asset.Animations,
            candidate => string.Equals(candidate.Name, "Walk_Loop", StringComparison.Ordinal));

        SkinningPalette bind = CreatePalette(asset.Mesh.Skin, asset.Mesh.Skin.Skeleton.CreateBindPose());
        SkinningPalette sample = CreatePalette(
            asset.Mesh.Skin,
            AnimationSampler.Sample(animation, 0.5f, AnimationPlaybackMode.Loop));

        Assert.Equal(65, sample.JointMatrices.Count);
        Assert.All(sample.JointMatrices, static matrix => AssertFinite(matrix));
        Assert.False(bind.JointMatrices.SequenceEqual(sample.JointMatrices));
    }

    [Fact]
    public void EveryBrowserClipProducesFinitePoseAndDebugGeometry()
    {
        SkeletalCharacterAsset asset = LoadSelectedAsset();
        var controller = new CharacterPlaybackController(asset.Animations, "Walk_Loop");

        Assert.Equal(43, controller.Clips.Count);
        Assert.Equal("A_TPose", controller.Clips[0].Name);
        Assert.Equal("Walk_Loop", controller.Clips[^1].Name);
        Assert.Equal(42, controller.CurrentClipIndex);
        foreach (AnimationClip clip in controller.Clips)
        {
            CharacterAnimationFrame frame = SdlGpuCharacterHarness.CreateAnimationFrame(
                asset.Mesh.Skin,
                clip,
                clip.Duration * 0.37f);

            Assert.Same(asset.Mesh.Skin.Skeleton, frame.GlobalPose.Skeleton);
            Assert.Equal(65, frame.Palette.JointMatrices.Count);
            Assert.All(frame.Palette.JointMatrices, static matrix => AssertFinite(matrix));

            SkeletonDebugGeometry skeleton = SkeletonDebugGeometry.Create(frame.GlobalPose, 0.04f);
            Assert.Equal(259, skeleton.LineCount);
            Assert.Equal(518, skeleton.Vertices.Length);
        }
    }

    private static SkinningPalette CreatePalette(SkinDefinition skin, SkeletonPose pose)
    {
        SkeletonGlobalPose global = SkeletonPoseEvaluator.EvaluateGlobal(pose);
        return SkeletonPoseEvaluator.CreateSkinningPalette(skin, global);
    }

    private static void AssertFinite(Matrix4x4 matrix)
    {
        Assert.All(new[]
        {
            matrix.M11, matrix.M12, matrix.M13, matrix.M14,
            matrix.M21, matrix.M22, matrix.M23, matrix.M24,
            matrix.M31, matrix.M32, matrix.M33, matrix.M34,
            matrix.M41, matrix.M42, matrix.M43, matrix.M44,
        }, static value => Assert.True(float.IsFinite(value)));
    }

    private static SkeletalCharacterAsset LoadSelectedAsset()
    {
        string root = FindRepositoryRoot();
        return SimpleMeshSkeletalAssetLoader.LoadFromFile(Path.Combine(
            root,
            "assets",
            "Quaternius",
            "Universal Animation Library[Standard]",
            "Unreal-Godot",
            "UAL1_Standard.glb"));
    }

    private static string FindRepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "ChronoFall.slnx")))
                return directory.FullName;
            directory = directory.Parent;
        }
        throw new DirectoryNotFoundException("Could not find ChronoFall.slnx from the test output directory.");
    }
}
