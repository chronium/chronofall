---
id: COORD-0001
title: Validate linked-project topology and identities
track: COORD
milestone: M0
createdAt: 2026-08-01T05:34:29.9356600Z
modifiedAt: 2026-08-01T05:48:46.9936210Z
---

Verify the shallow ChronoFall -> royale + starfall family using PM stable IDs, reciprocal declarations, committed project IDs, submodule path hints, pinned commits, read availability, local write trust, and pm doctor. Record warnings without inferring identity from filesystem proximity or Git remotes.

Acceptance criteria:
- Registry, declarations, and `.pm/project_id.txt` values agree.
- The family is shallow and reciprocal.
- Coordinator and both children pass PM validation or have explicit warnings.

## Notes

- 2026-08-01 05:48 UTC - Kickoff verified the shallow family and reciprocal declarations. Stable IDs: ChronoFall prj_E7QP3LUocfY7k3PYM-EQOlqc, Royale prj__-jXLQgm6GuD2gCKZ_bTa1m-, Starfall prj_pkIpzx0fzFD4URjvqBuYrGZF. IDs match each `.pm/project_id.txt`; path hints resolve to the intended submodules; list_linked_projects returned zero warnings with all members readable and write-trusted. `pm doctor` passed in all three repositories.