using System.Numerics;
using Imported = global::SimpleMesh;

namespace ChronoFall.CharacterExperiment.SimpleMesh.Tests;

public sealed class LoaderFailureTests
{
    [Fact]
    public void UnsupportedInterpolationReportsClipTargetAndPath()
    {
        Imported.Model model = CreateModel();
        model.Animations[0].Scales[0].Interpolation = Imported.AnimationInterpolation.Step;

        SkeletalAssetLoadException exception = Assert.Throws<SkeletalAssetLoadException>(() =>
            SimpleMeshSkeletalAssetLoader.MapModel(model, "fixture.glb"));

        Assert.Equal("fixture.glb", exception.SourcePath);
        Assert.Equal("Clip", exception.ClipName);
        Assert.Equal("root", exception.TargetNode);
        Assert.Equal("scale", exception.ChannelPath);
        Assert.Contains("only LINEAR", exception.Reason, StringComparison.Ordinal);
    }

    [Fact]
    public void UnresolvedTargetReportsChannelContext()
    {
        Imported.Model model = CreateModel();
        model.Animations[0].Translations[0].Target = "missing";

        SkeletalAssetLoadException exception = Assert.Throws<SkeletalAssetLoadException>(() =>
            SimpleMeshSkeletalAssetLoader.MapModel(model, "fixture.glb"));

        Assert.Equal("Clip", exception.ClipName);
        Assert.Equal("missing", exception.TargetNode);
        Assert.Equal("translation", exception.ChannelPath);
    }

    [Fact]
    public void EmptyAndMissingChannelsFailWithTheirPaths()
    {
        Imported.Model empty = CreateModel();
        empty.Animations[0].Rotations[0].Keyframes = [];
        SkeletalAssetLoadException emptyException = Assert.Throws<SkeletalAssetLoadException>(() =>
            SimpleMeshSkeletalAssetLoader.MapModel(empty, "fixture.glb"));
        Assert.Equal("rotation", emptyException.ChannelPath);
        Assert.Contains("at least one", emptyException.Reason, StringComparison.Ordinal);

        Imported.Model missing = CreateModel();
        missing.Animations[0].Scales = [];
        SkeletalAssetLoadException missingException = Assert.Throws<SkeletalAssetLoadException>(() =>
            SimpleMeshSkeletalAssetLoader.MapModel(missing, "fixture.glb"));
        Assert.Equal("scale", missingException.ChannelPath);
        Assert.Contains("missing", missingException.Reason, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void NonFiniteAndNonIncreasingKeysFailContextually()
    {
        Imported.Model nonFinite = CreateModel();
        nonFinite.Animations[0].Translations[0].Keyframes[1] =
            new Imported.TranslationKeyframe(1.0f, new Vector3(float.NaN, 0.0f, 0.0f));
        SkeletalAssetLoadException nonFiniteException = Assert.Throws<SkeletalAssetLoadException>(() =>
            SimpleMeshSkeletalAssetLoader.MapModel(nonFinite, "fixture.glb"));
        Assert.Equal("translation", nonFiniteException.ChannelPath);
        Assert.Contains("finite", nonFiniteException.Reason, StringComparison.OrdinalIgnoreCase);

        Imported.Model nonIncreasing = CreateModel();
        nonIncreasing.Animations[0].Scales[0].Keyframes[1] =
            new Imported.ScaleKeyframe(0.0f, Vector3.One);
        SkeletalAssetLoadException nonIncreasingException = Assert.Throws<SkeletalAssetLoadException>(() =>
            SimpleMeshSkeletalAssetLoader.MapModel(nonIncreasing, "fixture.glb"));
        Assert.Equal("scale", nonIncreasingException.ChannelPath);
        Assert.Contains("strictly increasing", nonIncreasingException.Reason, StringComparison.Ordinal);
    }

    private static Imported.Model CreateModel()
    {
        var material = new Imported.Material { Name = "material" };
        var vertices = new Imported.VertexArray(
            Imported.VertexAttributes.Normal |
            Imported.VertexAttributes.Texture1 |
            Imported.VertexAttributes.Joints,
            3);
        Vector3[] positions = [Vector3.Zero, Vector3.UnitX, Vector3.UnitZ];
        for (int index = 0; index < 3; index++)
        {
            vertices.Position[index] = positions[index];
            vertices.Normal[index] = Vector3.UnitY;
            vertices.Texture1[index] = Vector2.Zero;
            vertices.JointIndices[index] = new Imported.Point4<ushort>(0, 0, 0, 0);
            vertices.JointWeights[index] = Vector4.UnitX;
        }

        var geometry = new Imported.Geometry(vertices, new Imported.Indices((ushort[])[0, 1, 2]))
        {
            Name = "mesh",
            Kind = Imported.GeometryKind.Triangles,
            Groups = [
                new Imported.TriangleGroup(material)
                {
                    StartIndex = 0,
                    BaseVertex = 0,
                    IndexCount = 3,
                },
            ],
        };
        var root = new Imported.ModelNode { Name = "root", Transform = Matrix4x4.Identity };
        var skin = new Imported.Skin
        {
            Name = "skin",
            Bones = [root],
            InverseBindMatrices = [Matrix4x4.Identity],
        };
        var mesh = new Imported.ModelNode
        {
            Name = "mesh",
            Transform = Matrix4x4.Identity,
            Geometry = geometry,
            Skin = skin,
        };
        var armature = new Imported.ModelNode { Name = "Armature", Transform = Matrix4x4.Identity };
        armature.Children.Add(mesh);
        armature.Children.Add(root);

        return new Imported.Model
        {
            Roots = [armature],
            Geometries = [geometry],
            Skins = [skin],
            Animations = [CreateAnimation()],
        };
    }

    private static Imported.Animation CreateAnimation() =>
        new()
        {
            Name = "Clip",
            Translations = [
                new Imported.TranslationChannel
                {
                    Target = "root",
                    Interpolation = Imported.AnimationInterpolation.Linear,
                    Keyframes = [
                        new Imported.TranslationKeyframe(0.0f, Vector3.Zero),
                        new Imported.TranslationKeyframe(1.0f, Vector3.Zero),
                    ],
                },
            ],
            Rotations = [
                new Imported.RotationChannel
                {
                    Target = "root",
                    Interpolation = Imported.AnimationInterpolation.Linear,
                    Keyframes = [
                        new Imported.RotationKeyframe(0.0f, Quaternion.Identity),
                        new Imported.RotationKeyframe(1.0f, Quaternion.Identity),
                    ],
                },
            ],
            Scales = [
                new Imported.ScaleChannel
                {
                    Target = "root",
                    Interpolation = Imported.AnimationInterpolation.Linear,
                    Keyframes = [
                        new Imported.ScaleKeyframe(0.0f, Vector3.One),
                        new Imported.ScaleKeyframe(1.0f, Vector3.One),
                    ],
                },
            ],
        };
}
