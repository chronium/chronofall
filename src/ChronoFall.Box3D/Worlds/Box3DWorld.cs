using System.Numerics;
using System.Runtime.InteropServices;
using ChronoFall.Box3D.Bindings.Interop;
using ChronoFall.Box3D.Bodies;
using ChronoFall.Box3D.Geometry;

namespace ChronoFall.Box3D.Worlds;

public sealed unsafe class Box3DWorld : IDisposable
{
    private static readonly B3PlaneResultFcn PlaneResultCallback = CollectPlanes;
    private readonly List<Box3DBody> bodies = [];
    private bool disposed;

    private Box3DWorld(B3WorldId id) => Id = id;
    internal B3WorldId Id { get; }
    public bool IsDisposed => disposed;
    public bool IsValid => !disposed && Box3DBindingSurface.b3World_IsValid(Id);

    public static Box3DWorld Create(Vector3 gravity)
    {
        Box3DValueValidation.Finite(gravity, nameof(gravity));
        B3WorldDef def = Box3DBindingSurface.b3DefaultWorldDef();
        def.Gravity = Box3DValueValidation.Vector(gravity);
        B3WorldId id = Box3DBindingSurface.b3CreateWorld(in def);
        if (!Box3DBindingSurface.b3World_IsValid(id)) throw new InvalidOperationException("Box3D did not create a valid world.");
        return new Box3DWorld(id);
    }

    public void Step(float timeStepSeconds, int subStepCount)
    {
        Box3DValueValidation.Positive(timeStepSeconds, nameof(timeStepSeconds));
        if (subStepCount <= 0) throw new ArgumentOutOfRangeException(nameof(subStepCount));
        ThrowIfDisposed();
        Box3DBindingSurface.b3World_Step(Id, timeStepSeconds, subStepCount);
    }

    public Box3DBody CreateBody(Box3DBodyKind kind, Vector3 position, Quaternion rotation)
    {
        Box3DValueValidation.Finite(position, nameof(position));
        Box3DValueValidation.Rotation(rotation, nameof(rotation));
        ThrowIfDisposed();
        B3BodyDef def = Box3DBindingSurface.b3DefaultBodyDef();
        def.Type = kind switch { Box3DBodyKind.Static => B3BodyType.StaticBody, Box3DBodyKind.Kinematic => B3BodyType.KinematicBody, Box3DBodyKind.Dynamic => B3BodyType.DynamicBody, _ => throw new ArgumentOutOfRangeException(nameof(kind)) };
        def.Position = Box3DValueValidation.Position(position);
        def.Rotation = Box3DValueValidation.Rotation(rotation);
        B3BodyId id = Box3DBindingSurface.b3CreateBody(Id, in def);
        if (!Box3DBindingSurface.b3Body_IsValid(id)) throw new InvalidOperationException("Box3D did not create a valid body.");
        Box3DBody body = new(this, id);
        bodies.Add(body);
        return body;
    }

    public float CastMover(Vector3 origin, Vector3 center1, Vector3 center2, float radius, Vector3 translation, Box3DQueryFilter? filter = null)
    {
        B3Capsule capsule = ValidateMover(origin, center1, center2, radius, translation);
        ThrowIfDisposed();
        return Box3DBindingSurface.b3World_CastMover(Id, Box3DValueValidation.Position(origin), in capsule, Box3DValueValidation.Vector(translation), (filter ?? Box3DQueryFilter.All).ToNative(), null, nint.Zero);
    }

    public IReadOnlyList<Box3DMoverContact> CollectMoverContacts(Vector3 origin, Vector3 center1, Vector3 center2, float radius, Box3DQueryFilter? filter = null)
    {
        B3Capsule capsule = ValidateMover(origin, center1, center2, radius, Vector3.Zero);
        ThrowIfDisposed();
        List<Box3DMoverContact> contacts = [];
        GCHandle handle = GCHandle.Alloc(contacts);
        try
        {
            Box3DBindingSurface.b3World_CollideMover(Id, Box3DValueValidation.Position(origin), in capsule, (filter ?? Box3DQueryFilter.All).ToNative(), PlaneResultCallback, GCHandle.ToIntPtr(handle));
        }
        finally { handle.Free(); }
        contacts.Sort(CompareContacts);
        return contacts.AsReadOnly();
    }

    public void Dispose()
    {
        if (disposed) return;
        if (Box3DBindingSurface.b3World_IsValid(Id)) Box3DBindingSurface.b3DestroyWorld(Id);
        foreach (Box3DBody body in bodies) body.InvalidateFromWorld();
        disposed = true;
    }

    private static B3Capsule ValidateMover(Vector3 origin, Vector3 center1, Vector3 center2, float radius, Vector3 translation)
    {
        Box3DValueValidation.Finite(origin, nameof(origin));
        Box3DValueValidation.Finite(center1, nameof(center1));
        Box3DValueValidation.Finite(center2, nameof(center2));
        Box3DValueValidation.Finite(translation, nameof(translation));
        Box3DValueValidation.Positive(radius, nameof(radius));
        return new B3Capsule { Center1 = Box3DValueValidation.Vector(center1), Center2 = Box3DValueValidation.Vector(center2), Radius = radius };
    }

    private static unsafe bool CollectPlanes(B3ShapeId shapeId, B3PlaneResult* planes, int planeCount, nint context)
    {
        List<Box3DMoverContact> contacts = (List<Box3DMoverContact>)GCHandle.FromIntPtr(context).Target!;
        for (int i = 0; i < planeCount; i++)
        {
            B3PlaneResult plane = planes[i];
            contacts.Add(new Box3DMoverContact(Box3DShapeIdentity.FromNative(shapeId), Box3DValueValidation.Vector(plane.Plane.Normal), plane.Plane.Offset, Box3DValueValidation.Vector(plane.Point)));
        }
        return true;
    }

    private static int CompareContacts(Box3DMoverContact left, Box3DMoverContact right)
    {
        int value = left.Shape.CompareTo(right.Shape);
        if (value != 0) return value;
        value = left.Normal.X.CompareTo(right.Normal.X); if (value != 0) return value;
        value = left.Normal.Y.CompareTo(right.Normal.Y); if (value != 0) return value;
        value = left.Normal.Z.CompareTo(right.Normal.Z); if (value != 0) return value;
        value = left.PlaneOffset.CompareTo(right.PlaneOffset); if (value != 0) return value;
        value = left.Point.X.CompareTo(right.Point.X); if (value != 0) return value;
        value = left.Point.Y.CompareTo(right.Point.Y); return value != 0 ? value : left.Point.Z.CompareTo(right.Point.Z);
    }

    private void ThrowIfDisposed() => ObjectDisposedException.ThrowIf(disposed, this);
}
