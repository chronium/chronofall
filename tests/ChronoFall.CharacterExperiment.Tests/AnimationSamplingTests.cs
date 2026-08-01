using System.Numerics;

namespace ChronoFall.CharacterExperiment.Tests;

public sealed class AnimationSamplingTests
{
    [Theory]
    [InlineData(-1.0f, AnimationPlaybackMode.Clamp, 0.0f)]
    [InlineData(0.25f, AnimationPlaybackMode.Clamp, 0.25f)]
    [InlineData(2.0f, AnimationPlaybackMode.Clamp, 2.0f)]
    [InlineData(3.0f, AnimationPlaybackMode.Clamp, 2.0f)]
    [InlineData(-0.25f, AnimationPlaybackMode.Loop, 1.75f)]
    [InlineData(0.0f, AnimationPlaybackMode.Loop, 0.0f)]
    [InlineData(2.0f, AnimationPlaybackMode.Loop, 0.0f)]
    [InlineData(4.25f, AnimationPlaybackMode.Loop, 0.25f)]
    public void ResolveTimeUsesExplicitClampAndEuclideanLoop(
        float time,
        AnimationPlaybackMode playbackMode,
        float expected)
    {
        AnimationClip clip = CreateClip();

        float actual = AnimationSampler.ResolveTime(clip, time, playbackMode);

        Assert.Equal(expected, actual, 5);
    }

    [Fact]
    public void ResolveTimeRejectsNonFiniteTimeAndUnknownPlaybackMode()
    {
        AnimationClip clip = CreateClip();

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            AnimationSampler.ResolveTime(clip, float.NaN, AnimationPlaybackMode.Clamp));
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            AnimationSampler.ResolveTime(clip, 0.0f, (AnimationPlaybackMode)99));
    }

    [Fact]
    public void SampleInterpolatesTrsAndHoldsIndividualChannelEndpoints()
    {
        AnimationClip clip = CreateClip();

        SkeletonPose beforeFirstKey = AnimationSampler.Sample(clip, -1.0f, AnimationPlaybackMode.Clamp);
        SkeletonPose midpoint = AnimationSampler.Sample(clip, 1.0f, AnimationPlaybackMode.Clamp);
        SkeletonPose afterChildEnd = AnimationSampler.Sample(clip, 1.5f, AnimationPlaybackMode.Clamp);

        Assert.Same(clip.Skeleton, midpoint.Skeleton);
        AssertVector(new Vector3(1.0f, 0.0f, 0.0f), beforeFirstKey.LocalTransforms[1].Translation);
        AssertVector(new Vector3(5.0f, 0.0f, 0.0f), midpoint.LocalTransforms[0].Translation);
        AssertVector(new Vector3(2.0f), midpoint.LocalTransforms[0].Scale);
        AssertVector(new Vector3(3.0f, 0.0f, 0.0f), midpoint.LocalTransforms[1].Translation);
        AssertVector(new Vector3(3.0f, 0.0f, 0.0f), afterChildEnd.LocalTransforms[1].Translation);
        AssertVector(Vector3.One, midpoint.LocalTransforms[1].Scale);
        Assert.Equal(1.0f, midpoint.LocalTransforms[0].Rotation.Length(), 5);

        Vector3 rotated = Vector3.Transform(Vector3.UnitX, midpoint.LocalTransforms[0].Rotation);
        float halfSqrt = MathF.Sqrt(0.5f);
        AssertVector(new Vector3(halfSqrt, 0.0f, -halfSqrt), rotated);
    }

    [Fact]
    public void SampleUsesShortestQuaternionPathAndLoopsAtDuration()
    {
        SkeletonDefinition skeleton = new([new SkeletonJoint("root", -1, JointTransform.Identity)]);
        Quaternion rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitY, 0.75f);
        Quaternion antipodal = new(-rotation.X, -rotation.Y, -rotation.Z, -rotation.W);
        var clip = new AnimationClip("Antipodal", skeleton, [
            new JointAnimationTrack(
                0,
                Channel(Vector3.Zero, Vector3.Zero, 2.0f),
                new QuaternionAnimationChannel([
                    new QuaternionKeyframe(0.0f, rotation),
                    new QuaternionKeyframe(2.0f, antipodal),
                ]),
                Channel(Vector3.One, Vector3.One, 2.0f)),
        ]);

        SkeletonPose midpoint = AnimationSampler.Sample(clip, 1.0f, AnimationPlaybackMode.Clamp);
        SkeletonPose loopBoundary = AnimationSampler.Sample(clip, clip.Duration, AnimationPlaybackMode.Loop);

        AssertQuaternionEquivalent(rotation, midpoint.LocalTransforms[0].Rotation);
        AssertQuaternionEquivalent(rotation, loopBoundary.LocalTransforms[0].Rotation);
        Assert.Equal(1.0f, midpoint.LocalTransforms[0].Rotation.Length(), 5);
    }

    private static AnimationClip CreateClip()
    {
        var skeleton = new SkeletonDefinition([
            new SkeletonJoint("root", -1, JointTransform.Identity),
            new SkeletonJoint("child", 0, JointTransform.Identity),
        ]);
        return new AnimationClip("Clip", skeleton, [
            new JointAnimationTrack(
                0,
                Channel(Vector3.Zero, new Vector3(10.0f, 0.0f, 0.0f), 2.0f),
                new QuaternionAnimationChannel([
                    new QuaternionKeyframe(0.0f, Quaternion.Identity),
                    new QuaternionKeyframe(2.0f, Quaternion.CreateFromAxisAngle(Vector3.UnitY, MathF.PI * 0.5f)),
                ]),
                Channel(Vector3.One, new Vector3(3.0f), 2.0f)),
            new JointAnimationTrack(
                1,
                new Vector3AnimationChannel([
                    new Vector3Keyframe(0.5f, new Vector3(1.0f, 0.0f, 0.0f)),
                    new Vector3Keyframe(1.0f, new Vector3(3.0f, 0.0f, 0.0f)),
                ]),
                new QuaternionAnimationChannel([new QuaternionKeyframe(0.0f, Quaternion.Identity)]),
                new Vector3AnimationChannel([new Vector3Keyframe(0.0f, Vector3.One)])),
        ]);
    }

    private static Vector3AnimationChannel Channel(Vector3 start, Vector3 end, float endTime) =>
        new([
            new Vector3Keyframe(0.0f, start),
            new Vector3Keyframe(endTime, end),
        ]);

    private static void AssertQuaternionEquivalent(Quaternion expected, Quaternion actual)
    {
        float dot = MathF.Abs(Quaternion.Dot(expected, actual));
        Assert.Equal(1.0f, dot, 5);
    }

    private static void AssertVector(Vector3 expected, Vector3 actual)
    {
        Assert.Equal(expected.X, actual.X, 5);
        Assert.Equal(expected.Y, actual.Y, 5);
        Assert.Equal(expected.Z, actual.Z, 5);
    }
}
