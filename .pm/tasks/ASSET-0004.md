---
id: ASSET-0004
title: Acquire exact Draft 0 archer and bow-animation inputs
track: ASSET
milestone: M5
dependsOn:
- SHARED-0002
- SHARED-0017
- pm://project/prj_pkIpzx0fzFD4URjvqBuYrGZF/task/CONTENT-0011
- EXPERIMENT-0014
createdAt: 2026-08-02T16:19:38.2551810Z
modifiedAt: 2026-08-07T05:58:24.8352770Z
---

After Starfall completes its archer selection and the bounded technical bow-body proof completes, acquire and stage only the exact approved base/underlayer and minimum compatible bow-animation inputs with coordinator-owned provenance.

Acceptance boundary:
- Consume the canonical completed Starfall selection, `ASSET-0010` private-source inventory, and `EXPERIMENT-0014` visual evidence; record every exact pack-relative path, source hash, supplied licence/readme evidence, embedded identifier, rig/rest-transform compatibility result, and selected clip.
- Keep the complete owner-supplied UAL2 Source package private. Inventory and proof do not make the entire library a production input, and no absolute source path or source-equivalent export may be committed.
- Preserve the historical UAL1 recipe and cook unchanged; `Sword_Attack` is not a bow placeholder.
- Extend the provisional skeletal cook/stable-ID staging only as narrowly as the approved exact selection requires.
- Cook only the minimum approved idle, locomotion, notch, release, and bounded aim inputs; do not retain an entire animation library.
- Generate ignored client output only; do not modify Starfall, integrate presentation, retarget, rewrite sources, or create a general animation pipeline.

## Notes

- 2026-08-07 05:58 UTC - Implemented the bounded Basic Arrow animation acquisition without changing the historical UAL1 or six-clip experiment recipes.

  - Added `quaternius-ual2-source-bow-shot-body` for the exact non-root-motion pack-relative source `Unreal-Godot/UAL2.glb` (source SHA-256 `866c2ee822d30f0ceed521f50a5e84316d58ee4487d0b02158370bb988452416`).
  - Retained only `Bow_Notch`, `Bow_Aim_Neutral`, and `Bow_Shoot` on `Mannequin` / `Armature`.
  - Extended stable-project-ID staging with optional `--ual2-source-root`; the value remains runtime-only and external to the family worktree. Public-only staging removes only known generated optional UAL2 output and preserves established public cooks.
  - Private-mode output is 1,308,691 bytes, SHA-256 `5460a602d0ee3a8f4530c47f08ee5d88adda2b4224b20f2328b1d6f90d7b1966`; cook and provenance were byte-identical across two runs. Provenance contains only portable identities and normalized CC0 evidence.
  - Verified private mode, public-only cleanup, and final private restaging in Starfall's ignored client output. No child source, PM data, tracked files, or gitlinks changed.
  - Validation: shell syntax; focused CharacterCooker tests (13) and CharacterPresentation tests (91); full ChronoFall Debug and Release builds; 285 tests passed in each configuration; zero build warnings/errors; `git diff --check`.
  - Existing EXPERIMENT-0014 native visual proof remains the approved animation evidence; this acquisition introduced no new visual decision or checkpoint.