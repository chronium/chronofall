using System.Numerics;

namespace ChronoFall.CharacterPresentation.Tests;

public sealed class SkinnedMeshDataTests
{
    [Fact]
    public void MeshRequiresValidatedVerticesIndicesAndCompleteSections()
    {
        SkinDefinition skin = CreateSkin();
        SkinnedVertex[] vertices = CreateVertices();
        uint[] indices = [0, 1, 2];
        SkinnedMeshSection[] sections = [new("material", 0, 3)];

        var mesh = new SkinnedMeshDefinition("mesh", skin, vertices, indices, sections);
        vertices[0] = vertices[1];
        indices[0] = 99;
        sections[0] = new SkinnedMeshSection("changed", 0, 3);

        Assert.Equal(Vector3.Zero, mesh.Vertices[0].Position);
        Assert.Equal((uint)0, mesh.Indices[0]);
        Assert.Equal("material", mesh.Sections[0].MaterialName);
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new SkinnedMeshDefinition("mesh", skin, CreateVertices(), [0, 1, 3], [new("material", 0, 3)]));
        Assert.Throws<ArgumentException>(() =>
            new SkinnedMeshDefinition("mesh", skin, CreateVertices(), [0, 1, 2], [new("material", 0, 6)]));
    }

    [Fact]
    public void CharacterAssetRequiresUniqueClipsUsingTheMeshSkeleton()
    {
        SkinDefinition skin = CreateSkin();
        var mesh = new SkinnedMeshDefinition(
            "mesh",
            skin,
            CreateVertices(),
            [0, 1, 2],
            [new SkinnedMeshSection("material", 0, 3)]);
        AnimationClip clip = CreateClip("Idle", skin.Skeleton);

        var asset = new SkeletalCharacterAsset(mesh, [clip]);

        Assert.Same(skin.Skeleton, asset.Animations[0].Skeleton);
        Assert.Throws<ArgumentException>(() => new SkeletalCharacterAsset(mesh, [clip, clip]));
        Assert.Throws<ArgumentException>(() =>
            new SkeletalCharacterAsset(mesh, [CreateClip("Other", CreateSkin().Skeleton)]));
    }

    [Fact]
    public void DefaultVertexCannotBypassValidation()
    {
        SkinDefinition skin = CreateSkin();

        Assert.Throws<ArgumentException>(() =>
            new SkinnedMeshDefinition(
                "mesh",
                skin,
                [default, CreateVertices()[1], CreateVertices()[2]],
                [0, 1, 2],
                [new SkinnedMeshSection("material", 0, 3)]));
    }

    private static SkinDefinition CreateSkin()
    {
        var skeleton = new SkeletonDefinition([
            new SkeletonJoint("root", -1, JointTransform.Identity),
        ]);
        return new SkinDefinition(skeleton, [Matrix4x4.Identity]);
    }

    private static SkinnedVertex[] CreateVertices()
    {
        var influences = new SkinInfluences(new JointIndices4(0, 0, 0, 0), Vector4.UnitX);
        return [
            new SkinnedVertex(Vector3.Zero, Vector3.UnitY, Vector2.Zero, influences),
            new SkinnedVertex(Vector3.UnitX, Vector3.UnitY, Vector2.UnitX, influences),
            new SkinnedVertex(Vector3.UnitZ, Vector3.UnitY, Vector2.UnitY, influences),
        ];
    }

    private static AnimationClip CreateClip(string name, SkeletonDefinition skeleton) =>
        new(
            name,
            skeleton,
            [
                new JointAnimationTrack(
                    0,
                    new Vector3AnimationChannel([
                        new Vector3Keyframe(0.0f, Vector3.Zero),
                        new Vector3Keyframe(1.0f, Vector3.Zero),
                    ]),
                    new QuaternionAnimationChannel([
                        new QuaternionKeyframe(0.0f, Quaternion.Identity),
                        new QuaternionKeyframe(1.0f, Quaternion.Identity),
                    ]),
                    new Vector3AnimationChannel([
                        new Vector3Keyframe(0.0f, Vector3.One),
                        new Vector3Keyframe(1.0f, Vector3.One),
                    ])),
            ]);
}
