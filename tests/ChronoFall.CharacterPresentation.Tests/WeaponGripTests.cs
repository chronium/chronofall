using System.Numerics;

namespace ChronoFall.CharacterPresentation.Tests;

public sealed class WeaponGripTests
{
    [Fact]
    public void DefinitionSupportsOneOptionalOffHandTarget()
    {
        JointTransform primaryGrip = CreateTransform(0.25f, 0.0f, 0.0f);
        JointTransform offHandTarget = CreateTransform(-0.5f, 0.0f, 1.0f);

        var oneHanded = new WeaponGripDefinition(primaryGrip);
        var twoHanded = new WeaponGripDefinition(primaryGrip, offHandTarget);

        Assert.Equal(primaryGrip, oneHanded.PrimaryGripLocalTransform);
        Assert.Null(oneHanded.OffHandTargetLocalTransform);
        Assert.Equal(primaryGrip, twoHanded.PrimaryGripLocalTransform);
        Assert.Equal(offHandTarget, twoHanded.OffHandTargetLocalTransform);
    }

    [Fact]
    public void DefinitionRejectsInvalidOrScaledFrames()
    {
        JointTransform scaled = new(Vector3.Zero, Quaternion.Identity, new Vector3(1.0f, 2.0f, 1.0f));

        Assert.Throws<ArgumentException>(() => new WeaponGripDefinition(default));
        Assert.Throws<ArgumentException>(() => new WeaponGripDefinition(scaled));
        Assert.Throws<ArgumentException>(() => new WeaponGripDefinition(JointTransform.Identity, default(JointTransform)));
        Assert.Throws<ArgumentException>(() => new WeaponGripDefinition(JointTransform.Identity, scaled));
    }

    [Fact]
    public void EvaluatorAlignsPrimaryMarkerAndResolvesOffHandTarget()
    {
        var primaryGrip = new JointTransform(
            new Vector3(0.25f, -0.1f, 0.8f),
            Quaternion.CreateFromAxisAngle(Vector3.UnitY, MathF.PI / 3.0f),
            Vector3.One);
        var offHandTarget = new JointTransform(
            new Vector3(-0.2f, 0.15f, 1.4f),
            Quaternion.CreateFromAxisAngle(Vector3.UnitX, -MathF.PI / 4.0f),
            Vector3.One);
        var definition = new WeaponGripDefinition(primaryGrip, offHandTarget);
        Matrix4x4 primarySocketModel =
            Matrix4x4.CreateRotationZ(MathF.PI / 5.0f) *
            Matrix4x4.CreateTranslation(2.0f, 3.0f, 4.0f);

        WeaponGripPlacement placement =
            WeaponGripEvaluator.EvaluateModelSpace(definition, primarySocketModel);

        Assert.Same(definition, placement.Definition);
        AssertMatrix(
            primarySocketModel,
            primaryGrip.ToMatrix() * placement.WeaponModelTransform);
        Assert.NotNull(placement.OffHandTargetModelTransform);
        AssertMatrix(
            offHandTarget.ToMatrix() * placement.WeaponModelTransform,
            placement.OffHandTargetModelTransform.Value);
    }

    [Fact]
    public void EvaluatorLeavesOffHandTargetAbsentForOneHandedDefinition()
    {
        var definition = new WeaponGripDefinition(CreateTransform(0.0f, 0.0f, 0.5f));
        Matrix4x4 primarySocketModel = Matrix4x4.CreateTranslation(3.0f, 2.0f, 1.0f);

        WeaponGripPlacement placement =
            WeaponGripEvaluator.EvaluateModelSpace(definition, primarySocketModel);

        AssertMatrix(
            primarySocketModel,
            definition.PrimaryGripLocalTransform.ToMatrix() * placement.WeaponModelTransform);
        Assert.Null(placement.OffHandTargetModelTransform);
    }

    [Fact]
    public void PlacementAndEvaluatorRejectInvalidOrInconsistentData()
    {
        var oneHanded = new WeaponGripDefinition(JointTransform.Identity);
        var twoHanded = new WeaponGripDefinition(JointTransform.Identity, JointTransform.Identity);

        Assert.Throws<ArgumentException>(() =>
            WeaponGripEvaluator.EvaluateModelSpace(oneHanded, InvalidMatrix()));
        Assert.Throws<ArgumentException>(() =>
            new WeaponGripPlacement(oneHanded, InvalidMatrix(), null));
        Assert.Throws<ArgumentException>(() =>
            new WeaponGripPlacement(oneHanded, Matrix4x4.Identity, Matrix4x4.Identity));
        Assert.Throws<ArgumentException>(() =>
            new WeaponGripPlacement(twoHanded, Matrix4x4.Identity, null));
        Assert.Throws<ArgumentException>(() =>
            new WeaponGripPlacement(twoHanded, Matrix4x4.Identity, InvalidMatrix()));
    }

    [Fact]
    public void EvaluatorConsumesSyntheticSkeletonSocketModelTransform()
    {
        var skeleton = new SkeletonDefinition([
            new SkeletonJoint("root", -1, JointTransform.Identity),
            new SkeletonJoint("hand", 0, CreateTransform(0.0f, 1.0f, 0.0f)),
        ]);
        var pose = new SkeletonPose(skeleton, [
            CreateTransform(2.0f, 0.0f, 0.0f),
            new JointTransform(
                new Vector3(0.0f, 1.0f, 0.0f),
                Quaternion.CreateFromAxisAngle(Vector3.UnitZ, MathF.PI / 2.0f),
                Vector3.One),
        ]);
        var socketSet = new SkeletonSocketSet(
            skeleton,
            [new SkeletonSocketDefinition("primary-hand", 1, CreateTransform(0.1f, 0.0f, 0.0f))]);
        SkeletonSocketPose socketPose = SkeletonSocketEvaluator.EvaluateModelSpace(
            socketSet,
            SkeletonPoseEvaluator.EvaluateGlobal(pose));
        Assert.True(socketPose.TryGetModelTransform("primary-hand", out Matrix4x4 socketModel));
        var definition = new WeaponGripDefinition(
            CreateTransform(0.0f, 0.0f, 0.4f),
            CreateTransform(0.0f, 0.0f, 1.2f));

        WeaponGripPlacement placement = WeaponGripEvaluator.EvaluateModelSpace(definition, socketModel);

        AssertMatrix(
            socketModel,
            definition.PrimaryGripLocalTransform.ToMatrix() * placement.WeaponModelTransform);
        Assert.NotNull(placement.OffHandTargetModelTransform);
        Assert.True(IsFinite(placement.WeaponModelTransform));
        Assert.True(IsFinite(placement.OffHandTargetModelTransform.Value));
    }

    private static JointTransform CreateTransform(float x, float y, float z) =>
        new(new Vector3(x, y, z), Quaternion.Identity, Vector3.One);

    private static Matrix4x4 InvalidMatrix() =>
        new(float.NaN, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1);

    private static bool IsFinite(Matrix4x4 matrix) =>
        float.IsFinite(matrix.M11) && float.IsFinite(matrix.M12) &&
        float.IsFinite(matrix.M13) && float.IsFinite(matrix.M14) &&
        float.IsFinite(matrix.M21) && float.IsFinite(matrix.M22) &&
        float.IsFinite(matrix.M23) && float.IsFinite(matrix.M24) &&
        float.IsFinite(matrix.M31) && float.IsFinite(matrix.M32) &&
        float.IsFinite(matrix.M33) && float.IsFinite(matrix.M34) &&
        float.IsFinite(matrix.M41) && float.IsFinite(matrix.M42) &&
        float.IsFinite(matrix.M43) && float.IsFinite(matrix.M44);

    private static void AssertMatrix(Matrix4x4 expected, Matrix4x4 actual)
    {
        AssertVector(new Vector4(expected.M11, expected.M12, expected.M13, expected.M14), new Vector4(actual.M11, actual.M12, actual.M13, actual.M14));
        AssertVector(new Vector4(expected.M21, expected.M22, expected.M23, expected.M24), new Vector4(actual.M21, actual.M22, actual.M23, actual.M24));
        AssertVector(new Vector4(expected.M31, expected.M32, expected.M33, expected.M34), new Vector4(actual.M31, actual.M32, actual.M33, actual.M34));
        AssertVector(new Vector4(expected.M41, expected.M42, expected.M43, expected.M44), new Vector4(actual.M41, actual.M42, actual.M43, actual.M44));
    }

    private static void AssertVector(Vector4 expected, Vector4 actual)
    {
        Assert.Equal(expected.X, actual.X, 5);
        Assert.Equal(expected.Y, actual.Y, 5);
        Assert.Equal(expected.Z, actual.Z, 5);
        Assert.Equal(expected.W, actual.W, 5);
    }
}
