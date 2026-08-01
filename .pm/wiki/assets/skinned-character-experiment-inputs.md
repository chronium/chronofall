---
title: Selected Skinned-Character Experiment Inputs
createdAt: 2026-08-01T07:59:11.4568220Z
modifiedAt: 2026-08-01T07:59:11.4568220Z
---

## Decision

The M1 skinned-character proof uses one supplied file:

`assets/Quaternius/Universal Animation Library[Standard]/Unreal-Godot/UAL1_Standard.glb`

- SHA-256: `69591853d817488edaa8fd9bf8fc1d821eaeaf789f8627b3cd23b41c4ed67997`
- Size: 7,618,436 bytes
- Format: glTF 2.0 GLB with embedded binary data and no external resources
- Exporter: `Khronos glTF Blender I/O v4.5.48`
- Provenance: Quaternius, CC0 1.0 Universal / public-domain dedication
- License evidence: the pack's committed `License.txt` and `README.txt`

This selection is the canonical input contract for M1. Related provenance is recorded at `pm://project/prj_E7QP3LUocfY7k3PYM-EQOlqc/wiki/assets/quaternius-provenance`.

## Selected objects

| Purpose | Object inside the GLB | Evidence |
| --- | --- | --- |
| Humanoid | node `Mannequin`, mesh `Mannequin` | The mesh node binds skin index 0 directly. |
| Skeleton and skin | skin `Armature` | 65 unique joints from `root` through `ball_leaf_r`, with 65 finite inverse-bind matrices. |
| Idle | animation `Idle_Loop` | 2.500000000 seconds, 76 samples per channel. |
| Locomotion | animation `Walk_Loop` | 1.333333373 seconds, 41 samples per channel. |
| Attack | animation `Sword_Attack` | 1.533333302 seconds, 47 samples per channel. |

The attack clip is included because compatibility is structural rather than inferred from naming: the mesh, skin, inverse binds, and clip targets are all in the same source file.

## Compatibility evidence

The GLB contains 67 nodes, one mesh, one skin, and 43 animations. The `Mannequin` mesh node references the exact `Armature` skin selected above.

The mesh has two triangle primitives. Both expose `POSITION`, `NORMAL`, `TEXCOORD_0`, `TEXCOORD_1`, `JOINTS_0`, and `WEIGHTS_0`. There is no second joint/weight set. Vertices use at most four non-zero influences, and measured weight sums range from `0.9999998808` to `1.0000000894`.

Each selected clip contains exactly 195 channels:

- 65 translation channels;
- 65 rotation channels;
- 65 scale channels;
- 65 unique target nodes, exactly equal to the selected skin's joint set;
- `LINEAR` interpolation on every sampler.

This avoids the unresolved rest-pose and inverse-bind differences between Universal Base Characters and the UAL mannequin. No retargeting or cross-file joint mapping is required for M1.

## Root motion and scale

The unsuffixed `UAL1_Standard.glb` is the pack's root-motion-disabled variant. The root translation is exactly zero at every sampled key in all three selected clips:

| Clip | Root samples | Maximum root-translation magnitude |
| --- | ---: | ---: |
| `Idle_Loop` | 76 | 0 |
| `Walk_Loop` | 41 | 0 |
| `Sword_Attack` | 47 | 0 |

M1 therefore uses in-place animation. The `_RM` source is explicitly excluded.

Scale channels are present, but their values are identity within exporter noise. The largest measured deviation from 1.0 is `4.768371582e-7`. This makes the selected clips suitable for the narrow experiment, but it does not authorize permanently discarding scale channels. The loader contract remains owned by `pm://project/prj_E7QP3LUocfY7k3PYM-EQOlqc/task/EXPERIMENT-0002`.

## Coordinate, material, and audience notes

The file follows glTF's right-handed, Y-up, metre-based convention. The mannequin is approximately 1.829 metres tall. It contains no images or textures and uses two simple PBR materials, `M_Main` and `M_Joints`.

The selected source is presentation input only. Render meshes, skinning data, materials, and animation must remain outside headless server and simulation artifacts.

## Explicit exclusions

M1 does not select, copy, convert, or repair:

- `UAL1_Standard_RM.glb`;
- Universal Base Characters;
- Universal Animation Library 2;
- modular outfits, armour, weapons, or equipment;
- cross-rig deformation or retargeting;
- the supplied broken external image URIs;
- a permanent skeletal format, importer, or general asset framework.

Universal Base Characters and UAL cross-rig compatibility remain deferred evidence, not a prerequisite for the same-file UAL1 proof.

## Downstream handoff

Pinned SimpleMesh loads this GLB directly and exposes one skinned geometry, one 65-bone skin, and all 43 animations. It currently omits the source scale channels. `EXPERIMENT-0002` must decide whether the focused experiment uses, supplements, or replaces that loading path and how unsupported data or interpolation is reported.

The detailed capability evidence is at `pm://project/prj_E7QP3LUocfY7k3PYM-EQOlqc/wiki/experiments/royale-skeletal-capability-evaluation`. This selection does not approve source changes, rendering work, conversion, shared-engine extraction, or a permanent dependency.

## Reproduction

From the coordinator root:

```sh
shasum -a 256 'assets/Quaternius/Universal Animation Library[Standard]/Unreal-Godot/UAL1_Standard.glb'
stat -f '%z bytes' 'assets/Quaternius/Universal Animation Library[Standard]/Unreal-Godot/UAL1_Standard.glb'
```

For structural checks, parse the GLB JSON and embedded BIN chunks, honor accessor and buffer-view offsets/strides, and assert that the selected mesh node binds the selected skin; that all joints have finite inverse binds; and that every selected clip targets the complete joint set with LINEAR TRS channels. Decode every root-translation and scale sample rather than comparing only the first and last keys.