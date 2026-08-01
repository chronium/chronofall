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
            "ChronoFall Character Experiment | 2/2 Walk_Loop | 0.500/1.333 s | playing | direct 1.00 | skeleton off | joints 1 | palette 1",
            controller.CreateWindowTitle(1, 1));
        Assert.Equal(
            "GPU_HARNESS_DIAGNOSTIC clip=Walk_Loop index=2/2 sample=0.500 duration=1.333 state=playing phase=direct blend=1.000 skeleton=off joints=1 palette=1",
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

    [Fact]
    public void LocomotionRequestCrossfadesFromCurrentPoseToAdvancingDestination()
    {
        SkeletonDefinition skeleton = CreateSkeleton();
        AnimationClip idle = CreateConstantClip("Idle_Loop", skeleton, 2.0f, 2.0f);
        AnimationClip walk = CreateLinearClip("Walk_Loop", skeleton, 2.0f, 0.0f, 8.0f);
        var controller = new CharacterPlaybackController([idle, walk], idle.Name);
        controller.Advance(0.5);

        controller.RequestLocomotion(walk.Name);

        Assert.Equal(CharacterPlaybackPhase.LocomotionBlend, controller.Phase);
        Assert.Equal(0.0f, controller.BlendAmount);
        Assert.Equal(2.0f, controller.CreatePose().LocalTransforms[0].Translation.X);

        controller.Advance(CharacterPlaybackController.LocomotionBlendDuration * 0.5f);

        Assert.Equal(0.5f, controller.BlendAmount);
        Assert.Equal(1.25f, controller.CreatePose().LocalTransforms[0].Translation.X, precision: 5);

        controller.Advance(CharacterPlaybackController.LocomotionBlendDuration * 0.5f);

        Assert.Equal(CharacterPlaybackPhase.Locomotion, controller.Phase);
        Assert.Equal(1.0f, controller.CreatePose().LocalTransforms[0].Translation.X, precision: 5);
    }

    [Fact]
    public void RequestingCurrentDirectClipPreservesItsPhase()
    {
        SkeletonDefinition skeleton = CreateSkeleton();
        AnimationClip walk = CreateLinearClip("Walk_Loop", skeleton, 2.0f, 0.0f, 2.0f);
        var controller = new CharacterPlaybackController([walk], walk.Name);
        controller.Advance(0.75);
        SkeletonPose before = controller.CreatePose();

        controller.RequestLocomotion(walk.Name);

        Assert.Equal(CharacterPlaybackPhase.Locomotion, controller.Phase);
        Assert.Equal(0.75f, controller.SampleTime);
        Assert.Equal(
            before.LocalTransforms[0].Translation,
            controller.CreatePose().LocalTransforms[0].Translation);
    }

    [Fact]
    public void ActionSignalBlendsInPlaysOnceAndReturnsToAdvancingLocomotion()
    {
        SkeletonDefinition skeleton = CreateSkeleton();
        AnimationClip walk = CreateLinearClip("Walk_Loop", skeleton, 2.0f, 0.0f, 2.0f);
        AnimationClip attack = CreateLinearClip("Sword_Attack", skeleton, 1.5f, 10.0f, 25.0f);
        var controller = new CharacterPlaybackController([walk, attack], walk.Name);
        controller.RequestLocomotion(walk.Name);
        controller.Advance(0.4);
        float entrySource = controller.CreatePose().LocalTransforms[0].Translation.X;

        controller.SignalAction(attack.Name);
        controller.Advance(CharacterPlaybackController.ActionBlendInDuration * 0.5f);

        Assert.Equal(CharacterPlaybackPhase.ActionEntry, controller.Phase);
        Assert.Equal(0.5f, controller.BlendAmount);
        float attackAtEntry = 10.5f;
        Assert.Equal((entrySource + attackAtEntry) * 0.5f, controller.CreatePose().LocalTransforms[0].Translation.X, precision: 4);

        controller.Advance(CharacterPlaybackController.ActionBlendInDuration);
        Assert.Equal(CharacterPlaybackPhase.ActionBody, controller.Phase);

        controller.Advance(1.21);
        Assert.Equal(CharacterPlaybackPhase.ActionReturn, controller.Phase);
        Assert.InRange(controller.BlendAmount, 0.066f, 0.067f);

        controller.Advance(0.14);
        Assert.Equal(CharacterPlaybackPhase.Locomotion, controller.Phase);
        Assert.Equal("Walk_Loop", controller.CurrentClip.Name);
        Assert.Equal(1.9f, controller.SampleTime, precision: 4);
    }

    [Fact]
    public void RepeatedActionStartsFromDisplayedPoseAndPauseFreezesAllProgress()
    {
        SkeletonDefinition skeleton = CreateSkeleton();
        AnimationClip idle = CreateConstantClip("Idle_Loop", skeleton, 2.0f, 1.0f);
        AnimationClip attack = CreateLinearClip("Sword_Attack", skeleton, 1.5f, 10.0f, 25.0f);
        var controller = new CharacterPlaybackController([idle, attack], idle.Name);
        controller.SignalAction(attack.Name);
        controller.Advance(0.4);
        float displayed = controller.CreatePose().LocalTransforms[0].Translation.X;

        controller.SignalAction(attack.Name);

        Assert.Equal(CharacterPlaybackPhase.ActionEntry, controller.Phase);
        Assert.Equal(displayed, controller.CreatePose().LocalTransforms[0].Translation.X, precision: 5);
        controller.TogglePlaying();
        controller.Advance(0.5);
        Assert.Equal(0.0f, controller.SampleTime);
        Assert.Equal(displayed, controller.CreatePose().LocalTransforms[0].Translation.X, precision: 5);
    }

    [Fact]
    public void ActionFromDirectInspectionKeepsAdvancingTheCurrentClipAsItsReturnTarget()
    {
        SkeletonDefinition skeleton = CreateSkeleton();
        AnimationClip walk = CreateLinearClip("Walk_Loop", skeleton, 2.0f, 0.0f, 2.0f);
        AnimationClip attack = CreateLinearClip("Sword_Attack", skeleton, 1.5f, 10.0f, 25.0f);
        var controller = new CharacterPlaybackController([walk, attack], walk.Name);
        controller.Advance(0.75);

        controller.SignalAction(attack.Name);
        controller.Advance(attack.Duration);

        Assert.Equal(CharacterPlaybackPhase.Locomotion, controller.Phase);
        Assert.Equal(walk.Name, controller.CurrentClip.Name);
        Assert.Equal(0.25f, controller.SampleTime, precision: 5);
    }

    [Fact]
    public void LocomotionRequestDuringActionChangesReturnTarget()
    {
        SkeletonDefinition skeleton = CreateSkeleton();
        AnimationClip idle = CreateConstantClip("Idle_Loop", skeleton, 2.0f, 1.0f);
        AnimationClip walk = CreateConstantClip("Walk_Loop", skeleton, 2.0f, 3.0f);
        AnimationClip attack = CreateLinearClip("Sword_Attack", skeleton, 1.5f, 10.0f, 25.0f);
        var controller = new CharacterPlaybackController([idle, walk, attack], idle.Name);
        controller.SignalAction(attack.Name);

        controller.RequestLocomotion(walk.Name);
        controller.Advance(1.5);

        Assert.Equal(CharacterPlaybackPhase.Locomotion, controller.Phase);
        Assert.Equal(walk.Name, controller.CurrentClip.Name);
        Assert.Equal(3.0f, controller.CreatePose().LocalTransforms[0].Translation.X);
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

    private static AnimationClip CreateConstantClip(
        string name,
        SkeletonDefinition skeleton,
        float duration,
        float translation) =>
        CreateLinearClip(name, skeleton, duration, translation, translation);

    private static AnimationClip CreateLinearClip(
        string name,
        SkeletonDefinition skeleton,
        float duration,
        float startTranslation,
        float endTranslation) =>
        new(
            name,
            skeleton,
            [
                new JointAnimationTrack(
                    0,
                    new Vector3AnimationChannel([
                        new Vector3Keyframe(0.0f, Vector3.UnitX * startTranslation),
                        new Vector3Keyframe(duration, Vector3.UnitX * endTranslation),
                    ]),
                    new QuaternionAnimationChannel([new QuaternionKeyframe(0.0f, Quaternion.Identity)]),
                    new Vector3AnimationChannel([new Vector3Keyframe(0.0f, Vector3.One)])),
            ]);
}
