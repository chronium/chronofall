using System.Numerics;

namespace ChronoFall.CharacterPresentation.Tests;

public sealed class AimOffsetTests
{
    private const float Tolerance = 1e-5f;

    [Fact]
    public void EvaluatorUsesDocumentedYawAndPitchConvention()
    {
        const float yaw = 0.35f;
        const float pitch = 0.2f;
        Vector3 direction = Direction(yaw, pitch);

        AimOffset offset = AimOffsetEvaluator.EvaluateModelSpace(
            Matrix4x4.Identity,
            direction,
            new AimOffsetLimits(0.5f, 0.4f));

        Assert.Equal(yaw, offset.YawRadians, 5);
        Assert.Equal(pitch, offset.PitchRadians, 5);
        Assert.False(offset.WasClamped);
        AssertVector(direction, Vector3.Transform(Vector3.UnitZ, offset.ModelRotationDelta));
    }

    [Fact]
    public void EvaluatorClampsYawAndPitchSymmetrically()
    {
        var limits = new AimOffsetLimits(0.25f, 0.15f);

        AimOffset positive = AimOffsetEvaluator.EvaluateModelSpace(
            Matrix4x4.Identity,
            Direction(1.0f, 0.8f),
            limits);
        AimOffset negative = AimOffsetEvaluator.EvaluateModelSpace(
            Matrix4x4.Identity,
            Direction(-1.0f, -0.8f),
            limits);

        Assert.Equal(0.25f, positive.YawRadians, 5);
        Assert.Equal(0.15f, positive.PitchRadians, 5);
        Assert.Equal(-0.25f, negative.YawRadians, 5);
        Assert.Equal(-0.15f, negative.PitchRadians, 5);
        Assert.True(positive.WasClamped);
        Assert.True(negative.WasClamped);
    }

    [Fact]
    public void ApplierRotatesOneSelectedJointAndPreservesAllLocalTranslationAndScale()
    {
        SkeletonDefinition skeleton = CreateSkeleton();
        SkeletonPose source = skeleton.CreateBindPose();
        var referencePoints = new AttachmentReferencePointSet([
            new AttachmentReferencePointDefinition(
                "aim",
                AttachmentReferencePointRole.Aim,
                JointTransform.Identity),
        ]);
        SkeletonGlobalPose sourceGlobal = SkeletonPoseEvaluator.EvaluateGlobal(source);
        AttachmentReferencePointPose references = AttachmentReferencePointEvaluator.EvaluateModelSpace(
            referencePoints,
            sourceGlobal.GlobalTransforms[0]);
        Assert.True(references.TryGetModelTransform("aim", out Matrix4x4 aimReference));
        Vector3 desiredDirection = Direction(0.3f, 0.15f);
        AimOffset offset = AimOffsetEvaluator.EvaluateModelSpace(
            aimReference,
            desiredDirection,
            new AimOffsetLimits(0.5f, 0.5f));

        SkeletonPose applied = AimOffsetApplier.ApplyModelSpace(source, 0, offset, 1.0f);
        SkeletonGlobalPose appliedGlobal = SkeletonPoseEvaluator.EvaluateGlobal(applied);
        Vector3 actualDirection = Vector3.TransformNormal(
            Vector3.UnitZ,
            RotationMatrix(appliedGlobal.GlobalTransforms[0]));

        AssertVector(desiredDirection, actualDirection);
        Assert.Equal(source.LocalTransforms[0].Translation, applied.LocalTransforms[0].Translation);
        Assert.Equal(source.LocalTransforms[0].Scale, applied.LocalTransforms[0].Scale);
        Assert.Equal(source.LocalTransforms[1], applied.LocalTransforms[1]);
        Assert.Equal(source.LocalTransforms[2], applied.LocalTransforms[2]);
    }

    [Fact]
    public void ApplierPreservesExactZeroAmountAndProducesFinitePartialPose()
    {
        SkeletonDefinition skeleton = CreateSkeleton();
        SkeletonPose source = skeleton.CreateBindPose();
        AimOffset offset = AimOffsetEvaluator.EvaluateModelSpace(
            Matrix4x4.Identity,
            Direction(0.4f, -0.2f),
            new AimOffsetLimits(0.5f, 0.5f));

        SkeletonPose zero = AimOffsetApplier.ApplyModelSpace(source, 0, offset, 0.0f);
        SkeletonPose partial = AimOffsetApplier.ApplyModelSpace(source, 0, offset, 0.5f);

        Assert.Equal(source.LocalTransforms, zero.LocalTransforms);
        Assert.NotEqual(source.LocalTransforms[0].Rotation, partial.LocalTransforms[0].Rotation);
        Assert.All(
            SkeletonPoseEvaluator.EvaluateGlobal(partial).GlobalTransforms,
            static transform => Assert.True(Matrix4x4.Decompose(transform, out _, out _, out _)));
    }

    [Fact]
    public void AimContractsRejectInvalidInputs()
    {
        SkeletonDefinition skeleton = CreateSkeleton();

        Assert.Throws<ArgumentOutOfRangeException>(() => new AimOffsetLimits(-0.1f, 0.1f));
        Assert.Throws<ArgumentOutOfRangeException>(() => new AimOffsetLimits(0.1f, MathF.PI));
        Assert.Throws<ArgumentException>(() => AimOffsetEvaluator.EvaluateModelSpace(
            Matrix4x4.Identity,
            Vector3.Zero,
            new AimOffsetLimits(0.5f, 0.5f)));
        Assert.Throws<ArgumentException>(() => AimOffsetEvaluator.EvaluateModelSpace(
            Matrix4x4.CreateScale(1.0f, 2.0f, 1.0f),
            Vector3.UnitZ,
            new AimOffsetLimits(0.5f, 0.5f)));
        Assert.Throws<ArgumentException>(() => AimOffsetApplier.ApplyModelSpace(
            skeleton.CreateBindPose(),
            0,
            default,
            1.0f));
        Assert.Throws<ArgumentOutOfRangeException>(() => AimOffsetApplier.ApplyModelSpace(
            skeleton.CreateBindPose(),
            skeleton.JointCount,
            AimOffsetEvaluator.EvaluateModelSpace(
                Matrix4x4.Identity,
                Vector3.UnitZ,
                new AimOffsetLimits(0.5f, 0.5f)),
            1.0f));
    }

    private static SkeletonDefinition CreateSkeleton() => new([
        new SkeletonJoint("aim", -1, JointTransform.Identity),
        new SkeletonJoint(
            "child",
            0,
            new JointTransform(Vector3.UnitZ, Quaternion.Identity, Vector3.One)),
        new SkeletonJoint(
            "sibling",
            0,
            new JointTransform(Vector3.UnitX, Quaternion.Identity, Vector3.One)),
    ]);

    private static Vector3 Direction(float yaw, float pitch)
    {
        float cosinePitch = MathF.Cos(pitch);
        return Vector3.Normalize(new Vector3(
            MathF.Sin(yaw) * cosinePitch,
            MathF.Sin(pitch),
            MathF.Cos(yaw) * cosinePitch));
    }

    private static Matrix4x4 RotationMatrix(Matrix4x4 transform)
    {
        Assert.True(Matrix4x4.Decompose(transform, out _, out Quaternion rotation, out _));
        return Matrix4x4.CreateFromQuaternion(rotation);
    }

    private static void AssertVector(Vector3 expected, Vector3 actual)
    {
        Assert.InRange(actual.X, expected.X - Tolerance, expected.X + Tolerance);
        Assert.InRange(actual.Y, expected.Y - Tolerance, expected.Y + Tolerance);
        Assert.InRange(actual.Z, expected.Z - Tolerance, expected.Z + Tolerance);
    }
}
