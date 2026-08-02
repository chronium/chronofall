using System.Numerics;

namespace ChronoFall.CharacterPresentation;

public sealed class WeaponGripDefinition
{
    public WeaponGripDefinition(
        JointTransform primaryGripLocalTransform,
        JointTransform? offHandTargetLocalTransform = null)
    {
        ValidateRigidLocalTransform(
            primaryGripLocalTransform,
            nameof(primaryGripLocalTransform),
            "Primary grip");
        if (offHandTargetLocalTransform is JointTransform offHandTarget)
        {
            ValidateRigidLocalTransform(
                offHandTarget,
                nameof(offHandTargetLocalTransform),
                "Off-hand target");
        }

        PrimaryGripLocalTransform = primaryGripLocalTransform;
        OffHandTargetLocalTransform = offHandTargetLocalTransform;
    }

    public JointTransform PrimaryGripLocalTransform { get; }

    public JointTransform? OffHandTargetLocalTransform { get; }

    private static void ValidateRigidLocalTransform(
        JointTransform transform,
        string parameterName,
        string label)
    {
        transform.Validate(parameterName);
        if (transform.Scale != Vector3.One)
        {
            throw new ArgumentException(
                $"{label} must be a rigid frame with identity local scale.",
                parameterName);
        }
    }
}

public sealed class WeaponGripPlacement
{
    public WeaponGripPlacement(
        WeaponGripDefinition definition,
        Matrix4x4 weaponModelTransform,
        Matrix4x4? offHandTargetModelTransform)
    {
        ArgumentNullException.ThrowIfNull(definition);
        DataValidation.RequireFinite(
            weaponModelTransform,
            nameof(weaponModelTransform),
            "Weapon model transform");

        if (definition.OffHandTargetLocalTransform.HasValue != offHandTargetModelTransform.HasValue)
        {
            throw new ArgumentException(
                "Off-hand target placement must match the grip definition.",
                nameof(offHandTargetModelTransform));
        }

        if (offHandTargetModelTransform is Matrix4x4 offHandTarget)
        {
            DataValidation.RequireFinite(
                offHandTarget,
                nameof(offHandTargetModelTransform),
                "Off-hand target model transform");
        }

        Definition = definition;
        WeaponModelTransform = weaponModelTransform;
        OffHandTargetModelTransform = offHandTargetModelTransform;
    }

    public WeaponGripDefinition Definition { get; }

    public Matrix4x4 WeaponModelTransform { get; }

    public Matrix4x4? OffHandTargetModelTransform { get; }
}

public static class WeaponGripEvaluator
{
    public static WeaponGripPlacement EvaluateModelSpace(
        WeaponGripDefinition definition,
        Matrix4x4 primarySocketModelTransform)
    {
        ArgumentNullException.ThrowIfNull(definition);
        DataValidation.RequireFinite(
            primarySocketModelTransform,
            nameof(primarySocketModelTransform),
            "Primary socket model transform");

        Matrix4x4 primaryGripLocal = definition.PrimaryGripLocalTransform.ToMatrix();
        if (!Matrix4x4.Invert(primaryGripLocal, out Matrix4x4 inversePrimaryGripLocal))
        {
            throw new InvalidOperationException(
                "The validated rigid primary grip transform could not be inverted.");
        }

        Matrix4x4 weaponModelTransform = inversePrimaryGripLocal * primarySocketModelTransform;
        Matrix4x4? offHandTargetModelTransform =
            definition.OffHandTargetLocalTransform is JointTransform offHandTarget
                ? offHandTarget.ToMatrix() * weaponModelTransform
                : null;

        return new WeaponGripPlacement(
            definition,
            weaponModelTransform,
            offHandTargetModelTransform);
    }
}
