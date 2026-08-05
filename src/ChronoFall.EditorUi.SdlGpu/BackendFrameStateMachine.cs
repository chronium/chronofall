namespace ChronoFall.EditorUi.SdlGpu;

internal sealed class BackendFrameStateMachine
{
    private BackendFrameState state;

    internal BackendFrameState State => state;

    internal void EnsureCanBegin() => Require(BackendFrameState.Idle, "begin a frame");

    internal void EnsureCanPrepare() => Require(BackendFrameState.Building, "prepare draw data");

    internal void EnsureCanCompletePrepared() => Require(BackendFrameState.Prepared, "complete prepared draw data");

    internal void EnsureCanEndWithoutRendering() => Require(BackendFrameState.Building, "end an unrendered frame");

    internal void Begin()
    {
        EnsureCanBegin();
        state = BackendFrameState.Building;
    }

    internal void Prepare()
    {
        EnsureCanPrepare();
        state = BackendFrameState.Prepared;
    }

    internal void CompletePrepared()
    {
        EnsureCanCompletePrepared();
        state = BackendFrameState.Idle;
    }

    internal void EndWithoutRendering()
    {
        EnsureCanEndWithoutRendering();
        state = BackendFrameState.Idle;
    }

    internal void Dispose() => state = BackendFrameState.Disposed;

    private void Require(BackendFrameState required, string operation)
    {
        if (state == BackendFrameState.Disposed)
            throw new ObjectDisposedException(nameof(SdlGpuImGuiBackend));
        if (state != required)
            throw new InvalidOperationException($"Cannot {operation} while the ImGui backend is in state '{state}'.");
    }
}

internal enum BackendFrameState
{
    Idle,
    Building,
    Prepared,
    Disposed,
}
