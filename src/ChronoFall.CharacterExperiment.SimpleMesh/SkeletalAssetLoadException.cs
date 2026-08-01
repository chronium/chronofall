namespace ChronoFall.CharacterExperiment.SimpleMesh;

public sealed class SkeletalAssetLoadException : IOException
{
    public SkeletalAssetLoadException(
        string source,
        string reason,
        string? clipName = null,
        string? targetNode = null,
        string? channelPath = null,
        Exception? innerException = null)
        : base(FormatMessage(source, reason, clipName, targetNode, channelPath), innerException)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(source);
        ArgumentException.ThrowIfNullOrWhiteSpace(reason);

        SourcePath = source;
        Reason = reason;
        ClipName = clipName;
        TargetNode = targetNode;
        ChannelPath = channelPath;
    }

    public string SourcePath { get; }

    public string Reason { get; }

    public string? ClipName { get; }

    public string? TargetNode { get; }

    public string? ChannelPath { get; }

    private static string FormatMessage(
        string source,
        string reason,
        string? clipName,
        string? targetNode,
        string? channelPath)
    {
        string context = $"Skeletal asset '{source}'";
        if (clipName is not null)
            context += $", clip '{clipName}'";
        if (targetNode is not null)
            context += $", target '{targetNode}'";
        if (channelPath is not null)
            context += $", path '{channelPath}'";
        return $"{context}: {reason}";
    }
}
