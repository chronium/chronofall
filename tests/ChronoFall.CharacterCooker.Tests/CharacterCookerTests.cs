using System.Security.Cryptography;
using System.Text.Json;
using ChronoFall.CharacterExperiment.SimpleMesh;

namespace ChronoFall.CharacterCooker.Tests;

public sealed class CharacterCookerTests
{
    private const string SelectedSource = "assets/Quaternius/Universal Animation Library[Standard]/Unreal-Godot/UAL1_Standard.glb";
    private const string ExpectedSourceHash = "69591853d817488edaa8fd9bf8fc1d821eaeaf789f8627b3cd23b41c4ed67997";
    private const string ExpectedUal2SourceHash = "866c2ee822d30f0ceed521f50a5e84316d58ee4487d0b02158370bb988452416";

    [Fact]
    public void BasicArrowRecipeSelectsOnlyTheApprovedPrivateUal2Clips()
    {
        string root = FindRepositoryRoot();
        string recipePath = Path.Combine(
            root,
            "assets",
            "recipes",
            "quaternius-ual2-source-bow-shot-body.json");

        using JsonDocument document = JsonDocument.Parse(File.ReadAllText(recipePath));
        JsonElement recipe = document.RootElement;
        Assert.Equal(1, recipe.GetProperty("version").GetInt32());
        Assert.Equal("quaternius-ual2-source-bow-shot-body", recipe.GetProperty("assetId").GetString());
        Assert.Equal("Unreal-Godot/UAL2.glb", recipe.GetProperty("source").GetString());
        Assert.Equal(ExpectedUal2SourceHash, recipe.GetProperty("sourceSha256").GetString());
        Assert.Equal("CC0-1.0", recipe.GetProperty("licenseIdentifier").GetString());
        Assert.Equal(
            [
                "assets/provenance/Quaternius/Universal Animation Library 2 Source/License.txt",
                "assets/provenance/Quaternius/Universal Animation Library 2 Source/README.txt",
            ],
            recipe.GetProperty("licenseEvidence").EnumerateArray().Select(static item => item.GetString()));
        Assert.Equal("Mannequin", recipe.GetProperty("meshNodeName").GetString());
        Assert.Equal("Mannequin", recipe.GetProperty("meshName").GetString());
        Assert.Equal("Armature", recipe.GetProperty("skinName").GetString());
        Assert.Equal(
            ["Bow_Notch", "Bow_Aim_Neutral", "Bow_Shoot"],
            recipe.GetProperty("animationClips").EnumerateArray().Select(static item => item.GetString()));
        Assert.DoesNotContain(root, File.ReadAllText(recipePath), StringComparison.Ordinal);
    }

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
    public void ProvenanceIsPortableDeterministicAndMatchesCookedOutput()
    {
        string root = FindRepositoryRoot();
        string recipePath = Path.Combine(root, "assets", "recipes", "quaternius-ual1-standard.json");
        using var temporary = new TemporaryDirectory();
        string firstOutput = Path.Combine(temporary.Path, "first", "quaternius-ual1-standard.cfskel");
        string secondOutput = Path.Combine(temporary.Path, "second", "quaternius-ual1-standard.cfskel");
        string firstProvenance = Path.Combine(temporary.Path, "first", "quaternius-ual1-standard.provenance.json");
        string secondProvenance = Path.Combine(temporary.Path, "second", "quaternius-ual1-standard.provenance.json");

        CharacterCookResult first = CharacterCooker.Run(
            new CharacterCookOptions(root, recipePath, firstOutput, firstProvenance));
        CharacterCookResult second = CharacterCooker.Run(
            new CharacterCookOptions(root, recipePath, secondOutput, secondProvenance));

        Assert.Equal(File.ReadAllBytes(firstProvenance), File.ReadAllBytes(secondProvenance));
        string json = File.ReadAllText(firstProvenance);
        Assert.DoesNotContain(root, json, StringComparison.Ordinal);

        using JsonDocument document = JsonDocument.Parse(json);
        JsonElement value = document.RootElement;
        Assert.Equal(1, value.GetProperty("schemaVersion").GetInt32());
        Assert.Equal("client", value.GetProperty("audience").GetString());
        Assert.Equal("quaternius-ual1-standard", value.GetProperty("assetId").GetString());
        Assert.Equal("assets/recipes/quaternius-ual1-standard.json", value.GetProperty("recipePath").GetString());
        Assert.Equal(SelectedSource, value.GetProperty("sourcePath").GetString());
        Assert.Equal(ExpectedSourceHash, value.GetProperty("sourceSha256").GetString());
        Assert.Equal("CC0-1.0", value.GetProperty("licenseIdentifier").GetString());
        Assert.Equal(["Idle_Loop", "Walk_Loop", "Sword_Attack"],
            value.GetProperty("animationClips").EnumerateArray().Select(static item => item.GetString()));
        Assert.Equal(Path.GetFileName(firstOutput), value.GetProperty("cookedFile").GetString());
        Assert.Equal(first.OutputBytes, value.GetProperty("cookedBytes").GetInt64());
        Assert.Equal(first.OutputSha256, value.GetProperty("cookedSha256").GetString());
        Assert.Equal(first.OutputSha256, second.OutputSha256);
    }

    [Fact]
    public void RecipeAndLicenseEvidenceCanUseASeparateRootFromPrivateSource()
    {
        string root = FindRepositoryRoot();
        string sourceRoot = Path.Combine(
            root,
            "assets",
            "Quaternius",
            "Universal Animation Library[Standard]");
        string originalRecipe = Path.Combine(root, "assets", "recipes", "quaternius-ual1-standard.json");
        using var temporary = new TemporaryDirectory();
        string recipePath = Path.Combine(temporary.Path, "recipe.json");
        string outputPath = Path.Combine(temporary.Path, "output", "character.cfskel");
        string provenancePath = Path.Combine(temporary.Path, "output", "character.provenance.json");
        string recipe = File.ReadAllText(originalRecipe)
            .Replace(SelectedSource, "Unreal-Godot/UAL1_Standard.glb", StringComparison.Ordinal)
            .Replace(
                "assets/Quaternius/Universal Animation Library[Standard]/License.txt",
                "License.txt",
                StringComparison.Ordinal)
            .Replace(
                "assets/Quaternius/Universal Animation Library[Standard]/README.txt",
                "README.txt",
                StringComparison.Ordinal);
        File.WriteAllText(recipePath, recipe);
        File.Copy(Path.Combine(sourceRoot, "License.txt"), Path.Combine(temporary.Path, "License.txt"));
        File.Copy(Path.Combine(sourceRoot, "README.txt"), Path.Combine(temporary.Path, "README.txt"));

        CharacterCookResult result = CharacterCooker.Run(
            new CharacterCookOptions(
                sourceRoot,
                recipePath,
                outputPath,
                provenancePath,
                temporary.Path));

        Assert.Equal(3, result.ClipCount);
        using JsonDocument document = JsonDocument.Parse(File.ReadAllText(provenancePath));
        JsonElement value = document.RootElement;
        Assert.Equal("recipe.json", value.GetProperty("recipePath").GetString());
        Assert.Equal("Unreal-Godot/UAL1_Standard.glb", value.GetProperty("sourcePath").GetString());
        Assert.DoesNotContain(root, value.GetRawText(), StringComparison.Ordinal);
        Assert.DoesNotContain(temporary.Path, value.GetRawText(), StringComparison.Ordinal);
    }

    [Fact]
    public void CommandLineDefaultsRecipeRootToSourceRootAndAcceptsAnExplicitRoot()
    {
        CharacterCookOptions compatible = CharacterCookOptions.Parse([
            "--source-root", "source",
            "--recipe", "recipe.json",
            "--output", "output.cfskel",
            "--audience", "client",
        ]);
        CharacterCookOptions separated = CharacterCookOptions.Parse([
            "--source-root", "private-source",
            "--recipe-root", "coordinator",
            "--recipe", "recipe.json",
            "--output", "output.cfskel",
            "--audience", "client",
        ]);

        Assert.Null(compatible.RecipeRoot);
        Assert.Equal("coordinator", separated.RecipeRoot);
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
        string recipeJson = mutate(File.ReadAllText(originalRecipe)).Replace(
            "assets/Quaternius/Universal Animation Library[Standard]/",
            string.Empty,
            StringComparison.Ordinal);
        File.WriteAllText(recipe, recipeJson);
        string sourceDirectory = Path.Combine(
            root,
            "assets",
            "Quaternius",
            "Universal Animation Library[Standard]");
        File.Copy(Path.Combine(sourceDirectory, "License.txt"), Path.Combine(temporary.Path, "License.txt"));
        File.Copy(Path.Combine(sourceDirectory, "README.txt"), Path.Combine(temporary.Path, "README.txt"));

        Exception exception = Assert.ThrowsAny<Exception>(() =>
            CharacterCooker.Run(new CharacterCookOptions(sourceDirectory, recipe, output, RecipeRoot: temporary.Path)));

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
