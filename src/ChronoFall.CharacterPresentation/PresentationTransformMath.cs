using System.Numerics;

namespace ChronoFall.CharacterPresentation;

internal static class PresentationTransformMath
{
    internal const float DirectionEpsilon = 1e-6f;
    private const float UniformScaleTolerance = 1e-4f;

    internal static Quaternion ExtractNearRigidRotation(
        Matrix4x4 transform,
        string parameterName,
        string label)
    {
        DataValidation.RequireFinite(transform, parameterName, label);
        if (!Matrix4x4.Decompose(transform, out Vector3 scale, out Quaternion rotation, out _))
            throw new ArgumentException($"{label} must be decomposable.", parameterName);

        if (!DataValidation.IsFinite(scale) ||
            scale.X <= DirectionEpsilon ||
            scale.Y <= DirectionEpsilon ||
            scale.Z <= DirectionEpsilon)
        {
            throw new ArgumentException($"{label} must use positive non-zero scale.", parameterName);
        }

        float largestScale = MathF.Max(scale.X, MathF.Max(scale.Y, scale.Z));
        float tolerance = MathF.Max(1.0f, largestScale) * UniformScaleTolerance;
        if (MathF.Abs(scale.X - scale.Y) > tolerance ||
            MathF.Abs(scale.X - scale.Z) > tolerance ||
            MathF.Abs(scale.Y - scale.Z) > tolerance)
        {
            throw new ArgumentException(
                $"{label} must be rigid apart from uniform scale.",
                parameterName);
        }

        return DataValidation.NormalizeRotation(rotation, parameterName);
    }

    internal static Vector3 ExtractTranslation(Matrix4x4 transform) =>
        new(transform.M41, transform.M42, transform.M43);

    internal static Quaternion CreateShortestArcRotation(
        Vector3 sourceDirection,
        Vector3 destinationDirection,
        string parameterName)
    {
        Vector3 source = NormalizeDirection(sourceDirection, parameterName, "Source direction");
        Vector3 destination = NormalizeDirection(destinationDirection, parameterName, "Destination direction");
        float dot = Math.Clamp(Vector3.Dot(source, destination), -1.0f, 1.0f);
        if (dot >= 1.0f - DirectionEpsilon)
            return Quaternion.Identity;

        if (dot <= -1.0f + DirectionEpsilon)
        {
            Vector3 axis = CreateDeterministicPerpendicular(source);
            return Quaternion.CreateFromAxisAngle(axis, MathF.PI);
        }

        Vector3 rotationAxis = Vector3.Normalize(Vector3.Cross(source, destination));
        return DataValidation.NormalizeRotation(
            Quaternion.CreateFromAxisAngle(rotationAxis, MathF.Acos(dot)),
            parameterName);
    }

    internal static Quaternion AppendModelRotation(
        Quaternion currentModelRotation,
        Quaternion modelRotationDelta,
        string parameterName)
    {
        Matrix4x4 combined =
            Matrix4x4.CreateFromQuaternion(currentModelRotation) *
            Matrix4x4.CreateFromQuaternion(modelRotationDelta);
        if (!Matrix4x4.Decompose(combined, out _, out Quaternion rotation, out _))
            throw new InvalidOperationException("The model-space rotation could not be composed.");

        return DataValidation.NormalizeRotation(rotation, parameterName);
    }

    internal static Quaternion ConvertModelToLocalRotation(
        Quaternion desiredModelRotation,
        Quaternion? parentModelRotation,
        string parameterName)
    {
        Matrix4x4 localRotation = Matrix4x4.CreateFromQuaternion(desiredModelRotation);
        if (parentModelRotation is Quaternion parentRotation)
        {
            Matrix4x4 parentMatrix = Matrix4x4.CreateFromQuaternion(parentRotation);
            if (!Matrix4x4.Invert(parentMatrix, out Matrix4x4 inverseParent))
                throw new InvalidOperationException("The validated parent rotation could not be inverted.");
            localRotation *= inverseParent;
        }

        if (!Matrix4x4.Decompose(localRotation, out _, out Quaternion rotation, out _))
            throw new InvalidOperationException("The model-space rotation could not be converted to local space.");

        return DataValidation.NormalizeRotation(rotation, parameterName);
    }

    internal static Vector3 NormalizeDirection(
        Vector3 direction,
        string parameterName,
        string label)
    {
        if (!DataValidation.IsFinite(direction))
            throw new ArgumentException($"{label} must contain only finite values.", parameterName);

        float lengthSquared = direction.LengthSquared();
        if (lengthSquared <= DirectionEpsilon * DirectionEpsilon)
            throw new ArgumentException($"{label} must be non-zero.", parameterName);

        return direction / MathF.Sqrt(lengthSquared);
    }

    internal static Vector3 CreateDeterministicPerpendicular(Vector3 direction)
    {
        Vector3 normalized = Vector3.Normalize(direction);
        Vector3 reference = MathF.Abs(normalized.X) <= MathF.Abs(normalized.Y) &&
                            MathF.Abs(normalized.X) <= MathF.Abs(normalized.Z)
            ? Vector3.UnitX
            : MathF.Abs(normalized.Y) <= MathF.Abs(normalized.Z)
                ? Vector3.UnitY
                : Vector3.UnitZ;
        return Vector3.Normalize(Vector3.Cross(normalized, reference));
    }
}
