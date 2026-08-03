using System.Numerics;

namespace ChronoFall.CharacterExperiment.SdlGpu.Tests;

public sealed class BowBodySequenceControllerTests
{
    [Fact]
    public void SequenceUsesTheFocusedDraftZeroOrderAndLoopsDeterministically()
    {
        SkeletonDefinition skeleton = CreateSkeleton();
        BowBodySequence sequence = CreateSequence(skeleton, "Idle_Loop");

        Assert.Equal([
            BowBodySegmentKind.Neutral,
            BowBodySegmentKind.Notch,
            BowBodySegmentKind.AimNeutral,
            BowBodySegmentKind.Shoot,
            BowBodySegmentKind.Recovery,
            BowBodySegmentKind.RepeatNotch,
            BowBodySegmentKind.RepeatAimNeutral,
            BowBodySegmentKind.RepeatShoot,
            BowBodySegmentKind.RepeatRecovery,
            BowBodySegmentKind.Walk,
            BowBodySegmentKind.AimUp,
            BowBodySegmentKind.RapidShoot,
            BowBodySegmentKind.FinalRecovery,
        ], sequence.Segments.Select(static segment => segment.Kind));

        BowBodyFrame start = sequence.Evaluate(0.0f);
        BowBodyFrame loop = sequence.Evaluate(sequence.Duration);
        Assert.Equal(BowBodySegmentKind.Neutral, start.Segment);
        Assert.Equal(start.Pose.LocalTransforms, loop.Pose.LocalTransforms);
        Assert.Equal(0.0f, loop.SequenceTime);
    }

    [Fact]
    public void SequenceBlendsIntoNotchAndContainsTwoShotsAndThreeRapidCycles()
    {
        SkeletonDefinition skeleton = CreateSkeleton();
        BowBodySequence sequence = CreateSequence(skeleton, "Idle_Loop");
        BowBodySequenceSegment notch = sequence.Segments.Single(segment => segment.Kind == BowBodySegmentKind.Notch);
        BowBodySequenceSegment shoot = sequence.Segments.Single(segment => segment.Kind == BowBodySegmentKind.Shoot);
        BowBodySequenceSegment repeat = sequence.Segments.Single(segment => segment.Kind == BowBodySegmentKind.RepeatShoot);
        BowBodySequenceSegment rapid = sequence.Segments.Single(segment => segment.Kind == BowBodySegmentKind.RapidShoot);

        Assert.Equal(BowBodySequence.StandardBlendDuration, notch.BlendDuration);
        Assert.Equal(BowBodySequence.ReleaseBlendDuration, shoot.BlendDuration);
        Assert.Equal(shoot.Clip, repeat.Clip);
        Assert.Equal(rapid.Clip.Duration * 3.0f, rapid.Duration);
    }

    [Fact]
    public void PlaybackControllerUsesReferenceIdleAndStepsReleaseFramesExactly()
    {
        SkeletonDefinition skeleton = CreateSkeleton();
        BowBodyPlaybackController controller = CreateController(skeleton);

        controller.SelectMode(BowBodyViewMode.ShootFrames);
        controller.StepFrames(7);

        BowBodyFrame frame = controller.CreateFrame();
        Assert.False(controller.IsPlaying);
        Assert.Equal("Bow_Shoot", frame.Clip.Name);
        Assert.Equal(7, frame.SampleFrame);
        Assert.Equal(7.0f / 30.0f, frame.SampleTime, precision: 5);
        Assert.Contains("frame=7", controller.CreateDiagnostic(), StringComparison.Ordinal);
    }

    [Fact]
    public void ExactRebindAcceptsOnlyIdenticalSkeletonAndInverseBindContracts()
    {
        SkeletonDefinition sourceSkeleton = CreateSkeleton();
        SkeletonDefinition targetSkeleton = CreateSkeleton();
        SkinDefinition sourceSkin = new(sourceSkeleton, [Matrix4x4.Identity]);
        SkinDefinition targetSkin = new(targetSkeleton, [Matrix4x4.Identity]);
        AnimationClip source = CreateClip("Idle_Loop", sourceSkeleton, 1.0f, 0.0f);

        AnimationClip rebound = ExactSkeletonAnimationRebinder.Rebind(sourceSkin, source, targetSkin);

        Assert.Same(targetSkeleton, rebound.Skeleton);
        Assert.Equal(source.Name, rebound.Name);
        Assert.Same(source.Tracks[0], rebound.Tracks[0]);

        SkinDefinition mismatchedBind = new(targetSkeleton, [Matrix4x4.CreateTranslation(1.0f, 0.0f, 0.0f)]);
        Assert.Throws<ArgumentException>(() =>
            ExactSkeletonAnimationRebinder.Rebind(sourceSkin, source, mismatchedBind));
    }

    private static BowBodySequence CreateSequence(SkeletonDefinition skeleton, string idleName) =>
        new(
            CreateClip(idleName, skeleton, 2.5f, 0.0f),
            CreateClip("Walk_Fwd_Loop", skeleton, 4.0f / 3.0f, 1.0f),
            CreateClip("Bow_Notch", skeleton, 2.5f, 2.0f),
            CreateClip("Bow_Aim_Neutral", skeleton, 2.5f, 3.0f),
            CreateClip("Bow_Shoot", skeleton, 2.0f / 3.0f, 4.0f),
            CreateClip("Bow_Aim_Up", skeleton, 4.0f / 3.0f, 5.0f),
            CreateClip("Bow_RapidShoot_Loop", skeleton, 13.0f / 30.0f, 6.0f));

    private static BowBodyPlaybackController CreateController(SkeletonDefinition skeleton) =>
        new(
            CreateClip("Idle_Loop", skeleton, 2.5f, 0.5f),
            CreateClip("Walk_Fwd_Loop", skeleton, 4.0f / 3.0f, 1.0f),
            CreateClip("Bow_Notch", skeleton, 2.5f, 2.0f),
            CreateClip("Bow_Aim_Neutral", skeleton, 2.5f, 3.0f),
            CreateClip("Bow_Shoot", skeleton, 2.0f / 3.0f, 4.0f),
            CreateClip("Bow_Aim_Up", skeleton, 4.0f / 3.0f, 5.0f),
            CreateClip("Bow_RapidShoot_Loop", skeleton, 13.0f / 30.0f, 6.0f));

    private static SkeletonDefinition CreateSkeleton() =>
        new([new SkeletonJoint("root", -1, JointTransform.Identity)]);

    private static AnimationClip CreateClip(
        string name,
        SkeletonDefinition skeleton,
        float duration,
        float translation) =>
        new(
            name,
            skeleton,
            [
                new JointAnimationTrack(
                    0,
                    new Vector3AnimationChannel([
                        new Vector3Keyframe(0.0f, new Vector3(translation, 0.0f, 0.0f)),
                        new Vector3Keyframe(duration, new Vector3(translation + 0.25f, 0.0f, 0.0f)),
                    ]),
                    new QuaternionAnimationChannel([
                        new QuaternionKeyframe(0.0f, Quaternion.Identity),
                        new QuaternionKeyframe(duration, Quaternion.Identity),
                    ]),
                    new Vector3AnimationChannel([
                        new Vector3Keyframe(0.0f, Vector3.One),
                        new Vector3Keyframe(duration, Vector3.One),
                    ])),
            ]);
}
