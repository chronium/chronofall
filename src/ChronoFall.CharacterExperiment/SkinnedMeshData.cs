using System.Numerics;

namespace ChronoFall.CharacterExperiment;

public readonly record struct SkinnedVertex
{
    public SkinnedVertex(
        Vector3 position,
        Vector3 normal,
        Vector2 textureCoordinate,
        SkinInfluences influences)
    {
        if (!DataValidation.IsFinite(position))
            throw new ArgumentException("Position must contain only finite values.", nameof(position));
        if (!DataValidation.IsFinite(normal) || normal.LengthSquared() <= 1e-12f)
            throw new ArgumentException("Normal must have non-zero finite length.", nameof(normal));
        if (!DataValidation.IsFinite(textureCoordinate))
            throw new ArgumentException("Texture coordinate must contain only finite values.", nameof(textureCoordinate));

        Position = position;
        Normal = normal;
        TextureCoordinate = textureCoordinate;
        Influences = influences;
    }

    public Vector3 Position { get; }

    public Vector3 Normal { get; }

    public Vector2 TextureCoordinate { get; }

    public SkinInfluences Influences { get; }

    internal void Validate(SkeletonDefinition skeleton, string parameterName)
    {
        if (!DataValidation.IsFinite(Position))
            throw new ArgumentException("Vertex position must contain only finite values.", parameterName);
        if (!DataValidation.IsFinite(Normal) || Normal.LengthSquared() <= 1e-12f)
            throw new ArgumentException("Vertex normal must have non-zero finite length.", parameterName);
        if (!DataValidation.IsFinite(TextureCoordinate))
            throw new ArgumentException("Vertex texture coordinate must contain only finite values.", parameterName);

        Influences.ValidateForSkeleton(skeleton);
    }
}

public sealed class SkinnedMeshSection
{
    public SkinnedMeshSection(string materialName, int startIndex, int indexCount)
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

public sealed class SkinnedMeshDefinition
{
    public SkinnedMeshDefinition(
        string name,
        SkinDefinition skin,
        IEnumerable<SkinnedVertex> vertices,
        IEnumerable<uint> indices,
        IEnumerable<SkinnedMeshSection> sections)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(skin);
        ArgumentNullException.ThrowIfNull(vertices);
        ArgumentNullException.ThrowIfNull(indices);
        ArgumentNullException.ThrowIfNull(sections);

        SkinnedVertex[] vertexCopy = vertices.ToArray();
        uint[] indexCopy = indices.ToArray();
        SkinnedMeshSection[] sectionCopy = sections.ToArray();
        if (vertexCopy.Length == 0)
            throw new ArgumentException("A skinned mesh must contain vertices.", nameof(vertices));
        if (indexCopy.Length == 0 || indexCopy.Length % 3 != 0)
            throw new ArgumentException("A skinned mesh must contain complete indexed triangles.", nameof(indices));
        if (sectionCopy.Length == 0)
            throw new ArgumentException("A skinned mesh must contain at least one section.", nameof(sections));

        for (int index = 0; index < vertexCopy.Length; index++)
            vertexCopy[index].Validate(skin.Skeleton, nameof(vertices));
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
            SkinnedMeshSection section = sectionCopy[index] ??
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
        Skin = skin;
        Vertices = Array.AsReadOnly(vertexCopy);
        Indices = Array.AsReadOnly(indexCopy);
        Sections = Array.AsReadOnly(sectionCopy);
    }

    public string Name { get; }

    public SkinDefinition Skin { get; }

    public IReadOnlyList<SkinnedVertex> Vertices { get; }

    public IReadOnlyList<uint> Indices { get; }

    public IReadOnlyList<SkinnedMeshSection> Sections { get; }
}

public sealed class SkeletalCharacterAsset
{
    public SkeletalCharacterAsset(SkinnedMeshDefinition mesh, IEnumerable<AnimationClip> animations)
    {
        ArgumentNullException.ThrowIfNull(mesh);
        ArgumentNullException.ThrowIfNull(animations);

        AnimationClip[] animationCopy = animations.ToArray();
        if (animationCopy.Length == 0)
            throw new ArgumentException("A skeletal character asset must contain animations.", nameof(animations));

        var names = new HashSet<string>(StringComparer.Ordinal);
        for (int index = 0; index < animationCopy.Length; index++)
        {
            AnimationClip clip = animationCopy[index] ??
                throw new ArgumentException($"Animation {index} cannot be null.", nameof(animations));
            if (!ReferenceEquals(clip.Skeleton, mesh.Skin.Skeleton))
                throw new ArgumentException($"Animation '{clip.Name}' uses a different skeleton.", nameof(animations));
            if (!names.Add(clip.Name))
                throw new ArgumentException($"Animation name '{clip.Name}' is duplicated.", nameof(animations));
        }

        Mesh = mesh;
        Animations = Array.AsReadOnly(animationCopy);
    }

    public SkinnedMeshDefinition Mesh { get; }

    public IReadOnlyList<AnimationClip> Animations { get; }
}
