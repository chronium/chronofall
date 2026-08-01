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

    [Fact]
    public void SelectedLocomotionAndActionBlendsProduceFiniteDistinctPalettes()
    {
        SkeletalCharacterAsset asset = LoadSelectedAsset();
        AnimationClip idle = Select(asset, "Idle_Loop");
        AnimationClip walk = Select(asset, "Walk_Loop");
        AnimationClip action = Select(asset, "Sword_Attack");
        SkeletonPose idlePose = AnimationSampler.Sample(idle, 1.25f, AnimationPlaybackMode.Loop);
        SkeletonPose walkPose = AnimationSampler.Sample(walk, 0.5f, AnimationPlaybackMode.Loop);
        SkeletonPose locomotionMidpoint = SkeletonPoseBlender.Blend(idlePose, walkPose, 0.5f);
        SkeletonPose actionMidpoint = SkeletonPoseBlender.Blend(
            walkPose,
            AnimationSampler.Sample(action, 0.05f, AnimationPlaybackMode.Clamp),
            0.5f);

        SkinningPalette idlePalette = CreatePalette(asset.Mesh.Skin, idlePose);
        SkinningPalette walkPalette = CreatePalette(asset.Mesh.Skin, walkPose);
        SkinningPalette locomotionPalette = CreatePalette(asset.Mesh.Skin, locomotionMidpoint);
        SkinningPalette actionPalette = CreatePalette(asset.Mesh.Skin, actionMidpoint);

        Assert.All(locomotionPalette.JointMatrices, static matrix => AssertFinite(matrix));
        Assert.All(actionPalette.JointMatrices, static matrix => AssertFinite(matrix));
        Assert.False(idlePalette.JointMatrices.SequenceEqual(locomotionPalette.JointMatrices));
        Assert.False(walkPalette.JointMatrices.SequenceEqual(locomotionPalette.JointMatrices));
        Assert.False(walkPalette.JointMatrices.SequenceEqual(actionPalette.JointMatrices));
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

    private static AnimationClip Select(SkeletalCharacterAsset asset, string name) =>
        Assert.Single(asset.Animations, candidate => string.Equals(candidate.Name, name, StringComparison.Ordinal));

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
