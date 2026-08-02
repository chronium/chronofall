using System.Collections.ObjectModel;
using System.Numerics;

namespace ChronoFall.CharacterPresentation;

public enum AttachmentReferencePointRole
{
    Muzzle,
    ProjectileOrigin,
    CasingEjection,
    Aim,
}

public sealed class AttachmentReferencePointDefinition
{
    public AttachmentReferencePointDefinition(
        string name,
        AttachmentReferencePointRole role,
        JointTransform localTransform)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        if (!Enum.IsDefined(role))
            throw new ArgumentOutOfRangeException(nameof(role), role, "Reference point role must be defined.");

        localTransform.Validate(nameof(localTransform));
        if (localTransform.Scale != Vector3.One)
        {
            throw new ArgumentException(
                "Attachment reference points are rigid frames and must use identity local scale.",
                nameof(localTransform));
        }

        Name = name;
        Role = role;
        LocalTransform = localTransform;
    }

    public string Name { get; }

    public AttachmentReferencePointRole Role { get; }

    public JointTransform LocalTransform { get; }
}

public sealed class AttachmentReferencePointSet
{
    private readonly IReadOnlyDictionary<string, int> referencePointIndicesByName;
    private readonly IReadOnlyDictionary<AttachmentReferencePointRole, IReadOnlyList<int>>
        referencePointIndicesByRole;

    public AttachmentReferencePointSet(IEnumerable<AttachmentReferencePointDefinition> referencePoints)
    {
        ArgumentNullException.ThrowIfNull(referencePoints);

        AttachmentReferencePointDefinition[] copy = referencePoints.ToArray();
        var indicesByName = new Dictionary<string, int>(copy.Length, StringComparer.Ordinal);
        Dictionary<AttachmentReferencePointRole, List<int>> mutableIndicesByRole =
            Enum.GetValues<AttachmentReferencePointRole>()
                .ToDictionary(static role => role, static _ => new List<int>());

        for (int index = 0; index < copy.Length; index++)
        {
            AttachmentReferencePointDefinition referencePoint = copy[index] ??
                throw new ArgumentException($"Reference point {index} cannot be null.", nameof(referencePoints));
            if (!indicesByName.TryAdd(referencePoint.Name, index))
            {
                throw new ArgumentException(
                    $"Reference point name '{referencePoint.Name}' is duplicated.",
                    nameof(referencePoints));
            }

            mutableIndicesByRole[referencePoint.Role].Add(index);
        }

        var immutableIndicesByRole = new Dictionary<AttachmentReferencePointRole, IReadOnlyList<int>>(
            mutableIndicesByRole.Count);
        foreach ((AttachmentReferencePointRole role, List<int> indices) in mutableIndicesByRole)
            immutableIndicesByRole.Add(role, Array.AsReadOnly(indices.ToArray()));

        ReferencePoints = Array.AsReadOnly(copy);
        referencePointIndicesByName = new ReadOnlyDictionary<string, int>(indicesByName);
        referencePointIndicesByRole =
            new ReadOnlyDictionary<AttachmentReferencePointRole, IReadOnlyList<int>>(immutableIndicesByRole);
    }

    public IReadOnlyList<AttachmentReferencePointDefinition> ReferencePoints { get; }

    public int ReferencePointCount => ReferencePoints.Count;

    public bool TryGetReferencePointIndex(string name, out int referencePointIndex)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        return referencePointIndicesByName.TryGetValue(name, out referencePointIndex);
    }

    public IReadOnlyList<int> GetReferencePointIndices(AttachmentReferencePointRole role)
    {
        if (!Enum.IsDefined(role))
            throw new ArgumentOutOfRangeException(nameof(role), role, "Reference point role must be defined.");

        return referencePointIndicesByRole[role];
    }
}

public sealed class AttachmentReferencePointPose
{
    public AttachmentReferencePointPose(
        AttachmentReferencePointSet referencePointSet,
        IEnumerable<Matrix4x4> modelTransforms)
    {
        ArgumentNullException.ThrowIfNull(referencePointSet);
        ArgumentNullException.ThrowIfNull(modelTransforms);

        Matrix4x4[] copy = modelTransforms.ToArray();
        if (copy.Length != referencePointSet.ReferencePointCount)
        {
            throw new ArgumentException(
                $"Expected {referencePointSet.ReferencePointCount} reference point model transforms, " +
                $"but received {copy.Length}.",
                nameof(modelTransforms));
        }

        for (int index = 0; index < copy.Length; index++)
        {
            DataValidation.RequireFinite(
                copy[index],
                nameof(modelTransforms),
                $"Reference point model transform {index}");
        }

        ReferencePointSet = referencePointSet;
        ModelTransforms = Array.AsReadOnly(copy);
    }

    public AttachmentReferencePointSet ReferencePointSet { get; }

    public IReadOnlyList<Matrix4x4> ModelTransforms { get; }

    public bool TryGetModelTransform(string referencePointName, out Matrix4x4 modelTransform)
    {
        if (ReferencePointSet.TryGetReferencePointIndex(referencePointName, out int referencePointIndex))
        {
            modelTransform = ModelTransforms[referencePointIndex];
            return true;
        }

        modelTransform = default;
        return false;
    }
}

public static class AttachmentReferencePointEvaluator
{
    public static AttachmentReferencePointPose EvaluateModelSpace(
        AttachmentReferencePointSet referencePointSet,
        Matrix4x4 attachmentModelTransform)
    {
        ArgumentNullException.ThrowIfNull(referencePointSet);
        DataValidation.RequireFinite(
            attachmentModelTransform,
            nameof(attachmentModelTransform),
            "Attachment model transform");

        var transforms = new Matrix4x4[referencePointSet.ReferencePointCount];
        for (int index = 0; index < transforms.Length; index++)
        {
            transforms[index] =
                referencePointSet.ReferencePoints[index].LocalTransform.ToMatrix() *
                attachmentModelTransform;
        }

        return new AttachmentReferencePointPose(referencePointSet, transforms);
    }
}
