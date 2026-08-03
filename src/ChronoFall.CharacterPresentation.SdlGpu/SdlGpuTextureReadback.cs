using SDL;
using static SDL.SDL3;

namespace ChronoFall.CharacterPresentation.SdlGpu;

public static unsafe class SdlGpuTextureReadback
{
    public static SdlGpuReadbackRequest Submit(
        SDL_GPUDevice* device,
        SDL_GPUCommandBuffer* completedRenderCommand,
        SDL_GPUTexture* texture,
        int width,
        int height,
        SDL_GPUTextureFormat format)
    {
        if (device is null)
            throw new ArgumentNullException(nameof(device));
        if (completedRenderCommand is null)
            throw new ArgumentNullException(nameof(completedRenderCommand));
        if (texture is null)
            throw new ArgumentNullException(nameof(texture));
        if (!RgbaImage.IsSupportedGpuFormat(format))
            throw new NotSupportedException($"GPU readback format {format} is not supported.");

        int byteCount = RgbaImage.GetByteCount(width, height);
        var transferInfo = new SDL_GPUTransferBufferCreateInfo
        {
            usage = SDL_GPUTransferBufferUsage.SDL_GPU_TRANSFERBUFFERUSAGE_DOWNLOAD,
            size = checked((uint)byteCount),
        };
        SDL_GPUTransferBuffer* transfer = SDL_CreateGPUTransferBuffer(device, &transferInfo);
        if (transfer is null)
        {
            _ = SDL_CancelGPUCommandBuffer(completedRenderCommand);
            throw new InvalidOperationException($"SDL GPU readback transfer creation failed: {SDL_GetError()}");
        }

        bool submissionAttempted = false;
        try
        {
            SDL_GPUCopyPass* copy = SDL_BeginGPUCopyPass(completedRenderCommand);
            if (copy is null)
                throw new InvalidOperationException($"SDL GPU readback copy pass failed: {SDL_GetError()}");

            var source = new SDL_GPUTextureRegion
            {
                texture = texture,
                w = checked((uint)width),
                h = checked((uint)height),
                d = 1,
            };
            var destination = new SDL_GPUTextureTransferInfo
            {
                transfer_buffer = transfer,
                pixels_per_row = checked((uint)width),
                rows_per_layer = checked((uint)height),
            };
            SDL_DownloadFromGPUTexture(copy, &source, &destination);
            SDL_EndGPUCopyPass(copy);

            submissionAttempted = true;
            SDL_GPUFence* fence = SDL_SubmitGPUCommandBufferAndAcquireFence(completedRenderCommand);
            if (fence is null)
                throw new InvalidOperationException($"SDL GPU readback submission failed: {SDL_GetError()}");

            return new SdlGpuReadbackRequest(device, fence, transfer, byteCount, width, height, format);
        }
        catch
        {
            if (!submissionAttempted)
                _ = SDL_CancelGPUCommandBuffer(completedRenderCommand);
            SDL_ReleaseGPUTransferBuffer(device, transfer);
            throw;
        }
    }
}

public sealed unsafe class SdlGpuReadbackRequest : IDisposable
{
    private SDL_GPUDevice* device;
    private SDL_GPUFence* fence;
    private SDL_GPUTransferBuffer* transfer;
    private readonly int byteCount;
    private readonly SDL_GPUTextureFormat format;

    internal SdlGpuReadbackRequest(
        SDL_GPUDevice* device,
        SDL_GPUFence* fence,
        SDL_GPUTransferBuffer* transfer,
        int byteCount,
        int width,
        int height,
        SDL_GPUTextureFormat format)
    {
        this.device = device;
        this.fence = fence;
        this.transfer = transfer;
        this.byteCount = byteCount;
        this.format = format;
        Width = width;
        Height = height;
    }

    public int Width { get; }

    public int Height { get; }

    public bool TryComplete(out RgbaImage? image)
    {
        ThrowIfDisposed();
        if (!SDL_QueryGPUFence(device, fence))
        {
            image = null;
            return false;
        }

        try
        {
            WaitForFence();
            image = MapImage();
            return true;
        }
        finally
        {
            Dispose();
        }
    }

    public RgbaImage Wait()
    {
        ThrowIfDisposed();
        try
        {
            WaitForFence();
            return MapImage();
        }
        finally
        {
            Dispose();
        }
    }

    public void Dispose()
    {
        if (device is null)
            return;
        if (fence is not null)
            SDL_ReleaseGPUFence(device, fence);
        if (transfer is not null)
            SDL_ReleaseGPUTransferBuffer(device, transfer);
        fence = null;
        transfer = null;
        device = null;
    }

    private RgbaImage MapImage()
    {
        IntPtr mapped = SDL_MapGPUTransferBuffer(device, transfer, cycle: false);
        if (mapped == IntPtr.Zero)
            throw new InvalidOperationException($"SDL GPU readback mapping failed: {SDL_GetError()}");
        try
        {
            return RgbaImage.FromGpuPixels(
                Width,
                Height,
                new ReadOnlySpan<byte>((void*)mapped, byteCount),
                format);
        }
        finally
        {
            SDL_UnmapGPUTransferBuffer(device, transfer);
        }
    }

    private void WaitForFence()
    {
        SDL_GPUFence* value = fence;
        if (!SDL_WaitForGPUFences(device, wait_all: true, &value, 1))
            throw new InvalidOperationException($"SDL GPU readback fence wait failed: {SDL_GetError()}");
    }

    private void ThrowIfDisposed()
    {
        if (device is null)
            throw new ObjectDisposedException(nameof(SdlGpuReadbackRequest));
    }
}
