using ChronoFall.Box3D.Bindings.Interop;
using ChronoFall.Box3D.Bodies;

namespace ChronoFall.Box3D.Geometry;

public sealed class Box3DShape : IDisposable
{
    private readonly Box3DBody body;
    private bool disposed;

    internal Box3DShape(Box3DBody body, B3ShapeId id)
    {
        this.body = body;
        Id = id;
    }

    internal B3ShapeId Id { get; }
    public Box3DShapeIdentity Identity => Box3DShapeIdentity.FromNative(Id);
    public bool IsDisposed => disposed;
    public bool IsValid => !disposed && body.IsValid && Box3DBindingSurface.b3Shape_IsValid(Id);

    public Box3DFilter Filter
    {
        get
        {
            ThrowIfDisposed();
            B3Filter filter = Box3DBindingSurface.b3Shape_GetFilter(Id);
            return new Box3DFilter(filter.CategoryBits, filter.MaskBits, filter.GroupIndex);
        }
        set
        {
            ThrowIfDisposed();
            Box3DBindingSurface.b3Shape_SetFilter(Id, value.ToNative(), invokeContacts: true);
        }
    }

    public void Dispose()
    {
        if (disposed) return;
        if (body.IsValid && Box3DBindingSurface.b3Shape_IsValid(Id))
            Box3DBindingSurface.b3DestroyShape(Id, updateBodyMass: true);
        disposed = true;
    }

    internal void InvalidateFromBody() => disposed = true;
    private void ThrowIfDisposed() => ObjectDisposedException.ThrowIf(disposed, this);
}
