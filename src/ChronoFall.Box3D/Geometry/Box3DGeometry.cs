using System.Numerics;
using ChronoFall.Box3D.Bindings.Interop;

namespace ChronoFall.Box3D.Geometry;

public readonly record struct Box3DFilter(ulong CategoryBits, ulong MaskBits, int GroupIndex = 0)
{
    public static Box3DFilter All { get; } = new(ulong.MaxValue, ulong.MaxValue);
    internal B3Filter ToNative() => new() { CategoryBits = CategoryBits, MaskBits = MaskBits, GroupIndex = GroupIndex };
}

public readonly record struct Box3DQueryFilter(ulong CategoryBits, ulong MaskBits)
{
    public static Box3DQueryFilter All { get; } = new(ulong.MaxValue, ulong.MaxValue);
    internal B3QueryFilter ToNative()
    {
        B3QueryFilter value = Box3DBindingSurface.b3DefaultQueryFilter();
        value.CategoryBits = CategoryBits;
        value.MaskBits = MaskBits;
        return value;
    }
}

public readonly record struct Box3DShapeIdentity(int Index, ushort World, ushort Generation) : IComparable<Box3DShapeIdentity>
{
    internal static Box3DShapeIdentity FromNative(B3ShapeId id) => new(id.Index1, id.World0, id.Generation);
    public int CompareTo(Box3DShapeIdentity other)
    {
        int index = Index.CompareTo(other.Index);
        if (index != 0) return index;
        int world = World.CompareTo(other.World);
        return world != 0 ? world : Generation.CompareTo(other.Generation);
    }
}

public readonly record struct Box3DMoverContact(Box3DShapeIdentity Shape, Vector3 Normal, float PlaneOffset, Vector3 Point);

internal static class Box3DValueValidation
{
    internal static void Finite(Vector3 value, string name)
    {
        if (!float.IsFinite(value.X) || !float.IsFinite(value.Y) || !float.IsFinite(value.Z))
            throw new ArgumentException($"{name} must contain only finite values.", name);
    }

    internal static void Positive(Vector3 value, string name)
    {
        Finite(value, name);
        if (value.X <= 0 || value.Y <= 0 || value.Z <= 0)
            throw new ArgumentOutOfRangeException(name, "All components must be positive.");
    }

    internal static void Positive(float value, string name)
    {
        if (!float.IsFinite(value) || value <= 0)
            throw new ArgumentOutOfRangeException(name, "Value must be finite and positive.");
    }

    internal static void Rotation(Quaternion value, string name)
    {
        if (!float.IsFinite(value.X) || !float.IsFinite(value.Y) || !float.IsFinite(value.Z) || !float.IsFinite(value.W))
            throw new ArgumentException("Rotation must contain only finite values.", name);
        float lengthSquared = value.LengthSquared();
        if (!float.IsFinite(lengthSquared) || MathF.Abs(lengthSquared - 1.0f) > 0.001f)
            throw new ArgumentException("Rotation must be normalized.", name);
    }

    internal static B3Vec3 Vector(Vector3 value) => new() { X = value.X, Y = value.Y, Z = value.Z };
    internal static B3Pos Position(Vector3 value) => new() { X = value.X, Y = value.Y, Z = value.Z };
    internal static B3Quat Rotation(Quaternion value) => new() { V = new B3Vec3 { X = value.X, Y = value.Y, Z = value.Z }, S = value.W };
    internal static Vector3 Vector(B3Vec3 value) => new(value.X, value.Y, value.Z);
    internal static Vector3 Position(B3Pos value) => new(value.X, value.Y, value.Z);
    internal static Quaternion Rotation(B3Quat value) => new(value.V.X, value.V.Y, value.V.Z, value.S);
}
