using System.Security.Cryptography;
using ChronoFall.CharacterExperiment.SimpleMesh;

namespace ChronoFall.CharacterCooker.Tests;

public sealed class CharacterCookerTests
{
    private const string SelectedSource = "assets/Quaternius/Universal Animation Library[Standard]/Unreal-Godot/UAL1_Standard.glb";
    private const string ExpectedSourceHash = "69591853d817488edaa8fd9bf8fc1d821eaeaf789f8627b3cd23b41c4ed67997";

    [Fact]
    public void SelectedRecipeCooksByteIdenticalExactSelectionWithoutModifyingSource()
    {
        string root = FindRepositoryRoot();
        string sourcePath = Path.Combine(root, SelectedSource.Replace('/', Path.DirectorySeparatorChar));
        string beforeHash = Sha256(sourcePath);
        using var temporary = new TemporaryDirectory();
        string firstPath = Path.Combine(temporary.Path, "first.cfskel");
        string secondPath = Path.Combine(temporary.Path, "second.cfskel");
        string recipePath = Path.Combine(root, "assets", "recipes", "quaternius-ual1-standard.json");

        CharacterCookResult first = CharacterCooker.Run(new CharacterCookOptions(root, recipePath, firstPath));
        CharacterCookResult second = CharacterCooker.Run(new CharacterCookOptions(root, recipePath, secondPath));

        Assert.Equal(File.ReadAllBytes(firstPath), File.ReadAllBytes(secondPath));
        Assert.Equal(first.OutputSha256, second.OutputSha256);
        Assert.Equal(3, first.ClipCount);
        Assert.Equal(ExpectedSourceHash, beforeHash);
        Assert.Equal(beforeHash, Sha256(sourcePath));

        using FileStream stream = File.OpenRead(firstPath);
        CookedSkeletalCharacterAsset cooked = SkeletalAssetCookedFormat.Read(stream);
        Assert.Equal("quaternius-ual1-standard", cooked.Descriptor.AssetId);
        Assert.Equal(SelectedSource, cooked.Descriptor.SourcePath);
        Assert.Equal(ExpectedSourceHash, cooked.Descriptor.SourceSha256);
        Assert.Equal("CC0-1.0", cooked.Descriptor.LicenseIdentifier);
        Assert.Equal("Mannequin", cooked.Descriptor.SourceMeshNodeName);
        Assert.Equal("Mannequin", cooked.Descriptor.SourceMeshName);
        Assert.Equal("Armature", cooked.Descriptor.SourceSkinName);
        Assert.Equal(["Idle_Loop", "Walk_Loop", "Sword_Attack"], cooked.Asset.Animations.Select(static clip => clip.Name));

        SimpleMeshSkeletalSourceAsset imported = SimpleMeshSkeletalAssetLoader.LoadSourceFromFile(sourcePath);
        var expected = new SkeletalCharacterAsset(
            imported.Asset.Mesh,
            cooked.Asset.Animations.Select(clip => imported.Asset.Animations.Single(candidate => candidate.Name == clip.Name)));
        AssertEquivalent(expected, cooked.Asset);
    }

    [Fact]
    public void ServerAudienceIsRejectedExplicitly()
    {
        Assert.Throws<ArgumentException>(() => CharacterCookOptions.Parse([
            "--source-root", ".",
            "--recipe", "recipe.json",
            "--output", "output.cfskel",
            "--audience", "server",
        ]));
    }

    [Fact]
    public void WrongSourceHashFailsWithoutCreatingOutput()
    {
        RunInvalidRecipe(
            json => json.Replace(ExpectedSourceHash, new string('0', 64), StringComparison.Ordinal),
            "SHA-256");
    }

    [Fact]
    public void EscapingSourcePathFailsWithoutCreatingOutput()
    {
        RunInvalidRecipe(
            json => json.Replace(SelectedSource, "../escape.glb", StringComparison.Ordinal),
            "cannot contain");
    }

    [Fact]
    public void DuplicateClipSelectionFailsWithoutCreatingOutput()
    {
        RunInvalidRecipe(
            json => json.Replace("\"Sword_Attack\"", "\"Walk_Loop\"", StringComparison.Ordinal),
            "unique");
    }

    [Fact]
    public void MissingClipFailsWithoutCreatingOutput()
    {
        RunInvalidRecipe(
            json => json.Replace("\"Sword_Attack\"", "\"Missing_Clip\"", StringComparison.Ordinal),
            "was not found");
    }

    [Fact]
    public void MissingLicenseEvidenceFailsWithoutCreatingOutput()
    {
        RunInvalidRecipe(
            json => json.Replace("README.txt", "MISSING.txt", StringComparison.Ordinal),
            "was not found");
    }

    [Fact]
    public void MismatchedEmbeddedIdentifierFailsWithoutCreatingOutput()
    {
        RunInvalidRecipe(
            json => json.Replace("\"meshName\": \"Mannequin\"", "\"meshName\": \"WrongMesh\"", StringComparison.Ordinal),
            "expected 'WrongMesh'");
    }

    [Fact]
    public void OutputCannotOverwriteRecipe()
    {
        string root = FindRepositoryRoot();
        using var temporary = new TemporaryDirectory();
        string recipe = Path.Combine(temporary.Path, "recipe.cfskel");
        File.Copy(Path.Combine(root, "assets", "recipes", "quaternius-ual1-standard.json"), recipe);
        byte[] before = File.ReadAllBytes(recipe);

        Assert.Throws<ArgumentException>(() =>
            CharacterCooker.Run(new CharacterCookOptions(root, recipe, recipe)));

        Assert.Equal(before, File.ReadAllBytes(recipe));
    }

    private static void RunInvalidRecipe(Func<string, string> mutate, string expectedMessage)
    {
        string root = FindRepositoryRoot();
        string originalRecipe = Path.Combine(root, "assets", "recipes", "quaternius-ual1-standard.json");
        using var temporary = new TemporaryDirectory();
        string recipe = Path.Combine(temporary.Path, "recipe.json");
        string output = Path.Combine(temporary.Path, "output.cfskel");
        File.WriteAllText(recipe, mutate(File.ReadAllText(originalRecipe)));

        Exception exception = Assert.ThrowsAny<Exception>(() =>
            CharacterCooker.Run(new CharacterCookOptions(root, recipe, output)));

        Assert.Contains(expectedMessage, exception.Message, StringComparison.OrdinalIgnoreCase);
        Assert.False(File.Exists(output));
    }

    private static void AssertEquivalent(SkeletalCharacterAsset expected, SkeletalCharacterAsset actual)
    {
        Assert.Equal(expected.Mesh.Name, actual.Mesh.Name);
        Assert.Equal(expected.Mesh.Skin.Skeleton.JointCount, actual.Mesh.Skin.Skeleton.JointCount);
        for (int index = 0; index < expected.Mesh.Skin.Skeleton.JointCount; index++)
        {
            Assert.Equal(expected.Mesh.Skin.Skeleton.Joints[index].Name, actual.Mesh.Skin.Skeleton.Joints[index].Name);
            Assert.Equal(expected.Mesh.Skin.Skeleton.Joints[index].ParentIndex, actual.Mesh.Skin.Skeleton.Joints[index].ParentIndex);
            Assert.Equal(expected.Mesh.Skin.Skeleton.Joints[index].LocalBindTransform, actual.Mesh.Skin.Skeleton.Joints[index].LocalBindTransform);
            Assert.Equal(expected.Mesh.Skin.InverseBindMatrices[index], actual.Mesh.Skin.InverseBindMatrices[index]);
        }
        Assert.Equal(expected.Mesh.Vertices, actual.Mesh.Vertices);
        Assert.Equal(expected.Mesh.Indices, actual.Mesh.Indices);
        Assert.Equal(expected.Mesh.Sections.Count, actual.Mesh.Sections.Count);
        for (int index = 0; index < expected.Mesh.Sections.Count; index++)
        {
            Assert.Equal(expected.Mesh.Sections[index].MaterialName, actual.Mesh.Sections[index].MaterialName);
            Assert.Equal(expected.Mesh.Sections[index].StartIndex, actual.Mesh.Sections[index].StartIndex);
            Assert.Equal(expected.Mesh.Sections[index].IndexCount, actual.Mesh.Sections[index].IndexCount);
        }
        Assert.Equal(expected.Animations.Count, actual.Animations.Count);
        for (int clipIndex = 0; clipIndex < expected.Animations.Count; clipIndex++)
        {
            AnimationClip expectedClip = expected.Animations[clipIndex];
            AnimationClip actualClip = actual.Animations[clipIndex];
            Assert.Equal(expectedClip.Name, actualClip.Name);
            Assert.Equal(expectedClip.Duration, actualClip.Duration);
            for (int trackIndex = 0; trackIndex < expectedClip.Tracks.Count; trackIndex++)
            {
                JointAnimationTrack expectedTrack = expectedClip.Tracks[trackIndex];
                JointAnimationTrack actualTrack = actualClip.Tracks[trackIndex];
                Assert.Equal(expectedTrack.JointIndex, actualTrack.JointIndex);
                Assert.Equal(expectedTrack.Translations.Keyframes, actualTrack.Translations.Keyframes);
                Assert.Equal(expectedTrack.Rotations.Keyframes, actualTrack.Rotations.Keyframes);
                Assert.Equal(expectedTrack.Scales.Keyframes, actualTrack.Scales.Keyframes);
            }
        }
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
        throw new DirectoryNotFoundException("Could not find the ChronoFall repository root.");
    }

    private static string Sha256(string path)
    {
        using FileStream stream = File.OpenRead(path);
        return Convert.ToHexString(SHA256.HashData(stream)).ToLowerInvariant();
    }

    private sealed class TemporaryDirectory : IDisposable
    {
        internal TemporaryDirectory()
        {
            Path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"chronofall-cooker-tests-{Guid.NewGuid():N}");
            Directory.CreateDirectory(Path);
        }

        internal string Path { get; }

        public void Dispose()
        {
            if (Directory.Exists(Path))
                Directory.Delete(Path, recursive: true);
        }
    }
}
