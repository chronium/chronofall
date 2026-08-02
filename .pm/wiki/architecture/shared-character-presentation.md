---
title: Shared Character Presentation Foundation
createdAt: 2026-08-01T17:05:19.5488560Z
modifiedAt: 2026-08-02T17:14:01.0646120Z
---

## Decision

`pm://project/prj_E7QP3LUocfY7k3PYM-EQOlqc/task/SHARED-0001` promotes the M1-proven character contracts into a focused coordinator-owned module family:

| Module | Audience | Responsibility |
| --- | --- | --- |
| `ChronoFall.CharacterPresentation` | Client presentation and deterministic tools | Immutable skeleton, skin, mesh, animation, pose and palette data; sampling and pose evaluation |
| `ChronoFall.CharacterPresentation.Cooking` | Client build/runtime asset boundary | Provisional deterministic `.cfskel` descriptor, writer and reader over the promoted data contract |
| `ChronoFall.CharacterPresentation.SdlGpu` | SDL GPU clients only | Skinned geometry upload, per-instance palette transport, shaders, pipeline and draw recording |

None of these modules depends on Royale or Starfall. The core and cooking modules are BCL-only; the core has no cooking, SDL, importer, editor, simulation, server or child dependency, and cooking depends only on the core. The SDL GPU module depends on the coordinator-owned SDL3-CS pin and the core module.

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

## Skeleton sockets and attachment transform boundary

`pm://project/prj_E7QP3LUocfY7k3PYM-EQOlqc/task/SHARED-0006` adds a BCL-only model-space contract for stable semantic sockets without selecting or rendering equipment.

`SkeletonSocketDefinition` associates one caller-defined semantic name with a runtime joint index and a joint-local `JointTransform`. `SkeletonSocketSet` binds an ordered copied collection to exactly one skeleton instance, permits an empty set, validates joint indices, rejects duplicate names with ordinal comparison, and provides deterministic name-to-index lookup. The core defines no reserved names, anatomical policy, equipment slots, or joint-name mapping.

`SkeletonSocketEvaluator.EvaluateModelSpace(socketSet, globalPose)` requires the set and pose to share the same skeleton instance. It resolves every socket in stable set order as:

```text
socketModel = socketLocal * posedJointGlobal
```

`SkeletonSocketPose` retains those finite model-space matrices and provides semantic-name lookup. The evaluator consumes `SkeletonGlobalPose`, never inverse-bind or skinning-palette matrices. Under the established row-vector convention, a client that needs final placement composes `socketModel * characterWorld`; character world selection and validation remain caller-owned and this contract does not alter the SDL GPU renderer.

The unchanged `UAL1_Standard.glb` selected-asset proof maps `primary-hand` to `hand_r` and `back` to `spine_03`, resolves both from sampled `Sword_Attack` poses, proves the exact offset composition, and confirms the hand socket moves during the action. These names and offsets are test evidence only, not canonical game content.

Rendering representative attachments remains `SHARED-0007`; weapon grips and off-hand targets remain `SHARED-0010`; effect and aim reference points remain `SHARED-0011`; socket/equipment/IK visualization remains `SHARED-0013`. Serialization, cooking, child equipment schemas, and gameplay ownership also remain outside this contract.

## Weapon grip alignment and off-hand target boundary

`pm://project/prj_E7QP3LUocfY7k3PYM-EQOlqc/task/SHARED-0010` adds BCL-only weapon-local grip data and deterministic model-space placement without selecting a weapon asset or implementing IK.

`WeaponGripDefinition` contains one required primary grip frame and zero or one off-hand target frame. Both are rigid weapon-local `JointTransform` values with identity scale. They use the shared right-handed, Y-up, metre-based convention; local `+Z` is forward and local `+Y` is up.

The primary frame marks the actual grip location and orientation inside weapon space. It is not a direct placement offset. Given a caller-selected, already-evaluated skeleton socket, `WeaponGripEvaluator.EvaluateModelSpace` resolves:

```text
weaponModel = inverse(primaryGripLocal) * primarySocketModel
primaryGripLocal * weaponModel = primarySocketModel
offHandTargetModel = offHandTargetLocal * weaponModel
```

The second line is the alignment invariant. `WeaponGripPlacement` retains the finite weapon model transform and the optional finite off-hand target model transform. A one-handed definition produces no target. A two-handed definition produces one target frame for a later presentation IK operation.

The core assigns no right or left hand, anatomical joint, socket name, weapon ID, stance, grip profile, animation, or IK chain. Children select the primary socket and decide which presentation arm consumes the optional target. `SHARED-0012` owns the bounded two-bone IK and aim-offset behavior; Royale integration remains `pm://project/prj__-jXLQgm6GuD2gCKZ_bTa1m-/task/RENDER-013`.

Grip alignment is client presentation. It never decides equipment ownership, attacks, shots, casts, trajectories, hits, or damage. No asset selection, serialization/cooking format, renderer, protocol event, or gameplay rule is part of this contract. An aligned `WeaponGripPlacement.WeaponModelTransform` may later be passed to attachment rendering and `AttachmentReferencePointEvaluator`, but those integrations remain separately owned.

## Attachment effect and aim reference frames

`pm://project/prj_E7QP3LUocfY7k3PYM-EQOlqc/task/SHARED-0011` adds BCL-only attachment-local presentation frames without defining a weapon, projectile, effect, or gameplay model.

`AttachmentReferencePointRole` defines four presentation meanings:

| Role | Presentation meaning |
| --- | --- |
| `Muzzle` | Origin and orientation for client muzzle flash, smoke, light, or audio placement |
| `ProjectileOrigin` | Origin and orientation for a visual projectile or tracer; it is not the authoritative shot origin |
| `CasingEjection` | Origin and orientation for optional client-owned casing effects |
| `Aim` | Reference frame for client aim alignment, aim offsets, or IK presentation |

`AttachmentReferencePointDefinition` combines one of those roles with a caller-defined semantic name and an attachment-local rigid `JointTransform`. Local scale must be identity. Frames inherit the shared right-handed, Y-up, metre-based convention; local `+Z` is forward and local `+Y` is up.

`AttachmentReferencePointSet` copies definitions in stable order, permits an empty set, rejects duplicate names using ordinal comparison, and permits multiple points with the same role. The core provides stable name and role lookup but does not reserve names, select a primary point, or decide which points a child uses.

`AttachmentReferencePointEvaluator.EvaluateModelSpace(referencePointSet, attachmentModelTransform)` resolves:

```text
pointModel = pointLocal * attachmentModel
pointWorld = pointModel * characterWorld
```

The evaluator produces only the first line. Character world placement remains caller-owned, matching the socket boundary. Reference points are presentation metadata consumed after a child receives authoritative state or events. They never determine whether an attack, shot, projectile, cast, hit, damage result, or trajectory exists. If visual placement differs from authoritative gameplay, the child must preserve the authoritative outcome.

No weapon asset, serialization/cooking format, renderer, effect implementation, protocol event, or game-specific selection is part of this contract. Shared two-bone IK and aim behavior remains `SHARED-0012`; Royale integration remains `pm://project/prj__-jXLQgm6GuD2gCKZ_bTa1m-/task/RENDER-013`.

## Bounded two-bone IK and one-joint aim offsets

`pm://project/prj_E7QP3LUocfY7k3PYM-EQOlqc/task/SHARED-0012` adds two stateless BCL-only presentation operations over the approved grip and Aim-reference frames.

### Two-bone IK

`TwoBoneIkChain` binds a skeleton instance to direct root, middle and end joint indices. The shared API assigns no anatomical names or handedness. `TwoBoneIkSolver.ApplyModelSpace` consumes a same-skeleton local pose, the full off-hand target model transform produced by grip placement, a model-space pole position and an amount in `[0, 1]`.

At full amount, the solver:

1. evaluates current model-space root, middle and end positions;
2. derives the two current segment lengths;
3. clamps the requested target distance into `[abs(first - second), first + second]`;
4. selects the bend side from the pole, falling back to the current bend plane and then a deterministic perpendicular axis only when degenerate;
5. solves the desired middle position with the law of cosines;
6. rotates root and middle toward the solved segments and aligns the end-joint model orientation to the target.

All local translations and scales remain exactly from the source pose. Only root, middle and end local rotations can change; every unrelated local transform remains exact. Partial amounts use normalized shortest-path rotation interpolation. Inputs must be finite, direct-chain segments must have non-zero model-space length, and transforms must be decomposable with positive near-uniform scale. The bounded solver is not a general constraint graph and defines no joint limits, twist distribution, collision avoidance or gameplay result.

### Aim offset

`AimOffsetEvaluator.EvaluateModelSpace` consumes a finite Aim reference model transform, a finite non-zero desired model-space direction and symmetric `AimOffsetLimits`. It interprets local `+Z` as forward and `+Y` as up; positive yaw points toward `+X` and positive pitch toward `+Y`. It returns clamped yaw and pitch, whether clamping occurred, and the normalized roll-free model-space rotation delta.

`AimOffsetApplier.ApplyModelSpace` applies that delta to exactly one caller-selected joint with an amount in `[0, 1]`. It preserves every local translation and scale and every unselected local transform. Children choose the joint, target direction, limits, timing and whether to compose the operation across more than one joint; weighted spine distribution is not shared policy.

The caller-owned ordering is:

```text
sample/blend/layer pose
-> optional aim offset
-> evaluate global pose and primary socket
-> place weapon and its Aim/off-hand frames
-> optional off-hand two-bone IK
-> evaluate final global pose and GPU palette
```

Aim and IK consume client presentation inputs only. They never determine an attack, shot, cast, trajectory, hit, damage result or equipment state. The selected-rig proof uses `spine_03`, `upperarm_l -> lowerarm_l -> hand_l` and `hand_r` only as provisional harness mappings; they are not shared anatomy or content identifiers.

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

## Provisional skeletal cooking boundary

`pm://project/prj_E7QP3LUocfY7k3PYM-EQOlqc/task/SHARED-0002` adds one narrow client-only cooking boundary. `ChronoFall.CharacterPresentation.Cooking` writes and reads a deterministic version 1 `.cfskel` container containing source provenance plus the promoted mesh, skeleton, skin and animation data. It reconstructs the same immutable core types and preserves every selected single-precision contract value exactly.

The first committed recipe selects `Mannequin`, `Armature`, `Idle_Loop`, `Walk_Loop` and `Sword_Attack` from the unchanged supplied `UAL1_Standard.glb`. The build-time cooker verifies the source hash, CC0 evidence and embedded identifiers, then uses the provisional SimpleMesh adapter. SimpleMesh remains outside all shared runtime assemblies.

Generated cooks remain ignored under `artifacts/`. No cooked binary is committed or placed in a runtime manifest. The CLI accepts only client audience; server and simulation artifacts receive no skeletal presentation content or dependency. The exact recipe, format envelope, reproduction command, output hash and validation evidence are documented at `pm://project/prj_E7QP3LUocfY7k3PYM-EQOlqc/wiki/assets/shared-skeletal-cooking`.

Version 1 is deliberately provisional. It does not cook textures, production materials, UV1, animated bounds, sockets, equipment, grips, reference points, IK metadata, masks, layers or animation graphs, and it establishes no package publication or child-acquisition mechanism.

## Experiment and dependency status

`ChronoFall.CharacterExperiment.SimpleMesh` remains the provisional M1 loader adapter. It maps the selected source GLB into the promoted core and is consumed only by the build-time cooker and experiment validation. SimpleMesh is not a dependency of the core, cooked-format, SDL GPU, or child runtime assemblies and is not approved as a permanent importer.

`ChronoFall.CharacterExperiment.SdlGpu` remains the diagnostic host. It owns the window, camera, skeleton overlay, controls, offscreen targets, readback and captures while consuming the shared renderer for every character draw. The retained native fingerprints prove the shared path still produces the validated Metal output.

`pm://project/prj_E7QP3LUocfY7k3PYM-EQOlqc/task/SHARED-0016` establishes the first family source-consumption boundary. Approved child clients reference the core, cooking, and SDL GPU projects through the single `ChronoFallFamilyRoot` property in the canonical coordinator checkout. Repository and product ownership remain independent; full client build isolation outside that checkout is not currently required.

The coordinator continues to own its SDL3-CS pin, fetch verification, native runtime, and shadercross workflow. The SDL GPU project compiles that checked-out source directly, and children receive it transitively rather than through a direct reference or package. The stable-ID client staging workflow and generated-content layout are documented at `pm://project/prj_E7QP3LUocfY7k3PYM-EQOlqc/wiki/development/family-source-consumption`.

## Draft 0 static presentation and attachment path

`pm://project/prj_E7QP3LUocfY7k3PYM-EQOlqc/task/SHARED-0018` adds the first focused static-mesh boundary for the Starfall Draft 0 bow and bounded environment-prop needs without selecting or acquiring any asset.

`ChronoFall.CharacterPresentation` now owns `StaticVertex`, `StaticMeshSection`, and `StaticMeshDefinition`. The immutable BCL-only definition contains finite positions, non-zero normals, 32-bit indexed triangles, and ordered contiguous sections that exactly cover the index buffer. A section's `MaterialName` preserves an opaque source/cook diagnostic identity; it is not a runtime material object or asset catalogue.

`ChronoFall.CharacterPresentation.SdlGpu` owns `SdlGpuStaticMeshRenderer`, `SdlGpuStaticMesh`, `StaticMeshDraw`, and `SdlGpuStaticShaderSet`. The renderer:

- uses an internal 24-byte vertex ABI: position `float3` at byte 0 and normal `float3` at byte 12;
- uploads immutable vertex and 32-bit index buffers into caller-owned command buffers;
- records whole-mesh or section draws into caller-owned render passes;
- uses caller-supplied opaque RGB colour and directional light as the only material inputs;
- accepts MSL or SPIR-V bytecode and caller-selected colour/depth formats;
- retains counter-clockwise back-face culling, single-sample targets, depth testing and depth writes;
- accepts translation, rotation and positive uniform scale, while rejecting reflection, shear, singular matrices and non-uniform scale.

The caller still owns SDL initialization, device, window, targets, command-buffer acquisition/submission, render-pass lifetime, camera, draw ordering, scheduling and disposal order. Static GPU resources remain client-only and must be disposed before the caller destroys the device. The shared projects remain independent of Starfall, Royale, SimpleMesh and supplied asset packs.

The diagnostic host exposes an isolated `--static-proof` mode over deterministic synthetic two-section box geometry. It renders a baseline, transformed probe and repeated baseline, verifies section colour visibility, world-transform consumption and byte-identical repeatability, and can write `--static-capture <path>` or open the fixed native window with `--visible`. This proof neither imports nor claims compatibility with a selected bow, village, nature prop or other source asset.

`SHARED-0019` remains the owner of a provisional deterministic exact-selection cook that feeds this boundary. UVs, textures, samplers, PBR properties, alpha/blending, two-sided materials, bounds, instancing, engine vegetation/wind shaders and production material policy require later evidence and are not part of this contract.

`SHARED-0020` combines the completed model-space socket contract, this narrow static renderer, and the exact selected/acquired bow to prove one rendered socketed attachment. Starfall retains bow identity, content mapping, equipment, combat, aiming, and presentation integration. The proof excludes armour, IK, projectiles, shields, backpacks, and wings.

The broader `SHARED-0007` task remains the owner of representative weapons, shields, backpacks, wings, and later proven attachment categories. It depends on `SHARED-0020` and must review and reuse the narrow proof rather than independently recreate the same capability. Its existing downstream consumers remain intact.

Coordinator task and acquisition graph: `pm://project/prj_E7QP3LUocfY7k3PYM-EQOlqc/wiki/roadmap/starfall-draft-0-shared-enablers`.

## Deferred contracts

The focused shared foundation still does not decide or implement:

- stabilization, compatibility guarantees, compression, streaming, or replacement of the provisional `.cfskel` format;
- package versioning, publication, feed selection, independent checkout distribution, or a content-package contract;
- textures, production materials or animated bounds;
- blend trees, normalized locomotion parameters, a shared transition player, root motion, retargeting or animation graphs;
- weighted per-joint masks, named anatomical mask policy, arbitrary layer stacks or additive animation;
- socket, grip, reference-point or IK serialization and cooking, modular armour, rendered attachments, equipment schemas, multiple grip profiles or presentation debugging;
- shared anatomical chain names, joint-limit policy, twist distribution, multiple simultaneous constraints, weighted multi-joint aim distribution, collision avoidance or a general constraint graph;
- a render graph, scene system, ECS or generic component framework;
- Royale or Starfall adapters, source changes or gitlink advancement.

Child integration remains owned by Royale `pm://project/prj__-jXLQgm6GuD2gCKZ_bTa1m-/task/RENDER-012` and Starfall `pm://project/prj_pkIpzx0fzFD4URjvqBuYrGZF/task/CLIENT-0006`. They consume only the approved source allowlist through `ChronoFallFamilyRoot` and the ignored generated client output through the stable-ID coordinator workflow. Each child owns its references, runtime mapping, validation, and implementation commit. The coordinator then records the reviewed child commit through an automatic pointer-only commit in the same approved cycle, with no separate PM task.