using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using SDL;
using static SDL.SDL3;

namespace ChronoFall.CharacterPresentation.SdlGpu;

public sealed unsafe class SdlGpuSkinnedCharacterRenderer : IDisposable
{
    private readonly SDL_GPUDevice* device;
    private SDL_GPUShader* vertexShader;
    private SDL_GPUShader* fragmentShader;
    private SDL_GPUGraphicsPipeline* pipeline;
    private bool disposed;

    public SdlGpuSkinnedCharacterRenderer(
        SDL_GPUDevice* device,
        SDL_GPUTextureFormat colorFormat,
        SDL_GPUTextureFormat depthFormat,
        SdlGpuSkinnedShaderSet shaders)
    {
        if (device is null)
            throw new ArgumentNullException(nameof(device));
        ArgumentNullException.ThrowIfNull(shaders);

        this.device = device;
        try
        {
            vertexShader = CreateShader(
                shaders.VertexShader,
                shaders.EntryPoint,
                shaders.Format,
                SDL_GPUShaderStage.SDL_GPU_SHADERSTAGE_VERTEX,
                storageBuffers: 1,
                uniformBuffers: 1);
            fragmentShader = CreateShader(
                shaders.FragmentShader,
                shaders.EntryPoint,
                shaders.Format,
                SDL_GPUShaderStage.SDL_GPU_SHADERSTAGE_FRAGMENT,
                storageBuffers: 0,
                uniformBuffers: 1);
            pipeline = CreatePipeline(colorFormat, depthFormat);
        }
        catch
        {
            Dispose();
            throw;
        }
    }

    public SdlGpuSkinnedMesh UploadMesh(
        SDL_GPUCommandBuffer* commandBuffer,
        SkinnedMeshDefinition source)
    {
        ThrowIfDisposed();
        if (commandBuffer is null)
            throw new ArgumentNullException(nameof(commandBuffer));
        ArgumentNullException.ThrowIfNull(source);

        GpuSkinnedVertex[] vertices = GpuSkinningData.CreateVertices(source);
        uint[] indices = source.Indices.ToArray();
        GpuMeshSection[] sections = GpuSkinningData.CreateSections(source);
        SDL_GPUBuffer* vertexBuffer = null;
        SDL_GPUBuffer* indexBuffer = null;
        try
        {
            vertexBuffer = CreateBuffer(
                SDL_GPUBufferUsageFlags.SDL_GPU_BUFFERUSAGE_VERTEX,
                checked((uint)(vertices.Length * GpuSkinnedVertex.Stride)));
            indexBuffer = CreateBuffer(
                SDL_GPUBufferUsageFlags.SDL_GPU_BUFFERUSAGE_INDEX,
                checked((uint)(indices.Length * sizeof(uint))));
            UploadBuffer(commandBuffer, vertexBuffer, vertices, "skinned vertex");
            UploadBuffer(commandBuffer, indexBuffer, indices, "skinned index");
            return new SdlGpuSkinnedMesh(
                this,
                device,
                vertexBuffer,
                indexBuffer,
                sections,
                source.Skin.Skeleton.JointCount);
        }
        catch
        {
            if (indexBuffer is not null)
                SDL_ReleaseGPUBuffer(device, indexBuffer);
            if (vertexBuffer is not null)
                SDL_ReleaseGPUBuffer(device, vertexBuffer);
            throw;
        }
    }

    public SdlGpuSkinningPalette CreatePalette(int jointCount)
    {
        ThrowIfDisposed();
        if (jointCount <= 0)
            throw new ArgumentOutOfRangeException(nameof(jointCount));

        uint byteCount = checked((uint)(jointCount * sizeof(Matrix4x4)));
        SDL_GPUBuffer* buffer = CreateBuffer(
            SDL_GPUBufferUsageFlags.SDL_GPU_BUFFERUSAGE_GRAPHICS_STORAGE_READ,
            byteCount);
        SDL_GPUTransferBuffer* transfer = null;
        try
        {
            transfer = CreateTransfer(SDL_GPUTransferBufferUsage.SDL_GPU_TRANSFERBUFFERUSAGE_UPLOAD, byteCount);
            return new SdlGpuSkinningPalette(this, device, buffer, transfer, jointCount);
        }
        catch
        {
            if (transfer is not null)
                SDL_ReleaseGPUTransferBuffer(device, transfer);
            SDL_ReleaseGPUBuffer(device, buffer);
            throw;
        }
    }

    public void UploadPalette(
        SDL_GPUCommandBuffer* commandBuffer,
        SdlGpuSkinningPalette destination,
        SkinningPalette source)
    {
        ThrowIfDisposed();
        if (commandBuffer is null)
            throw new ArgumentNullException(nameof(commandBuffer));
        ValidateOwner(destination);
        ArgumentNullException.ThrowIfNull(source);
        if (source.JointMatrices.Count != destination.JointCount)
        {
            throw new ArgumentException(
                $"Expected {destination.JointCount} palette matrices, received {source.JointMatrices.Count}.",
                nameof(source));
        }

        Matrix4x4[] matrices = GpuSkinningData.PackPalette(source);
        uint byteCount = checked((uint)(matrices.Length * sizeof(Matrix4x4)));
        IntPtr mapped = SDL_MapGPUTransferBuffer(device, destination.TransferBuffer, cycle: true);
        if (mapped == IntPtr.Zero)
            throw new InvalidOperationException($"SDL GPU palette upload mapping failed: {SDL_GetError()}");
        fixed (Matrix4x4* sourcePointer = matrices)
            Buffer.MemoryCopy(sourcePointer, (void*)mapped, byteCount, byteCount);
        SDL_UnmapGPUTransferBuffer(device, destination.TransferBuffer);

        SDL_GPUCopyPass* copyPass = SDL_BeginGPUCopyPass(commandBuffer);
        if (copyPass is null)
            throw new InvalidOperationException($"SDL GPU palette copy pass failed: {SDL_GetError()}");
        var sourceLocation = new SDL_GPUTransferBufferLocation { transfer_buffer = destination.TransferBuffer };
        var destinationRegion = new SDL_GPUBufferRegion
        {
            buffer = destination.Buffer,
            size = byteCount,
        };
        SDL_UploadToGPUBuffer(copyPass, &sourceLocation, &destinationRegion, cycle: true);
        SDL_EndGPUCopyPass(copyPass);
    }

    public void Draw(
        SDL_GPUCommandBuffer* commandBuffer,
        SDL_GPURenderPass* renderPass,
        SdlGpuSkinnedMesh mesh,
        SdlGpuSkinningPalette palette,
        SkinnedCharacterDraw draw)
    {
        PrepareDraw(commandBuffer, renderPass, mesh, palette, draw);
        foreach (GpuMeshSection section in mesh.Sections)
            SDL_DrawGPUIndexedPrimitives(renderPass, section.IndexCount, 1, section.FirstIndex, 0, 0);
    }

    public void DrawSection(
        SDL_GPUCommandBuffer* commandBuffer,
        SDL_GPURenderPass* renderPass,
        SdlGpuSkinnedMesh mesh,
        SdlGpuSkinningPalette palette,
        int sectionIndex,
        SkinnedCharacterDraw draw)
    {
        PrepareDraw(commandBuffer, renderPass, mesh, palette, draw);
        if ((uint)sectionIndex >= (uint)mesh.SectionCount)
            throw new ArgumentOutOfRangeException(nameof(sectionIndex));

        GpuMeshSection section = mesh.Sections[sectionIndex];
        SDL_DrawGPUIndexedPrimitives(renderPass, section.IndexCount, 1, section.FirstIndex, 0, 0);
    }

    public void Dispose()
    {
        if (disposed)
            return;
        disposed = true;
        if (pipeline is not null)
            SDL_ReleaseGPUGraphicsPipeline(device, pipeline);
        pipeline = null;
        if (fragmentShader is not null)
            SDL_ReleaseGPUShader(device, fragmentShader);
        fragmentShader = null;
        if (vertexShader is not null)
            SDL_ReleaseGPUShader(device, vertexShader);
        vertexShader = null;
    }

    private void ValidateOwner(SdlGpuSkinnedMesh mesh)
    {
        ArgumentNullException.ThrowIfNull(mesh);
        mesh.ThrowIfDisposed();
        if (!ReferenceEquals(mesh.Owner, this))
            throw new ArgumentException("The skinned mesh belongs to a different renderer.", nameof(mesh));
    }

    private void PrepareDraw(
        SDL_GPUCommandBuffer* commandBuffer,
        SDL_GPURenderPass* renderPass,
        SdlGpuSkinnedMesh mesh,
        SdlGpuSkinningPalette palette,
        SkinnedCharacterDraw draw)
    {
        ThrowIfDisposed();
        if (commandBuffer is null)
            throw new ArgumentNullException(nameof(commandBuffer));
        if (renderPass is null)
            throw new ArgumentNullException(nameof(renderPass));
        ValidateOwner(mesh);
        ValidateOwner(palette);
        if (mesh.JointCount != palette.JointCount)
        {
            throw new ArgumentException(
                $"Mesh requires {mesh.JointCount} joints, but the palette has {palette.JointCount}.",
                nameof(palette));
        }
        if (!Matrix4x4.Invert(draw.World, out Matrix4x4 worldInverse))
            throw new ArgumentException("World transform must be invertible.", nameof(draw));

        Matrix4x4 vertexConstants = Matrix4x4.Transpose(draw.World * draw.ViewProjection);
        Vector3 modelSpaceLightDirection = Vector3.TransformNormal(draw.LightDirection, worldInverse);
        var surfaceConstants = new SurfaceConstants(
            draw.BaseColor,
            new Vector4(modelSpaceLightDirection, 0.0f));

        SDL_BindGPUGraphicsPipeline(renderPass, pipeline);
        var vertexBinding = new SDL_GPUBufferBinding { buffer = mesh.VertexBuffer };
        var indexBinding = new SDL_GPUBufferBinding { buffer = mesh.IndexBuffer };
        SDL_BindGPUVertexBuffers(renderPass, 0, &vertexBinding, 1);
        SDL_BindGPUIndexBuffer(renderPass, &indexBinding, SDL_GPUIndexElementSize.SDL_GPU_INDEXELEMENTSIZE_32BIT);
        SDL_GPUBuffer* paletteBuffer = palette.Buffer;
        SDL_BindGPUVertexStorageBuffers(renderPass, 0, &paletteBuffer, 1);
        SDL_PushGPUVertexUniformData(commandBuffer, 0, (IntPtr)(&vertexConstants), (uint)sizeof(Matrix4x4));
        SDL_PushGPUFragmentUniformData(commandBuffer, 0, (IntPtr)(&surfaceConstants), (uint)sizeof(SurfaceConstants));
    }

    private void ValidateOwner(SdlGpuSkinningPalette palette)
    {
        ArgumentNullException.ThrowIfNull(palette);
        palette.ThrowIfDisposed();
        if (!ReferenceEquals(palette.Owner, this))
            throw new ArgumentException("The skinning palette belongs to a different renderer.", nameof(palette));
    }

    private SDL_GPUShader* CreateShader(
        ReadOnlyMemory<byte> bytecode,
        string entryPointValue,
        SDL_GPUShaderFormat format,
        SDL_GPUShaderStage stage,
        uint storageBuffers,
        uint uniformBuffers)
    {
        byte[] code = bytecode.ToArray();
        byte[] entryPoint = Encoding.UTF8.GetBytes(entryPointValue + '\0');
        fixed (byte* codePointer = code)
        fixed (byte* entryPointPointer = entryPoint)
        {
            var info = new SDL_GPUShaderCreateInfo
            {
                code_size = (nuint)code.Length,
                code = codePointer,
                entrypoint = entryPointPointer,
                format = format,
                stage = stage,
                num_storage_buffers = storageBuffers,
                num_uniform_buffers = uniformBuffers,
            };
            SDL_GPUShader* shader = SDL_CreateGPUShader(device, &info);
            if (shader is null)
                throw new InvalidOperationException($"SDL GPU skinned character shader creation failed: {SDL_GetError()}");
            return shader;
        }
    }

    private SDL_GPUGraphicsPipeline* CreatePipeline(
        SDL_GPUTextureFormat colorFormat,
        SDL_GPUTextureFormat depthFormat)
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
                depth_stencil_format = depthFormat,
                has_depth_stencil_target = true,
            },
        };
        SDL_GPUGraphicsPipeline* result = SDL_CreateGPUGraphicsPipeline(device, &info);
        if (result is null)
            throw new InvalidOperationException($"SDL GPU skinned character pipeline creation failed: {SDL_GetError()}");
        return result;
    }

    private void UploadBuffer<T>(
        SDL_GPUCommandBuffer* commandBuffer,
        SDL_GPUBuffer* destination,
        T[] values,
        string label)
        where T : unmanaged
    {
        uint byteCount = checked((uint)(values.Length * sizeof(T)));
        SDL_GPUTransferBuffer* transfer = CreateTransfer(
            SDL_GPUTransferBufferUsage.SDL_GPU_TRANSFERBUFFERUSAGE_UPLOAD,
            byteCount);
        try
        {
            IntPtr mapped = SDL_MapGPUTransferBuffer(device, transfer, cycle: false);
            if (mapped == IntPtr.Zero)
                throw new InvalidOperationException($"SDL GPU {label} upload mapping failed: {SDL_GetError()}");
            fixed (T* source = values)
                Buffer.MemoryCopy(source, (void*)mapped, byteCount, byteCount);
            SDL_UnmapGPUTransferBuffer(device, transfer);

            SDL_GPUCopyPass* copyPass = SDL_BeginGPUCopyPass(commandBuffer);
            if (copyPass is null)
                throw new InvalidOperationException($"SDL GPU {label} copy pass failed: {SDL_GetError()}");
            var sourceLocation = new SDL_GPUTransferBufferLocation { transfer_buffer = transfer };
            var destinationRegion = new SDL_GPUBufferRegion { buffer = destination, size = byteCount };
            SDL_UploadToGPUBuffer(copyPass, &sourceLocation, &destinationRegion, cycle: false);
            SDL_EndGPUCopyPass(copyPass);
        }
        finally
        {
            SDL_ReleaseGPUTransferBuffer(device, transfer);
        }
    }

    private SDL_GPUBuffer* CreateBuffer(SDL_GPUBufferUsageFlags usage, uint size)
    {
        var info = new SDL_GPUBufferCreateInfo { usage = usage, size = size };
        SDL_GPUBuffer* result = SDL_CreateGPUBuffer(device, &info);
        if (result is null)
            throw new InvalidOperationException($"SDL GPU buffer creation failed: {SDL_GetError()}");
        return result;
    }

    private SDL_GPUTransferBuffer* CreateTransfer(SDL_GPUTransferBufferUsage usage, uint size)
    {
        var info = new SDL_GPUTransferBufferCreateInfo { usage = usage, size = size };
        SDL_GPUTransferBuffer* result = SDL_CreateGPUTransferBuffer(device, &info);
        if (result is null)
            throw new InvalidOperationException($"SDL GPU transfer buffer creation failed: {SDL_GetError()}");
        return result;
    }

    private void ThrowIfDisposed()
    {
        ObjectDisposedException.ThrowIf(disposed, this);
    }

    [StructLayout(LayoutKind.Sequential)]
    private readonly record struct SurfaceConstants(Vector4 BaseColor, Vector4 LightDirection);
}

public sealed unsafe class SdlGpuSkinnedMesh : IDisposable
{
    private readonly SDL_GPUDevice* device;
    private SDL_GPUBuffer* vertexBuffer;
    private SDL_GPUBuffer* indexBuffer;

    internal SdlGpuSkinnedMesh(
        SdlGpuSkinnedCharacterRenderer owner,
        SDL_GPUDevice* device,
        SDL_GPUBuffer* vertexBuffer,
        SDL_GPUBuffer* indexBuffer,
        GpuMeshSection[] sections,
        int jointCount)
    {
        Owner = owner;
        this.device = device;
        this.vertexBuffer = vertexBuffer;
        this.indexBuffer = indexBuffer;
        Sections = sections;
        JointCount = jointCount;
    }

    public int JointCount { get; }

    public int SectionCount => Sections.Length;

    internal SdlGpuSkinnedCharacterRenderer Owner { get; }

    internal SDL_GPUBuffer* VertexBuffer => vertexBuffer;

    internal SDL_GPUBuffer* IndexBuffer => indexBuffer;

    internal GpuMeshSection[] Sections { get; }

    public void Dispose()
    {
        if (indexBuffer is not null)
            SDL_ReleaseGPUBuffer(device, indexBuffer);
        indexBuffer = null;
        if (vertexBuffer is not null)
            SDL_ReleaseGPUBuffer(device, vertexBuffer);
        vertexBuffer = null;
    }

    internal void ThrowIfDisposed()
    {
        ObjectDisposedException.ThrowIf(vertexBuffer is null || indexBuffer is null, this);
    }
}

public sealed unsafe class SdlGpuSkinningPalette : IDisposable
{
    private readonly SDL_GPUDevice* device;
    private SDL_GPUBuffer* buffer;
    private SDL_GPUTransferBuffer* transferBuffer;

    internal SdlGpuSkinningPalette(
        SdlGpuSkinnedCharacterRenderer owner,
        SDL_GPUDevice* device,
        SDL_GPUBuffer* buffer,
        SDL_GPUTransferBuffer* transferBuffer,
        int jointCount)
    {
        Owner = owner;
        this.device = device;
        this.buffer = buffer;
        this.transferBuffer = transferBuffer;
        JointCount = jointCount;
    }

    public int JointCount { get; }

    internal SdlGpuSkinnedCharacterRenderer Owner { get; }

    internal SDL_GPUBuffer* Buffer => buffer;

    internal SDL_GPUTransferBuffer* TransferBuffer => transferBuffer;

    public void Dispose()
    {
        if (transferBuffer is not null)
            SDL_ReleaseGPUTransferBuffer(device, transferBuffer);
        transferBuffer = null;
        if (buffer is not null)
            SDL_ReleaseGPUBuffer(device, buffer);
        buffer = null;
    }

    internal void ThrowIfDisposed()
    {
        ObjectDisposedException.ThrowIf(buffer is null || transferBuffer is null, this);
    }
}
