using System.Numerics;

namespace ChronoFall.EditorUi.SdlGpu.Tests;

public sealed class FrameMetricsTests
{
    [Fact]
    public void FramebufferScaleUsesCallerLogicalAndPixelDimensions()
    {
        var metrics = new SdlGpuImGuiFrameMetrics(1280, 800, 2560, 1600, 1.0 / 60.0);

        Assert.Equal(new Vector2(2, 2), metrics.FramebufferScale);
        Assert.Equal(1.0 / 60.0, metrics.DeltaSeconds);
    }

    [Theory]
    [InlineData(0, 800, 1280, 800, 0.016)]
    [InlineData(1280, 0, 1280, 800, 0.016)]
    [InlineData(1280, 800, 0, 800, 0.016)]
    [InlineData(1280, 800, 1280, 0, 0.016)]
    [InlineData(1280, 800, 1280, 800, 0)]
    [InlineData(1280, 800, 1280, 800, double.NaN)]
    [InlineData(1280, 800, 1280, 800, double.PositiveInfinity)]
    public void InvalidFrameMetricsFailExplicitly(
        int logicalWidth,
        int logicalHeight,
        int pixelWidth,
        int pixelHeight,
        double deltaSeconds)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new SdlGpuImGuiFrameMetrics(logicalWidth, logicalHeight, pixelWidth, pixelHeight, deltaSeconds));
    }
}
