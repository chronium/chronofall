using System.Runtime.InteropServices;

namespace ChronoFall.EditorUi.SdlGpu.Tests;

public sealed class NativeResolutionTests
{
    [Fact]
    public void BothManagedImportNamesResolveToTheOneCoordinatorArtifact()
    {
        string binding = NativeLibraryResolver.GetExpectedPath(
            NativeLibraryResolver.BindingImportName,
            NativeLibraryResolver.MacOSArm64Rid);
        string backend = NativeLibraryResolver.GetExpectedPath(
            NativeLibraryResolver.BackendImportName,
            NativeLibraryResolver.MacOSArm64Rid);

        Assert.Equal(binding, backend);
        Assert.EndsWith(
            "runtimes/osx-arm64/native/libchronofall_imgui.dylib",
            binding,
            StringComparison.Ordinal);
    }

    [Fact]
    public void UnsupportedPlatformsAndImportsFailExplicitly()
    {
        Assert.Throws<PlatformNotSupportedException>(() =>
            NativeLibraryResolver.GetCurrentRuntimeIdentifier(false, Architecture.X64));
        Assert.Throws<PlatformNotSupportedException>(() =>
            NativeLibraryResolver.GetCurrentRuntimeIdentifier(true, Architecture.X64));
        Assert.Throws<DllNotFoundException>(() =>
            NativeLibraryResolver.GetExpectedPath("unrelated", NativeLibraryResolver.MacOSArm64Rid));
    }
}
