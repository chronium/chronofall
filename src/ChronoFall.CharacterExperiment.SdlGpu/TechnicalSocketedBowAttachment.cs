using System.Numerics;

namespace ChronoFall.CharacterExperiment.SdlGpu;

internal readonly record struct TechnicalSocketedBowFrame(
    SkeletonGlobalPose GlobalPose,
    Matrix4x4 SocketModelTransform,
    Matrix4x4 BowWorldTransform);

internal sealed class TechnicalSocketedBowAttachment
{
    internal const string JointName = "hand_l";
    internal const string SocketName = "technical-primary-hand";
    internal const float DefaultGripOffsetMetres = 0.09f;
    internal const float DefaultPalmDepthMetres = 0.03f;
    internal const float DefaultTwistDegrees = 80.0f;
    internal const float DefaultRollDegrees = -70.0f;

    private readonly SkeletonDefinition skeleton;
    private readonly SkeletonSocketSet sockets;
    private readonly JointTransform bowLocalTransform;

    internal TechnicalSocketedBowAttachment(
        SkeletonDefinition skeleton,
        JointTransform bowLocalTransform)
    {
        ArgumentNullException.ThrowIfNull(skeleton);
        if (!skeleton.TryGetJointIndex(JointName, out int jointIndex))
            throw new InvalidOperationException($"Required technical joint '{JointName}' was not found.");

        if (!IsFinite(bowLocalTransform.Translation) ||
            !IsFinite(bowLocalTransform.Scale) ||
            !IsFinite(bowLocalTransform.Rotation) ||
            MathF.Abs(bowLocalTransform.Rotation.LengthSquared() - 1.0f) > 1e-5f)
        {
            throw new ArgumentException(
                "The technical bow-local transform must contain finite values and a normalized rotation.",
                nameof(bowLocalTransform));
        }
        if (bowLocalTransform.Scale != Vector3.One)
        {
            throw new ArgumentException(
                "The technical bow-local transform must be rigid with identity scale.",
                nameof(bowLocalTransform));
        }

        this.skeleton = skeleton;
        this.bowLocalTransform = bowLocalTransform;
        sockets = new SkeletonSocketSet(
            skeleton,
            [new SkeletonSocketDefinition(SocketName, jointIndex, JointTransform.Identity)]);
    }

    internal static JointTransform DefaultBowLocalTransform => CreateBowLocalTransform(
        DefaultGripOffsetMetres,
        DefaultPalmDepthMetres,
        DefaultTwistDegrees,
        DefaultRollDegrees);

    internal static JointTransform CreateBowLocalTransform(
        float gripOffsetMetres,
        float palmDepthMetres,
        float twistDegrees,
        float rollDegrees) =>
        new(
            new Vector3(palmDepthMetres, gripOffsetMetres, 0.0f),
            Quaternion.CreateFromRotationMatrix(
                Matrix4x4.CreateRotationY(twistDegrees * MathF.PI / 180.0f) *
                Matrix4x4.CreateRotationX(rollDegrees * MathF.PI / 180.0f)),
            Vector3.One);

    internal int JointIndex => sockets.Sockets[0].JointIndex;

    internal JointTransform BowLocalTransform => bowLocalTransform;

    internal TechnicalSocketedBowFrame Evaluate(
        SkeletonGlobalPose globalPose,
        Matrix4x4 characterWorld)
    {
        ArgumentNullException.ThrowIfNull(globalPose);
        if (!ReferenceEquals(globalPose.Skeleton, skeleton))
            throw new ArgumentException("The technical bow attachment requires its configured skeleton.", nameof(globalPose));

        SkeletonSocketPose socketPose = SkeletonSocketEvaluator.EvaluateModelSpace(sockets, globalPose);
        if (!socketPose.TryGetModelTransform(SocketName, out Matrix4x4 socketModel))
            throw new InvalidOperationException("The technical primary-hand socket did not resolve.");

        Matrix4x4 bowWorld = bowLocalTransform.ToMatrix() * socketModel * characterWorld;
        _ = new StaticMeshDraw(bowWorld, Matrix4x4.Identity, Vector3.One, Vector3.UnitY);
        return new TechnicalSocketedBowFrame(globalPose, socketModel, bowWorld);
    }

    private static bool IsFinite(Vector3 value) =>
        float.IsFinite(value.X) && float.IsFinite(value.Y) && float.IsFinite(value.Z);

    private static bool IsFinite(Quaternion value) =>
        float.IsFinite(value.X) && float.IsFinite(value.Y) &&
        float.IsFinite(value.Z) && float.IsFinite(value.W);
}
