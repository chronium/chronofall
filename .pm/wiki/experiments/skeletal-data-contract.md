---
title: Skeletal Experiment Data Contract
createdAt: 2026-08-01T09:27:13.9177630Z
modifiedAt: 2026-08-01T17:09:05.7238440Z
---

## Status and ownership

The M1 data contract has been deliberately promoted by `pm://project/prj_E7QP3LUocfY7k3PYM-EQOlqc/task/SHARED-0001` into coordinator-owned `ChronoFall.CharacterPresentation`.

The library remains presentation-only and BCL-only: it has no SDL, GPU, SimpleMesh, Royale, Starfall, editor, server or simulation dependency. Headless gameplay must not reference it. The durable promoted module boundary is documented at `pm://project/prj_E7QP3LUocfY7k3PYM-EQOlqc/wiki/architecture/shared-character-presentation`.

`ChronoFall.CharacterExperiment.SimpleMesh` remains a separate provisional adapter that maps selected source data into the promoted types. The experiment SDL host and GPU harness remain validation consumers.

## Selected source shape

The canonical source remains `assets/Quaternius/Universal Animation Library[Standard]/Unreal-Godot/UAL1_Standard.glb`, recorded at `pm://project/prj_E7QP3LUocfY7k3PYM-EQOlqc/wiki/assets/skinned-character-experiment-inputs`.

The selected skin contains one parent-first 65-joint hierarchy with `root` at index 0, one inverse-bind matrix per joint, and at most four influences per vertex. The `Mannequin` mesh and `root` hierarchy are identity-space siblings beneath the `Armature` node. All selected clips fully key translation, rotation, and scale for all 65 joints with LINEAR interpolation.

## Public data contract

| Area | Types | Contract |
| --- | --- | --- |
| Local transforms | `JointTransform` | Finite translation and scale, non-zero finite normalized quaternion, and `Scale * Rotation * Translation` composition. |
| Skeleton | `SkeletonJoint`, `SkeletonDefinition` | One non-empty, ordinally unique, parent-first hierarchy. Joint 0 is the only root; every other parent index refers to an earlier joint. |
| Skin | `SkinDefinition` | Exactly one finite inverse-bind matrix per skeleton joint. |
| Vertex influences | `JointIndices4`, `SkinInfluences` | Four non-negative joint indices and four finite non-negative weights whose sum is within `1e-4` of one. All lanes must resolve against the selected skeleton, including zero-weight lanes. |
| Pose | `SkeletonPose` | Exactly one validated local transform per joint. `SkeletonDefinition.CreateBindPose` copies the joints' local bind transforms. |
| Animation | `Vector3Keyframe`, `QuaternionKeyframe`, LINEAR channels, `JointAnimationTrack`, `AnimationClip` | One ordered complete TRS track per joint, non-empty finite keyframes, strictly increasing non-negative times, normalized rotations, and duration derived from the latest terminal key. |
| Playback | `AnimationPlaybackMode`, `AnimationSampler` | Looping is an explicit caller decision (`Clamp` or `Loop`); clip names do not imply behavior. Sampling produces one complete local `SkeletonPose`. |
| Global pose | `SkeletonGlobalPose`, `SkeletonPoseEvaluator` | One finite parent-first global matrix per joint, retaining exact skeleton identity. |
| Palette | `SkinningPalette` | Exactly one finite CPU matrix per skin joint, created from a matching skin and global pose. It is not a GPU buffer layout or shader ABI. |

Aggregate constructors defensively copy caller collections and expose read-only views. Invalid caller-created contract data fails through `ArgumentException` or `ArgumentOutOfRangeException` with the offending property, count, joint, track, or lane where available. Asset-path, clip-name, node-name, and channel-path context remains the loader adapter's responsibility.

### Loader-owned mesh additions

| Area | Types | Contract |
| --- | --- | --- |
| Vertex | `SkinnedVertex` | Finite position and UV0, non-zero finite normal, and four validated skin influences. UV1 is intentionally outside M1. |
| Sections | `SkinnedMeshSection` | Non-empty material name plus a contiguous complete-triangle range. Material properties are not part of the experiment contract. |
| Mesh | `SkinnedMeshDefinition` | One immutable vertex buffer, one global index buffer, complete ordered section coverage, and one `SkinDefinition`. |
| Loaded asset | `SkeletalCharacterAsset` | One skinned mesh plus uniquely named animation clips that share the exact mesh skeleton. |

`ChronoFall.CharacterExperiment.SimpleMesh` is a separate provisional adapter assembly. It owns the SimpleMesh dependency and exposes `SimpleMeshSkeletalAssetLoader.LoadFromFile` plus structured `SkeletalAssetLoadException` context. This separation keeps the core data assembly BCL-only.

## Matrix convention

The promoted core uses `System.Numerics` row-vector matrices:

```text
local             = Scale * Rotation * Translation
posedGlobal[root] = local[root]
posedGlobal[j]    = local[j] * posedGlobal[parent[j]]
palette[j]        = inverseBind[j] * posedGlobal[j]
```

The provisional loader preserves glTF's right-handed, Y-up, metre-based model space. It does not flatten the skeleton or bake a coordinate conversion. For the selected M1 input, the loader asserts that the mesh and skeleton share the documented identity `Armature` space. A different relationship remains an unsupported input requiring a reviewed contract rather than silent normalization.

CPU matrices remain untransposed in `ChronoFall.CharacterPresentation`. `ChronoFall.CharacterPresentation.SdlGpu` owns its internal 48-byte vertex packing, palette transport, shader matrix layout and the single upload transpose. Camera framing, windows, targets, submission and captures remain host concerns. The exact native ABI and retained validation are recorded at `pm://project/prj_E7QP3LUocfY7k3PYM-EQOlqc/wiki/experiments/sdl-gpu-bind-pose`.

## Debug visualization boundary

The provisional skeleton diagnostic established by `pm://project/prj_E7QP3LUocfY7k3PYM-EQOlqc/task/EXPERIMENT-0007` consumes `SkeletonGlobalPose.GlobalTransforms` directly. Joint origins come from each evaluated global transform, hierarchy links use the skeleton's parent indices, and local RGB axes transform through the same evaluated matrices.

Debug geometry does not derive joint locations from the skinning palette: palette matrices include inverse binds and exist for vertex deformation, while global pose matrices describe the inspectable joint hierarchy. `ChronoFall.CharacterPresentation.SdlGpu` owns only skinned-mesh packing and drawing; diagnostic colors, axis scale, line rendering, depth behavior, captures and native validation remain private to `ChronoFall.CharacterExperiment.SdlGpu`. No debug-rendering dependency enters the BCL-only data contract.

## Animation boundary

`AnimationInterpolation` currently contains only `Linear`. STEP or CUBICSPLINE input is rejected by the provisional loader adapter established in `pm://project/prj_E7QP3LUocfY7k3PYM-EQOlqc/task/EXPERIMENT-0012`; unsupported modes are never silently represented as LINEAR data.

`AnimationClip.Duration` is the maximum terminal time across its complete joint tracks. `AnimationSampler.ResolveTime` rejects non-finite input and applies explicit caller-selected playback:

- `Clamp` maps into the inclusive `[0, duration]` interval;
- `Loop` uses Euclidean modulo, so negative time wraps correctly and exact duration boundaries resolve to zero.

`AnimationSampler.Sample` performs LINEAR vector interpolation and normalized shortest-path quaternion interpolation. Before a channel's first key it holds the first value; after its final key it holds the final value. The selected contract remains complete TRS, so no bind/default fallback is required.

`SkeletonPoseEvaluator.EvaluateGlobal` processes the parent-first hierarchy using the documented row-vector convention. `CreateSkinningPalette` requires exact skeleton identity and computes `inverseBind * posedGlobal` without transposition. These promoted APIs retain the semantics implemented by `pm://project/prj_E7QP3LUocfY7k3PYM-EQOlqc/task/EXPERIMENT-0004`.

The diagnostic composition root selects `Walk_Loop` by exact ordinal name, samples with explicit `Loop` playback, evaluates the parent-first pose and creates the untransposed CPU palette. It passes that palette to `ChronoFall.CharacterPresentation.SdlGpu`, which alone transposes matrices for upload. The visible clock and palette uploads remain presentation-owned; no animation state enters gameplay authority or a headless dependency.

Deterministic GPU checks use time zero, 0.5 seconds and exact duration. CPU tests require the 0.5-second palette to be finite and distinct from bind pose; exact duration must match time zero. Root motion, blending, retargeting and animation-graph contracts remain excluded.

## Explicit exclusions

- The BCL-only core contract does not reference SimpleMesh; the separate experiment adapter owns loading and the pinned dependency.
- GPU packing, shaders and SDL integration live only in the separate `ChronoFall.CharacterPresentation.SdlGpu` module; they are not part of the core contract.
- No asset conversion, cooking, source modification, retargeting, root motion, blending, modular armour, IK, animation graph or permanent cooked format.
- No window, camera, target, capture, debug-line, device-lifecycle or submission framework is promoted.
- No child source, PM data, build, commit, dependency or gitlink change.

## Validation

The coordinator solution targets .NET SDK 10.0.301. Core tests cover TRS order, hierarchy, immutable collections, finite values, influences, mesh indices/sections, shared skeleton identity, complete clips, and playback modes.

Core tests cover clamp and Euclidean loop boundaries, negative and non-finite time, LINEAR vector sampling, normalized shortest-path quaternion sampling, per-channel endpoint holding, complete local poses, parent-first global transforms, inverse-bind palette construction, finite global matrices, defensive copies, and skeleton identity mismatches.

Adapter tests verify the pin, license, reversible patch application, and absence of a SimpleMesh reference from the core project. Malformed-model tests cover unsupported interpolation, unresolved targets, empty and missing channels, non-finite values, and non-increasing key times with structured context.

The unchanged selected UAL1 GLB loads as 8,546 vertices, 41,232 indices, sections `M_Main` and `M_Joints`, one 65-joint skin, and 43 animations. `Idle_Loop`, `Walk_Loop`, and `Sword_Attack` each map to 65 complete LINEAR TRS tracks with their documented durations and sample counts.

`EXPERIMENT-0004` adds deterministic selected-asset fixtures for `Idle_Loop` at 1.25 seconds, `Walk_Loop` at 0.5 seconds, and `Sword_Attack` at 0.75 seconds, including root, pelvis, and left-hand evidence. Exact loop boundaries match time zero across all 65 joints. The selected bind pose produces identity palette matrices within `1e-4`; the measured maximum component error is approximately `7.2e-7`.

No renderer or native visual validation is part of this sampling task.