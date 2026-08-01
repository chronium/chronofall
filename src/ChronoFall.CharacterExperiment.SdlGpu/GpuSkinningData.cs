using System.Numerics;
using System.Runtime.InteropServices;

namespace ChronoFall.CharacterExperiment.SdlGpu;

[StructLayout(LayoutKind.Sequential, Pack = 4)]
internal readonly struct GpuSkinnedVertex
{
    internal const int PositionOffset = 0;
    internal const int NormalOffset = 12;
    internal const int JointIndicesOffset = 24;
    internal const int WeightsOffset = 32;
    internal const int Stride = 48;

    internal GpuSkinnedVertex(SkinnedVertex source)
    {
        Position = source.Position;
        Normal = source.Normal;
        Joint0 = checked((ushort)source.Influences.Joints.X);
        Joint1 = checked((ushort)source.Influences.Joints.Y);
        Joint2 = checked((ushort)source.Influences.Joints.Z);
        Joint3 = checked((ushort)source.Influences.Joints.W);
        Weights = source.Influences.Weights;
    }

    internal readonly Vector3 Position;
    internal readonly Vector3 Normal;
    internal readonly ushort Joint0;
    internal readonly ushort Joint1;
    internal readonly ushort Joint2;
    internal readonly ushort Joint3;
    internal readonly Vector4 Weights;
}

internal readonly record struct GpuMeshSection(string MaterialName, uint FirstIndex, uint IndexCount);

internal sealed class GpuSkinnedMeshData
{
    private GpuSkinnedMeshData(
        GpuSkinnedVertex[] vertices,
        uint[] indices,
        GpuMeshSection[] sections,
        MeshBounds bounds,
        int jointCount)
    {
        Vertices = vertices;
        Indices = indices;
        Sections = sections;
        Bounds = bounds;
        JointCount = jointCount;
    }

    internal GpuSkinnedVertex[] Vertices { get; }
    internal uint[] Indices { get; }
    internal GpuMeshSection[] Sections { get; }
    internal MeshBounds Bounds { get; }
    internal int JointCount { get; }

    internal static GpuSkinnedMeshData Create(SkinnedMeshDefinition mesh)
    {
        ArgumentNullException.ThrowIfNull(mesh);

        var vertices = new GpuSkinnedVertex[mesh.Vertices.Count];
        var positions = new Vector3[mesh.Vertices.Count];
        for (int index = 0; index < vertices.Length; index++)
        {
            vertices[index] = new GpuSkinnedVertex(mesh.Vertices[index]);
            positions[index] = mesh.Vertices[index].Position;
        }

        uint[] indices = mesh.Indices.ToArray();
        GpuMeshSection[] sections = mesh.Sections
            .Select(static section => new GpuMeshSection(
                section.MaterialName,
                checked((uint)section.StartIndex),
                checked((uint)section.IndexCount)))
            .ToArray();

        return new GpuSkinnedMeshData(
            vertices,
            indices,
            sections,
            MeshBounds.Create(positions),
            mesh.Skin.Skeleton.JointCount);
    }
}

internal readonly record struct MeshBounds(Vector3 Minimum, Vector3 Maximum)
{
    internal Vector3 Center => (Minimum + Maximum) * 0.5f;
    internal Vector3 Extents => (Maximum - Minimum) * 0.5f;
    internal float Radius => Extents.Length();

    internal static MeshBounds Create(IReadOnlyList<Vector3> positions)
    {
        ArgumentNullException.ThrowIfNull(positions);
        if (positions.Count == 0)
            throw new ArgumentException("Mesh bounds require at least one position.", nameof(positions));

        Vector3 minimum = positions[0];
        Vector3 maximum = positions[0];
        for (int index = 1; index < positions.Count; index++)
        {
            minimum = Vector3.Min(minimum, positions[index]);
            maximum = Vector3.Max(maximum, positions[index]);
        }

        return new MeshBounds(minimum, maximum);
    }
}

internal static class GpuMatrixPacking
{
    internal static Matrix4x4[] PackTransposed(SkinningPalette palette, Matrix4x4? postTransform = null)
    {
        ArgumentNullException.ThrowIfNull(palette);
        Matrix4x4 transform = postTransform ?? Matrix4x4.Identity;
        var packed = new Matrix4x4[palette.JointMatrices.Count];
        for (int index = 0; index < packed.Length; index++)
            packed[index] = Matrix4x4.Transpose(palette.JointMatrices[index] * transform);
        return packed;
    }
}
