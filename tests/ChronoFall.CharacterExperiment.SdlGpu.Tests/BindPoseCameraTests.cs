using System.Numerics;

namespace ChronoFall.CharacterExperiment.SdlGpu.Tests;

public sealed class BindPoseCameraTests
{
    [Fact]
    public void CameraFramingIsDeterministicAndTargetsBoundsCenter()
    {
        var bounds = new MeshBounds(new Vector3(-0.5f, 0.0f, -0.25f), new Vector3(0.5f, 1.8f, 0.25f));

        BindPoseCamera first = BindPoseCamera.Create(bounds, 512, 512);
        BindPoseCamera second = BindPoseCamera.Create(bounds, 512, 512);

        Assert.Equal(bounds.Center, first.Target);
        Assert.Equal(first, second);
        Assert.Equal(Matrix4x4.Transpose(first.ViewProjection), first.TransposedViewProjection);
        Assert.True(Vector3.Distance(first.Position, first.Target) > bounds.Radius);
    }
}
