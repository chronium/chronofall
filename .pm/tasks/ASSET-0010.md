---
id: ASSET-0010
title: Inventory private UAL2 Source bow-body inputs
track: ASSET
milestone: M3
priority: medium
dependsOn:
- ASSET-0001
createdAt: 2026-08-03T12:28:28.4897400Z
modifiedAt: 2026-08-03T12:32:53.6726390Z
---

Inventory the owner-supplied private Quaternius Universal Animation Library 2 Source snapshot and select the bounded technical bow-body-animation proof inputs.

Acceptance criteria:

- Treat the complete purchased package as read-only private source material and persist no absolute owner path.
- Record the exact pack-relative file inventory, sizes, formats, hashes, supplied CC0 1.0 evidence, official source, source-snapshot identity, and redistribution boundary.
- Confirm the non-root-motion and `_RM` organization, 134-animation count, and that every animation from the historical 43-clip UAL2 Standard snapshot is present.
- Record exact bow-related clip names, durations, 30 Hz samples, interpolation, root behavior, intended loop/one-shot semantics, and missing bow-ready/recovery/locomotion coverage.
- Compare the private UAL2 technical mannequin against the proven UAL1 mannequin contract: hierarchy, ordered joints, rest transforms, inverse binds, mesh/skin evidence, coordinates, scale, and known Universal Base Character/Ranger compatibility limits.
- Carry both proven UAL1 `Idle_Loop` and UAL2 `Idle_No_Loop` forward as visual-proof candidates; do not freeze the proof recipe.
- Select only the bounded UAL2 proof candidates for later evaluation and record that Basic/Fire Arrow may share one body sequence while Arrow Rain remains evidence-gated.
- Commit only normalized text snapshots of the supplied `License.txt` and `README.txt`; do not copy GLB, FBX, Blend, addon, images, source-equivalent exports, or generated/cooked output.
- Update coordinator provenance, character-animation inventory, and Draft 0 enabler documentation.
- Allocate but do not activate the focused technical bow-body-animation sequence proof task and wire its approved local dependency into `ASSET-0004`.
- Validate PM state, receipts, linked-family warnings, repository diff, private-path absence, staged file list, and `git diff --check`; complete and commit only this inventory task.

## Notes

- 2026-08-03 12:32 UTC - Completed the approved read-only UAL2 Source inventory and roadmap handoff.

  - Verified an owner-private 18-file, approximately 246 MiB Source snapshot with no symlinks; no package file was modified.
  - Anchored the snapshot by the non-RM GLB SHA-256 `866c2ee822d30f0ceed521f50a5e84316d58ee4487d0b02158370bb988452416`, RM GLB SHA-256 `fecf2e0bf90808b51d12957d179c0cc6587c58d270a7b5b5c29cc2bae0e34332`, and supplied licence/readme hashes.
  - Confirmed 134 clips in both library GLBs and all 43 historical UAL2 Standard names present, for 91 additional Source clips.
  - Recorded exact six bow clips, timings, 30 Hz LINEAR sampling, in-place/RM evidence, missing bow-ready/recovery/bow-locomotion coverage, and bounded proof candidates.
  - Confirmed the UAL2 Source technical mannequin matches the established UAL1 mannequin's ordered 65-joint hierarchy, local rest transforms, inverse binds and mesh accessor payloads; Universal Base Character/Ranger compatibility remains unresolved and no retargeting is authorized.
  - Preserved both UAL1 `Idle_Loop` and UAL2 `Idle_No_Loop` as visual comparison candidates rather than freezing a recipe.
  - Created `assets/quaternius-ual2-source-bow-evaluation` and updated the character inventory, Quaternius provenance and Starfall shared-enabler roadmap.
  - Committed-repository payload is limited to normalized text snapshots of `License.txt` and `README.txt` under `assets/provenance/Quaternius/Universal Animation Library 2 Source/`; their normalization and both source/committed hashes are documented.
  - Added the private-Quaternius-Source routing rule to the repository asset-provenance skill.
  - Allocated todo `EXPERIMENT-0014` with local dependency `ASSET-0010`; added it to todo `ASSET-0004`, which remains blocked on both Starfall `CONTENT-0011` and the experiment. No child task was mutated or activated.
  - All mutation receipts target only coordinator project `prj_E7QP3LUocfY7k3PYM-EQOlqc` and the expected task/state/wiki paths.
  - Validation before completion: PM MCP validation and `pm doctor` passed; linked-family inspection returned zero warnings; `git diff --check` passed; private absolute-path search returned no repository match; Royale and Starfall remained clean with unchanged gitlinks.