using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.InteropServices;

namespace ChronoFall.Box3D.Bindings.Interop;

internal static class NativeLibraryResolver
{
    internal const string MacOSArm64Rid = "osx-arm64";
    internal const string LinuxX64Rid = "linux-x64";
    private static readonly object Gate = new();
    private static bool configured;

    internal static string CurrentRuntimeIdentifier => GetCurrentRuntimeIdentifier(
        OperatingSystem.IsMacOS(),
        OperatingSystem.IsLinux(),
        RuntimeInformation.ProcessArchitecture);

    internal static string GetCurrentRuntimeIdentifier(bool isMacOS, bool isLinux, Architecture architecture) =>
        (isMacOS, isLinux, architecture) switch
        {
            (true, false, Architecture.Arm64) => MacOSArm64Rid,
            (false, true, Architecture.X64) => LinuxX64Rid,
            _ => throw new PlatformNotSupportedException(
                $"ChronoFall Box3D supports only macOS ARM64 and Linux x64; current platform is {RuntimeInformation.OSDescription} {architecture}.")
        };

    internal static string GetExpectedPath(string importName, string runtimeIdentifier) =>
        Path.Combine(AppContext.BaseDirectory, "runtimes", runtimeIdentifier, "native", GetNativeFileName(importName, runtimeIdentifier));

    internal static void ConfigureForAssembly(Assembly assembly)
    {
        lock (Gate)
        {
            if (configured)
                return;
            NativeLibrary.SetDllImportResolver(assembly, Resolve);
            configured = true;
        }
    }

    private static IntPtr Resolve(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
    {
        string rid = CurrentRuntimeIdentifier;
        string path = GetExpectedPath(libraryName, rid);
        if (!File.Exists(path))
            throw new DllNotFoundException($"Native library '{libraryName}' is missing for RID '{rid}'. Expected path: {path}");
        return NativeLibrary.Load(path);
    }

    private static string GetNativeFileName(string importName, string rid) => (rid, importName) switch
    {
        (MacOSArm64Rid, Box3DBindingSurface.NativeLibraryName) => "libbox3d.dylib",
        (LinuxX64Rid, Box3DBindingSurface.NativeLibraryName) => "libbox3d.so",
        _ => throw new DllNotFoundException($"Native library '{importName}' has no resolver mapping for RID '{rid}'.")
    };
}
