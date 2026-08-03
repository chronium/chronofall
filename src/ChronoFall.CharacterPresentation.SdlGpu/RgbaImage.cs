using SDL;

namespace ChronoFall.CharacterPresentation.SdlGpu;

public sealed class RgbaImage
{
    private const int BytesPerPixel = 4;
    private readonly byte[] pixels;

    public RgbaImage(int width, int height, ReadOnlySpan<byte> pixels)
    {
        int expectedByteCount = GetByteCount(width, height);
        if (pixels.Length != expectedByteCount)
        {
            throw new ArgumentException(
                $"RGBA pixel buffer must contain exactly {expectedByteCount} bytes.",
                nameof(pixels));
        }

        Width = width;
        Height = height;
        this.pixels = pixels.ToArray();
    }

    public int Width { get; }

    public int Height { get; }

    public ReadOnlyMemory<byte> Pixels => pixels;

    public static RgbaImage FromGpuPixels(
        int width,
        int height,
        ReadOnlySpan<byte> pixels,
        SDL_GPUTextureFormat format)
    {
        int expectedByteCount = GetByteCount(width, height);
        if (pixels.Length != expectedByteCount)
        {
            throw new ArgumentException(
                $"GPU pixel buffer must contain exactly {expectedByteCount} bytes.",
                nameof(pixels));
        }
        if (!IsSupportedGpuFormat(format))
        {
            throw new NotSupportedException(
                $"GPU readback format {format} is not a supported RGBA or BGRA 8-bit format.");
        }

        byte[] normalized = pixels.ToArray();
        if (format is SDL_GPUTextureFormat.SDL_GPU_TEXTUREFORMAT_B8G8R8A8_UNORM or
            SDL_GPUTextureFormat.SDL_GPU_TEXTUREFORMAT_B8G8R8A8_UNORM_SRGB)
        {
            for (int offset = 0; offset < normalized.Length; offset += BytesPerPixel)
                (normalized[offset], normalized[offset + 2]) = (normalized[offset + 2], normalized[offset]);
        }

        return new RgbaImage(width, height, normalized);
    }

    internal static int GetByteCount(int width, int height)
    {
        if (width < 1)
            throw new ArgumentOutOfRangeException(nameof(width), "Image width must be positive.");
        if (height < 1)
            throw new ArgumentOutOfRangeException(nameof(height), "Image height must be positive.");

        return checked(width * height * BytesPerPixel);
    }

    internal static bool IsSupportedGpuFormat(SDL_GPUTextureFormat format) =>
        format is SDL_GPUTextureFormat.SDL_GPU_TEXTUREFORMAT_R8G8B8A8_UNORM or
            SDL_GPUTextureFormat.SDL_GPU_TEXTUREFORMAT_R8G8B8A8_UNORM_SRGB or
            SDL_GPUTextureFormat.SDL_GPU_TEXTUREFORMAT_B8G8R8A8_UNORM or
            SDL_GPUTextureFormat.SDL_GPU_TEXTUREFORMAT_B8G8R8A8_UNORM_SRGB;
}
