using System.Security.Cryptography;

namespace ChronoFall.CharacterExperiment.SimpleMesh.Tests;

public sealed class SelectedAssetTests
{
    private const string ExpectedHash = "69591853d817488edaa8fd9bf8fc1d821eaeaf789f8627b3cd23b41c4ed67997";

    [Fact]
    public void SelectedUalAssetMapsCompleteGeometrySkinAndAnimations()
    {
        string path = Path.Combine(
            RepositoryPaths.Root,
            "assets",
            "Quaternius",
            "Universal Animation Library[Standard]",
            "Unreal-Godot",
            "UAL1_Standard.glb");
        string beforeHash = Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(path))).ToLowerInvariant();

        SkeletalCharacterAsset asset = SimpleMeshSkeletalAssetLoader.LoadFromFile(path);

        string afterHash = Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(path))).ToLowerInvariant();
        Assert.Equal(ExpectedHash, beforeHash);
        Assert.Equal(beforeHash, afterHash);
        Assert.Equal(65, asset.Mesh.Skin.Skeleton.JointCount);
        Assert.Equal("root", asset.Mesh.Skin.Skeleton.Joints[0].Name);
        Assert.Equal(65, asset.Mesh.Skin.InverseBindMatrices.Count);
        Assert.Equal(8_546, asset.Mesh.Vertices.Count);
        Assert.Equal(41_232, asset.Mesh.Indices.Count);
        Assert.Collection(
            asset.Mesh.Sections,
            section =>
            {
                Assert.Equal("M_Main", section.MaterialName);
                Assert.Equal(0, section.StartIndex);
                Assert.Equal(17_196, section.IndexCount);
            },
            section =>
            {
                Assert.Equal("M_Joints", section.MaterialName);
                Assert.Equal(17_196, section.StartIndex);
                Assert.Equal(24_036, section.IndexCount);
            });
        Assert.All(asset.Mesh.Vertices, vertex => vertex.Influences.ValidateForSkeleton(asset.Mesh.Skin.Skeleton));
        Assert.Equal(43, asset.Animations.Count);

        AssertClip(asset, "Idle_Loop", 2.5f, 76);
        AssertClip(asset, "Walk_Loop", 1.3333334f, 41);
        AssertClip(asset, "Sword_Attack", 1.5333333f, 47);
    }

    private static void AssertClip(
        SkeletalCharacterAsset asset,
        string name,
        float expectedDuration,
        int expectedSamples)
    {
        AnimationClip clip = Assert.Single(asset.Animations, candidate => candidate.Name == name);
        Assert.Equal(expectedDuration, clip.Duration, 5);
        Assert.Equal(65, clip.Tracks.Count);
        Assert.All(clip.Tracks, track =>
        {
            Assert.Equal(expectedSamples, track.Translations.Keyframes.Count);
            Assert.Equal(expectedSamples, track.Rotations.Keyframes.Count);
            Assert.Equal(expectedSamples, track.Scales.Keyframes.Count);
            Assert.Equal(AnimationInterpolation.Linear, track.Translations.Interpolation);
            Assert.Equal(AnimationInterpolation.Linear, track.Rotations.Interpolation);
            Assert.Equal(AnimationInterpolation.Linear, track.Scales.Interpolation);
        });
    }
}
