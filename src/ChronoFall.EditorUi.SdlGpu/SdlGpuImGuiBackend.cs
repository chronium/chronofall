using System.Numerics;
using System.Runtime.InteropServices;
using Evergine.Bindings.Imgui;
using SDL;
using ImGuiVector2 = Evergine.Mathematics.Vector2;
using static SDL.SDL3;

namespace ChronoFall.EditorUi.SdlGpu;

public sealed unsafe class SdlGpuImGuiBackend : IDisposable
{
    private readonly int ownerThreadId;
    private readonly BackendFrameStateMachine frameState = new();
    private IntPtr context;
    private IntPtr iniPathStorage;
    private ImDrawData* drawData;
    private bool sdl3Initialized;
    private bool sdlGpuInitialized;

    private SdlGpuImGuiBackend() => ownerThreadId = Environment.CurrentManagedThreadId;

    public Vector2 FramebufferScale { get; private set; } = Vector2.One;

    public ImGuiCaptureState Capture
    {
        get
        {
            EnsureUsable();
            SetCurrentContext();
            ImGuiIO* io = RequireIo();
            return new ImGuiCaptureState(
                io->WantCaptureMouse != 0,
                io->WantCaptureKeyboard != 0,
                io->WantTextInput != 0);
        }
    }

    public static SdlGpuImGuiBackend Create(
        SDL_Window* window,
        SDL_GPUDevice* device,
        SDL_GPUTextureFormat colorTargetFormat,
        SDL_GPUSampleCount msaaSamples = SDL_GPUSampleCount.SDL_GPU_SAMPLECOUNT_1,
        SdlGpuImGuiBackendOptions options = default)
    {
        if (window is null)
            throw new ArgumentNullException(nameof(window));
        if (device is null)
            throw new ArgumentNullException(nameof(device));
        if (colorTargetFormat == SDL_GPUTextureFormat.SDL_GPU_TEXTUREFORMAT_INVALID)
            throw new ArgumentOutOfRangeException(nameof(colorTargetFormat), colorTargetFormat, "A valid SDL GPU color target format is required.");
        if (!Enum.IsDefined(msaaSamples))
            throw new ArgumentOutOfRangeException(nameof(msaaSamples), msaaSamples, "The SDL GPU sample count is not defined.");
        options.Validate();

        NativeLibraryResolver.ConfigureResolvers();
        var backend = new SdlGpuImGuiBackend();
        try
        {
            backend.context = ImguiNative.igCreateContext(null);
            if (backend.context == IntPtr.Zero)
                throw new InvalidOperationException("ImGui context creation failed.");

            backend.SetCurrentContext();
            ImGuiIO* io = RequireIo();
            io->IniFilename = null;
            io->ConfigFlags &= ~ImGuiConfigFlags.ViewportsEnable;
            if (options.EnableDocking)
                io->ConfigFlags |= ImGuiConfigFlags.DockingEnable;
            if (options.IniPath is not null)
            {
                backend.iniPathStorage = Marshal.StringToCoTaskMemUTF8(options.IniPath);
                io->IniFilename = (byte*)backend.iniPathStorage;
            }
            options.ConfigureFonts?.Invoke(io->Fonts);

            if (!NativeMethods.chronofall_imgui_sdl3_init_for_sdlgpu(window))
                throw new InvalidOperationException($"ImGui SDL3 platform backend initialization failed: {SDL_GetError()}");
            backend.sdl3Initialized = true;

            if (!NativeMethods.chronofall_imgui_sdlgpu3_init(device, (int)colorTargetFormat, (int)msaaSamples))
                throw new InvalidOperationException($"ImGui SDL GPU renderer backend initialization failed: {SDL_GetError()}");
            backend.sdlGpuInitialized = true;
            return backend;
        }
        catch
        {
            backend.DisposeCore();
            throw;
        }
    }

    public bool ProcessEvent(SDL_Event* sdlEvent)
    {
        if (sdlEvent is null)
            throw new ArgumentNullException(nameof(sdlEvent));
        EnsureUsable();
        SetCurrentContext();
        return NativeMethods.chronofall_imgui_sdl3_process_event(sdlEvent);
    }

    public void SetMouseInputEnabled(bool enabled)
    {
        EnsureUsable();
        SetCurrentContext();
        ImGuiIO* io = RequireIo();
        if (enabled)
            io->ConfigFlags &= ~ImGuiConfigFlags.NoMouse;
        else
            io->ConfigFlags |= ImGuiConfigFlags.NoMouse;
    }

    public void BeginFrame(SdlGpuImGuiFrameMetrics metrics)
    {
        EnsureUsable();
        frameState.EnsureCanBegin();
        SetCurrentContext();
        ImGuiIO* io = RequireIo();
        if ((io->ConfigFlags & ImGuiConfigFlags.ViewportsEnable) != 0)
            throw new NotSupportedException("ChronoFall's caller-controlled ImGui backend does not support secondary platform viewports.");

        FramebufferScale = metrics.FramebufferScale;
        io->DisplaySize = new ImGuiVector2(metrics.LogicalWidth, metrics.LogicalHeight);
        io->DisplayFramebufferScale = new ImGuiVector2(FramebufferScale.X, FramebufferScale.Y);
        io->DeltaTime = (float)metrics.DeltaSeconds;

        NativeMethods.chronofall_imgui_sdlgpu3_new_frame();
        NativeMethods.chronofall_imgui_sdl3_new_frame();
        ImguiNative.igNewFrame();
        frameState.Begin();
    }

    public void PrepareDrawData(SDL_GPUCommandBuffer* commandBuffer)
    {
        if (commandBuffer is null)
            throw new ArgumentNullException(nameof(commandBuffer));
        EnsureUsable();
        frameState.EnsureCanPrepare();
        SetCurrentContext();
        ImguiNative.igRender();
        drawData = ImguiNative.igGetDrawData();
        frameState.Prepare();
        if (drawData is not null)
            NativeMethods.chronofall_imgui_sdlgpu3_prepare_draw_data(drawData, commandBuffer);
    }

    public void RecordDrawData(SDL_GPUCommandBuffer* commandBuffer, SDL_GPURenderPass* renderPass)
    {
        if (commandBuffer is null)
            throw new ArgumentNullException(nameof(commandBuffer));
        if (renderPass is null)
            throw new ArgumentNullException(nameof(renderPass));
        EnsureUsable();
        frameState.EnsureCanCompletePrepared();
        SetCurrentContext();
        if (drawData is not null)
            NativeMethods.chronofall_imgui_sdlgpu3_render_draw_data(drawData, commandBuffer, renderPass);
        drawData = null;
        frameState.CompletePrepared();
    }

    public void DiscardPreparedDrawData()
    {
        EnsureUsable();
        frameState.EnsureCanCompletePrepared();
        drawData = null;
        frameState.CompletePrepared();
    }

    public void EndFrameWithoutRendering()
    {
        EnsureUsable();
        frameState.EnsureCanEndWithoutRendering();
        SetCurrentContext();
        ImguiNative.igEndFrame();
        frameState.EndWithoutRendering();
    }

    public void Dispose()
    {
        EnsureOwnerThread();
        DisposeCore();
        GC.SuppressFinalize(this);
    }

    private void DisposeCore()
    {
        if (context == IntPtr.Zero)
            return;

        SetCurrentContext();
        if (frameState.State == BackendFrameState.Building)
            ImguiNative.igEndFrame();
        if (sdlGpuInitialized)
        {
            NativeMethods.chronofall_imgui_sdlgpu3_shutdown();
            sdlGpuInitialized = false;
        }
        if (sdl3Initialized)
        {
            NativeMethods.chronofall_imgui_sdl3_shutdown();
            sdl3Initialized = false;
        }
        ImguiNative.igDestroyContext(context);
        context = IntPtr.Zero;
        drawData = null;
        if (iniPathStorage != IntPtr.Zero)
        {
            Marshal.FreeCoTaskMem(iniPathStorage);
            iniPathStorage = IntPtr.Zero;
        }
        frameState.Dispose();
    }

    private void EnsureUsable()
    {
        EnsureOwnerThread();
        if (context == IntPtr.Zero)
            throw new ObjectDisposedException(nameof(SdlGpuImGuiBackend));
    }

    private void EnsureOwnerThread()
    {
        if (Environment.CurrentManagedThreadId != ownerThreadId)
            throw new InvalidOperationException("The ImGui backend is thread-affine and must be used on its creating thread.");
    }

    private void SetCurrentContext() => ImguiNative.igSetCurrentContext(context);

    private static ImGuiIO* RequireIo()
    {
        ImGuiIO* io = ImguiNative.igGetIO_Nil();
        if (io is null)
            throw new InvalidOperationException("ImGui IO is unavailable for the current context.");
        return io;
    }
}
