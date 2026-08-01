---
title: Quaternius Asset Provenance and Intake
createdAt: 2026-08-01T05:44:07.0108550Z
modifiedAt: 2026-08-01T07:17:24.3227160Z
---

## Provenance

The supplied files under `assets/Quaternius/` are the authoritative intake. Every present pack declares CC0 1.0 Universal / public-domain dedication and identifies the models as by Quaternius. Preserve the included license and readme files. Do not download substitutes.

Kickoff inventory found these Standard/free collections:

- Universal Base Characters: 127 MB, 114 files; glTF/external BIN and FBX characters plus textures.
- Modular Character Outfits - Fantasy: 292 MB, 121 files; glTF/external BIN and FBX modular parts/outfits plus textures.
- Universal Animation Library: 61 MB, 10 files; FBX and GLB, with in-place and `_RM` root-motion variants.
- Universal Animation Library 2: 69 MB, 13 files; FBX and GLB variants plus a female mannequin.
- Medieval Weapons: 24 MB, 98 files; Blender, FBX, OBJ/MTL.
- Medieval Village MegaKit: 168 MB, 936 files; glTF/external BIN, FBX, OBJ/MTL, and textures.

The inspected male and female base-character glTFs each contain one skin with 65 joints, 69 nodes, three meshes, no animations, external BIN data, and external textures. The animation-library readmes distinguish `_RM` root-motion files from root-motion-disabled files. The outfit readme says the pack works with Universal Base Characters and recommends hiding/removing unseen base-body parts to avoid clipping.

## Intake rule

`ASSET-0001` must record exact filenames/formats, external resources, hierarchy/joint count/names, weights/influence count, inverse-bind matrices, channels/interpolation/duration/timing, coordinate system/scale, compatibility, and conversion evidence. Naming similarity is not compatibility evidence.

The first proof selects the smallest useful subset: one humanoid, one skeleton, one idle, one locomotion clip, and one compatible attack if demonstrated. Do not process the entire collection. If skeletons differ, report evidence and plan the smallest resolving experiment; do not invent retargeting.

Client cooks may contain rendering/skeletal/animation data. Headless/server output contains only authoritative or collision data and never rendering dependencies.

`ASSET-0001` completed the detailed, reproducible inventory at `pm://project/prj_E7QP3LUocfY7k3PYM-EQOlqc/wiki/assets/character-animation-inventory`. It records the exact files and hashes, 65-joint hierarchy, four-influence weights, inverse binds, 30 Hz LINEAR animation channels, root-motion variants, scale evidence, compatibility limits, and three unresolved supplied texture URIs. Exact joint identity is demonstrated, but deformation compatibility still requires the narrow follow-up probe; no retargeting or source-asset rewrite is authorized.