---
id: SHARED-0022
title: Promote bounded SDL GPU screenshot capture
track: SHARED
milestone: M3
dependsOn:
- SHARED-0016
- EXPERIMENT-0009
- pm://project/prj__-jXLQgm6GuD2gCKZ_bTa1m-/task/EDITOR-002
createdAt: 2026-08-03T15:16:09.8324270Z
modifiedAt: 2026-08-03T15:16:16.0760800Z
---

Promote the already-proven SDL GPU screenshot boundary into a focused coordinator-owned module for family clients.

Acceptance criteria:
- Add a narrow parent-owned SDL GPU capture project that depends on the coordinator-pinned SDL3-CS source and never on Royale or Starfall.
- Preserve caller ownership of windows, GPU devices, command buffers, render passes, render scheduling and gameplay presentation.
- Support deterministic one-shot readback of an existing RGBA8 or BGRA8 GPU texture through correctly owned download transfer buffers and fences.
- Normalize supported RGBA/BGRA formats into a tightly packed RGBA image and fail explicitly for unsupported formats, invalid dimensions and malformed pixel buffers.
- Encode exact RGBA images as PNG through an explicitly pinned, client/tooling-only dependency; do not add rendering, scene, editor, asynchronous thumbnail or general image-framework scope.
- Derive the contract from completed ChronoFall experiment capture evidence and Royale's proven screenshot implementation without adding a coordinator-to-child source dependency.
- Add deterministic managed tests for format normalization, validation and PNG output plus a native macOS ARM64 SDL GPU readback harness.
- Correct the repository contact-sheet compositor's pixel/backing-scale handling so equal-sized 16:9 captures tile without hidden Retina padding; keep raw captures outside source control.
- Document the shared ownership boundary, current Starfall consumer, and deferred Royale adoption.