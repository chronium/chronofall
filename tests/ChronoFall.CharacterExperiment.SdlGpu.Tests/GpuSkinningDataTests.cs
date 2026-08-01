using System.Numerics;
using System.Runtime.InteropServices;
using ChronoFall.CharacterExperiment.SimpleMesh;

namespace ChronoFall.CharacterExperiment.SdlGpu.Tests;

public sealed class GpuSkinningDataTests
{
    [Fact]
    public void VertexAbiHasExpectedStrideAndOffsets()
    {
        Assert.Equal(48, Marshal.SizeOf<GpuSkinnedVertex>());
        Assert.Equal(0, Marshal.OffsetOf<GpuSkinnedVertex>(nameof(GpuSkinnedVertex.Position)).ToInt32());
        Assert.Equal(12, Marshal.OffsetOf<GpuSkinnedVertex>(nameof(GpuSkinnedVertex.Normal)).ToInt32());
        Assert.Equal(24, Marshal.OffsetOf<GpuSkinnedVertex>(nameof(GpuSkinnedVertex.Joint0)).ToInt32());
        Assert.Equal(32, Marshal.OffsetOf<GpuSkinnedVertex>(nameof(GpuSkinnedVertex.Weights)).ToInt32());
    }

    [Fact]
    public void MeshConversionPreservesIndicesSectionsAndInfluences()
    {
        SkeletonDefinition skeleton = CreateSkeleton();
        var skin = new SkinDefinition(skeleton, [Matrix4x4.Identity, Matrix4x4.Identity]);
        var influences = new SkinInfluences(new JointIndices4(0, 1, 0, 0), new Vector4(0.75f, 0.25f, 0.0f, 0.0f));
        var mesh = new SkinnedMeshDefinition(
            "triangle",
            skin,
            [
                new SkinnedVertex(Vector3.Zero, Vector3.UnitZ, Vector2.Zero, influences),
                new SkinnedVertex(Vector3.UnitX, Vector3.UnitZ, Vector2.UnitX, influences),
                new SkinnedVertex(Vector3.UnitY, Vector3.UnitZ, Vector2.UnitY, influences),
            ],
            [0u, 1u, 2u],
            [new SkinnedMeshSection("diagnostic", 0, 3)]);

        GpuSkinnedMeshData converted = GpuSkinnedMeshData.Create(mesh);

        Assert.Equal([0u, 1u, 2u], converted.Indices);
        Assert.Equal(2, converted.JointCount);
        Assert.Equal(new GpuMeshSection("diagnostic", 0, 3), Assert.Single(converted.Sections));
        Assert.Equal((ushort)0, converted.Vertices[0].Joint0);
        Assert.Equal((ushort)1, converted.Vertices[0].Joint1);
        Assert.Equal(new Vector4(0.75f, 0.25f, 0.0f, 0.0f), converted.Vertices[0].Weights);
    }

    [Fact]
    public void PalettePackingTransposesAtGpuBoundaryAndPreservesExpectedSize()
    {
        SkeletonDefinition skeleton = CreateSkeleton();
        var skin = new SkinDefinition(skeleton, [Matrix4x4.Identity, Matrix4x4.Identity]);
        Matrix4x4 first = Matrix4x4.CreateScale(2.0f) * Matrix4x4.CreateTranslation(1.0f, 2.0f, 3.0f);
        Matrix4x4 second = Matrix4x4.CreateRotationY(0.5f);
        var palette = new SkinningPalette(skin, [first, second]);

        Matrix4x4[] packed = GpuMatrixPacking.PackTransposed(palette);

        Assert.Equal(Matrix4x4.Transpose(first), packed[0]);
        Assert.Equal(Matrix4x4.Transpose(second), packed[1]);
        Assert.Equal(128, MemoryMarshal.AsBytes(packed.AsSpan()).Length);
    }

    [Fact]
    public void SelectedAssetPalettePacksTo4160Bytes()
    {
        string root = FindRepositoryRoot();
        SkeletalCharacterAsset asset = SimpleMeshSkeletalAssetLoader.LoadFromFile(Path.Combine(
            root,
            "assets",
            "Quaternius",
            "Universal Animation Library[Standard]",
            "Unreal-Godot",
            "UAL1_Standard.glb"));
        SkeletonGlobalPose bindPose = SkeletonPoseEvaluator.EvaluateGlobal(asset.Mesh.Skin.Skeleton.CreateBindPose());
        SkinningPalette palette = SkeletonPoseEvaluator.CreateSkinningPalette(asset.Mesh.Skin, bindPose);

        Matrix4x4[] packed = GpuMatrixPacking.PackTransposed(palette);

        Assert.Equal(65, packed.Length);
        Assert.Equal(4160, MemoryMarshal.AsBytes(packed.AsSpan()).Length);
    }

    [Fact]
    public void SelectedWalkSampleProducesFiniteDistinctGpuPalette()
    {
        SkeletalCharacterAsset asset = LoadSelectedAsset();
        AnimationClip animation = Assert.Single(
            asset.Animations,
            candidate => string.Equals(candidate.Name, "Walk_Loop", StringComparison.Ordinal));

        Matrix4x4[] bind = Pack(asset.Mesh.Skin, asset.Mesh.Skin.Skeleton.CreateBindPose());
        Matrix4x4[] sample = Pack(
            asset.Mesh.Skin,
            AnimationSampler.Sample(animation, 0.5f, AnimationPlaybackMode.Loop));

        Assert.Equal(65, sample.Length);
        Assert.Equal(4160, MemoryMarshal.AsBytes(sample.AsSpan()).Length);
        foreach (float value in MemoryMarshal.Cast<Matrix4x4, float>(sample.AsSpan()))
            Assert.True(float.IsFinite(value));
        Assert.False(
            MemoryMarshal.AsBytes(bind.AsSpan()).SequenceEqual(MemoryMarshal.AsBytes(sample.AsSpan())),
            "The deterministic Walk_Loop sample unexpectedly matched the bind-pose palette.");
    }

    [Fact]
    public void SelectedWalkLoopBoundaryPacksExactlyLikeStart()
    {
        SkeletalCharacterAsset asset = LoadSelectedAsset();
        AnimationClip animation = Assert.Single(
            asset.Animations,
            candidate => string.Equals(candidate.Name, "Walk_Loop", StringComparison.Ordinal));

        Matrix4x4[] start = Pack(
            asset.Mesh.Skin,
            AnimationSampler.Sample(animation, 0.0f, AnimationPlaybackMode.Loop));
        Matrix4x4[] boundary = Pack(
            asset.Mesh.Skin,
            AnimationSampler.Sample(animation, animation.Duration, AnimationPlaybackMode.Loop));

        Assert.True(
            MemoryMarshal.AsBytes(start.AsSpan()).SequenceEqual(MemoryMarshal.AsBytes(boundary.AsSpan())),
            "The exact Walk_Loop duration did not reproduce the start palette.");
    }

    [Fact]
    public void EveryBrowserClipProducesFiniteGpuDataOnTheSelectedSkeleton()
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
            Assert.Equal(65, frame.PackedPalette.Length);
            Assert.Equal(4160, MemoryMarshal.AsBytes(frame.PackedPalette.AsSpan()).Length);
            foreach (float value in MemoryMarshal.Cast<Matrix4x4, float>(frame.PackedPalette.AsSpan()))
                Assert.True(float.IsFinite(value), $"Animation '{clip.Name}' produced a non-finite GPU value.");

            SkeletonDebugGeometry skeleton = SkeletonDebugGeometry.Create(frame.GlobalPose, 0.04f);
            Assert.Equal(259, skeleton.LineCount);
            Assert.Equal(518, skeleton.Vertices.Length);
        }
    }

    private static Matrix4x4[] Pack(SkinDefinition skin, SkeletonPose pose)
    {
        SkeletonGlobalPose global = SkeletonPoseEvaluator.EvaluateGlobal(pose);
        SkinningPalette palette = SkeletonPoseEvaluator.CreateSkinningPalette(skin, global);
        return GpuMatrixPacking.PackTransposed(palette);
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

    private static SkeletonDefinition CreateSkeleton() => new([
        new SkeletonJoint("root", -1, JointTransform.Identity),
        new SkeletonJoint("child", 0, new JointTransform(Vector3.UnitY, Quaternion.Identity, Vector3.One)),
    ]);

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
