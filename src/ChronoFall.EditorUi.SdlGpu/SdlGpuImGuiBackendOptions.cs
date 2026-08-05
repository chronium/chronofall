using Evergine.Bindings.Imgui;

namespace ChronoFall.EditorUi.SdlGpu;

public unsafe delegate void ImGuiFontAtlasConfigurator(ImFontAtlas* fontAtlas);

public readonly record struct SdlGpuImGuiBackendOptions(
    bool EnableDocking = false,
    string? IniPath = null,
    ImGuiFontAtlasConfigurator? ConfigureFonts = null)
{
    internal void Validate()
    {
        if (IniPath is not null && string.IsNullOrWhiteSpace(IniPath))
            throw new ArgumentException("The ImGui ini path must be null or a non-empty path.", nameof(IniPath));
    }
}
