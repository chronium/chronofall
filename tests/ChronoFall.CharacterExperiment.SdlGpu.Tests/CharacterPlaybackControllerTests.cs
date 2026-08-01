using System.Numerics;

namespace ChronoFall.CharacterExperiment.SdlGpu.Tests;

public sealed class CharacterPlaybackControllerTests
{
    [Fact]
    public void NavigationAndControlsAreDeterministic()
    {
        SkeletonDefinition skeleton = CreateSkeleton();
        AnimationClip[] clips =
        [
            CreateClip("A", skeleton, 2.0f),
            CreateClip("B", skeleton, 2.0f),
            CreateClip("C", skeleton, 2.0f),
        ];
        var controller = new CharacterPlaybackController(clips, "B");

        Assert.Equal(1, controller.CurrentClipIndex);
        Assert.Equal("B", controller.CurrentClip.Name);
        controller.Advance(2.5);
        Assert.Equal(0.5f, controller.SampleTime);

        controller.TogglePlaying();
        controller.Advance(1.0);
        Assert.False(controller.IsPlaying);
        Assert.Equal(0.5f, controller.SampleTime);

        controller.SelectNext();
        Assert.Equal("C", controller.CurrentClip.Name);
        Assert.Equal(0.0f, controller.SampleTime);
        Assert.False(controller.IsPlaying);
        controller.SelectNext();
        Assert.Equal("A", controller.CurrentClip.Name);
        controller.SelectPrevious();
        Assert.Equal("C", controller.CurrentClip.Name);

        controller.SelectByName("B");
        controller.TogglePlaying();
        controller.Advance(0.75);
        controller.Restart();
        controller.ToggleSkeleton();

        Assert.Equal("B", controller.CurrentClip.Name);
        Assert.Equal(0.0f, controller.SampleTime);
        Assert.True(controller.IsPlaying);
        Assert.True(controller.IsSkeletonVisible);
    }

    [Fact]
    public void DiagnosticsUseInvariantCompleteState()
    {
        SkeletonDefinition skeleton = CreateSkeleton();
        var controller = new CharacterPlaybackController(
            [CreateClip("Idle_Loop", skeleton, 2.5f), CreateClip("Walk_Loop", skeleton, 4.0f / 3.0f)],
            "Walk_Loop");
        controller.Advance(0.5);

        Assert.Equal(
            "ChronoFall Character Experiment | 2/2 Walk_Loop | 0.500/1.333 s | playing | skeleton off | joints 1 | palette 1",
            controller.CreateWindowTitle(1, 1));
        Assert.Equal(
            "GPU_HARNESS_DIAGNOSTIC clip=Walk_Loop index=2/2 sample=0.500 duration=1.333 state=playing skeleton=off joints=1 palette=1",
            controller.CreateConsoleDiagnostic(1, 1));
    }

    [Fact]
    public void ValidationErrorsIdentifyTheOffendingContract()
    {
        SkeletonDefinition first = CreateSkeleton();
        SkeletonDefinition second = CreateSkeleton();

        ArgumentException mismatch = Assert.Throws<ArgumentException>(() =>
            new CharacterPlaybackController(
                [CreateClip("First", first, 1.0f), CreateClip("Second", second, 1.0f)],
                "First"));
        Assert.Contains("Second", mismatch.Message, StringComparison.Ordinal);

        ArgumentException missing = Assert.Throws<ArgumentException>(() =>
            new CharacterPlaybackController([CreateClip("Available", first, 1.0f)], "Missing"));
        Assert.Contains("Available", missing.Message, StringComparison.Ordinal);

        var controller = new CharacterPlaybackController([CreateClip("Available", first, 1.0f)], "Available");
        InvalidOperationException palette = Assert.Throws<InvalidOperationException>(() =>
            controller.CreateWindowTitle(1, 2));
        Assert.Contains("Available", palette.Message, StringComparison.Ordinal);
        Assert.Contains("2 palette matrices", palette.Message, StringComparison.Ordinal);
    }

    [Theory]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    [InlineData(-0.01)]
    public void AdvanceRejectsInvalidElapsedTime(double elapsedSeconds)
    {
        SkeletonDefinition skeleton = CreateSkeleton();
        var controller = new CharacterPlaybackController([CreateClip("Clip", skeleton, 1.0f)], "Clip");

        Assert.Throws<ArgumentOutOfRangeException>(() => controller.Advance(elapsedSeconds));
    }

    private static SkeletonDefinition CreateSkeleton() =>
        new([new SkeletonJoint("root", -1, JointTransform.Identity)]);

    private static AnimationClip CreateClip(string name, SkeletonDefinition skeleton, float duration) =>
        new(
            name,
            skeleton,
            [
                new JointAnimationTrack(
                    0,
                    new Vector3AnimationChannel([
                        new Vector3Keyframe(0.0f, Vector3.Zero),
                        new Vector3Keyframe(duration, Vector3.UnitX),
                    ]),
                    new QuaternionAnimationChannel([new QuaternionKeyframe(0.0f, Quaternion.Identity)]),
                    new Vector3AnimationChannel([new Vector3Keyframe(0.0f, Vector3.One)])),
            ]);
}
