using System.Globalization;

namespace ChronoFall.CharacterExperiment.SdlGpu;

internal enum CharacterPlaybackPhase
{
    Direct,
    LocomotionBlend,
    Locomotion,
    ActionEntry,
    ActionBody,
    ActionReturn,
}

internal sealed class CharacterPlaybackController
{
    internal const float LocomotionBlendDuration = 0.25f;
    internal const float ActionBlendInDuration = 0.10f;
    internal const float ActionBlendOutDuration = 0.15f;

    private readonly AnimationClip[] clips;
    private readonly IReadOnlyList<AnimationClip> readOnlyClips;
    private int directClipIndex;
    private double directSampleTime;
    private AnimationClip locomotionClip;
    private double locomotionSampleTime;
    private SkeletonPose? transitionSourcePose;
    private double transitionElapsed;
    private AnimationClip? actionClip;
    private SkeletonPose? actionEntrySourcePose;
    private SkeletonJointMask? actionMask;
    private double actionSampleTime;

    internal CharacterPlaybackController(IEnumerable<AnimationClip> clips, string initialClipName)
    {
        ArgumentNullException.ThrowIfNull(clips);
        ArgumentException.ThrowIfNullOrWhiteSpace(initialClipName);

        this.clips = clips.ToArray();
        if (this.clips.Length == 0)
            throw new ArgumentException("At least one compatible animation clip is required.", nameof(clips));

        SkeletonDefinition? skeleton = null;
        var names = new HashSet<string>(StringComparer.Ordinal);
        for (int index = 0; index < this.clips.Length; index++)
        {
            AnimationClip clip = this.clips[index] ??
                throw new ArgumentException($"Animation clip {index} cannot be null.", nameof(clips));
            skeleton ??= clip.Skeleton;
            if (!ReferenceEquals(clip.Skeleton, skeleton))
                throw new ArgumentException($"Animation '{clip.Name}' does not share the browser skeleton.", nameof(clips));
            if (!names.Add(clip.Name))
                throw new ArgumentException($"Animation name '{clip.Name}' is duplicated.", nameof(clips));
        }

        directClipIndex = FindClipIndex(initialClipName, nameof(initialClipName));
        locomotionClip = this.clips[directClipIndex];
        readOnlyClips = Array.AsReadOnly(this.clips);
    }

    internal IReadOnlyList<AnimationClip> Clips => readOnlyClips;

    internal AnimationClip CurrentClip => Phase is
        CharacterPlaybackPhase.ActionEntry or
        CharacterPlaybackPhase.ActionBody or
        CharacterPlaybackPhase.ActionReturn
            ? actionClip!
            : Phase is CharacterPlaybackPhase.Locomotion or CharacterPlaybackPhase.LocomotionBlend
                ? locomotionClip
                : clips[directClipIndex];

    internal int CurrentClipIndex => Array.IndexOf(clips, CurrentClip);

    internal float SampleTime => (float)(Phase switch
    {
        CharacterPlaybackPhase.Direct => directSampleTime,
        CharacterPlaybackPhase.LocomotionBlend or CharacterPlaybackPhase.Locomotion => locomotionSampleTime,
        _ => actionSampleTime,
    });

    internal CharacterPlaybackPhase Phase { get; private set; }

    internal float BlendAmount => Phase switch
    {
        CharacterPlaybackPhase.LocomotionBlend => ResolveAmount(transitionElapsed, LocomotionBlendDuration),
        CharacterPlaybackPhase.ActionEntry => ResolveAmount(actionSampleTime, ActionBlendInDuration),
        CharacterPlaybackPhase.ActionReturn => ResolveAmount(
            actionSampleTime - (actionClip!.Duration - ActionBlendOutDuration),
            ActionBlendOutDuration),
        _ => 1.0f,
    };

    internal bool IsPlaying { get; private set; } = true;

    internal bool IsSkeletonVisible { get; private set; }

    internal bool IsLayeredAction => actionMask is not null;

    internal void Advance(double elapsedSeconds)
    {
        if (!double.IsFinite(elapsedSeconds) || elapsedSeconds < 0.0)
            throw new ArgumentOutOfRangeException(nameof(elapsedSeconds), "Elapsed time must be finite and non-negative.");
        if (!IsPlaying || elapsedSeconds == 0.0)
            return;

        switch (Phase)
        {
            case CharacterPlaybackPhase.Direct:
                directSampleTime = ResolveLoopTime(directSampleTime + elapsedSeconds, CurrentClip.Duration);
                break;
            case CharacterPlaybackPhase.LocomotionBlend:
                AdvanceLocomotion(elapsedSeconds);
                transitionElapsed += elapsedSeconds;
                if (transitionElapsed >= LocomotionBlendDuration)
                {
                    transitionElapsed = LocomotionBlendDuration;
                    transitionSourcePose = null;
                    Phase = CharacterPlaybackPhase.Locomotion;
                }
                break;
            case CharacterPlaybackPhase.Locomotion:
                AdvanceLocomotion(elapsedSeconds);
                break;
            case CharacterPlaybackPhase.ActionEntry:
            case CharacterPlaybackPhase.ActionBody:
            case CharacterPlaybackPhase.ActionReturn:
                AdvanceLocomotion(elapsedSeconds);
                actionSampleTime += elapsedSeconds;
                UpdateActionPhase();
                break;
            default:
                throw new InvalidOperationException($"Unsupported playback phase {Phase}.");
        }
    }

    internal SkeletonPose CreatePose()
    {
        return Phase switch
        {
            CharacterPlaybackPhase.Direct => AnimationSampler.Sample(CurrentClip, SampleTime, AnimationPlaybackMode.Loop),
            CharacterPlaybackPhase.Locomotion => CreateLocomotionPose(),
            CharacterPlaybackPhase.LocomotionBlend => SkeletonPoseBlender.Blend(
                transitionSourcePose!,
                CreateLocomotionPose(),
                BlendAmount),
            CharacterPlaybackPhase.ActionEntry => CreateActionEntryPose(),
            CharacterPlaybackPhase.ActionBody => CreateActionBodyPose(),
            CharacterPlaybackPhase.ActionReturn => CreateActionReturnPose(),
            _ => throw new InvalidOperationException($"Unsupported playback phase {Phase}."),
        };
    }

    internal void RequestLocomotion(string name)
    {
        AnimationClip selected = clips[FindClipIndex(name, nameof(name))];
        if (Phase is CharacterPlaybackPhase.ActionEntry or CharacterPlaybackPhase.ActionBody or CharacterPlaybackPhase.ActionReturn)
        {
            if (!ReferenceEquals(locomotionClip, selected))
            {
                locomotionClip = selected;
                locomotionSampleTime = 0.0;
            }
            return;
        }

        AnimationClip currentClip = CurrentClip;
        if (ReferenceEquals(currentClip, selected))
        {
            if (Phase == CharacterPlaybackPhase.Direct)
            {
                locomotionClip = selected;
                locomotionSampleTime = directSampleTime;
            }
            transitionSourcePose = null;
            transitionElapsed = LocomotionBlendDuration;
            Phase = CharacterPlaybackPhase.Locomotion;
            return;
        }

        SkeletonPose sourcePose = CreatePose();
        locomotionClip = selected;
        locomotionSampleTime = 0.0;
        transitionSourcePose = sourcePose;
        transitionElapsed = 0.0;
        Phase = CharacterPlaybackPhase.LocomotionBlend;
    }

    internal void SignalAction(string name) =>
        SignalAction(FindClip(name, nameof(name)), mask: null);

    internal void SignalLayeredAction(string name, SkeletonJointMask mask)
    {
        ArgumentNullException.ThrowIfNull(mask);
        AnimationClip selected = FindClip(name, nameof(name));
        if (!ReferenceEquals(selected.Skeleton, mask.Skeleton))
            throw new ArgumentException("The action mask must use the browser skeleton.", nameof(mask));
        if (mask.IncludedJointCount == 0)
            throw new ArgumentException("The action mask must include at least one joint.", nameof(mask));

        SignalAction(selected, mask);
    }

    private void SignalAction(AnimationClip selected, SkeletonJointMask? mask)
    {
        if (selected.Duration <= ActionBlendInDuration + ActionBlendOutDuration)
        {
            throw new ArgumentException(
                $"Action animation '{selected.Name}' must be longer than the combined blend duration.",
                nameof(selected));
        }

        if (Phase == CharacterPlaybackPhase.Direct)
        {
            locomotionClip = CurrentClip;
            locomotionSampleTime = directSampleTime;
        }
        actionEntrySourcePose = CreatePose();
        actionClip = selected;
        actionMask = mask;
        actionSampleTime = 0.0;
        transitionSourcePose = null;
        transitionElapsed = 0.0;
        Phase = CharacterPlaybackPhase.ActionEntry;
    }

    internal void SelectNext() => SelectIndex((directClipIndex + 1) % clips.Length);

    internal void SelectPrevious() => SelectIndex((directClipIndex - 1 + clips.Length) % clips.Length);

    internal void SelectByName(string name) => SelectIndex(FindClipIndex(name, nameof(name)));

    internal void TogglePlaying() => IsPlaying = !IsPlaying;

    internal void Restart()
    {
        switch (Phase)
        {
            case CharacterPlaybackPhase.Direct:
                directSampleTime = 0.0;
                break;
            case CharacterPlaybackPhase.LocomotionBlend:
            case CharacterPlaybackPhase.Locomotion:
                locomotionSampleTime = 0.0;
                transitionSourcePose = null;
                transitionElapsed = LocomotionBlendDuration;
                Phase = CharacterPlaybackPhase.Locomotion;
                break;
            case CharacterPlaybackPhase.ActionEntry:
            case CharacterPlaybackPhase.ActionBody:
            case CharacterPlaybackPhase.ActionReturn:
                if (actionMask is null)
                    SignalAction(actionClip!.Name);
                else
                    SignalLayeredAction(actionClip!.Name, actionMask);
                break;
            default:
                throw new InvalidOperationException($"Unsupported playback phase {Phase}.");
        }
    }

    internal void ToggleSkeleton() => IsSkeletonVisible = !IsSkeletonVisible;

    internal string CreateWindowTitle(int jointCount, int paletteCount)
    {
        ValidateDiagnosticCounts(jointCount, paletteCount);
        return string.Create(
            CultureInfo.InvariantCulture,
            $"ChronoFall Character Experiment | {CurrentClipIndex + 1}/{clips.Length} {CurrentClip.Name} | " +
            $"{SampleTime:F3}/{CurrentClip.Duration:F3} s | {(IsPlaying ? "playing" : "paused")} | " +
            $"{CreatePhaseLabel()} {BlendAmount:F2} | layer {CreateLayerLabel()} | " +
            $"skeleton {(IsSkeletonVisible ? "on" : "off")} | " +
            $"joints {jointCount} | palette {paletteCount}");
    }

    internal string CreateConsoleDiagnostic(int jointCount, int paletteCount)
    {
        ValidateDiagnosticCounts(jointCount, paletteCount);
        return string.Create(
            CultureInfo.InvariantCulture,
            $"GPU_HARNESS_DIAGNOSTIC clip={CurrentClip.Name} index={CurrentClipIndex + 1}/{clips.Length} " +
            $"sample={SampleTime:F3} duration={CurrentClip.Duration:F3} " +
            $"state={(IsPlaying ? "playing" : "paused")} phase={CreatePhaseLabel()} blend={BlendAmount:F3} " +
            $"layer={CreateLayerLabel()} skeleton={(IsSkeletonVisible ? "on" : "off")} " +
            $"joints={jointCount} palette={paletteCount}");
    }

    private void SelectIndex(int index)
    {
        directClipIndex = index;
        directSampleTime = 0.0;
        locomotionClip = clips[index];
        locomotionSampleTime = 0.0;
        transitionSourcePose = null;
        transitionElapsed = 0.0;
        actionClip = null;
        actionEntrySourcePose = null;
        actionMask = null;
        actionSampleTime = 0.0;
        Phase = CharacterPlaybackPhase.Direct;
    }

    private int FindClipIndex(string name, string parameterName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name, parameterName);
        int selected = Array.FindIndex(
            clips,
            candidate => string.Equals(candidate.Name, name, StringComparison.Ordinal));
        if (selected >= 0)
            return selected;

        string available = string.Join(", ", clips.Select(static candidate => candidate.Name));
        throw new ArgumentException(
            $"Animation '{name}' was not found by ordinal name. Available clips: {available}",
            parameterName);
    }

    private AnimationClip FindClip(string name, string parameterName) =>
        clips[FindClipIndex(name, parameterName)];

    private void AdvanceLocomotion(double elapsedSeconds) =>
        locomotionSampleTime = ResolveLoopTime(locomotionSampleTime + elapsedSeconds, locomotionClip.Duration);

    private SkeletonPose CreateLocomotionPose() =>
        AnimationSampler.Sample(locomotionClip, (float)locomotionSampleTime, AnimationPlaybackMode.Loop);

    private SkeletonPose CreateActionPose() =>
        AnimationSampler.Sample(actionClip!, (float)actionSampleTime, AnimationPlaybackMode.Clamp);

    private SkeletonPose CreateActionEntryPose()
    {
        if (actionMask is null)
            return SkeletonPoseBlender.Blend(actionEntrySourcePose!, CreateActionPose(), BlendAmount);

        SkeletonPose advancingBase = CreateLocomotionPose();
        SkeletonPose displayedSource = SkeletonPoseLayerer.Apply(
            advancingBase,
            actionEntrySourcePose!,
            actionMask,
            1.0f);
        return SkeletonPoseLayerer.Apply(displayedSource, CreateActionPose(), actionMask, BlendAmount);
    }

    private SkeletonPose CreateActionBodyPose()
    {
        SkeletonPose actionPose = CreateActionPose();
        return actionMask is null
            ? actionPose
            : SkeletonPoseLayerer.Apply(CreateLocomotionPose(), actionPose, actionMask, 1.0f);
    }

    private SkeletonPose CreateActionReturnPose()
    {
        SkeletonPose actionPose = CreateActionPose();
        SkeletonPose locomotionPose = CreateLocomotionPose();
        return actionMask is null
            ? SkeletonPoseBlender.Blend(actionPose, locomotionPose, BlendAmount)
            : SkeletonPoseLayerer.Apply(locomotionPose, actionPose, actionMask, 1.0f - BlendAmount);
    }

    private void UpdateActionPhase()
    {
        if (actionSampleTime >= actionClip!.Duration)
        {
            actionSampleTime = 0.0;
            actionClip = null;
            actionEntrySourcePose = null;
            actionMask = null;
            Phase = CharacterPlaybackPhase.Locomotion;
            return;
        }

        Phase = actionSampleTime < ActionBlendInDuration
            ? CharacterPlaybackPhase.ActionEntry
            : actionSampleTime >= actionClip.Duration - ActionBlendOutDuration
                ? CharacterPlaybackPhase.ActionReturn
                : CharacterPlaybackPhase.ActionBody;
    }

    private string CreatePhaseLabel() => Phase switch
    {
        CharacterPlaybackPhase.Direct => "direct",
        CharacterPlaybackPhase.LocomotionBlend => "locomotion-blend",
        CharacterPlaybackPhase.Locomotion => "locomotion",
        CharacterPlaybackPhase.ActionEntry => "action-entry",
        CharacterPlaybackPhase.ActionBody => "action-body",
        CharacterPlaybackPhase.ActionReturn => "action-return",
        _ => throw new InvalidOperationException($"Unsupported playback phase {Phase}."),
    };

    private string CreateLayerLabel() => Phase is
        CharacterPlaybackPhase.ActionEntry or
        CharacterPlaybackPhase.ActionBody or
        CharacterPlaybackPhase.ActionReturn
            ? actionMask is null
                ? "full"
                : $"{actionMask.IncludedJointCount}/{actionMask.Skeleton.JointCount}"
            : "none";

    private void ValidateDiagnosticCounts(int jointCount, int paletteCount)
    {
        if (jointCount != CurrentClip.Skeleton.JointCount)
        {
            throw new InvalidOperationException(
                $"Animation '{CurrentClip.Name}' at {SampleTime:F3} seconds expected " +
                $"{CurrentClip.Skeleton.JointCount} joints, but diagnostics received {jointCount}.");
        }
        if (paletteCount != jointCount)
        {
            throw new InvalidOperationException(
                $"Animation '{CurrentClip.Name}' at {SampleTime:F3} seconds has " +
                $"{jointCount} joints but {paletteCount} palette matrices.");
        }
    }

    private static double ResolveLoopTime(double time, float duration) => time % duration;

    private static float ResolveAmount(double elapsed, float duration) =>
        Math.Clamp((float)(elapsed / duration), 0.0f, 1.0f);
}
