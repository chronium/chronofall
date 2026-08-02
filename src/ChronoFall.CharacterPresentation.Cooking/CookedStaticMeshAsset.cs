namespace ChronoFall.CharacterPresentation.Cooking;

public sealed class StaticAssetFileEvidence
{
    public StaticAssetFileEvidence(string path, string sha256)
    {
        StaticAssetCookDescriptor.ValidateRelativePath(path, nameof(path));
        StaticAssetCookDescriptor.ValidateSha256(sha256, nameof(sha256));
        Path = path;
        Sha256 = sha256.ToLowerInvariant();
    }

    public string Path { get; }

    public string Sha256 { get; }
}

public sealed class StaticAssetCookDescriptor
{
    public const string SectionNamesOnlyMaterialPolicy = "section-names-only";

    public StaticAssetCookDescriptor(
        string assetId,
        StaticAssetFileEvidence primarySource,
        IEnumerable<StaticAssetFileEvidence> externalResources,
        string licenseIdentifier,
        IEnumerable<StaticAssetFileEvidence> licenseEvidence,
        float metersPerSourceUnit,
        string materialPolicy)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(assetId);
        ArgumentNullException.ThrowIfNull(primarySource);
        ArgumentNullException.ThrowIfNull(externalResources);
        ArgumentException.ThrowIfNullOrWhiteSpace(licenseIdentifier);
        ArgumentNullException.ThrowIfNull(licenseEvidence);
        ValidateAssetId(assetId);
        if (!float.IsFinite(metersPerSourceUnit) || metersPerSourceUnit <= 0.0f)
            throw new ArgumentOutOfRangeException(nameof(metersPerSourceUnit));
        if (!string.Equals(materialPolicy, SectionNamesOnlyMaterialPolicy, StringComparison.Ordinal))
            throw new ArgumentException($"Material policy must be '{SectionNamesOnlyMaterialPolicy}'.", nameof(materialPolicy));

        StaticAssetFileEvidence[] resources = externalResources.ToArray();
        StaticAssetFileEvidence[] evidence = licenseEvidence.ToArray();
        ValidateFileSet(resources, nameof(externalResources), allowEmpty: true);
        ValidateFileSet(evidence, nameof(licenseEvidence), allowEmpty: false);

        string[] allPaths =
        [
            primarySource.Path,
            .. resources.Select(static item => item.Path),
            .. evidence.Select(static item => item.Path),
        ];
        if (allPaths.Distinct(StringComparer.Ordinal).Count() != allPaths.Length)
            throw new ArgumentException("Primary, external, and license-evidence paths must be unique across roles.");

        AssetId = assetId;
        PrimarySource = primarySource;
        ExternalResources = Array.AsReadOnly(resources);
        LicenseIdentifier = licenseIdentifier;
        LicenseEvidence = Array.AsReadOnly(evidence);
        MetersPerSourceUnit = metersPerSourceUnit;
        MaterialPolicy = materialPolicy;
    }

    public string AssetId { get; }

    public StaticAssetFileEvidence PrimarySource { get; }

    public IReadOnlyList<StaticAssetFileEvidence> ExternalResources { get; }

    public string LicenseIdentifier { get; }

    public IReadOnlyList<StaticAssetFileEvidence> LicenseEvidence { get; }

    public float MetersPerSourceUnit { get; }

    public string MaterialPolicy { get; }

    internal static void ValidateRelativePath(string path, string parameterName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path, parameterName);
        if (Path.IsPathRooted(path) || path.Contains('\\'))
            throw new ArgumentException("Asset paths must be portable relative paths using '/'.", parameterName);
        if (path.Split('/').Any(static segment => segment.Length == 0 || segment is "." or ".."))
            throw new ArgumentException("Asset paths cannot contain empty, current-directory, or parent-directory segments.", parameterName);
    }

    internal static void ValidateSha256(string value, string parameterName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value, parameterName);
        if (value.Length != 64 || value.Any(static character => !Uri.IsHexDigit(character)))
            throw new ArgumentException("SHA-256 values must contain exactly 64 hexadecimal characters.", parameterName);
    }

    private static void ValidateAssetId(string assetId)
    {
        foreach (char character in assetId)
        {
            bool valid = character is >= 'a' and <= 'z' or >= '0' and <= '9' || character == '-';
            if (!valid)
                throw new ArgumentException("Asset IDs must contain only lowercase ASCII letters, digits, or '-'.", nameof(assetId));
        }
    }

    private static void ValidateFileSet(StaticAssetFileEvidence[] files, string parameterName, bool allowEmpty)
    {
        if (!allowEmpty && files.Length == 0)
            throw new ArgumentException("At least one file is required.", parameterName);
        if (files.Length > StaticMeshCookedFormat.MaxEvidenceFiles)
            throw new ArgumentException($"At most {StaticMeshCookedFormat.MaxEvidenceFiles} files are allowed.", parameterName);
        if (files.Any(static file => file is null))
            throw new ArgumentException("File evidence cannot contain null entries.", parameterName);
        if (files.Select(static file => file.Path).Distinct(StringComparer.Ordinal).Count() != files.Length)
            throw new ArgumentException("File evidence paths must be unique.", parameterName);
    }
}

public sealed class CookedStaticMeshAsset
{
    public CookedStaticMeshAsset(StaticAssetCookDescriptor descriptor, StaticMeshDefinition mesh)
    {
        Descriptor = descriptor ?? throw new ArgumentNullException(nameof(descriptor));
        Mesh = mesh ?? throw new ArgumentNullException(nameof(mesh));
    }

    public StaticAssetCookDescriptor Descriptor { get; }

    public StaticMeshDefinition Mesh { get; }
}
