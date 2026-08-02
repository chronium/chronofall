using System.Numerics;

namespace ChronoFall.CharacterPresentation.Tests;

public sealed class AttachmentReferencePointTests
{
    [Fact]
    public void SetCopiesDefinitionsAndProvidesStableNameAndRoleLookup()
    {
        var source = new[]
        {
            CreatePoint("Muzzle", AttachmentReferencePointRole.Muzzle),
            CreatePoint("muzzle", AttachmentReferencePointRole.Muzzle),
            CreatePoint("sight", AttachmentReferencePointRole.Aim),
        };

        var set = new AttachmentReferencePointSet(source);
        source[0] = CreatePoint("replacement", AttachmentReferencePointRole.CasingEjection);

        Assert.Equal(3, set.ReferencePointCount);
        Assert.Equal("Muzzle", set.ReferencePoints[0].Name);
        Assert.True(set.TryGetReferencePointIndex("Muzzle", out int upperIndex));
        Assert.Equal(0, upperIndex);
        Assert.True(set.TryGetReferencePointIndex("muzzle", out int lowerIndex));
        Assert.Equal(1, lowerIndex);
        Assert.False(set.TryGetReferencePointIndex("missing", out _));
        Assert.Equal(
            new[] { 0, 1 },
            set.GetReferencePointIndices(AttachmentReferencePointRole.Muzzle));
        Assert.Equal(
            new[] { 2 },
            set.GetReferencePointIndices(AttachmentReferencePointRole.Aim));
        Assert.Empty(set.GetReferencePointIndices(AttachmentReferencePointRole.ProjectileOrigin));
    }

    [Fact]
    public void SetAllowsNoReferencePoints()
    {
        var set = new AttachmentReferencePointSet([]);

        Assert.Empty(set.ReferencePoints);
        Assert.Equal(0, set.ReferencePointCount);
        Assert.False(set.TryGetReferencePointIndex("missing", out _));
        Assert.Empty(set.GetReferencePointIndices(AttachmentReferencePointRole.CasingEjection));
    }

    [Fact]
    public void DefinitionsAndSetRejectInvalidData()
    {
        Assert.Throws<ArgumentException>(() =>
            new AttachmentReferencePointDefinition(
                "",
                AttachmentReferencePointRole.Muzzle,
                JointTransform.Identity));
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new AttachmentReferencePointDefinition(
                "invalid-role",
                (AttachmentReferencePointRole)99,
                JointTransform.Identity));
        Assert.Throws<ArgumentException>(() =>
            new AttachmentReferencePointDefinition(
                "invalid-transform",
                AttachmentReferencePointRole.Aim,
                default));
        Assert.Throws<ArgumentException>(() =>
            new AttachmentReferencePointDefinition(
                "scaled",
                AttachmentReferencePointRole.ProjectileOrigin,
                new JointTransform(Vector3.Zero, Quaternion.Identity, new Vector3(1.0f, 2.0f, 1.0f))));
        Assert.Throws<ArgumentException>(() =>
            new AttachmentReferencePointSet(
                [
                    CreatePoint("duplicate", AttachmentReferencePointRole.Muzzle),
                    CreatePoint("duplicate", AttachmentReferencePointRole.Aim),
                ]));
        Assert.Throws<ArgumentException>(() =>
            new AttachmentReferencePointSet([null!]));

        var set = new AttachmentReferencePointSet([]);
        Assert.Throws<ArgumentException>(() => set.TryGetReferencePointIndex("", out _));
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            set.GetReferencePointIndices((AttachmentReferencePointRole)99));
    }

    [Fact]
    public void EvaluatorComposesLocalFrameBeforeAttachmentModelAndPreservesAxes()
    {
        var localFrame = new JointTransform(
            new Vector3(0.25f, 0.5f, 1.0f),
            Quaternion.CreateFromAxisAngle(Vector3.UnitY, MathF.PI / 2.0f),
            Vector3.One);
        var set = new AttachmentReferencePointSet(
            [new AttachmentReferencePointDefinition("primary-muzzle", AttachmentReferencePointRole.Muzzle, localFrame)]);
        Matrix4x4 attachmentModel =
            Matrix4x4.CreateRotationX(MathF.PI / 4.0f) *
            Matrix4x4.CreateTranslation(2.0f, 3.0f, 4.0f);

        AttachmentReferencePointPose pose =
            AttachmentReferencePointEvaluator.EvaluateModelSpace(set, attachmentModel);
        Matrix4x4 expected = localFrame.ToMatrix() * attachmentModel;

        Assert.Same(set, pose.ReferencePointSet);
        Assert.Single(pose.ModelTransforms);
        Assert.Equal(expected, pose.ModelTransforms[0]);
        Assert.True(pose.TryGetModelTransform("primary-muzzle", out Matrix4x4 resolved));
        Assert.Equal(expected, resolved);
        Assert.False(pose.TryGetModelTransform("missing", out Matrix4x4 missing));
        Assert.Equal(default, missing);

        Vector3 origin = Vector3.Transform(Vector3.Zero, expected);
        Vector3 forward = Vector3.Normalize(Vector3.Transform(Vector3.UnitZ, expected) - origin);
        Vector3 up = Vector3.Normalize(Vector3.Transform(Vector3.UnitY, expected) - origin);
        AssertVector(Vector3.UnitX, forward);
        float diagonal = MathF.Sqrt(0.5f);
        AssertVector(new Vector3(0.0f, diagonal, diagonal), up);
    }

    [Fact]
    public void EvaluatorAndPoseRejectInvalidMatricesAndPoseCopiesInput()
    {
        var set = new AttachmentReferencePointSet(
            [CreatePoint("projectile", AttachmentReferencePointRole.ProjectileOrigin)]);
        Matrix4x4 expected = Matrix4x4.CreateTranslation(1.0f, 2.0f, 3.0f);
        Matrix4x4[] source = [expected];

        var pose = new AttachmentReferencePointPose(set, source);
        source[0] = Matrix4x4.Identity;

        Assert.Equal(expected, pose.ModelTransforms[0]);
        Assert.Throws<ArgumentException>(() => new AttachmentReferencePointPose(set, []));
        Assert.Throws<ArgumentException>(() =>
            new AttachmentReferencePointPose(set, [InvalidMatrix()]));
        Assert.Throws<ArgumentException>(() =>
            AttachmentReferencePointEvaluator.EvaluateModelSpace(set, InvalidMatrix()));
    }

    private static AttachmentReferencePointDefinition CreatePoint(
        string name,
        AttachmentReferencePointRole role) =>
        new(name, role, JointTransform.Identity);

    private static Matrix4x4 InvalidMatrix() =>
        new(float.NaN, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1);

    private static void AssertVector(Vector3 expected, Vector3 actual)
    {
        Assert.InRange(MathF.Abs(expected.X - actual.X), 0.0f, 0.00001f);
        Assert.InRange(MathF.Abs(expected.Y - actual.Y), 0.0f, 0.00001f);
        Assert.InRange(MathF.Abs(expected.Z - actual.Z), 0.0f, 0.00001f);
    }
}
