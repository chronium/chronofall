using System.Diagnostics;
using System.Text;

namespace ChronoFall.CharacterExperiment.SdlGpu.Tests;

public sealed class SdlGpuIntegrationTests
{
    [Fact]
    public async Task StandaloneHarnessRendersSelectedLoopingAnimationWhenEnabled()
    {
        if (!string.Equals(Environment.GetEnvironmentVariable("CHRONOFALL_GPU_TESTS"), "1", StringComparison.Ordinal))
            return;

        string harnessPath = Path.Combine(AppContext.BaseDirectory, "gpu-harness", "ChronoFall.CharacterExperiment.GpuHarness.dll");
        Assert.True(File.Exists(harnessPath), $"GPU harness was not packaged at '{harnessPath}'.");
        using var captures = new TemporaryDirectory();
        using var blendCaptures = new TemporaryDirectory();
        using var layeredCaptures = new TemporaryDirectory();
        using var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                ArgumentList =
                {
                    harnessPath,
                    "--capture-suite",
                    captures.Path,
                    "--blend-capture-suite",
                    blendCaptures.Path,
                    "--layered-capture-suite",
                    layeredCaptures.Path,
                },
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
        Assert.Contains("GPU_HARNESS_PASS skeleton-debug", stdout, StringComparison.Ordinal);
        Assert.Contains("GPU_HARNESS_PASS animation clip=Walk_Loop sample=0.500", stdout, StringComparison.Ordinal);
        Assert.Contains("later-sample=1.000", stdout, StringComparison.Ordinal);
        Assert.Contains(
            "GPU_HARNESS_PASS blending locomotion=247702bbf7799ca9/620021052adb3084/a2b427aea339d460 " +
            "action=8d03eaf0fe5dd28e/b8ad9c8aa7d18175/771344f116121af7",
            stdout,
            StringComparison.Ordinal);
        Assert.Contains("fingerprint=408d3a4c16278bbc", stdout, StringComparison.Ordinal);
        Assert.Contains("fingerprint=4fd2e63aea97f7a3", stdout, StringComparison.Ordinal);
        Assert.Contains("fingerprint=c6ad39a45245afed", stdout, StringComparison.Ordinal);
        Assert.Contains("GPU_HARNESS_CAPTURE_SUITE", stdout, StringComparison.Ordinal);
        Assert.Contains("GPU_HARNESS_BLEND_CAPTURE_SUITE", stdout, StringComparison.Ordinal);
        Assert.Contains(
            "GPU_HARNESS_PASS layering mask=spine_01:53/65 " +
            "comparison=1b80c2c70e8e2d89/b8ad9c8aa7d18175/902ee9ea51c7bb1f " +
            "transition=c987968d560c8090/e34e7058c81a532e/85c5d42b4eac399d",
            stdout,
            StringComparison.Ordinal);
        Assert.Contains("GPU_HARNESS_LAYERED_CAPTURE_SUITE", stdout, StringComparison.Ordinal);
        Assert.Contains("GPU_HARNESS_SUCCESS", stdout, StringComparison.Ordinal);

        string[] expectedFiles =
        [
            "animation-0000ms.ppm",
            "animation-0500ms.ppm",
            "animation-1000ms.ppm",
            "animation-loop-boundary.ppm",
            "bind-pose.ppm",
        ];
        string[] actualFiles = Directory.GetFiles(captures.Path)
            .Select(static path => Path.GetFileName(path)!)
            .Order(StringComparer.Ordinal)
            .ToArray();
        Assert.Equal(expectedFiles, actualFiles);
        Assert.All(expectedFiles, file => AssertPpm(Path.Combine(captures.Path, file)));

        string[] expectedBlendFiles =
        [
            "blend-action-body.ppm",
            "blend-action-entry.ppm",
            "blend-action-return.ppm",
            "blend-locomotion-idle.ppm",
            "blend-locomotion-midpoint.ppm",
            "blend-locomotion-walk.ppm",
        ];
        string[] actualBlendFiles = Directory.GetFiles(blendCaptures.Path)
            .Select(static path => Path.GetFileName(path)!)
            .Order(StringComparer.Ordinal)
            .ToArray();
        Assert.Equal(expectedBlendFiles, actualBlendFiles);
        Assert.All(expectedBlendFiles, file => AssertPpm(Path.Combine(blendCaptures.Path, file)));

        string[] expectedLayeredFiles =
        [
            "layer-action-entry.ppm",
            "layer-action-return.ppm",
            "layer-full-action.ppm",
            "layer-upper-action.ppm",
            "layer-walk-advanced.ppm",
            "layer-walk-base.ppm",
        ];
        string[] actualLayeredFiles = Directory.GetFiles(layeredCaptures.Path)
            .Select(static path => Path.GetFileName(path)!)
            .Order(StringComparer.Ordinal)
            .ToArray();
        Assert.Equal(expectedLayeredFiles, actualLayeredFiles);
        Assert.All(expectedLayeredFiles, file => AssertPpm(Path.Combine(layeredCaptures.Path, file)));
    }

    private static void AssertPpm(string path)
    {
        byte[] header = Encoding.ASCII.GetBytes("P6\n512 512\n255\n");
        byte[] contents = File.ReadAllBytes(path);
        Assert.Equal(header.Length + 512 * 512 * 3, contents.Length);
        Assert.True(contents.AsSpan().StartsWith(header), $"Capture '{path}' did not have the expected PPM header.");
    }

    private sealed class TemporaryDirectory : IDisposable
    {
        internal TemporaryDirectory()
        {
            Path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"chronofall-captures-{Guid.NewGuid():N}");
        }

        internal string Path { get; }

        public void Dispose()
        {
            if (Directory.Exists(Path))
                Directory.Delete(Path, recursive: true);
        }
    }
}
