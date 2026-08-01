using System.Numerics;
using System.Runtime.InteropServices;

namespace ChronoFall.CharacterPresentation.SdlGpu;

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

internal readonly record struct GpuMeshSection(uint FirstIndex, uint IndexCount);

internal static class GpuSkinningData
{
    internal static GpuSkinnedVertex[] CreateVertices(SkinnedMeshDefinition mesh)
    {
        var vertices = new GpuSkinnedVertex[mesh.Vertices.Count];
        for (int index = 0; index < vertices.Length; index++)
            vertices[index] = new GpuSkinnedVertex(mesh.Vertices[index]);
        return vertices;
    }

    internal static GpuMeshSection[] CreateSections(SkinnedMeshDefinition mesh) =>
        mesh.Sections
            .Select(static section => new GpuMeshSection(
                checked((uint)section.StartIndex),
                checked((uint)section.IndexCount)))
            .ToArray();

    internal static Matrix4x4[] PackPalette(SkinningPalette palette)
    {
        var packed = new Matrix4x4[palette.JointMatrices.Count];
        for (int index = 0; index < packed.Length; index++)
            packed[index] = Matrix4x4.Transpose(palette.JointMatrices[index]);
        return packed;
    }
}
