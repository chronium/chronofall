using System.Runtime.InteropServices;

namespace ChronoFall.Box3D.Bindings.Interop;

public static class Box3DBindingSurface
{
    public const string NativeLibraryName = "box3d";

    static Box3DBindingSurface() => NativeLibraryResolver.ConfigureForAssembly(typeof(Box3DBindingSurface).Assembly);

    [DllImport(NativeLibraryName, CallingConvention = CallingConvention.Cdecl)] public static extern B3WorldDef b3DefaultWorldDef();
    [DllImport(NativeLibraryName, CallingConvention = CallingConvention.Cdecl)] public static extern B3WorldId b3CreateWorld(in B3WorldDef def);
    [DllImport(NativeLibraryName, CallingConvention = CallingConvention.Cdecl)] public static extern void b3DestroyWorld(B3WorldId id);
    [DllImport(NativeLibraryName, CallingConvention = CallingConvention.Cdecl)][return: MarshalAs(UnmanagedType.I1)] public static extern bool b3World_IsValid(B3WorldId id);
    [DllImport(NativeLibraryName, CallingConvention = CallingConvention.Cdecl)] public static extern void b3World_Step(B3WorldId id, float timeStep, int subStepCount);
    [DllImport(NativeLibraryName, CallingConvention = CallingConvention.Cdecl)] public static extern B3BodyDef b3DefaultBodyDef();
    [DllImport(NativeLibraryName, CallingConvention = CallingConvention.Cdecl)] public static extern B3BodyId b3CreateBody(B3WorldId worldId, in B3BodyDef def);
    [DllImport(NativeLibraryName, CallingConvention = CallingConvention.Cdecl)] public static extern void b3DestroyBody(B3BodyId id);
    [DllImport(NativeLibraryName, CallingConvention = CallingConvention.Cdecl)][return: MarshalAs(UnmanagedType.I1)] public static extern bool b3Body_IsValid(B3BodyId id);
    [DllImport(NativeLibraryName, CallingConvention = CallingConvention.Cdecl)] public static extern B3WorldTransform b3Body_GetTransform(B3BodyId id);
    [DllImport(NativeLibraryName, CallingConvention = CallingConvention.Cdecl)] public static extern void b3Body_SetTransform(B3BodyId id, B3Pos position, B3Quat rotation);
    [DllImport(NativeLibraryName, CallingConvention = CallingConvention.Cdecl)] public static extern B3Vec3 b3Body_GetLinearVelocity(B3BodyId id);
    [DllImport(NativeLibraryName, CallingConvention = CallingConvention.Cdecl)] public static extern void b3Body_SetLinearVelocity(B3BodyId id, B3Vec3 velocity);
    [DllImport(NativeLibraryName, CallingConvention = CallingConvention.Cdecl)] public static extern B3ShapeDef b3DefaultShapeDef();
    [DllImport(NativeLibraryName, CallingConvention = CallingConvention.Cdecl)] public static extern B3BoxHull b3MakeBoxHull(float hx, float hy, float hz);
    [DllImport(NativeLibraryName, CallingConvention = CallingConvention.Cdecl)] public static extern B3ShapeId b3CreateHullShape(B3BodyId bodyId, in B3ShapeDef def, in B3HullData hull);
    [DllImport(NativeLibraryName, CallingConvention = CallingConvention.Cdecl)] public static extern B3ShapeId b3CreateCapsuleShape(B3BodyId bodyId, in B3ShapeDef def, in B3Capsule capsule);
    [DllImport(NativeLibraryName, CallingConvention = CallingConvention.Cdecl)] public static extern void b3DestroyShape(B3ShapeId id, [MarshalAs(UnmanagedType.I1)] bool updateBodyMass);
    [DllImport(NativeLibraryName, CallingConvention = CallingConvention.Cdecl)][return: MarshalAs(UnmanagedType.I1)] public static extern bool b3Shape_IsValid(B3ShapeId id);
    [DllImport(NativeLibraryName, CallingConvention = CallingConvention.Cdecl)] public static extern B3Filter b3Shape_GetFilter(B3ShapeId id);
    [DllImport(NativeLibraryName, CallingConvention = CallingConvention.Cdecl)] public static extern void b3Shape_SetFilter(B3ShapeId id, B3Filter filter, [MarshalAs(UnmanagedType.I1)] bool invokeContacts);
    [DllImport(NativeLibraryName, CallingConvention = CallingConvention.Cdecl)] public static extern B3QueryFilter b3DefaultQueryFilter();
    [DllImport(NativeLibraryName, CallingConvention = CallingConvention.Cdecl)] public static extern float b3World_CastMover(B3WorldId worldId, B3Pos origin, in B3Capsule mover, B3Vec3 translation, B3QueryFilter filter, B3MoverFilterFcn? fcn, nint context);
    [DllImport(NativeLibraryName, CallingConvention = CallingConvention.Cdecl)] public static extern void b3World_CollideMover(B3WorldId worldId, B3Pos origin, in B3Capsule mover, B3QueryFilter filter, B3PlaneResultFcn fcn, nint context);
}

public static class Box3DBindingRuntime
{
    public static string CurrentRuntimeIdentifier => NativeLibraryResolver.CurrentRuntimeIdentifier;
    public static string GetExpectedPath(string runtimeIdentifier) => NativeLibraryResolver.GetExpectedPath(Box3DBindingSurface.NativeLibraryName, runtimeIdentifier);
}
