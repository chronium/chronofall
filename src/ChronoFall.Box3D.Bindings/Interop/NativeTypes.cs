using System.Runtime.InteropServices;

namespace ChronoFall.Box3D.Bindings.Interop;

[StructLayout(LayoutKind.Sequential)] public struct B3Vec3 { public float X; public float Y; public float Z; }
[StructLayout(LayoutKind.Sequential)] public struct B3Pos { public float X; public float Y; public float Z; }
[StructLayout(LayoutKind.Sequential)] public struct B3Quat { public B3Vec3 V; public float S; }
[StructLayout(LayoutKind.Sequential)] public struct B3WorldTransform { public B3Pos P; public B3Quat Q; }
[StructLayout(LayoutKind.Sequential)] public struct B3Matrix3 { public B3Vec3 Cx; public B3Vec3 Cy; public B3Vec3 Cz; }
[StructLayout(LayoutKind.Sequential)] public struct B3Aabb { public B3Vec3 LowerBound; public B3Vec3 UpperBound; }
[StructLayout(LayoutKind.Sequential)] public struct B3Plane { public B3Vec3 Normal; public float Offset; }
[StructLayout(LayoutKind.Sequential)] public struct B3WorldId { public ushort Index1; public ushort Generation; }
[StructLayout(LayoutKind.Sequential)] public struct B3BodyId { public int Index1; public ushort World0; public ushort Generation; }
[StructLayout(LayoutKind.Sequential)] public struct B3ShapeId { public int Index1; public ushort World0; public ushort Generation; }

[StructLayout(LayoutKind.Sequential)]
public struct B3Capacity
{
    public int StaticShapeCount;
    public int DynamicShapeCount;
    public int StaticBodyCount;
    public int DynamicBodyCount;
    public int ContactCount;
}

[StructLayout(LayoutKind.Sequential)]
public struct B3WorldDef
{
    public B3Vec3 Gravity;
    public float RestitutionThreshold;
    public float HitEventThreshold;
    public float ContactHertz;
    public float ContactDampingRatio;
    public float ContactSpeed;
    public float MaximumLinearSpeed;
    public nint FrictionCallback;
    public nint RestitutionCallback;
    [MarshalAs(UnmanagedType.I1)] public bool EnableSleep;
    [MarshalAs(UnmanagedType.I1)] public bool EnableContinuous;
    public uint WorkerCount;
    public nint EnqueueTask;
    public nint FinishTask;
    public nint UserTaskContext;
    public nint UserData;
    public nint CreateDebugShape;
    public nint DestroyDebugShape;
    public nint UserDebugShapeContext;
    public B3Capacity Capacity;
    public int InternalValue;
}

[StructLayout(LayoutKind.Sequential)]
public struct B3MotionLocks
{
    [MarshalAs(UnmanagedType.I1)] public bool LinearX;
    [MarshalAs(UnmanagedType.I1)] public bool LinearY;
    [MarshalAs(UnmanagedType.I1)] public bool LinearZ;
    [MarshalAs(UnmanagedType.I1)] public bool AngularX;
    [MarshalAs(UnmanagedType.I1)] public bool AngularY;
    [MarshalAs(UnmanagedType.I1)] public bool AngularZ;
}

public enum B3BodyType { StaticBody = 0, KinematicBody = 1, DynamicBody = 2, BodyTypeCount = 3 }

[StructLayout(LayoutKind.Sequential)]
public struct B3BodyDef
{
    public B3BodyType Type;
    public B3Pos Position;
    public B3Quat Rotation;
    public B3Vec3 LinearVelocity;
    public B3Vec3 AngularVelocity;
    public float LinearDamping;
    public float AngularDamping;
    public float GravityScale;
    public float SleepThreshold;
    public nint Name;
    public nint UserData;
    public B3MotionLocks MotionLocks;
    [MarshalAs(UnmanagedType.I1)] public bool EnableSleep;
    [MarshalAs(UnmanagedType.I1)] public bool IsAwake;
    [MarshalAs(UnmanagedType.I1)] public bool IsBullet;
    [MarshalAs(UnmanagedType.I1)] public bool IsEnabled;
    [MarshalAs(UnmanagedType.I1)] public bool AllowFastRotation;
    [MarshalAs(UnmanagedType.I1)] public bool EnableContactRecycling;
    public int InternalValue;
}

[StructLayout(LayoutKind.Sequential)]
public struct B3SurfaceMaterial
{
    public float Friction;
    public float Restitution;
    public float RollingResistance;
    public B3Vec3 TangentVelocity;
    public ulong UserMaterialId;
    public uint CustomColor;
    public uint Padding;
}

[StructLayout(LayoutKind.Sequential)] public struct B3Filter { public ulong CategoryBits; public ulong MaskBits; public int GroupIndex; }
[StructLayout(LayoutKind.Sequential)] public struct B3QueryFilter { public ulong CategoryBits; public ulong MaskBits; public ulong Id; public nint Name; }

[StructLayout(LayoutKind.Sequential)]
public struct B3ShapeDef
{
    public nint Name;
    public nint UserData;
    public nint Materials;
    public int MaterialCount;
    public B3SurfaceMaterial BaseMaterial;
    public float Density;
    public float ExplosionScale;
    public B3Filter Filter;
    [MarshalAs(UnmanagedType.I1)] public bool EnableCustomFiltering;
    [MarshalAs(UnmanagedType.I1)] public bool IsSensor;
    [MarshalAs(UnmanagedType.I1)] public bool EnableSensorEvents;
    [MarshalAs(UnmanagedType.I1)] public bool EnableContactEvents;
    [MarshalAs(UnmanagedType.I1)] public bool EnableHitEvents;
    [MarshalAs(UnmanagedType.I1)] public bool EnablePreSolveEvents;
    [MarshalAs(UnmanagedType.I1)] public bool InvokeContactCreation;
    [MarshalAs(UnmanagedType.I1)] public bool UpdateBodyMass;
    [MarshalAs(UnmanagedType.I1)] public bool EnableSpeculativeContact;
    public int InternalValue;
}

[StructLayout(LayoutKind.Sequential)] public struct B3Capsule { public B3Vec3 Center1; public B3Vec3 Center2; public float Radius; }
[StructLayout(LayoutKind.Sequential)] public struct B3PlaneResult { public B3Plane Plane; public B3Vec3 Point; }

[StructLayout(LayoutKind.Sequential)]
public struct B3HullData
{
    public ulong Version;
    public int ByteCount;
    public uint Hash;
    public B3Aabb Aabb;
    public float SurfaceArea;
    public float Volume;
    public float InnerRadius;
    public B3Vec3 Center;
    public B3Matrix3 CentralInertia;
    public int VertexCount;
    public int VertexOffset;
    public int PointOffset;
    public int EdgeCount;
    public int EdgeOffset;
    public int FaceCount;
    public int PlaneOffset;
    public int FaceOffset;
    public int SoaVertexOffset;
    public int SoaNormalOffset;
    public int Padding;
}

[StructLayout(LayoutKind.Sequential)]
public unsafe struct B3BoxHull
{
    public B3HullData Base;
    public fixed byte BoxVertices[8];
    public fixed float BoxPoints[24];
    public fixed byte BoxEdges[96];
    public fixed float BoxPlanes[24];
    public fixed byte BoxFaces[6];
    public fixed byte Padding[10];
    public fixed float Vx[8];
    public fixed float Vy[8];
    public fixed float Vz[8];
    public fixed float Nx[8];
    public fixed float Ny[8];
    public fixed float Nz[8];
}

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
[return: MarshalAs(UnmanagedType.I1)]
public delegate bool B3MoverFilterFcn(B3ShapeId shapeId, nint context);

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
[return: MarshalAs(UnmanagedType.I1)]
public unsafe delegate bool B3PlaneResultFcn(B3ShapeId shapeId, B3PlaneResult* planes, int planeCount, nint context);
