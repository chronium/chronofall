using System.Globalization;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using SDL;
using static SDL.SDL3;

namespace ChronoFall.CharacterExperiment.SdlGpu;

internal sealed record StaticMeshHarnessOptions(
    int Width = 512,
    int Height = 512,
    bool Visible = false,
    string? CapturePath = null,
    StaticMeshDefinition? Mesh = null);

internal sealed record StaticMeshHarnessResult(
    SDL_GPUShaderFormat ShaderFormat,
    ulong BaselineFingerprint,
    ulong TransformedFingerprint,
    ulong RepeatedFingerprint,
    int BaselinePixels,
    int FirstSectionPixels,
    int SecondSectionPixels,
    float BaselineCentroidX,
    float TransformedCentroidX);

internal static class SdlGpuStaticMeshHarness
{
    private static readonly SDL_FColor ClearColor = new() { r = 0.035f, g = 0.045f, b = 0.070f, a = 1.0f };
    private static readonly Vector3 FirstSectionColor = new(0.95f, 0.28f, 0.18f);
    private static readonly Vector3 SecondSectionColor = new(0.16f, 0.62f, 0.98f);
    private static readonly Vector3 LightDirection = new(-0.35f, -0.70f, -0.62f);

    internal static StaticMeshHarnessResult Run(StaticMeshHarnessOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        if (options.Width < 64 || options.Height < 64)
            throw new ArgumentOutOfRangeException(nameof(options), "The static GPU harness target must be at least 64x64.");

        StaticMeshDefinition mesh = options.Mesh ?? CreateDiagnosticMesh();
        Matrix4x4 viewProjection = CreateViewProjection(options.Width, options.Height);
        Matrix4x4 baselineWorld = Matrix4x4.CreateRotationY(-0.18f);
        Matrix4x4 transformedWorld =
            Matrix4x4.CreateScale(0.82f) *
            Matrix4x4.CreateRotationY(0.28f) *
            Matrix4x4.CreateTranslation(0.65f, 0.0f, 0.0f);

        using var gpu = new StaticMeshGpuSession(mesh, options.Width, options.Height, options.Visible);
        byte[] baseline = gpu.RenderOffscreen(baselineWorld, viewProjection);
        byte[] transformed = gpu.RenderOffscreen(transformedWorld, viewProjection);
        byte[] repeated = gpu.RenderOffscreen(baselineWorld, viewProjection);

        StaticFrameAnalysis baselineAnalysis = Analyze(baseline, options.Width, options.Height, "baseline");
        StaticFrameAnalysis transformedAnalysis = Analyze(transformed, options.Width, options.Height, "transformed");
        ulong baselineFingerprint = Fingerprint(baseline);
        ulong transformedFingerprint = Fingerprint(transformed);
        ulong repeatedFingerprint = Fingerprint(repeated);

        Require(baselineFingerprint == repeatedFingerprint, "Repeated static render did not reproduce the baseline fingerprint.");
        Require(baselineFingerprint != transformedFingerprint, "Transformed static render reproduced the baseline fingerprint.");
        Require(baselineAnalysis.RenderedPixels >= 1000, "Static baseline rendered too few diagnostic pixels.");
        Require(baselineAnalysis.FirstSectionPixels >= 250, "Static baseline did not visibly render its first section.");
        Require(baselineAnalysis.SecondSectionPixels >= 250, "Static baseline did not visibly render its second section.");
        Require(
            MathF.Abs(transformedAnalysis.CentroidX - baselineAnalysis.CentroidX) >= options.Width * 0.05f,
            $"Static transformed probe shifted the centroid by only {MathF.Abs(transformedAnalysis.CentroidX - baselineAnalysis.CentroidX):F2} pixels.");

        if (!string.IsNullOrWhiteSpace(options.CapturePath))
            WritePpm(options.CapturePath, options.Width, options.Height, baseline);
        if (options.Visible)
        {
            Console.WriteLine("GPU_STATIC_HARNESS_VISIBLE Close the window or press Escape after inspecting both lit sections.");
            gpu.RunVisible(baselineWorld, viewProjection);
        }

        Console.WriteLine(string.Create(
            CultureInfo.InvariantCulture,
            $"GPU_STATIC_HARNESS_PASS pixels={baselineAnalysis.RenderedPixels} " +
            $"sections={baselineAnalysis.FirstSectionPixels}/{baselineAnalysis.SecondSectionPixels} " +
            $"centroid={baselineAnalysis.CentroidX:F2}->{transformedAnalysis.CentroidX:F2} " +
            $"fingerprints={baselineFingerprint:x16}/{transformedFingerprint:x16}/{repeatedFingerprint:x16}"));
        return new StaticMeshHarnessResult(
            gpu.ShaderFormat,
            baselineFingerprint,
            transformedFingerprint,
            repeatedFingerprint,
            baselineAnalysis.RenderedPixels,
            baselineAnalysis.FirstSectionPixels,
            baselineAnalysis.SecondSectionPixels,
            baselineAnalysis.CentroidX,
            transformedAnalysis.CentroidX);
    }

    internal static StaticMeshDefinition CreateDiagnosticMesh()
    {
        var vertices = new List<StaticVertex>();
        var indices = new List<uint>();
        AddBox(vertices, indices, new Vector3(-1.25f, -0.55f, -0.42f), new Vector3(-0.12f, 0.52f, 0.42f));
        int firstSectionIndexCount = indices.Count;
        AddBox(vertices, indices, new Vector3(0.12f, -0.55f, -0.32f), new Vector3(1.22f, 0.92f, 0.32f));
        return new StaticMeshDefinition(
            "static-two-section-diagnostic",
            vertices,
            indices,
            [
                new StaticMeshSection("diagnostic-orange", 0, firstSectionIndexCount),
                new StaticMeshSection("diagnostic-blue", firstSectionIndexCount, indices.Count - firstSectionIndexCount),
            ]);
    }

    private static Matrix4x4 CreateViewProjection(int width, int height)
    {
        Matrix4x4 view = Matrix4x4.CreateLookAt(
            new Vector3(3.0f, 2.35f, 4.2f),
            new Vector3(0.0f, 0.12f, 0.0f),
            Vector3.UnitY);
        Matrix4x4 projection = Matrix4x4.CreatePerspectiveFieldOfView(
            MathF.PI / 4.0f,
            width / (float)height,
            0.1f,
            100.0f);
        return view * projection;
    }

    private static void AddBox(
        List<StaticVertex> vertices,
        List<uint> indices,
        Vector3 minimum,
        Vector3 maximum)
    {
        AddQuad(vertices, indices, new(minimum.X, minimum.Y, maximum.Z), new(maximum.X, minimum.Y, maximum.Z), new(maximum.X, maximum.Y, maximum.Z), new(minimum.X, maximum.Y, maximum.Z), Vector3.UnitZ);
        AddQuad(vertices, indices, new(maximum.X, minimum.Y, minimum.Z), new(minimum.X, minimum.Y, minimum.Z), new(minimum.X, maximum.Y, minimum.Z), new(maximum.X, maximum.Y, minimum.Z), -Vector3.UnitZ);
        AddQuad(vertices, indices, new(maximum.X, minimum.Y, minimum.Z), new(maximum.X, maximum.Y, minimum.Z), new(maximum.X, maximum.Y, maximum.Z), new(maximum.X, minimum.Y, maximum.Z), Vector3.UnitX);
        AddQuad(vertices, indices, new(minimum.X, minimum.Y, maximum.Z), new(minimum.X, maximum.Y, maximum.Z), new(minimum.X, maximum.Y, minimum.Z), new(minimum.X, minimum.Y, minimum.Z), -Vector3.UnitX);
        AddQuad(vertices, indices, new(minimum.X, maximum.Y, maximum.Z), new(maximum.X, maximum.Y, maximum.Z), new(maximum.X, maximum.Y, minimum.Z), new(minimum.X, maximum.Y, minimum.Z), Vector3.UnitY);
        AddQuad(vertices, indices, new(minimum.X, minimum.Y, minimum.Z), new(maximum.X, minimum.Y, minimum.Z), new(maximum.X, minimum.Y, maximum.Z), new(minimum.X, minimum.Y, maximum.Z), -Vector3.UnitY);
    }

    private static void AddQuad(
        List<StaticVertex> vertices,
        List<uint> indices,
        Vector3 first,
        Vector3 second,
        Vector3 third,
        Vector3 fourth,
        Vector3 normal)
    {
        uint start = checked((uint)vertices.Count);
        vertices.Add(new StaticVertex(first, normal));
        vertices.Add(new StaticVertex(second, normal));
        vertices.Add(new StaticVertex(third, normal));
        vertices.Add(new StaticVertex(fourth, normal));
        indices.AddRange([start, start + 1, start + 2, start, start + 2, start + 3]);
    }

    private static StaticFrameAnalysis Analyze(byte[] pixels, int width, int height, string label)
    {
        int clearRed = ToByte(ClearColor.r);
        int clearGreen = ToByte(ClearColor.g);
        int clearBlue = ToByte(ClearColor.b);
        int rendered = 0;
        int first = 0;
        int second = 0;
        long sumX = 0;
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int offset = (y * width + x) * 4;
                int red = pixels[offset];
                int green = pixels[offset + 1];
                int blue = pixels[offset + 2];
                if (Math.Abs(red - clearRed) <= 2 && Math.Abs(green - clearGreen) <= 2 && Math.Abs(blue - clearBlue) <= 2)
                    continue;

                rendered++;
                sumX += x;
                if (red >= 40 && red > blue * 1.35f)
                    first++;
                if (blue >= 40 && blue > red * 1.35f)
                    second++;
            }
        }

        Require(rendered > 0, $"Static {label} frame contained no rendered pixels.");
        return new StaticFrameAnalysis(rendered, first, second, sumX / (float)rendered);
    }

    private static ulong Fingerprint(ReadOnlySpan<byte> pixels)
    {
        const ulong offset = 14695981039346656037;
        const ulong prime = 1099511628211;
        ulong hash = offset;
        foreach (byte value in pixels)
        {
            hash ^= value;
            hash *= prime;
        }
        return hash;
    }

    private static void WritePpm(string path, int width, int height, byte[] pixels)
    {
        string fullPath = Path.GetFullPath(path);
        string? directory = Path.GetDirectoryName(fullPath);
        if (!string.IsNullOrEmpty(directory))
            Directory.CreateDirectory(directory);
        byte[] header = Encoding.ASCII.GetBytes($"P6\n{width} {height}\n255\n");
        byte[] rgb = new byte[checked(width * height * 3)];
        for (int source = 0, destination = 0; source < pixels.Length; source += 4)
        {
            rgb[destination++] = pixels[source];
            rgb[destination++] = pixels[source + 1];
            rgb[destination++] = pixels[source + 2];
        }
        using FileStream stream = File.Create(fullPath);
        stream.Write(header);
        stream.Write(rgb);
        Console.WriteLine($"GPU_STATIC_HARNESS_CAPTURE {fullPath}");
    }

    private static byte ToByte(float value) => (byte)MathF.Round(Math.Clamp(value, 0.0f, 1.0f) * byte.MaxValue);

    private static void Require(bool condition, string message)
    {
        if (!condition)
            throw new InvalidOperationException(message);
    }

    private readonly record struct StaticFrameAnalysis(
        int RenderedPixels,
        int FirstSectionPixels,
        int SecondSectionPixels,
        float CentroidX);

    private sealed unsafe class StaticMeshGpuSession : IDisposable
    {
        private const SDL_GPUTextureFormat DepthFormat = SDL_GPUTextureFormat.SDL_GPU_TEXTUREFORMAT_D32_FLOAT;
        private readonly int width;
        private readonly int height;
        private SDL_Window* window;
        private SDL_GPUDevice* device;
        private SdlGpuStaticMeshRenderer? renderer;
        private SdlGpuStaticMesh? mesh;
        private SDL_GPUTexture* offscreenColor;
        private SDL_GPUTexture* offscreenDepth;
        private SDL_GPUTexture* visibleDepth;
        private uint visibleDepthWidth;
        private uint visibleDepthHeight;
        private bool windowClaimed;

        internal StaticMeshGpuSession(StaticMeshDefinition source, int width, int height, bool visible)
        {
            ArgumentNullException.ThrowIfNull(source);
            this.width = width;
            this.height = height;
            if (!SDL_Init(SDL_InitFlags.SDL_INIT_VIDEO))
                throw new InvalidOperationException($"SDL video initialization failed: {SDL_GetError()}");

            try
            {
                window = SDL_CreateWindow(
                    "ChronoFall static mesh experiment",
                    width,
                    height,
                    SdlGpuCharacterHarness.SelectWindowFlags(visible));
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
                renderer = new SdlGpuStaticMeshRenderer(device, colorFormat, DepthFormat, LoadShaders());
                SDL_GPUCommandBuffer* command = AcquireCommand();
                try
                {
                    mesh = renderer.UploadMesh(command, source);
                    if (!SDL_SubmitGPUCommandBuffer(command))
                        throw new InvalidOperationException($"SDL GPU static geometry submission failed: {SDL_GetError()}");
                    command = null;
                }
                finally
                {
                    if (command is not null)
                        _ = SDL_CancelGPUCommandBuffer(command);
                }

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

        internal byte[] RenderOffscreen(Matrix4x4 world, Matrix4x4 viewProjection)
        {
            uint byteCount = checked((uint)(width * height * 4));
            SDL_GPUTransferBuffer* transfer = CreateTransfer(SDL_GPUTransferBufferUsage.SDL_GPU_TRANSFERBUFFERUSAGE_DOWNLOAD, byteCount);
            SDL_GPUFence* fence = null;
            SDL_GPUCommandBuffer* command = AcquireCommand();
            try
            {
                Render(command, offscreenColor, offscreenDepth, world, viewProjection);
                SDL_GPUCopyPass* copy = SDL_BeginGPUCopyPass(command);
                if (copy is null)
                    throw new InvalidOperationException($"SDL GPU static readback copy pass failed: {SDL_GetError()}");
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
                    throw new InvalidOperationException($"SDL GPU static readback submission failed: {SDL_GetError()}");
                command = null;

                SDL_GPUFence* fenceValue = fence;
                if (!SDL_WaitForGPUFences(device, wait_all: true, &fenceValue, 1))
                    throw new InvalidOperationException($"SDL GPU static fence wait failed: {SDL_GetError()}");
                IntPtr mapped = SDL_MapGPUTransferBuffer(device, transfer, cycle: false);
                if (mapped == IntPtr.Zero)
                    throw new InvalidOperationException($"SDL GPU static readback mapping failed: {SDL_GetError()}");
                try
                {
                    byte[] pixels = new byte[byteCount];
                    Marshal.Copy(mapped, pixels, 0, pixels.Length);
                    NormalizeToRgba(pixels, SDL_GetGPUSwapchainTextureFormat(device, window));
                    return pixels;
                }
                finally
                {
                    SDL_UnmapGPUTransferBuffer(device, transfer);
                }
            }
            finally
            {
                if (command is not null)
                    _ = SDL_CancelGPUCommandBuffer(command);
                if (fence is not null)
                    SDL_ReleaseGPUFence(device, fence);
                SDL_ReleaseGPUTransferBuffer(device, transfer);
            }
        }

        internal void RunVisible(Matrix4x4 world, Matrix4x4 viewProjection)
        {
            if (!SDL_ShowWindow(window))
                throw new InvalidOperationException($"SDL could not show the static validation window: {SDL_GetError()}");
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
                    }
                }
                if (!running)
                    break;

                SDL_GPUCommandBuffer* command = AcquireCommand();
                bool swapchainAcquired = false;
                try
                {
                    SDL_GPUTexture* swapchain;
                    uint swapchainWidth;
                    uint swapchainHeight;
                    if (!SDL_WaitAndAcquireGPUSwapchainTexture(command, window, &swapchain, &swapchainWidth, &swapchainHeight))
                        throw new InvalidOperationException($"SDL GPU static swapchain acquisition failed: {SDL_GetError()}");
                    swapchainAcquired = true;
                    if (swapchain is not null)
                    {
                        EnsureVisibleDepth(swapchainWidth, swapchainHeight);
                        Render(command, swapchain, visibleDepth, world, viewProjection);
                    }
                    if (!SDL_SubmitGPUCommandBuffer(command))
                        throw new InvalidOperationException($"SDL GPU static visible submission failed: {SDL_GetError()}");
                    command = null;
                }
                catch
                {
                    if (command is not null && swapchainAcquired)
                    {
                        _ = SDL_SubmitGPUCommandBuffer(command);
                        command = null;
                    }
                    throw;
                }
                finally
                {
                    if (command is not null)
                        _ = SDL_CancelGPUCommandBuffer(command);
                }
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
            mesh?.Dispose();
            mesh = null;
            renderer?.Dispose();
            renderer = null;
            if (device is not null)
            {
                if (windowClaimed && window is not null)
                    SDL_ReleaseWindowFromGPUDevice(device, window);
                SDL_DestroyGPUDevice(device);
            }
            device = null;
            if (window is not null)
                SDL_DestroyWindow(window);
            window = null;
            SDL_Quit();
        }

        private void Render(
            SDL_GPUCommandBuffer* command,
            SDL_GPUTexture* color,
            SDL_GPUTexture* depth,
            Matrix4x4 world,
            Matrix4x4 viewProjection)
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
                throw new InvalidOperationException($"SDL GPU static render pass failed: {SDL_GetError()}");
            renderer!.DrawSection(command, pass, mesh!, 0, new StaticMeshDraw(world, viewProjection, FirstSectionColor, LightDirection));
            renderer.DrawSection(command, pass, mesh!, 1, new StaticMeshDraw(world, viewProjection, SecondSectionColor, LightDirection));
            SDL_EndGPURenderPass(pass);
        }

        private SdlGpuStaticShaderSet LoadShaders()
        {
            string vertexPath = Path.Combine(AppContext.BaseDirectory, "shaders", ShaderAssetSelector.GetFileName("static-mesh.vert", ShaderFormat));
            string fragmentPath = Path.Combine(AppContext.BaseDirectory, "shaders", ShaderAssetSelector.GetFileName("static-mesh.frag", ShaderFormat));
            if (!File.Exists(vertexPath))
                throw new FileNotFoundException($"SDL GPU static shader asset was not found: {vertexPath}", vertexPath);
            if (!File.Exists(fragmentPath))
                throw new FileNotFoundException($"SDL GPU static shader asset was not found: {fragmentPath}", fragmentPath);
            return new SdlGpuStaticShaderSet(
                ShaderFormat,
                File.ReadAllBytes(vertexPath),
                File.ReadAllBytes(fragmentPath),
                ShaderAssetSelector.GetEntrypoint(ShaderFormat));
        }

        private SDL_GPUCommandBuffer* AcquireCommand()
        {
            SDL_GPUCommandBuffer* command = SDL_AcquireGPUCommandBuffer(device);
            if (command is null)
                throw new InvalidOperationException($"SDL GPU command acquisition failed: {SDL_GetError()}");
            return command;
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
