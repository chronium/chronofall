using System.Numerics;

namespace ChronoFall.CharacterPresentation;

internal static class AnimationInterpolationMath
{
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
