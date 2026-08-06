---
name: chronofall-asset-pipeline-provenance
description: Inspect and manage ChronoFall supplied assets and cooking boundaries. Use for Quaternius or Kenney CC0 provenance, external-library curation, character/animation format inventory, exact experiment input selection, embedded or external glTF resources, skeleton compatibility, conversion evidence, client/server asset separation, collision cooking, or introducing asset-pipeline dependencies.
---

# Asset Pipeline And Provenance

## Treat Supplied Files As Authoritative

Inspect `assets/Quaternius/` directly. Record pack name, source path, license/readme text, formats, file counts, and whether the pack is a Standard/free subset. Preserve CC0 1.0 provenance and Quaternius attribution information; do not download replacements.

Purchased Quaternius Source packages supplied outside the coordinator remain private, read-only source material. Never persist an owner's absolute source path or copy a complete GLB/FBX/Blend package, addon, setup media, or lossless source-equivalent export into a public family repository. A focused task may commit normalized licence/readme evidence under `assets/provenance/Quaternius/`, exact pack-relative identities and hashes, a bounded recipe, and ignored cooked runtime output. Inventory and availability do not make the full package a selected runtime input.

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

## Run Native Blender Asset Reviews Safely

Use Blender as a focused, owner-visible dimensional and composition review tool when an asset task needs human confirmation of scale, orientation, pivot implications, silhouette, or relative proportions.

1. Probe the connected Blender MCP with a read-only scene or file summary before attempting native review work. If Blender is closed, the MCP add-on is disconnected, or the probe fails, tell the owner that Blender must be opened with the MCP add-on enabled and stop the native-review path until it is available. Do not install, launch, configure, or replace Blender autonomously.
2. Inspect the open file, scene names, active scene, mode, unit settings, dirty state, and relevant objects before changing anything. Preserve user work. Never reset Blender, reload a file, switch to factory settings, or discard an existing scene merely to obtain a clean review surface.
3. Create a clearly named temporary scene or collection such as `<TASK-ID> Scale Review (UNSAVED)`. Append or duplicate only the exact task-selected source objects. Keep the source scene and source datablocks intact; do not edit the supplied mesh in place.
4. Use metric scene units and apply the exact approved conversion uniformly. Retain source orientation and pivot unless the task explicitly owns a conversion. Where source variants use different authoring axes, state that difference instead of silently rotating the durable cook contract.
5. Add only simple diagnostic context needed for the decision: a ground plane, dimensional ruler, a known-height reference, concise labels, and a neutral view. Freeze measured dimensions from transformed bounds and report them in metres. Do not embellish the scene into an art, socket, animation, gameplay, or renderer proof.
6. Frame the exact question for the owner, for example whether a 1.36 metre bow and 0.68 metre arrow are credible beside a 1.8 metre reference. State explicitly what the review does not prove.
7. Never save over a supplied `.blend`, export replacement source files, run lossy conversions, or persist the owner's machine paths. Treat the comparison scene as unsaved validation unless a separately approved task owns a reusable diagnostic fixture.
8. After confirmation, record the selected source identities, scale, measured bounds, review setup, owner conclusion, and exclusions in the owning task and wiki. A screenshot or project-history artifact is a separate owner choice; do not preserve it automatically.

If cleanup is needed, remove only objects, collections, or scenes created by the review after confirming that doing so cannot discard owner work. When uncertain, leave the temporary scene open and remind the owner not to save the supplied source file.

## Preserve Audience Boundaries

Client output may contain render meshes, materials, textures, skeletons, and animation. Server output receives only content required by authoritative rules/collision. Keep render dependencies and source-authoring files out of headless artifacts.

Stage shared character content into a child only through the coordinator command using that child's stable project ID. Preserve the deterministic provenance sidecar and source licence evidence. Refuse destinations that do not resolve through the committed family declaration and reciprocal identity, are not the exact ignored generated tree, contain tracked or unexpected files, or cross a symlink. Do not copy the raw source GLB or add generated output to a runtime manifest as part of staging.

Treat loader choice, conversion tooling, permanent formats, and new dependencies as Plan-mode owner decisions. Document reproducibility and provenance in the coordinator wiki.
