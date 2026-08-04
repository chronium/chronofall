using System.Numerics;
using ChronoFall.Box3D.Bodies;
using ChronoFall.Box3D.Geometry;
using ChronoFall.Box3D.Worlds;

namespace ChronoFall.Box3D.Tests;

public sealed class ManagedBoundaryTests
{
    [Fact]
    public void WorldOwnsBodiesAndShapesRecursively()
    {
        Box3DWorld world = Box3DWorld.Create(Vector3.Zero);
        Box3DBody body = world.CreateBody(Box3DBodyKind.Static, Vector3.Zero, Quaternion.Identity);
        Box3DShape shape = body.CreateBoxShape(Vector3.One);

        world.Dispose();
        world.Dispose();

        Assert.True(world.IsDisposed);
        Assert.True(body.IsDisposed);
        Assert.True(shape.IsDisposed);
        Assert.Throws<ObjectDisposedException>(() => world.Step(1f / 60f, 4));
        Assert.Throws<ObjectDisposedException>(() => body.LinearVelocity = Vector3.One);
        Assert.Throws<ObjectDisposedException>(() => shape.Filter = Box3DFilter.All);
    }

    [Fact]
    public void TransformVelocityShapesAndFilterRoundTrip()
    {
        using Box3DWorld world = Box3DWorld.Create(Vector3.Zero);
        using Box3DBody body = world.CreateBody(Box3DBodyKind.Kinematic, new Vector3(1, 2, 3), Quaternion.Identity);
        body.LinearVelocity = new Vector3(4, 0, -2);
        body.Transform = new Box3DTransform(new Vector3(5, 6, 7), Quaternion.Identity);
        using Box3DShape box = body.CreateBoxShape(new Vector3(1, 2, 3), new Box3DFilter(2, 4));
        using Box3DShape capsule = body.CreateCapsuleShape(new Vector3(0, -0.5f, 0), new Vector3(0, 0.5f, 0), 0.25f);

        Assert.Equal(new Vector3(4, 0, -2), body.LinearVelocity);
        Assert.Equal(new Vector3(5, 6, 7), body.Transform.Position);
        Assert.Equal(new Box3DFilter(2, 4), box.Filter);
        Assert.True(capsule.IsValid);
    }

    [Fact]
    public void InvalidValuesFailBeforeNativeUse()
    {
        Assert.Throws<ArgumentException>(() => Box3DWorld.Create(new Vector3(float.NaN, 0, 0)));
        using Box3DWorld world = Box3DWorld.Create(Vector3.Zero);
        Assert.Throws<ArgumentException>(() => world.CreateBody(Box3DBodyKind.Static, Vector3.Zero, new Quaternion(0, 0, 0, 0)));
        using Box3DBody body = world.CreateBody(Box3DBodyKind.Static, Vector3.Zero, Quaternion.Identity);
        Assert.Throws<ArgumentOutOfRangeException>(() => world.Step(0, 1));
        Assert.Throws<ArgumentOutOfRangeException>(() => world.Step(1f / 60f, 0));
        Assert.Throws<ArgumentOutOfRangeException>(() => body.CreateBoxShape(new Vector3(1, 0, 1)));
        Assert.Throws<ArgumentOutOfRangeException>(() => body.CreateCapsuleShape(Vector3.Zero, Vector3.One, 0));
        Assert.Throws<ArgumentException>(() => world.CastMover(Vector3.Zero, Vector3.Zero, Vector3.One, 0.25f, new Vector3(float.PositiveInfinity, 0, 0)));
    }

    [Fact]
    public void FallingBoxSettlesOnStaticGround()
    {
        using Box3DWorld world = Box3DWorld.Create(new Vector3(0, -10, 0));
        using Box3DBody ground = world.CreateBody(Box3DBodyKind.Static, new Vector3(0, -10, 0), Quaternion.Identity);
        ground.CreateBoxShape(new Vector3(50, 10, 50));
        using Box3DBody falling = world.CreateBody(Box3DBodyKind.Dynamic, new Vector3(0, 4, 0), Quaternion.Identity);
        falling.CreateBoxShape(Vector3.One);

        for (int i = 0; i < 90; i++) world.Step(1f / 60f, 4);

        Assert.InRange(falling.Transform.Position.Y, 0.98f, 1.05f);
    }

    [Fact]
    public void MoverCastContactsAndFilteringAreBoundedAndStable()
    {
        using Box3DWorld world = Box3DWorld.Create(Vector3.Zero);
        using Box3DBody obstacle = world.CreateBody(Box3DBodyKind.Static, Vector3.Zero, Quaternion.Identity);
        using Box3DShape shape = obstacle.CreateBoxShape(Vector3.One, new Box3DFilter(2, 2));
        world.Step(1f / 60f, 1);

        float excluded = world.CastMover(Vector3.Zero, new Vector3(-5, -0.3f, 0), new Vector3(-5, 0.3f, 0), 0.25f, new Vector3(10, 0, 0), new Box3DQueryFilter(1, 1));
        float hit = world.CastMover(Vector3.Zero, new Vector3(-5, -0.3f, 0), new Vector3(-5, 0.3f, 0), 0.25f, new Vector3(10, 0, 0), new Box3DQueryFilter(2, 2));
        IReadOnlyList<Box3DMoverContact> contacts = world.CollectMoverContacts(Vector3.Zero, new Vector3(-1.1f, -0.3f, 0), new Vector3(-1.1f, 0.3f, 0), 0.3f, new Box3DQueryFilter(2, 2));

        Assert.Equal(1f, excluded);
        Assert.InRange(hit, 0.35f, 0.40f);
        Assert.NotEmpty(contacts);
        Assert.All(contacts, contact => Assert.Equal(shape.Identity, contact.Shape));
        Assert.Throws<NotSupportedException>(() => ((IList<Box3DMoverContact>)contacts).Add(default));
        Assert.Equal(contacts.OrderBy(static x => x.Shape).ThenBy(static x => x.Normal.X).ThenBy(static x => x.Normal.Y).ThenBy(static x => x.Normal.Z).ThenBy(static x => x.PlaneOffset).ThenBy(static x => x.Point.X).ThenBy(static x => x.Point.Y).ThenBy(static x => x.Point.Z), contacts);
    }
}
