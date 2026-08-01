using System.Numerics;

namespace ChronoFall.CharacterExperiment;

public static class AnimationSampler
{
    public static float ResolveTime(
        AnimationClip clip,
        float time,
        AnimationPlaybackMode playbackMode)
    {
        ArgumentNullException.ThrowIfNull(clip);
        if (!float.IsFinite(time))
            throw new ArgumentOutOfRangeException(nameof(time), "Sample time must be finite.");

        return playbackMode switch
        {
            AnimationPlaybackMode.Clamp => Math.Clamp(time, 0.0f, clip.Duration),
            AnimationPlaybackMode.Loop => ResolveLoopTime(time, clip.Duration),
            _ => throw new ArgumentOutOfRangeException(nameof(playbackMode)),
        };
    }

    public static SkeletonPose Sample(
        AnimationClip clip,
        float time,
        AnimationPlaybackMode playbackMode)
    {
        ArgumentNullException.ThrowIfNull(clip);
        float sampleTime = ResolveTime(clip, time, playbackMode);
        var transforms = new JointTransform[clip.Tracks.Count];
        for (int index = 0; index < transforms.Length; index++)
        {
            JointAnimationTrack track = clip.Tracks[index];
            transforms[index] = new JointTransform(
                Sample(track.Translations, sampleTime),
                Sample(track.Rotations, sampleTime),
                Sample(track.Scales, sampleTime));
        }

        return new SkeletonPose(clip.Skeleton, transforms);
    }

    private static float ResolveLoopTime(float time, float duration)
    {
        float resolved = time % duration;
        if (resolved < 0.0f)
            resolved += duration;

        return resolved == 0.0f || resolved >= duration ? 0.0f : resolved;
    }

    private static Vector3 Sample(Vector3AnimationChannel channel, float time)
    {
        IReadOnlyList<Vector3Keyframe> keyframes = channel.Keyframes;
        if (keyframes.Count == 1 || time <= keyframes[0].Time)
            return keyframes[0].Value;
        if (time >= keyframes[^1].Time)
            return keyframes[^1].Value;

        int upperIndex = FindUpperKeyframe(keyframes, time, static keyframe => keyframe.Time);
        Vector3Keyframe lower = keyframes[upperIndex - 1];
        Vector3Keyframe upper = keyframes[upperIndex];
        float amount = (time - lower.Time) / (upper.Time - lower.Time);
        return Vector3.Lerp(lower.Value, upper.Value, amount);
    }

    private static Quaternion Sample(QuaternionAnimationChannel channel, float time)
    {
        IReadOnlyList<QuaternionKeyframe> keyframes = channel.Keyframes;
        if (keyframes.Count == 1 || time <= keyframes[0].Time)
            return keyframes[0].Value;
        if (time >= keyframes[^1].Time)
            return keyframes[^1].Value;

        int upperIndex = FindUpperKeyframe(keyframes, time, static keyframe => keyframe.Time);
        QuaternionKeyframe lower = keyframes[upperIndex - 1];
        QuaternionKeyframe upper = keyframes[upperIndex];
        Quaternion upperValue = Quaternion.Dot(lower.Value, upper.Value) < 0.0f
            ? new Quaternion(-upper.Value.X, -upper.Value.Y, -upper.Value.Z, -upper.Value.W)
            : upper.Value;
        float amount = (time - lower.Time) / (upper.Time - lower.Time);
        return DataValidation.NormalizeRotation(
            Quaternion.Slerp(lower.Value, upperValue, amount),
            nameof(channel));
    }

    private static int FindUpperKeyframe<T>(
        IReadOnlyList<T> keyframes,
        float time,
        Func<T, float> getTime)
    {
        int lower = 0;
        int upper = keyframes.Count - 1;
        while (upper - lower > 1)
        {
            int candidate = lower + ((upper - lower) / 2);
            if (time < getTime(keyframes[candidate]))
                upper = candidate;
            else
                lower = candidate;
        }

        return upper;
    }
}
