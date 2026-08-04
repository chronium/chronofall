using System.Numerics;
using ChronoFall.Box3D.Bodies;
using ChronoFall.Box3D.Geometry;
using ChronoFall.Box3D.Runtime;
using ChronoFall.Box3D.Worlds;

using Box3DWorld world = Box3DWorld.Create(Vector3.Zero);
using Box3DBody obstacle = world.CreateBody(Box3DBodyKind.Static, Vector3.Zero, Quaternion.Identity);
obstacle.CreateBoxShape(Vector3.One);
world.Step(1f / 60f, 1);
float fraction = world.CastMover(Vector3.Zero, new Vector3(-5, -0.3f, 0), new Vector3(-5, 0.3f, 0), 0.25f, new Vector3(10, 0, 0), Box3DQueryFilter.All);
if (fraction is < 0.35f or > 0.40f) throw new InvalidOperationException($"Unexpected mover fraction {fraction}.");
Console.WriteLine(FormattableString.Invariant($"ChronoFall Box3D headless source consumer OK ({Box3DRuntime.CurrentRuntimeIdentifier}, fraction={fraction:F3})."));
