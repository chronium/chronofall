---
id: ASSET-0003
title: Curate directly useful Kenney assets for the ChronoFall family
track: ASSET
priority: high
dependsOn:
- ASSET-0002
createdAt: 2026-08-01T08:23:21.7321340Z
modifiedAt: 2026-08-01T08:35:08.8307460Z
---

Review the owner-supplied external Kenney All-in-One v3.6.0 snapshot as a read-only source library and produce a practical two-level catalogue for ChronoFall-family asset discovery.

Acceptance criteria:
- Inventory every included pack at pack level; do not catalogue every file individually.
- Create an asset-level shortlist only for directly useful candidates.
- Classify candidates by consumer: shared/editor tooling, Royale, Starfall, potentially useful later, or archive only.
- Apply usefulness labels: Immediate, Near-term, Situational, or Archive.
- Prioritize prototype textures and geometry; UI, icons, and input prompts; editor/debug visuals; Medieval Weapons Early Access; generic environmental props; gameplay markers and effects; and suitable audio.
- Record exact bundle-relative source paths, formats, approximate scale, preview references, relevant limitations, and CC0 provenance.
- Record bundle-changelog renames so later searches use current names while preserving actual snapshot paths.
- Note stylistic compatibility with Quaternius.
- Preserve Quaternius as selected for characters, the canonical skeleton, animations, armour/clothing, and Medieval Village MegaKit[Standard].
- Allow both Kenney and Quaternius weapons as future task-owned candidates without selecting or integrating one here.
- Keep the complete purchased compilation external; do not persist the owner's absolute bundle path.
- Do not copy, integrate, convert, cook, rename, delete, or modify assets.
- Do not add candidates to builds, runtime manifests, renderers, editors, or cooking pipelines.
- Recommend a small initial import set with exact source paths for later focused tasks.
- Produce concise searchable coordinator wiki documentation and record validation evidence in task notes.

## Notes

- 2026-08-01 08:35 UTC - Completed read-only curation of the owner-supplied external Kenney All-in-One v3.6.0 snapshot.

  Inventory evidence:
  - 264 pack directories.
  - 88,346 files excluding machine metadata; 975.07 MiB.
  - Three .DS_Store files ignored.
  - 58 ZIP archives retained in the external source; none was proven fully duplicated by expanded files.
  - No symlinks, case-insensitive collisions, Windows-reserved names, trailing-dot/space paths, control characters, or files at common Git hosting size limits.
  - Five non-ASCII path components cover 701 files and are documented for later imports.
  - Early access/Medieval Weapons is supplied as CC0 v0.1: 94 non-metadata files, 1.69 MiB, with 17 models each in GLB, FBX, and OBJ/MTL plus previews.

  Durable outputs:
  - assets/kenney-all-in-one-curation: provenance, source-selection policy, high-value packs, changelog aliases, portability findings, import contract, and exact first-import recommendation.
  - assets/kenney-pack-catalog: every supplied pack at pack level.
  - assets/kenney-candidate-index: 112 directly useful assets with exact relative paths, formats, approximate scale/duration, supplied preview paths, limitations, and Quaternius compatibility.
  - Candidate split: shared/editor tooling 53, Royale 8, Starfall 48, potentially useful later 3.
  - Usefulness split: Immediate 28, Near-term 54, Situational 30. Archive-only material remains in the pack catalogue rather than inflating the direct shortlist.
  - Coordinator asset-provenance skill updated to keep purchased compilations external/read-only and require focused provenance-preserving imports.

  Selection policy preserved:
  - Quaternius remains selected for humanoid characters, canonical/reference skeleton, animation libraries, modular armour/clothing, and Medieval Village MegaKit[Standard].
  - Kenney and Quaternius medieval weapons remain later task-owned candidates; no weapon was selected.
  - No assets were copied, modified, converted, cooked, imported, or added to builds/manifests. No child repository or gitlink was changed.

  Validation:
  - PM MCP project validation passed with zero issues.
  - pm doctor passed.
  - Linked family inspection returned all three projects available/readable/trusted with zero warnings.
  - git diff --check passed.
  - No absolute external bundle path is present in committed policy/wiki/task content.