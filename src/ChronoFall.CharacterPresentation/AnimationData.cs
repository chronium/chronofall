using System.Numerics;

namespace ChronoFall.CharacterPresentation;

public enum AnimationInterpolation
{
    Linear,
}

public enum AnimationPlaybackMode
{
    Clamp,
    Loop,
}

public readonly record struct Vector3Keyframe
{
    public Vector3Keyframe(float time, Vector3 value)
    {
        if (!float.IsFinite(time) || time < 0.0f)
            throw new ArgumentOutOfRangeException(nameof(time), "Keyframe time must be finite and non-negative.");
        if (!DataValidation.IsFinite(value))
            throw new ArgumentException("Keyframe value must contain only finite values.", nameof(value));

        Time = time;
        Value = value;
    }

    public float Time { get; }

    public Vector3 Value { get; }
}

public readonly record struct QuaternionKeyframe
{
    public QuaternionKeyframe(float time, Quaternion value)
    {
        if (!float.IsFinite(time) || time < 0.0f)
            throw new ArgumentOutOfRangeException(nameof(time), "Keyframe time must be finite and non-negative.");

        Time = time;
        Value = DataValidation.NormalizeRotation(value, nameof(value));
    }

    public float Time { get; }

    public Quaternion Value { get; }
}

public sealed class Vector3AnimationChannel
{
    public Vector3AnimationChannel(
        IEnumerable<Vector3Keyframe> keyframes,
        AnimationInterpolation interpolation = AnimationInterpolation.Linear)
    {
        ArgumentNullException.ThrowIfNull(keyframes);
        if (interpolation != AnimationInterpolation.Linear)
            throw new ArgumentOutOfRangeException(nameof(interpolation), "The M1 experiment supports only LINEAR interpolation.");

        Vector3Keyframe[] copy = keyframes.ToArray();
        ValidateTimes(copy.Select(static keyframe => keyframe.Time), copy.Length, nameof(keyframes));
        Keyframes = Array.AsReadOnly(copy);
        Interpolation = interpolation;
    }

    public IReadOnlyList<Vector3Keyframe> Keyframes { get; }

    public AnimationInterpolation Interpolation { get; }

    public float EndTime => Keyframes[^1].Time;

    internal static void ValidateTimes(IEnumerable<float> times, int count, string parameterName)
    {
        if (count == 0)
            throw new ArgumentException("An animation channel must contain at least one keyframe.", parameterName);

        float previous = -1.0f;
        int index = 0;
        foreach (float time in times)
        {
            if (index > 0 && time <= previous)
                throw new ArgumentException("Animation keyframe times must be strictly increasing.", parameterName);
            previous = time;
            index++;
        }
    }
}

public sealed class QuaternionAnimationChannel
{
    public QuaternionAnimationChannel(
        IEnumerable<QuaternionKeyframe> keyframes,
        AnimationInterpolation interpolation = AnimationInterpolation.Linear)
    {
        ArgumentNullException.ThrowIfNull(keyframes);
        if (interpolation != AnimationInterpolation.Linear)
            throw new ArgumentOutOfRangeException(nameof(interpolation), "The M1 experiment supports only LINEAR interpolation.");

        QuaternionKeyframe[] copy = keyframes.ToArray();
        Vector3AnimationChannel.ValidateTimes(
            copy.Select(static keyframe => keyframe.Time),
            copy.Length,
            nameof(keyframes));
        for (int index = 0; index < copy.Length; index++)
            DataValidation.NormalizeRotation(copy[index].Value, nameof(keyframes));
        Keyframes = Array.AsReadOnly(copy);
        Interpolation = interpolation;
    }

    public IReadOnlyList<QuaternionKeyframe> Keyframes { get; }

    public AnimationInterpolation Interpolation { get; }

    public float EndTime => Keyframes[^1].Time;
}

public sealed class JointAnimationTrack
{
    public JointAnimationTrack(
        int jointIndex,
        Vector3AnimationChannel translations,
        QuaternionAnimationChannel rotations,
        Vector3AnimationChannel scales)
    {
        if (jointIndex < 0)
            throw new ArgumentOutOfRangeException(nameof(jointIndex));

        JointIndex = jointIndex;
        Translations = translations ?? throw new ArgumentNullException(nameof(translations));
        Rotations = rotations ?? throw new ArgumentNullException(nameof(rotations));
        Scales = scales ?? throw new ArgumentNullException(nameof(scales));
    }

    public int JointIndex { get; }

    public Vector3AnimationChannel Translations { get; }

    public QuaternionAnimationChannel Rotations { get; }

    public Vector3AnimationChannel Scales { get; }

    public float EndTime => MathF.Max(Translations.EndTime, MathF.Max(Rotations.EndTime, Scales.EndTime));
}

public sealed class AnimationClip
{
    public AnimationClip(string name, SkeletonDefinition skeleton, IEnumerable<JointAnimationTrack> tracks)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(skeleton);
        ArgumentNullException.ThrowIfNull(tracks);

        JointAnimationTrack[] copy = tracks.ToArray();
        if (copy.Length != skeleton.JointCount)
        {
            throw new ArgumentException(
                $"Expected {skeleton.JointCount} joint tracks, but received {copy.Length}.",
                nameof(tracks));
        }

        float duration = 0.0f;
        for (int index = 0; index < copy.Length; index++)
        {
            JointAnimationTrack track = copy[index] ??
                throw new ArgumentException($"Track {index} cannot be null.", nameof(tracks));
            if (track.JointIndex != index)
            {
                throw new ArgumentException(
                    $"Track {index} must target joint {index}, but targets joint {track.JointIndex}.",
                    nameof(tracks));
            }

            duration = MathF.Max(duration, track.EndTime);
        }

        if (!float.IsFinite(duration) || duration <= 0.0f)
            throw new ArgumentException("Animation duration must be finite and greater than zero.", nameof(tracks));

        Name = name;
        Skeleton = skeleton;
        Tracks = Array.AsReadOnly(copy);
        Duration = duration;
    }

    public string Name { get; }

    public SkeletonDefinition Skeleton { get; }

    public IReadOnlyList<JointAnimationTrack> Tracks { get; }

    public float Duration { get; }
}
