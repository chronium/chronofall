using System.Numerics;
using System.Runtime.InteropServices;
using SDL;

namespace ChronoFall.CharacterPresentation.SdlGpu.Tests;

public sealed class GpuStaticMeshContractTests
{
    [Fact]
    public void VertexAbiHasTheReviewedInternalLayout()
    {
        Assert.Equal(24, Marshal.SizeOf<GpuStaticVertex>());
        Assert.Equal(0, Marshal.OffsetOf<GpuStaticVertex>(nameof(GpuStaticVertex.Position)).ToInt32());
        Assert.Equal(12, Marshal.OffsetOf<GpuStaticVertex>(nameof(GpuStaticVertex.Normal)).ToInt32());
    }

    [Fact]
    public void ConversionPreservesGeometryAndSections()
    {
        StaticMeshDefinition mesh = CreateMesh();

        GpuStaticVertex[] vertices = GpuStaticMeshData.CreateVertices(mesh);
        GpuMeshSection[] sections = GpuStaticMeshData.CreateSections(mesh);

        Assert.Equal(new Vector3(-1.0f, 0.0f, 0.0f), vertices[0].Position);
        Assert.Equal(Vector3.UnitZ, vertices[0].Normal);
        Assert.Equal(new GpuMeshSection(0, 3), sections[0]);
        Assert.Equal(new GpuMeshSection(3, 3), sections[1]);
    }

    [Fact]
    public void DrawAcceptsRigidPositiveUniformTransforms()
    {
        Matrix4x4 world =
            Matrix4x4.CreateScale(2.0f) *
            Matrix4x4.CreateRotationY(0.4f) *
            Matrix4x4.CreateTranslation(1.0f, 2.0f, 3.0f);

        var draw = new StaticMeshDraw(
            world,
            Matrix4x4.Identity,
            new Vector3(0.2f, 0.4f, 0.8f),
            new Vector3(-0.3f, -0.7f, -0.6f));

        Assert.Equal(world, draw.World);
        Assert.Equal(new Vector3(0.2f, 0.4f, 0.8f), draw.BaseColor);
    }

    [Fact]
    public void DrawRejectsUnsupportedWorldTransforms()
    {
        Assert.Throws<ArgumentException>(() => CreateDraw(Matrix4x4.CreateScale(1.0f, 2.0f, 1.0f)));
        Assert.Throws<ArgumentException>(() => CreateDraw(Matrix4x4.CreateScale(-1.0f, 1.0f, 1.0f)));

        Matrix4x4 shear = Matrix4x4.Identity;
        shear.M12 = 0.25f;
        Assert.Throws<ArgumentException>(() => CreateDraw(shear));
    }

    [Fact]
    public void DrawRejectsMalformedSurfaceInputs()
    {
        Assert.Throws<ArgumentException>(() => new StaticMeshDraw(
            Matrix4x4.Identity,
            Matrix4x4.Identity,
            new Vector3(1.1f, 0.0f, 0.0f),
            Vector3.UnitY));
        Assert.Throws<ArgumentException>(() => new StaticMeshDraw(
            Matrix4x4.Identity,
            Matrix4x4.Identity,
            Vector3.One,
            Vector3.Zero));
    }

    [Fact]
    public void ShaderSetAcceptsOnlyProvenFormatsAndCopiesBytecode()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new SdlGpuStaticShaderSet(
            SDL_GPUShaderFormat.SDL_GPU_SHADERFORMAT_DXIL,
            new byte[] { 1 },
            new byte[] { 2 },
            "main"));
        Assert.Throws<ArgumentException>(() => new SdlGpuStaticShaderSet(
            SDL_GPUShaderFormat.SDL_GPU_SHADERFORMAT_MSL,
            ReadOnlyMemory<byte>.Empty,
            new byte[] { 2 },
            "main0"));

        byte[] vertex = [1];
        byte[] fragment = [2];
        var shaders = new SdlGpuStaticShaderSet(
            SDL_GPUShaderFormat.SDL_GPU_SHADERFORMAT_SPIRV,
            vertex,
            fragment,
            "main");
        vertex[0] = 9;
        fragment[0] = 9;

        Assert.Equal((byte)1, shaders.VertexShader.Span[0]);
        Assert.Equal((byte)2, shaders.FragmentShader.Span[0]);
        Assert.Equal("main", shaders.EntryPoint);
    }

    private static StaticMeshDraw CreateDraw(Matrix4x4 world) => new(
        world,
        Matrix4x4.Identity,
        Vector3.One,
        Vector3.UnitY);

    private static StaticMeshDefinition CreateMesh() => new(
        "two-section-quad",
        [
            new StaticVertex(new Vector3(-1.0f, 0.0f, 0.0f), Vector3.UnitZ),
            new StaticVertex(new Vector3(0.0f, 0.0f, 0.0f), Vector3.UnitZ),
            new StaticVertex(new Vector3(-1.0f, 1.0f, 0.0f), Vector3.UnitZ),
            new StaticVertex(new Vector3(1.0f, 0.0f, 0.0f), Vector3.UnitZ),
            new StaticVertex(new Vector3(1.0f, 1.0f, 0.0f), Vector3.UnitZ),
        ],
        [0, 1, 2, 1, 3, 4],
        [new StaticMeshSection("left", 0, 3), new StaticMeshSection("right", 3, 3)]);
}
