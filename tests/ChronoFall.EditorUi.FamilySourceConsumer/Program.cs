using ChronoFall.EditorUi.SdlGpu;

SdlGpuImGuiFrameMetrics metrics = new(640, 360, 1280, 720, 1.0 / 60.0);
Console.WriteLine(FormattableString.Invariant(
    $"ChronoFall Editor UI family-source smoke scale: {metrics.FramebufferScale.X:0.0}x{metrics.FramebufferScale.Y:0.0}"));
