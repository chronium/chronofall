using System.Numerics;

namespace ChronoFall.EditorUi.SdlGpu;

public readonly record struct SdlGpuImGuiFrameMetrics
{
    public SdlGpuImGuiFrameMetrics(
        int logicalWidth,
        int logicalHeight,
        int pixelWidth,
        int pixelHeight,
        double deltaSeconds)
    {
        if (logicalWidth <= 0)
            throw new ArgumentOutOfRangeException(nameof(logicalWidth), logicalWidth, "Logical width must be positive.");
        if (logicalHeight <= 0)
            throw new ArgumentOutOfRangeException(nameof(logicalHeight), logicalHeight, "Logical height must be positive.");
        if (pixelWidth <= 0)
            throw new ArgumentOutOfRangeException(nameof(pixelWidth), pixelWidth, "Pixel width must be positive.");
        if (pixelHeight <= 0)
            throw new ArgumentOutOfRangeException(nameof(pixelHeight), pixelHeight, "Pixel height must be positive.");
        if (!double.IsFinite(deltaSeconds) || deltaSeconds <= 0)
            throw new ArgumentOutOfRangeException(nameof(deltaSeconds), deltaSeconds, "Delta time must be positive and finite.");

        LogicalWidth = logicalWidth;
        LogicalHeight = logicalHeight;
        PixelWidth = pixelWidth;
        PixelHeight = pixelHeight;
        DeltaSeconds = deltaSeconds;
    }

    public int LogicalWidth { get; }
    public int LogicalHeight { get; }
    public int PixelWidth { get; }
    public int PixelHeight { get; }
    public double DeltaSeconds { get; }
    public Vector2 FramebufferScale => new(
        PixelWidth / (float)LogicalWidth,
        PixelHeight / (float)LogicalHeight);
}
