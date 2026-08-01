using System.Numerics;

namespace ChronoFall.CharacterPresentation.Tests;

public sealed class AnimationDataTests
{
    [Fact]
    public void ClipRequiresCompleteOrderedTrsTracksAndDerivesDuration()
    {
        SkeletonDefinition skeleton = CreateSkeleton();
        JointAnimationTrack[] tracks = [Track(0, 1.0f), Track(1, 2.5f)];

        var clip = new AnimationClip("Test", skeleton, tracks);
        tracks[1] = Track(1, 9.0f);

        Assert.Equal(2.5f, clip.Duration, 5);
        Assert.Equal(2, clip.Tracks.Count);
        Assert.Equal(2.5f, clip.Tracks[1].EndTime, 5);
        Assert.Throws<ArgumentException>(() => new AnimationClip("Test", skeleton, [Track(0, 1.0f)]));
        Assert.Throws<ArgumentException>(() => new AnimationClip("Test", skeleton, [Track(0, 1.0f), Track(0, 1.0f)]));
    }

    [Fact]
    public void ChannelsRequireFiniteStrictlyIncreasingKeyframes()
    {
        Assert.Throws<ArgumentException>(() => new Vector3AnimationChannel([]));
        Assert.Throws<ArgumentException>(() => new Vector3AnimationChannel([
            new Vector3Keyframe(0.5f, Vector3.Zero),
            new Vector3Keyframe(0.5f, Vector3.One),
        ]));
        Assert.Throws<ArgumentOutOfRangeException>(() => new Vector3Keyframe(float.NaN, Vector3.Zero));
        Assert.Throws<ArgumentException>(() => new QuaternionKeyframe(0.0f, default));
        Assert.Throws<ArgumentException>(() => new QuaternionAnimationChannel([default]));
        Assert.Throws<ArgumentOutOfRangeException>(() => new Vector3AnimationChannel(
            [new Vector3Keyframe(0.0f, Vector3.Zero)],
            (AnimationInterpolation)99));
    }

    [Fact]
    public void ChannelsDefensivelyCopyAndPlaybackModeIsExplicit()
    {
        Vector3Keyframe[] keys = [
            new Vector3Keyframe(0.0f, Vector3.Zero),
            new Vector3Keyframe(1.0f, Vector3.One),
        ];
        var channel = new Vector3AnimationChannel(keys);
        keys[1] = new Vector3Keyframe(3.0f, new Vector3(3.0f));

        Assert.Equal(1.0f, channel.EndTime, 5);
        Assert.Equal(AnimationInterpolation.Linear, channel.Interpolation);
        Assert.NotEqual(AnimationPlaybackMode.Clamp, AnimationPlaybackMode.Loop);
    }

    private static JointAnimationTrack Track(int jointIndex, float endTime) =>
        new(
            jointIndex,
            new Vector3AnimationChannel([
                new Vector3Keyframe(0.0f, Vector3.Zero),
                new Vector3Keyframe(endTime, Vector3.One),
            ]),
            new QuaternionAnimationChannel([
                new QuaternionKeyframe(0.0f, Quaternion.Identity),
                new QuaternionKeyframe(endTime, Quaternion.CreateFromAxisAngle(Vector3.UnitY, 0.5f)),
            ]),
            new Vector3AnimationChannel([
                new Vector3Keyframe(0.0f, Vector3.One),
                new Vector3Keyframe(endTime, Vector3.One),
            ]));

    private static SkeletonDefinition CreateSkeleton() =>
        new([
            new SkeletonJoint("root", -1, JointTransform.Identity),
            new SkeletonJoint("child", 0, JointTransform.Identity),
        ]);
}
