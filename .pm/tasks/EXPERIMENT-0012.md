---
id: EXPERIMENT-0012
title: Establish coordinator skeletal loader adapter and scoped dependency acquisition
track: EXPERIMENT
milestone: M1
dependsOn:
- EXPERIMENT-0003
createdAt: 2026-08-01T09:10:26.9854900Z
modifiedAt: 2026-08-01T10:31:46.9591200Z
---

Establish coordinator-local acquisition for the approved SimpleMesh revision, add the focused LINEAR TRS/interpolation patch, and map loaded data into the experiment-only skeletal contract. Add deterministic selected-asset and failure tests. Do not add SDL, rendering, a new importer, a permanent format, or child dependencies.

Acceptance criteria:
- Pin SimpleMesh revision `9f46341e35fa5876fbea7b96bd021bc3abd7842d` in coordinator-local dependency-management files and preserve Apache-2.0 license evidence.
- Fetch into an ignored coordinator-local source directory; do not commit an upstream clone or reference `royale/thirdparty`.
- Add a focused, reproducible patch that exposes scale channels and retains interpolation metadata for translation, rotation, and scale.
- Map imported geometry, hierarchy, skin, inverse binds, joints/weights, and LINEAR TRS channels into the experiment-only data contract.
- Deterministically reject unsupported interpolation, unresolved targets, empty or malformed channels, non-finite values, and non-increasing key times with contextual errors.
- Load the selected UAL1 GLB without modifying it and verify the selected idle, locomotion, and attack clips retain all 65 translation, rotation, and scale channels.
- Add focused patch-application, selected-asset, and failure-path tests.
- Keep SDL/GPU, headless integration, root motion, retargeting, blending, permanent formats, new importers, and child changes out of scope.

## Notes

- 2026-08-01 10:31 UTC - Implemented the coordinator-local skeletal loader adapter and scoped SimpleMesh acquisition.

  Implementation:
  - Pinned SimpleMesh `9f46341e35fa5876fbea7b96bd021bc3abd7842d` from `https://github.com/CallumDev/SimpleMesh` under coordinator-owned `thirdparty/` management.
  - Preserved the Apache-2.0 license; the upstream checkout remains ignored at `thirdparty/repos/SimpleMesh`.
  - Added a reproducible patch exposing scale channels and retaining translation/rotation/scale interpolation metadata. Patched files carry ChronoFall modification notices.
  - Added `ChronoFall.CharacterExperiment.SimpleMesh` as the only SimpleMesh-consuming adapter; the core experiment assembly remains BCL-only.
  - Added immutable skinned vertex, section, mesh, and loaded-character asset contracts.
  - Added contextual `SkeletalAssetLoadException` rejection for unsupported interpolation, unresolved/duplicate targets, missing/duplicate/empty channels, non-finite values, non-increasing times, invalid hierarchy/skin data, and invalid geometry.
  - Updated the loader-decision and skeletal-data-contract wiki pages through PM MCP.

  Selected-asset evidence:
  - The source SHA-256 remained `69591853d817488edaa8fd9bf8fc1d821eaeaf789f8627b3cd23b41c4ed67997`.
  - Mapped 8,546 vertices, 41,232 indices, sections `M_Main` and `M_Joints`, one 65-joint skin, and all 43 animations.
  - `Idle_Loop`, `Walk_Loop`, and `Sword_Attack` each retain 65 complete LINEAR translation, rotation, and scale tracks with documented durations and sample counts.

  Validation:
  - Clean fetch/patch/verify completed twice; pin, origin, license text, patch diagnostics, and reverse applicability passed.
  - `dotnet restore ChronoFall.slnx` passed.
  - Debug build passed with 0 warnings/errors.
  - Release rebuild passed with 0 warnings/errors and consumed the Release SimpleMesh binary.
  - Debug and Release tests passed: 19/19 each (13 core, 6 adapter/acquisition).
  - Formatting verification passed for all four coordinator-owned projects. Solution-wide formatting is intentionally not used because it reports pre-existing formatting differences throughout ignored upstream SimpleMesh source.
  - PM MCP validation and `pm doctor` passed.
  - `git diff --check`, dependency/package audits, ignored-checkout verification, and SimpleMesh `git diff --check` passed.
  - Family inspection returned zero warnings. Royale and Starfall worktrees and gitlinks are unchanged.
  - No native or owner visual validation was required because this task adds no renderer or visual output.