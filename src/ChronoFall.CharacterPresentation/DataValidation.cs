using System.Numerics;

namespace ChronoFall.CharacterPresentation;

internal static class DataValidation
{
    public static bool IsFinite(Vector2 value) =>
        float.IsFinite(value.X) &&
        float.IsFinite(value.Y);

    public static bool IsFinite(Vector3 value) =>
        float.IsFinite(value.X) &&
        float.IsFinite(value.Y) &&
        float.IsFinite(value.Z);

    public static bool IsFinite(Vector4 value) =>
        float.IsFinite(value.X) &&
        float.IsFinite(value.Y) &&
        float.IsFinite(value.Z) &&
        float.IsFinite(value.W);

    public static bool IsFinite(Quaternion value) =>
        float.IsFinite(value.X) &&
        float.IsFinite(value.Y) &&
        float.IsFinite(value.Z) &&
        float.IsFinite(value.W);

    public static bool IsFinite(Matrix4x4 value) =>
        float.IsFinite(value.M11) && float.IsFinite(value.M12) &&
        float.IsFinite(value.M13) && float.IsFinite(value.M14) &&
        float.IsFinite(value.M21) && float.IsFinite(value.M22) &&
        float.IsFinite(value.M23) && float.IsFinite(value.M24) &&
        float.IsFinite(value.M31) && float.IsFinite(value.M32) &&
        float.IsFinite(value.M33) && float.IsFinite(value.M34) &&
        float.IsFinite(value.M41) && float.IsFinite(value.M42) &&
        float.IsFinite(value.M43) && float.IsFinite(value.M44);

    public static Quaternion NormalizeRotation(Quaternion rotation, string parameterName)
    {
        if (!IsFinite(rotation))
            throw new ArgumentException("Rotation must contain only finite values.", parameterName);

        float lengthSquared = rotation.LengthSquared();
        if (!float.IsFinite(lengthSquared) || lengthSquared <= 1e-12f)
            throw new ArgumentException("Rotation must have non-zero finite length.", parameterName);

        return Quaternion.Normalize(rotation);
    }

    public static void RequireNormalizedRotation(Quaternion rotation, string parameterName)
    {
        if (!IsFinite(rotation))
            throw new ArgumentException("Rotation must contain only finite values.", parameterName);

        float lengthSquared = rotation.LengthSquared();
        if (!float.IsFinite(lengthSquared) || MathF.Abs(lengthSquared - 1.0f) > 1e-4f)
            throw new ArgumentException("Rotation must have finite unit length.", parameterName);
    }

    public static void RequireFinite(Matrix4x4 matrix, string parameterName, string label)
    {
        if (!IsFinite(matrix))
            throw new ArgumentException($"{label} must contain only finite values.", parameterName);
    }
}
