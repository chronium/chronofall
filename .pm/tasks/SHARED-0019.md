---
id: SHARED-0019
title: Add deterministic shared static-asset cooking
track: SHARED
milestone: M2
dependsOn:
- SHARED-0018
- SHARED-0017
createdAt: 2026-08-02T16:19:37.7738830Z
modifiedAt: 2026-08-02T16:30:51.8866330Z
---

Add a provisional coordinator-owned client-only cook for exact selected static meshes consumed by the narrow shared static renderer.

Acceptance boundary:
- Consume only exact task-selected source paths with hashes, CC0 evidence, stable identifiers, scale/material evidence, and explicit client audience.
- Produce deterministic bounded output that the shared static renderer can read; reject malformed, unsupported, escaping, unprovenanced, or whole-pack inputs.
- Preserve generated-output isolation and extend stable-project-ID staging only for an explicitly approved consuming selection.
- Keep server, simulation, Balance Lab, protocol, and content projects free of presentation payloads and native dependencies.
- Record format and conversion decisions as provisional evidence rather than a permanent generic asset format.
- Do not cook entire packs, add terrain/vegetation systems, silently repair sources, introduce a large importer, or implement child presentation.