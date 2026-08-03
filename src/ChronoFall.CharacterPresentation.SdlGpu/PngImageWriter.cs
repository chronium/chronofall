using StbImageWriteSharp;

namespace ChronoFall.CharacterPresentation.SdlGpu;

public static class PngImageWriter
{
    private static ReadOnlySpan<byte> Signature => [137, 80, 78, 71, 13, 10, 26, 10];

    public static byte[] Encode(RgbaImage image)
    {
        ArgumentNullException.ThrowIfNull(image);
        using var output = new MemoryStream();
        new ImageWriter().WritePng(
            image.Pixels.ToArray(),
            image.Width,
            image.Height,
            ColorComponents.RedGreenBlueAlpha,
            output);
        byte[] encoded = output.ToArray();
        if (!encoded.AsSpan().StartsWith(Signature))
            throw new InvalidDataException("PNG encoder did not produce a PNG signature.");
        return encoded;
    }

    public static void Write(string path, RgbaImage image)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        ArgumentNullException.ThrowIfNull(image);
        if (!string.Equals(Path.GetExtension(path), ".png", StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException("Screenshot path must end in .png.", nameof(path));

        string fullPath = Path.GetFullPath(path);
        string? directory = Path.GetDirectoryName(fullPath);
        if (string.IsNullOrEmpty(directory))
            throw new ArgumentException("Screenshot path must resolve to a parent directory.", nameof(path));
        Directory.CreateDirectory(directory);

        string temporaryPath = fullPath + ".tmp-" + Guid.NewGuid().ToString("N");
        try
        {
            File.WriteAllBytes(temporaryPath, Encode(image));
            File.Move(temporaryPath, fullPath, overwrite: true);
        }
        finally
        {
            if (File.Exists(temporaryPath))
                File.Delete(temporaryPath);
        }
    }
}
