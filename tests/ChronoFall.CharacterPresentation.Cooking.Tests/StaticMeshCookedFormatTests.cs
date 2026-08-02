using System.Buffers.Binary;
using System.Numerics;

namespace ChronoFall.CharacterPresentation.Cooking.Tests;

public sealed class StaticMeshCookedFormatTests
{
    [Fact]
    public void RoundTripPreservesDescriptorAndMesh()
    {
        CookedStaticMeshAsset expected = CreateAsset();
        using var stream = new MemoryStream();

        StaticMeshCookedFormat.Write(stream, expected);
        stream.Position = 0;
        CookedStaticMeshAsset actual = StaticMeshCookedFormat.Read(stream);

        Assert.Equal(expected.Descriptor.AssetId, actual.Descriptor.AssetId);
        AssertFile(expected.Descriptor.PrimarySource, actual.Descriptor.PrimarySource);
        Assert.Equal(expected.Descriptor.ExternalResources.Count, actual.Descriptor.ExternalResources.Count);
        AssertFile(expected.Descriptor.ExternalResources[0], actual.Descriptor.ExternalResources[0]);
        Assert.Equal(expected.Descriptor.LicenseIdentifier, actual.Descriptor.LicenseIdentifier);
        AssertFile(expected.Descriptor.LicenseEvidence[0], actual.Descriptor.LicenseEvidence[0]);
        Assert.Equal(expected.Descriptor.MetersPerSourceUnit, actual.Descriptor.MetersPerSourceUnit);
        Assert.Equal(StaticAssetCookDescriptor.SectionNamesOnlyMaterialPolicy, actual.Descriptor.MaterialPolicy);
        AssertMesh(expected.Mesh, actual.Mesh);
    }

    [Fact]
    public void RepeatedWritesAreByteIdentical()
    {
        CookedStaticMeshAsset asset = CreateAsset();
        Assert.Equal(Write(asset), Write(asset));
    }

    [Fact]
    public void ReaderRejectsBadMagicVersionTruncationTrailingDataAndUnboundedStrings()
    {
        byte[] valid = Write(CreateAsset());
        byte[] badMagic = valid.ToArray();
        badMagic[0] ^= 0xff;
        AssertInvalid(badMagic);

        byte[] badVersion = valid.ToArray();
        BinaryPrimitives.WriteUInt32LittleEndian(badVersion.AsSpan(8, 4), StaticMeshCookedFormat.CurrentVersion + 1);
        AssertInvalid(badVersion);

        byte[] unboundedString = valid.ToArray();
        BinaryPrimitives.WriteUInt32LittleEndian(unboundedString.AsSpan(12, 4), uint.MaxValue);
        AssertInvalid(unboundedString);
        AssertInvalid(valid[..^1]);
        AssertInvalid([.. valid, 0xff]);
    }

    [Theory]
    [InlineData("../escape.obj")]
    [InlineData("/absolute.obj")]
    [InlineData("assets\\windows.obj")]
    [InlineData("assets//empty.obj")]
    public void FileEvidenceRejectsNonPortablePaths(string path)
    {
        Assert.Throws<ArgumentException>(() => new StaticAssetFileEvidence(path, new string('0', 64)));
    }

    [Fact]
    public void DescriptorRejectsInvalidScalePolicyAndDuplicateSources()
    {
        StaticAssetFileEvidence primary = File("assets/source.obj");
        Assert.Throws<ArgumentOutOfRangeException>(() => Descriptor(primary, metersPerSourceUnit: 0.0f));
        Assert.Throws<ArgumentException>(() => Descriptor(primary, materialPolicy: "pbr"));
        Assert.Throws<ArgumentException>(() => new StaticAssetCookDescriptor(
            "test-static",
            primary,
            [File("assets/source.obj")],
            "CC0-1.0",
            [File("assets/LICENSE.txt")],
            1.0f,
            StaticAssetCookDescriptor.SectionNamesOnlyMaterialPolicy));
    }

    [Fact]
    public void CookingAssemblyHasNoImporterRendererOrChildDependency()
    {
        string[] references = typeof(StaticMeshCookedFormat).Assembly
            .GetReferencedAssemblies()
            .Select(static assembly => assembly.Name!)
            .ToArray();

        Assert.DoesNotContain(references, name =>
            name.Contains("SimpleMesh", StringComparison.OrdinalIgnoreCase) ||
            name.Contains("SDL", StringComparison.OrdinalIgnoreCase) ||
            name.Contains("Royale", StringComparison.OrdinalIgnoreCase) ||
            name.Contains("Starfall", StringComparison.OrdinalIgnoreCase));
    }

    private static CookedStaticMeshAsset CreateAsset()
    {
        var mesh = new StaticMeshDefinition(
            "two-triangles",
            [
                new StaticVertex(Vector3.Zero, Vector3.UnitZ),
                new StaticVertex(Vector3.UnitX, Vector3.UnitZ),
                new StaticVertex(Vector3.UnitY, Vector3.UnitZ),
                new StaticVertex(Vector3.One, Vector3.UnitZ),
            ],
            [0u, 1u, 2u, 1u, 3u, 2u],
            [
                new StaticMeshSection("first", 0, 3),
                new StaticMeshSection("second", 3, 3),
            ]);
        return new CookedStaticMeshAsset(Descriptor(File("assets/source.obj")), mesh);
    }

    private static StaticAssetCookDescriptor Descriptor(
        StaticAssetFileEvidence primary,
        float metersPerSourceUnit = 0.01f,
        string materialPolicy = StaticAssetCookDescriptor.SectionNamesOnlyMaterialPolicy) => new(
            "test-static",
            primary,
            [File("assets/source.mtl")],
            "CC0-1.0",
            [File("assets/LICENSE.txt")],
            metersPerSourceUnit,
            materialPolicy);

    private static StaticAssetFileEvidence File(string path) => new(path, new string('0', 64));

    private static byte[] Write(CookedStaticMeshAsset asset)
    {
        using var stream = new MemoryStream();
        StaticMeshCookedFormat.Write(stream, asset);
        return stream.ToArray();
    }

    private static void AssertInvalid(byte[] bytes)
    {
        using var stream = new MemoryStream(bytes);
        Assert.Throws<InvalidDataException>(() => StaticMeshCookedFormat.Read(stream));
    }

    private static void AssertFile(StaticAssetFileEvidence expected, StaticAssetFileEvidence actual)
    {
        Assert.Equal(expected.Path, actual.Path);
        Assert.Equal(expected.Sha256, actual.Sha256);
    }

    internal static void AssertMesh(StaticMeshDefinition expected, StaticMeshDefinition actual)
    {
        Assert.Equal(expected.Name, actual.Name);
        Assert.Equal(expected.Vertices, actual.Vertices);
        Assert.Equal(expected.Indices, actual.Indices);
        Assert.Equal(expected.Sections.Count, actual.Sections.Count);
        for (int index = 0; index < expected.Sections.Count; index++)
        {
            Assert.Equal(expected.Sections[index].MaterialName, actual.Sections[index].MaterialName);
            Assert.Equal(expected.Sections[index].StartIndex, actual.Sections[index].StartIndex);
            Assert.Equal(expected.Sections[index].IndexCount, actual.Sections[index].IndexCount);
        }
    }
}
