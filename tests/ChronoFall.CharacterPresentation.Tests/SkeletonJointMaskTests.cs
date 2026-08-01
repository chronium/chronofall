namespace ChronoFall.CharacterPresentation.Tests;

public sealed class SkeletonJointMaskTests
{
    [Fact]
    public void ConstructorCopiesBinaryMembership()
    {
        SkeletonDefinition skeleton = CreateSkeleton();
        bool[] included = [false, false, true, true, false];

        var mask = new SkeletonJointMask(skeleton, included);
        included[2] = false;

        Assert.Same(skeleton, mask.Skeleton);
        Assert.Equal(2, mask.IncludedJointCount);
        Assert.Equal([false, false, true, true, false], mask.IncludedJoints);
        Assert.True(mask[2]);
    }

    [Fact]
    public void CreateSubtreeIncludesOnlyRootAndDescendants()
    {
        SkeletonDefinition skeleton = CreateSkeleton();

        SkeletonJointMask mask = SkeletonJointMask.CreateSubtree(skeleton, 2);

        Assert.Equal([false, false, true, true, false], mask.IncludedJoints);
        Assert.Equal(2, mask.IncludedJointCount);
    }

    [Fact]
    public void CreateRootSubtreeIncludesEveryJoint()
    {
        SkeletonDefinition skeleton = CreateSkeleton();

        SkeletonJointMask mask = SkeletonJointMask.CreateSubtree(skeleton, 0);

        Assert.All(mask.IncludedJoints, Assert.True);
        Assert.Equal(skeleton.JointCount, mask.IncludedJointCount);
    }

    [Fact]
    public void ConstructorRejectsWrongMembershipCount()
    {
        SkeletonDefinition skeleton = CreateSkeleton();

        ArgumentException exception = Assert.Throws<ArgumentException>(() =>
            new SkeletonJointMask(skeleton, [true]));

        Assert.Contains("Expected 5", exception.Message, StringComparison.Ordinal);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(5)]
    public void CreateSubtreeRejectsInvalidRoot(int rootJointIndex)
    {
        SkeletonDefinition skeleton = CreateSkeleton();

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            SkeletonJointMask.CreateSubtree(skeleton, rootJointIndex));
    }

    private static SkeletonDefinition CreateSkeleton() => new([
        new SkeletonJoint("root", -1, JointTransform.Identity),
        new SkeletonJoint("pelvis", 0, JointTransform.Identity),
        new SkeletonJoint("spine", 1, JointTransform.Identity),
        new SkeletonJoint("hand", 2, JointTransform.Identity),
        new SkeletonJoint("leg", 1, JointTransform.Identity),
    ]);
}
