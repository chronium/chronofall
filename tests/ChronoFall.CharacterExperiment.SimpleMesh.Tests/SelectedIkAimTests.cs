using System.Numerics;

namespace ChronoFall.CharacterExperiment.SimpleMesh.Tests;

public sealed class SelectedIkAimTests
{
    private const float PositionTolerance = 2e-3f;

    [Fact]
    public void SelectedRigComposesAimGripAndOffHandIkIntoFinitePose()
    {
        SkeletalCharacterAsset asset = LoadAsset();
        SkeletonDefinition skeleton = asset.Mesh.Skin.Skeleton;
        int spine = FindJoint(skeleton, "spine_03");
        int primaryHand = FindJoint(skeleton, "hand_r");
        int root = FindJoint(skeleton, "upperarm_l");
        int middle = FindJoint(skeleton, "lowerarm_l");
        int end = FindJoint(skeleton, "hand_l");
        var chain = new TwoBoneIkChain(skeleton, root, middle, end);
        AnimationClip attack = Assert.Single(asset.Animations, clip => clip.Name == "Sword_Attack");
        SkeletonPose source = AnimationSampler.Sample(attack, 0.75f, AnimationPlaybackMode.Clamp);

        PresentationProbe sourceProbe = CreateProbe(source, primaryHand, root, middle, end);
        AimOffset aim = AimOffsetEvaluator.EvaluateModelSpace(
            sourceProbe.AimReferenceModel,
            OffsetDirection(sourceProbe.AimReferenceModel, Degrees(20.0f), Degrees(10.0f)),
            new AimOffsetLimits(Degrees(25.0f), Degrees(15.0f)));
        SkeletonPose aimed = AimOffsetApplier.ApplyModelSpace(source, spine, aim, 1.0f);
        PresentationProbe aimedProbe = CreateProbe(aimed, primaryHand, root, middle, end);
        SkeletonPose solved = TwoBoneIkSolver.ApplyModelSpace(
            aimed,
            chain,
            aimedProbe.GripPlacement.OffHandTargetModelTransform!.Value,
            aimedProbe.PoleModelPosition,
            1.0f);
        SkeletonGlobalPose solvedGlobal = SkeletonPoseEvaluator.EvaluateGlobal(solved);
        SkinningPalette palette = SkeletonPoseEvaluator.CreateSkinningPalette(asset.Mesh.Skin, solvedGlobal);

        Vector3 target = Position(aimedProbe.GripPlacement.OffHandTargetModelTransform.Value);
        Vector3 actual = Position(solvedGlobal.GlobalTransforms[end]);
        Assert.InRange(Vector3.Distance(target, actual), 0.0f, PositionTolerance);
        Assert.All(solvedGlobal.GlobalTransforms, AssertFinite);
        Assert.All(palette.JointMatrices, AssertFinite);
        Assert.Equal(65, palette.JointMatrices.Count);
    }

    private static PresentationProbe CreateProbe(
        SkeletonPose pose,
        int primaryHand,
        int chainRoot,
        int chainMiddle,
        int chainEnd)
    {
        SkeletonGlobalPose global = SkeletonPoseEvaluator.EvaluateGlobal(pose);
        var sockets = new SkeletonSocketSet(
            pose.Skeleton,
            [new SkeletonSocketDefinition("primary-hand", primaryHand, JointTransform.Identity)]);
        SkeletonSocketPose socketPose = SkeletonSocketEvaluator.EvaluateModelSpace(sockets, global);
        Assert.True(socketPose.TryGetModelTransform("primary-hand", out Matrix4x4 primarySocket));

        Vector3 rootPosition = Position(global.GlobalTransforms[chainRoot]);
        Vector3 middlePosition = Position(global.GlobalTransforms[chainMiddle]);
        Vector3 endPosition = Position(global.GlobalTransforms[chainEnd]);
        float reach = Vector3.Distance(rootPosition, middlePosition) + Vector3.Distance(middlePosition, endPosition);
        Matrix4x4 endRelativeToWeapon = global.GlobalTransforms[chainEnd] * Invert(primarySocket);
        Assert.True(Matrix4x4.Decompose(endRelativeToWeapon, out _, out Quaternion localRotation, out Vector3 localTranslation));
        Vector3 towardPrimary = localTranslation.LengthSquared() > 1e-8f
            ? -Vector3.Normalize(localTranslation)
            : Vector3.UnitZ;
        var grip = new WeaponGripDefinition(
            JointTransform.Identity,
            new JointTransform(
                localTranslation + towardPrimary * (reach * 0.15f),
                localRotation,
                Vector3.One));
        WeaponGripPlacement placement = WeaponGripEvaluator.EvaluateModelSpace(grip, primarySocket);
        var referencePoints = new AttachmentReferencePointSet([
            new AttachmentReferencePointDefinition(
                "aim",
                AttachmentReferencePointRole.Aim,
                JointTransform.Identity),
        ]);
        AttachmentReferencePointPose referencePose = AttachmentReferencePointEvaluator.EvaluateModelSpace(
            referencePoints,
            placement.WeaponModelTransform);
        Assert.True(referencePose.TryGetModelTransform("aim", out Matrix4x4 aimReference));
        return new PresentationProbe(placement, aimReference, middlePosition);
    }

    private static Vector3 OffsetDirection(Matrix4x4 reference, float yaw, float pitch)
    {
        Assert.True(Matrix4x4.Decompose(reference, out _, out Quaternion rotation, out _));
        float cosinePitch = MathF.Cos(pitch);
        Vector3 local = new(
            MathF.Sin(yaw) * cosinePitch,
            MathF.Sin(pitch),
            MathF.Cos(yaw) * cosinePitch);
        return Vector3.Normalize(Vector3.Transform(local, rotation));
    }

    private static int FindJoint(SkeletonDefinition skeleton, string name)
    {
        Assert.True(skeleton.TryGetJointIndex(name, out int index));
        return index;
    }

    private static Matrix4x4 Invert(Matrix4x4 transform)
    {
        Assert.True(Matrix4x4.Invert(transform, out Matrix4x4 inverse));
        return inverse;
    }

    private static Vector3 Position(Matrix4x4 transform) =>
        new(transform.M41, transform.M42, transform.M43);

    private static float Degrees(float degrees) => degrees * MathF.PI / 180.0f;

    private static void AssertFinite(Matrix4x4 matrix)
    {
        Assert.All(new[]
        {
            matrix.M11, matrix.M12, matrix.M13, matrix.M14,
            matrix.M21, matrix.M22, matrix.M23, matrix.M24,
            matrix.M31, matrix.M32, matrix.M33, matrix.M34,
            matrix.M41, matrix.M42, matrix.M43, matrix.M44,
        }, static value => Assert.True(float.IsFinite(value)));
    }

    private static SkeletalCharacterAsset LoadAsset()
    {
        string path = Path.Combine(
            RepositoryPaths.Root,
            "assets",
            "Quaternius",
            "Universal Animation Library[Standard]",
            "Unreal-Godot",
            "UAL1_Standard.glb");
        return SimpleMeshSkeletalAssetLoader.LoadFromFile(path);
    }

    private sealed record PresentationProbe(
        WeaponGripPlacement GripPlacement,
        Matrix4x4 AimReferenceModel,
        Vector3 PoleModelPosition);
}
