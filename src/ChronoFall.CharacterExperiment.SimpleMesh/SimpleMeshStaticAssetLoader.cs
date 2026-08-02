using System.Numerics;
using System.Security.Cryptography;
using Imported = global::SimpleMesh;

namespace ChronoFall.CharacterExperiment.SimpleMesh;

public sealed record StaticSourceMaterialEvidence(
    string Name,
    Vector4 DiffuseColor,
    string? DiffuseTexture,
    bool MetallicRoughness,
    float MetallicFactor,
    float RoughnessFactor);

public sealed class SimpleMeshStaticSourceAsset
{
    internal SimpleMeshStaticSourceAsset(
        StaticMeshDefinition mesh,
        IEnumerable<StaticSourceMaterialEvidence> materials,
        IEnumerable<string> openedExternalResources)
    {
        Mesh = mesh;
        Materials = Array.AsReadOnly(materials.ToArray());
        OpenedExternalResources = Array.AsReadOnly(openedExternalResources.ToArray());
    }

    public StaticMeshDefinition Mesh { get; }

    public IReadOnlyList<StaticSourceMaterialEvidence> Materials { get; }

    public IReadOnlyList<string> OpenedExternalResources { get; }
}

public static class SimpleMeshStaticAssetLoader
{
    private const float DeterminantTolerance = 1e-8f;

    public static SimpleMeshStaticSourceAsset LoadFromFile(
        string assetId,
        string sourceRoot,
        string sourcePath,
        IReadOnlyDictionary<string, string> externalResourceHashes,
        float metersPerSourceUnit)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(assetId);
        ArgumentException.ThrowIfNullOrWhiteSpace(sourceRoot);
        ArgumentException.ThrowIfNullOrWhiteSpace(sourcePath);
        ArgumentNullException.ThrowIfNull(externalResourceHashes);
        if (!float.IsFinite(metersPerSourceUnit) || metersPerSourceUnit <= 0.0f)
            throw new ArgumentOutOfRangeException(nameof(metersPerSourceUnit));

        string root = Path.GetFullPath(sourceRoot);
        string source = Path.GetFullPath(sourcePath);
        try
        {
            RequireWithinRoot(root, source, "primary source");
            RequireNoSymlinkComponents(root, source, "primary source");
            string extension = Path.GetExtension(source);
            if (!extension.Equals(".obj", StringComparison.OrdinalIgnoreCase) &&
                !extension.Equals(".gltf", StringComparison.OrdinalIgnoreCase) &&
                !extension.Equals(".glb", StringComparison.OrdinalIgnoreCase))
            {
                throw Error(source, $"Extension '{extension}' is unsupported; only .obj, .gltf, and .glb are accepted.");
            }
            if (!File.Exists(source))
                throw Error(source, "The primary source file does not exist.");
            if (new FileInfo(source).LinkTarget is not null)
                throw Error(source, "The primary source file cannot be a symbolic link.");

            var warnings = new List<string>();
            var resources = new ExactExternalResources(root, source, externalResourceHashes);
            Imported.Model model;
            using (FileStream stream = File.OpenRead(source))
                model = Imported.Model.FromStream(stream, resources, warnings);

            if (warnings.Count > 0)
                throw Error(source, $"SimpleMesh reported warnings: {string.Join(" | ", warnings)}");
            resources.RequireAllOpened();
            if (model.Skins.Length != 0)
                throw Error(source, $"Static sources cannot contain skins; found {model.Skins.Length}.");
            if (model.Animations.Length != 0)
                throw Error(source, $"Static sources cannot contain animations; found {model.Animations.Length}.");

            return MapModel(assetId, model, source, metersPerSourceUnit, resources.OpenedPaths);
        }
        catch (StaticAssetLoadException)
        {
            throw;
        }
        catch (Exception exception)
        {
            throw Error(source, $"SimpleMesh import failed: {exception.Message}", exception);
        }
    }

    internal static SimpleMeshStaticSourceAsset MapModel(
        string assetId,
        Imported.Model model,
        string source,
        float metersPerSourceUnit,
        IEnumerable<string>? openedExternalResources = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(assetId);
        ArgumentNullException.ThrowIfNull(model);
        ArgumentException.ThrowIfNullOrWhiteSpace(source);
        if (!float.IsFinite(metersPerSourceUnit) || metersPerSourceUnit <= 0.0f)
            throw new ArgumentOutOfRangeException(nameof(metersPerSourceUnit));
        if (model.Roots.Length == 0)
            throw Error(source, "The model contains no scene roots.");

        var vertices = new List<StaticVertex>();
        var indices = new List<uint>();
        var sections = new List<StaticMeshSection>();
        var materials = new List<StaticSourceMaterialEvidence>();
        var visited = new HashSet<Imported.ModelNode>();
        for (int rootIndex = 0; rootIndex < model.Roots.Length; rootIndex++)
            AppendNode(model.Roots[rootIndex], Matrix4x4.Identity, $"root[{rootIndex}]", visited);

        if (sections.Count == 0)
            throw Error(source, "The selected scene contains no indexed triangle geometry.");
        try
        {
            return new SimpleMeshStaticSourceAsset(
                new StaticMeshDefinition(assetId, vertices, indices, sections),
                materials,
                openedExternalResources ?? []);
        }
        catch (Exception exception)
        {
            throw Error(source, exception.Message, exception);
        }

        void AppendNode(Imported.ModelNode? node, Matrix4x4 parentTransform, string structuralPath, HashSet<Imported.ModelNode> seen)
        {
            if (node is null)
                throw Error(source, $"Scene path '{structuralPath}' contains a null node.");
            if (!seen.Add(node))
                throw Error(source, $"Node '{node.Name}' appears more than once in the scene hierarchy.");
            if (!IsFinite(node.Transform))
                throw Error(source, $"Node '{node.Name}' has a non-finite transform.");

            Matrix4x4 world = node.Transform * parentTransform;
            if (!Matrix4x4.Invert(world, out Matrix4x4 inverse))
                throw Error(source, $"Node '{node.Name}' has a non-invertible world transform.");
            float determinant = world.GetDeterminant();
            if (!float.IsFinite(determinant) || determinant <= DeterminantTolerance)
                throw Error(source, $"Node '{node.Name}' has a reflected or singular world transform.");

            string nodeName = string.IsNullOrWhiteSpace(node.Name) ? "unnamed" : node.Name;
            string nodePath = $"{structuralPath}:{nodeName}";
            if (node.Geometry is not null)
                AppendGeometry(node.Geometry, world, Matrix4x4.Transpose(inverse), nodePath);
            for (int childIndex = 0; childIndex < node.Children.Count; childIndex++)
                AppendNode(node.Children[childIndex], world, $"{nodePath}/child[{childIndex}]", seen);
        }

        void AppendGeometry(Imported.Geometry geometry, Matrix4x4 world, Matrix4x4 normalTransform, string nodePath)
        {
            if (geometry.Kind != Imported.GeometryKind.Triangles)
                throw Error(source, $"Node '{nodePath}' contains unsupported geometry kind '{geometry.Kind}'.");
            if ((geometry.Vertices.Descriptor.Attributes & Imported.VertexAttributes.Normal) != Imported.VertexAttributes.Normal)
                throw Error(source, $"Node '{nodePath}' has no source normals; normal generation is not permitted.");
            if (geometry.Groups.Length == 0)
                throw Error(source, $"Node '{nodePath}' has no material groups.");

            uint baseVertex = checked((uint)vertices.Count);
            for (int vertexIndex = 0; vertexIndex < geometry.Vertices.Count; vertexIndex++)
            {
                Vector3 position = Vector3.Transform(geometry.Vertices.Position[vertexIndex], world) * metersPerSourceUnit;
                Vector3 normal = Vector3.TransformNormal(geometry.Vertices.Normal[vertexIndex], normalTransform);
                if (!IsFinite(position))
                    throw Error(source, $"Node '{nodePath}' contains a non-finite transformed position.");
                if (!IsFinite(normal) || normal.LengthSquared() <= 1e-12f)
                    throw Error(source, $"Node '{nodePath}' contains a zero or non-finite transformed normal.");
                vertices.Add(new StaticVertex(position, Vector3.Normalize(normal)));
            }

            string geometryName = string.IsNullOrWhiteSpace(geometry.Name) ? "unnamed-geometry" : geometry.Name;
            for (int groupIndex = 0; groupIndex < geometry.Groups.Length; groupIndex++)
            {
                Imported.TriangleGroup group = geometry.Groups[groupIndex] ??
                    throw Error(source, $"Node '{nodePath}' contains a null material group.");
                if (group.Material is null)
                    throw Error(source, $"Node '{nodePath}' group {groupIndex} has no material.");
                if (group.IndexCount <= 0 || group.IndexCount % 3 != 0 || group.StartIndex < 0 ||
                    group.StartIndex + group.IndexCount > geometry.Indices.Length)
                {
                    throw Error(source, $"Node '{nodePath}' group {groupIndex} has an invalid triangle index range.");
                }

                int sectionStart = indices.Count;
                for (int localIndex = 0; localIndex < group.IndexCount; localIndex++)
                {
                    long resolved = (long)geometry.Indices[group.StartIndex + localIndex] + group.BaseVertex;
                    if (resolved < 0 || resolved >= geometry.Vertices.Count)
                        throw Error(source, $"Node '{nodePath}' group {groupIndex} references vertex {resolved} outside its geometry.");
                    indices.Add(checked(baseVertex + (uint)resolved));
                }

                string materialName = string.IsNullOrWhiteSpace(group.Material.Name)
                    ? $"unnamed-material[{groupIndex}]"
                    : group.Material.Name;
                sections.Add(new StaticMeshSection(
                    $"{nodePath}/{geometryName}/group[{groupIndex}]:{materialName}",
                    sectionStart,
                    group.IndexCount));
                var diffuse = new Vector4(
                    group.Material.DiffuseColor.R,
                    group.Material.DiffuseColor.G,
                    group.Material.DiffuseColor.B,
                    group.Material.DiffuseColor.A);
                if (!IsFinite(diffuse) || !float.IsFinite(group.Material.MetallicFactor) || !float.IsFinite(group.Material.RoughnessFactor))
                    throw Error(source, $"Node '{nodePath}' material '{materialName}' contains non-finite evidence values.");
                materials.Add(new StaticSourceMaterialEvidence(
                    materialName,
                    diffuse,
                    group.Material.DiffuseTexture?.Name,
                    group.Material.MetallicRoughness,
                    group.Material.MetallicFactor,
                    group.Material.RoughnessFactor));
            }
        }
    }

    private static void RequireWithinRoot(string root, string path, string field)
    {
        string relative = Path.GetRelativePath(root, path);
        if (Path.IsPathRooted(relative) || relative == ".." || relative.StartsWith(".." + Path.DirectorySeparatorChar, StringComparison.Ordinal))
            throw new InvalidDataException($"The {field} path '{path}' escapes source root '{root}'.");
    }

    private static void RequireNoSymlinkComponents(string root, string path, string field)
    {
        string relative = Path.GetRelativePath(root, path);
        string current = root;
        foreach (string component in relative.Split(Path.DirectorySeparatorChar))
        {
            current = Path.Combine(current, component);
            FileSystemInfo info = Directory.Exists(current) ? new DirectoryInfo(current) : new FileInfo(current);
            if (info.LinkTarget is not null)
                throw new InvalidDataException($"The {field} path contains symbolic link '{current}'.");
        }
    }

    private static bool IsFinite(Vector3 value) =>
        float.IsFinite(value.X) && float.IsFinite(value.Y) && float.IsFinite(value.Z);

    private static bool IsFinite(Vector4 value) =>
        float.IsFinite(value.X) && float.IsFinite(value.Y) && float.IsFinite(value.Z) && float.IsFinite(value.W);

    private static bool IsFinite(Matrix4x4 value) =>
        float.IsFinite(value.M11) && float.IsFinite(value.M12) && float.IsFinite(value.M13) && float.IsFinite(value.M14) &&
        float.IsFinite(value.M21) && float.IsFinite(value.M22) && float.IsFinite(value.M23) && float.IsFinite(value.M24) &&
        float.IsFinite(value.M31) && float.IsFinite(value.M32) && float.IsFinite(value.M33) && float.IsFinite(value.M34) &&
        float.IsFinite(value.M41) && float.IsFinite(value.M42) && float.IsFinite(value.M43) && float.IsFinite(value.M44);

    private static StaticAssetLoadException Error(string source, string reason, Exception? inner = null) =>
        new(source, reason, inner);

    private sealed class ExactExternalResources : Imported.IExternalResources
    {
        private readonly string root;
        private readonly string sourceDirectory;
        private readonly IReadOnlyDictionary<string, string> hashes;
        private readonly HashSet<string> opened = new(StringComparer.Ordinal);

        internal ExactExternalResources(string root, string sourcePath, IReadOnlyDictionary<string, string> hashes)
        {
            this.root = root;
            sourceDirectory = Path.GetDirectoryName(sourcePath) ?? root;
            var validated = new Dictionary<string, string>(StringComparer.Ordinal);
            foreach ((string path, string hash) in hashes)
            {
                ValidatePortablePath(path, nameof(hashes));
                if (hash.Length != 64 || hash.Any(static character => !Uri.IsHexDigit(character)))
                    throw new ArgumentException($"External resource '{path}' has an invalid SHA-256.", nameof(hashes));
                if (!validated.TryAdd(path, hash.ToLowerInvariant()))
                    throw new ArgumentException($"External resource '{path}' is duplicated.", nameof(hashes));
            }
            this.hashes = validated;
        }

        public bool CanLoadResources => true;

        internal IReadOnlyList<string> OpenedPaths => opened.Order(StringComparer.Ordinal).ToArray();

        public Stream? OpenStream(string filename)
        {
            if (string.IsNullOrWhiteSpace(filename) || Path.IsPathRooted(filename) || filename.Contains('\\'))
                throw new InvalidDataException($"External resource reference '{filename}' is not portable.");
            string fullPath = Path.GetFullPath(Path.Combine(sourceDirectory, filename));
            RequireWithinRoot(root, fullPath, "external resource");
            RequireNoSymlinkComponents(root, fullPath, "external resource");
            string portable = Path.GetRelativePath(root, fullPath).Replace(Path.DirectorySeparatorChar, '/');
            if (!hashes.TryGetValue(portable, out string? expectedHash))
                throw new InvalidDataException($"External resource '{portable}' is not declared by the recipe.");
            if (!File.Exists(fullPath))
                throw new FileNotFoundException($"Declared external resource '{portable}' was not found.", fullPath);
            if (new FileInfo(fullPath).LinkTarget is not null)
                throw new InvalidDataException($"External resource '{portable}' cannot be a symbolic link.");

            byte[] bytes = File.ReadAllBytes(fullPath);
            string actualHash = Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant();
            if (!string.Equals(actualHash, expectedHash, StringComparison.Ordinal))
                throw new InvalidDataException($"External resource '{portable}' SHA-256 was {actualHash}, expected {expectedHash}.");
            opened.Add(portable);
            return new MemoryStream(bytes, writable: false);
        }

        internal void RequireAllOpened()
        {
            string[] unused = hashes.Keys.Except(opened, StringComparer.Ordinal).Order(StringComparer.Ordinal).ToArray();
            if (unused.Length != 0)
                throw new InvalidDataException($"Recipe declares unused external resources: {string.Join(", ", unused)}.");
        }

        private static void ValidatePortablePath(string path, string parameterName)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(path, parameterName);
            if (Path.IsPathRooted(path) || path.Contains('\\') ||
                path.Split('/').Any(static segment => segment.Length == 0 || segment is "." or ".."))
            {
                throw new ArgumentException("External resource paths must be portable root-relative paths.", parameterName);
            }
        }
    }
}
