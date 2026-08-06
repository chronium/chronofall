using System.Numerics;
using System.Security.Cryptography;
using System.Text.Json;

namespace ChronoFall.StaticMeshCooker.Tests;

public sealed class StaticMeshCookerTests
{
    private const string FixtureRecipe = "tests/fixtures/static-cooking/two-boxes.recipe.json";

    [Theory]
    [InlineData(
        "assets/recipes/quaternius-medieval-weapons-bow-wooden.json",
        "quaternius-medieval-weapons-bow-wooden",
        "assets/Quaternius/Medieval Weapons Pack by @Quaternius/OBJ/Bow_Wooden.obj",
        "788c9e72bdd839a86704113de4809a96cfedf09441bb3f98f383a7abfe751e6d",
        3,
        -0.09612475f, -0.6795755f, -0.030426f,
        0.27085575f, 0.6795755f, 0.0304265f)]
    [InlineData(
        "assets/recipes/quaternius-medieval-weapons-arrow.json",
        "quaternius-medieval-weapons-arrow",
        "assets/Quaternius/Medieval Weapons Pack by @Quaternius/OBJ/Arrow.obj",
        "6960c207e3a8e6f2f09cbfd31b7fe990119cd260ef692729c498738a86698bf1",
        4,
        -0.034382f, -0.0213025f, -0.01055675f,
        0.03415425f, 0.038266f, 0.67289925f)]
    public void SelectedMedievalWeaponCooksAreExactPortableAndDeterministic(
        string recipeRelativePath,
        string expectedAssetId,
        string sourceRelativePath,
        string expectedSourceHash,
        int expectedSections,
        float minimumX,
        float minimumY,
        float minimumZ,
        float maximumX,
        float maximumY,
        float maximumZ)
    {
        string root = FindRepositoryRoot();
        string recipe = Path.Combine(root, recipeRelativePath.Replace('/', Path.DirectorySeparatorChar));
        string source = Path.Combine(root, sourceRelativePath.Replace('/', Path.DirectorySeparatorChar));
        using var temporary = new TemporaryDirectory();
        string firstOutput = Path.Combine(temporary.Path, "first", $"{expectedAssetId}.cfmesh");
        string secondOutput = Path.Combine(temporary.Path, "second", $"{expectedAssetId}.cfmesh");
        string firstProvenance = Path.Combine(temporary.Path, "first", $"{expectedAssetId}.json");
        string secondProvenance = Path.Combine(temporary.Path, "second", $"{expectedAssetId}.json");

        StaticMeshCookResult first = StaticMeshCooker.Run(
            new StaticMeshCookOptions(root, recipe, firstOutput, firstProvenance));
        StaticMeshCookResult second = StaticMeshCooker.Run(
            new StaticMeshCookOptions(root, recipe, secondOutput, secondProvenance));

        Assert.Equal(File.ReadAllBytes(firstOutput), File.ReadAllBytes(secondOutput));
        Assert.Equal(File.ReadAllBytes(firstProvenance), File.ReadAllBytes(secondProvenance));
        Assert.Equal(first.OutputSha256, second.OutputSha256);
        Assert.Equal(expectedSourceHash, Sha256(source));
        Assert.Equal(expectedAssetId, first.AssetId);
        Assert.Equal(expectedSections, first.SectionCount);

        using FileStream stream = File.OpenRead(firstOutput);
        CookedStaticMeshAsset cooked = StaticMeshCookedFormat.Read(stream);
        Assert.Equal(expectedAssetId, cooked.Descriptor.AssetId);
        Assert.Equal(sourceRelativePath, cooked.Descriptor.PrimarySource.Path);
        Assert.Equal(expectedSourceHash, cooked.Descriptor.PrimarySource.Sha256);
        Assert.Equal(0.25f, cooked.Descriptor.MetersPerSourceUnit);
        Assert.Equal("CC0-1.0", cooked.Descriptor.LicenseIdentifier);
        Assert.Equal("section-names-only", cooked.Descriptor.MaterialPolicy);
        Assert.Single(cooked.Descriptor.ExternalResources);
        Assert.Single(cooked.Descriptor.LicenseEvidence);
        Assert.Equal(expectedSections, cooked.Mesh.Sections.Count);

        Vector3 actualMinimum = new(
            cooked.Mesh.Vertices.Min(static vertex => vertex.Position.X),
            cooked.Mesh.Vertices.Min(static vertex => vertex.Position.Y),
            cooked.Mesh.Vertices.Min(static vertex => vertex.Position.Z));
        Vector3 actualMaximum = new(
            cooked.Mesh.Vertices.Max(static vertex => vertex.Position.X),
            cooked.Mesh.Vertices.Max(static vertex => vertex.Position.Y),
            cooked.Mesh.Vertices.Max(static vertex => vertex.Position.Z));
        AssertVectorClose(new(minimumX, minimumY, minimumZ), actualMinimum);
        AssertVectorClose(new(maximumX, maximumY, maximumZ), actualMaximum);

        string provenance = File.ReadAllText(firstProvenance);
        Assert.DoesNotContain(root, provenance, StringComparison.Ordinal);
        using JsonDocument document = JsonDocument.Parse(provenance);
        JsonElement value = document.RootElement;
        Assert.Equal(recipeRelativePath, value.GetProperty("recipePath").GetString());
        Assert.Equal(expectedAssetId, value.GetProperty("assetId").GetString());
        Assert.Equal(first.OutputSha256, value.GetProperty("cookedSha256").GetString());
        Assert.Equal(expectedSections, value.GetProperty("materials").GetArrayLength());
        Assert.False(value.TryGetProperty("timestamp", out _));
    }

    [Fact]
    public void ExactFixtureCooksDeterministicallyWithoutModifyingInputs()
    {
        string root = FindRepositoryRoot();
        string recipe = Path.Combine(root, FixtureRecipe.Replace('/', Path.DirectorySeparatorChar));
        string source = Path.Combine(root, "tests", "fixtures", "static-cooking", "two-boxes.obj");
        string beforeSourceHash = Sha256(source);
        using var temporary = new TemporaryDirectory();
        string firstOutput = Path.Combine(temporary.Path, "first", "fixture.cfmesh");
        string secondOutput = Path.Combine(temporary.Path, "second", "fixture.cfmesh");
        string firstProvenance = Path.Combine(temporary.Path, "first", "fixture.json");
        string secondProvenance = Path.Combine(temporary.Path, "second", "fixture.json");

        StaticMeshCookResult first = StaticMeshCooker.Run(new StaticMeshCookOptions(root, recipe, firstOutput, firstProvenance));
        StaticMeshCookResult second = StaticMeshCooker.Run(new StaticMeshCookOptions(root, recipe, secondOutput, secondProvenance));

        Assert.Equal(File.ReadAllBytes(firstOutput), File.ReadAllBytes(secondOutput));
        Assert.Equal(File.ReadAllBytes(firstProvenance), File.ReadAllBytes(secondProvenance));
        Assert.Equal(first.OutputSha256, second.OutputSha256);
        Assert.Equal(2, first.SectionCount);
        Assert.Equal(beforeSourceHash, Sha256(source));

        using FileStream stream = File.OpenRead(firstOutput);
        CookedStaticMeshAsset cooked = StaticMeshCookedFormat.Read(stream);
        Assert.Equal("chronofall-static-two-boxes", cooked.Descriptor.AssetId);
        Assert.Equal(48, cooked.Mesh.Vertices.Count);
        Assert.Equal(72, cooked.Mesh.Indices.Count);
        Assert.Equal(2, cooked.Mesh.Sections.Count);
    }

    [Fact]
    public void ProvenanceIsPortableAndRecordsExactEvidenceAndLimitations()
    {
        string root = FindRepositoryRoot();
        string recipe = Path.Combine(root, FixtureRecipe.Replace('/', Path.DirectorySeparatorChar));
        using var temporary = new TemporaryDirectory();
        string output = Path.Combine(temporary.Path, "fixture.cfmesh");
        string provenance = Path.Combine(temporary.Path, "fixture.json");

        StaticMeshCookResult result = StaticMeshCooker.Run(new StaticMeshCookOptions(root, recipe, output, provenance));
        string json = File.ReadAllText(provenance);
        Assert.DoesNotContain(root, json, StringComparison.Ordinal);
        using JsonDocument document = JsonDocument.Parse(json);
        JsonElement value = document.RootElement;
        Assert.Equal(1, value.GetProperty("schemaVersion").GetInt32());
        Assert.Equal("client", value.GetProperty("audience").GetString());
        Assert.Equal(FixtureRecipe, value.GetProperty("recipePath").GetString());
        Assert.Equal("section-names-only", value.GetProperty("materialPolicy").GetString());
        Assert.Equal("CC0-1.0", value.GetProperty("licenseIdentifier").GetString());
        Assert.Equal(result.OutputSha256, value.GetProperty("cookedSha256").GetString());
        Assert.Equal(2, value.GetProperty("materials").GetArrayLength());
    }

    [Fact]
    public void ServerAudienceIsRejectedExplicitly()
    {
        Assert.Throws<ArgumentException>(() => StaticMeshCookOptions.Parse([
            "--source-root", ".",
            "--recipe", "recipe.json",
            "--output", "output.cfmesh",
            "--audience", "server",
        ]));
    }

    [Theory]
    [InlineData("sourceSha256", "0000000000000000000000000000000000000000000000000000000000000000", "SHA-256")]
    [InlineData("licensePath", "tests/fixtures/static-cooking/MISSING.txt", "not found")]
    [InlineData("material", "not-the-material", "material sequence")]
    public void InvalidRecipesFailWithoutCreatingOutput(string mutation, string replacement, string expectedMessage)
    {
        using TemporaryFixture fixture = TemporaryFixture.Create();
        string json = File.ReadAllText(fixture.RecipePath);
        json = mutation switch
        {
            "sourceSha256" => json.Replace(fixture.SourceHash, replacement, StringComparison.Ordinal),
            "licensePath" => json.Replace("tests/fixtures/static-cooking/LICENSE.txt", replacement, StringComparison.Ordinal),
            "material" => json.Replace("diagnostic-blue", replacement, StringComparison.Ordinal),
            _ => throw new ArgumentOutOfRangeException(nameof(mutation)),
        };
        File.WriteAllText(fixture.RecipePath, json);
        string output = Path.Combine(fixture.Root, "output.cfmesh");

        Exception exception = Assert.ThrowsAny<Exception>(() => StaticMeshCooker.Run(
            new StaticMeshCookOptions(fixture.Root, fixture.RecipePath, output, null)));

        Assert.Contains(expectedMessage, exception.Message, StringComparison.OrdinalIgnoreCase);
        Assert.False(File.Exists(output));
    }

    [Fact]
    public void OutputCannotOverwriteRecipe()
    {
        using TemporaryFixture fixture = TemporaryFixture.Create("recipe.cfmesh");
        byte[] before = File.ReadAllBytes(fixture.RecipePath);

        Assert.Throws<ArgumentException>(() => StaticMeshCooker.Run(
            new StaticMeshCookOptions(fixture.Root, fixture.RecipePath, fixture.RecipePath, null)));
        Assert.Equal(before, File.ReadAllBytes(fixture.RecipePath));
    }

    [Fact]
    public void ProvenanceCannotOverwriteDeclaredLicenseEvidence()
    {
        using TemporaryFixture fixture = TemporaryFixture.Create();
        string license = Path.Combine(fixture.Root, "tests", "fixtures", "static-cooking", "LICENSE.txt");
        string protectedEvidence = Path.ChangeExtension(license, ".json");
        File.Move(license, protectedEvidence);
        string recipe = File.ReadAllText(fixture.RecipePath)
            .Replace("tests/fixtures/static-cooking/LICENSE.txt", "tests/fixtures/static-cooking/LICENSE.json", StringComparison.Ordinal);
        File.WriteAllText(fixture.RecipePath, recipe);
        byte[] before = File.ReadAllBytes(protectedEvidence);
        string output = Path.Combine(fixture.Root, "output.cfmesh");

        Assert.Throws<ArgumentException>(() => StaticMeshCooker.Run(
            new StaticMeshCookOptions(fixture.Root, fixture.RecipePath, output, protectedEvidence)));
        Assert.Equal(before, File.ReadAllBytes(protectedEvidence));
        Assert.False(File.Exists(output));
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

    private static void AssertVectorClose(Vector3 expected, Vector3 actual)
    {
        const float tolerance = 0.000001f;
        Assert.InRange(actual.X, expected.X - tolerance, expected.X + tolerance);
        Assert.InRange(actual.Y, expected.Y - tolerance, expected.Y + tolerance);
        Assert.InRange(actual.Z, expected.Z - tolerance, expected.Z + tolerance);
    }

    private sealed class TemporaryDirectory : IDisposable
    {
        internal TemporaryDirectory()
        {
            Path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"chronofall-static-cooker-{Guid.NewGuid():N}");
            Directory.CreateDirectory(Path);
        }

        internal string Path { get; }

        public void Dispose() => Directory.Delete(Path, recursive: true);
    }

    private sealed class TemporaryFixture : IDisposable
    {
        private TemporaryFixture(string root, string recipePath, string sourceHash)
        {
            Root = root;
            RecipePath = recipePath;
            SourceHash = sourceHash;
        }

        internal string Root { get; }

        internal string RecipePath { get; }

        internal string SourceHash { get; }

        internal static TemporaryFixture Create(string recipeName = "two-boxes.recipe.json")
        {
            string sourceRoot = FindRepositoryRoot();
            string root = System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"chronofall-static-fixture-{Guid.NewGuid():N}");
            string target = System.IO.Path.Combine(root, "tests", "fixtures", "static-cooking");
            Directory.CreateDirectory(target);
            foreach (string file in Directory.GetFiles(System.IO.Path.Combine(sourceRoot, "tests", "fixtures", "static-cooking")))
                File.Copy(file, System.IO.Path.Combine(target, System.IO.Path.GetFileName(file)));
            string originalRecipe = System.IO.Path.Combine(target, "two-boxes.recipe.json");
            string recipe = System.IO.Path.Combine(target, recipeName);
            if (!string.Equals(originalRecipe, recipe, StringComparison.Ordinal))
                File.Move(originalRecipe, recipe);
            return new TemporaryFixture(root, recipe, Sha256(System.IO.Path.Combine(target, "two-boxes.obj")));
        }

        public void Dispose() => Directory.Delete(Root, recursive: true);
    }
}
