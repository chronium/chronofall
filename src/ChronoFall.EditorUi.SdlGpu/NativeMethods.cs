using System.Runtime.InteropServices;
using Evergine.Bindings.Imgui;
using SDL;

namespace ChronoFall.EditorUi.SdlGpu;

internal static unsafe class NativeMethods
{
    [DllImport(NativeLibraryResolver.BackendImportName, CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.I1)]
    internal static extern bool chronofall_imgui_sdl3_init_for_sdlgpu(SDL_Window* window);

    [DllImport(NativeLibraryResolver.BackendImportName, CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.I1)]
    internal static extern bool chronofall_imgui_sdl3_process_event(SDL_Event* sdlEvent);

    [DllImport(NativeLibraryResolver.BackendImportName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void chronofall_imgui_sdl3_new_frame();

    [DllImport(NativeLibraryResolver.BackendImportName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void chronofall_imgui_sdl3_shutdown();

    [DllImport(NativeLibraryResolver.BackendImportName, CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.I1)]
    internal static extern bool chronofall_imgui_sdlgpu3_init(
        SDL_GPUDevice* device,
        int colorTargetFormat,
        int msaaSamples);

    [DllImport(NativeLibraryResolver.BackendImportName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void chronofall_imgui_sdlgpu3_new_frame();

    [DllImport(NativeLibraryResolver.BackendImportName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void chronofall_imgui_sdlgpu3_shutdown();

    [DllImport(NativeLibraryResolver.BackendImportName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void chronofall_imgui_sdlgpu3_prepare_draw_data(
        ImDrawData* drawData,
        SDL_GPUCommandBuffer* commandBuffer);

    [DllImport(NativeLibraryResolver.BackendImportName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void chronofall_imgui_sdlgpu3_render_draw_data(
        ImDrawData* drawData,
        SDL_GPUCommandBuffer* commandBuffer,
        SDL_GPURenderPass* renderPass);
}
