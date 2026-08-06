using System.Numerics;

namespace ChronoFall.CharacterExperiment.SdlGpu.Tests;

public sealed class TechnicalSocketedBowAttachmentTests
{
    [Fact]
    public void DefaultTransformFreezesOwnerValidatedTechnicalGrip()
    {
        Assert.Equal(0.09f, TechnicalSocketedBowAttachment.DefaultGripOffsetMetres);
        Assert.Equal(0.03f, TechnicalSocketedBowAttachment.DefaultPalmDepthMetres);
        Assert.Equal(80.0f, TechnicalSocketedBowAttachment.DefaultTwistDegrees);
        Assert.Equal(-70.0f, TechnicalSocketedBowAttachment.DefaultRollDegrees);

        JointTransform expected = TechnicalSocketedBowAttachment.CreateBowLocalTransform(
            0.09f,
            0.03f,
            80.0f,
            -70.0f);

        Assert.Equal(expected, TechnicalSocketedBowAttachment.DefaultBowLocalTransform);
    }

    [Fact]
    public void EvaluatesBowLocalSocketAndCharacterWorldInRowVectorOrder()
    {
        SkeletonDefinition skeleton = CreateSkeleton();
        var bowLocal = new JointTransform(
            new Vector3(0.1f, 0.2f, 0.3f),
            Quaternion.CreateFromAxisAngle(Vector3.UnitZ, 0.25f),
            Vector3.One);
        var attachment = new TechnicalSocketedBowAttachment(skeleton, bowLocal);
        SkeletonPose pose = new(
            skeleton,
            [
                JointTransform.Identity,
                new JointTransform(
                    new Vector3(1.0f, 2.0f, 3.0f),
                    Quaternion.CreateFromAxisAngle(Vector3.UnitY, 0.5f),
                    Vector3.One),
            ]);
        SkeletonGlobalPose global = SkeletonPoseEvaluator.EvaluateGlobal(pose);
        Matrix4x4 world =
            Matrix4x4.CreateRotationY(-0.3f) *
            Matrix4x4.CreateTranslation(4.0f, 0.0f, -2.0f);

        TechnicalSocketedBowFrame result = attachment.Evaluate(global, world);

        Assert.Equal(1, attachment.JointIndex);
        Assert.Equal(global.GlobalTransforms[1], result.SocketModelTransform);
        AssertMatrixEqual(
            bowLocal.ToMatrix() * global.GlobalTransforms[1] * world,
            result.BowWorldTransform);
        _ = new StaticMeshDraw(
            result.BowWorldTransform,
            Matrix4x4.Identity,
            Vector3.One,
            Vector3.UnitY);
    }

    [Fact]
    public void PosedHandMotionChangesBowTransformAndRepeatedEvaluationIsExact()
    {
        SkeletonDefinition skeleton = CreateSkeleton();
        var attachment = new TechnicalSocketedBowAttachment(
            skeleton,
            TechnicalSocketedBowAttachment.DefaultBowLocalTransform);
        SkeletonGlobalPose first = SkeletonPoseEvaluator.EvaluateGlobal(
            new SkeletonPose(skeleton, [JointTransform.Identity, JointTransform.Identity]));
        SkeletonGlobalPose second = SkeletonPoseEvaluator.EvaluateGlobal(
            new SkeletonPose(
                skeleton,
                [
                    JointTransform.Identity,
                    new JointTransform(new Vector3(0.5f, 0.25f, -0.1f), Quaternion.Identity, Vector3.One),
                ]));

        TechnicalSocketedBowFrame firstResult = attachment.Evaluate(first, Matrix4x4.Identity);
        TechnicalSocketedBowFrame repeated = attachment.Evaluate(first, Matrix4x4.Identity);
        TechnicalSocketedBowFrame secondResult = attachment.Evaluate(second, Matrix4x4.Identity);

        Assert.Equal(firstResult.BowWorldTransform, repeated.BowWorldTransform);
        Assert.NotEqual(firstResult.BowWorldTransform, secondResult.BowWorldTransform);
    }

    [Fact]
    public void RejectsMissingJointMismatchedSkeletonAndScaledLocalTransform()
    {
        SkeletonDefinition missing = new(
            [new SkeletonJoint("root", -1, JointTransform.Identity)]);
        InvalidOperationException missingException = Assert.Throws<InvalidOperationException>(
            () => new TechnicalSocketedBowAttachment(missing, JointTransform.Identity));
        Assert.Contains("hand_l", missingException.Message, StringComparison.Ordinal);

        SkeletonDefinition skeleton = CreateSkeleton();
        Assert.Throws<ArgumentException>(
            () => new TechnicalSocketedBowAttachment(
                skeleton,
                new JointTransform(Vector3.Zero, Quaternion.Identity, new Vector3(2.0f))));

        var attachment = new TechnicalSocketedBowAttachment(skeleton, JointTransform.Identity);
        SkeletonDefinition other = CreateSkeleton();
        SkeletonGlobalPose otherPose = SkeletonPoseEvaluator.EvaluateGlobal(other.CreateBindPose());
        Assert.Throws<ArgumentException>(() => attachment.Evaluate(otherPose, Matrix4x4.Identity));
    }

    private static SkeletonDefinition CreateSkeleton() =>
        new(
            [
                new SkeletonJoint("root", -1, JointTransform.Identity),
                new SkeletonJoint("hand_l", 0, JointTransform.Identity),
            ]);

    private static void AssertMatrixEqual(Matrix4x4 expected, Matrix4x4 actual)
    {
        Assert.Equal(expected.M11, actual.M11);
        Assert.Equal(expected.M12, actual.M12);
        Assert.Equal(expected.M13, actual.M13);
        Assert.Equal(expected.M14, actual.M14);
        Assert.Equal(expected.M21, actual.M21);
        Assert.Equal(expected.M22, actual.M22);
        Assert.Equal(expected.M23, actual.M23);
        Assert.Equal(expected.M24, actual.M24);
        Assert.Equal(expected.M31, actual.M31);
        Assert.Equal(expected.M32, actual.M32);
        Assert.Equal(expected.M33, actual.M33);
        Assert.Equal(expected.M34, actual.M34);
        Assert.Equal(expected.M41, actual.M41);
        Assert.Equal(expected.M42, actual.M42);
        Assert.Equal(expected.M43, actual.M43);
        Assert.Equal(expected.M44, actual.M44);
    }
}
