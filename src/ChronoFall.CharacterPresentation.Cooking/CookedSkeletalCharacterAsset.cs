namespace ChronoFall.CharacterPresentation.Cooking;

public sealed class SkeletalAssetCookDescriptor
{
    public SkeletalAssetCookDescriptor(
        string assetId,
        string sourcePath,
        string sourceSha256,
        string licenseIdentifier,
        IEnumerable<string> licenseEvidencePaths,
        string sourceMeshNodeName,
        string sourceMeshName,
        string sourceSkinName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(assetId);
        ArgumentException.ThrowIfNullOrWhiteSpace(sourceSha256);
        ArgumentException.ThrowIfNullOrWhiteSpace(licenseIdentifier);
        ArgumentException.ThrowIfNullOrWhiteSpace(sourceMeshNodeName);
        ArgumentException.ThrowIfNullOrWhiteSpace(sourceMeshName);
        ArgumentException.ThrowIfNullOrWhiteSpace(sourceSkinName);
        ArgumentNullException.ThrowIfNull(licenseEvidencePaths);

        ValidateAssetId(assetId);
        ValidateRelativePath(sourcePath, nameof(sourcePath));
        ValidateSha256(sourceSha256);

        string[] evidence = licenseEvidencePaths.ToArray();
        if (evidence.Length == 0)
            throw new ArgumentException("At least one license-evidence path is required.", nameof(licenseEvidencePaths));
        if (evidence.Distinct(StringComparer.Ordinal).Count() != evidence.Length)
            throw new ArgumentException("License-evidence paths must be unique.", nameof(licenseEvidencePaths));
        foreach (string path in evidence)
            ValidateRelativePath(path, nameof(licenseEvidencePaths));

        AssetId = assetId;
        SourcePath = sourcePath;
        SourceSha256 = sourceSha256.ToLowerInvariant();
        LicenseIdentifier = licenseIdentifier;
        LicenseEvidencePaths = Array.AsReadOnly(evidence);
        SourceMeshNodeName = sourceMeshNodeName;
        SourceMeshName = sourceMeshName;
        SourceSkinName = sourceSkinName;
    }

    public string AssetId { get; }

    public string SourcePath { get; }

    public string SourceSha256 { get; }

    public string LicenseIdentifier { get; }

    public IReadOnlyList<string> LicenseEvidencePaths { get; }

    public string SourceMeshNodeName { get; }

    public string SourceMeshName { get; }

    public string SourceSkinName { get; }

    private static void ValidateAssetId(string assetId)
    {
        foreach (char character in assetId)
        {
            bool valid = character is >= 'a' and <= 'z' or >= '0' and <= '9' || character == '-';
            if (!valid)
                throw new ArgumentException("Asset IDs must contain only lowercase ASCII letters, digits, or '-'.", nameof(assetId));
        }
    }

    internal static void ValidateRelativePath(string path, string parameterName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path, parameterName);
        if (Path.IsPathRooted(path) || path.Contains('\\'))
            throw new ArgumentException("Asset paths must be portable relative paths using '/'.", parameterName);
        if (path.Split('/').Any(static segment => segment.Length == 0 || segment is "." or ".."))
            throw new ArgumentException("Asset paths cannot contain empty, current-directory, or parent-directory segments.", parameterName);
    }

    private static void ValidateSha256(string value)
    {
        if (value.Length != 64 || value.Any(static character => !Uri.IsHexDigit(character)))
            throw new ArgumentException("Source SHA-256 must contain exactly 64 hexadecimal characters.", nameof(value));
    }
}

public sealed class CookedSkeletalCharacterAsset
{
    public CookedSkeletalCharacterAsset(
        SkeletalAssetCookDescriptor descriptor,
        SkeletalCharacterAsset asset)
    {
        Descriptor = descriptor ?? throw new ArgumentNullException(nameof(descriptor));
        Asset = asset ?? throw new ArgumentNullException(nameof(asset));
    }

    public SkeletalAssetCookDescriptor Descriptor { get; }

    public SkeletalCharacterAsset Asset { get; }
}
