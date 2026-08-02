using System.Numerics;

namespace ChronoFall.CharacterPresentation.Tests;

public sealed class TwoBoneIkTests
{
    private const float Tolerance = 1e-4f;

    [Fact]
    public void ChainRequiresDirectParentChildTopology()
    {
        SkeletonDefinition skeleton = CreateSkeleton();

        var chain = new TwoBoneIkChain(skeleton, 0, 1, 2);

        Assert.Same(skeleton, chain.Skeleton);
        Assert.Equal(0, chain.RootJointIndex);
        Assert.Equal(1, chain.MiddleJointIndex);
        Assert.Equal(2, chain.EndJointIndex);
        Assert.Throws<ArgumentOutOfRangeException>(() => new TwoBoneIkChain(skeleton, -1, 1, 2));
        Assert.Throws<ArgumentException>(() => new TwoBoneIkChain(skeleton, 0, 3, 2));
        Assert.Throws<ArgumentException>(() => new TwoBoneIkChain(skeleton, 0, 1, 3));
    }

    [Fact]
    public void FullSolveReachesTargetAndAlignsEndOrientation()
    {
        SkeletonDefinition skeleton = CreateSkeleton();
        SkeletonPose source = skeleton.CreateBindPose();
        var chain = new TwoBoneIkChain(skeleton, 0, 1, 2);
        Quaternion targetRotation = Quaternion.CreateFromAxisAngle(Vector3.UnitY, 0.45f);
        Matrix4x4 target =
            Matrix4x4.CreateFromQuaternion(targetRotation) *
            Matrix4x4.CreateTranslation(1.0f, 1.0f, 0.0f);

        SkeletonPose solved = TwoBoneIkSolver.ApplyModelSpace(
            source,
            chain,
            target,
            new Vector3(0.0f, 0.0f, 1.0f),
            1.0f);
        SkeletonGlobalPose global = SkeletonPoseEvaluator.EvaluateGlobal(solved);

        AssertVector(new Vector3(1.0f, 1.0f, 0.0f), Position(global, 2));
        AssertRotationEquivalent(targetRotation, Rotation(global, 2));
        Assert.True(Position(global, 1).Z > 0.0f);
        Assert.Equal(source.LocalTransforms[0].Translation, solved.LocalTransforms[0].Translation);
        Assert.Equal(source.LocalTransforms[1].Translation, solved.LocalTransforms[1].Translation);
        Assert.Equal(source.LocalTransforms[2].Translation, solved.LocalTransforms[2].Translation);
        Assert.Equal(source.LocalTransforms[3], solved.LocalTransforms[3]);
    }

    [Fact]
    public void PoleSelectsBendSideAndDegeneratePoleFallsBackDeterministically()
    {
        SkeletonDefinition skeleton = CreateSkeleton();
        SkeletonPose source = skeleton.CreateBindPose();
        var chain = new TwoBoneIkChain(skeleton, 0, 1, 2);
        Matrix4x4 target = Matrix4x4.CreateTranslation(1.0f, 0.0f, 0.0f);

        SkeletonPose positive = TwoBoneIkSolver.ApplyModelSpace(
            source,
            chain,
            target,
            new Vector3(0.0f, 1.0f, 0.0f),
            1.0f);
        SkeletonPose negative = TwoBoneIkSolver.ApplyModelSpace(
            source,
            chain,
            target,
            new Vector3(0.0f, -1.0f, 0.0f),
            1.0f);
        SkeletonPose fallbackA = TwoBoneIkSolver.ApplyModelSpace(
            source,
            chain,
            target,
            new Vector3(2.0f, 0.0f, 0.0f),
            1.0f);
        SkeletonPose fallbackB = TwoBoneIkSolver.ApplyModelSpace(
            source,
            chain,
            target,
            new Vector3(2.0f, 0.0f, 0.0f),
            1.0f);

        Assert.True(Position(SkeletonPoseEvaluator.EvaluateGlobal(positive), 1).Y > 0.0f);
        Assert.True(Position(SkeletonPoseEvaluator.EvaluateGlobal(negative), 1).Y < 0.0f);
        Assert.Equal(fallbackA.LocalTransforms, fallbackB.LocalTransforms);
    }

    [Fact]
    public void SolverClampsUnreachableTargetAndPreservesAmountEndpoints()
    {
        SkeletonDefinition skeleton = CreateSkeleton();
        SkeletonPose source = skeleton.CreateBindPose();
        var chain = new TwoBoneIkChain(skeleton, 0, 1, 2);
        Matrix4x4 target = Matrix4x4.CreateTranslation(5.0f, 0.0f, 0.0f);

        SkeletonPose zero = TwoBoneIkSolver.ApplyModelSpace(
            source,
            chain,
            target,
            Vector3.UnitY,
            0.0f);
        SkeletonPose full = TwoBoneIkSolver.ApplyModelSpace(
            source,
            chain,
            target,
            Vector3.UnitY,
            1.0f);

        Assert.Equal(source.LocalTransforms, zero.LocalTransforms);
        AssertVector(new Vector3(2.0f, 0.0f, 0.0f), Position(SkeletonPoseEvaluator.EvaluateGlobal(full), 2));
    }

    [Fact]
    public void SolverHandlesFullyFoldedEqualLengthChainDeterministically()
    {
        var skeleton = new SkeletonDefinition([
            new SkeletonJoint("root", -1, JointTransform.Identity),
            new SkeletonJoint("middle", 0, CreateTransform(1.0f, 0.0f, 0.0f)),
            new SkeletonJoint("end", 1, CreateTransform(-1.0f, 0.0f, 0.0f)),
        ]);
        var chain = new TwoBoneIkChain(skeleton, 0, 1, 2);

        SkeletonPose first = TwoBoneIkSolver.ApplyModelSpace(
            skeleton.CreateBindPose(),
            chain,
            Matrix4x4.Identity,
            Vector3.UnitY,
            1.0f);
        SkeletonPose second = TwoBoneIkSolver.ApplyModelSpace(
            skeleton.CreateBindPose(),
            chain,
            Matrix4x4.Identity,
            Vector3.UnitY,
            1.0f);

        Assert.Equal(first.LocalTransforms, second.LocalTransforms);
        AssertVector(Vector3.Zero, Position(SkeletonPoseEvaluator.EvaluateGlobal(first), 2));
        Assert.True(Position(SkeletonPoseEvaluator.EvaluateGlobal(first), 1).Y > 0.0f);
    }

    [Fact]
    public void SolverConsumesWeaponGripOffHandTarget()
    {
        SkeletonDefinition skeleton = CreateSkeleton();
        var chain = new TwoBoneIkChain(skeleton, 0, 1, 2);
        var grip = new WeaponGripDefinition(
            JointTransform.Identity,
            new JointTransform(new Vector3(-1.0f, 1.0f, 0.0f), Quaternion.Identity, Vector3.One));
        WeaponGripPlacement placement = WeaponGripEvaluator.EvaluateModelSpace(
            grip,
            Matrix4x4.CreateTranslation(2.0f, 0.0f, 0.0f));

        SkeletonPose solved = TwoBoneIkSolver.ApplyModelSpace(
            skeleton.CreateBindPose(),
            chain,
            placement.OffHandTargetModelTransform!.Value,
            Vector3.UnitZ,
            1.0f);

        AssertVector(
            new Vector3(1.0f, 1.0f, 0.0f),
            Position(SkeletonPoseEvaluator.EvaluateGlobal(solved), 2));
    }

    [Fact]
    public void SolverRejectsInvalidInputsAndUnsupportedTransforms()
    {
        SkeletonDefinition skeleton = CreateSkeleton();
        SkeletonDefinition differentSkeleton = CreateSkeleton();
        var chain = new TwoBoneIkChain(skeleton, 0, 1, 2);
        var zeroLengthSkeleton = new SkeletonDefinition([
            new SkeletonJoint("root", -1, JointTransform.Identity),
            new SkeletonJoint("middle", 0, JointTransform.Identity),
            new SkeletonJoint("end", 1, CreateTransform(1.0f, 0.0f, 0.0f)),
        ]);
        var zeroLengthChain = new TwoBoneIkChain(zeroLengthSkeleton, 0, 1, 2);

        Assert.Throws<ArgumentException>(() => TwoBoneIkSolver.ApplyModelSpace(
            differentSkeleton.CreateBindPose(), chain, Matrix4x4.Identity, Vector3.UnitY, 1.0f));
        Assert.Throws<ArgumentException>(() => TwoBoneIkSolver.ApplyModelSpace(
            skeleton.CreateBindPose(), chain, Matrix4x4.CreateScale(1.0f, 2.0f, 1.0f), Vector3.UnitY, 1.0f));
        Assert.Throws<ArgumentException>(() => TwoBoneIkSolver.ApplyModelSpace(
            skeleton.CreateBindPose(), chain, Matrix4x4.Identity, new Vector3(float.NaN, 0.0f, 0.0f), 1.0f));
        Assert.Throws<ArgumentOutOfRangeException>(() => TwoBoneIkSolver.ApplyModelSpace(
            skeleton.CreateBindPose(), chain, Matrix4x4.Identity, Vector3.UnitY, -0.1f));
        Assert.Throws<ArgumentException>(() => TwoBoneIkSolver.ApplyModelSpace(
            zeroLengthSkeleton.CreateBindPose(), zeroLengthChain, Matrix4x4.Identity, Vector3.UnitY, 1.0f));
    }

    private static SkeletonDefinition CreateSkeleton() => new([
        new SkeletonJoint("root", -1, JointTransform.Identity),
        new SkeletonJoint("middle", 0, CreateTransform(1.0f, 0.0f, 0.0f)),
        new SkeletonJoint("end", 1, CreateTransform(1.0f, 0.0f, 0.0f)),
        new SkeletonJoint("unrelated", 0, CreateTransform(0.0f, 2.0f, 0.0f)),
    ]);

    private static JointTransform CreateTransform(float x, float y, float z) =>
        new(new Vector3(x, y, z), Quaternion.Identity, Vector3.One);

    private static Vector3 Position(SkeletonGlobalPose pose, int jointIndex)
    {
        Matrix4x4 transform = pose.GlobalTransforms[jointIndex];
        return new Vector3(transform.M41, transform.M42, transform.M43);
    }

    private static Quaternion Rotation(SkeletonGlobalPose pose, int jointIndex)
    {
        Assert.True(Matrix4x4.Decompose(pose.GlobalTransforms[jointIndex], out _, out Quaternion rotation, out _));
        return rotation;
    }

    private static void AssertVector(Vector3 expected, Vector3 actual)
    {
        Assert.InRange(actual.X, expected.X - Tolerance, expected.X + Tolerance);
        Assert.InRange(actual.Y, expected.Y - Tolerance, expected.Y + Tolerance);
        Assert.InRange(actual.Z, expected.Z - Tolerance, expected.Z + Tolerance);
    }

    private static void AssertRotationEquivalent(Quaternion expected, Quaternion actual)
    {
        float dot = MathF.Abs(Quaternion.Dot(expected, actual));
        Assert.InRange(dot, 1.0f - Tolerance, 1.0f + Tolerance);
    }
}
