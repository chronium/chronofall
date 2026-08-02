---
title: Shared Skeletal Asset Cooking
createdAt: 2026-08-02T07:06:03.6421350Z
modifiedAt: 2026-08-02T07:10:40.2808100Z
---

## Status and ownership

`pm://project/prj_E7QP3LUocfY7k3PYM-EQOlqc/task/SHARED-0002` establishes one coordinator-owned provisional client cook for the proven character-presentation data. It is evidence for the current source envelope, not a permanent file-format, package-distribution, or child-acquisition promise.

`ChronoFall.CharacterPresentation.Cooking` is BCL-only and depends only on `ChronoFall.CharacterPresentation`. The build-time `ChronoFall.CharacterCooker` is the only new consumer of the provisional SimpleMesh adapter. Neither shared assembly depends on SimpleMesh, SDL, Royale, Starfall, server, editor, or simulation code.

## Source recipe and provenance

The committed recipe is:

`assets/recipes/quaternius-ual1-standard.json`

It selects the authoritative supplied source:

`assets/Quaternius/Universal Animation Library[Standard]/Unreal-Godot/UAL1_Standard.glb`

- source SHA-256: `69591853d817488edaa8fd9bf8fc1d821eaeaf789f8627b3cd23b41c4ed67997`;
- source format: embedded glTF 2.0 GLB;
- mesh node and mesh: `Mannequin`;
- skin: `Armature`, 65 parent-first joints;
- selected clips, in deterministic order: `Idle_Loop`, `Walk_Loop`, `Sword_Attack`;
- licence: Quaternius CC0 1.0 Universal / public-domain dedication;
- committed evidence: the pack's `License.txt` and `README.txt`.

The cooker accepts only portable repository-relative source and licence paths, verifies every evidence file exists, verifies the source hash before and after cooking, and requires exact ordinal mesh-node, mesh, skin, and clip identifiers. The source is never copied, rewritten, converted in place, or repaired.

## Reproduction

From the coordinator root:

```sh
dotnet run --project tools/ChronoFall.CharacterCooker/ChronoFall.CharacterCooker.csproj -c Release -- \
  --source-root . \
  --recipe assets/recipes/quaternius-ual1-standard.json \
  --output artifacts/character-cooking/quaternius-ual1-standard.cfskel \
  --audience client
```

The generated output remains ignored under `artifacts/`. It is not committed or added to a runtime manifest.

The initial Release cook is 1,278,301 bytes with SHA-256:

`37d2ecd2c614a4cc74fe359906c84408432100f0338b86d7ce4f4dddb6b585d3`

Two independently generated files compared byte-for-byte identical.

## Provisional binary contract

The `.cfskel` version 1 container uses fixed magic, an unsigned version, explicit little-endian integers, IEEE-754 single-precision values, bounded collection counts, and length-prefixed strict UTF-8 strings.

Its deterministic order is:

1. source/provenance descriptor;
2. parent-first skeleton and local bind transforms;
3. inverse-bind matrices in joint order;
4. skinned mesh name, vertices, indices, and ordered sections;
5. the recipe-selected animation clips, complete joint tracks, and LINEAR translation, rotation, and scale keyframes.

The reader reconstructs the promoted immutable presentation types and rejects wrong magic/version, malformed or unbounded strings/counts, unsupported interpolation, truncated or trailing data, invalid hierarchy or influences, duplicate names, and non-finite or otherwise invalid contract values.

The first format intentionally performs no compression, quantization, coordinate conversion, retargeting, root-motion extraction, texture/material cooking, UV1 handling, or metadata cooking for sockets, equipment, grips, reference points, IK, masks, layers, or animation graphs. It preserves the selected source values exactly, including normalized quaternion bits through an internal validated reconstruction path.

## Audience and authority

This is client presentation content. The CLI requires `--audience client` and explicitly rejects server audience. No headless artifact, server manifest, gameplay simulation, protocol, or authoritative state consumes this mesh, skin, animation, or cooking code.

Animation remains presentation-only. Cooking does not decide attacks, hits, movement, equipment, damage, or any other gameplay outcome.

## Validation evidence

The selected cook round-trips every retained descriptor, mesh, skeleton, skin, section, influence, matrix, clip, channel, keyframe, and interpolation value exactly. Recipe tests cover hash enforcement, portable paths, licence evidence, duplicate clips, embedded identifier mismatch, client-only audience, deterministic output, and protected-source/output boundaries. Format tests cover exact round trips, deterministic writes, corruption, bounds, versioning, truncation, trailing bytes, and dependency separation.

Debug and Release builds completed with zero warnings/errors. Each configuration passed 142 managed tests. Native macOS ARM64 Metal validation rendered the source and cooked paths through the same harness and required every existing bind, animation, blend, layer, skeleton, Aim, and IK capture to compare byte-for-byte. Established fingerprints remained unchanged. The owner exercised all three cooked clips, the skeleton overlay, Aim, and IK and confirmed everything still looks the same.

No project-history artifact is retained because this task proves transport fidelity rather than a new visual capability.

## Deferred decisions

Future task-owned evidence must decide:

- whether `.cfskel` is retained, revised, versioned, or replaced;
- package publication and how independent child repositories acquire shared binaries and cooked content;
- child content manifests and build integration;
- modular armour and shared-rig mesh composition;
- material, texture, bounds, socket, equipment, grip, reference-point, mask, layer, and IK metadata cooking;
- compression, quantization, streaming, patching, and content-addressed distribution.

Royale and Starfall remain unchanged. They must not use parent-relative project references, and neither child receives this generated artifact until its own approved integration and distribution task.