using SDL;

namespace ChronoFall.CharacterExperiment.SdlGpu;

internal static class ShaderAssetSelector
{
    internal static SDL_GPUShaderFormat SelectPreferred(SDL_GPUShaderFormat supported)
    {
        if (supported.HasFlag(SDL_GPUShaderFormat.SDL_GPU_SHADERFORMAT_MSL))
            return SDL_GPUShaderFormat.SDL_GPU_SHADERFORMAT_MSL;
        if (supported.HasFlag(SDL_GPUShaderFormat.SDL_GPU_SHADERFORMAT_SPIRV))
            return SDL_GPUShaderFormat.SDL_GPU_SHADERFORMAT_SPIRV;

        throw new NotSupportedException($"SDL GPU supports no requested shader format. Reported formats: {supported}.");
    }

    internal static string GetFileName(string shaderName, SDL_GPUShaderFormat format) => format switch
    {
        SDL_GPUShaderFormat.SDL_GPU_SHADERFORMAT_MSL => shaderName + ".msl",
        SDL_GPUShaderFormat.SDL_GPU_SHADERFORMAT_SPIRV => shaderName + ".spv",
        _ => throw new NotSupportedException($"Shader format {format} is not supported by this experiment."),
    };

    internal static string GetEntrypoint(SDL_GPUShaderFormat format) => format switch
    {
        SDL_GPUShaderFormat.SDL_GPU_SHADERFORMAT_MSL => "main0",
        SDL_GPUShaderFormat.SDL_GPU_SHADERFORMAT_SPIRV => "main",
        _ => throw new NotSupportedException($"Shader format {format} is not supported by this experiment."),
    };
}
