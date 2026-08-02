using System.Buffers.Binary;
using System.Numerics;

namespace ChronoFall.CharacterPresentation.Cooking.Tests;

public sealed class SkeletalAssetCookedFormatTests
{
    [Fact]
    public void RoundTripPreservesDescriptorAndCompleteAsset()
    {
        CookedSkeletalCharacterAsset expected = CreateAsset();
        using var stream = new MemoryStream();

        SkeletalAssetCookedFormat.Write(stream, expected);
        stream.Position = 0;
        CookedSkeletalCharacterAsset actual = SkeletalAssetCookedFormat.Read(stream);

        AssertDescriptor(expected.Descriptor, actual.Descriptor);
        AssertAsset(expected.Asset, actual.Asset);
    }

    [Fact]
    public void RepeatedWritesAreByteIdentical()
    {
        CookedSkeletalCharacterAsset asset = CreateAsset();
        using var first = new MemoryStream();
        using var second = new MemoryStream();

        SkeletalAssetCookedFormat.Write(first, asset);
        SkeletalAssetCookedFormat.Write(second, asset);

        Assert.Equal(first.ToArray(), second.ToArray());
    }

    [Fact]
    public void ReaderRejectsBadMagicVersionTruncationAndTrailingData()
    {
        byte[] valid = Write(CreateAsset());

        byte[] badMagic = valid.ToArray();
        badMagic[0] ^= 0xff;
        AssertInvalid(badMagic);

        byte[] badVersion = valid.ToArray();
        BinaryPrimitives.WriteUInt32LittleEndian(badVersion.AsSpan(8, 4), SkeletalAssetCookedFormat.CurrentVersion + 1);
        AssertInvalid(badVersion);

        AssertInvalid(valid[..^1]);
        AssertInvalid([.. valid, 0xff]);
    }

    [Fact]
    public void ReaderRejectsUnboundedStringLengthBeforeAllocation()
    {
        byte[] bytes = Write(CreateAsset());
        BinaryPrimitives.WriteUInt32LittleEndian(bytes.AsSpan(12, 4), uint.MaxValue);

        AssertInvalid(bytes);
    }

    [Theory]
    [InlineData("../escape.glb")]
    [InlineData("/absolute.glb")]
    [InlineData("assets\\windows.glb")]
    [InlineData("assets//empty.glb")]
    public void DescriptorRejectsNonPortableSourcePaths(string path)
    {
        Assert.Throws<ArgumentException>(() => new SkeletalAssetCookDescriptor(
            "test-character",
            path,
            new string('0', 64),
            "CC0-1.0",
            ["licenses/CC0.txt"],
            "MeshNode",
            "Mesh",
            "Skin"));
    }

    [Fact]
    public void CookingAssemblyHasNoImporterRendererOrChildDependency()
    {
        string[] references = typeof(SkeletalAssetCookedFormat).Assembly
            .GetReferencedAssemblies()
            .Select(static assembly => assembly.Name!)
            .ToArray();

        Assert.DoesNotContain(references, name =>
            name.Contains("SimpleMesh", StringComparison.OrdinalIgnoreCase) ||
            name.Contains("SDL", StringComparison.OrdinalIgnoreCase) ||
            name.Contains("Royale", StringComparison.OrdinalIgnoreCase) ||
            name.Contains("Starfall", StringComparison.OrdinalIgnoreCase));
    }

    internal static void AssertAsset(SkeletalCharacterAsset expected, SkeletalCharacterAsset actual)
    {
        Assert.Equal(expected.Mesh.Name, actual.Mesh.Name);
        Assert.Equal(expected.Mesh.Skin.Skeleton.JointCount, actual.Mesh.Skin.Skeleton.JointCount);
        for (int index = 0; index < expected.Mesh.Skin.Skeleton.JointCount; index++)
        {
            SkeletonJoint expectedJoint = expected.Mesh.Skin.Skeleton.Joints[index];
            SkeletonJoint actualJoint = actual.Mesh.Skin.Skeleton.Joints[index];
            Assert.Equal(expectedJoint.Name, actualJoint.Name);
            Assert.Equal(expectedJoint.ParentIndex, actualJoint.ParentIndex);
            Assert.Equal(expectedJoint.LocalBindTransform, actualJoint.LocalBindTransform);
            Assert.Equal(expected.Mesh.Skin.InverseBindMatrices[index], actual.Mesh.Skin.InverseBindMatrices[index]);
        }

        Assert.Equal(expected.Mesh.Vertices, actual.Mesh.Vertices);
        Assert.Equal(expected.Mesh.Indices, actual.Mesh.Indices);
        Assert.Equal(expected.Mesh.Sections.Count, actual.Mesh.Sections.Count);
        for (int index = 0; index < expected.Mesh.Sections.Count; index++)
        {
            SkinnedMeshSection expectedSection = expected.Mesh.Sections[index];
            SkinnedMeshSection actualSection = actual.Mesh.Sections[index];
            Assert.Equal(expectedSection.MaterialName, actualSection.MaterialName);
            Assert.Equal(expectedSection.StartIndex, actualSection.StartIndex);
            Assert.Equal(expectedSection.IndexCount, actualSection.IndexCount);
        }

        Assert.Equal(expected.Animations.Count, actual.Animations.Count);
        for (int clipIndex = 0; clipIndex < expected.Animations.Count; clipIndex++)
        {
            AnimationClip expectedClip = expected.Animations[clipIndex];
            AnimationClip actualClip = actual.Animations[clipIndex];
            Assert.Equal(expectedClip.Name, actualClip.Name);
            Assert.Equal(expectedClip.Duration, actualClip.Duration);
            Assert.Equal(expectedClip.Tracks.Count, actualClip.Tracks.Count);
            for (int trackIndex = 0; trackIndex < expectedClip.Tracks.Count; trackIndex++)
            {
                JointAnimationTrack expectedTrack = expectedClip.Tracks[trackIndex];
                JointAnimationTrack actualTrack = actualClip.Tracks[trackIndex];
                Assert.Equal(expectedTrack.JointIndex, actualTrack.JointIndex);
                Assert.Equal(expectedTrack.Translations.Interpolation, actualTrack.Translations.Interpolation);
                Assert.Equal(expectedTrack.Translations.Keyframes, actualTrack.Translations.Keyframes);
                Assert.Equal(expectedTrack.Rotations.Interpolation, actualTrack.Rotations.Interpolation);
                Assert.Equal(expectedTrack.Rotations.Keyframes, actualTrack.Rotations.Keyframes);
                Assert.Equal(expectedTrack.Scales.Interpolation, actualTrack.Scales.Interpolation);
                Assert.Equal(expectedTrack.Scales.Keyframes, actualTrack.Scales.Keyframes);
            }
        }
    }

    private static void AssertDescriptor(SkeletalAssetCookDescriptor expected, SkeletalAssetCookDescriptor actual)
    {
        Assert.Equal(expected.AssetId, actual.AssetId);
        Assert.Equal(expected.SourcePath, actual.SourcePath);
        Assert.Equal(expected.SourceSha256, actual.SourceSha256);
        Assert.Equal(expected.LicenseIdentifier, actual.LicenseIdentifier);
        Assert.Equal(expected.LicenseEvidencePaths, actual.LicenseEvidencePaths);
        Assert.Equal(expected.SourceMeshNodeName, actual.SourceMeshNodeName);
        Assert.Equal(expected.SourceMeshName, actual.SourceMeshName);
        Assert.Equal(expected.SourceSkinName, actual.SourceSkinName);
    }

    private static byte[] Write(CookedSkeletalCharacterAsset asset)
    {
        using var stream = new MemoryStream();
        SkeletalAssetCookedFormat.Write(stream, asset);
        return stream.ToArray();
    }

    private static void AssertInvalid(byte[] bytes)
    {
        using var stream = new MemoryStream(bytes);
        Assert.Throws<InvalidDataException>(() => SkeletalAssetCookedFormat.Read(stream));
    }

    private static CookedSkeletalCharacterAsset CreateAsset()
    {
        var skeleton = new SkeletonDefinition([
            new SkeletonJoint("root", -1, JointTransform.Identity),
            new SkeletonJoint("child", 0, new JointTransform(new Vector3(0, 1, 0), Quaternion.Identity, Vector3.One)),
        ]);
        var skin = new SkinDefinition(skeleton, [Matrix4x4.Identity, Matrix4x4.CreateTranslation(0, -1, 0)]);
        SkinInfluences rootInfluence = new(new JointIndices4(0, 0, 0, 0), new Vector4(1, 0, 0, 0));
        var mesh = new SkinnedMeshDefinition(
            "triangle",
            skin,
            [
                new SkinnedVertex(Vector3.Zero, Vector3.UnitZ, Vector2.Zero, rootInfluence),
                new SkinnedVertex(Vector3.UnitX, Vector3.UnitZ, Vector2.UnitX, rootInfluence),
                new SkinnedVertex(Vector3.UnitY, Vector3.UnitZ, Vector2.UnitY, rootInfluence),
            ],
            [0u, 1u, 2u],
            [new SkinnedMeshSection("material", 0, 3)]);
        JointAnimationTrack[] tracks = Enumerable.Range(0, skeleton.JointCount)
            .Select(index => new JointAnimationTrack(
                index,
                new Vector3AnimationChannel([new Vector3Keyframe(0, Vector3.Zero), new Vector3Keyframe(1, new Vector3(index, 0, 0))]),
                new QuaternionAnimationChannel([new QuaternionKeyframe(0, Quaternion.Identity), new QuaternionKeyframe(1, Quaternion.CreateFromAxisAngle(Vector3.UnitY, 0.25f))]),
                new Vector3AnimationChannel([new Vector3Keyframe(0, Vector3.One), new Vector3Keyframe(1, Vector3.One)])))
            .ToArray();
        var asset = new SkeletalCharacterAsset(mesh, [new AnimationClip("idle", skeleton, tracks)]);
        var descriptor = new SkeletalAssetCookDescriptor(
            "test-character",
            "assets/source.glb",
            new string('0', 64),
            "CC0-1.0",
            ["assets/LICENSE.txt"],
            "MeshNode",
            "triangle",
            "Skin");
        return new CookedSkeletalCharacterAsset(descriptor, asset);
    }
}
