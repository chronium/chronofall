namespace ChronoFall.EditorUi.SdlGpu.Tests;

public sealed class BackendFrameStateMachineTests
{
    [Fact]
    public void RenderedFrameUsesTheRequiredCallerControlledSequence()
    {
        var state = new BackendFrameStateMachine();

        state.Begin();
        state.Prepare();
        state.CompletePrepared();

        Assert.Equal(BackendFrameState.Idle, state.State);
    }

    [Fact]
    public void PreparedFrameCanBeDiscardedAndNextFrameCanBegin()
    {
        var state = new BackendFrameStateMachine();

        state.Begin();
        state.Prepare();
        state.CompletePrepared();
        state.Begin();
        state.EndWithoutRendering();

        Assert.Equal(BackendFrameState.Idle, state.State);
    }

    [Fact]
    public void CallOrderViolationsFailBeforeNativeSubmission()
    {
        var state = new BackendFrameStateMachine();

        Assert.Throws<InvalidOperationException>(state.EnsureCanPrepare);
        state.Begin();
        Assert.Throws<InvalidOperationException>(state.EnsureCanBegin);
        Assert.Throws<InvalidOperationException>(state.EnsureCanCompletePrepared);
    }

    [Fact]
    public void DisposedStateAlwaysFailsExplicitly()
    {
        var state = new BackendFrameStateMachine();
        state.Dispose();

        Assert.Throws<ObjectDisposedException>(state.EnsureCanBegin);
        Assert.Throws<ObjectDisposedException>(state.EnsureCanPrepare);
    }
}
