---
id: SHARED-0019
title: Add deterministic shared static-asset cooking
track: SHARED
milestone: M2
dependsOn:
- SHARED-0018
- SHARED-0017
createdAt: 2026-08-02T16:19:37.7738830Z
modifiedAt: 2026-08-02T17:57:33.0471850Z
---

Add a provisional coordinator-owned client-only cook for exact selected static meshes consumed by the narrow shared static renderer.

Acceptance boundary:
- Consume only exact task-selected source paths with hashes, CC0 evidence, stable identifiers, scale/material evidence, and explicit client audience.
- Produce deterministic bounded output that the shared static renderer can read; reject malformed, unsupported, escaping, unprovenanced, or whole-pack inputs.
- Preserve generated-output isolation and extend stable-project-ID staging only for an explicitly approved consuming selection.
- Keep server, simulation, Balance Lab, protocol, and content projects free of presentation payloads and native dependencies.
- Record format and conversion decisions as provisional evidence rather than a permanent generic asset format.
- Do not cook entire packs, add terrain/vegetation systems, silently repair sources, introduce a large importer, or implement child presentation.

## Notes

- 2026-08-02 17:57 UTC - Implemented the approved deterministic static-asset cooking boundary.

  Implementation:
  - Added provisional bounded `.cfmesh` v1 read/write in the BCL-only cooking assembly with exact source, external-resource, licence, scale, stable-ID and material-policy evidence.
  - Added the build-time-only SimpleMesh OBJ/glTF/GLB adapter and `ChronoFall.StaticMeshCooker`; exact resources are hashed, undeclared/unused/escaping/symlinked resources fail, all declared inputs are protected from output overwrite, hierarchy transforms and uniform metre conversion are baked deterministically, and unsupported/warning/skin/animation/generated-normal/reflection inputs fail explicitly.
  - Added coordinator-authored CC0 two-box OBJ/MTL fixture matching the completed SHARED-0018 direct proof. No Quaternius/Kenney asset was selected, no child staging allowlist or runtime manifest was added, and no generated output was committed.
  - Added reproducible SimpleMesh invariant-culture patch `0002-use-invariant-culture-for-obj-floats.patch` after tests exposed decimal-comma OBJ parsing; the pinned checkout remains reproducible.
  - Added cooked-asset harness input and direct-versus-cooked native capture comparison. Updated shared presentation, family source-consumption, and new shared static-cooking wiki documentation.

  Deterministic evidence:
  - cooked fixture: 1,967 bytes, SHA-256 `c04d5071091d36a1b18edc187854e29395f107e0529cbaa8c63e1c8c592b78c2`
  - provenance SHA-256 `20dc5e30892bdc8dc4edbce13bc93c4f289c8e85d43c1ad205078111e0ee3312`
  - native fingerprints `247198b9ff0e2862 / 7d2c37c52e46fb19 / 247198b9ff0e2862`
  - direct and cooked PPMs byte-identical; SHA-256 `5c45a75532678dc94a69334d6d693b08d0f4544c247a92177d893acc690f0b43`

  Validation:
  - `dotnet restore ChronoFall.slnx`
  - Debug and Release builds: 0 warnings, 0 errors
  - Debug and Release managed tests: 184/184 each
  - opt-in native SDL GPU integration tests: 2/2 on native macOS ARM64/Metal
  - `sh thirdparty/verify-simplemesh.sh`
  - Starfall restore/build: 0 warnings, 0 errors; architecture tests 23/23
  - inspected Starfall World, Simulation and BalanceLab outputs: no ChronoFall presentation, SDL, shader or client asset payloads
  - coordinator, Royale and Starfall `pm doctor` passed; family has 3 readable/trusted members and 0 warnings
  - `git diff --check` passed; Royale and Starfall worktrees/gitlinks remain unchanged.

  The static cook remains provisional and client-only. ASSET-0006/ASSET-0007 still own any later exact selected Starfall input and stable-ID staging extension.