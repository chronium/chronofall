using System.Diagnostics;
using System.Numerics;
using System.Reflection;
using System.Runtime.InteropServices;
using ChronoFall.EditorUi.SdlGpu;
using Evergine.Bindings.Imgui;
using Evergine.Bindings.Imguizmo;
using SDL;
using ImGuiVector2 = Evergine.Mathematics.Vector2;
using static SDL.SDL3;

namespace ChronoFall.EditorUi.GpuHarness;

internal static unsafe class Program
{
    private const int InitialWidth = 1280;
    private const int InitialHeight = 800;

    public static int Main()
    {
        ConfigureSdlResolver();
        if (!SDL_Init(SDL_InitFlags.SDL_INIT_VIDEO))
            throw new InvalidOperationException($"SDL video initialization failed: {SDL_GetError()}");

        SDL_Window* window = null;
        SDL_GPUDevice* device = null;
        bool windowClaimed = false;
        try
        {
            SDL_WindowFlags flags = (SDL_WindowFlags)(SDL_WINDOW_RESIZABLE | SDL_WINDOW_HIGH_PIXEL_DENSITY);
            window = SDL_CreateWindow(
                "ChronoFall - caller-controlled ImGui backend proof",
                InitialWidth,
                InitialHeight,
                flags);
            if (window is null)
                throw new InvalidOperationException($"SDL window creation failed: {SDL_GetError()}");

            const SDL_GPUShaderFormat requested =
                SDL_GPUShaderFormat.SDL_GPU_SHADERFORMAT_MSL |
                SDL_GPUShaderFormat.SDL_GPU_SHADERFORMAT_SPIRV;
            device = SDL_CreateGPUDevice(requested, debug_mode: true, name: (byte*)null);
            if (device is null)
                throw new InvalidOperationException($"SDL GPU device creation failed: {SDL_GetError()}");
            if (!SDL_ClaimWindowForGPUDevice(device, window))
                throw new InvalidOperationException($"SDL GPU window claim failed: {SDL_GetError()}");
            windowClaimed = true;

            bool fontHookCalled = false;
            using SdlGpuImGuiBackend backend = SdlGpuImGuiBackend.Create(
                window,
                device,
                SDL_GetGPUSwapchainTextureFormat(device, window),
                options: new SdlGpuImGuiBackendOptions(
                    EnableDocking: true,
                    ConfigureFonts: atlas =>
                    {
                        if (atlas is null)
                            throw new InvalidOperationException("The ImGui font atlas hook received a null atlas.");
                        _ = ImguiNative.ImFontAtlas_AddFontDefault(atlas, null);
                        fontHookCalled = true;
                    }));
            if (!fontHookCalled)
                throw new InvalidOperationException("The ImGui font injection hook was not invoked.");

            Run(window, device, backend);
            _ = SDL_WaitForGPUIdle(device);
            return 0;
        }
        finally
        {
            if (device is not null)
            {
                _ = SDL_WaitForGPUIdle(device);
                if (windowClaimed && window is not null)
                    SDL_ReleaseWindowFromGPUDevice(device, window);
                SDL_DestroyGPUDevice(device);
            }
            if (window is not null)
                SDL_DestroyWindow(window);
            SDL_Quit();
        }
    }

    private static void Run(SDL_Window* window, SDL_GPUDevice* device, SdlGpuImGuiBackend backend)
    {
        bool running = true;
        byte checkbox = 1;
        float value = 0.45f;
        Matrix4x4 transform = Matrix4x4.Identity;
        var stopwatch = Stopwatch.StartNew();
        double previousSeconds = stopwatch.Elapsed.TotalSeconds;

        while (running)
        {
            SDL_Event sdlEvent;
            while (SDL_PollEvent(&sdlEvent))
            {
                _ = backend.ProcessEvent(&sdlEvent);
                if (sdlEvent.Type is SDL_EventType.SDL_EVENT_QUIT or SDL_EventType.SDL_EVENT_WINDOW_CLOSE_REQUESTED ||
                    sdlEvent.Type == SDL_EventType.SDL_EVENT_KEY_DOWN && sdlEvent.key.key == SDL_Keycode.SDLK_ESCAPE)
                    running = false;
            }
            if (!running)
                break;

            int logicalWidth;
            int logicalHeight;
            int pixelWidth;
            int pixelHeight;
            if (!SDL_GetWindowSize(window, &logicalWidth, &logicalHeight) ||
                !SDL_GetWindowSizeInPixels(window, &pixelWidth, &pixelHeight) ||
                logicalWidth <= 0 || logicalHeight <= 0 || pixelWidth <= 0 || pixelHeight <= 0)
            {
                SDL_Delay(16);
                continue;
            }

            double nowSeconds = stopwatch.Elapsed.TotalSeconds;
            double deltaSeconds = Math.Max(nowSeconds - previousSeconds, 1.0 / 1000.0);
            previousSeconds = nowSeconds;
            backend.BeginFrame(new SdlGpuImGuiFrameMetrics(
                logicalWidth,
                logicalHeight,
                pixelWidth,
                pixelHeight,
                deltaSeconds));
            DrawNeutralProof(backend, ref checkbox, ref value, ref transform);

            SDL_GPUCommandBuffer* command = SDL_AcquireGPUCommandBuffer(device);
            if (command is null)
            {
                backend.EndFrameWithoutRendering();
                throw new InvalidOperationException($"SDL GPU command acquisition failed: {SDL_GetError()}");
            }

            bool swapchainAcquired = false;
            bool prepared = false;
            try
            {
                SDL_GPUTexture* swapchain;
                uint swapchainWidth;
                uint swapchainHeight;
                if (!SDL_WaitAndAcquireGPUSwapchainTexture(
                        command,
                        window,
                        &swapchain,
                        &swapchainWidth,
                        &swapchainHeight))
                    throw new InvalidOperationException($"SDL GPU swapchain acquisition failed: {SDL_GetError()}");
                swapchainAcquired = true;

                if (swapchain is null)
                {
                    backend.EndFrameWithoutRendering();
                }
                else
                {
                    backend.PrepareDrawData(command);
                    prepared = true;
                    var target = new SDL_GPUColorTargetInfo
                    {
                        texture = swapchain,
                        clear_color = new SDL_FColor { r = 0.035f, g = 0.04f, b = 0.05f, a = 1.0f },
                        load_op = SDL_GPULoadOp.SDL_GPU_LOADOP_CLEAR,
                        store_op = SDL_GPUStoreOp.SDL_GPU_STOREOP_STORE,
                    };
                    SDL_GPURenderPass* renderPass = SDL_BeginGPURenderPass(command, &target, 1, null);
                    if (renderPass is null)
                        throw new InvalidOperationException($"SDL GPU render pass creation failed: {SDL_GetError()}");
                    backend.RecordDrawData(command, renderPass);
                    prepared = false;
                    SDL_EndGPURenderPass(renderPass);
                }

                if (!SDL_SubmitGPUCommandBuffer(command))
                    throw new InvalidOperationException($"SDL GPU command submission failed: {SDL_GetError()}");
                command = null;
            }
            catch
            {
                if (prepared)
                    backend.DiscardPreparedDrawData();
                if (command is not null)
                {
                    if (swapchainAcquired)
                        _ = SDL_SubmitGPUCommandBuffer(command);
                    else
                        _ = SDL_CancelGPUCommandBuffer(command);
                    command = null;
                }
                throw;
            }
            finally
            {
                if (command is not null)
                    _ = SDL_CancelGPUCommandBuffer(command);
            }

            SDL_Delay(1);
        }
    }

    private static void DrawNeutralProof(
        SdlGpuImGuiBackend backend,
        ref byte checkbox,
        ref float value,
        ref Matrix4x4 transform)
    {
        ImguizmoNative.ImGuizmo_BeginFrame();
        ImguiNative.igSetNextWindowSize(new ImGuiVector2(760, 560), ImGuiCond.FirstUseEver);
        if (ImguiNative.igBegin("Neutral backend proof - not Starfall UI", null, ImGuiWindowFlags.None))
        {
            ImguiNative.igText("ChronoFall owns context and SDL GPU draw recording; this harness owns the pass.");
            ImguiNative.igText($"Framebuffer scale: {backend.FramebufferScale.X:0.00} x {backend.FramebufferScale.Y:0.00}");
            fixed (byte* checkboxPointer = &checkbox)
                _ = ImguiNative.igCheckbox("Interactive checkbox", checkboxPointer);
            fixed (float* valuePointer = &value)
                _ = ImguiNative.igSliderFloat("Interactive value", valuePointer, 0, 1, "%.2f", ImGuiSliderFlags.None);

            ImGuiVector2 position = ImguiNative.igGetWindowPos();
            ImGuiVector2 size = ImguiNative.igGetWindowSize();
            ImguizmoNative.ImGuizmo_SetDrawlist(ImguiNative.igGetWindowDrawList());
            ImguizmoNative.ImGuizmo_SetRect(position.X, position.Y + 115, size.X, Math.Max(size.Y - 115, 1));

            Matrix4x4 view = Matrix4x4.CreateLookAt(new Vector3(4, 3, 4), Vector3.Zero, Vector3.UnitY);
            Matrix4x4 projection = Matrix4x4.CreatePerspectiveFieldOfView(
                MathF.PI / 4,
                Math.Max(size.X / Math.Max(size.Y - 115, 1), 0.1f),
                0.1f,
                100);
            float* viewPointer = &view.M11;
            float* projectionPointer = &projection.M11;
            fixed (float* transformPointer = &transform.M11)
            {
                _ = ImguizmoNative.ImGuizmo_Manipulate(
                    viewPointer,
                    projectionPointer,
                    OPERATION.TRANSLATE,
                    MODE.WORLD,
                    transformPointer,
                    null,
                    null,
                    null,
                    null);
            }
        }
        ImguiNative.igEnd();
    }

    private static void ConfigureSdlResolver() =>
        NativeLibrary.SetDllImportResolver(typeof(SDL3).Assembly, ResolveSdl);

    private static IntPtr ResolveSdl(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
    {
        if (libraryName != "SDL3")
            return IntPtr.Zero;
        string path = Path.Combine(
            AppContext.BaseDirectory,
            "runtimes",
            "osx-arm64",
            "native",
            "libSDL3.dylib");
        if (!File.Exists(path))
            throw new DllNotFoundException($"SDL3 native library is missing. Expected path: {path}");
        return NativeLibrary.Load(path);
    }
}
