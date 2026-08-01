---
title: Shared Character Presentation Foundation
createdAt: 2026-08-01T17:05:19.5488560Z
modifiedAt: 2026-08-01T18:32:01.3586810Z
---

## Decision

`pm://project/prj_E7QP3LUocfY7k3PYM-EQOlqc/task/SHARED-0001` promotes the M1-proven character contracts into two focused coordinator-owned libraries:

| Module | Audience | Responsibility |
| --- | --- | --- |
| `ChronoFall.CharacterPresentation` | Client presentation and deterministic tools | Immutable skeleton, skin, mesh, animation, pose and palette data; sampling and pose evaluation |
| `ChronoFall.CharacterPresentation.SdlGpu` | SDL GPU clients only | Skinned geometry upload, per-instance palette transport, shaders, pipeline and draw recording |

Neither module depends on Royale or Starfall. The core module is BCL-only and has no SDL, importer, editor, simulation, server or child dependency. The SDL GPU module depends on the coordinator-owned SDL3-CS pin and the core module.

## Core contract

The promoted core preserves the validated M1 semantics:

- right-handed, Y-up, metre-based model space for the proven input envelope;
- four joint indices and four normalized weights per vertex;
- immutable parent-first skeletons with one root;
- explicit LINEAR translation, rotation and scale channels;
- deterministic clamp and Euclidean loop time resolution;
- endpoint holding, binary keyframe lookup and shortest-path quaternion interpolation;
- local transforms composed as scale, rotation, translation;
- global transforms evaluated parent first;
- palettes evaluated as inverse bind multiplied by posed global;
- global poses kept distinct from GPU palettes so diagnostics consume authoritative presentation transforms rather than shader-packed matrices.

These are presentation contracts. Server-authoritative gameplay state and events select animations; animation never decides attacks, hits, movement, equipment, damage, death or persistence.

## Focused pose blending

`pm://project/prj_E7QP3LUocfY7k3PYM-EQOlqc/task/SHARED-0008` adds one stateless full-body operation to the BCL-only core: `SkeletonPoseBlender.Blend(source, destination, amount)`.

Both poses must use the same skeleton instance and the amount must be finite within `[0, 1]`. The operation blends local translation and scale linearly and uses normalized shortest-path quaternion interpolation. It returns a new validated local pose. Global transforms, inverse binds and GPU palettes are evaluated only after blending through the existing contracts.

The shared module does not select clips, advance clocks, choose transition durations, queue or interrupt actions, interpret protocol messages, or decide gameplay. Each child consumes its own authoritative state and events and owns that presentation policy. The coordinator harness proves the math with `Idle_Loop`, `Walk_Loop` and a full-body `Sword_Attack`; its 0.25-second locomotion transition, 0.10-second action entry and 0.15-second action return remain provisional validation policy rather than shared API.

## Bounded binary masks and pose layering

`pm://project/prj_E7QP3LUocfY7k3PYM-EQOlqc/task/SHARED-0009` adds two focused BCL-only contracts for needs already declared by Royale `pm://project/prj__-jXLQgm6GuD2gCKZ_bTa1m-/task/GAME-018` and Starfall `pm://project/prj_pkIpzx0fzFD4URjvqBuYrGZF/task/CLIENT-0007`.

`SkeletonJointMask` binds copied binary membership to one skeleton instance. `CreateSubtree(skeleton, rootJointIndex)` uses the existing parent-first hierarchy to include exactly the selected joint and its descendants. The shared API does not assign anatomical meaning to joint names or choose an upper body.

`SkeletonPoseLayerer.Apply(basePose, layerPose, mask, amount)` returns a new validated local pose. All three inputs must use the same skeleton instance and the amount must be finite in `[0, 1]`. Unmasked transforms remain exactly from the base pose. Masked translation and scale interpolate linearly; masked rotation uses the same normalized shortest-path interpolation as full-body blending. Global poses and GPU palettes are evaluated only after composition.

This operation remains stateless. Children own masks, clocks, clips, action signals, interruption and transition policy. Binary membership plus one global amount is the approved envelope; weighted per-joint masks and general layer stacks are deferred until demonstrated.

## SDL GPU host boundary

The shared SDL layer accepts an existing `SDL_GPUDevice*`, target formats and caller-supplied MSL or SPIR-V bytecode. It exposes:

- `SdlGpuSkinnedCharacterRenderer`, which owns the skinned shaders and pipeline;
- `SdlGpuSkinnedMesh`, which owns immutable uploaded vertex and index buffers;
- `SdlGpuSkinningPalette`, which owns one character instance's palette storage and upload transfer buffer;
- `SkinnedCharacterDraw`, which supplies world, view-projection, flat colour and light direction;
- whole-mesh and indexed-section draw operations.

Uploads record copy passes into a caller-owned command buffer. Draws record into a caller-owned render pass. The caller owns SDL initialization, windows, devices, targets, depth allocation, command acquisition, pass ordering, submission, synchronization, cameras, frame scheduling and error context. Shared resources must be disposed before the caller destroys their device.

This boundary matches Royale's existing device and render-pass orchestration without making the shared module depend on Royale. Starfall may compose the same primitive through its own client lifecycle.

## Initial GPU ABI

The renderer deliberately hides its first GPU ABI behind public resource objects:

- 48-byte vertex stride;
- position at byte 0 as `float3`;
- normal at byte 12 as `float3`;
- four unsigned-short joint indices at byte 24;
- four float weights at byte 32;
- 32-bit indices;
- one structured-buffer palette binding;
- one matrix transpose at the SDL GPU upload boundary;
- back-face, counter-clockwise triangle rendering with caller-selected colour/depth formats and single-sample targets;
- simple flat colour and directional lighting.

The ABI is tested and reviewed for this foundation, but it is not a permanent cooked asset format or public vertex struct. Textures, additional attributes, material policy and animated bounds require later task-owned evidence.

## Experiment and dependency status

`ChronoFall.CharacterExperiment.SimpleMesh` remains the provisional M1 loader adapter. It maps the selected source GLB into the promoted core but SimpleMesh is not a dependency of either shared module and is not approved as a permanent importer.

`ChronoFall.CharacterExperiment.SdlGpu` remains the diagnostic host. It owns the window, camera, skeleton overlay, controls, offscreen targets, readback and captures while consuming the shared renderer for every character draw. The retained native fingerprints prove the shared path still produces the validated Metal output.

The coordinator continues to own its SDL3-CS pin, fetch verification, native runtime and shadercross workflow. Children retain their independently useful dependency acquisition until an explicit distribution contract is approved.

## Deferred contracts

The focused shared foundation still does not decide or implement:

- skeletal cooking or a permanent file format;
- package versioning, publication, feed selection or child acquisition;
- textures, production materials or animated bounds;
- blend trees, normalized locomotion parameters, a shared transition player, root motion, retargeting or animation graphs;
- weighted per-joint masks, named anatomical mask policy, arbitrary layer stacks or additive animation;
- modular armour, attachments, equipment, sockets or IK;
- a render graph, scene system, ECS or generic component framework;
- Royale or Starfall adapters, source changes or gitlink advancement.

Child integration remains owned by Royale `pm://project/prj__-jXLQgm6GuD2gCKZ_bTa1m-/task/RENDER-012` and Starfall `pm://project/prj_pkIpzx0fzFD4URjvqBuYrGZF/task/CLIENT-0006`. Their Plan-mode discussions must choose a reproducible distribution mechanism; they must not use a parent-relative project reference that breaks an independent child checkout.