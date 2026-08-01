---
id: SHARED-0006
title: Add bone sockets and attachment contracts
track: SHARED
milestone: M2
dependsOn:
- SHARED-0001
createdAt: 2026-08-01T05:34:57.5008700Z
modifiedAt: 2026-08-01T19:02:07.2074320Z
---

Define skeleton-bound semantic socket data and deterministic model-space resolution in the coordinator-owned BCL-only character-presentation core. Callers own character world placement, asset/content mapping, rendering, and gameplay policy.

## Acceptance criteria

- `SkeletonSocketDefinition` stores an immutable semantic name, joint index, and joint-local `JointTransform`; names are caller-defined and carry no built-in anatomical or equipment policy.
- `SkeletonSocketSet` binds a copied ordered socket collection to one skeleton instance, permits an empty set, validates joint indices, rejects duplicate names with ordinal comparison, and provides deterministic name-to-index lookup.
- `SkeletonSocketPose` stores one finite model-space matrix per socket and provides semantic-name lookup while preserving socket order and ownership.
- `SkeletonSocketEvaluator.EvaluateModelSpace(socketSet, globalPose)` requires the same skeleton instance and resolves each transform as `socket local * posed joint global` under the established row-vector convention.
- World placement remains caller-owned as `socket model * character world`; the shared API does not accept a world matrix or alter the SDL GPU renderer contract.
- Core tests prove copied immutable inputs, empty sets, name/index validation, lookup, exact transform order, mismatched skeleton rejection, matrix validation, and pose immutability.
- A selected-asset test uses the unchanged `UAL1_Standard.glb`, semantic sockets over `hand_r` and `spine_03`, and sampled `Sword_Attack` poses to prove exact finite composition and animated hand movement.
- Focused formatting, Debug and Release builds/tests, and the unchanged opt-in native macOS ARM64 Metal regression pass; shared-presentation wiki and task notes record the contract and evidence.
- No serialization/cooking format, attachment mesh, asset selection, equipment slot, grip, IK, effect/aim point, socket visualization, renderer/shader change, child change, or gitlink update is introduced.

## Notes

- 2026-08-01 19:02 UTC - Implemented the approved BCL-only skeleton socket boundary. Added immutable `SkeletonSocketDefinition`, `SkeletonSocketSet`, and `SkeletonSocketPose` data plus `SkeletonSocketEvaluator.EvaluateModelSpace`, with ordinal semantic lookup and exact `socket local * posed joint global` composition. World placement remains caller-owned; no renderer, shader, world-transform, equipment, grip, IK, effect-point, serialization, cooking, or child contract was added. Core tests cover copied inputs, empty sets, ordinal names, invalid data, exact composition, mismatched skeletons, lookup, and finite immutable socket poses. The selected-asset proof loads the unchanged CC0 `UAL1_Standard.glb`, maps `primary-hand` to `hand_r` and `back` to `spine_03`, samples `Sword_Attack`, verifies exact finite model transforms, and confirms the animated hand socket moves. Focused formatting passed. Debug and Release builds each succeeded with zero warnings and each passed 100 tests (57 core, 28 experiment, 9 SimpleMesh selected-loader, 6 SDL GPU). The unchanged opt-in native macOS ARM64 Metal integration test passed. PM validation and doctor passed with zero family warnings. Royale and Starfall remained clean at their pinned commits, with no child or gitlink changes. No visual artifact or owner visual gate was required because the task intentionally added no rendered behavior.