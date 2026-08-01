using System.Diagnostics;

namespace ChronoFall.CharacterExperiment.SimpleMesh.Tests;

public sealed class ThirdPartyAcquisitionTests
{
    [Fact]
    public void PinnedCheckoutContainsTheReversiblyAppliedPatchAndLicense()
    {
        ProcessResult result = Run("sh", "thirdparty/verify-simplemesh.sh");

        Assert.True(result.ExitCode == 0, $"Verification failed.\nstdout:\n{result.StandardOutput}\nstderr:\n{result.StandardError}");
        string coreProject = File.ReadAllText(Path.Combine(
            RepositoryPaths.Root,
            "src",
            "ChronoFall.CharacterExperiment",
            "ChronoFall.CharacterExperiment.csproj"));
        Assert.DoesNotContain("SimpleMesh", coreProject, StringComparison.Ordinal);
    }

    private static ProcessResult Run(string fileName, string arguments)
    {
        var startInfo = new ProcessStartInfo(fileName, arguments)
        {
            WorkingDirectory = RepositoryPaths.Root,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
        };
        using Process process = Process.Start(startInfo) ?? throw new InvalidOperationException("Failed to start verification process.");
        string standardOutput = process.StandardOutput.ReadToEnd();
        string standardError = process.StandardError.ReadToEnd();
        process.WaitForExit();
        return new ProcessResult(process.ExitCode, standardOutput, standardError);
    }

    private sealed record ProcessResult(int ExitCode, string StandardOutput, string StandardError);
}
