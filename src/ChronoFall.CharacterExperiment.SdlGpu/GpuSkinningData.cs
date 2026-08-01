using System.Numerics;

namespace ChronoFall.CharacterExperiment.SdlGpu;

internal readonly record struct MeshBounds(Vector3 Minimum, Vector3 Maximum)
{
    internal Vector3 Center => (Minimum + Maximum) * 0.5f;
    internal Vector3 Extents => (Maximum - Minimum) * 0.5f;
    internal float Radius => Extents.Length();

    internal static MeshBounds Create(IReadOnlyList<Vector3> positions)
    {
        ArgumentNullException.ThrowIfNull(positions);
        if (positions.Count == 0)
            throw new ArgumentException("Mesh bounds require at least one position.", nameof(positions));

        Vector3 minimum = positions[0];
        Vector3 maximum = positions[0];
        for (int index = 1; index < positions.Count; index++)
        {
            minimum = Vector3.Min(minimum, positions[index]);
            maximum = Vector3.Max(maximum, positions[index]);
        }

        return new MeshBounds(minimum, maximum);
    }
}
