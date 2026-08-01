using System.Numerics;
using System.Runtime.InteropServices;

namespace ChronoFall.CharacterExperiment.SdlGpu;

[StructLayout(LayoutKind.Sequential, Pack = 4)]
internal readonly struct GpuDebugLineVertex
{
    internal const int PositionOffset = 0;
    internal const int ColorOffset = 12;
    internal const int Stride = 28;

    internal GpuDebugLineVertex(Vector3 position, Vector4 color)
    {
        Position = position;
        Color = color;
    }

    internal readonly Vector3 Position;
    internal readonly Vector4 Color;
}

internal sealed class SkeletonDebugGeometry
{
    internal static readonly Vector4 LinkColor = new(1.0f, 0.85f, 0.15f, 1.0f);
    internal static readonly Vector4 XAxisColor = new(1.0f, 0.10f, 0.10f, 1.0f);
    internal static readonly Vector4 YAxisColor = new(0.10f, 1.0f, 0.10f, 1.0f);
    internal static readonly Vector4 ZAxisColor = new(0.15f, 0.45f, 1.0f, 1.0f);

    private SkeletonDebugGeometry(GpuDebugLineVertex[] vertices, int linkCount, int axisCount)
    {
        Vertices = vertices;
        LinkCount = linkCount;
        AxisCount = axisCount;
    }

    internal GpuDebugLineVertex[] Vertices { get; }
    internal int LinkCount { get; }
    internal int AxisCount { get; }
    internal int LineCount => LinkCount + AxisCount;

    internal static SkeletonDebugGeometry Create(SkeletonGlobalPose pose, float axisLength)
    {
        ArgumentNullException.ThrowIfNull(pose);
        if (!float.IsFinite(axisLength) || axisLength <= 0.0f)
            throw new ArgumentOutOfRangeException(nameof(axisLength), "Joint-axis length must be positive and finite.");

        int linkCount = pose.Skeleton.JointCount - 1;
        int axisCount = checked(pose.Skeleton.JointCount * 3);
        var vertices = new GpuDebugLineVertex[checked((linkCount + axisCount) * 2)];
        int vertexIndex = 0;

        for (int jointIndex = 1; jointIndex < pose.Skeleton.JointCount; jointIndex++)
        {
            int parentIndex = pose.Skeleton.Joints[jointIndex].ParentIndex;
            AddLine(
                vertices,
                ref vertexIndex,
                GetOrigin(pose.GlobalTransforms[parentIndex]),
                GetOrigin(pose.GlobalTransforms[jointIndex]),
                LinkColor);
        }

        for (int jointIndex = 0; jointIndex < pose.Skeleton.JointCount; jointIndex++)
        {
            Matrix4x4 transform = pose.GlobalTransforms[jointIndex];
            Vector3 origin = GetOrigin(transform);
            AddLine(vertices, ref vertexIndex, origin, Vector3.Transform(Vector3.UnitX * axisLength, transform), XAxisColor);
            AddLine(vertices, ref vertexIndex, origin, Vector3.Transform(Vector3.UnitY * axisLength, transform), YAxisColor);
            AddLine(vertices, ref vertexIndex, origin, Vector3.Transform(Vector3.UnitZ * axisLength, transform), ZAxisColor);
        }

        return new SkeletonDebugGeometry(vertices, linkCount, axisCount);
    }

    private static Vector3 GetOrigin(Matrix4x4 transform) =>
        new(transform.M41, transform.M42, transform.M43);

    private static void AddLine(
        GpuDebugLineVertex[] vertices,
        ref int vertexIndex,
        Vector3 start,
        Vector3 end,
        Vector4 color)
    {
        if (!IsFinite(start) || !IsFinite(end))
            throw new ArgumentException("Skeleton debug geometry produced a non-finite line endpoint.");

        vertices[vertexIndex++] = new GpuDebugLineVertex(start, color);
        vertices[vertexIndex++] = new GpuDebugLineVertex(end, color);
    }

    private static bool IsFinite(Vector3 value) =>
        float.IsFinite(value.X) && float.IsFinite(value.Y) && float.IsFinite(value.Z);
}
