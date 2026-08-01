using System.Numerics;

namespace ChronoFall.CharacterExperiment.SdlGpu;

internal readonly record struct BindPoseCamera(
    Vector3 Position,
    Vector3 Target,
    Matrix4x4 ViewProjection,
    Matrix4x4 TransposedViewProjection)
{
    internal static BindPoseCamera Create(MeshBounds bounds, int width, int height)
    {
        if (width <= 0)
            throw new ArgumentOutOfRangeException(nameof(width));
        if (height <= 0)
            throw new ArgumentOutOfRangeException(nameof(height));
        if (!float.IsFinite(bounds.Radius) || bounds.Radius <= 1e-5f)
            throw new ArgumentException("Mesh bounds must have a positive finite radius.", nameof(bounds));

        const float fieldOfView = MathF.PI / 5.0f;
        Vector3 direction = Vector3.Normalize(new Vector3(1.35f, 0.45f, 2.1f));
        float limitingHalfExtent = MathF.Max(bounds.Extents.Y, bounds.Radius * 0.78f);
        float distance = limitingHalfExtent / MathF.Tan(fieldOfView * 0.5f) * 1.18f;
        Vector3 position = bounds.Center + direction * distance;
        Matrix4x4 view = Matrix4x4.CreateLookAt(position, bounds.Center, Vector3.UnitY);
        Matrix4x4 projection = Matrix4x4.CreatePerspectiveFieldOfView(
            fieldOfView,
            width / (float)height,
            MathF.Max(0.01f, distance - bounds.Radius * 2.0f),
            distance + bounds.Radius * 3.0f);
        Matrix4x4 viewProjection = view * projection;

        return new BindPoseCamera(position, bounds.Center, viewProjection, Matrix4x4.Transpose(viewProjection));
    }
}
