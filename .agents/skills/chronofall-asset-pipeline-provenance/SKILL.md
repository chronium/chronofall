---
name: chronofall-asset-pipeline-provenance
description: Inspect and manage ChronoFall supplied assets and cooking boundaries. Use for Quaternius or Kenney CC0 provenance, external-library curation, character/animation format inventory, exact experiment input selection, embedded or external glTF resources, skeleton compatibility, conversion evidence, client/server asset separation, collision cooking, or introducing asset-pipeline dependencies.
---

# Asset Pipeline And Provenance

## Treat Supplied Files As Authoritative

Inspect `assets/Quaternius/` directly. Record pack name, source path, license/readme text, formats, file counts, and whether the pack is a Standard/free subset. Preserve CC0 1.0 provenance and Quaternius attribution information; do not download replacements.

Purchased compilation libraries such as Kenney All-in-One remain outside the repository and read-only. Never persist the owner's absolute source path. Catalogue every supplied pack at pack level, then index only unusually relevant individual assets by bundle-relative path, consumer, usefulness, format, scale, preview, limitation, and license evidence. Do not turn a curation task into a file-by-file inventory.

The complete Kenney All-in-One compilation must not be redistributed. A later focused coordinator task may copy only task-selected files or a separately redistributable pack into `assets/Kenney/`. That task must record the bundle version, pack name and Early Access status, original bundle-relative path, copied files, official source, license, changes, intended consumer, and confirmation that the compilation itself is not being redistributed. Preserve source license/readme files. Do not copy, convert, cook, rename, reorganize, or add candidates to manifests during curation.

For current work, Quaternius remains selected for humanoid characters, the canonical/reference skeleton, animation, modular armour/clothing, and `Medieval Village MegaKit[Standard]`. Kenney alternatives remain available for later owner-approved evaluation. Both `Medieval Weapons Pack by @Quaternius` and Kenney `Early access/Medieval Weapons` are candidate weapon sources; selection and integration require their own tasks.

## Inventory Before Conversion

For candidate character and animation files, record:

- source filenames/formats and embedded/external resources;
- skeleton hierarchy, joint count, names, and roots;
- weights, joint indices, influence count, and inverse-bind matrices;
- animation clips, channels, interpolation, duration, sample timing, and root motion;
- coordinate system, handedness, unit scale, and exporter metadata;
- character/animation skeleton compatibility and required conversion steps.

Use evidence from the file, not naming similarity. If compatibility fails, stop at the evidence and propose the smallest experiment; do not create a retargeter.

## Select Minimally

Choose only one humanoid, one skeleton, one idle, one locomotion clip, and a compatible attack if available. Persist exact repository paths and rationale. Do not process the whole collection or copy unrelated outfits, weapons, or village assets into an experiment.

## Preserve Audience Boundaries

Client output may contain render meshes, materials, textures, skeletons, and animation. Server output receives only content required by authoritative rules/collision. Keep render dependencies and source-authoring files out of headless artifacts.

Treat loader choice, conversion tooling, permanent formats, and new dependencies as Plan-mode owner decisions. Document reproducibility and provenance in the coordinator wiki.
