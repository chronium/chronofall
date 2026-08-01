using System.Globalization;

namespace ChronoFall.CharacterExperiment.SdlGpu;

internal sealed class CharacterPlaybackController
{
    private readonly AnimationClip[] clips;
    private readonly IReadOnlyList<AnimationClip> readOnlyClips;
    private int clipIndex;
    private double sampleTime;

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

        clipIndex = Array.FindIndex(
            this.clips,
            candidate => string.Equals(candidate.Name, initialClipName, StringComparison.Ordinal));
        if (clipIndex < 0)
        {
            string available = string.Join(", ", this.clips.Select(static candidate => candidate.Name));
            throw new ArgumentException(
                $"Initial animation '{initialClipName}' was not found by ordinal name. Available clips: {available}",
                nameof(initialClipName));
        }

        readOnlyClips = Array.AsReadOnly(this.clips);
    }

    internal IReadOnlyList<AnimationClip> Clips => readOnlyClips;

    internal AnimationClip CurrentClip => clips[clipIndex];

    internal int CurrentClipIndex => clipIndex;

    internal float SampleTime => (float)sampleTime;

    internal bool IsPlaying { get; private set; } = true;

    internal bool IsSkeletonVisible { get; private set; }

    internal void Advance(double elapsedSeconds)
    {
        if (!double.IsFinite(elapsedSeconds) || elapsedSeconds < 0.0)
            throw new ArgumentOutOfRangeException(nameof(elapsedSeconds), "Elapsed time must be finite and non-negative.");
        if (!IsPlaying || elapsedSeconds == 0.0)
            return;

        sampleTime = (sampleTime + elapsedSeconds) % CurrentClip.Duration;
    }

    internal void SelectNext() => SelectIndex((clipIndex + 1) % clips.Length);

    internal void SelectPrevious() => SelectIndex((clipIndex - 1 + clips.Length) % clips.Length);

    internal void SelectByName(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        int selected = Array.FindIndex(
            clips,
            candidate => string.Equals(candidate.Name, name, StringComparison.Ordinal));
        if (selected < 0)
            throw new ArgumentException($"Animation '{name}' is not available in the browser.", nameof(name));
        SelectIndex(selected);
    }

    internal void TogglePlaying() => IsPlaying = !IsPlaying;

    internal void Restart() => sampleTime = 0.0;

    internal void ToggleSkeleton() => IsSkeletonVisible = !IsSkeletonVisible;

    internal string CreateWindowTitle(int jointCount, int paletteCount)
    {
        ValidateDiagnosticCounts(jointCount, paletteCount);
        return string.Create(
            CultureInfo.InvariantCulture,
            $"ChronoFall Character Experiment | {clipIndex + 1}/{clips.Length} {CurrentClip.Name} | " +
            $"{SampleTime:F3}/{CurrentClip.Duration:F3} s | {(IsPlaying ? "playing" : "paused")} | " +
            $"skeleton {(IsSkeletonVisible ? "on" : "off")} | joints {jointCount} | palette {paletteCount}");
    }

    internal string CreateConsoleDiagnostic(int jointCount, int paletteCount)
    {
        ValidateDiagnosticCounts(jointCount, paletteCount);
        return string.Create(
            CultureInfo.InvariantCulture,
            $"GPU_HARNESS_DIAGNOSTIC clip={CurrentClip.Name} index={clipIndex + 1}/{clips.Length} " +
            $"sample={SampleTime:F3} duration={CurrentClip.Duration:F3} " +
            $"state={(IsPlaying ? "playing" : "paused")} skeleton={(IsSkeletonVisible ? "on" : "off")} " +
            $"joints={jointCount} palette={paletteCount}");
    }

    private void SelectIndex(int index)
    {
        clipIndex = index;
        sampleTime = 0.0;
    }

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
}
