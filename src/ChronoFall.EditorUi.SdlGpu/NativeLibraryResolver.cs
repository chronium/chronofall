using System.Reflection;
using System.Runtime.InteropServices;
using Evergine.Bindings.Imgui;

namespace ChronoFall.EditorUi.SdlGpu;

internal static class NativeLibraryResolver
{
    internal const string MacOSArm64Rid = "osx-arm64";
    internal const string BindingImportName = "cimgui";
    internal const string BackendImportName = "chronofall_imgui";
    private static readonly object Gate = new();
    private static readonly HashSet<Assembly> ConfiguredAssemblies = [];

    internal static string CurrentRuntimeIdentifier => GetCurrentRuntimeIdentifier(
        OperatingSystem.IsMacOS(),
        RuntimeInformation.ProcessArchitecture);

    internal static string GetCurrentRuntimeIdentifier(bool isMacOS, Architecture architecture) =>
        (isMacOS, architecture) switch
        {
            (true, Architecture.Arm64) => MacOSArm64Rid,
            _ => throw new PlatformNotSupportedException(
                $"ChronoFall SDL GPU ImGui currently supports only macOS ARM64; current platform is {RuntimeInformation.OSDescription} {architecture}."),
        };

    internal static string GetExpectedPath(string importName, string runtimeIdentifier)
    {
        if (runtimeIdentifier != MacOSArm64Rid ||
            (importName != BindingImportName && importName != BackendImportName))
            throw new DllNotFoundException(
                $"Native library '{importName}' has no ChronoFall ImGui mapping for RID '{runtimeIdentifier}'.");

        return Path.Combine(
            AppContext.BaseDirectory,
            "runtimes",
            runtimeIdentifier,
            "native",
            "libchronofall_imgui.dylib");
    }

    internal static void ConfigureResolvers()
    {
        ConfigureForAssembly(typeof(ImguiNative).Assembly);
        ConfigureForAssembly(typeof(SdlGpuImGuiBackend).Assembly);
    }

    private static void ConfigureForAssembly(Assembly assembly)
    {
        lock (Gate)
        {
            if (!ConfiguredAssemblies.Add(assembly))
                return;
            NativeLibrary.SetDllImportResolver(assembly, Resolve);
        }
    }

    private static IntPtr Resolve(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
    {
        if (libraryName != BindingImportName && libraryName != BackendImportName)
            return IntPtr.Zero;

        string rid = CurrentRuntimeIdentifier;
        string path = GetExpectedPath(libraryName, rid);
        if (!File.Exists(path))
            throw new DllNotFoundException(
                $"ChronoFall ImGui native library '{libraryName}' is missing for RID '{rid}'. Expected path: {path}");
        return NativeLibrary.Load(path);
    }
}
