using System.Numerics;

namespace ChronoFall.CharacterPresentation.Tests;

public sealed class JointTransformTests
{
    [Fact]
    public void ToMatrixUsesScaleRotationTranslationRowVectorOrder()
    {
        var transform = new JointTransform(
            new Vector3(5.0f, -2.0f, 1.0f),
            Quaternion.CreateFromAxisAngle(Vector3.UnitZ, MathF.PI * 0.5f),
            new Vector3(2.0f, 3.0f, 4.0f));

        Vector3 transformed = Vector3.Transform(Vector3.UnitX, transform.ToMatrix());

        AssertVector(new Vector3(5.0f, 0.0f, 1.0f), transformed);
    }

    [Fact]
    public void ConstructorNormalizesFiniteRotation()
    {
        var transform = new JointTransform(Vector3.Zero, new Quaternion(0.0f, 0.0f, 0.0f, 2.0f), Vector3.One);

        Assert.Equal(1.0f, transform.Rotation.Length(), 5);
    }

    [Fact]
    public void ConstructorRejectsInvalidValues()
    {
        Assert.Throws<ArgumentException>(() => new JointTransform(
            new Vector3(float.NaN, 0.0f, 0.0f),
            Quaternion.Identity,
            Vector3.One));
        Assert.Throws<ArgumentException>(() => new JointTransform(Vector3.Zero, default, Vector3.One));
        Assert.Throws<ArgumentException>(() => new JointTransform(
            Vector3.Zero,
            Quaternion.Identity,
            new Vector3(1.0f, float.PositiveInfinity, 1.0f)));
    }

    private static void AssertVector(Vector3 expected, Vector3 actual)
    {
        Assert.Equal(expected.X, actual.X, 5);
        Assert.Equal(expected.Y, actual.Y, 5);
        Assert.Equal(expected.Z, actual.Z, 5);
    }
}
