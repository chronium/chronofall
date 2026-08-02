using System.Numerics;
using System.Runtime.InteropServices;

namespace ChronoFall.CharacterPresentation.SdlGpu;

[StructLayout(LayoutKind.Sequential, Pack = 4)]
internal readonly struct GpuStaticVertex
{
    internal const int PositionOffset = 0;
    internal const int NormalOffset = 12;
    internal const int Stride = 24;

    internal GpuStaticVertex(StaticVertex source)
    {
        Position = source.Position;
        Normal = source.Normal;
    }

    internal readonly Vector3 Position;
    internal readonly Vector3 Normal;
}

internal static class GpuStaticMeshData
{
    internal static GpuStaticVertex[] CreateVertices(StaticMeshDefinition mesh)
    {
        var vertices = new GpuStaticVertex[mesh.Vertices.Count];
        for (int index = 0; index < vertices.Length; index++)
            vertices[index] = new GpuStaticVertex(mesh.Vertices[index]);
        return vertices;
    }

    internal static GpuMeshSection[] CreateSections(StaticMeshDefinition mesh) =>
        mesh.Sections
            .Select(static section => new GpuMeshSection(
                checked((uint)section.StartIndex),
                checked((uint)section.IndexCount)))
            .ToArray();
}
