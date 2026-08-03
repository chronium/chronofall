using System.Globalization;

namespace ChronoFall.CharacterExperiment.SdlGpu;

internal enum BowBodyViewMode
{
    FullSequence,
    ShootFrames,
    RapidShootFrames,
}

internal enum BowBodySegmentKind
{
    Neutral,
    Notch,
    AimNeutral,
    Shoot,
    Recovery,
    RepeatNotch,
    RepeatAimNeutral,
    RepeatShoot,
    RepeatRecovery,
    Walk,
    AimUp,
    RapidShoot,
    FinalRecovery,
}

internal sealed record BowBodySequenceSegment(
    BowBodySegmentKind Kind,
    AnimationClip Clip,
    float Duration,
    AnimationPlaybackMode PlaybackMode,
    float BlendDuration);

internal sealed record BowBodyFrame(
    BowBodyViewMode ViewMode,
    BowBodySegmentKind? Segment,
    AnimationClip Clip,
    float SampleTime,
    int SampleFrame,
    SkeletonPose Pose,
    float SequenceTime,
    float SequenceDuration);

internal sealed class BowBodySequence
{
    internal const float FrameRate = 30.0f;
    internal const float StandardBlendDuration = 0.15f;
    internal const float ReleaseBlendDuration = 0.10f;

    private readonly BowBodySequenceSegment[] segments;
    private readonly float[] segmentStarts;

    internal BowBodySequence(
        AnimationClip idle,
        AnimationClip walk,
        AnimationClip notch,
        AnimationClip aimNeutral,
        AnimationClip shoot,
        AnimationClip aimUp,
        AnimationClip rapidShoot)
    {
        ArgumentNullException.ThrowIfNull(idle);
        AnimationClip[] clips = [idle, walk, notch, aimNeutral, shoot, aimUp, rapidShoot];
        SkeletonDefinition skeleton = idle.Skeleton;
        if (clips.Any(clip => !ReferenceEquals(clip.Skeleton, skeleton)))
            throw new ArgumentException("Every bow-body sequence clip must use the same skeleton instance.", nameof(walk));

        AnimationPlaybackMode idleMode = idle.Name.EndsWith("_Loop", StringComparison.Ordinal)
            ? AnimationPlaybackMode.Loop
            : AnimationPlaybackMode.Clamp;
        segments = [
            new(BowBodySegmentKind.Neutral, idle, 1.0f, idleMode, 0.0f),
            new(BowBodySegmentKind.Notch, notch, notch.Duration, AnimationPlaybackMode.Clamp, StandardBlendDuration),
            new(BowBodySegmentKind.AimNeutral, aimNeutral, 1.0f, AnimationPlaybackMode.Loop, StandardBlendDuration),
            new(BowBodySegmentKind.Shoot, shoot, shoot.Duration, AnimationPlaybackMode.Clamp, ReleaseBlendDuration),
            new(BowBodySegmentKind.Recovery, idle, 0.75f, idleMode, StandardBlendDuration),
            new(BowBodySegmentKind.RepeatNotch, notch, notch.Duration, AnimationPlaybackMode.Clamp, StandardBlendDuration),
            new(BowBodySegmentKind.RepeatAimNeutral, aimNeutral, 0.75f, AnimationPlaybackMode.Loop, StandardBlendDuration),
            new(BowBodySegmentKind.RepeatShoot, shoot, shoot.Duration, AnimationPlaybackMode.Clamp, ReleaseBlendDuration),
            new(BowBodySegmentKind.RepeatRecovery, idle, 0.75f, idleMode, StandardBlendDuration),
            new(BowBodySegmentKind.Walk, walk, walk.Duration * 2.0f, AnimationPlaybackMode.Loop, StandardBlendDuration),
            new(BowBodySegmentKind.AimUp, aimUp, aimUp.Duration, AnimationPlaybackMode.Clamp, StandardBlendDuration),
            new(BowBodySegmentKind.RapidShoot, rapidShoot, rapidShoot.Duration * 3.0f, AnimationPlaybackMode.Loop, ReleaseBlendDuration),
            new(BowBodySegmentKind.FinalRecovery, idle, 1.0f, idleMode, StandardBlendDuration),
        ];
        segmentStarts = new float[segments.Length];
        float duration = 0.0f;
        for (int index = 0; index < segments.Length; index++)
        {
            segmentStarts[index] = duration;
            duration += segments[index].Duration;
        }
        Duration = duration;
    }

    internal IReadOnlyList<BowBodySequenceSegment> Segments => segments;

    internal float Duration { get; }

    internal BowBodyFrame Evaluate(float sequenceTime)
    {
        if (!float.IsFinite(sequenceTime))
            throw new ArgumentOutOfRangeException(nameof(sequenceTime));
        float time = ResolveLoopTime(sequenceTime, Duration);
        int index = FindSegment(time);
        BowBodySequenceSegment segment = segments[index];
        float localTime = time - segmentStarts[index];
        float sampleTime = ResolveSampleTime(segment, localTime);
        SkeletonPose pose = AnimationSampler.Sample(segment.Clip, sampleTime, segment.PlaybackMode);
        if (index > 0 && segment.BlendDuration > 0.0f && localTime < segment.BlendDuration)
        {
            BowBodySequenceSegment previous = segments[index - 1];
            float previousSample = ResolveSampleTime(previous, previous.Duration);
            SkeletonPose source = AnimationSampler.Sample(previous.Clip, previousSample, previous.PlaybackMode);
            pose = SkeletonPoseBlender.Blend(source, pose, localTime / segment.BlendDuration);
        }

        return new BowBodyFrame(
            BowBodyViewMode.FullSequence,
            segment.Kind,
            segment.Clip,
            sampleTime,
            ToFrame(sampleTime, segment.Clip.Duration),
            pose,
            time,
            Duration);
    }

    private int FindSegment(float time)
    {
        for (int index = segments.Length - 1; index >= 0; index--)
        {
            if (time >= segmentStarts[index])
                return index;
        }
        return 0;
    }

    private static float ResolveSampleTime(BowBodySequenceSegment segment, float localTime) =>
        segment.PlaybackMode == AnimationPlaybackMode.Loop
            ? ResolveLoopTime(localTime, segment.Clip.Duration)
            : Math.Clamp(localTime, 0.0f, segment.Clip.Duration);

    internal static int ToFrame(float sampleTime, float duration) =>
        Math.Clamp((int)MathF.Round(sampleTime * FrameRate), 0, (int)MathF.Round(duration * FrameRate));

    internal static float ResolveLoopTime(float time, float duration)
    {
        float result = time % duration;
        return result < 0.0f ? result + duration : result;
    }
}

internal sealed class BowBodyPlaybackController
{
    private readonly BowBodySequence sequence;
    private readonly AnimationClip shoot;
    private readonly AnimationClip rapidShoot;
    private float time;

    internal BowBodyPlaybackController(
        AnimationClip referenceIdle,
        AnimationClip walk,
        AnimationClip notch,
        AnimationClip aimNeutral,
        AnimationClip shoot,
        AnimationClip aimUp,
        AnimationClip rapidShoot)
    {
        sequence = new(referenceIdle, walk, notch, aimNeutral, shoot, aimUp, rapidShoot);
        this.shoot = shoot;
        this.rapidShoot = rapidShoot;
    }

    internal BowBodyViewMode ViewMode { get; private set; }

    internal bool IsPlaying { get; private set; } = true;

    internal bool IsSkeletonVisible { get; private set; }

    internal BowBodyFrame CreateFrame()
    {
        if (ViewMode == BowBodyViewMode.FullSequence)
            return sequence.Evaluate(time);

        AnimationClip clip = ViewMode == BowBodyViewMode.ShootFrames ? shoot : rapidShoot;
        const AnimationPlaybackMode playback = AnimationPlaybackMode.Clamp;
        float sampleTime = Math.Clamp(time, 0.0f, clip.Duration);
        return new BowBodyFrame(
            ViewMode,
            null,
            clip,
            sampleTime,
            BowBodySequence.ToFrame(sampleTime, clip.Duration),
            AnimationSampler.Sample(clip, sampleTime, playback),
            time,
            clip.Duration);
    }

    internal void Advance(double elapsedSeconds)
    {
        if (!double.IsFinite(elapsedSeconds) || elapsedSeconds < 0.0)
            throw new ArgumentOutOfRangeException(nameof(elapsedSeconds));
        if (!IsPlaying)
            return;
        time += (float)elapsedSeconds;
        NormalizeTime();
    }

    internal void SelectMode(BowBodyViewMode mode)
    {
        ViewMode = mode;
        time = 0.0f;
    }

    internal void TogglePlaying() => IsPlaying = !IsPlaying;

    internal void ToggleSkeleton() => IsSkeletonVisible = !IsSkeletonVisible;

    internal void Restart() => time = 0.0f;

    internal void StepFrames(int frames)
    {
        IsPlaying = false;
        time += frames / BowBodySequence.FrameRate;
        if (ViewMode == BowBodyViewMode.FullSequence)
            time = Math.Clamp(time, 0.0f, sequence.Duration - 1.0f / BowBodySequence.FrameRate);
        else
            time = Math.Clamp(time, 0.0f, CurrentInspectionClip.Duration);
    }

    internal string CreateDiagnostic()
    {
        BowBodyFrame frame = CreateFrame();
        string segment = frame.Segment?.ToString() ?? "frame-inspector";
        return string.Create(
            CultureInfo.InvariantCulture,
            $"BOW_BODY_DIAGNOSTIC mode={frame.ViewMode} idle=UAL1-Idle_Loop " +
            $"segment={segment} clip={frame.Clip.Name} sample={frame.SampleTime:F3} frame={frame.SampleFrame} " +
            $"sequence={frame.SequenceTime:F3}/{frame.SequenceDuration:F3} state={(IsPlaying ? "playing" : "paused")} " +
            $"skeleton={(IsSkeletonVisible ? "on" : "off")}");
    }

    private AnimationClip CurrentInspectionClip => ViewMode == BowBodyViewMode.ShootFrames ? shoot : rapidShoot;

    private void NormalizeTime()
    {
        if (ViewMode == BowBodyViewMode.FullSequence)
            time = BowBodySequence.ResolveLoopTime(time, sequence.Duration);
        else
            time = Math.Clamp(time, 0.0f, CurrentInspectionClip.Duration);
    }
}
