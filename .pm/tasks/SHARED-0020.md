---
id: SHARED-0020
title: Prove one rendered socketed static bow attachment
track: SHARED
milestone: M5
dependsOn:
- SHARED-0006
- SHARED-0018
- ASSET-0006
createdAt: 2026-08-02T16:19:38.0097790Z
modifiedAt: 2026-08-06T07:23:05.0010150Z
---

Prove the narrow shared contract for rendering one exact selected static bow from a posed character socket.

Acceptance boundary:
- Consume the completed BCL-only socket contract, the reusable static renderer, and the exact coordinator-acquired bow selection.
- Use a harness-local technical socket and local bow transform solely to prove the reusable shared rendering contract.
- Render one bow through the caller-owned SDL GPU frame lifecycle and validate transforms deterministically plus native macOS ARM64 appearance.
- Keep this proof narrow enough to be reviewed and reused by later SHARED-0007 broad attachment work.
- Preserve Starfall ownership of its provisional semantic hand socket, local bow transform, rendering integration, and native placement validation in CLIENT-0011.
- Do not include shields, backpacks, wings, armour, weapon IK, aiming, projectiles, gameplay integration, equipment semantics, generalized grip systems, generic attachment categories, or a scene framework.