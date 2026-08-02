using System.Numerics;
using System.Text;

namespace ChronoFall.CharacterPresentation.Cooking;

public static class SkeletalAssetCookedFormat
{
    public const uint CurrentVersion = 1;
    public const string FileExtension = ".cfskel";

    private const int MaxStringBytes = 16 * 1024;
    private const int MaxLicenseEvidencePaths = 64;
    private const int MaxJoints = 1024;
    private const int MaxVertices = 2_000_000;
    private const int MaxIndices = 6_000_000;
    private const int MaxSections = 4096;
    private const int MaxAnimations = 1024;
    private const int MaxKeyframesPerChannel = 1_000_000;

    private static readonly byte[] Magic = "CFSKEL\0\0"u8.ToArray();
    private static readonly UTF8Encoding StrictUtf8 = new(false, true);

    public static void Write(Stream destination, CookedSkeletalCharacterAsset source)
    {
        ArgumentNullException.ThrowIfNull(destination);
        ArgumentNullException.ThrowIfNull(source);
        if (!destination.CanWrite)
            throw new ArgumentException("The destination stream must be writable.", nameof(destination));

        using var writer = new BinaryWriter(destination, StrictUtf8, leaveOpen: true);
        writer.Write(Magic);
        writer.Write(CurrentVersion);
        WriteDescriptor(writer, source.Descriptor);
        WriteAsset(writer, source.Asset);
        writer.Flush();
    }

    public static CookedSkeletalCharacterAsset Read(Stream source)
    {
        ArgumentNullException.ThrowIfNull(source);
        if (!source.CanRead)
            throw new ArgumentException("The source stream must be readable.", nameof(source));

        try
        {
            using var reader = new BinaryReader(source, StrictUtf8, leaveOpen: true);
            byte[] magic = ReadExact(reader, Magic.Length, "file magic");
            if (!magic.AsSpan().SequenceEqual(Magic))
                throw new InvalidDataException("Cooked skeletal asset magic is invalid.");

            uint version = reader.ReadUInt32();
            if (version != CurrentVersion)
                throw new InvalidDataException($"Cooked skeletal asset version {version} is unsupported; expected {CurrentVersion}.");

            SkeletalAssetCookDescriptor descriptor = ReadDescriptor(reader);
            SkeletalCharacterAsset asset = ReadAsset(reader);
            if (source.ReadByte() != -1)
                throw new InvalidDataException("Cooked skeletal asset contains trailing data.");
            return new CookedSkeletalCharacterAsset(descriptor, asset);
        }
        catch (InvalidDataException)
        {
            throw;
        }
        catch (Exception exception) when (exception is EndOfStreamException or DecoderFallbackException or ArgumentException or OverflowException)
        {
            throw new InvalidDataException("Cooked skeletal asset is malformed.", exception);
        }
    }

    private static void WriteDescriptor(BinaryWriter writer, SkeletalAssetCookDescriptor descriptor)
    {
        WriteString(writer, descriptor.AssetId);
        WriteString(writer, descriptor.SourcePath);
        writer.Write(Convert.FromHexString(descriptor.SourceSha256));
        WriteString(writer, descriptor.LicenseIdentifier);
        WriteCount(writer, descriptor.LicenseEvidencePaths.Count, MaxLicenseEvidencePaths, "license-evidence path");
        foreach (string path in descriptor.LicenseEvidencePaths)
            WriteString(writer, path);
        WriteString(writer, descriptor.SourceMeshNodeName);
        WriteString(writer, descriptor.SourceMeshName);
        WriteString(writer, descriptor.SourceSkinName);
    }

    private static SkeletalAssetCookDescriptor ReadDescriptor(BinaryReader reader)
    {
        string assetId = ReadString(reader);
        string sourcePath = ReadString(reader);
        string sourceSha256 = Convert.ToHexString(ReadExact(reader, 32, "source SHA-256")).ToLowerInvariant();
        string license = ReadString(reader);
        int evidenceCount = ReadCount(reader, MaxLicenseEvidencePaths, "license-evidence path");
        var evidence = new string[evidenceCount];
        for (int index = 0; index < evidence.Length; index++)
            evidence[index] = ReadString(reader);
        return new SkeletalAssetCookDescriptor(
            assetId,
            sourcePath,
            sourceSha256,
            license,
            evidence,
            ReadString(reader),
            ReadString(reader),
            ReadString(reader));
    }

    private static void WriteAsset(BinaryWriter writer, SkeletalCharacterAsset asset)
    {
        SkeletonDefinition skeleton = asset.Mesh.Skin.Skeleton;
        WriteCount(writer, skeleton.JointCount, MaxJoints, "joint");
        foreach (SkeletonJoint joint in skeleton.Joints)
        {
            WriteString(writer, joint.Name);
            writer.Write(joint.ParentIndex);
            WriteTransform(writer, joint.LocalBindTransform);
        }

        WriteCount(writer, asset.Mesh.Skin.InverseBindMatrices.Count, MaxJoints, "inverse-bind matrix");
        foreach (Matrix4x4 matrix in asset.Mesh.Skin.InverseBindMatrices)
            WriteMatrix(writer, matrix);

        WriteString(writer, asset.Mesh.Name);
        WriteCount(writer, asset.Mesh.Vertices.Count, MaxVertices, "vertex");
        foreach (SkinnedVertex vertex in asset.Mesh.Vertices)
        {
            WriteVector3(writer, vertex.Position);
            WriteVector3(writer, vertex.Normal);
            WriteVector2(writer, vertex.TextureCoordinate);
            writer.Write(checked((uint)vertex.Influences.Joints.X));
            writer.Write(checked((uint)vertex.Influences.Joints.Y));
            writer.Write(checked((uint)vertex.Influences.Joints.Z));
            writer.Write(checked((uint)vertex.Influences.Joints.W));
            WriteVector4(writer, vertex.Influences.Weights);
        }

        WriteCount(writer, asset.Mesh.Indices.Count, MaxIndices, "index");
        foreach (uint index in asset.Mesh.Indices)
            writer.Write(index);

        WriteCount(writer, asset.Mesh.Sections.Count, MaxSections, "mesh section");
        foreach (SkinnedMeshSection section in asset.Mesh.Sections)
        {
            WriteString(writer, section.MaterialName);
            writer.Write(section.StartIndex);
            writer.Write(section.IndexCount);
        }

        WriteCount(writer, asset.Animations.Count, MaxAnimations, "animation");
        foreach (AnimationClip animation in asset.Animations)
        {
            WriteString(writer, animation.Name);
            WriteCount(writer, animation.Tracks.Count, MaxJoints, "animation track");
            foreach (JointAnimationTrack track in animation.Tracks)
            {
                writer.Write(track.JointIndex);
                WriteVector3Channel(writer, track.Translations);
                WriteQuaternionChannel(writer, track.Rotations);
                WriteVector3Channel(writer, track.Scales);
            }
        }
    }

    private static SkeletalCharacterAsset ReadAsset(BinaryReader reader)
    {
        int jointCount = ReadCount(reader, MaxJoints, "joint", allowZero: false);
        var joints = new SkeletonJoint[jointCount];
        for (int index = 0; index < joints.Length; index++)
            joints[index] = new SkeletonJoint(ReadString(reader), reader.ReadInt32(), ReadTransform(reader));
        var skeleton = new SkeletonDefinition(joints);

        int inverseBindCount = ReadCount(reader, MaxJoints, "inverse-bind matrix", allowZero: false);
        var inverseBinds = new Matrix4x4[inverseBindCount];
        for (int index = 0; index < inverseBinds.Length; index++)
            inverseBinds[index] = ReadMatrix(reader);
        var skin = new SkinDefinition(skeleton, inverseBinds);

        string meshName = ReadString(reader);
        int vertexCount = ReadCount(reader, MaxVertices, "vertex", allowZero: false);
        var vertices = new SkinnedVertex[vertexCount];
        for (int index = 0; index < vertices.Length; index++)
        {
            Vector3 position = ReadVector3(reader);
            Vector3 normal = ReadVector3(reader);
            Vector2 uv = ReadVector2(reader);
            var jointIndices = new JointIndices4(
                checked((int)reader.ReadUInt32()),
                checked((int)reader.ReadUInt32()),
                checked((int)reader.ReadUInt32()),
                checked((int)reader.ReadUInt32()));
            vertices[index] = new SkinnedVertex(position, normal, uv, new SkinInfluences(jointIndices, ReadVector4(reader)));
        }

        int indexCount = ReadCount(reader, MaxIndices, "index", allowZero: false);
        var indices = new uint[indexCount];
        for (int index = 0; index < indices.Length; index++)
            indices[index] = reader.ReadUInt32();

        int sectionCount = ReadCount(reader, MaxSections, "mesh section", allowZero: false);
        var sections = new SkinnedMeshSection[sectionCount];
        for (int index = 0; index < sections.Length; index++)
            sections[index] = new SkinnedMeshSection(ReadString(reader), reader.ReadInt32(), reader.ReadInt32());
        var mesh = new SkinnedMeshDefinition(meshName, skin, vertices, indices, sections);

        int animationCount = ReadCount(reader, MaxAnimations, "animation", allowZero: false);
        var animations = new AnimationClip[animationCount];
        for (int animationIndex = 0; animationIndex < animations.Length; animationIndex++)
        {
            string name = ReadString(reader);
            int trackCount = ReadCount(reader, MaxJoints, "animation track", allowZero: false);
            var tracks = new JointAnimationTrack[trackCount];
            for (int trackIndex = 0; trackIndex < tracks.Length; trackIndex++)
            {
                tracks[trackIndex] = new JointAnimationTrack(
                    reader.ReadInt32(),
                    ReadVector3Channel(reader),
                    ReadQuaternionChannel(reader),
                    ReadVector3Channel(reader));
            }
            animations[animationIndex] = new AnimationClip(name, skeleton, tracks);
        }
        return new SkeletalCharacterAsset(mesh, animations);
    }

    private static void WriteVector3Channel(BinaryWriter writer, Vector3AnimationChannel channel)
    {
        writer.Write((byte)channel.Interpolation);
        WriteCount(writer, channel.Keyframes.Count, MaxKeyframesPerChannel, "vector keyframe");
        foreach (Vector3Keyframe keyframe in channel.Keyframes)
        {
            writer.Write(keyframe.Time);
            WriteVector3(writer, keyframe.Value);
        }
    }

    private static Vector3AnimationChannel ReadVector3Channel(BinaryReader reader)
    {
        AnimationInterpolation interpolation = ReadInterpolation(reader);
        int count = ReadCount(reader, MaxKeyframesPerChannel, "vector keyframe", allowZero: false);
        var keyframes = new Vector3Keyframe[count];
        for (int index = 0; index < keyframes.Length; index++)
            keyframes[index] = new Vector3Keyframe(reader.ReadSingle(), ReadVector3(reader));
        return new Vector3AnimationChannel(keyframes, interpolation);
    }

    private static void WriteQuaternionChannel(BinaryWriter writer, QuaternionAnimationChannel channel)
    {
        writer.Write((byte)channel.Interpolation);
        WriteCount(writer, channel.Keyframes.Count, MaxKeyframesPerChannel, "quaternion keyframe");
        foreach (QuaternionKeyframe keyframe in channel.Keyframes)
        {
            writer.Write(keyframe.Time);
            WriteQuaternion(writer, keyframe.Value);
        }
    }

    private static QuaternionAnimationChannel ReadQuaternionChannel(BinaryReader reader)
    {
        AnimationInterpolation interpolation = ReadInterpolation(reader);
        int count = ReadCount(reader, MaxKeyframesPerChannel, "quaternion keyframe", allowZero: false);
        var keyframes = new QuaternionKeyframe[count];
        for (int index = 0; index < keyframes.Length; index++)
            keyframes[index] = QuaternionKeyframe.FromValidatedComponents(reader.ReadSingle(), ReadQuaternion(reader));
        return new QuaternionAnimationChannel(keyframes, interpolation);
    }

    private static AnimationInterpolation ReadInterpolation(BinaryReader reader)
    {
        byte value = reader.ReadByte();
        if (value != (byte)AnimationInterpolation.Linear)
            throw new InvalidDataException($"Animation interpolation value {value} is unsupported.");
        return AnimationInterpolation.Linear;
    }

    private static void WriteTransform(BinaryWriter writer, JointTransform value)
    {
        WriteVector3(writer, value.Translation);
        WriteQuaternion(writer, value.Rotation);
        WriteVector3(writer, value.Scale);
    }

    private static JointTransform ReadTransform(BinaryReader reader) =>
        JointTransform.FromValidatedComponents(ReadVector3(reader), ReadQuaternion(reader), ReadVector3(reader));

    private static void WriteMatrix(BinaryWriter writer, Matrix4x4 value)
    {
        writer.Write(value.M11); writer.Write(value.M12); writer.Write(value.M13); writer.Write(value.M14);
        writer.Write(value.M21); writer.Write(value.M22); writer.Write(value.M23); writer.Write(value.M24);
        writer.Write(value.M31); writer.Write(value.M32); writer.Write(value.M33); writer.Write(value.M34);
        writer.Write(value.M41); writer.Write(value.M42); writer.Write(value.M43); writer.Write(value.M44);
    }

    private static Matrix4x4 ReadMatrix(BinaryReader reader) => new(
        reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(),
        reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(),
        reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(),
        reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());

    private static void WriteVector2(BinaryWriter writer, Vector2 value)
    {
        writer.Write(value.X); writer.Write(value.Y);
    }

    private static Vector2 ReadVector2(BinaryReader reader) => new(reader.ReadSingle(), reader.ReadSingle());

    private static void WriteVector3(BinaryWriter writer, Vector3 value)
    {
        writer.Write(value.X); writer.Write(value.Y); writer.Write(value.Z);
    }

    private static Vector3 ReadVector3(BinaryReader reader) => new(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());

    private static void WriteVector4(BinaryWriter writer, Vector4 value)
    {
        writer.Write(value.X); writer.Write(value.Y); writer.Write(value.Z); writer.Write(value.W);
    }

    private static Vector4 ReadVector4(BinaryReader reader) =>
        new(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());

    private static void WriteQuaternion(BinaryWriter writer, Quaternion value)
    {
        writer.Write(value.X); writer.Write(value.Y); writer.Write(value.Z); writer.Write(value.W);
    }

    private static Quaternion ReadQuaternion(BinaryReader reader) =>
        new(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());

    private static void WriteString(BinaryWriter writer, string value)
    {
        byte[] bytes = StrictUtf8.GetBytes(value);
        if (bytes.Length == 0 || bytes.Length > MaxStringBytes)
            throw new ArgumentException($"Cooked strings must contain between 1 and {MaxStringBytes} UTF-8 bytes.", nameof(value));
        writer.Write((uint)bytes.Length);
        writer.Write(bytes);
    }

    private static string ReadString(BinaryReader reader)
    {
        int length = ReadCount(reader, MaxStringBytes, "UTF-8 string byte", allowZero: false);
        return StrictUtf8.GetString(ReadExact(reader, length, "UTF-8 string"));
    }

    private static byte[] ReadExact(BinaryReader reader, int count, string field)
    {
        byte[] bytes = reader.ReadBytes(count);
        if (bytes.Length != count)
            throw new EndOfStreamException($"Cooked skeletal asset ended while reading {field}.");
        return bytes;
    }

    private static void WriteCount(BinaryWriter writer, int count, int maximum, string field)
    {
        if (count < 0 || count > maximum)
            throw new ArgumentOutOfRangeException(nameof(count), $"The {field} count must be between 0 and {maximum}.");
        writer.Write((uint)count);
    }

    private static int ReadCount(BinaryReader reader, int maximum, string field, bool allowZero = true)
    {
        uint value = reader.ReadUInt32();
        if ((!allowZero && value == 0) || value > maximum)
            throw new InvalidDataException($"Cooked skeletal asset {field} count {value} is outside the supported range.");
        return checked((int)value);
    }
}
