---
id: SHARED-0002
title: Add shared skeletal asset cooking
track: SHARED
milestone: M2
dependsOn:
- SHARED-0001
createdAt: 2026-08-01T05:34:56.5484040Z
modifiedAt: 2026-08-02T07:07:03.9947870Z
---

Cook the proven skeletal inputs for client use while preserving explicit source provenance, deterministic output, and client/server audience separation. Prove one provisional versioned binary representation without making it a permanent format or distribution contract.

## Acceptance criteria

- Add a coordinator-owned BCL-only cooking assembly that depends only on `ChronoFall.CharacterPresentation`; the promoted core remains independent of cooking, SimpleMesh, SDL, children, server, and simulation code.
- Add a build-time cooker that uses the existing provisional SimpleMesh adapter without promoting SimpleMesh or adding another dependency.
- Commit one strict recipe for `UAL1_Standard.glb` containing only repository-relative source and CC0 license evidence, the approved source SHA-256, `Mannequin`, `Armature`, and exactly `Idle_Loop`, `Walk_Loop`, and `Sword_Attack`.
- Require explicit client audience and reject absolute or escaping paths, wrong source hashes, missing provenance, mismatched embedded identifiers, and missing or duplicate selected clips.
- Write one deterministic little-endian `.cfskel` version 1 container with explicit magic/version, UTF-8 strings, finite single-precision contract values, bounded counts, source provenance, mesh, skeleton, skin, sections, and complete LINEAR TRS for the selected clips.
- Read the container back into the promoted immutable types and reject bad magic/version, malformed strings or counts, unsupported values, truncated/trailing data, and semantically invalid skeletal content.
- Preserve source values losslessly and introduce no compression, quantization, coordinate conversion, retargeting, root motion, material/texture payload, UV1, sockets, equipment, grip/reference/IK metadata, or animation graph.
- Generate cooked output only under ignored `artifacts/`; do not commit cooked binaries. Record output size and SHA-256 and require two independent cooks to be byte-identical.
- Add a cooked-asset path to the existing native GPU harness while preserving its default GLB path and 43-clip source browser.
- Require exact source-versus-cooked semantic equality and unchanged deterministic Metal fingerprints/captures for the selected presentation path, followed by explicit owner visual validation.
- Document the provisional format, recipe, provenance, reproduction command, client-only audience, generated-output policy, and deferred distribution/evolution decisions in the coordinator wiki.
- No Royale or Starfall source/PM change, package publication, runtime manifest integration, child distribution, gitlink update, server asset, or project-history artifact is introduced.

## Notes

- 2026-08-02 07:07 UTC - Implemented the approved coordinator-owned skeletal cooking proof. Added the BCL-only `ChronoFall.CharacterPresentation.Cooking` descriptor/reader/writer over the promoted types, a strict build-time `ChronoFall.CharacterCooker` that alone consumes the provisional SimpleMesh adapter, and `assets/recipes/quaternius-ual1-standard.json` selecting `Mannequin`, `Armature`, `Idle_Loop`, `Walk_Loop`, and `Sword_Attack` with the committed CC0 evidence and source SHA-256. The client-only CLI rejects server audience, unsafe paths, hash/provenance/identifier mismatches, duplicate or missing clips, and protected output paths.

  The deterministic little-endian `.cfskel` version 1 container retains source provenance plus the complete selected mesh, 65-joint skeleton/skin, sections, influences, matrices, and LINEAR TRS samples. Exact quaternion bits are reconstructed through an internal validated core path without changing public normalization. No compression, quantization, coordinate conversion, retargeting, material/texture payload, root motion, socket/equipment/grip/reference/IK metadata, runtime manifest, package publication, child integration, or permanent format promise was added.

  Two independent Release cooks were byte-identical at 1,278,301 bytes with SHA-256 `37d2ecd2c614a4cc74fe359906c84408432100f0338b86d7ce4f4dddb6b585d3`; generated files remain ignored under `artifacts/` or temporary storage. Focused formatting passed for every affected project. Debug and Release builds completed with zero warnings/errors and each passed 142 managed tests (80 core, 9 cooking, 9 cooker, 29 experiment SDL GPU, 10 SimpleMesh adapter, 6 SDL GPU presentation). The opt-in native macOS ARM64 Metal integration passed and compared every established source/cooked bind, animation, blend, layer, skeleton, Aim, and IK capture byte-for-byte with unchanged fingerprints. The owner exercised all three cooked clips, the skeleton overlay, Aim, and IK and confirmed everything still looks the same. No visual-history artifact was retained because this is transport-fidelity evidence rather than a new visual capability.

  Published `pm://project/prj_E7QP3LUocfY7k3PYM-EQOlqc/wiki/assets/shared-skeletal-cooking` and updated the shared-presentation architecture. Royale and Starfall source, PM data, worktrees, and gitlinks remain unchanged.