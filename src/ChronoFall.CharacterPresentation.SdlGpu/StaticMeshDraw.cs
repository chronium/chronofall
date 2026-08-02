using System.Numerics;

namespace ChronoFall.CharacterPresentation.SdlGpu;

public readonly record struct StaticMeshDraw
{
    private const float TransformTolerance = 1e-5f;

    public StaticMeshDraw(
        Matrix4x4 world,
        Matrix4x4 viewProjection,
        Vector3 baseColor,
        Vector3 lightDirection)
    {
        ValidateWorld(world);
        if (!MatrixValidation.IsFinite(viewProjection))
            throw new ArgumentException("View-projection transform must contain only finite values.", nameof(viewProjection));
        if (!MatrixValidation.IsFinite(baseColor) || !IsNormalizedColor(baseColor))
            throw new ArgumentException("Base color must contain finite values between zero and one.", nameof(baseColor));
        if (!MatrixValidation.IsFinite(lightDirection) || lightDirection.LengthSquared() <= 1e-12f)
            throw new ArgumentException("Light direction must have non-zero finite length.", nameof(lightDirection));

        World = world;
        ViewProjection = viewProjection;
        BaseColor = baseColor;
        LightDirection = lightDirection;
    }

    public Matrix4x4 World { get; }

    public Matrix4x4 ViewProjection { get; }

    public Vector3 BaseColor { get; }

    public Vector3 LightDirection { get; }

    private static bool IsNormalizedColor(Vector3 value) =>
        value.X is >= 0.0f and <= 1.0f &&
        value.Y is >= 0.0f and <= 1.0f &&
        value.Z is >= 0.0f and <= 1.0f;

    private static void ValidateWorld(Matrix4x4 world)
    {
        if (!MatrixValidation.IsFinite(world))
            throw new ArgumentException("World transform must contain only finite values.", nameof(world));
        if (!Matrix4x4.Decompose(world, out Vector3 scale, out Quaternion rotation, out Vector3 translation) ||
            scale.X <= 0.0f || scale.Y <= 0.0f || scale.Z <= 0.0f)
        {
            throw new ArgumentException(
                "World transform must contain only translation, rotation, and positive uniform scale.",
                nameof(world));
        }

        float scaleTolerance = TransformTolerance * MathF.Max(1.0f, MathF.Max(scale.X, MathF.Max(scale.Y, scale.Z)));
        if (MathF.Abs(scale.X - scale.Y) > scaleTolerance || MathF.Abs(scale.X - scale.Z) > scaleTolerance)
        {
            throw new ArgumentException(
                "World transform must use positive uniform scale; non-uniform scale is not supported.",
                nameof(world));
        }

        Matrix4x4 reconstructed =
            Matrix4x4.CreateScale(scale) *
            Matrix4x4.CreateFromQuaternion(rotation) *
            Matrix4x4.CreateTranslation(translation);
        if (!NearlyEqual(world, reconstructed))
        {
            throw new ArgumentException(
                "World transform cannot contain shear or reflection.",
                nameof(world));
        }
    }

    private static bool NearlyEqual(Matrix4x4 left, Matrix4x4 right)
    {
        ReadOnlySpan<float> leftValues =
        [
            left.M11, left.M12, left.M13, left.M14,
            left.M21, left.M22, left.M23, left.M24,
            left.M31, left.M32, left.M33, left.M34,
            left.M41, left.M42, left.M43, left.M44,
        ];
        ReadOnlySpan<float> rightValues =
        [
            right.M11, right.M12, right.M13, right.M14,
            right.M21, right.M22, right.M23, right.M24,
            right.M31, right.M32, right.M33, right.M34,
            right.M41, right.M42, right.M43, right.M44,
        ];
        for (int index = 0; index < leftValues.Length; index++)
        {
            float tolerance = TransformTolerance * MathF.Max(1.0f, MathF.Max(MathF.Abs(leftValues[index]), MathF.Abs(rightValues[index])));
            if (MathF.Abs(leftValues[index] - rightValues[index]) > tolerance)
                return false;
        }
        return true;
    }
}
