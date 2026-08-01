---
id: COORD-0006
title: Add committed Starfall submodule path hint
track: COORD
milestone: M0
createdAt: 2026-08-01T06:10:51.4498170Z
modifiedAt: 2026-08-01T06:16:50.4763240Z
---

Add the Starfall path hint through PM's supported linked-project configuration workflow, verify that it resolves to the intended submodule checkout without relying on local registry identity, and rerun coordinator validation.

## Notes

- 2026-08-01 06:16 UTC - Added `pathHint: starfall` using `pm project update-child` for stable child project `prj_pkIpzx0fzFD4URjvqBuYrGZF`. The hint resolves to the checked-out `starfall` submodule and its `.pm/project_id.txt` matches the declaration. Coordinator `pm doctor` passes. Local family reads remain available with zero structured warnings; this machine currently resolves Starfall from the registry first and reports it read-only for linked MCP writes, so child corrections were performed through supported PM CLI from the owning Starfall context.