using System.Numerics;

namespace ChronoFall.CharacterPresentation;

public readonly record struct StaticVertex
{
    public StaticVertex(Vector3 position, Vector3 normal)
    {
        Validate(position, normal, nameof(position));
        Position = position;
        Normal = normal;
    }

    public Vector3 Position { get; }

    public Vector3 Normal { get; }

    internal void Validate(string parameterName) => Validate(Position, Normal, parameterName);

    private static void Validate(Vector3 position, Vector3 normal, string parameterName)
    {
        if (!DataValidation.IsFinite(position))
            throw new ArgumentException("Position must contain only finite values.", parameterName);
        if (!DataValidation.IsFinite(normal) || normal.LengthSquared() <= 1e-12f)
            throw new ArgumentException("Normal must have non-zero finite length.", parameterName);
    }
}

public sealed class StaticMeshSection
{
    public StaticMeshSection(string materialName, int startIndex, int indexCount)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(materialName);
        if (startIndex < 0)
            throw new ArgumentOutOfRangeException(nameof(startIndex));
        if (indexCount <= 0 || indexCount % 3 != 0)
            throw new ArgumentOutOfRangeException(nameof(indexCount), "A mesh section must contain complete triangles.");

        MaterialName = materialName;
        StartIndex = startIndex;
        IndexCount = indexCount;
    }

    public string MaterialName { get; }

    public int StartIndex { get; }

    public int IndexCount { get; }
}

public sealed class StaticMeshDefinition
{
    public StaticMeshDefinition(
        string name,
        IEnumerable<StaticVertex> vertices,
        IEnumerable<uint> indices,
        IEnumerable<StaticMeshSection> sections)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(vertices);
        ArgumentNullException.ThrowIfNull(indices);
        ArgumentNullException.ThrowIfNull(sections);

        StaticVertex[] vertexCopy = vertices.ToArray();
        uint[] indexCopy = indices.ToArray();
        StaticMeshSection[] sectionCopy = sections.ToArray();
        if (vertexCopy.Length == 0)
            throw new ArgumentException("A static mesh must contain vertices.", nameof(vertices));
        if (indexCopy.Length == 0 || indexCopy.Length % 3 != 0)
            throw new ArgumentException("A static mesh must contain complete indexed triangles.", nameof(indices));
        if (sectionCopy.Length == 0)
            throw new ArgumentException("A static mesh must contain at least one section.", nameof(sections));

        for (int index = 0; index < vertexCopy.Length; index++)
            vertexCopy[index].Validate(nameof(vertices));
        for (int index = 0; index < indexCopy.Length; index++)
        {
            if (indexCopy[index] >= vertexCopy.Length)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(indices),
                    $"Index {index} references vertex {indexCopy[index]}, but the mesh has {vertexCopy.Length} vertices.");
            }
        }

        int expectedStart = 0;
        for (int index = 0; index < sectionCopy.Length; index++)
        {
            StaticMeshSection section = sectionCopy[index] ??
                throw new ArgumentException($"Section {index} cannot be null.", nameof(sections));
            if (section.StartIndex != expectedStart)
            {
                throw new ArgumentException(
                    $"Section {index} must start at index {expectedStart}, but starts at {section.StartIndex}.",
                    nameof(sections));
            }

            expectedStart = checked(expectedStart + section.IndexCount);
            if (expectedStart > indexCopy.Length)
                throw new ArgumentException($"Section {index} exceeds the mesh index buffer.", nameof(sections));
        }

        if (expectedStart != indexCopy.Length)
            throw new ArgumentException("Mesh sections must cover the complete index buffer.", nameof(sections));

        Name = name;
        Vertices = Array.AsReadOnly(vertexCopy);
        Indices = Array.AsReadOnly(indexCopy);
        Sections = Array.AsReadOnly(sectionCopy);
    }

    public string Name { get; }

    public IReadOnlyList<StaticVertex> Vertices { get; }

    public IReadOnlyList<uint> Indices { get; }

    public IReadOnlyList<StaticMeshSection> Sections { get; }
}
