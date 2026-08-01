using System.Numerics;
using SDL;

namespace ChronoFall.CharacterExperiment.SdlGpu.Tests;

public sealed class SdlGpuHarnessPolicyTests
{
    [Fact]
    public void WindowFlagsKeepVisibleAndHiddenHarnessesFixedSize()
    {
        SDL_WindowFlags visible = SdlGpuCharacterHarness.SelectWindowFlags(visible: true);
        SDL_WindowFlags hidden = SdlGpuCharacterHarness.SelectWindowFlags(visible: false);

        Assert.Equal((SDL_WindowFlags)0, visible);
        Assert.Equal(SDL_WindowFlags.SDL_WINDOW_HIDDEN, hidden);
        Assert.Equal((SDL_WindowFlags)0, visible & SDL_WindowFlags.SDL_WINDOW_RESIZABLE);
        Assert.Equal((SDL_WindowFlags)0, hidden & SDL_WindowFlags.SDL_WINDOW_RESIZABLE);
    }

    [Fact]
    public void InteractiveFrameDiagnosticAllowsSuccessfulOperation()
    {
        AnimationClip clip = CreateClip("Walk_Loop");
        int executions = 0;

        SdlGpuCharacterHarness.ExecuteInteractiveFrame(clip, 0.5f, 1, () => executions++);

        Assert.Equal(1, executions);
    }

    [Fact]
    public void InteractiveFrameDiagnosticPreservesLateGpuFailureAndContext()
    {
        AnimationClip clip = CreateClip("Walk_Loop");
        var failure = new InvalidOperationException("SDL GPU visible submission failed: sentinel");

        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() =>
            SdlGpuCharacterHarness.ExecuteInteractiveFrame(clip, 0.5f, 1, () => throw failure));

        Assert.Equal(
            "Interactive animation validation failed for clip 'Walk_Loop' at sample 0.500 seconds (joints=1).",
            exception.Message);
        Assert.Same(failure, exception.InnerException);
        InvalidOperationException preserved = Assert.IsType<InvalidOperationException>(exception.InnerException);
        Assert.Contains("visible submission", preserved.Message, StringComparison.Ordinal);
    }

    private static AnimationClip CreateClip(string name)
    {
        var skeleton = new SkeletonDefinition([new SkeletonJoint("root", -1, JointTransform.Identity)]);
        return new AnimationClip(
            name,
            skeleton,
            [
                new JointAnimationTrack(
                    0,
                    new Vector3AnimationChannel([
                        new Vector3Keyframe(0.0f, Vector3.Zero),
                        new Vector3Keyframe(1.0f, Vector3.UnitX),
                    ]),
                    new QuaternionAnimationChannel([new QuaternionKeyframe(0.0f, Quaternion.Identity)]),
                    new Vector3AnimationChannel([new Vector3Keyframe(0.0f, Vector3.One)])),
            ]);
    }
}
