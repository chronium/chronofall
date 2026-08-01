using System.Numerics;
using System.Runtime.InteropServices;
using ChronoFall.CharacterExperiment.SimpleMesh;

namespace ChronoFall.CharacterExperiment.SdlGpu.Tests;

public sealed class SkeletonDebugGeometryTests
{
    [Fact]
    public void DebugVertexAbiHasExpectedStrideAndOffsets()
    {
        Assert.Equal(28, Marshal.SizeOf<GpuDebugLineVertex>());
        Assert.Equal(0, Marshal.OffsetOf<GpuDebugLineVertex>(nameof(GpuDebugLineVertex.Position)).ToInt32());
        Assert.Equal(12, Marshal.OffsetOf<GpuDebugLineVertex>(nameof(GpuDebugLineVertex.Color)).ToInt32());
    }

    [Fact]
    public void TwoJointPoseProducesOneLinkAndThreeAxesPerJoint()
    {
        var skeleton = new SkeletonDefinition([
            new SkeletonJoint(
                "root",
                -1,
                new JointTransform(
                    new Vector3(1.0f, 2.0f, 3.0f),
                    Quaternion.CreateFromAxisAngle(Vector3.UnitZ, MathF.PI * 0.5f),
                    Vector3.One)),
            new SkeletonJoint(
                "child",
                0,
                new JointTransform(Vector3.UnitX, Quaternion.Identity, Vector3.One)),
        ]);
        SkeletonGlobalPose pose = SkeletonPoseEvaluator.EvaluateGlobal(skeleton.CreateBindPose());

        SkeletonDebugGeometry geometry = SkeletonDebugGeometry.Create(pose, axisLength: 0.5f);

        Assert.Equal(1, geometry.LinkCount);
        Assert.Equal(6, geometry.AxisCount);
        Assert.Equal(7, geometry.LineCount);
        Assert.Equal(14, geometry.Vertices.Length);

        AssertVertex(geometry.Vertices[0], new Vector3(1.0f, 2.0f, 3.0f), SkeletonDebugGeometry.LinkColor);
        AssertVertex(geometry.Vertices[1], new Vector3(1.0f, 3.0f, 3.0f), SkeletonDebugGeometry.LinkColor);

        AssertVertex(geometry.Vertices[2], new Vector3(1.0f, 2.0f, 3.0f), SkeletonDebugGeometry.XAxisColor);
        AssertVertex(geometry.Vertices[3], new Vector3(1.0f, 2.5f, 3.0f), SkeletonDebugGeometry.XAxisColor);
        AssertVertex(geometry.Vertices[4], new Vector3(1.0f, 2.0f, 3.0f), SkeletonDebugGeometry.YAxisColor);
        AssertVertex(geometry.Vertices[5], new Vector3(0.5f, 2.0f, 3.0f), SkeletonDebugGeometry.YAxisColor);
        AssertVertex(geometry.Vertices[6], new Vector3(1.0f, 2.0f, 3.0f), SkeletonDebugGeometry.ZAxisColor);
        AssertVertex(geometry.Vertices[7], new Vector3(1.0f, 2.0f, 3.5f), SkeletonDebugGeometry.ZAxisColor);
    }

    [Fact]
    public void SelectedAssetProducesCompleteFiniteSkeletonOverlay()
    {
        string root = FindRepositoryRoot();
        SkeletalCharacterAsset asset = SimpleMeshSkeletalAssetLoader.LoadFromFile(Path.Combine(
            root,
            "assets",
            "Quaternius",
            "Universal Animation Library[Standard]",
            "Unreal-Godot",
            "UAL1_Standard.glb"));
        SkeletonGlobalPose pose = SkeletonPoseEvaluator.EvaluateGlobal(asset.Mesh.Skin.Skeleton.CreateBindPose());

        SkeletonDebugGeometry geometry = SkeletonDebugGeometry.Create(pose, axisLength: 0.04f);

        Assert.Equal(64, geometry.LinkCount);
        Assert.Equal(195, geometry.AxisCount);
        Assert.Equal(259, geometry.LineCount);
        Assert.Equal(518, geometry.Vertices.Length);
        Assert.All(geometry.Vertices, static vertex =>
        {
            AssertFinite(vertex.Position);
            AssertFinite(vertex.Color);
        });
    }

    private static void AssertVertex(GpuDebugLineVertex actual, Vector3 expectedPosition, Vector4 expectedColor)
    {
        AssertVector(actual.Position, expectedPosition);
        Assert.Equal(expectedColor, actual.Color);
    }

    private static void AssertVector(Vector3 actual, Vector3 expected)
    {
        Assert.InRange(actual.X, expected.X - 0.0001f, expected.X + 0.0001f);
        Assert.InRange(actual.Y, expected.Y - 0.0001f, expected.Y + 0.0001f);
        Assert.InRange(actual.Z, expected.Z - 0.0001f, expected.Z + 0.0001f);
    }

    private static void AssertFinite(Vector3 value)
    {
        Assert.True(float.IsFinite(value.X));
        Assert.True(float.IsFinite(value.Y));
        Assert.True(float.IsFinite(value.Z));
    }

    private static void AssertFinite(Vector4 value)
    {
        Assert.True(float.IsFinite(value.X));
        Assert.True(float.IsFinite(value.Y));
        Assert.True(float.IsFinite(value.Z));
        Assert.True(float.IsFinite(value.W));
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
        throw new DirectoryNotFoundException("Could not find ChronoFall.slnx from the test output directory.");
    }
}
