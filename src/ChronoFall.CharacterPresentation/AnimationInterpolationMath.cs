using System.Numerics;

namespace ChronoFall.CharacterPresentation;

internal static class AnimationInterpolationMath
{
    internal static JointTransform InterpolateTransform(
        JointTransform source,
        JointTransform destination,
        float amount,
        string parameterName)
    {
        if (amount == 0.0f)
            return source;
        if (amount == 1.0f)
            return destination;

        return new JointTransform(
            Vector3.Lerp(source.Translation, destination.Translation, amount),
            InterpolateRotation(source.Rotation, destination.Rotation, amount, parameterName),
            Vector3.Lerp(source.Scale, destination.Scale, amount));
    }

    internal static Quaternion InterpolateRotation(
        Quaternion source,
        Quaternion destination,
        float amount,
        string parameterName)
    {
        Quaternion shortestDestination = Quaternion.Dot(source, destination) < 0.0f
            ? new Quaternion(-destination.X, -destination.Y, -destination.Z, -destination.W)
            : destination;
        return DataValidation.NormalizeRotation(
            Quaternion.Slerp(source, shortestDestination, amount),
            parameterName);
    }
}
