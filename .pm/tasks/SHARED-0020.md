---
id: SHARED-0020
title: Prove one rendered socketed static bow attachment
track: SHARED
milestone: M2
dependsOn:
- SHARED-0006
- SHARED-0018
- ASSET-0006
createdAt: 2026-08-02T16:19:38.0097790Z
modifiedAt: 2026-08-02T16:20:04.8498220Z
---

Prove the narrow shared contract for rendering one exact selected static bow from a posed character socket.

Acceptance boundary:
- Consume the completed BCL-only socket contract, the reusable static renderer, and the exact coordinator-acquired bow selection.
- Resolve bow placement from caller-selected Starfall socket/grip content while keeping gameplay equipment, attacks, aiming, and item identity child-owned.
- Render one bow through the caller-owned SDL GPU frame lifecycle and validate transforms deterministically plus native macOS ARM64 appearance.
- Keep this proof narrow enough to be reviewed and reused by later SHARED-0007 broad attachment work.
- Do not include shields, backpacks, wings, armour, weapon IK, projectiles, gameplay integration, generic attachment categories, or a scene framework.