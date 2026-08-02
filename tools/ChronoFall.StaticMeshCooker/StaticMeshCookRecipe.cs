using System.Text.Json;
using System.Text.Json.Serialization;
using ChronoFall.CharacterPresentation.Cooking;

namespace ChronoFall.StaticMeshCooker;

internal sealed record StaticAssetFileRecipe
{
    [JsonRequired]
    public string Path { get; init; } = string.Empty;

    [JsonRequired]
    public string Sha256 { get; init; } = string.Empty;

    internal StaticAssetFileEvidence CreateEvidence() => new(Path, Sha256);
}

internal sealed record StaticMeshCookRecipe
{
    internal const int CurrentVersion = 1;

    [JsonRequired]
    public int Version { get; init; }

    [JsonRequired]
    public string AssetId { get; init; } = string.Empty;

    [JsonRequired]
    public StaticAssetFileRecipe Source { get; init; } = new();

    [JsonRequired]
    public List<StaticAssetFileRecipe> ExternalResources { get; init; } = [];

    [JsonRequired]
    public string LicenseIdentifier { get; init; } = string.Empty;

    [JsonRequired]
    public List<StaticAssetFileRecipe> LicenseEvidence { get; init; } = [];

    [JsonRequired]
    public float MetersPerSourceUnit { get; init; }

    [JsonRequired]
    public string MaterialPolicy { get; init; } = string.Empty;

    [JsonRequired]
    public List<string> ExpectedMaterials { get; init; } = [];

    internal StaticAssetCookDescriptor CreateDescriptor() => new(
        AssetId,
        Source.CreateEvidence(),
        ExternalResources.Select(static item => item.CreateEvidence()),
        LicenseIdentifier,
        LicenseEvidence.Select(static item => item.CreateEvidence()),
        MetersPerSourceUnit,
        MaterialPolicy);
}

internal static class StaticMeshCookRecipeLoader
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = false,
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true,
        UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow,
    };

    internal static StaticMeshCookRecipe Load(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        try
        {
            using FileStream stream = File.OpenRead(path);
            StaticMeshCookRecipe recipe = JsonSerializer.Deserialize<StaticMeshCookRecipe>(stream, Options) ??
                throw new InvalidDataException("The static mesh cook recipe did not contain a JSON object.");
            Validate(recipe);
            return recipe;
        }
        catch (JsonException exception)
        {
            throw new InvalidDataException($"Static mesh cook recipe '{path}' is invalid JSON.", exception);
        }
    }

    private static void Validate(StaticMeshCookRecipe recipe)
    {
        if (recipe.Version != StaticMeshCookRecipe.CurrentVersion)
            throw new InvalidDataException($"Static mesh cook recipe version must be {StaticMeshCookRecipe.CurrentVersion}.");
        _ = recipe.CreateDescriptor();
        if (recipe.ExpectedMaterials.Count == 0)
            throw new InvalidDataException("Static mesh cook recipes must name at least one expected material.");
        if (recipe.ExpectedMaterials.Any(string.IsNullOrWhiteSpace))
            throw new InvalidDataException("Expected material names cannot be empty.");
        if (recipe.Source.Path.IndexOfAny(['*', '?', '[', ']']) >= 0 ||
            recipe.ExternalResources.Any(static item => item.Path.IndexOfAny(['*', '?', '[', ']']) >= 0))
        {
            throw new InvalidDataException("Static mesh cook recipes cannot use globs.");
        }
    }
}
