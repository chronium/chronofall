---
id: EXPERIMENT-0007
title: Add skeleton and joint debug visualization
track: EXPERIMENT
milestone: M1
dependsOn:
- EXPERIMENT-0005
createdAt: 2026-08-01T05:34:32.7408080Z
modifiedAt: 2026-08-01T14:52:59.8039660Z
---

Render the evaluated joint hierarchy and axes/links through focused debug visualization so pose and skinning defects are inspectable.

## Notes

- 2026-08-01 14:52 UTC - Implemented coordinator-only skeleton and joint debug visualization for the selected Quaternius UAL1 mannequin.

  - Deterministic CPU geometry consumes SkeletonGlobalPose directly: 64 yellow parent-child links plus RGB local axes for all 65 joints, totaling 259 lines / 518 vertices. Axis length is 4% of mesh-bound radius.
  - Added a 28-byte position/color line ABI, dedicated HLSL compiled to MSL/SPIR-V, an SDL GPU line-list buffer/pipeline, x-ray rendering after the mesh with depth disabled, explicit native lifecycle handling, and --skeleton-capture while preserving existing --capture behavior.
  - Added ABI, hierarchy, transform, color, selected-asset count, finite-data, and environment-gated native assertions.
  - Third-party verification passed for SimpleMesh and SDL3-CS.
  - ChronoFall solution build passed with 0 warnings and 0 errors.
  - Full solution tests passed: 45/45 (10 SDL GPU, 8 SimpleMesh adapter, 27 core).
  - Native macOS ARM64 Metal validation passed: 259 lines, 2,076 changed pixels, 872 yellow-link pixels, 349 green Y-axis pixels, skeleton fingerprint c6ad39a45245afed. Existing bind and palette-probe fingerprints remained 408d3a4c16278bbc and 4fd2e63aea97f7a3.
  - Agent inspected the deterministic capture. Owner inspected the native window on 2026-08-01 and confirmed: “that looks like a skeleton, fingers and all.”
  - PM validation and git diff checks passed. Royale, Starfall, and both coordinator gitlinks remained unchanged. No animation playback, shared promotion, generic debug framework, or child changes were included.