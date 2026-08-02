using System.Numerics;
using System.Security.Cryptography;
using System.Text.Json;
using ChronoFall.CharacterExperiment.SimpleMesh;
using ChronoFall.CharacterPresentation.Cooking;

namespace ChronoFall.StaticMeshCooker;

public static class Program
{
    public static int Main(string[] args)
    {
        try
        {
            StaticMeshCookResult result = StaticMeshCooker.Run(StaticMeshCookOptions.Parse(args));
            Console.WriteLine(
                $"STATIC_MESH_COOK_SUCCESS asset={result.AssetId} sections={result.SectionCount} " +
                $"bytes={result.OutputBytes} sha256={result.OutputSha256} output={result.OutputPath}");
            return 0;
        }
        catch (Exception exception) when (exception is not OutOfMemoryException)
        {
            Console.Error.WriteLine($"STATIC_MESH_COOK_FAILURE: {exception.Message}");
            return 1;
        }
    }
}

internal static class StaticMeshCooker
{
    internal static StaticMeshCookResult Run(StaticMeshCookOptions options)
    {
        string root = Path.GetFullPath(options.SourceRoot);
        string recipePath = Path.GetFullPath(options.RecipePath);
        string outputPath = Path.GetFullPath(options.OutputPath);
        string? provenancePath = options.ProvenanceOutput is null ? null : Path.GetFullPath(options.ProvenanceOutput);
        RequireWithinRoot(root, recipePath, "recipe");
        if (!string.Equals(Path.GetExtension(outputPath), StaticMeshCookedFormat.FileExtension, StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException($"Static mesh cooker output must use the '{StaticMeshCookedFormat.FileExtension}' extension.");
        if (provenancePath is not null && !string.Equals(Path.GetExtension(provenancePath), ".json", StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException("Static mesh cooker provenance output must use the '.json' extension.");
        RequireDistinctOutput(outputPath, recipePath, "recipe");
        if (provenancePath is not null)
        {
            RequireDistinctOutput(provenancePath, outputPath, "cooked output");
            RequireDistinctOutput(provenancePath, recipePath, "recipe");
        }

        string recipeHash = Sha256(recipePath);
        StaticMeshCookRecipe recipe = StaticMeshCookRecipeLoader.Load(recipePath);
        StaticAssetCookDescriptor descriptor = recipe.CreateDescriptor();
        var protectedInputs = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            [recipePath] = recipeHash,
        };
        string sourcePath = ValidateEvidenceFile(root, descriptor.PrimarySource, "primary source", protectedInputs);
        foreach (StaticAssetFileEvidence resource in descriptor.ExternalResources)
            _ = ValidateEvidenceFile(root, resource, "external resource", protectedInputs);
        foreach (StaticAssetFileEvidence evidence in descriptor.LicenseEvidence)
            _ = ValidateEvidenceFile(root, evidence, "license evidence", protectedInputs);
        foreach (string protectedInput in protectedInputs.Keys)
        {
            RequireDistinctOutput(outputPath, protectedInput, "declared input");
            if (provenancePath is not null)
                RequireDistinctOutput(provenancePath, protectedInput, "declared input");
        }

        var externalHashes = descriptor.ExternalResources.ToDictionary(
            static item => item.Path,
            static item => item.Sha256,
            StringComparer.Ordinal);
        SimpleMeshStaticSourceAsset imported = SimpleMeshStaticAssetLoader.LoadFromFile(
            descriptor.AssetId,
            root,
            sourcePath,
            externalHashes,
            descriptor.MetersPerSourceUnit);
        string[] actualMaterials = imported.Materials.Select(static material => material.Name).ToArray();
        if (!actualMaterials.SequenceEqual(recipe.ExpectedMaterials, StringComparer.Ordinal))
        {
            throw new InvalidDataException(
                $"Imported material sequence [{string.Join(", ", actualMaterials)}] did not match recipe sequence " +
                $"[{string.Join(", ", recipe.ExpectedMaterials)}].");
        }

        var cooked = new CookedStaticMeshAsset(descriptor, imported.Mesh);
        string outputDirectory = RequireOutputDirectory(outputPath, nameof(options.OutputPath));
        Directory.CreateDirectory(outputDirectory);
        string temporaryOutput = Path.Combine(outputDirectory, $".{Path.GetFileName(outputPath)}.{Guid.NewGuid():N}.tmp");
        string? temporaryProvenance = null;
        try
        {
            using (FileStream stream = new(temporaryOutput, FileMode.CreateNew, FileAccess.Write, FileShare.None))
                StaticMeshCookedFormat.Write(stream, cooked);
            var info = new FileInfo(temporaryOutput);
            var result = new StaticMeshCookResult(
                descriptor.AssetId,
                imported.Mesh.Sections.Count,
                outputPath,
                info.Length,
                Sha256(temporaryOutput));

            if (provenancePath is not null)
            {
                string provenanceDirectory = RequireOutputDirectory(provenancePath, nameof(options.ProvenanceOutput));
                Directory.CreateDirectory(provenanceDirectory);
                temporaryProvenance = Path.Combine(
                    provenanceDirectory,
                    $".{Path.GetFileName(provenancePath)}.{Guid.NewGuid():N}.tmp");
                WriteProvenance(root, recipePath, recipeHash, temporaryProvenance, descriptor, imported, result);
            }

            foreach ((string path, string beforeHash) in protectedInputs)
            {
                string afterHash = Sha256(path);
                if (!string.Equals(afterHash, beforeHash, StringComparison.Ordinal))
                    throw new IOException($"Protected input '{GetPortableRelativePath(root, path, "input")}' changed while cooking.");
            }

            File.Move(temporaryOutput, outputPath, overwrite: true);
            if (provenancePath is not null)
            {
                File.Move(temporaryProvenance!, provenancePath, overwrite: true);
                temporaryProvenance = null;
            }
            return result;
        }
        finally
        {
            if (File.Exists(temporaryOutput))
                File.Delete(temporaryOutput);
            if (temporaryProvenance is not null && File.Exists(temporaryProvenance))
                File.Delete(temporaryProvenance);
        }
    }

    private static void WriteProvenance(
        string root,
        string recipePath,
        string recipeHash,
        string path,
        StaticAssetCookDescriptor descriptor,
        SimpleMeshStaticSourceAsset imported,
        StaticMeshCookResult result)
    {
        StaticMaterialProvenance[] materials = imported.Materials.Select(static material => new StaticMaterialProvenance(
            material.Name,
            material.DiffuseColor.X,
            material.DiffuseColor.Y,
            material.DiffuseColor.Z,
            material.DiffuseColor.W,
            material.DiffuseTexture,
            material.MetallicRoughness,
            material.MetallicFactor,
            material.RoughnessFactor)).ToArray();
        var provenance = new StaticMeshCookProvenance(
            1,
            "client",
            descriptor.AssetId,
            GetPortableRelativePath(root, recipePath, "recipe"),
            recipeHash,
            descriptor.PrimarySource,
            descriptor.ExternalResources,
            descriptor.LicenseIdentifier,
            descriptor.LicenseEvidence,
            descriptor.MetersPerSourceUnit,
            descriptor.MaterialPolicy,
            imported.Mesh.Sections.Select(static section => section.MaterialName).ToArray(),
            materials,
            Path.GetFileName(result.OutputPath),
            result.OutputBytes,
            result.OutputSha256);
        byte[] contents = JsonSerializer.SerializeToUtf8Bytes(provenance, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true,
        });
        File.WriteAllBytes(path, contents);
    }

    private static string ValidateEvidenceFile(
        string root,
        StaticAssetFileEvidence evidence,
        string field,
        IDictionary<string, string> protectedInputs)
    {
        string path = ResolveRequiredFile(root, evidence.Path, field);
        string hash = Sha256(path);
        if (!string.Equals(hash, evidence.Sha256, StringComparison.Ordinal))
            throw new InvalidDataException($"{field} '{evidence.Path}' SHA-256 was {hash}, expected {evidence.Sha256}.");
        if (!protectedInputs.TryAdd(path, hash))
            throw new InvalidDataException($"Input '{evidence.Path}' is declared more than once across recipe roles.");
        return path;
    }

    private static string ResolveRequiredFile(string root, string relativePath, string field)
    {
        StaticAssetCookDescriptor.ValidateRelativePath(relativePath, nameof(relativePath));
        string path = Path.GetFullPath(Path.Combine(root, relativePath.Replace('/', Path.DirectorySeparatorChar)));
        RequireWithinRoot(root, path, field);
        RequireNoSymlinkComponents(root, path, field);
        if (!File.Exists(path))
            throw new FileNotFoundException($"Required {field} '{relativePath}' was not found under '{root}'.", path);
        if (new FileInfo(path).LinkTarget is not null)
            throw new InvalidDataException($"Required {field} '{relativePath}' cannot be a symbolic link.");
        return path;
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

    private static string GetPortableRelativePath(string root, string path, string field)
    {
        RequireWithinRoot(root, path, field);
        return Path.GetRelativePath(root, path).Replace(Path.DirectorySeparatorChar, '/');
    }

    private static string RequireOutputDirectory(string path, string parameterName) =>
        Path.GetDirectoryName(path) is { Length: > 0 } directory
            ? directory
            : throw new ArgumentException("Output paths must include a directory.", parameterName);

    private static void RequireDistinctOutput(string outputPath, string protectedPath, string field)
    {
        if (string.Equals(outputPath, protectedPath, StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException($"Static mesh cooker output cannot overwrite the {field} at '{protectedPath}'.");
    }

    private static string Sha256(string path)
    {
        using FileStream stream = File.OpenRead(path);
        return Convert.ToHexString(SHA256.HashData(stream)).ToLowerInvariant();
    }
}

internal sealed record StaticMeshCookOptions(
    string SourceRoot,
    string RecipePath,
    string OutputPath,
    string? ProvenanceOutput)
{
    internal static StaticMeshCookOptions Parse(string[] args)
    {
        string? sourceRoot = null;
        string? recipe = null;
        string? output = null;
        string? provenance = null;
        string? audience = null;
        for (int index = 0; index < args.Length; index++)
        {
            string option = args[index];
            if (!option.StartsWith("--", StringComparison.Ordinal))
                throw new ArgumentException($"Unexpected static mesh cooker argument '{option}'.");
            if (++index >= args.Length || args[index].StartsWith("--", StringComparison.Ordinal))
                throw new ArgumentException($"{option} requires a value.");
            string value = args[index];
            switch (option)
            {
                case "--source-root": sourceRoot = value; break;
                case "--recipe": recipe = value; break;
                case "--output": output = value; break;
                case "--provenance-output": provenance = value; break;
                case "--audience": audience = value; break;
                default: throw new ArgumentException($"Unknown static mesh cooker argument '{option}'.");
            }
        }
        if (!string.Equals(audience, "client", StringComparison.Ordinal))
            throw new ArgumentException("--audience must be 'client'; static presentation assets have no server output.");
        return new StaticMeshCookOptions(
            sourceRoot ?? throw new ArgumentException("--source-root is required."),
            recipe ?? throw new ArgumentException("--recipe is required."),
            output ?? throw new ArgumentException("--output is required."),
            provenance);
    }
}

internal sealed record StaticMeshCookResult(
    string AssetId,
    int SectionCount,
    string OutputPath,
    long OutputBytes,
    string OutputSha256);

internal sealed record StaticMaterialProvenance(
    string Name,
    float DiffuseRed,
    float DiffuseGreen,
    float DiffuseBlue,
    float DiffuseAlpha,
    string? DiffuseTexture,
    bool MetallicRoughness,
    float MetallicFactor,
    float RoughnessFactor);

internal sealed record StaticMeshCookProvenance(
    int SchemaVersion,
    string Audience,
    string AssetId,
    string RecipePath,
    string RecipeSha256,
    StaticAssetFileEvidence PrimarySource,
    IReadOnlyList<StaticAssetFileEvidence> ExternalResources,
    string LicenseIdentifier,
    IReadOnlyList<StaticAssetFileEvidence> LicenseEvidence,
    float MetersPerSourceUnit,
    string MaterialPolicy,
    IReadOnlyList<string> Sections,
    IReadOnlyList<StaticMaterialProvenance> Materials,
    string CookedFile,
    long CookedBytes,
    string CookedSha256);
