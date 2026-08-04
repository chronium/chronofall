using System.Runtime.InteropServices;
using ChronoFall.Box3D.Bindings.Interop;

namespace ChronoFall.Box3D.Tests;

public sealed class NativeLayoutTests
{
    [Fact]
    public unsafe void NarrowAbiMatchesPinnedBox3D()
    {
        Assert.Equal(12, sizeof(B3Vec3));
        Assert.Equal(16, sizeof(B3Quat));
        Assert.Equal(12, sizeof(B3Pos));
        Assert.Equal(28, sizeof(B3WorldTransform));
        Assert.Equal(24, sizeof(B3Aabb));
        Assert.Equal(16, sizeof(B3Plane));
        Assert.Equal(4, sizeof(B3WorldId));
        Assert.Equal(8, sizeof(B3BodyId));
        Assert.Equal(8, sizeof(B3ShapeId));
        Assert.Equal(20, sizeof(B3Capacity));
        Assert.Equal(144, sizeof(B3WorldDef));
        Assert.Equal(6, sizeof(B3MotionLocks));
        Assert.Equal(104, sizeof(B3BodyDef));
        Assert.Equal(40, sizeof(B3SurfaceMaterial));
        Assert.Equal(120, sizeof(B3ShapeDef));
        Assert.Equal(28, sizeof(B3Capsule));
        Assert.Equal(24, sizeof(B3Filter));
        Assert.Equal(32, sizeof(B3QueryFilter));
        Assert.Equal(28, sizeof(B3PlaneResult));
        Assert.Equal(144, sizeof(B3HullData));
        Assert.Equal(648, sizeof(B3BoxHull));
    }

    [Fact]
    public void CriticalOffsetsMatchPinnedBox3D()
    {
        Assert.Equal(56, Marshal.OffsetOf<B3WorldDef>(nameof(B3WorldDef.EnableSleep)).ToInt32());
        Assert.Equal(120, Marshal.OffsetOf<B3WorldDef>(nameof(B3WorldDef.Capacity)).ToInt32());
        Assert.Equal(88, Marshal.OffsetOf<B3BodyDef>(nameof(B3BodyDef.MotionLocks)).ToInt32());
        Assert.Equal(100, Marshal.OffsetOf<B3BodyDef>(nameof(B3BodyDef.InternalValue)).ToInt32());
        Assert.Equal(80, Marshal.OffsetOf<B3ShapeDef>(nameof(B3ShapeDef.Filter)).ToInt32());
        Assert.Equal(116, Marshal.OffsetOf<B3ShapeDef>(nameof(B3ShapeDef.InternalValue)).ToInt32());
        Assert.Equal(24, Marshal.OffsetOf<B3Capsule>(nameof(B3Capsule.Radius)).ToInt32());
        Assert.Equal(24, Marshal.OffsetOf<B3QueryFilter>(nameof(B3QueryFilter.Name)).ToInt32());
    }
}
