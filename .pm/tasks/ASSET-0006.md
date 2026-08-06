---
id: ASSET-0006
title: Acquire exact Draft 0 bow and arrow inputs
track: ASSET
milestone: M5
dependsOn:
- SHARED-0019
- SHARED-0017
- pm://project/prj_pkIpzx0fzFD4URjvqBuYrGZF/task/CONTENT-0011
createdAt: 2026-08-02T16:19:38.7257910Z
modifiedAt: 2026-08-06T17:46:10.6242610Z
---

Acquire and stage the exact Quaternius Medieval Weapons Pack inputs selected by Starfall CONTENT-0011: Bow_Wooden and Arrow.

Acceptance boundary:
- Record exact pack-relative OBJ and MTL paths, SHA-256 hashes, CC0 evidence, material sections, source orientation and pivot behavior.
- Add deterministic client-only .cfmesh recipes for the two selected objects at 0.25 metres per source unit.
- Extend the established stable-project-ID workflow with a fixed allowlist that stages only the two cooked meshes, deterministic provenance and preserved licence evidence into Starfall's ignored generated client tree.
- Prove byte-identical cooking and exact cooked bounds through focused tests, then obtain owner native scale confirmation against a 1.8 metre reference.
- Preserve the supplied source files unchanged; do not copy raw models or import the complete weapon pack.
- Do not define Starfall equipment, sockets, grip, aim, projectile behavior, combat, materials/textures or final presentation.

## Notes

- 2026-08-06 17:46 UTC - ## Completion evidence — 2026-08-06

  - Added exact deterministic client recipes for Quaternius Medieval Weapons Pack `Bow_Wooden.obj` and `Arrow.obj`, including their MTL and CC0 licence hashes. Both cook at `0.25` metres per source unit through the established `section-names-only` .cfmesh v1 boundary.
  - Preserved source files unchanged and selected no other pack content. The staging script copies no OBJ, MTL, Blend, FBX or GLB.
  - Extended stable-project-ID staging for Starfall `prj_pkIpzx0fzFD4URjvqBuYrGZF`. The ignored client tree contains exactly the existing UAL1 cook plus the bow/arrow .cfmesh files, deterministic provenance and the two preserved licence sets; no tracked child file or gitlink changed.
  - Bow output: 43,185 bytes, SHA-256 `4c0ab766e7c622c0f52ff0ade3cb1992c6d96664233a4695fc049a3a9b1d642e`.
  - Arrow output: 11,492 bytes, SHA-256 `4eeb80dc06e1f729b67606eb6c12110b954068cfb7ea39590706771e4c02d9c3`.
  - Focused deterministic tests passed: StaticMeshCooker 10/10, CharacterPresentation 90/90, CharacterPresentation.Cooking 18/18 and CharacterPresentation.SdlGpu 21/21. `ChronoFall.slnx` Release build passed with 0 warnings and 0 errors. Shell syntax and `git diff --check` passed.
  - An unsaved native Blender comparison used the exact supplied Blend meshes at the same uniform 0.25 scale beside a 1.8 metre reference. The measured bow was approximately 1.36 metres and the arrow approximately 0.68 metres. The owner confirmed that both sizes and proportions make complete sense. This is scale evidence, not an equipped socket/grip proof.
  - Created `assets/quaternius-medieval-weapons-bow-arrow-cook` and updated the shared static-cooking and family-source-consumption wiki contracts.
  - PM MCP validation passed and linked-family inspection returned all three projects available/readable/trusted with zero structured warnings. `pm doctor` passed in all three repositories; only the existing legacy milestone-schema warnings and Starfall's existing empty M3 warning remain.
  - Coordinator owns every changed source, recipe, script, test, PM and wiki path. Starfall and Royale worktrees remained clean at pins `db60c0dcfb1421bfc1e5ceec2918f0998fe7f3e3` and `3b1bc45e4c8be76d110d8cf9613284db342db42e`.