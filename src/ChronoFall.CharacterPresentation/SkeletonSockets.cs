using System.Collections.ObjectModel;
using System.Numerics;

namespace ChronoFall.CharacterPresentation;

public sealed class SkeletonSocketDefinition
{
    public SkeletonSocketDefinition(
        string name,
        int jointIndex,
        JointTransform localTransform)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        if (jointIndex < 0)
            throw new ArgumentOutOfRangeException(nameof(jointIndex), "Socket joint index must be non-negative.");

        localTransform.Validate(nameof(localTransform));
        Name = name;
        JointIndex = jointIndex;
        LocalTransform = localTransform;
    }

    public string Name { get; }

    public int JointIndex { get; }

    public JointTransform LocalTransform { get; }
}

public sealed class SkeletonSocketSet
{
    private readonly IReadOnlyDictionary<string, int> socketIndicesByName;

    public SkeletonSocketSet(
        SkeletonDefinition skeleton,
        IEnumerable<SkeletonSocketDefinition> sockets)
    {
        ArgumentNullException.ThrowIfNull(skeleton);
        ArgumentNullException.ThrowIfNull(sockets);

        SkeletonSocketDefinition[] copy = sockets.ToArray();
        var indicesByName = new Dictionary<string, int>(copy.Length, StringComparer.Ordinal);
        for (int index = 0; index < copy.Length; index++)
        {
            SkeletonSocketDefinition socket = copy[index] ??
                throw new ArgumentException($"Socket {index} cannot be null.", nameof(sockets));
            if (socket.JointIndex >= skeleton.JointCount)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(sockets),
                    $"Socket {index} ('{socket.Name}') references joint {socket.JointIndex}, " +
                    $"but the skeleton has {skeleton.JointCount} joints.");
            }
            if (!indicesByName.TryAdd(socket.Name, index))
                throw new ArgumentException($"Socket name '{socket.Name}' is duplicated.", nameof(sockets));
        }

        Skeleton = skeleton;
        Sockets = Array.AsReadOnly(copy);
        socketIndicesByName = new ReadOnlyDictionary<string, int>(indicesByName);
    }

    public SkeletonDefinition Skeleton { get; }

    public IReadOnlyList<SkeletonSocketDefinition> Sockets { get; }

    public int SocketCount => Sockets.Count;

    public bool TryGetSocketIndex(string name, out int socketIndex)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        return socketIndicesByName.TryGetValue(name, out socketIndex);
    }
}

public sealed class SkeletonSocketPose
{
    public SkeletonSocketPose(
        SkeletonSocketSet socketSet,
        IEnumerable<Matrix4x4> modelTransforms)
    {
        ArgumentNullException.ThrowIfNull(socketSet);
        ArgumentNullException.ThrowIfNull(modelTransforms);

        Matrix4x4[] copy = modelTransforms.ToArray();
        if (copy.Length != socketSet.SocketCount)
        {
            throw new ArgumentException(
                $"Expected {socketSet.SocketCount} socket model transforms, but received {copy.Length}.",
                nameof(modelTransforms));
        }

        for (int index = 0; index < copy.Length; index++)
            DataValidation.RequireFinite(copy[index], nameof(modelTransforms), $"Socket model transform {index}");

        SocketSet = socketSet;
        ModelTransforms = Array.AsReadOnly(copy);
    }

    public SkeletonSocketSet SocketSet { get; }

    public IReadOnlyList<Matrix4x4> ModelTransforms { get; }

    public bool TryGetModelTransform(string socketName, out Matrix4x4 modelTransform)
    {
        if (SocketSet.TryGetSocketIndex(socketName, out int socketIndex))
        {
            modelTransform = ModelTransforms[socketIndex];
            return true;
        }

        modelTransform = default;
        return false;
    }
}

public static class SkeletonSocketEvaluator
{
    public static SkeletonSocketPose EvaluateModelSpace(
        SkeletonSocketSet socketSet,
        SkeletonGlobalPose globalPose)
    {
        ArgumentNullException.ThrowIfNull(socketSet);
        ArgumentNullException.ThrowIfNull(globalPose);
        if (!ReferenceEquals(socketSet.Skeleton, globalPose.Skeleton))
        {
            throw new ArgumentException(
                "The socket set and global pose must use the same skeleton instance.",
                nameof(globalPose));
        }

        var transforms = new Matrix4x4[socketSet.SocketCount];
        for (int index = 0; index < transforms.Length; index++)
        {
            SkeletonSocketDefinition socket = socketSet.Sockets[index];
            transforms[index] =
                socket.LocalTransform.ToMatrix() *
                globalPose.GlobalTransforms[socket.JointIndex];
        }

        return new SkeletonSocketPose(socketSet, transforms);
    }
}
