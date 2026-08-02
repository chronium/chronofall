---
id: SHARED-0011
title: Define muzzle, projectile, casing, and aim reference points
track: SHARED
milestone: M2
dependsOn:
- SHARED-0006
createdAt: 2026-08-01T05:34:58.7348810Z
modifiedAt: 2026-08-02T05:41:20.6687510Z
---

Define coordinator-owned BCL-only attachment-local presentation frames for muzzle effects, visual projectile origins, casing ejection, and aiming. Child clients consume these frames while attacks, shots, trajectories, hits, damage, and casts remain server-owned outcomes.

## Acceptance criteria

- `AttachmentReferencePointRole` defines exactly `Muzzle`, `ProjectileOrigin`, `CasingEjection`, and `Aim`; these roles describe presentation use and never gameplay authority.
- `AttachmentReferencePointDefinition` stores an immutable caller-defined semantic name, typed role, and attachment-local rigid `JointTransform`.
- Reference frames require finite translation, normalized rotation, and identity local scale. They use the shared right-handed, Y-up, metre-based model convention with local `+Z` forward and `+Y` up.
- `AttachmentReferencePointSet` copies stable ordered definitions, permits an empty set and repeated roles, rejects duplicate names with ordinal comparison, and provides deterministic name and role lookup.
- `AttachmentReferencePointPose` stores one finite model-space transform per point with semantic-name lookup.
- `AttachmentReferencePointEvaluator.EvaluateModelSpace(set, attachmentModelTransform)` resolves `point local * attachment model`; world placement remains caller-owned as `point model * character world`.
- Tests cover copied inputs, empty sets, ordinal names, repeated-role ordering, invalid roles/transforms/scales, lookups, pose validation, exact transform order, and the `+Z` forward/`+Y` up convention.
- The shared presentation wiki records role meanings, transform/axis rules, authority, and downstream ownership.
- Focused formatting, Debug and Release builds/tests, assembly-boundary validation, and the unchanged opt-in native macOS ARM64 GPU regression pass.
- No asset selection, serialization/cooking format, attachment renderer, effect implementation, protocol/gameplay change, child change, gitlink update, visual checkpoint, or owner visual gate is introduced.

## Notes

- 2026-08-02 05:41 UTC - Implemented the approved coordinator-owned BCL-only attachment reference-point boundary. Added typed Muzzle, ProjectileOrigin, CasingEjection, and Aim roles; immutable named rigid local frames; copied stable sets with ordinal name and repeated-role lookup; finite model-space poses; and exact point-local * attachment-model evaluation. Local frames use identity scale, +Z forward, and +Y up. World placement, primary-point selection, content mapping, rendering, serialization/cooking, protocol, and all gameplay authority remain caller- or child-owned.

  Deterministic tests cover copied and empty sets, case-sensitive names, repeated role ordering, invalid roles/transforms/scales, lookup, pose copying/validation, exact transform order, and axis behavior. Focused formatting verification passed for the changed core and test projects. Debug and Release solution builds each completed with zero warnings and errors; each configuration passed 105 tests (62 core, 28 experiment SDL GPU, 9 SimpleMesh adapter, and 6 SDL GPU presentation). The opt-in native macOS ARM64 Metal suite passed all 28 tests. PM MCP validation and pm doctor passed, the linked family reported zero warnings, git diff --check passed, both children remained clean at their pinned commits, and no gitlink changed. No new rendered behavior or visual-history candidate was produced, so no owner visual gate was required.