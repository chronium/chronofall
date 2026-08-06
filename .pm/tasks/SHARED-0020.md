---
id: SHARED-0020
title: Prove one rendered socketed static bow attachment
track: SHARED
milestone: M5
dependsOn:
- SHARED-0006
- SHARED-0018
- ASSET-0006
createdAt: 2026-08-02T16:19:38.0097790Z
modifiedAt: 2026-08-06T18:42:50.3171480Z
---

Prove the narrow shared contract for rendering one exact selected static bow from a posed character socket.

Acceptance boundary:
- Consume the completed BCL-only socket contract, the reusable static renderer, and the exact coordinator-acquired bow selection.
- Use a harness-local technical socket and local bow transform solely to prove the reusable shared rendering contract.
- Render one bow through the caller-owned SDL GPU frame lifecycle and validate transforms deterministically plus native macOS ARM64 appearance.
- Keep this proof narrow enough to be reviewed and reused by later SHARED-0007 broad attachment work.
- Preserve Starfall ownership of its provisional semantic hand socket, local bow transform, rendering integration, and native placement validation in CLIENT-0011.
- Do not include shields, backpacks, wings, armour, weapon IK, aiming, projectiles, gameplay integration, equipment semantics, generalized grip systems, generic attachment categories, or a scene framework.

## Notes

- 2026-08-06 18:42 UTC - Implemented the narrow coordinator-owned socketed static-bow proof.

  - Consumes the deterministic UAL1 technical humanoid cook and exact acquired Quaternius Bow_Wooden .cfmesh.
  - Resolves the posed hand_l socket and composes bowLocal * socketModel * characterWorld.
  - Renders the skinned humanoid and static bow in the same caller-owned SDL GPU command flow, render pass and depth target.
  - Freezes the owner-approved harness-local technical placement at twist 80 degrees, grip offset 0.09 m and palm-depth offset +0.03 m.
  - Keeps Starfall semantic placement, equipment, aiming, off-hand IK, arrows, projectiles and gameplay outside this task.
  - Deterministic native Metal evidence: bow pixels 15426/15493; fingerprints 8d01823335cf6f94, 4cb833897572116b and repeated 8d01823335cf6f94.
  - Validation: Debug and Release solution builds passed with zero warnings/errors; Debug and Release suites passed 283 tests; focused post-placement suite passed 42 tests; opt-in native macOS ARM64 SDL GPU suite passed 3 tests. Solution-wide dotnet format remains non-actionable because it traverses pinned upstream third-party projects with existing style differences; focused changed-project format checks passed.
  - The owner inspected the native proof from multiple angles and approved hand_l, 80-degree twist, 0.09-metre grip and +0.03-metre palm depth.
  - The owner selected the framed 2032 x 1220 native screenshot for permanent coordinator history at docs/project-history/2026-08-06-socketed-bow-proof/; preserved PNG SHA-256 eaf657827f8976407ef2747326064b0c661d3fd2064d60ebca8931e07a712063.
  - Royale and Starfall worktrees and gitlinks were not changed.