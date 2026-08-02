using System.Numerics;

namespace ChronoFall.CharacterPresentation.Tests;

public sealed class StaticMeshDataTests
{
    [Fact]
    public void DefinitionCopiesCallerOwnedCollectionsAndPreservesSectionOrder()
    {
        StaticVertex[] vertices =
        [
            new(Vector3.Zero, Vector3.UnitZ),
            new(Vector3.UnitX, Vector3.UnitZ),
            new(Vector3.UnitY, Vector3.UnitZ),
            new(Vector3.One, Vector3.UnitZ),
        ];
        uint[] indices = [0, 1, 2, 1, 3, 2];
        StaticMeshSection[] sections =
        [
            new("first", 0, 3),
            new("second", 3, 3),
        ];

        var mesh = new StaticMeshDefinition("diagnostic", vertices, indices, sections);
        vertices[0] = new StaticVertex(Vector3.One, Vector3.UnitX);
        indices[0] = 3;
        sections[0] = new StaticMeshSection("replacement", 0, 3);

        Assert.Equal(Vector3.Zero, mesh.Vertices[0].Position);
        Assert.Equal(0u, mesh.Indices[0]);
        Assert.Equal(["first", "second"], mesh.Sections.Select(static section => section.MaterialName));
    }

    [Fact]
    public void DefinitionRejectsInvalidVerticesAndOutOfRangeIndices()
    {
        Assert.Throws<ArgumentException>(() => new StaticMeshDefinition(
            "invalid-vertex",
            [default, new StaticVertex(Vector3.UnitX, Vector3.UnitZ), new StaticVertex(Vector3.UnitY, Vector3.UnitZ)],
            [0, 1, 2],
            [new StaticMeshSection("material", 0, 3)]));

        Assert.Throws<ArgumentOutOfRangeException>(() => new StaticMeshDefinition(
            "invalid-index",
            CreateTriangleVertices(),
            [0, 1, 3],
            [new StaticMeshSection("material", 0, 3)]));
    }

    [Fact]
    public void DefinitionRequiresContiguousCompleteTriangleSections()
    {
        StaticVertex[] vertices = CreateTriangleVertices();
        uint[] indices = [0, 1, 2, 0, 2, 1];

        Assert.Throws<ArgumentException>(() => new StaticMeshDefinition(
            "gap",
            vertices,
            indices,
            [new StaticMeshSection("first", 0, 3), new StaticMeshSection("second", 4, 3)]));
        Assert.Throws<ArgumentException>(() => new StaticMeshDefinition(
            "incomplete",
            vertices,
            indices,
            [new StaticMeshSection("first", 0, 3)]));
        Assert.Throws<ArgumentException>(() => new StaticMeshDefinition(
            "overflow",
            vertices,
            indices,
            [new StaticMeshSection("first", 0, 3), new StaticMeshSection("second", 3, 6)]));
    }

    [Fact]
    public void VertexAndSectionRejectMalformedInputs()
    {
        Assert.Throws<ArgumentException>(() => new StaticVertex(new Vector3(float.NaN, 0, 0), Vector3.UnitY));
        Assert.Throws<ArgumentException>(() => new StaticVertex(Vector3.Zero, Vector3.Zero));
        Assert.Throws<ArgumentException>(() => new StaticMeshSection(" ", 0, 3));
        Assert.Throws<ArgumentOutOfRangeException>(() => new StaticMeshSection("material", -1, 3));
        Assert.Throws<ArgumentOutOfRangeException>(() => new StaticMeshSection("material", 0, 4));
    }

    private static StaticVertex[] CreateTriangleVertices() =>
    [
        new(Vector3.Zero, Vector3.UnitZ),
        new(Vector3.UnitX, Vector3.UnitZ),
        new(Vector3.UnitY, Vector3.UnitZ),
    ];
}
