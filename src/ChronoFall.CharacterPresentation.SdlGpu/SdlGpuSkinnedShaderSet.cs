using SDL;

namespace ChronoFall.CharacterPresentation.SdlGpu;

public sealed class SdlGpuSkinnedShaderSet
{
    public SdlGpuSkinnedShaderSet(
        SDL_GPUShaderFormat format,
        ReadOnlyMemory<byte> vertexShader,
        ReadOnlyMemory<byte> fragmentShader,
        string entryPoint)
    {
        if (format is not (SDL_GPUShaderFormat.SDL_GPU_SHADERFORMAT_MSL or
            SDL_GPUShaderFormat.SDL_GPU_SHADERFORMAT_SPIRV))
        {
            throw new ArgumentOutOfRangeException(nameof(format), "Skinned character shaders support MSL or SPIR-V bytecode.");
        }
        if (vertexShader.IsEmpty)
            throw new ArgumentException("Vertex shader bytecode cannot be empty.", nameof(vertexShader));
        if (fragmentShader.IsEmpty)
            throw new ArgumentException("Fragment shader bytecode cannot be empty.", nameof(fragmentShader));
        ArgumentException.ThrowIfNullOrWhiteSpace(entryPoint);

        Format = format;
        VertexShader = vertexShader.ToArray();
        FragmentShader = fragmentShader.ToArray();
        EntryPoint = entryPoint;
    }

    public SDL_GPUShaderFormat Format { get; }

    internal ReadOnlyMemory<byte> VertexShader { get; }

    internal ReadOnlyMemory<byte> FragmentShader { get; }

    public string EntryPoint { get; }
}
