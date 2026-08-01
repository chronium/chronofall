namespace ChronoFall.CharacterPresentation;

public sealed class SkeletonJointMask
{
    private readonly bool[] includedJoints;
    private readonly IReadOnlyList<bool> readOnlyIncludedJoints;

    public SkeletonJointMask(
        SkeletonDefinition skeleton,
        IEnumerable<bool> includedJoints)
    {
        ArgumentNullException.ThrowIfNull(skeleton);
        ArgumentNullException.ThrowIfNull(includedJoints);

        this.includedJoints = includedJoints.ToArray();
        if (this.includedJoints.Length != skeleton.JointCount)
        {
            throw new ArgumentException(
                $"Expected {skeleton.JointCount} joint-mask entries, but received {this.includedJoints.Length}.",
                nameof(includedJoints));
        }

        Skeleton = skeleton;
        readOnlyIncludedJoints = Array.AsReadOnly(this.includedJoints);
        IncludedJointCount = this.includedJoints.Count(static included => included);
    }

    public SkeletonDefinition Skeleton { get; }

    public IReadOnlyList<bool> IncludedJoints => readOnlyIncludedJoints;

    public int IncludedJointCount { get; }

    public bool this[int jointIndex] => includedJoints[jointIndex];

    public static SkeletonJointMask CreateSubtree(
        SkeletonDefinition skeleton,
        int rootJointIndex)
    {
        ArgumentNullException.ThrowIfNull(skeleton);
        if (rootJointIndex < 0 || rootJointIndex >= skeleton.JointCount)
        {
            throw new ArgumentOutOfRangeException(
                nameof(rootJointIndex),
                $"Subtree root must be between zero and {skeleton.JointCount - 1}.");
        }

        var included = new bool[skeleton.JointCount];
        included[rootJointIndex] = true;
        for (int index = rootJointIndex + 1; index < skeleton.JointCount; index++)
            included[index] = included[skeleton.Joints[index].ParentIndex];

        return new SkeletonJointMask(skeleton, included);
    }
}
