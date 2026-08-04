using System.Numerics;
using ChronoFall.Box3D.Bindings.Interop;
using ChronoFall.Box3D.Geometry;
using ChronoFall.Box3D.Worlds;

namespace ChronoFall.Box3D.Bodies;

public enum Box3DBodyKind { Static, Kinematic, Dynamic }

public readonly record struct Box3DTransform(Vector3 Position, Quaternion Rotation);

public sealed class Box3DBody : IDisposable
{
    private readonly Box3DWorld world;
    private readonly List<Box3DShape> shapes = [];
    private bool disposed;

    internal Box3DBody(Box3DWorld world, B3BodyId id)
    {
        this.world = world;
        Id = id;
    }

    internal B3BodyId Id { get; }
    public bool IsDisposed => disposed;
    public bool IsValid => !disposed && world.IsValid && Box3DBindingSurface.b3Body_IsValid(Id);

    public Box3DTransform Transform
    {
        get
        {
            ThrowIfDisposed();
            B3WorldTransform value = Box3DBindingSurface.b3Body_GetTransform(Id);
            return new Box3DTransform(Box3DValueValidation.Position(value.P), Box3DValueValidation.Rotation(value.Q));
        }
        set
        {
            Box3DValueValidation.Finite(value.Position, nameof(value));
            Box3DValueValidation.Rotation(value.Rotation, nameof(value));
            ThrowIfDisposed();
            Box3DBindingSurface.b3Body_SetTransform(Id, Box3DValueValidation.Position(value.Position), Box3DValueValidation.Rotation(value.Rotation));
        }
    }

    public Vector3 LinearVelocity
    {
        get { ThrowIfDisposed(); return Box3DValueValidation.Vector(Box3DBindingSurface.b3Body_GetLinearVelocity(Id)); }
        set
        {
            Box3DValueValidation.Finite(value, nameof(value));
            ThrowIfDisposed();
            Box3DBindingSurface.b3Body_SetLinearVelocity(Id, Box3DValueValidation.Vector(value));
        }
    }

    public Box3DShape CreateBoxShape(Vector3 halfExtents, Box3DFilter? filter = null)
    {
        Box3DValueValidation.Positive(halfExtents, nameof(halfExtents));
        ThrowIfDisposed();
        B3BoxHull hull = Box3DBindingSurface.b3MakeBoxHull(halfExtents.X, halfExtents.Y, halfExtents.Z);
        B3ShapeDef def = ShapeDefinition(filter);
        return Own(Box3DBindingSurface.b3CreateHullShape(Id, in def, in hull.Base), "box");
    }

    public Box3DShape CreateCapsuleShape(Vector3 center1, Vector3 center2, float radius, Box3DFilter? filter = null)
    {
        Box3DValueValidation.Finite(center1, nameof(center1));
        Box3DValueValidation.Finite(center2, nameof(center2));
        Box3DValueValidation.Positive(radius, nameof(radius));
        ThrowIfDisposed();
        B3Capsule capsule = new() { Center1 = Box3DValueValidation.Vector(center1), Center2 = Box3DValueValidation.Vector(center2), Radius = radius };
        B3ShapeDef def = ShapeDefinition(filter);
        return Own(Box3DBindingSurface.b3CreateCapsuleShape(Id, in def, in capsule), "capsule");
    }

    public void Dispose()
    {
        if (disposed) return;
        if (world.IsValid && Box3DBindingSurface.b3Body_IsValid(Id))
            Box3DBindingSurface.b3DestroyBody(Id);
        foreach (Box3DShape shape in shapes) shape.InvalidateFromBody();
        disposed = true;
    }

    internal void InvalidateFromWorld()
    {
        if (disposed) return;
        foreach (Box3DShape shape in shapes) shape.InvalidateFromBody();
        disposed = true;
    }

    private B3ShapeDef ShapeDefinition(Box3DFilter? filter)
    {
        B3ShapeDef def = Box3DBindingSurface.b3DefaultShapeDef();
        if (filter.HasValue) def.Filter = filter.Value.ToNative();
        return def;
    }

    private Box3DShape Own(B3ShapeId id, string kind)
    {
        if (!Box3DBindingSurface.b3Shape_IsValid(id)) throw new InvalidOperationException($"Box3D did not create a valid {kind} shape.");
        Box3DShape shape = new(this, id);
        shapes.Add(shape);
        return shape;
    }

    private void ThrowIfDisposed() => ObjectDisposedException.ThrowIf(disposed, this);
}
