using System.Runtime.InteropServices;
using ChronoFall.Box3D.Bindings.Interop;

namespace ChronoFall.Box3D.Tests;

public sealed class RuntimeResolutionTests
{
    [Fact]
    public void SupportedRuntimePathsAreExplicit()
    {
        Assert.EndsWith("runtimes/osx-arm64/native/libbox3d.dylib", Box3DBindingRuntime.GetExpectedPath("osx-arm64"), StringComparison.Ordinal);
        Assert.EndsWith("runtimes/linux-x64/native/libbox3d.so", Box3DBindingRuntime.GetExpectedPath("linux-x64"), StringComparison.Ordinal);
    }

    [Fact]
    public void UnsupportedRuntimeFailsExplicitly()
    {
        Assert.Throws<PlatformNotSupportedException>(() => NativeLibraryResolver.GetCurrentRuntimeIdentifier(false, false, Architecture.X64));
        Assert.Throws<DllNotFoundException>(() => Box3DBindingRuntime.GetExpectedPath("win-x64"));
    }
}
