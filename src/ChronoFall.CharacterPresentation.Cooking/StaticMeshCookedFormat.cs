using System.Numerics;
using System.Text;

namespace ChronoFall.CharacterPresentation.Cooking;

public static class StaticMeshCookedFormat
{
    public const uint CurrentVersion = 1;
    public const string FileExtension = ".cfmesh";
    internal const int MaxEvidenceFiles = 64;

    private const int MaxStringBytes = 16 * 1024;
    private const int MaxVertices = 2_000_000;
    private const int MaxIndices = 6_000_000;
    private const int MaxSections = 4096;
    private static readonly byte[] Magic = "CFMESH\0\0"u8.ToArray();
    private static readonly UTF8Encoding StrictUtf8 = new(false, true);

    public static void Write(Stream destination, CookedStaticMeshAsset source)
    {
        ArgumentNullException.ThrowIfNull(destination);
        ArgumentNullException.ThrowIfNull(source);
        if (!destination.CanWrite)
            throw new ArgumentException("The destination stream must be writable.", nameof(destination));

        using var writer = new BinaryWriter(destination, StrictUtf8, leaveOpen: true);
        writer.Write(Magic);
        writer.Write(CurrentVersion);
        WriteDescriptor(writer, source.Descriptor);
        WriteMesh(writer, source.Mesh);
        writer.Flush();
    }

    public static CookedStaticMeshAsset Read(Stream source)
    {
        ArgumentNullException.ThrowIfNull(source);
        if (!source.CanRead)
            throw new ArgumentException("The source stream must be readable.", nameof(source));

        try
        {
            using var reader = new BinaryReader(source, StrictUtf8, leaveOpen: true);
            if (!ReadExact(reader, Magic.Length, "file magic").AsSpan().SequenceEqual(Magic))
                throw new InvalidDataException("Cooked static mesh magic is invalid.");
            uint version = reader.ReadUInt32();
            if (version != CurrentVersion)
                throw new InvalidDataException($"Cooked static mesh version {version} is unsupported; expected {CurrentVersion}.");

            StaticAssetCookDescriptor descriptor = ReadDescriptor(reader);
            StaticMeshDefinition mesh = ReadMesh(reader);
            if (source.ReadByte() != -1)
                throw new InvalidDataException("Cooked static mesh contains trailing data.");
            return new CookedStaticMeshAsset(descriptor, mesh);
        }
        catch (InvalidDataException)
        {
            throw;
        }
        catch (Exception exception) when (exception is EndOfStreamException or DecoderFallbackException or ArgumentException or OverflowException)
        {
            throw new InvalidDataException("Cooked static mesh is malformed.", exception);
        }
    }

    private static void WriteDescriptor(BinaryWriter writer, StaticAssetCookDescriptor descriptor)
    {
        WriteString(writer, descriptor.AssetId);
        WriteFile(writer, descriptor.PrimarySource);
        WriteFiles(writer, descriptor.ExternalResources);
        WriteString(writer, descriptor.LicenseIdentifier);
        WriteFiles(writer, descriptor.LicenseEvidence);
        writer.Write(descriptor.MetersPerSourceUnit);
        WriteString(writer, descriptor.MaterialPolicy);
    }

    private static StaticAssetCookDescriptor ReadDescriptor(BinaryReader reader) => new(
        ReadString(reader),
        ReadFile(reader),
        ReadFiles(reader),
        ReadString(reader),
        ReadFiles(reader),
        reader.ReadSingle(),
        ReadString(reader));

    private static void WriteMesh(BinaryWriter writer, StaticMeshDefinition mesh)
    {
        WriteString(writer, mesh.Name);
        WriteCount(writer, mesh.Vertices.Count, MaxVertices, "vertex");
        foreach (StaticVertex vertex in mesh.Vertices)
        {
            WriteVector3(writer, vertex.Position);
            WriteVector3(writer, vertex.Normal);
        }

        WriteCount(writer, mesh.Indices.Count, MaxIndices, "index");
        foreach (uint index in mesh.Indices)
            writer.Write(index);

        WriteCount(writer, mesh.Sections.Count, MaxSections, "section");
        foreach (StaticMeshSection section in mesh.Sections)
        {
            WriteString(writer, section.MaterialName);
            writer.Write(section.StartIndex);
            writer.Write(section.IndexCount);
        }
    }

    private static StaticMeshDefinition ReadMesh(BinaryReader reader)
    {
        string name = ReadString(reader);
        int vertexCount = ReadCount(reader, MaxVertices, "vertex", allowZero: false);
        var vertices = new StaticVertex[vertexCount];
        for (int index = 0; index < vertices.Length; index++)
            vertices[index] = new StaticVertex(ReadVector3(reader), ReadVector3(reader));

        int indexCount = ReadCount(reader, MaxIndices, "index", allowZero: false);
        var indices = new uint[indexCount];
        for (int index = 0; index < indices.Length; index++)
            indices[index] = reader.ReadUInt32();

        int sectionCount = ReadCount(reader, MaxSections, "section", allowZero: false);
        var sections = new StaticMeshSection[sectionCount];
        for (int index = 0; index < sections.Length; index++)
            sections[index] = new StaticMeshSection(ReadString(reader), reader.ReadInt32(), reader.ReadInt32());
        return new StaticMeshDefinition(name, vertices, indices, sections);
    }

    private static void WriteFiles(BinaryWriter writer, IReadOnlyList<StaticAssetFileEvidence> files)
    {
        WriteCount(writer, files.Count, MaxEvidenceFiles, "file evidence");
        foreach (StaticAssetFileEvidence file in files)
            WriteFile(writer, file);
    }

    private static StaticAssetFileEvidence[] ReadFiles(BinaryReader reader)
    {
        int count = ReadCount(reader, MaxEvidenceFiles, "file evidence");
        var files = new StaticAssetFileEvidence[count];
        for (int index = 0; index < files.Length; index++)
            files[index] = ReadFile(reader);
        return files;
    }

    private static void WriteFile(BinaryWriter writer, StaticAssetFileEvidence file)
    {
        WriteString(writer, file.Path);
        writer.Write(Convert.FromHexString(file.Sha256));
    }

    private static StaticAssetFileEvidence ReadFile(BinaryReader reader) =>
        new(ReadString(reader), Convert.ToHexString(ReadExact(reader, 32, "SHA-256")).ToLowerInvariant());

    private static void WriteString(BinaryWriter writer, string value)
    {
        byte[] bytes = StrictUtf8.GetBytes(value);
        WriteCount(writer, bytes.Length, MaxStringBytes, "string byte");
        writer.Write(bytes);
    }

    private static string ReadString(BinaryReader reader)
    {
        int length = ReadCount(reader, MaxStringBytes, "string byte");
        return StrictUtf8.GetString(ReadExact(reader, length, "string"));
    }

    private static void WriteCount(BinaryWriter writer, int count, int maximum, string field)
    {
        if (count < 0 || count > maximum)
            throw new InvalidDataException($"Cooked static mesh {field} count {count} exceeds the limit {maximum}.");
        writer.Write(checked((uint)count));
    }

    private static int ReadCount(BinaryReader reader, int maximum, string field, bool allowZero = true)
    {
        uint raw = reader.ReadUInt32();
        if (raw > maximum || !allowZero && raw == 0)
            throw new InvalidDataException($"Cooked static mesh {field} count {raw} is invalid.");
        return checked((int)raw);
    }

    private static byte[] ReadExact(BinaryReader reader, int count, string field)
    {
        byte[] bytes = reader.ReadBytes(count);
        if (bytes.Length != count)
            throw new EndOfStreamException($"Cooked static mesh ended while reading {field}.");
        return bytes;
    }

    private static void WriteVector3(BinaryWriter writer, Vector3 value)
    {
        writer.Write(value.X);
        writer.Write(value.Y);
        writer.Write(value.Z);
    }

    private static Vector3 ReadVector3(BinaryReader reader) =>
        new(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
}
