using System.Globalization;
using System.Numerics;
using System.Security.Cryptography;
using SimpleMesh;

namespace ChronoFall.CharacterExperiment.SimpleMesh.Tests;

public sealed class StaticMeshLoaderTests
{
    [Fact]
    public void ExactFixtureLoadsWithOnlyDeclaredResourceAndMaterialEvidence()
    {
        string root = FindRepositoryRoot();
        string source = Path.Combine(root, "tests", "fixtures", "static-cooking", "two-boxes.obj");
        string material = Path.Combine(root, "tests", "fixtures", "static-cooking", "two-boxes.mtl");

        CultureInfo previousCulture = CultureInfo.CurrentCulture;
        SimpleMeshStaticSourceAsset imported;
        try
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("ro-RO");
            imported = SimpleMeshStaticAssetLoader.LoadFromFile(
                "chronofall-static-two-boxes",
                root,
                source,
                new Dictionary<string, string>
                {
                    ["tests/fixtures/static-cooking/two-boxes.mtl"] = Sha256(material),
                },
                1.0f);
        }
        finally
        {
            CultureInfo.CurrentCulture = previousCulture;
        }

        Assert.Equal(48, imported.Mesh.Vertices.Count);
        Assert.Equal(72, imported.Mesh.Indices.Count);
        Assert.Equal(2, imported.Mesh.Sections.Count);
        Assert.Equal(["diagnostic-orange", "diagnostic-blue"], imported.Materials.Select(static item => item.Name));
        Assert.Equal(["tests/fixtures/static-cooking/two-boxes.mtl"], imported.OpenedExternalResources);
    }

    [Fact]
    public void ExternalResourcesAreHashCheckedExactAndFullyConsumed()
    {
        string root = FindRepositoryRoot();
        string source = Path.Combine(root, "tests", "fixtures", "static-cooking", "two-boxes.obj");
        string relative = "tests/fixtures/static-cooking/two-boxes.mtl";

        Assert.Throws<StaticAssetLoadException>(() => SimpleMeshStaticAssetLoader.LoadFromFile(
            "fixture", root, source, new Dictionary<string, string> { [relative] = new string('0', 64) }, 1.0f));

        var unused = new Dictionary<string, string>
        {
            [relative] = Sha256(Path.Combine(root, relative)),
            ["tests/fixtures/static-cooking/LICENSE.txt"] = Sha256(Path.Combine(root, "tests/fixtures/static-cooking/LICENSE.txt")),
        };
        StaticAssetLoadException exception = Assert.Throws<StaticAssetLoadException>(() =>
            SimpleMeshStaticAssetLoader.LoadFromFile("fixture", root, source, unused, 1.0f));
        Assert.Contains("unused", exception.Message, StringComparison.OrdinalIgnoreCase);

        StaticAssetLoadException undeclared = Assert.Throws<StaticAssetLoadException>(() =>
            SimpleMeshStaticAssetLoader.LoadFromFile("fixture", root, source, new Dictionary<string, string>(), 1.0f));
        Assert.Contains("not declared", undeclared.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void ExternalGltfBufferIsExactAndProducesTriangleGeometry()
    {
        using var temporary = new TemporaryDirectory();
        string buffer = Path.Combine(temporary.Path, "triangle.bin");
        using (var stream = File.Create(buffer))
        using (var writer = new BinaryWriter(stream))
        {
            foreach (float value in new[] { 0f, 0f, 0f, 1f, 0f, 0f, 0f, 1f, 0f })
                writer.Write(value);
            foreach (float value in new[] { 0f, 0f, 1f, 0f, 0f, 1f, 0f, 0f, 1f })
                writer.Write(value);
            writer.Write((ushort)0);
            writer.Write((ushort)1);
            writer.Write((ushort)2);
        }
        string source = Path.Combine(temporary.Path, "triangle.gltf");
        File.WriteAllText(source, """
            {
              "asset": { "version": "2.0" },
              "scene": 0,
              "scenes": [{ "nodes": [0] }],
              "nodes": [{ "name": "Triangle", "mesh": 0 }],
              "meshes": [{ "name": "Triangle", "primitives": [{
                "attributes": { "POSITION": 0, "NORMAL": 1 },
                "indices": 2,
                "material": 0
              }] }],
              "materials": [{ "name": "flat" }],
              "buffers": [{ "uri": "triangle.bin", "byteLength": 78 }],
              "bufferViews": [
                { "buffer": 0, "byteOffset": 0, "byteLength": 36 },
                { "buffer": 0, "byteOffset": 36, "byteLength": 36 },
                { "buffer": 0, "byteOffset": 72, "byteLength": 6 }
              ],
              "accessors": [
                { "bufferView": 0, "componentType": 5126, "count": 3, "type": "VEC3", "min": [0,0,0], "max": [1,1,0] },
                { "bufferView": 1, "componentType": 5126, "count": 3, "type": "VEC3" },
                { "bufferView": 2, "componentType": 5123, "count": 3, "type": "SCALAR" }
              ]
            }
            """);

        SimpleMeshStaticSourceAsset imported = SimpleMeshStaticAssetLoader.LoadFromFile(
            "triangle",
            temporary.Path,
            source,
            new Dictionary<string, string> { ["triangle.bin"] = Sha256(buffer) },
            1.0f);

        Assert.Equal(3, imported.Mesh.Vertices.Count);
        Assert.Equal([0u, 1u, 2u], imported.Mesh.Indices);
        Assert.Equal(["flat"], imported.Materials.Select(static material => material.Name));
    }

    [Fact]
    public void MappingBakesHierarchyAndScaleUsingInverseTransposeNormals()
    {
        Model model = CreateTriangleModel(VertexAttributes.Normal);
        model.Roots[0].Transform = Matrix4x4.CreateScale(2.0f, 1.0f, 0.5f) * Matrix4x4.CreateTranslation(2, 3, 4);

        SimpleMeshStaticSourceAsset imported = SimpleMeshStaticAssetLoader.MapModel("triangle", model, "memory", 0.5f);

        Assert.Equal(new Vector3(1, 1.5f, 2), imported.Mesh.Vertices[0].Position);
        Assert.Equal(Vector3.UnitZ, imported.Mesh.Vertices[0].Normal);
        Assert.Equal([0u, 1u, 2u], imported.Mesh.Indices);
        Assert.Single(imported.Mesh.Sections);
    }

    [Fact]
    public void MappingRejectsMissingNormalsReflectionsAndNonTriangles()
    {
        Assert.Throws<StaticAssetLoadException>(() =>
            SimpleMeshStaticAssetLoader.MapModel("triangle", CreateTriangleModel(VertexAttributes.None), "memory", 1.0f));

        Model reflected = CreateTriangleModel(VertexAttributes.Normal);
        reflected.Roots[0].Transform = Matrix4x4.CreateScale(-1, 1, 1);
        Assert.Throws<StaticAssetLoadException>(() =>
            SimpleMeshStaticAssetLoader.MapModel("triangle", reflected, "memory", 1.0f));

        Model lines = CreateTriangleModel(VertexAttributes.Normal);
        lines.Roots[0].Geometry!.Kind = GeometryKind.Lines;
        Assert.Throws<StaticAssetLoadException>(() =>
            SimpleMeshStaticAssetLoader.MapModel("triangle", lines, "memory", 1.0f));
    }

    private static Model CreateTriangleModel(VertexAttributes attributes)
    {
        var vertices = new VertexArray(attributes, 3);
        vertices.Position[0] = Vector3.Zero;
        vertices.Position[1] = Vector3.UnitX;
        vertices.Position[2] = Vector3.UnitY;
        if ((attributes & VertexAttributes.Normal) == VertexAttributes.Normal)
        {
            vertices.Normal[0] = Vector3.UnitZ;
            vertices.Normal[1] = Vector3.UnitZ;
            vertices.Normal[2] = Vector3.UnitZ;
        }
        var material = new Material { Name = "flat", DiffuseColor = LinearColor.White };
        var geometry = new Geometry(vertices, Indices.FromBuffer([0u, 1u, 2u]))
        {
            Name = "triangle",
            Kind = GeometryKind.Triangles,
            Groups = [new TriangleGroup(material) { StartIndex = 0, BaseVertex = 0, IndexCount = 3 }],
        };
        return new Model
        {
            Roots = [new ModelNode { Name = "root", Geometry = geometry }],
            Geometries = [geometry],
            Materials = new Dictionary<string, Material> { [material.Name] = material },
        };
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
            Path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"chronofall-static-loader-{Guid.NewGuid():N}");
            Directory.CreateDirectory(Path);
        }

        internal string Path { get; }

        public void Dispose() => Directory.Delete(Path, recursive: true);
    }
}
