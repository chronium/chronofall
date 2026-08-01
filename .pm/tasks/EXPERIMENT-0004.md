---
id: EXPERIMENT-0004
title: Add deterministic transform and animation sampling tests
track: EXPERIMENT
milestone: M1
dependsOn:
- EXPERIMENT-0003
- EXPERIMENT-0012
createdAt: 2026-08-01T05:34:32.0438110Z
modifiedAt: 2026-08-01T12:04:23.8895450Z
---

Implement deterministic experiment-only animation sampling, hierarchy evaluation, and CPU palette generation before GPU rendering relies on them.

Acceptance criteria:
- Depend on the completed skeletal data contract and coordinator loader adapter tasks.
- Resolve finite sample time with explicit Clamp and Euclidean Loop behavior; exact loop-duration boundaries map to zero and negative loop time wraps correctly.
- Sample complete LINEAR translation, rotation, and scale tracks into a SkeletonPose.
- Hold channel endpoints outside each channel's key range and use normalized shortest-path quaternion interpolation.
- Evaluate parent-first global transforms with the established System.Numerics row-vector local * parentGlobal convention.
- Compute one CPU skinning matrix per joint as inverseBind * posedGlobal and reject mismatched skeleton identities.
- Add synthetic deterministic tests for time mapping, interpolation, hierarchy, bind pose, inverse binds, invalid inputs, and mismatches.
- Load the unchanged selected UAL1 GLB and verify deterministic bind-pose and selected clip timestamp fixtures.
- Keep the core experiment BCL-only and exclude SDL/GPU, rendering, root motion, blending, retargeting, asset changes, child changes, and shared-engine promotion.
- Update durable experiment documentation and task validation notes.

## Notes

- 2026-08-01 12:04 UTC - Implemented deterministic animation sampling and pose evaluation.

  Implementation:
  - Added AnimationSampler.ResolveTime and Sample with explicit Clamp and Euclidean Loop behavior, per-channel endpoint holding, LINEAR vector interpolation, and normalized shortest-path quaternion interpolation.
  - Added immutable SkeletonGlobalPose plus SkeletonPoseEvaluator.EvaluateGlobal and CreateSkinningPalette using the documented row-vector hierarchy and inverseBind * posedGlobal convention.
  - Kept ChronoFall.CharacterExperiment BCL-only with no package or project references.
  - Updated the skeletal data-contract wiki through PM MCP.

  Selected-asset evidence:
  - The unchanged UAL1 GLB bind pose produces 65 identity palette matrices within 1e-4; measured maximum component error was approximately 7.2e-7.
  - Hard-coded fixtures cover Idle_Loop at 1.25 seconds, Walk_Loop at 0.5 seconds, and Sword_Attack at 0.75 seconds using root, pelvis, and hand_l evidence.
  - Exact loop-duration boundaries match time zero across all 65 joints.

  Validation:
  - dotnet restore ChronoFall.slnx passed.
  - Debug and Release builds passed with 0 warnings/errors.
  - Debug and Release tests passed 35/35 each: 27 core and 8 adapter/selected-asset tests.
  - First-party dotnet format verification, SimpleMesh acquisition verification, git diff --check, PM MCP validation, and pm doctor passed.
  - Dependency audits confirmed no packages, project references, SimpleMesh, SDL, GPU, child, server, or simulation references in the core experiment.
  - Family inspection returned zero warnings; Royale and Starfall worktrees and gitlinks are unchanged.
  - Final coordinator review found no defects. Native and owner visual validation were not required because this task adds no renderer or visual output.