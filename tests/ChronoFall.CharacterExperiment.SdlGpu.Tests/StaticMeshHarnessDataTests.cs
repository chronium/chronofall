using System.Security.Cryptography;
using ChronoFall.CharacterExperiment.SimpleMesh;

namespace ChronoFall.CharacterExperiment.SdlGpu.Tests;

public sealed class StaticMeshHarnessDataTests
{
    [Fact]
    public void DiagnosticMeshIsDeterministicAndContainsTwoCompleteBoxes()
    {
        StaticMeshDefinition first = SdlGpuStaticMeshHarness.CreateDiagnosticMesh();
        StaticMeshDefinition second = SdlGpuStaticMeshHarness.CreateDiagnosticMesh();

        Assert.Equal("static-two-section-diagnostic", first.Name);
        Assert.Equal(48, first.Vertices.Count);
        Assert.Equal(72, first.Indices.Count);
        Assert.Equal(["diagnostic-orange", "diagnostic-blue"], first.Sections.Select(static section => section.MaterialName));
        Assert.Equal(first.Vertices, second.Vertices);
        Assert.Equal(first.Indices, second.Indices);
    }

    [Fact]
    public void CookFixtureImportsAsTheExactDiagnosticGeometry()
    {
        string root = FindRepositoryRoot();
        const string materialRelative = "tests/fixtures/static-cooking/two-boxes.mtl";
        SimpleMeshStaticSourceAsset imported = SimpleMeshStaticAssetLoader.LoadFromFile(
            "static-two-section-diagnostic",
            root,
            Path.Combine(root, "tests", "fixtures", "static-cooking", "two-boxes.obj"),
            new Dictionary<string, string>
            {
                [materialRelative] = Sha256(Path.Combine(root, materialRelative.Replace('/', Path.DirectorySeparatorChar))),
            },
            1.0f);
        StaticMeshDefinition expected = SdlGpuStaticMeshHarness.CreateDiagnosticMesh();

        Assert.Equal(expected.Vertices.Count, imported.Mesh.Vertices.Count);
        for (int index = 0; index < expected.Vertices.Count; index++)
        {
            Assert.True(
                expected.Vertices[index] == imported.Mesh.Vertices[index],
                $"Vertex {index}: expected {expected.Vertices[index]}, actual {imported.Mesh.Vertices[index]}.");
        }
        Assert.Equal(expected.Indices, imported.Mesh.Indices);
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
        throw new DirectoryNotFoundException("Could not locate the repository root.");
    }

    private static string Sha256(string path)
    {
        using FileStream stream = File.OpenRead(path);
        return Convert.ToHexString(SHA256.HashData(stream)).ToLowerInvariant();
    }
}
