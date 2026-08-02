namespace ChronoFall.CharacterExperiment.SimpleMesh;

public sealed class StaticAssetLoadException : IOException
{
    public StaticAssetLoadException(string source, string reason, Exception? innerException = null)
        : base($"Static asset '{source}': {reason}", innerException)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(source);
        ArgumentException.ThrowIfNullOrWhiteSpace(reason);
        SourcePath = source;
        Reason = reason;
    }

    public string SourcePath { get; }

    public string Reason { get; }
}
