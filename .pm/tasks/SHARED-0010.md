---
id: SHARED-0010
title: Define weapon grips and off-hand IK targets
track: SHARED
milestone: M2
dependsOn:
- SHARED-0006
createdAt: 2026-08-01T05:34:58.5131110Z
modifiedAt: 2026-08-02T05:53:52.4586700Z
---

Define coordinator-owned BCL-only weapon-local presentation data and deterministic model-space alignment for one required primary grip marker and one optional off-hand IK end-effector target. Children select sockets, weapons, hands, and authoritative gameplay state.

## Acceptance criteria

- `WeaponGripDefinition` stores one required rigid weapon-local primary grip transform and zero or one rigid weapon-local off-hand target transform.
- Authored grip and target frames require finite translation, normalized rotation, and identity local scale. They use the shared right-handed, Y-up, metre-based convention with local `+Z` forward and `+Y` up.
- `WeaponGripEvaluator.EvaluateModelSpace(definition, primarySocketModelTransform)` aligns the weapon-local grip marker to an evaluated socket as `weapon model = inverse(primary grip local) * primary socket model`.
- `WeaponGripPlacement` stores the finite weapon model transform and optional off-hand target model transform; the target resolves as `off-hand target local * weapon model`.
- Exact alignment is preserved as `primary grip local * weapon model = primary socket model` under the established row-vector convention.
- The shared API defines no handedness, anatomical joints, socket names, weapon IDs, stance/profile selection, IK chain, solver, or gameplay policy.
- Tests cover one- and two-handed definitions, invalid/default/scaled transforms, finite placement data, exact inverse alignment, optional target composition, and integration with synthetic `SkeletonSocketEvaluator` output.
- The shared presentation wiki records coordinate spaces, equations, optional-target semantics, authority, and downstream ownership.
- Focused formatting, Debug and Release builds/tests, assembly-boundary validation, and the unchanged opt-in native macOS ARM64 GPU regression pass.
- No weapon asset selection/import, serialization/cooking format, renderer, IK solve, protocol/gameplay change, child change, gitlink update, visual checkpoint, or owner visual gate is introduced.

## Notes

- 2026-08-02 05:53 UTC - Implemented the approved coordinator-owned BCL-only weapon grip boundary. Added WeaponGripDefinition with one required rigid weapon-local primary marker and one optional rigid off-hand target, finite WeaponGripPlacement data, and WeaponGripEvaluator.EvaluateModelSpace using inverse(primary grip local) * primary socket model. Deterministic tests cover one- and two-handed definitions, invalid/default/scaled frames, finite and presence-consistent placement data, exact primary alignment, optional target composition, and direct consumption of synthetic SkeletonSocketEvaluator output. No handedness, anatomy, socket name, weapon ID, profile selection, IK chain/solver, asset, renderer, serialization/cooking, protocol, or gameplay policy was added. Focused formatting passed for both changed projects. Debug and Release builds each completed with zero warnings and errors; each configuration passed 111 tests (68 core, 28 experiment SDL GPU, 9 SimpleMesh adapter, and 6 SDL GPU presentation). The opt-in native macOS ARM64 Metal suite passed all 28 tests. PM validation and pm doctor passed, the linked family reported zero warnings, git diff --check passed, both children remained clean at their pinned commits, and no gitlink changed. No rendered behavior or visual-history candidate was produced, so no owner visual gate was required.