using System.Globalization;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using SDL;
using static SDL.SDL3;

namespace ChronoFall.CharacterExperiment.SdlGpu;

internal sealed record CharacterHarnessOptions(
    int Width = 512,
    int Height = 512,
    bool Visible = false,
    string? CapturePath = null,
    string? SkeletonCapturePath = null,
    string? AnimationCapturePath = null,
    string? CaptureSuiteDirectory = null);

internal sealed record CharacterHarnessResult(
    SDL_GPUShaderFormat ShaderFormat,
    FrameAnalysis BindPose,
    FrameAnalysis TranslatedProbe,
    FrameAnalysis SkeletonDebug,
    FrameAnalysis AnimationStart,
    FrameAnalysis AnimationSample,
    FrameAnalysis AnimationLaterSample,
    FrameAnalysis AnimationLoopBoundary,
    SkeletonOverlayAnalysis SkeletonOverlay,
    ulong BindPoseFingerprint,
    ulong TranslatedProbeFingerprint,
    ulong SkeletonDebugFingerprint,
    ulong AnimationStartFingerprint,
    ulong AnimationSampleFingerprint,
    ulong AnimationLaterSampleFingerprint,
    ulong AnimationLoopBoundaryFingerprint,
    int SkeletonLineCount);

internal readonly record struct FrameAnalysis(
    int RenderedPixels,
    int FirstSectionPixels,
    int SecondSectionPixels,
    int MinimumX,
    int MinimumY,
    int MaximumX,
    int MaximumY,
    float CentroidX,
    float CentroidY);

internal readonly record struct SkeletonOverlayAnalysis(
    int ChangedPixels,
    int LinkPixels,
    int YAxisPixels);

internal readonly record struct CharacterAnimationFrame(
    float SampleTime,
    SkeletonGlobalPose GlobalPose,
    Matrix4x4[] PackedPalette);

internal static class SdlGpuCharacterHarness
{
    private static readonly SDL_FColor ClearColor = new() { r = 0.035f, g = 0.045f, b = 0.070f, a = 1.0f };
    private const float DeterministicAnimationSampleTime = 0.5f;
    private const float DeterministicAnimationLaterSampleTime = 1.0f;

    internal static CharacterHarnessResult Run(
        SkeletalCharacterAsset asset,
        AnimationClip animation,
        CharacterHarnessOptions options)
    {
        ArgumentNullException.ThrowIfNull(asset);
        ArgumentNullException.ThrowIfNull(animation);
        ArgumentNullException.ThrowIfNull(options);
        if (options.Width < 64 || options.Height < 64)
            throw new ArgumentOutOfRangeException(nameof(options), "The GPU harness target must be at least 64x64.");
        if (!ReferenceEquals(animation.Skeleton, asset.Mesh.Skin.Skeleton))
            throw new ArgumentException($"Animation '{animation.Name}' does not use the selected mesh skeleton.", nameof(animation));

        GpuSkinnedMeshData mesh = GpuSkinnedMeshData.Create(asset.Mesh);
        SkeletonPose bindPose = asset.Mesh.Skin.Skeleton.CreateBindPose();
        SkeletonGlobalPose globalPose = SkeletonPoseEvaluator.EvaluateGlobal(bindPose);
        SkinningPalette palette = SkeletonPoseEvaluator.CreateSkinningPalette(asset.Mesh.Skin, globalPose);
        BindPoseCamera camera = BindPoseCamera.Create(mesh.Bounds, options.Width, options.Height);
        float skeletonAxisLength = mesh.Bounds.Radius * 0.04f;
        SkeletonDebugGeometry skeletonDebug = SkeletonDebugGeometry.Create(globalPose, skeletonAxisLength);

        using var gpu = new CharacterGpuSession(mesh, skeletonDebug, options.Width, options.Height, options.Visible);

        gpu.UploadPalette(GpuMatrixPacking.PackTransposed(palette));
        byte[] bindPixels = gpu.RenderOffscreen(camera.TransposedViewProjection, includeSkeleton: false);
        FrameAnalysis bindAnalysis = Analyze(bindPixels, options.Width, options.Height, "bind-pose");

        float probeDistance = mesh.Bounds.Radius * 0.08f;
        gpu.UploadPalette(GpuMatrixPacking.PackTransposed(
            palette,
            Matrix4x4.CreateTranslation(probeDistance, 0.0f, 0.0f)));
        byte[] probePixels = gpu.RenderOffscreen(camera.TransposedViewProjection, includeSkeleton: false);
        FrameAnalysis probeAnalysis = Analyze(probePixels, options.Width, options.Height, "translated-palette");

        gpu.UploadPalette(GpuMatrixPacking.PackTransposed(palette));
        byte[] skeletonPixels = gpu.RenderOffscreen(camera.TransposedViewProjection, includeSkeleton: true);
        FrameAnalysis skeletonAnalysis = Analyze(skeletonPixels, options.Width, options.Height, "skeleton-debug");
        SkeletonOverlayAnalysis skeletonOverlay = AnalyzeSkeletonOverlay(bindPixels, skeletonPixels, options.Width, options.Height);

        CharacterAnimationFrame animationStart = CreateAnimationFrame(asset.Mesh.Skin, animation, 0.0f);
        gpu.UploadPalette(animationStart.PackedPalette);
        byte[] animationStartPixels = gpu.RenderOffscreen(camera.TransposedViewProjection, includeSkeleton: false);
        FrameAnalysis animationStartAnalysis = Analyze(animationStartPixels, options.Width, options.Height, "animation-start");

        CharacterAnimationFrame animationSample = CreateAnimationFrame(asset.Mesh.Skin, animation, DeterministicAnimationSampleTime);
        gpu.UploadPalette(animationSample.PackedPalette);
        byte[] animationSamplePixels = gpu.RenderOffscreen(camera.TransposedViewProjection, includeSkeleton: false);
        FrameAnalysis animationSampleAnalysis = Analyze(animationSamplePixels, options.Width, options.Height, "animation-sample");

        CharacterAnimationFrame animationLaterSample = CreateAnimationFrame(asset.Mesh.Skin, animation, DeterministicAnimationLaterSampleTime);
        gpu.UploadPalette(animationLaterSample.PackedPalette);
        byte[] animationLaterSamplePixels = gpu.RenderOffscreen(camera.TransposedViewProjection, includeSkeleton: false);
        FrameAnalysis animationLaterSampleAnalysis = Analyze(
            animationLaterSamplePixels,
            options.Width,
            options.Height,
            "animation-later-sample");

        CharacterAnimationFrame animationLoopBoundary = CreateAnimationFrame(asset.Mesh.Skin, animation, animation.Duration);
        gpu.UploadPalette(animationLoopBoundary.PackedPalette);
        byte[] animationLoopBoundaryPixels = gpu.RenderOffscreen(camera.TransposedViewProjection, includeSkeleton: false);
        FrameAnalysis animationLoopBoundaryAnalysis = Analyze(animationLoopBoundaryPixels, options.Width, options.Height, "animation-loop-boundary");

        ulong bindFingerprint = Fingerprint(bindPixels);
        ulong probeFingerprint = Fingerprint(probePixels);
        ulong skeletonFingerprint = Fingerprint(skeletonPixels);
        ulong animationStartFingerprint = Fingerprint(animationStartPixels);
        ulong animationSampleFingerprint = Fingerprint(animationSamplePixels);
        ulong animationLaterSampleFingerprint = Fingerprint(animationLaterSamplePixels);
        ulong animationLoopBoundaryFingerprint = Fingerprint(animationLoopBoundaryPixels);
        Require(bindFingerprint != probeFingerprint, "Translated palette probe produced the bind-pose fingerprint.");
        Require(bindFingerprint != skeletonFingerprint, "Skeleton debug overlay produced the bind-pose fingerprint.");
        Require(
            animationStartFingerprint == animationLoopBoundaryFingerprint,
            $"Animation loop boundary {animationLoopBoundaryFingerprint:x16} did not reproduce start {animationStartFingerprint:x16}.");
        Require(animationSampleFingerprint != animationStartFingerprint, "Animation sample produced the loop-start fingerprint.");
        Require(animationSampleFingerprint != bindFingerprint, "Animation sample produced the bind-pose fingerprint.");
        Require(animationLaterSampleFingerprint != animationStartFingerprint, "Later animation sample produced the loop-start fingerprint.");
        Require(animationLaterSampleFingerprint != bindFingerprint, "Later animation sample produced the bind-pose fingerprint.");
        Require(animationLaterSampleFingerprint != animationSampleFingerprint, "Animation samples at 0.5 and 1.0 seconds produced the same fingerprint.");
        Require(
            MathF.Abs(probeAnalysis.CentroidX - bindAnalysis.CentroidX) >= options.Width * 0.025f,
            $"Translated palette probe shifted the rendered centroid by only {MathF.Abs(probeAnalysis.CentroidX - bindAnalysis.CentroidX):F2} pixels.");

        if (!string.IsNullOrWhiteSpace(options.CapturePath))
            WritePpm(options.CapturePath, options.Width, options.Height, bindPixels);
        if (!string.IsNullOrWhiteSpace(options.SkeletonCapturePath))
            WritePpm(options.SkeletonCapturePath, options.Width, options.Height, skeletonPixels);
        if (!string.IsNullOrWhiteSpace(options.AnimationCapturePath))
            WritePpm(options.AnimationCapturePath, options.Width, options.Height, animationSamplePixels);
        if (!string.IsNullOrWhiteSpace(options.CaptureSuiteDirectory))
        {
            WriteCaptureSuite(
                options.CaptureSuiteDirectory,
                options.Width,
                options.Height,
                bindPixels,
                animationStartPixels,
                animationSamplePixels,
                animationLaterSamplePixels,
                animationLoopBoundaryPixels);
        }

        Console.WriteLine(
            $"GPU_HARNESS_PASS bind-pose shader={gpu.ShaderFormat} pixels={bindAnalysis.RenderedPixels} " +
            $"sections={bindAnalysis.FirstSectionPixels}/{bindAnalysis.SecondSectionPixels} " +
            $"bounds={bindAnalysis.MinimumX},{bindAnalysis.MinimumY}-{bindAnalysis.MaximumX},{bindAnalysis.MaximumY} " +
            $"fingerprint={bindFingerprint:x16}");
        Console.WriteLine(
            $"GPU_HARNESS_PASS palette-probe shift={probeAnalysis.CentroidX - bindAnalysis.CentroidX:F2} " +
            $"fingerprint={probeFingerprint:x16}");
        Console.WriteLine(
            $"GPU_HARNESS_PASS skeleton-debug lines={skeletonDebug.LineCount} changed={skeletonOverlay.ChangedPixels} " +
            $"links={skeletonOverlay.LinkPixels} y-axes={skeletonOverlay.YAxisPixels} " +
            $"fingerprint={skeletonFingerprint:x16}");
        Console.WriteLine(string.Create(CultureInfo.InvariantCulture,
            $"GPU_HARNESS_PASS animation clip={animation.Name} sample={DeterministicAnimationSampleTime:F3} " +
            $"duration={animation.Duration:F6} start={animationStartFingerprint:x16} " +
            $"sample-fingerprint={animationSampleFingerprint:x16} " +
            $"later-sample={DeterministicAnimationLaterSampleTime:F3} " +
            $"later-fingerprint={animationLaterSampleFingerprint:x16} loop={animationLoopBoundaryFingerprint:x16}"));

        if (options.Visible)
        {
            Console.WriteLine($"GPU_HARNESS_VISIBLE Playing {animation.Name} at normal speed with root motion disabled. Close the window or press Escape after inspection.");
            gpu.RunVisible(
                camera.TransposedViewProjection,
                asset.Animations,
                animation,
                asset.Mesh.Skin,
                skeletonAxisLength);
        }

        return new CharacterHarnessResult(
            gpu.ShaderFormat,
            bindAnalysis,
            probeAnalysis,
            skeletonAnalysis,
            animationStartAnalysis,
            animationSampleAnalysis,
            animationLaterSampleAnalysis,
            animationLoopBoundaryAnalysis,
            skeletonOverlay,
            bindFingerprint,
            probeFingerprint,
            skeletonFingerprint,
            animationStartFingerprint,
            animationSampleFingerprint,
            animationLaterSampleFingerprint,
            animationLoopBoundaryFingerprint,
            skeletonDebug.LineCount);
    }

    internal static CharacterAnimationFrame CreateAnimationFrame(
        SkinDefinition skin,
        AnimationClip animation,
        float time)
    {
        ArgumentNullException.ThrowIfNull(skin);
        ArgumentNullException.ThrowIfNull(animation);
        if (!ReferenceEquals(skin.Skeleton, animation.Skeleton))
            throw new ArgumentException($"Animation '{animation.Name}' does not use the skin skeleton.", nameof(animation));

        float sampleTime = AnimationSampler.ResolveTime(animation, time, AnimationPlaybackMode.Loop);
        SkeletonPose pose = AnimationSampler.Sample(animation, sampleTime, AnimationPlaybackMode.Clamp);
        SkeletonGlobalPose globalPose = SkeletonPoseEvaluator.EvaluateGlobal(pose);
        SkinningPalette palette = SkeletonPoseEvaluator.CreateSkinningPalette(skin, globalPose);
        return new CharacterAnimationFrame(sampleTime, globalPose, GpuMatrixPacking.PackTransposed(palette));
    }

    private static SkeletonOverlayAnalysis AnalyzeSkeletonOverlay(
        byte[] baseline,
        byte[] overlay,
        int width,
        int height)
    {
        int expectedLength = checked(width * height * 4);
        Require(baseline.Length == expectedLength, $"skeleton-debug: unexpected baseline byte count {baseline.Length}.");
        Require(overlay.Length == expectedLength, $"skeleton-debug: unexpected overlay byte count {overlay.Length}.");

        int changedPixels = 0;
        int linkPixels = 0;
        int yAxisPixels = 0;
        for (int offset = 0; offset < overlay.Length; offset += 4)
        {
            byte red = overlay[offset];
            byte green = overlay[offset + 1];
            byte blue = overlay[offset + 2];
            if (Math.Abs(red - baseline[offset]) > 3 ||
                Math.Abs(green - baseline[offset + 1]) > 3 ||
                Math.Abs(blue - baseline[offset + 2]) > 3)
            {
                changedPixels++;
            }

            if (red > 180 && green > 150 && blue < 100)
                linkPixels++;
            if (green > 180 && red < 100 && blue < 100)
                yAxisPixels++;
        }

        Require(changedPixels > 100, $"skeleton-debug: only {changedPixels} pixels changed from the bind-pose frame.");
        Require(linkPixels > 25, $"skeleton-debug: only {linkPixels} hierarchy-link pixels were classified.");
        Require(yAxisPixels > 25, $"skeleton-debug: only {yAxisPixels} Y-axis pixels were classified.");
        return new SkeletonOverlayAnalysis(changedPixels, linkPixels, yAxisPixels);
    }

    private static FrameAnalysis Analyze(byte[] rgba, int width, int height, string passName)
    {
        Require(rgba.Length == checked(width * height * 4), $"{passName}: unexpected RGBA byte count {rgba.Length}.");
        byte clearRed = ToByte(ClearColor.r);
        byte clearGreen = ToByte(ClearColor.g);
        byte clearBlue = ToByte(ClearColor.b);
        int rendered = 0;
        int firstSection = 0;
        int secondSection = 0;
        int minimumX = width;
        int minimumY = height;
        int maximumX = -1;
        int maximumY = -1;
        long xTotal = 0;
        long yTotal = 0;

        for (int y = 0; y < height; y++)
        for (int x = 0; x < width; x++)
        {
            int offset = (y * width + x) * 4;
            byte red = rgba[offset];
            byte green = rgba[offset + 1];
            byte blue = rgba[offset + 2];
            Require(rgba[offset + 3] == byte.MaxValue, $"{passName}: pixel {x},{y} was not opaque.");
            bool differs = Math.Abs(red - clearRed) > 3 || Math.Abs(green - clearGreen) > 3 || Math.Abs(blue - clearBlue) > 3;
            if (!differs)
                continue;

            rendered++;
            xTotal += x;
            yTotal += y;
            minimumX = Math.Min(minimumX, x);
            minimumY = Math.Min(minimumY, y);
            maximumX = Math.Max(maximumX, x);
            maximumY = Math.Max(maximumY, y);
            if (red > blue + 18 && red > green)
                firstSection++;
            if (blue > red + 18 && blue > green)
                secondSection++;
        }

        Require(rendered > width * height / 80, $"{passName}: only {rendered} pixels differed from the clear color.");
        Require(firstSection > 100, $"{passName}: first diagnostic section produced only {firstSection} classified pixels.");
        Require(secondSection > 100, $"{passName}: second diagnostic section produced only {secondSection} classified pixels.");
        Require(minimumX > 4 && minimumY > 4 && maximumX < width - 5 && maximumY < height - 5,
            $"{passName}: rendered bounds {minimumX},{minimumY}-{maximumX},{maximumY} touch the target edge.");
        float centroidX = xTotal / (float)rendered;
        float centroidY = yTotal / (float)rendered;
        Require(MathF.Abs(centroidX - (width - 1) * 0.5f) < width * 0.16f,
            $"{passName}: horizontal centroid {centroidX:F2} is not centered.");
        Require(MathF.Abs(centroidY - (height - 1) * 0.5f) < height * 0.16f,
            $"{passName}: vertical centroid {centroidY:F2} is not centered.");

        return new FrameAnalysis(
            rendered,
            firstSection,
            secondSection,
            minimumX,
            minimumY,
            maximumX,
            maximumY,
            centroidX,
            centroidY);
    }

    private static ulong Fingerprint(ReadOnlySpan<byte> bytes)
    {
        const ulong offsetBasis = 14695981039346656037;
        const ulong prime = 1099511628211;
        ulong hash = offsetBasis;
        foreach (byte value in bytes)
        {
            hash ^= value;
            hash *= prime;
        }
        return hash;
    }

    private static void WritePpm(string path, int width, int height, byte[] rgba)
    {
        string fullPath = Path.GetFullPath(path);
        Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);
        byte[] header = Encoding.ASCII.GetBytes($"P6\n{width} {height}\n255\n");
        byte[] output = new byte[checked(header.Length + width * height * 3)];
        header.CopyTo(output, 0);
        int destination = header.Length;
        for (int source = 0; source < rgba.Length; source += 4)
        {
            output[destination++] = rgba[source];
            output[destination++] = rgba[source + 1];
            output[destination++] = rgba[source + 2];
        }
        File.WriteAllBytes(fullPath, output);
        Console.WriteLine($"GPU_HARNESS_CAPTURE {fullPath}");
    }

    private static void WriteCaptureSuite(
        string directory,
        int width,
        int height,
        byte[] bindPose,
        byte[] animationStart,
        byte[] animationSample,
        byte[] animationLaterSample,
        byte[] animationLoopBoundary)
    {
        string fullDirectory = Path.GetFullPath(directory);
        WritePpm(Path.Combine(fullDirectory, "bind-pose.ppm"), width, height, bindPose);
        WritePpm(Path.Combine(fullDirectory, "animation-0000ms.ppm"), width, height, animationStart);
        WritePpm(Path.Combine(fullDirectory, "animation-0500ms.ppm"), width, height, animationSample);
        WritePpm(Path.Combine(fullDirectory, "animation-1000ms.ppm"), width, height, animationLaterSample);
        WritePpm(Path.Combine(fullDirectory, "animation-loop-boundary.ppm"), width, height, animationLoopBoundary);
        Console.WriteLine($"GPU_HARNESS_CAPTURE_SUITE {fullDirectory}");
    }

    private static byte ToByte(float value) => (byte)MathF.Round(Math.Clamp(value, 0.0f, 1.0f) * byte.MaxValue);

    private static void Require(bool condition, string message)
    {
        if (!condition)
            throw new InvalidOperationException(message);
    }

    internal static SDL_WindowFlags SelectWindowFlags(bool visible) =>
        visible ? (SDL_WindowFlags)0 : SDL_WindowFlags.SDL_WINDOW_HIDDEN;

    internal static void ExecuteInteractiveFrame(
        AnimationClip clip,
        float sampleTime,
        int jointCount,
        Action operation)
    {
        ArgumentNullException.ThrowIfNull(clip);
        ArgumentNullException.ThrowIfNull(operation);

        try
        {
            operation();
        }
        catch (Exception exception)
        {
            throw new InvalidOperationException(
                string.Create(
                    CultureInfo.InvariantCulture,
                    $"Interactive animation validation failed for clip '{clip.Name}' " +
                    $"at sample {sampleTime:F3} seconds (joints={jointCount})."),
                exception);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    private readonly record struct MaterialConstants(Vector4 BaseColor, Vector4 LightDirection);

    private sealed unsafe class CharacterGpuSession : IDisposable
    {
        private const SDL_GPUTextureFormat DepthFormat = SDL_GPUTextureFormat.SDL_GPU_TEXTUREFORMAT_D32_FLOAT;
        private readonly int width;
        private readonly int height;
        private SDL_Window* window;
        private SDL_GPUDevice* device;
        private SDL_GPUShader* vertexShader;
        private SDL_GPUShader* fragmentShader;
        private SDL_GPUGraphicsPipeline* pipeline;
        private SDL_GPUShader* skeletonVertexShader;
        private SDL_GPUShader* skeletonFragmentShader;
        private SDL_GPUGraphicsPipeline* skeletonPipeline;
        private SDL_GPUBuffer* vertexBuffer;
        private SDL_GPUBuffer* indexBuffer;
        private SDL_GPUBuffer* paletteBuffer;
        private SDL_GPUTransferBuffer* paletteTransferBuffer;
        private SDL_GPUBuffer* skeletonVertexBuffer;
        private SDL_GPUTransferBuffer* skeletonTransferBuffer;
        private SDL_GPUTexture* offscreenColor;
        private SDL_GPUTexture* offscreenDepth;
        private SDL_GPUTexture* visibleDepth;
        private uint visibleDepthWidth;
        private uint visibleDepthHeight;
        private bool windowClaimed;
        private readonly GpuMeshSection[] sections;
        private readonly int jointCount;
        private readonly uint skeletonVertexCount;

        internal CharacterGpuSession(
            GpuSkinnedMeshData mesh,
            SkeletonDebugGeometry skeletonDebug,
            int width,
            int height,
            bool visible)
        {
            ArgumentNullException.ThrowIfNull(skeletonDebug);
            this.width = width;
            this.height = height;
            sections = mesh.Sections;
            jointCount = mesh.JointCount;
            skeletonVertexCount = checked((uint)skeletonDebug.Vertices.Length);

            if (!SDL_Init(SDL_InitFlags.SDL_INIT_VIDEO))
                throw new InvalidOperationException($"SDL video initialization failed: {SDL_GetError()}");

            try
            {
                SDL_WindowFlags flags = SelectWindowFlags(visible);
                window = SDL_CreateWindow("ChronoFall character animation experiment", width, height, flags);
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

                ShaderFormat = ShaderAssetSelector.SelectPreferred(SDL_GetGPUShaderFormats(device));
                SDL_GPUTextureFormat colorFormat = SDL_GetGPUSwapchainTextureFormat(device, window);
                vertexShader = LoadShader("bind-pose.vert", SDL_GPUShaderStage.SDL_GPU_SHADERSTAGE_VERTEX, storageBuffers: 1, uniformBuffers: 1);
                fragmentShader = LoadShader("bind-pose.frag", SDL_GPUShaderStage.SDL_GPU_SHADERSTAGE_FRAGMENT, storageBuffers: 0, uniformBuffers: 1);
                pipeline = CreatePipeline(colorFormat);
                skeletonVertexShader = LoadShader("skeleton-debug.vert", SDL_GPUShaderStage.SDL_GPU_SHADERSTAGE_VERTEX, storageBuffers: 0, uniformBuffers: 1);
                skeletonFragmentShader = LoadShader("skeleton-debug.frag", SDL_GPUShaderStage.SDL_GPU_SHADERSTAGE_FRAGMENT, storageBuffers: 0, uniformBuffers: 0);
                skeletonPipeline = CreateSkeletonPipeline(colorFormat);

                vertexBuffer = CreateBuffer(SDL_GPUBufferUsageFlags.SDL_GPU_BUFFERUSAGE_VERTEX, checked((uint)(mesh.Vertices.Length * GpuSkinnedVertex.Stride)));
                indexBuffer = CreateBuffer(SDL_GPUBufferUsageFlags.SDL_GPU_BUFFERUSAGE_INDEX, checked((uint)(mesh.Indices.Length * sizeof(uint))));
                paletteBuffer = CreateBuffer(
                    SDL_GPUBufferUsageFlags.SDL_GPU_BUFFERUSAGE_GRAPHICS_STORAGE_READ,
                    checked((uint)(jointCount * sizeof(Matrix4x4))));
                paletteTransferBuffer = CreateTransfer(
                    SDL_GPUTransferBufferUsage.SDL_GPU_TRANSFERBUFFERUSAGE_UPLOAD,
                    checked((uint)(jointCount * sizeof(Matrix4x4))));
                skeletonVertexBuffer = CreateBuffer(
                    SDL_GPUBufferUsageFlags.SDL_GPU_BUFFERUSAGE_VERTEX,
                    checked(skeletonVertexCount * GpuDebugLineVertex.Stride));
                skeletonTransferBuffer = CreateTransfer(
                    SDL_GPUTransferBufferUsage.SDL_GPU_TRANSFERBUFFERUSAGE_UPLOAD,
                    checked(skeletonVertexCount * GpuDebugLineVertex.Stride));
                UploadGeometry(mesh.Vertices, mesh.Indices);
                UploadBuffer(skeletonVertexBuffer, skeletonDebug.Vertices);

                offscreenColor = CreateTexture(colorFormat, SDL_GPUTextureUsageFlags.SDL_GPU_TEXTUREUSAGE_COLOR_TARGET, (uint)width, (uint)height);
                offscreenDepth = CreateTexture(DepthFormat, SDL_GPUTextureUsageFlags.SDL_GPU_TEXTUREUSAGE_DEPTH_STENCIL_TARGET, (uint)width, (uint)height);
            }
            catch
            {
                Dispose();
                throw;
            }
        }

        internal SDL_GPUShaderFormat ShaderFormat { get; }

        internal void UploadPalette(Matrix4x4[] matrices)
        {
            ArgumentNullException.ThrowIfNull(matrices);
            if (matrices.Length != jointCount)
                throw new ArgumentException($"Expected {jointCount} palette matrices, received {matrices.Length}.", nameof(matrices));
            UploadCycled(paletteTransferBuffer, paletteBuffer, matrices, "palette");
        }

        internal void UploadSkeleton(GpuDebugLineVertex[] vertices)
        {
            ArgumentNullException.ThrowIfNull(vertices);
            if (vertices.Length != skeletonVertexCount)
            {
                throw new ArgumentException(
                    $"Expected {skeletonVertexCount} skeleton vertices, received {vertices.Length}.",
                    nameof(vertices));
            }
            UploadCycled(skeletonTransferBuffer, skeletonVertexBuffer, vertices, "skeleton");
        }

        internal byte[] RenderOffscreen(Matrix4x4 transposedViewProjection, bool includeSkeleton)
        {
            uint byteCount = checked((uint)(width * height * 4));
            SDL_GPUTransferBuffer* transfer = CreateTransfer(SDL_GPUTransferBufferUsage.SDL_GPU_TRANSFERBUFFERUSAGE_DOWNLOAD, byteCount);
            SDL_GPUFence* fence = null;
            try
            {
                SDL_GPUCommandBuffer* command = AcquireCommand();
                Render(command, offscreenColor, offscreenDepth, (uint)width, (uint)height, transposedViewProjection, includeSkeleton);
                SDL_GPUCopyPass* copy = SDL_BeginGPUCopyPass(command);
                if (copy is null)
                    throw new InvalidOperationException($"SDL GPU readback copy pass failed: {SDL_GetError()}");
                var source = new SDL_GPUTextureRegion
                {
                    texture = offscreenColor,
                    w = (uint)width,
                    h = (uint)height,
                    d = 1,
                };
                var destination = new SDL_GPUTextureTransferInfo
                {
                    transfer_buffer = transfer,
                    pixels_per_row = (uint)width,
                    rows_per_layer = (uint)height,
                };
                SDL_DownloadFromGPUTexture(copy, &source, &destination);
                SDL_EndGPUCopyPass(copy);
                fence = SDL_SubmitGPUCommandBufferAndAcquireFence(command);
                if (fence is null)
                    throw new InvalidOperationException($"SDL GPU readback submission failed: {SDL_GetError()}");
                SDL_GPUFence* fenceValue = fence;
                if (!SDL_WaitForGPUFences(device, wait_all: true, &fenceValue, 1))
                    throw new InvalidOperationException($"SDL GPU fence wait failed: {SDL_GetError()}");
                IntPtr mapped = SDL_MapGPUTransferBuffer(device, transfer, cycle: false);
                if (mapped == IntPtr.Zero)
                    throw new InvalidOperationException($"SDL GPU readback mapping failed: {SDL_GetError()}");
                try
                {
                    byte[] pixels = new byte[byteCount];
                    Marshal.Copy(mapped, pixels, 0, pixels.Length);
                    SDL_GPUTextureFormat format = SDL_GetGPUSwapchainTextureFormat(device, window);
                    NormalizeToRgba(pixels, format);
                    return pixels;
                }
                finally
                {
                    SDL_UnmapGPUTransferBuffer(device, transfer);
                }
            }
            finally
            {
                if (fence is not null)
                    SDL_ReleaseGPUFence(device, fence);
                SDL_ReleaseGPUTransferBuffer(device, transfer);
            }
        }

        internal void RunVisible(
            Matrix4x4 transposedViewProjection,
            IReadOnlyList<AnimationClip> animations,
            AnimationClip initialAnimation,
            SkinDefinition skin,
            float skeletonAxisLength)
        {
            ArgumentNullException.ThrowIfNull(animations);
            ArgumentNullException.ThrowIfNull(initialAnimation);
            ArgumentNullException.ThrowIfNull(skin);
            if (!SDL_ShowWindow(window))
                throw new InvalidOperationException($"SDL could not show the validation window: {SDL_GetError()}");

            var playback = new CharacterPlaybackController(animations, initialAnimation.Name);
            ulong frequency = SDL_GetPerformanceFrequency();
            if (frequency == 0)
                throw new InvalidOperationException("SDL returned a zero performance-counter frequency.");
            ulong previousCounter = SDL_GetPerformanceCounter();
            ulong lastTitleCounter = 0;
            bool titleDirty = true;
            Console.WriteLine(
                "GPU_HARNESS_CONTROLS Left/Right=clip 1=Idle_Loop 2=Walk_Loop 3=Sword_Attack " +
                "Space=pause/resume R=restart D=skeleton Escape=close");
            Console.WriteLine(playback.CreateConsoleDiagnostic(jointCount, jointCount));
            bool running = true;
            while (running)
            {
                SDL_Event sdlEvent;
                while (SDL_PollEvent(&sdlEvent))
                {
                    if (sdlEvent.Type is SDL_EventType.SDL_EVENT_QUIT or SDL_EventType.SDL_EVENT_WINDOW_CLOSE_REQUESTED ||
                        sdlEvent.Type == SDL_EventType.SDL_EVENT_KEY_DOWN && sdlEvent.key.key == SDL_Keycode.SDLK_ESCAPE)
                    {
                        running = false;
                        continue;
                    }

                    if (sdlEvent.Type == SDL_EventType.SDL_EVENT_KEY_DOWN &&
                        ApplyControl(playback, sdlEvent.key.key))
                    {
                        titleDirty = true;
                        Console.WriteLine(playback.CreateConsoleDiagnostic(jointCount, jointCount));
                    }
                }

                if (!running)
                    break;

                ulong currentCounter = SDL_GetPerformanceCounter();
                double elapsedSeconds = (currentCounter - previousCounter) / (double)frequency;
                previousCounter = currentCounter;
                playback.Advance(elapsedSeconds);

                AnimationClip frameClip = playback.CurrentClip;
                float frameSampleTime = playback.SampleTime;
                ExecuteInteractiveFrame(frameClip, frameSampleTime, jointCount, () =>
                {
                    CharacterAnimationFrame frame = CreateAnimationFrame(skin, frameClip, frameSampleTime);
                    UploadPalette(frame.PackedPalette);
                    if (playback.IsSkeletonVisible)
                    {
                        SkeletonDebugGeometry skeleton = SkeletonDebugGeometry.Create(frame.GlobalPose, skeletonAxisLength);
                        UploadSkeleton(skeleton.Vertices);
                    }

                    if (titleDirty || currentCounter - lastTitleCounter >= frequency / 10)
                    {
                        SetWindowTitle(playback.CreateWindowTitle(jointCount, frame.PackedPalette.Length));
                        lastTitleCounter = currentCounter;
                        titleDirty = false;
                    }

                    SDL_GPUCommandBuffer* command = AcquireCommand();
                    SDL_GPUTexture* swapchain;
                    uint swapchainWidth;
                    uint swapchainHeight;
                    if (!SDL_WaitAndAcquireGPUSwapchainTexture(command, window, &swapchain, &swapchainWidth, &swapchainHeight))
                        throw new InvalidOperationException($"SDL GPU swapchain acquisition failed: {SDL_GetError()}");
                    if (swapchain is not null)
                    {
                        EnsureVisibleDepth(swapchainWidth, swapchainHeight);
                        Render(
                            command,
                            swapchain,
                            visibleDepth,
                            swapchainWidth,
                            swapchainHeight,
                            transposedViewProjection,
                            includeSkeleton: playback.IsSkeletonVisible);
                    }
                    if (!SDL_SubmitGPUCommandBuffer(command))
                        throw new InvalidOperationException($"SDL GPU visible submission failed: {SDL_GetError()}");
                });
                SDL_Delay(16);
            }
        }

        public void Dispose()
        {
            if (device is not null)
                _ = SDL_WaitForGPUIdle(device);
            ReleaseTexture(ref visibleDepth);
            ReleaseTexture(ref offscreenDepth);
            ReleaseTexture(ref offscreenColor);
            if (skeletonTransferBuffer is not null && device is not null)
                SDL_ReleaseGPUTransferBuffer(device, skeletonTransferBuffer);
            skeletonTransferBuffer = null;
            if (paletteTransferBuffer is not null && device is not null)
                SDL_ReleaseGPUTransferBuffer(device, paletteTransferBuffer);
            paletteTransferBuffer = null;
            ReleaseBuffer(ref skeletonVertexBuffer);
            ReleaseBuffer(ref paletteBuffer);
            ReleaseBuffer(ref indexBuffer);
            ReleaseBuffer(ref vertexBuffer);
            if (skeletonPipeline is not null && device is not null)
                SDL_ReleaseGPUGraphicsPipeline(device, skeletonPipeline);
            skeletonPipeline = null;
            if (pipeline is not null && device is not null)
                SDL_ReleaseGPUGraphicsPipeline(device, pipeline);
            pipeline = null;
            if (skeletonFragmentShader is not null && device is not null)
                SDL_ReleaseGPUShader(device, skeletonFragmentShader);
            skeletonFragmentShader = null;
            if (skeletonVertexShader is not null && device is not null)
                SDL_ReleaseGPUShader(device, skeletonVertexShader);
            skeletonVertexShader = null;
            if (fragmentShader is not null && device is not null)
                SDL_ReleaseGPUShader(device, fragmentShader);
            fragmentShader = null;
            if (vertexShader is not null && device is not null)
                SDL_ReleaseGPUShader(device, vertexShader);
            vertexShader = null;
            if (device is not null)
            {
                if (windowClaimed && window is not null)
                {
                    SDL_ReleaseWindowFromGPUDevice(device, window);
                    windowClaimed = false;
                }
                SDL_DestroyGPUDevice(device);
            }
            device = null;
            if (window is not null)
                SDL_DestroyWindow(window);
            window = null;
            SDL_Quit();
        }

        private static bool ApplyControl(CharacterPlaybackController playback, SDL_Keycode key)
        {
            switch (key)
            {
                case SDL_Keycode.SDLK_LEFT:
                    playback.SelectPrevious();
                    return true;
                case SDL_Keycode.SDLK_RIGHT:
                    playback.SelectNext();
                    return true;
                case SDL_Keycode.SDLK_1:
                    playback.SelectByName("Idle_Loop");
                    return true;
                case SDL_Keycode.SDLK_2:
                    playback.SelectByName("Walk_Loop");
                    return true;
                case SDL_Keycode.SDLK_3:
                    playback.SelectByName("Sword_Attack");
                    return true;
                case SDL_Keycode.SDLK_SPACE:
                    playback.TogglePlaying();
                    return true;
                case SDL_Keycode.SDLK_R:
                    playback.Restart();
                    return true;
                case SDL_Keycode.SDLK_D:
                    playback.ToggleSkeleton();
                    return true;
                default:
                    return false;
            }
        }

        private void SetWindowTitle(string title)
        {
            byte[] titleBytes = Encoding.UTF8.GetBytes(title + '\0');
            fixed (byte* titlePointer = titleBytes)
            {
                if (!SDL_SetWindowTitle(window, titlePointer))
                    throw new InvalidOperationException($"SDL could not update the diagnostic window title: {SDL_GetError()}");
            }
        }

        private void Render(
            SDL_GPUCommandBuffer* command,
            SDL_GPUTexture* color,
            SDL_GPUTexture* depth,
            uint renderWidth,
            uint renderHeight,
            Matrix4x4 transposedViewProjection,
            bool includeSkeleton)
        {
            var colorTarget = new SDL_GPUColorTargetInfo
            {
                texture = color,
                clear_color = ClearColor,
                load_op = SDL_GPULoadOp.SDL_GPU_LOADOP_CLEAR,
                store_op = SDL_GPUStoreOp.SDL_GPU_STOREOP_STORE,
            };
            var depthTarget = new SDL_GPUDepthStencilTargetInfo
            {
                texture = depth,
                clear_depth = 1.0f,
                load_op = SDL_GPULoadOp.SDL_GPU_LOADOP_CLEAR,
                store_op = SDL_GPUStoreOp.SDL_GPU_STOREOP_DONT_CARE,
                stencil_load_op = SDL_GPULoadOp.SDL_GPU_LOADOP_DONT_CARE,
                stencil_store_op = SDL_GPUStoreOp.SDL_GPU_STOREOP_DONT_CARE,
            };
            SDL_GPURenderPass* pass = SDL_BeginGPURenderPass(command, &colorTarget, 1, &depthTarget);
            if (pass is null)
                throw new InvalidOperationException($"SDL GPU render pass failed: {SDL_GetError()}");

            SDL_BindGPUGraphicsPipeline(pass, pipeline);
            var vertexBinding = new SDL_GPUBufferBinding { buffer = vertexBuffer };
            var indexBinding = new SDL_GPUBufferBinding { buffer = indexBuffer };
            SDL_BindGPUVertexBuffers(pass, 0, &vertexBinding, 1);
            SDL_BindGPUIndexBuffer(pass, &indexBinding, SDL_GPUIndexElementSize.SDL_GPU_INDEXELEMENTSIZE_32BIT);
            SDL_GPUBuffer* palette = paletteBuffer;
            SDL_BindGPUVertexStorageBuffers(pass, 0, &palette, 1);
            SDL_PushGPUVertexUniformData(command, 0, (IntPtr)(&transposedViewProjection), (uint)sizeof(Matrix4x4));

            for (int sectionIndex = 0; sectionIndex < sections.Length; sectionIndex++)
            {
                Vector4 colorValue = sectionIndex % 2 == 0
                    ? new Vector4(0.95f, 0.28f, 0.18f, 1.0f)
                    : new Vector4(0.16f, 0.62f, 0.98f, 1.0f);
                var material = new MaterialConstants(colorValue, new Vector4(-0.35f, -0.70f, -0.62f, 0.0f));
                SDL_PushGPUFragmentUniformData(command, 0, (IntPtr)(&material), (uint)sizeof(MaterialConstants));
                GpuMeshSection section = sections[sectionIndex];
                SDL_DrawGPUIndexedPrimitives(pass, section.IndexCount, 1, section.FirstIndex, 0, 0);
            }

            if (includeSkeleton)
            {
                SDL_BindGPUGraphicsPipeline(pass, skeletonPipeline);
                var skeletonBinding = new SDL_GPUBufferBinding { buffer = skeletonVertexBuffer };
                SDL_BindGPUVertexBuffers(pass, 0, &skeletonBinding, 1);
                SDL_PushGPUVertexUniformData(command, 0, (IntPtr)(&transposedViewProjection), (uint)sizeof(Matrix4x4));
                SDL_DrawGPUPrimitives(pass, skeletonVertexCount, 1, 0, 0);
            }

            SDL_EndGPURenderPass(pass);
        }

        private SDL_GPUShader* LoadShader(string name, SDL_GPUShaderStage stage, uint storageBuffers, uint uniformBuffers)
        {
            string path = Path.Combine(AppContext.BaseDirectory, "shaders", ShaderAssetSelector.GetFileName(name, ShaderFormat));
            if (!File.Exists(path))
                throw new FileNotFoundException($"SDL GPU shader asset was not found: {path}", path);
            byte[] code = File.ReadAllBytes(path);
            byte[] entrypointBytes = Encoding.UTF8.GetBytes(ShaderAssetSelector.GetEntrypoint(ShaderFormat) + '\0');
            fixed (byte* codePointer = code)
            fixed (byte* entrypoint = entrypointBytes)
            {
                var info = new SDL_GPUShaderCreateInfo
                {
                    code_size = (nuint)code.Length,
                    code = codePointer,
                    entrypoint = entrypoint,
                    format = ShaderFormat,
                    stage = stage,
                    num_storage_buffers = storageBuffers,
                    num_uniform_buffers = uniformBuffers,
                };
                SDL_GPUShader* shader = SDL_CreateGPUShader(device, &info);
                if (shader is null)
                    throw new InvalidOperationException($"SDL GPU shader creation failed for {path}: {SDL_GetError()}");
                return shader;
            }
        }

        private SDL_GPUGraphicsPipeline* CreatePipeline(SDL_GPUTextureFormat colorFormat)
        {
            var vertexBufferDescription = new SDL_GPUVertexBufferDescription
            {
                slot = 0,
                pitch = GpuSkinnedVertex.Stride,
                input_rate = SDL_GPUVertexInputRate.SDL_GPU_VERTEXINPUTRATE_VERTEX,
            };
            SDL_GPUVertexAttribute* attributes = stackalloc SDL_GPUVertexAttribute[4];
            attributes[0] = new SDL_GPUVertexAttribute { location = 0, buffer_slot = 0, format = SDL_GPUVertexElementFormat.SDL_GPU_VERTEXELEMENTFORMAT_FLOAT3, offset = GpuSkinnedVertex.PositionOffset };
            attributes[1] = new SDL_GPUVertexAttribute { location = 1, buffer_slot = 0, format = SDL_GPUVertexElementFormat.SDL_GPU_VERTEXELEMENTFORMAT_FLOAT3, offset = GpuSkinnedVertex.NormalOffset };
            attributes[2] = new SDL_GPUVertexAttribute { location = 2, buffer_slot = 0, format = SDL_GPUVertexElementFormat.SDL_GPU_VERTEXELEMENTFORMAT_USHORT4, offset = GpuSkinnedVertex.JointIndicesOffset };
            attributes[3] = new SDL_GPUVertexAttribute { location = 3, buffer_slot = 0, format = SDL_GPUVertexElementFormat.SDL_GPU_VERTEXELEMENTFORMAT_FLOAT4, offset = GpuSkinnedVertex.WeightsOffset };
            var colorDescription = new SDL_GPUColorTargetDescription { format = colorFormat };
            var info = new SDL_GPUGraphicsPipelineCreateInfo
            {
                vertex_shader = vertexShader,
                fragment_shader = fragmentShader,
                vertex_input_state = new SDL_GPUVertexInputState
                {
                    vertex_buffer_descriptions = &vertexBufferDescription,
                    num_vertex_buffers = 1,
                    vertex_attributes = attributes,
                    num_vertex_attributes = 4,
                },
                primitive_type = SDL_GPUPrimitiveType.SDL_GPU_PRIMITIVETYPE_TRIANGLELIST,
                rasterizer_state = new SDL_GPURasterizerState
                {
                    fill_mode = SDL_GPUFillMode.SDL_GPU_FILLMODE_FILL,
                    cull_mode = SDL_GPUCullMode.SDL_GPU_CULLMODE_BACK,
                    front_face = SDL_GPUFrontFace.SDL_GPU_FRONTFACE_COUNTER_CLOCKWISE,
                },
                multisample_state = new SDL_GPUMultisampleState { sample_count = SDL_GPUSampleCount.SDL_GPU_SAMPLECOUNT_1 },
                depth_stencil_state = new SDL_GPUDepthStencilState
                {
                    compare_op = SDL_GPUCompareOp.SDL_GPU_COMPAREOP_LESS,
                    enable_depth_test = true,
                    enable_depth_write = true,
                },
                target_info = new SDL_GPUGraphicsPipelineTargetInfo
                {
                    color_target_descriptions = &colorDescription,
                    num_color_targets = 1,
                    depth_stencil_format = DepthFormat,
                    has_depth_stencil_target = true,
                },
            };
            SDL_GPUGraphicsPipeline* created = SDL_CreateGPUGraphicsPipeline(device, &info);
            if (created is null)
                throw new InvalidOperationException($"SDL GPU pipeline creation failed: {SDL_GetError()}");
            return created;
        }

        private SDL_GPUGraphicsPipeline* CreateSkeletonPipeline(SDL_GPUTextureFormat colorFormat)
        {
            var vertexBufferDescription = new SDL_GPUVertexBufferDescription
            {
                slot = 0,
                pitch = GpuDebugLineVertex.Stride,
                input_rate = SDL_GPUVertexInputRate.SDL_GPU_VERTEXINPUTRATE_VERTEX,
            };
            SDL_GPUVertexAttribute* attributes = stackalloc SDL_GPUVertexAttribute[2];
            attributes[0] = new SDL_GPUVertexAttribute
            {
                location = 0,
                buffer_slot = 0,
                format = SDL_GPUVertexElementFormat.SDL_GPU_VERTEXELEMENTFORMAT_FLOAT3,
                offset = GpuDebugLineVertex.PositionOffset,
            };
            attributes[1] = new SDL_GPUVertexAttribute
            {
                location = 1,
                buffer_slot = 0,
                format = SDL_GPUVertexElementFormat.SDL_GPU_VERTEXELEMENTFORMAT_FLOAT4,
                offset = GpuDebugLineVertex.ColorOffset,
            };
            var colorDescription = new SDL_GPUColorTargetDescription { format = colorFormat };
            var info = new SDL_GPUGraphicsPipelineCreateInfo
            {
                vertex_shader = skeletonVertexShader,
                fragment_shader = skeletonFragmentShader,
                vertex_input_state = new SDL_GPUVertexInputState
                {
                    vertex_buffer_descriptions = &vertexBufferDescription,
                    num_vertex_buffers = 1,
                    vertex_attributes = attributes,
                    num_vertex_attributes = 2,
                },
                primitive_type = SDL_GPUPrimitiveType.SDL_GPU_PRIMITIVETYPE_LINELIST,
                rasterizer_state = new SDL_GPURasterizerState
                {
                    fill_mode = SDL_GPUFillMode.SDL_GPU_FILLMODE_FILL,
                    cull_mode = SDL_GPUCullMode.SDL_GPU_CULLMODE_NONE,
                    front_face = SDL_GPUFrontFace.SDL_GPU_FRONTFACE_COUNTER_CLOCKWISE,
                },
                multisample_state = new SDL_GPUMultisampleState { sample_count = SDL_GPUSampleCount.SDL_GPU_SAMPLECOUNT_1 },
                depth_stencil_state = new SDL_GPUDepthStencilState
                {
                    compare_op = SDL_GPUCompareOp.SDL_GPU_COMPAREOP_ALWAYS,
                    enable_depth_test = false,
                    enable_depth_write = false,
                },
                target_info = new SDL_GPUGraphicsPipelineTargetInfo
                {
                    color_target_descriptions = &colorDescription,
                    num_color_targets = 1,
                    depth_stencil_format = DepthFormat,
                    has_depth_stencil_target = true,
                },
            };
            SDL_GPUGraphicsPipeline* created = SDL_CreateGPUGraphicsPipeline(device, &info);
            if (created is null)
                throw new InvalidOperationException($"SDL GPU skeleton pipeline creation failed: {SDL_GetError()}");
            return created;
        }

        private void UploadGeometry(GpuSkinnedVertex[] vertices, uint[] indices)
        {
            UploadBuffer(vertexBuffer, vertices);
            UploadBuffer(indexBuffer, indices);
        }

        private void UploadCycled<T>(
            SDL_GPUTransferBuffer* transfer,
            SDL_GPUBuffer* destination,
            T[] values,
            string label)
            where T : unmanaged
        {
            uint byteCount = checked((uint)(values.Length * sizeof(T)));
            IntPtr mapped = SDL_MapGPUTransferBuffer(device, transfer, cycle: true);
            if (mapped == IntPtr.Zero)
                throw new InvalidOperationException($"SDL GPU {label} upload mapping failed: {SDL_GetError()}");
            fixed (T* source = values)
                Buffer.MemoryCopy(source, (void*)mapped, byteCount, byteCount);
            SDL_UnmapGPUTransferBuffer(device, transfer);

            SDL_GPUCommandBuffer* command = AcquireCommand();
            SDL_GPUCopyPass* copy = SDL_BeginGPUCopyPass(command);
            if (copy is null)
                throw new InvalidOperationException($"SDL GPU {label} upload copy pass failed: {SDL_GetError()}");
            var sourceLocation = new SDL_GPUTransferBufferLocation { transfer_buffer = transfer };
            var region = new SDL_GPUBufferRegion { buffer = destination, size = byteCount };
            SDL_UploadToGPUBuffer(copy, &sourceLocation, &region, cycle: true);
            SDL_EndGPUCopyPass(copy);
            if (!SDL_SubmitGPUCommandBuffer(command))
                throw new InvalidOperationException($"SDL GPU {label} upload submission failed: {SDL_GetError()}");
        }

        private void UploadBuffer<T>(SDL_GPUBuffer* destination, T[] values) where T : unmanaged
        {
            uint byteCount = checked((uint)(values.Length * sizeof(T)));
            SDL_GPUTransferBuffer* transfer = CreateTransfer(SDL_GPUTransferBufferUsage.SDL_GPU_TRANSFERBUFFERUSAGE_UPLOAD, byteCount);
            try
            {
                IntPtr mapped = SDL_MapGPUTransferBuffer(device, transfer, cycle: false);
                if (mapped == IntPtr.Zero)
                    throw new InvalidOperationException($"SDL GPU upload mapping failed: {SDL_GetError()}");
                fixed (T* source = values)
                    Buffer.MemoryCopy(source, (void*)mapped, byteCount, byteCount);
                SDL_UnmapGPUTransferBuffer(device, transfer);

                SDL_GPUCommandBuffer* command = AcquireCommand();
                SDL_GPUCopyPass* copy = SDL_BeginGPUCopyPass(command);
                if (copy is null)
                    throw new InvalidOperationException($"SDL GPU upload copy pass failed: {SDL_GetError()}");
                var sourceLocation = new SDL_GPUTransferBufferLocation { transfer_buffer = transfer };
                var region = new SDL_GPUBufferRegion { buffer = destination, size = byteCount };
                SDL_UploadToGPUBuffer(copy, &sourceLocation, &region, cycle: false);
                SDL_EndGPUCopyPass(copy);
                if (!SDL_SubmitGPUCommandBuffer(command))
                    throw new InvalidOperationException($"SDL GPU upload submission failed: {SDL_GetError()}");
            }
            finally
            {
                SDL_ReleaseGPUTransferBuffer(device, transfer);
            }
        }

        private SDL_GPUCommandBuffer* AcquireCommand()
        {
            SDL_GPUCommandBuffer* command = SDL_AcquireGPUCommandBuffer(device);
            if (command is null)
                throw new InvalidOperationException($"SDL GPU command acquisition failed: {SDL_GetError()}");
            return command;
        }

        private SDL_GPUBuffer* CreateBuffer(SDL_GPUBufferUsageFlags usage, uint size)
        {
            var info = new SDL_GPUBufferCreateInfo { usage = usage, size = size };
            SDL_GPUBuffer* buffer = SDL_CreateGPUBuffer(device, &info);
            if (buffer is null)
                throw new InvalidOperationException($"SDL GPU buffer creation failed: {SDL_GetError()}");
            return buffer;
        }

        private SDL_GPUTransferBuffer* CreateTransfer(SDL_GPUTransferBufferUsage usage, uint size)
        {
            var info = new SDL_GPUTransferBufferCreateInfo { usage = usage, size = size };
            SDL_GPUTransferBuffer* transfer = SDL_CreateGPUTransferBuffer(device, &info);
            if (transfer is null)
                throw new InvalidOperationException($"SDL GPU transfer creation failed: {SDL_GetError()}");
            return transfer;
        }

        private SDL_GPUTexture* CreateTexture(SDL_GPUTextureFormat format, SDL_GPUTextureUsageFlags usage, uint textureWidth, uint textureHeight)
        {
            var info = new SDL_GPUTextureCreateInfo
            {
                type = SDL_GPUTextureType.SDL_GPU_TEXTURETYPE_2D,
                format = format,
                usage = usage,
                width = textureWidth,
                height = textureHeight,
                layer_count_or_depth = 1,
                num_levels = 1,
                sample_count = SDL_GPUSampleCount.SDL_GPU_SAMPLECOUNT_1,
            };
            SDL_GPUTexture* texture = SDL_CreateGPUTexture(device, &info);
            if (texture is null)
                throw new InvalidOperationException($"SDL GPU texture creation failed: {SDL_GetError()}");
            return texture;
        }

        private void EnsureVisibleDepth(uint textureWidth, uint textureHeight)
        {
            if (visibleDepth is not null && visibleDepthWidth == textureWidth && visibleDepthHeight == textureHeight)
                return;
            ReleaseTexture(ref visibleDepth);
            visibleDepthWidth = textureWidth;
            visibleDepthHeight = textureHeight;
            visibleDepth = CreateTexture(DepthFormat, SDL_GPUTextureUsageFlags.SDL_GPU_TEXTUREUSAGE_DEPTH_STENCIL_TARGET, textureWidth, textureHeight);
        }

        private void ReleaseBuffer(ref SDL_GPUBuffer* buffer)
        {
            if (buffer is not null && device is not null)
                SDL_ReleaseGPUBuffer(device, buffer);
            buffer = null;
        }

        private void ReleaseTexture(ref SDL_GPUTexture* texture)
        {
            if (texture is not null && device is not null)
                SDL_ReleaseGPUTexture(device, texture);
            texture = null;
        }

        private static void NormalizeToRgba(byte[] pixels, SDL_GPUTextureFormat format)
        {
            if (format is SDL_GPUTextureFormat.SDL_GPU_TEXTUREFORMAT_R8G8B8A8_UNORM or
                SDL_GPUTextureFormat.SDL_GPU_TEXTUREFORMAT_R8G8B8A8_UNORM_SRGB)
                return;
            if (format is not (SDL_GPUTextureFormat.SDL_GPU_TEXTUREFORMAT_B8G8R8A8_UNORM or
                SDL_GPUTextureFormat.SDL_GPU_TEXTUREFORMAT_B8G8R8A8_UNORM_SRGB))
                throw new NotSupportedException($"GPU readback format {format} is not supported by this experiment.");
            for (int offset = 0; offset < pixels.Length; offset += 4)
                (pixels[offset], pixels[offset + 2]) = (pixels[offset + 2], pixels[offset]);
        }
    }
}
