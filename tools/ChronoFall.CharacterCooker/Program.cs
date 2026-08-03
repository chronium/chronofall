using System.Security.Cryptography;
using System.Text.Json;
using ChronoFall.CharacterExperiment.SimpleMesh;
using ChronoFall.CharacterPresentation;
using ChronoFall.CharacterPresentation.Cooking;

namespace ChronoFall.CharacterCooker;

public static class Program
{
    public static int Main(string[] args)
    {
        try
        {
            CharacterCookResult result = CharacterCooker.Run(CharacterCookOptions.Parse(args));
            Console.WriteLine(
                $"CHARACTER_COOK_SUCCESS asset={result.AssetId} clips={result.ClipCount} " +
                $"bytes={result.OutputBytes} sha256={result.OutputSha256} output={result.OutputPath}");
            return 0;
        }
        catch (Exception exception) when (exception is not OutOfMemoryException)
        {
            Console.Error.WriteLine($"CHARACTER_COOK_FAILURE: {exception.Message}");
            return 1;
        }
    }
}

internal static class CharacterCooker
{
    internal static CharacterCookResult Run(CharacterCookOptions options)
    {
        string sourceRoot = Path.GetFullPath(options.SourceRoot);
        string recipeRoot = Path.GetFullPath(options.RecipeRoot ?? options.SourceRoot);
        string recipePath = Path.GetFullPath(options.RecipePath);
        string outputPath = Path.GetFullPath(options.OutputPath);
        string? provenancePath = options.ProvenanceOutput is null
            ? null
            : Path.GetFullPath(options.ProvenanceOutput);
        if (!string.Equals(Path.GetExtension(outputPath), SkeletalAssetCookedFormat.FileExtension, StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException($"Character cooker output must use the '{SkeletalAssetCookedFormat.FileExtension}' extension.", nameof(options));
        if (provenancePath is not null && !string.Equals(Path.GetExtension(provenancePath), ".json", StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException("Character cooker provenance output must use the '.json' extension.", nameof(options));
        RequireDistinctOutput(outputPath, recipePath, "recipe");
        if (provenancePath is not null)
        {
            RequireDistinctOutput(provenancePath, outputPath, "cooked output");
            RequireDistinctOutput(provenancePath, recipePath, "recipe");
        }
        _ = GetPortableRelativePath(recipeRoot, recipePath, "recipe");
        string recipeHash = Sha256(recipePath);
        CharacterCookRecipe recipe = CharacterCookRecipeLoader.Load(recipePath);
        SkeletalAssetCookDescriptor descriptor = recipe.CreateDescriptor();

        string sourcePath = ResolveRequiredFile(sourceRoot, descriptor.SourcePath, "source asset");
        RequireDistinctOutput(outputPath, sourcePath, "source asset");
        if (provenancePath is not null)
            RequireDistinctOutput(provenancePath, sourcePath, "source asset");
        string sourceHash = Sha256(sourcePath);
        if (!string.Equals(sourceHash, descriptor.SourceSha256, StringComparison.Ordinal))
        {
            throw new InvalidDataException(
                $"Source asset '{descriptor.SourcePath}' SHA-256 was {sourceHash}, expected {descriptor.SourceSha256}.");
        }

        foreach (string evidence in descriptor.LicenseEvidencePaths)
        {
            string evidencePath = ResolveRequiredFile(recipeRoot, evidence, "license evidence");
            RequireDistinctOutput(outputPath, evidencePath, "license evidence");
            if (provenancePath is not null)
                RequireDistinctOutput(provenancePath, evidencePath, "license evidence");
        }

        SimpleMeshSkeletalSourceAsset imported = SimpleMeshSkeletalAssetLoader.LoadSourceFromFile(sourcePath);
        RequireIdentifier("mesh node", imported.MeshNodeName, descriptor.SourceMeshNodeName);
        RequireIdentifier("mesh", imported.MeshName, descriptor.SourceMeshName);
        RequireIdentifier("skin", imported.SkinName, descriptor.SourceSkinName);

        var clipsByName = imported.Asset.Animations.ToDictionary(static clip => clip.Name, StringComparer.Ordinal);
        var selectedClips = new AnimationClip[recipe.AnimationClips.Count];
        for (int index = 0; index < selectedClips.Length; index++)
        {
            string name = recipe.AnimationClips[index];
            if (!clipsByName.TryGetValue(name, out AnimationClip? clip))
                throw new InvalidDataException($"Selected animation clip '{name}' was not found in '{descriptor.SourcePath}'.");
            selectedClips[index] = clip;
        }

        var cooked = new CookedSkeletalCharacterAsset(
            descriptor,
            new SkeletalCharacterAsset(imported.Asset.Mesh, selectedClips));
        string? outputDirectory = Path.GetDirectoryName(outputPath);
        if (string.IsNullOrEmpty(outputDirectory))
            throw new ArgumentException("The output path must include a directory.", nameof(options));
        Directory.CreateDirectory(outputDirectory);

        string temporaryPath = Path.Combine(outputDirectory, $".{Path.GetFileName(outputPath)}.{Guid.NewGuid():N}.tmp");
        try
        {
            using (FileStream stream = new(temporaryPath, FileMode.CreateNew, FileAccess.Write, FileShare.None))
                SkeletalAssetCookedFormat.Write(stream, cooked);
            string afterCookSourceHash = Sha256(sourcePath);
            if (!string.Equals(sourceHash, afterCookSourceHash, StringComparison.Ordinal))
                throw new IOException($"Source asset '{descriptor.SourcePath}' changed while it was being cooked.");
            string afterCookRecipeHash = Sha256(recipePath);
            if (!string.Equals(recipeHash, afterCookRecipeHash, StringComparison.Ordinal))
                throw new IOException($"Recipe '{recipePath}' changed while it was being cooked.");
            File.Move(temporaryPath, outputPath, overwrite: true);
        }
        finally
        {
            if (File.Exists(temporaryPath))
                File.Delete(temporaryPath);
        }

        var info = new FileInfo(outputPath);
        var result = new CharacterCookResult(
            descriptor.AssetId,
            selectedClips.Length,
            outputPath,
            info.Length,
            Sha256(outputPath));

        if (provenancePath is not null)
            WriteProvenance(recipeRoot, recipePath, recipeHash, provenancePath, descriptor, selectedClips, result);

        return result;
    }

    private static void WriteProvenance(
        string recipeRoot,
        string recipePath,
        string recipeHash,
        string provenancePath,
        SkeletalAssetCookDescriptor descriptor,
        IReadOnlyList<AnimationClip> selectedClips,
        CharacterCookResult result)
    {
        string recipeRelativePath = GetPortableRelativePath(recipeRoot, recipePath, "recipe");
        var provenance = new CharacterCookProvenance(
            1,
            "client",
            descriptor.AssetId,
            recipeRelativePath,
            recipeHash,
            descriptor.SourcePath,
            descriptor.SourceSha256,
            descriptor.LicenseIdentifier,
            descriptor.LicenseEvidencePaths,
            selectedClips.Select(static clip => clip.Name).ToArray(),
            Path.GetFileName(result.OutputPath),
            result.OutputBytes,
            result.OutputSha256);

        string? directory = Path.GetDirectoryName(provenancePath);
        if (string.IsNullOrEmpty(directory))
            throw new ArgumentException("The provenance output path must include a directory.", nameof(provenancePath));
        Directory.CreateDirectory(directory);

        byte[] contents = JsonSerializer.SerializeToUtf8Bytes(
            provenance,
            new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true,
            });
        string temporaryPath = Path.Combine(directory, $".{Path.GetFileName(provenancePath)}.{Guid.NewGuid():N}.tmp");
        try
        {
            File.WriteAllBytes(temporaryPath, contents);
            File.Move(temporaryPath, provenancePath, overwrite: true);
        }
        finally
        {
            if (File.Exists(temporaryPath))
                File.Delete(temporaryPath);
        }
    }

    private static string GetPortableRelativePath(string root, string path, string field)
    {
        string relative = Path.GetRelativePath(root, path);
        if (Path.IsPathRooted(relative) || relative == ".." || relative.StartsWith(".." + Path.DirectorySeparatorChar, StringComparison.Ordinal))
            throw new InvalidDataException($"The {field} path '{path}' is outside source root '{root}'.");
        return relative.Replace(Path.DirectorySeparatorChar, '/');
    }

    private static string ResolveRequiredFile(string root, string relativePath, string field)
    {
        SkeletalAssetCookDescriptor.ValidateRelativePath(relativePath, nameof(relativePath));
        string path = Path.GetFullPath(Path.Combine(root, relativePath.Replace('/', Path.DirectorySeparatorChar)));
        string relative = Path.GetRelativePath(root, path);
        if (Path.IsPathRooted(relative) || relative == ".." || relative.StartsWith(".." + Path.DirectorySeparatorChar, StringComparison.Ordinal))
            throw new InvalidDataException($"The {field} path '{relativePath}' escapes source root '{root}'.");
        if (!File.Exists(path))
            throw new FileNotFoundException($"Required {field} '{relativePath}' was not found under '{root}'.", path);
        return path;
    }

    private static void RequireIdentifier(string field, string actual, string expected)
    {
        if (!string.Equals(actual, expected, StringComparison.Ordinal))
            throw new InvalidDataException($"Imported {field} was '{actual}', expected '{expected}'.");
    }

    private static void RequireDistinctOutput(string outputPath, string protectedPath, string field)
    {
        if (string.Equals(outputPath, protectedPath, StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException($"Character cooker output cannot overwrite the {field} at '{protectedPath}'.");
    }

    private static string Sha256(string path)
    {
        using FileStream stream = File.OpenRead(path);
        return Convert.ToHexString(SHA256.HashData(stream)).ToLowerInvariant();
    }
}

internal sealed record CharacterCookOptions(
    string SourceRoot,
    string RecipePath,
    string OutputPath,
    string? ProvenanceOutput = null,
    string? RecipeRoot = null)
{
    internal static CharacterCookOptions Parse(string[] args)
    {
        string? sourceRoot = null;
        string? recipeRoot = null;
        string? recipe = null;
        string? output = null;
        string? audience = null;
        string? provenanceOutput = null;
        for (int index = 0; index < args.Length; index++)
        {
            string option = args[index];
            if (!option.StartsWith("--", StringComparison.Ordinal))
                throw new ArgumentException($"Unexpected character cooker argument '{option}'.");
            if (++index >= args.Length || args[index].StartsWith("--", StringComparison.Ordinal))
                throw new ArgumentException($"{option} requires a value.");
            string value = args[index];
            switch (option)
            {
                case "--source-root": sourceRoot = value; break;
                case "--recipe-root": recipeRoot = value; break;
                case "--recipe": recipe = value; break;
                case "--output": output = value; break;
                case "--audience": audience = value; break;
                case "--provenance-output": provenanceOutput = value; break;
                default: throw new ArgumentException($"Unknown character cooker argument '{option}'.");
            }
        }

        if (!string.Equals(audience, "client", StringComparison.Ordinal))
            throw new ArgumentException("--audience must be 'client'; skeletal presentation assets have no server output.");
        return new CharacterCookOptions(
            sourceRoot ?? throw new ArgumentException("--source-root is required."),
            recipe ?? throw new ArgumentException("--recipe is required."),
            output ?? throw new ArgumentException("--output is required."),
            provenanceOutput,
            recipeRoot);
    }
}

internal sealed record CharacterCookResult(
    string AssetId,
    int ClipCount,
    string OutputPath,
    long OutputBytes,
    string OutputSha256);

internal sealed record CharacterCookProvenance(
    int SchemaVersion,
    string Audience,
    string AssetId,
    string RecipePath,
    string RecipeSha256,
    string SourcePath,
    string SourceSha256,
    string LicenseIdentifier,
    IReadOnlyList<string> LicenseEvidence,
    IReadOnlyList<string> AnimationClips,
    string CookedFile,
    long CookedBytes,
    string CookedSha256);
