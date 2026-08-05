using System.Runtime.InteropServices;

namespace ChronoFall.EditorUi.SdlGpu.Tests;

public sealed class NativeArtifactTests
{
    [Fact]
    public void MacOSArm64ArtifactExportsTheApprovedNativeSurface()
    {
        if (!OperatingSystem.IsMacOS() || RuntimeInformation.ProcessArchitecture != Architecture.Arm64)
            return;

        string path = Path.Combine(
            AppContext.BaseDirectory,
            "runtimes",
            "osx-arm64",
            "native",
            "libchronofall_imgui.dylib");
        Assert.True(File.Exists(path), path);

        IntPtr library = NativeLibrary.Load(path);
        try
        {
            string[] exports =
            [
                "igCreateContext",
                "ImGuizmo_BeginFrame",
                "chronofall_imgui_sdl3_init_for_sdlgpu",
                "chronofall_imgui_sdlgpu3_prepare_draw_data",
                "chronofall_imgui_sdlgpu3_render_draw_data",
            ];

            Assert.All(exports, export =>
                Assert.True(NativeLibrary.TryGetExport(library, export, out _), export));
        }
        finally
        {
            NativeLibrary.Free(library);
        }
    }
}
