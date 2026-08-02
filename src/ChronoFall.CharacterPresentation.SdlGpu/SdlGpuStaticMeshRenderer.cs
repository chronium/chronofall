using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using SDL;
using static SDL.SDL3;

namespace ChronoFall.CharacterPresentation.SdlGpu;

public sealed unsafe class SdlGpuStaticMeshRenderer : IDisposable
{
    private readonly SDL_GPUDevice* device;
    private SDL_GPUShader* vertexShader;
    private SDL_GPUShader* fragmentShader;
    private SDL_GPUGraphicsPipeline* pipeline;
    private bool disposed;

    public SdlGpuStaticMeshRenderer(
        SDL_GPUDevice* device,
        SDL_GPUTextureFormat colorFormat,
        SDL_GPUTextureFormat depthFormat,
        SdlGpuStaticShaderSet shaders)
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
                SDL_GPUShaderStage.SDL_GPU_SHADERSTAGE_VERTEX);
            fragmentShader = CreateShader(
                shaders.FragmentShader,
                shaders.EntryPoint,
                shaders.Format,
                SDL_GPUShaderStage.SDL_GPU_SHADERSTAGE_FRAGMENT);
            pipeline = CreatePipeline(colorFormat, depthFormat);
        }
        catch
        {
            Dispose();
            throw;
        }
    }

    public SdlGpuStaticMesh UploadMesh(
        SDL_GPUCommandBuffer* commandBuffer,
        StaticMeshDefinition source)
    {
        ThrowIfDisposed();
        if (commandBuffer is null)
            throw new ArgumentNullException(nameof(commandBuffer));
        ArgumentNullException.ThrowIfNull(source);

        GpuStaticVertex[] vertices = GpuStaticMeshData.CreateVertices(source);
        uint[] indices = source.Indices.ToArray();
        GpuMeshSection[] sections = GpuStaticMeshData.CreateSections(source);
        SDL_GPUBuffer* vertexBuffer = null;
        SDL_GPUBuffer* indexBuffer = null;
        try
        {
            vertexBuffer = CreateBuffer(
                SDL_GPUBufferUsageFlags.SDL_GPU_BUFFERUSAGE_VERTEX,
                checked((uint)(vertices.Length * GpuStaticVertex.Stride)));
            indexBuffer = CreateBuffer(
                SDL_GPUBufferUsageFlags.SDL_GPU_BUFFERUSAGE_INDEX,
                checked((uint)(indices.Length * sizeof(uint))));
            UploadBuffer(commandBuffer, vertexBuffer, vertices, "static vertex");
            UploadBuffer(commandBuffer, indexBuffer, indices, "static index");
            return new SdlGpuStaticMesh(this, device, vertexBuffer, indexBuffer, sections);
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

    public void Draw(
        SDL_GPUCommandBuffer* commandBuffer,
        SDL_GPURenderPass* renderPass,
        SdlGpuStaticMesh mesh,
        StaticMeshDraw draw)
    {
        PrepareDraw(commandBuffer, renderPass, mesh, draw);
        foreach (GpuMeshSection section in mesh.Sections)
            SDL_DrawGPUIndexedPrimitives(renderPass, section.IndexCount, 1, section.FirstIndex, 0, 0);
    }

    public void DrawSection(
        SDL_GPUCommandBuffer* commandBuffer,
        SDL_GPURenderPass* renderPass,
        SdlGpuStaticMesh mesh,
        int sectionIndex,
        StaticMeshDraw draw)
    {
        PrepareDraw(commandBuffer, renderPass, mesh, draw);
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

    private void PrepareDraw(
        SDL_GPUCommandBuffer* commandBuffer,
        SDL_GPURenderPass* renderPass,
        SdlGpuStaticMesh mesh,
        StaticMeshDraw draw)
    {
        ThrowIfDisposed();
        if (commandBuffer is null)
            throw new ArgumentNullException(nameof(commandBuffer));
        if (renderPass is null)
            throw new ArgumentNullException(nameof(renderPass));
        ValidateOwner(mesh);
        if (!Matrix4x4.Invert(draw.World, out Matrix4x4 worldInverse))
            throw new ArgumentException("World transform must be invertible.", nameof(draw));

        Matrix4x4 vertexConstants = Matrix4x4.Transpose(draw.World * draw.ViewProjection);
        Vector3 modelSpaceLightDirection = Vector3.TransformNormal(draw.LightDirection, worldInverse);
        var surfaceConstants = new SurfaceConstants(
            new Vector4(draw.BaseColor, 1.0f),
            new Vector4(modelSpaceLightDirection, 0.0f));

        SDL_BindGPUGraphicsPipeline(renderPass, pipeline);
        var vertexBinding = new SDL_GPUBufferBinding { buffer = mesh.VertexBuffer };
        var indexBinding = new SDL_GPUBufferBinding { buffer = mesh.IndexBuffer };
        SDL_BindGPUVertexBuffers(renderPass, 0, &vertexBinding, 1);
        SDL_BindGPUIndexBuffer(renderPass, &indexBinding, SDL_GPUIndexElementSize.SDL_GPU_INDEXELEMENTSIZE_32BIT);
        SDL_PushGPUVertexUniformData(commandBuffer, 0, (IntPtr)(&vertexConstants), (uint)sizeof(Matrix4x4));
        SDL_PushGPUFragmentUniformData(commandBuffer, 0, (IntPtr)(&surfaceConstants), (uint)sizeof(SurfaceConstants));
    }

    private void ValidateOwner(SdlGpuStaticMesh mesh)
    {
        ArgumentNullException.ThrowIfNull(mesh);
        mesh.ThrowIfDisposed();
        if (!ReferenceEquals(mesh.Owner, this))
            throw new ArgumentException("The static mesh belongs to a different renderer.", nameof(mesh));
    }

    private SDL_GPUShader* CreateShader(
        ReadOnlyMemory<byte> bytecode,
        string entryPointValue,
        SDL_GPUShaderFormat format,
        SDL_GPUShaderStage stage)
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
                num_uniform_buffers = 1,
            };
            SDL_GPUShader* shader = SDL_CreateGPUShader(device, &info);
            if (shader is null)
                throw new InvalidOperationException($"SDL GPU static mesh shader creation failed: {SDL_GetError()}");
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
            pitch = GpuStaticVertex.Stride,
            input_rate = SDL_GPUVertexInputRate.SDL_GPU_VERTEXINPUTRATE_VERTEX,
        };
        SDL_GPUVertexAttribute* attributes = stackalloc SDL_GPUVertexAttribute[2];
        attributes[0] = new SDL_GPUVertexAttribute
        {
            location = 0,
            buffer_slot = 0,
            format = SDL_GPUVertexElementFormat.SDL_GPU_VERTEXELEMENTFORMAT_FLOAT3,
            offset = GpuStaticVertex.PositionOffset,
        };
        attributes[1] = new SDL_GPUVertexAttribute
        {
            location = 1,
            buffer_slot = 0,
            format = SDL_GPUVertexElementFormat.SDL_GPU_VERTEXELEMENTFORMAT_FLOAT3,
            offset = GpuStaticVertex.NormalOffset,
        };
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
                num_vertex_attributes = 2,
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
            throw new InvalidOperationException($"SDL GPU static mesh pipeline creation failed: {SDL_GetError()}");
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

public sealed unsafe class SdlGpuStaticMesh : IDisposable
{
    private readonly SDL_GPUDevice* device;
    private SDL_GPUBuffer* vertexBuffer;
    private SDL_GPUBuffer* indexBuffer;

    internal SdlGpuStaticMesh(
        SdlGpuStaticMeshRenderer owner,
        SDL_GPUDevice* device,
        SDL_GPUBuffer* vertexBuffer,
        SDL_GPUBuffer* indexBuffer,
        GpuMeshSection[] sections)
    {
        Owner = owner;
        this.device = device;
        this.vertexBuffer = vertexBuffer;
        this.indexBuffer = indexBuffer;
        Sections = sections;
    }

    public int SectionCount => Sections.Length;

    internal SdlGpuStaticMeshRenderer Owner { get; }

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
