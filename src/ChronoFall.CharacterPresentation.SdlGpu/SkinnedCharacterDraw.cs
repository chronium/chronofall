using System.Numerics;

namespace ChronoFall.CharacterPresentation.SdlGpu;

public readonly record struct SkinnedCharacterDraw
{
    public SkinnedCharacterDraw(
        Matrix4x4 world,
        Matrix4x4 viewProjection,
        Vector4 baseColor,
        Vector3 lightDirection)
    {
        if (!MatrixValidation.IsFinite(world))
            throw new ArgumentException("World transform must contain only finite values.", nameof(world));
        if (!Matrix4x4.Invert(world, out _))
            throw new ArgumentException("World transform must be invertible.", nameof(world));
        if (!MatrixValidation.IsFinite(viewProjection))
            throw new ArgumentException("View-projection transform must contain only finite values.", nameof(viewProjection));
        if (!MatrixValidation.IsFinite(baseColor))
            throw new ArgumentException("Base color must contain only finite values.", nameof(baseColor));
        if (!MatrixValidation.IsFinite(lightDirection) || lightDirection.LengthSquared() <= 1e-12f)
            throw new ArgumentException("Light direction must have non-zero finite length.", nameof(lightDirection));

        World = world;
        ViewProjection = viewProjection;
        BaseColor = baseColor;
        LightDirection = lightDirection;
    }

    public Matrix4x4 World { get; }

    public Matrix4x4 ViewProjection { get; }

    public Vector4 BaseColor { get; }

    public Vector3 LightDirection { get; }
}

internal static class MatrixValidation
{
    internal static bool IsFinite(Vector3 value) =>
        float.IsFinite(value.X) && float.IsFinite(value.Y) && float.IsFinite(value.Z);

    internal static bool IsFinite(Vector4 value) =>
        float.IsFinite(value.X) && float.IsFinite(value.Y) &&
        float.IsFinite(value.Z) && float.IsFinite(value.W);

    internal static bool IsFinite(Matrix4x4 value) =>
        float.IsFinite(value.M11) && float.IsFinite(value.M12) &&
        float.IsFinite(value.M13) && float.IsFinite(value.M14) &&
        float.IsFinite(value.M21) && float.IsFinite(value.M22) &&
        float.IsFinite(value.M23) && float.IsFinite(value.M24) &&
        float.IsFinite(value.M31) && float.IsFinite(value.M32) &&
        float.IsFinite(value.M33) && float.IsFinite(value.M34) &&
        float.IsFinite(value.M41) && float.IsFinite(value.M42) &&
        float.IsFinite(value.M43) && float.IsFinite(value.M44);
}
