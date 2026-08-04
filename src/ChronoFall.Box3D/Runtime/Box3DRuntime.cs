using ChronoFall.Box3D.Bindings.Interop;

namespace ChronoFall.Box3D.Runtime;

public static class Box3DRuntime
{
    public static string NativeLibraryName => Box3DBindingSurface.NativeLibraryName;
    public static string CurrentRuntimeIdentifier => Box3DBindingRuntime.CurrentRuntimeIdentifier;
    public static string ExpectedNativeLibraryPath => Box3DBindingRuntime.GetExpectedPath(CurrentRuntimeIdentifier);
}
