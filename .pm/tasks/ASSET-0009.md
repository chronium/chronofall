---
id: ASSET-0009
title: Remove the public UAL2 Standard source snapshot
track: ASSET
milestone: M3
createdAt: 2026-08-03T12:22:15.1421420Z
modifiedAt: 2026-08-03T12:24:07.4102900Z
---

Remove the already-absent tracked `assets/Quaternius/Universal Animation Library 2[Standard]/` snapshot as a focused coordinator cleanup before private UAL2 Source work begins.

Acceptance criteria:

- Delete only the 13 tracked files from the UAL2 Standard snapshot; do not touch UAL1 or any other supplied pack.
- Update coordinator asset provenance, character-animation inventory, and source-routing documentation so they distinguish historical inspection evidence from files currently retained in the repository.
- Preserve the historical hashes, structural findings, and 43-clip evidence where useful, while marking their source paths as no longer present/reproducible from the checkout.
- Keep the owner-supplied UAL2 Source package private and out of scope: do not inventory it durably, copy files, add recipes, cook assets, or persist its absolute path.
- Do not modify source code, children, gitlinks, generated artifacts, or runtime manifests.
- Validate coordinator PM state, linked-family warnings, repository diffs, staged deletions, and `git diff --check`; complete and commit only this task.

## Notes

- 2026-08-03 12:24 UTC - Cleanup completed as approved.

  - Removed the 13 tracked files formerly under `assets/Quaternius/Universal Animation Library 2[Standard]/`; the directory now contains zero files.
  - Preserved the historical UAL2 Standard hashes, 43-clip measurements, skeleton findings, and CC0/Quaternius attribution context while marking the deleted paths as unavailable in the current checkout.
  - Updated `assets/character-animation-inventory`, `assets/quaternius-provenance`, `assets/kenney-all-in-one-curation`, and `roadmap/starfall-draft-0-shared-enablers`.
  - Mutation receipts targeted only coordinator project `prj_E7QP3LUocfY7k3PYM-EQOlqc`: task/state paths plus the four expected wiki paths.
  - Kept the owner-supplied private UAL2 Source package outside the repository; no absolute private source path, source-equivalent file, recipe, cook, generated artifact, source code, child change, or gitlink change was introduced.
  - Validation: PM MCP validation passed with zero issues; `pm doctor` passed; linked-family inspection returned three available/readable/trusted members with zero warnings; `git diff --check` passed; Royale and Starfall worktrees remained clean and their pins unchanged.