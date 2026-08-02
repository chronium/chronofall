using System.Text.Json;
using System.Text.Json.Serialization;
using ChronoFall.CharacterPresentation.Cooking;

namespace ChronoFall.CharacterCooker;

internal sealed record CharacterCookRecipe
{
    internal const int CurrentVersion = 1;

    [JsonRequired]
    public int Version { get; init; }

    [JsonRequired]
    public string AssetId { get; init; } = string.Empty;

    [JsonRequired]
    public string Source { get; init; } = string.Empty;

    [JsonRequired]
    public string SourceSha256 { get; init; } = string.Empty;

    [JsonRequired]
    public string LicenseIdentifier { get; init; } = string.Empty;

    [JsonRequired]
    public List<string> LicenseEvidence { get; init; } = [];

    [JsonRequired]
    public string MeshNodeName { get; init; } = string.Empty;

    [JsonRequired]
    public string MeshName { get; init; } = string.Empty;

    [JsonRequired]
    public string SkinName { get; init; } = string.Empty;

    [JsonRequired]
    public List<string> AnimationClips { get; init; } = [];

    internal SkeletalAssetCookDescriptor CreateDescriptor() => new(
        AssetId,
        Source,
        SourceSha256,
        LicenseIdentifier,
        LicenseEvidence,
        MeshNodeName,
        MeshName,
        SkinName);
}

internal static class CharacterCookRecipeLoader
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = false,
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true,
        UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow,
    };

    internal static CharacterCookRecipe Load(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        try
        {
            using FileStream stream = File.OpenRead(path);
            CharacterCookRecipe recipe = JsonSerializer.Deserialize<CharacterCookRecipe>(stream, Options)
                ?? throw new InvalidDataException("The recipe did not contain a JSON object.");
            Validate(recipe);
            return recipe;
        }
        catch (JsonException exception)
        {
            throw new InvalidDataException($"Character cook recipe '{path}' is invalid JSON.", exception);
        }
    }

    private static void Validate(CharacterCookRecipe recipe)
    {
        if (recipe.Version != CharacterCookRecipe.CurrentVersion)
            throw new InvalidDataException($"Character cook recipe version must be {CharacterCookRecipe.CurrentVersion}.");

        _ = recipe.CreateDescriptor();
        if (recipe.AnimationClips.Count == 0)
            throw new InvalidDataException("Character cook recipe must select at least one animation clip.");
        if (recipe.AnimationClips.Any(string.IsNullOrWhiteSpace))
            throw new InvalidDataException("Character cook recipe animation names cannot be empty.");
        if (recipe.AnimationClips.Distinct(StringComparer.Ordinal).Count() != recipe.AnimationClips.Count)
            throw new InvalidDataException("Character cook recipe animation names must be unique.");
    }
}
