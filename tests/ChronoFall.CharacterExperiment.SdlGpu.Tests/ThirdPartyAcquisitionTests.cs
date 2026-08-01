namespace ChronoFall.CharacterExperiment.SdlGpu.Tests;

public sealed class ThirdPartyAcquisitionTests
{
    [Fact]
    public void SdlDependencyPinAndLicenseEvidenceAreCommitted()
    {
        string root = FindRepositoryRoot();
        string versions = File.ReadAllText(Path.Combine(root, "thirdparty", "versions.env"));
        Assert.Contains("SDL3_CS_COMMIT=\"a0a5276a874c0c48db705696ab7e2adc8b5db0a1\"", versions, StringComparison.Ordinal);
        Assert.Contains("SDL3_CS_OSX_ARM64_SHA256=\"35797abd1dc9e130f8e7ca8aeee33d68f8eecbf0af479184913297aaad4760ca\"", versions, StringComparison.Ordinal);
        Assert.True(File.Exists(Path.Combine(root, "thirdparty", "licenses", "SDL3-CS", "LICENCE")));
        Assert.True(File.Exists(Path.Combine(root, "thirdparty", "licenses", "SDL3-CS", "SDL-license-header.txt")));
        Assert.True(File.Exists(Path.Combine(root, "thirdparty", "fetch-sdl3-cs.sh")));
        Assert.True(File.Exists(Path.Combine(root, "thirdparty", "verify-sdl3-cs.sh")));
        Assert.True(File.Exists(Path.Combine(root, "thirdparty", "patches", "SDL3-CS", "0001-disable-android-target-for-coordinator.patch")));
    }

    private static string FindRepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "ChronoFall.slnx")))
                return directory.FullName;
            directory = directory.Parent;
        }
        throw new DirectoryNotFoundException("Could not find ChronoFall.slnx from the test output directory.");
    }
}
