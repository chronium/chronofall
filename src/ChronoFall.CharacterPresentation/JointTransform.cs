using System.Numerics;

namespace ChronoFall.CharacterPresentation;

public readonly record struct JointTransform
{
    public static JointTransform Identity { get; } = new(Vector3.Zero, Quaternion.Identity, Vector3.One);

    public JointTransform(Vector3 translation, Quaternion rotation, Vector3 scale)
    {
        if (!DataValidation.IsFinite(translation))
            throw new ArgumentException("Translation must contain only finite values.", nameof(translation));
        if (!DataValidation.IsFinite(scale))
            throw new ArgumentException("Scale must contain only finite values.", nameof(scale));

        Translation = translation;
        Rotation = DataValidation.NormalizeRotation(rotation, nameof(rotation));
        Scale = scale;
    }

    public Vector3 Translation { get; }

    public Quaternion Rotation { get; }

    public Vector3 Scale { get; }

    public Matrix4x4 ToMatrix() =>
        Matrix4x4.CreateScale(Scale) *
        Matrix4x4.CreateFromQuaternion(Rotation) *
        Matrix4x4.CreateTranslation(Translation);

    internal void Validate(string parameterName)
    {
        if (!DataValidation.IsFinite(Translation) || !DataValidation.IsFinite(Scale))
            throw new ArgumentException("Joint transform must contain only finite values.", parameterName);

        DataValidation.NormalizeRotation(Rotation, parameterName);
    }
}
