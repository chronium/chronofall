using System.Collections.ObjectModel;
using System.Numerics;

namespace ChronoFall.CharacterExperiment;

public sealed class SkeletonJoint
{
    public SkeletonJoint(string name, int parentIndex, JointTransform localBindTransform)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        if (parentIndex < -1)
            throw new ArgumentOutOfRangeException(nameof(parentIndex), "Parent index must be -1 or a non-negative joint index.");

        localBindTransform.Validate(nameof(localBindTransform));
        Name = name;
        ParentIndex = parentIndex;
        LocalBindTransform = localBindTransform;
    }

    public string Name { get; }

    public int ParentIndex { get; }

    public JointTransform LocalBindTransform { get; }
}

public sealed class SkeletonDefinition
{
    private readonly IReadOnlyDictionary<string, int> _jointIndicesByName;

    public SkeletonDefinition(IEnumerable<SkeletonJoint> joints)
    {
        ArgumentNullException.ThrowIfNull(joints);
        SkeletonJoint[] copy = joints.ToArray();
        if (copy.Length == 0)
            throw new ArgumentException("A skeleton must contain at least one joint.", nameof(joints));

        var indicesByName = new Dictionary<string, int>(copy.Length, StringComparer.Ordinal);
        for (int index = 0; index < copy.Length; index++)
        {
            SkeletonJoint joint = copy[index] ??
                throw new ArgumentException($"Joint {index} cannot be null.", nameof(joints));

            if (!indicesByName.TryAdd(joint.Name, index))
                throw new ArgumentException($"Joint name '{joint.Name}' is duplicated.", nameof(joints));

            if (index == 0)
            {
                if (joint.ParentIndex != -1)
                    throw new ArgumentException("Joint 0 must be the single skeleton root with parent index -1.", nameof(joints));
            }
            else if (joint.ParentIndex < 0 || joint.ParentIndex >= index)
            {
                throw new ArgumentException(
                    $"Joint {index} ('{joint.Name}') must reference an earlier parent joint.",
                    nameof(joints));
            }
        }

        Joints = Array.AsReadOnly(copy);
        _jointIndicesByName = new ReadOnlyDictionary<string, int>(indicesByName);
    }

    public IReadOnlyList<SkeletonJoint> Joints { get; }

    public int JointCount => Joints.Count;

    public bool TryGetJointIndex(string name, out int jointIndex)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        return _jointIndicesByName.TryGetValue(name, out jointIndex);
    }

    public SkeletonPose CreateBindPose() =>
        new(this, Joints.Select(static joint => joint.LocalBindTransform));
}

public sealed class SkinDefinition
{
    public SkinDefinition(SkeletonDefinition skeleton, IEnumerable<Matrix4x4> inverseBindMatrices)
    {
        ArgumentNullException.ThrowIfNull(skeleton);
        ArgumentNullException.ThrowIfNull(inverseBindMatrices);

        Matrix4x4[] copy = inverseBindMatrices.ToArray();
        if (copy.Length != skeleton.JointCount)
        {
            throw new ArgumentException(
                $"Expected {skeleton.JointCount} inverse-bind matrices, but received {copy.Length}.",
                nameof(inverseBindMatrices));
        }

        for (int index = 0; index < copy.Length; index++)
            DataValidation.RequireFinite(copy[index], nameof(inverseBindMatrices), $"Inverse-bind matrix {index}");

        Skeleton = skeleton;
        InverseBindMatrices = Array.AsReadOnly(copy);
    }

    public SkeletonDefinition Skeleton { get; }

    public IReadOnlyList<Matrix4x4> InverseBindMatrices { get; }
}

public readonly record struct JointIndices4
{
    public JointIndices4(int x, int y, int z, int w)
    {
        if (x < 0 || y < 0 || z < 0 || w < 0)
            throw new ArgumentOutOfRangeException(nameof(x), "Joint indices must be non-negative.");

        X = x;
        Y = y;
        Z = z;
        W = w;
    }

    public int X { get; }

    public int Y { get; }

    public int Z { get; }

    public int W { get; }

    public int this[int lane] => lane switch
    {
        0 => X,
        1 => Y,
        2 => Z,
        3 => W,
        _ => throw new ArgumentOutOfRangeException(nameof(lane)),
    };
}

public readonly record struct SkinInfluences
{
    public const float WeightSumTolerance = 1e-4f;

    public SkinInfluences(JointIndices4 joints, Vector4 weights)
    {
        ValidateWeights(weights, nameof(weights));

        Joints = joints;
        Weights = weights;
    }

    public JointIndices4 Joints { get; }

    public Vector4 Weights { get; }

    public void ValidateForSkeleton(SkeletonDefinition skeleton)
    {
        ArgumentNullException.ThrowIfNull(skeleton);
        ValidateWeights(Weights, nameof(Weights));
        for (int lane = 0; lane < 4; lane++)
        {
            int jointIndex = Joints[lane];
            if (jointIndex >= skeleton.JointCount)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(skeleton),
                    $"Influence lane {lane} references joint {jointIndex}, but the skeleton has {skeleton.JointCount} joints.");
            }
        }
    }

    private static void ValidateWeights(Vector4 weights, string parameterName)
    {
        if (!DataValidation.IsFinite(weights))
            throw new ArgumentException("Skin weights must contain only finite values.", parameterName);
        if (weights.X < 0.0f || weights.Y < 0.0f || weights.Z < 0.0f || weights.W < 0.0f)
            throw new ArgumentException("Skin weights must be non-negative.", parameterName);

        float sum = weights.X + weights.Y + weights.Z + weights.W;
        if (MathF.Abs(sum - 1.0f) > WeightSumTolerance)
        {
            throw new ArgumentException(
                $"Skin weights must sum to one within {WeightSumTolerance}.",
                parameterName);
        }
    }
}

public sealed class SkeletonPose
{
    public SkeletonPose(SkeletonDefinition skeleton, IEnumerable<JointTransform> localTransforms)
    {
        ArgumentNullException.ThrowIfNull(skeleton);
        ArgumentNullException.ThrowIfNull(localTransforms);

        JointTransform[] copy = localTransforms.ToArray();
        if (copy.Length != skeleton.JointCount)
        {
            throw new ArgumentException(
                $"Expected {skeleton.JointCount} local transforms, but received {copy.Length}.",
                nameof(localTransforms));
        }

        for (int index = 0; index < copy.Length; index++)
            copy[index].Validate(nameof(localTransforms));

        Skeleton = skeleton;
        LocalTransforms = Array.AsReadOnly(copy);
    }

    public SkeletonDefinition Skeleton { get; }

    public IReadOnlyList<JointTransform> LocalTransforms { get; }
}

public sealed class SkinningPalette
{
    public SkinningPalette(SkinDefinition skin, IEnumerable<Matrix4x4> jointMatrices)
    {
        ArgumentNullException.ThrowIfNull(skin);
        ArgumentNullException.ThrowIfNull(jointMatrices);

        Matrix4x4[] copy = jointMatrices.ToArray();
        if (copy.Length != skin.Skeleton.JointCount)
        {
            throw new ArgumentException(
                $"Expected {skin.Skeleton.JointCount} palette matrices, but received {copy.Length}.",
                nameof(jointMatrices));
        }

        for (int index = 0; index < copy.Length; index++)
            DataValidation.RequireFinite(copy[index], nameof(jointMatrices), $"Palette matrix {index}");

        Skin = skin;
        JointMatrices = Array.AsReadOnly(copy);
    }

    public SkinDefinition Skin { get; }

    public IReadOnlyList<Matrix4x4> JointMatrices { get; }
}
