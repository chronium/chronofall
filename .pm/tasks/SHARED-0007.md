---
id: SHARED-0007
title: Present weapons, shields, backpacks, and wings
track: SHARED
milestone: M2
dependsOn:
- SHARED-0004
- SHARED-0006
- SHARED-0020
createdAt: 2026-08-01T05:34:57.8206500Z
modifiedAt: 2026-08-02T16:20:21.2706120Z
---

Use the focused equipment-slot and socket contracts to render representative weapons, shields, backpacks, wings, and other proven static attachment categories while gameplay ownership and item schemas remain child-specific.

Before implementation, review and reuse the narrow socketed-bow proof in SHARED-0020, including its static-render host boundary, placement evidence, and validation path. Do not independently recreate the same capability or force broad attachment categories into the bow proof.

Keep this as the broader deferred attachment-presentation task. Its implementation must preserve existing downstream consumers and must not become a generic scene, item, equipment, or attachment framework.