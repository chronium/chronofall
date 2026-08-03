using System.Buffers.Binary;
using SDL;

namespace ChronoFall.CharacterPresentation.SdlGpu.Tests;

public sealed class ScreenshotContractTests
{
    private static readonly SDL_GPUTextureFormat[] RgbaFormats =
    [
        SDL_GPUTextureFormat.SDL_GPU_TEXTUREFORMAT_R8G8B8A8_UNORM,
        SDL_GPUTextureFormat.SDL_GPU_TEXTUREFORMAT_R8G8B8A8_UNORM_SRGB,
    ];

    private static readonly SDL_GPUTextureFormat[] BgraFormats =
    [
        SDL_GPUTextureFormat.SDL_GPU_TEXTUREFORMAT_B8G8R8A8_UNORM,
        SDL_GPUTextureFormat.SDL_GPU_TEXTUREFORMAT_B8G8R8A8_UNORM_SRGB,
    ];

    [Fact]
    public void RgbaImageRejectsInvalidDimensionsAndPixelCounts()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new RgbaImage(0, 1, []));
        Assert.Throws<ArgumentOutOfRangeException>(() => new RgbaImage(1, 0, []));
        Assert.Throws<OverflowException>(() => new RgbaImage(int.MaxValue, 2, []));
        Assert.Throws<ArgumentException>(() => new RgbaImage(1, 1, [1, 2, 3]));
        Assert.Throws<ArgumentException>(() => RgbaImage.FromGpuPixels(
            1,
            1,
            [1, 2, 3],
            SDL_GPUTextureFormat.SDL_GPU_TEXTUREFORMAT_B8G8R8A8_UNORM));
    }

    [Fact]
    public void RgbaImageOwnsACopyOfCallerPixels()
    {
        byte[] source = [1, 2, 3, 4];

        var image = new RgbaImage(1, 1, source);
        source[0] = 9;

        Assert.Equal(new byte[] { 1, 2, 3, 4 }, image.Pixels.ToArray());
    }

    [Theory]
    [MemberData(nameof(GetRgbaFormats))]
    public void RgbaGpuFormatsPreserveChannelOrder(SDL_GPUTextureFormat format)
    {
        RgbaImage image = RgbaImage.FromGpuPixels(1, 1, [10, 20, 30, 40], format);

        Assert.Equal(new byte[] { 10, 20, 30, 40 }, image.Pixels.ToArray());
    }

    [Theory]
    [MemberData(nameof(GetBgraFormats))]
    public void BgraGpuFormatsNormalizeToRgba(SDL_GPUTextureFormat format)
    {
        RgbaImage image = RgbaImage.FromGpuPixels(2, 1, [30, 20, 10, 40, 3, 2, 1, 4], format);

        Assert.Equal(new byte[] { 10, 20, 30, 40, 1, 2, 3, 4 }, image.Pixels.ToArray());
    }

    [Fact]
    public void UnsupportedGpuFormatFailsExplicitly()
    {
        Assert.Throws<NotSupportedException>(() => RgbaImage.FromGpuPixels(
            1,
            1,
            [1, 2, 3, 4],
            SDL_GPUTextureFormat.SDL_GPU_TEXTUREFORMAT_D32_FLOAT));
    }

    [Fact]
    public void PngEncodingIsDeterministicAndPreservesDimensions()
    {
        var image = new RgbaImage(
            2,
            2,
            [
                255, 0, 0, 255,
                0, 255, 0, 255,
                0, 0, 255, 255,
                255, 255, 255, 128,
            ]);

        byte[] first = PngImageWriter.Encode(image);
        byte[] second = PngImageWriter.Encode(image);

        Assert.Equal(first, second);
        Assert.Equal(new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 }, first[..8]);
        Assert.Equal("IHDR", System.Text.Encoding.ASCII.GetString(first, 12, 4));
        Assert.Equal(2, BinaryPrimitives.ReadInt32BigEndian(first.AsSpan(16, 4)));
        Assert.Equal(2, BinaryPrimitives.ReadInt32BigEndian(first.AsSpan(20, 4)));
    }

    [Fact]
    public void PngWriterRequiresPngPathAndAtomicallyReplacesOutput()
    {
        var image = new RgbaImage(1, 1, [10, 20, 30, 255]);
        string directory = Path.Combine(Path.GetTempPath(), "chronofall-png-tests-" + Guid.NewGuid().ToString("N"));
        string path = Path.Combine(directory, "capture.png");
        try
        {
            Assert.Throws<ArgumentException>(() => PngImageWriter.Write(Path.Combine(directory, "capture.ppm"), image));

            PngImageWriter.Write(path, image);
            byte[] first = File.ReadAllBytes(path);
            PngImageWriter.Write(path, image);

            Assert.Equal(first, File.ReadAllBytes(path));
            Assert.Empty(Directory.EnumerateFiles(directory, "*.tmp-*"));
        }
        finally
        {
            if (Directory.Exists(directory))
                Directory.Delete(directory, recursive: true);
        }
    }

    public static IEnumerable<object[]> GetRgbaFormats() =>
        RgbaFormats.Select(static format => new object[] { format });

    public static IEnumerable<object[]> GetBgraFormats() =>
        BgraFormats.Select(static format => new object[] { format });
}
