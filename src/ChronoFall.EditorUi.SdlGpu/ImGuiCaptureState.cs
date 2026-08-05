namespace ChronoFall.EditorUi.SdlGpu;

public readonly record struct ImGuiCaptureState(
    bool WantsMouse,
    bool WantsKeyboard,
    bool WantsTextInput);
