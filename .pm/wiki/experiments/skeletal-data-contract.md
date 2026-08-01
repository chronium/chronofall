---
title: Skeletal Experiment Data Contract
createdAt: 2026-08-01T09:27:13.9177630Z
modifiedAt: 2026-08-01T09:27:13.9177630Z
---

## Status and ownership

`ChronoFall.CharacterExperiment` is the executable, coordinator-owned data contract for the M1 skinned-character proof. It is explicitly provisional: completing M1 may validate some contracts for later promotion, but this assembly is not itself a shared engine package.

The library is presentation-only and has no runtime package, SDL, GPU, SimpleMesh, Royale, Starfall, server, or simulation dependency. Headless gameplay must not reference it.

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
| Playback | `AnimationPlaybackMode` | Looping is an explicit caller decision (`Clamp` or `Loop`); clip names do not imply behavior. Time mapping and sampling are implemented later. |
| Palette | `SkinningPalette` | Exactly one finite CPU matrix per skin joint. It is not a GPU buffer layout or shader ABI. |

Aggregate constructors defensively copy caller collections and expose read-only views. Invalid caller-created contract data fails through `ArgumentException` or `ArgumentOutOfRangeException` with the offending property, count, joint, track, or lane where available. Asset-path, clip-name, node-name, and channel-path context remains the loader adapter's responsibility.

## Matrix convention

The experiment uses `System.Numerics` row-vector matrices:

```text
local             = Scale * Rotation * Translation
posedGlobal[root] = local[root]
posedGlobal[j]    = local[j] * posedGlobal[parent[j]]
palette[j]        = inverseBind[j] * posedGlobal[j]
```

The loader preserves glTF's right-handed, Y-up, metre-based model space. It does not flatten the skeleton or bake a coordinate conversion. For M1, the loader must assert the selected mesh and skeleton share the documented identity `Armature` space. A different relationship is an unsupported input requiring a reviewed contract rather than silent normalization.

CPU matrices remain untransposed. The later SDL GPU task owns vertex packing, palette transport, shader matrix layout, any upload transposition, skinned normal handling, and bounds.

## Animation boundary

`AnimationInterpolation` currently contains only `Linear`. STEP or CUBICSPLINE input is rejected by the loader adapter established in `pm://project/prj_E7QP3LUocfY7k3PYM-EQOlqc/task/EXPERIMENT-0012`; unsupported modes are never silently represented as LINEAR experiment data.

`AnimationClip.Duration` is the maximum terminal time across its complete joint tracks. The following deterministic work belongs to `pm://project/prj_E7QP3LUocfY7k3PYM-EQOlqc/task/EXPERIMENT-0004`:

- clamp and Euclidean loop time mapping;
- LINEAR vector interpolation and normalized shortest-path quaternion interpolation;
- bind/default fallback policy if the contract is later widened beyond complete TRS tracks;
- parent-first global hierarchy evaluation;
- inverse-bind palette calculation and bind-pose identity checks;
- selected timestamp fixtures.

## Explicit exclusions

- No GLB parsing, SimpleMesh pin, fetch script, patch, adapter, or asset-backed load test.
- No animation sampling, loop calculation, hierarchy evaluator, or palette calculator.
- No GPU vertex structure, buffer layout, shader, SDL dependency, renderer, native execution, or visual output.
- No retargeting, root motion, blending, modular armour, IK, animation graph, permanent cooked format, or shared-engine promotion.
- No child source, PM data, build, commit, or gitlink change.

## Validation

The coordinator solution targets .NET SDK 10.0.301. Its test project uses xUnit only as a test dependency. Focused tests cover TRS order, quaternion normalization, hierarchy structure, defensive copies, joint/count and finite-value validation, influence tolerance/range, complete ordered clip tracks, derived duration, channel ordering, and explicit playback modes.

`EXPERIMENT-0012` may now implement the approved loader adapter against these types. `EXPERIMENT-0004` may now add deterministic evaluation and sampling. Both remain separate owner-planned tasks.