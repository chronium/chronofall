using System.Numerics;
using System.Runtime.InteropServices;
using SDL;

namespace ChronoFall.CharacterPresentation.SdlGpu.Tests;

public sealed class GpuSkinningContractTests
{
    [Fact]
    public void VertexAbiHasTheReviewedInternalLayout()
    {
        Assert.Equal(48, Marshal.SizeOf<GpuSkinnedVertex>());
        Assert.Equal(0, Marshal.OffsetOf<GpuSkinnedVertex>(nameof(GpuSkinnedVertex.Position)).ToInt32());
        Assert.Equal(12, Marshal.OffsetOf<GpuSkinnedVertex>(nameof(GpuSkinnedVertex.Normal)).ToInt32());
        Assert.Equal(24, Marshal.OffsetOf<GpuSkinnedVertex>(nameof(GpuSkinnedVertex.Joint0)).ToInt32());
        Assert.Equal(32, Marshal.OffsetOf<GpuSkinnedVertex>(nameof(GpuSkinnedVertex.Weights)).ToInt32());
    }

    [Fact]
    public void ConversionPreservesFourInfluencesAndSections()
    {
        SkinnedMeshDefinition mesh = CreateMesh();

        GpuSkinnedVertex[] vertices = GpuSkinningData.CreateVertices(mesh);
        GpuMeshSection[] sections = GpuSkinningData.CreateSections(mesh);

        Assert.Equal((ushort)0, vertices[0].Joint0);
        Assert.Equal((ushort)1, vertices[0].Joint1);
        Assert.Equal(new Vector4(0.75f, 0.25f, 0.0f, 0.0f), vertices[0].Weights);
        Assert.Equal(new GpuMeshSection(0, 3), Assert.Single(sections));
    }

    [Fact]
    public void PalettePackingOwnsTheSingleGpuTranspose()
    {
        SkeletonDefinition skeleton = CreateSkeleton();
        var skin = new SkinDefinition(skeleton, [Matrix4x4.Identity, Matrix4x4.Identity]);
        Matrix4x4 first = Matrix4x4.CreateScale(2.0f) * Matrix4x4.CreateTranslation(1.0f, 2.0f, 3.0f);
        Matrix4x4 second = Matrix4x4.CreateRotationY(0.5f);

        Matrix4x4[] packed = GpuSkinningData.PackPalette(new SkinningPalette(skin, [first, second]));

        Assert.Equal(Matrix4x4.Transpose(first), packed[0]);
        Assert.Equal(Matrix4x4.Transpose(second), packed[1]);
        Assert.Equal(128, MemoryMarshal.AsBytes(packed.AsSpan()).Length);
    }

    [Fact]
    public void DrawContractRejectsInvalidTransformsAndAcceptsFiniteLightDirection()
    {
        Assert.Throws<ArgumentException>(() => new SkinnedCharacterDraw(
            new Matrix4x4(),
            Matrix4x4.Identity,
            Vector4.One,
            Vector3.UnitY));
        Assert.Throws<ArgumentException>(() => new SkinnedCharacterDraw(
            Matrix4x4.Identity,
            Matrix4x4.Identity,
            Vector4.One,
            Vector3.Zero));

        var draw = new SkinnedCharacterDraw(
            Matrix4x4.CreateTranslation(1.0f, 2.0f, 3.0f),
            Matrix4x4.Identity,
            Vector4.One,
            new Vector3(0.0f, -2.0f, 0.0f));

        Assert.Equal(new Vector3(0.0f, -2.0f, 0.0f), draw.LightDirection);
    }

    [Fact]
    public void ShaderSetAcceptsOnlyProvenFormatsAndNonEmptyBytecode()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new SdlGpuSkinnedShaderSet(
            SDL_GPUShaderFormat.SDL_GPU_SHADERFORMAT_DXIL,
            new byte[] { 1 },
            new byte[] { 2 },
            "main"));
        Assert.Throws<ArgumentException>(() => new SdlGpuSkinnedShaderSet(
            SDL_GPUShaderFormat.SDL_GPU_SHADERFORMAT_MSL,
            ReadOnlyMemory<byte>.Empty,
            new byte[] { 2 },
            "main0"));

        var shaders = new SdlGpuSkinnedShaderSet(
            SDL_GPUShaderFormat.SDL_GPU_SHADERFORMAT_SPIRV,
            new byte[] { 1 },
            new byte[] { 2 },
            "main");

        Assert.Equal("main", shaders.EntryPoint);
    }

    [Fact]
    public void ShaderSetCopiesCallerOwnedBytecode()
    {
        byte[] vertex = [1];
        byte[] fragment = [2];
        var shaders = new SdlGpuSkinnedShaderSet(
            SDL_GPUShaderFormat.SDL_GPU_SHADERFORMAT_MSL,
            vertex,
            fragment,
            "main0");

        vertex[0] = 9;
        fragment[0] = 9;

        Assert.Equal((byte)1, shaders.VertexShader.Span[0]);
        Assert.Equal((byte)2, shaders.FragmentShader.Span[0]);
    }

    private static SkinnedMeshDefinition CreateMesh()
    {
        SkeletonDefinition skeleton = CreateSkeleton();
        var skin = new SkinDefinition(skeleton, [Matrix4x4.Identity, Matrix4x4.Identity]);
        var influences = new SkinInfluences(
            new JointIndices4(0, 1, 0, 0),
            new Vector4(0.75f, 0.25f, 0.0f, 0.0f));
        return new SkinnedMeshDefinition(
            "triangle",
            skin,
            [
                new SkinnedVertex(Vector3.Zero, Vector3.UnitZ, Vector2.Zero, influences),
                new SkinnedVertex(Vector3.UnitX, Vector3.UnitZ, Vector2.UnitX, influences),
                new SkinnedVertex(Vector3.UnitY, Vector3.UnitZ, Vector2.UnitY, influences),
            ],
            [0u, 1u, 2u],
            [new SkinnedMeshSection("diagnostic", 0, 3)]);
    }

    private static SkeletonDefinition CreateSkeleton() => new([
        new SkeletonJoint("root", -1, JointTransform.Identity),
        new SkeletonJoint("child", 0, new JointTransform(Vector3.UnitY, Quaternion.Identity, Vector3.One)),
    ]);
}
