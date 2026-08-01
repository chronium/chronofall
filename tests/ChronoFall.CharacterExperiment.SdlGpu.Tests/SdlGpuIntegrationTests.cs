using System.Diagnostics;

namespace ChronoFall.CharacterExperiment.SdlGpu.Tests;

public sealed class SdlGpuIntegrationTests
{
    [Fact]
    public async Task StandaloneHarnessRendersSelectedBindPoseWhenEnabled()
    {
        if (!string.Equals(Environment.GetEnvironmentVariable("CHRONOFALL_GPU_TESTS"), "1", StringComparison.Ordinal))
            return;

        string harnessPath = Path.Combine(AppContext.BaseDirectory, "gpu-harness", "ChronoFall.CharacterExperiment.GpuHarness.dll");
        Assert.True(File.Exists(harnessPath), $"GPU harness was not packaged at '{harnessPath}'.");
        using var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                ArgumentList = { harnessPath },
                WorkingDirectory = Path.GetDirectoryName(harnessPath)!,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            },
        };

        Assert.True(process.Start(), "GPU harness process did not start.");
        Task<string> stdoutTask = process.StandardOutput.ReadToEndAsync();
        Task<string> stderrTask = process.StandardError.ReadToEndAsync();
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(45));
        try
        {
            await process.WaitForExitAsync(timeout.Token);
        }
        catch (OperationCanceledException)
        {
            process.Kill(entireProcessTree: true);
            await process.WaitForExitAsync();
            Assert.Fail($"GPU harness exceeded 45 seconds.\nstdout:\n{await stdoutTask}\nstderr:\n{await stderrTask}");
        }

        string stdout = await stdoutTask;
        string stderr = await stderrTask;
        Assert.True(process.ExitCode == 0, $"GPU harness exited with code {process.ExitCode}.\nstdout:\n{stdout}\nstderr:\n{stderr}");
        Assert.Contains("GPU_HARNESS_PASS bind-pose", stdout, StringComparison.Ordinal);
        Assert.Contains("GPU_HARNESS_PASS palette-probe", stdout, StringComparison.Ordinal);
        Assert.Contains("GPU_HARNESS_SUCCESS", stdout, StringComparison.Ordinal);
    }
}
