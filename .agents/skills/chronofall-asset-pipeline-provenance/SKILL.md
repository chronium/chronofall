---
name: chronofall-asset-pipeline-provenance
description: Inspect and manage ChronoFall supplied assets and cooking boundaries. Use for Quaternius CC0 provenance, character/animation format inventory, exact experiment input selection, embedded or external glTF resources, skeleton compatibility, conversion evidence, client/server asset separation, collision cooking, or introducing asset-pipeline dependencies.
---

# Asset Pipeline And Provenance

## Treat Supplied Files As Authoritative

Inspect `assets/Quaternius/` directly. Record pack name, source path, license/readme text, formats, file counts, and whether the pack is a Standard/free subset. Preserve CC0 1.0 provenance and Quaternius attribution information; do not download replacements.

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
