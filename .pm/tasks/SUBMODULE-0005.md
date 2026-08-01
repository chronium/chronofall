---
id: SUBMODULE-0005
title: Advance Starfall submodule after service architecture decision
track: SUBMODULE
milestone: M0
dependsOn:
- pm://project/prj_pkIpzx0fzFD4URjvqBuYrGZF/task/ARCH-0004
createdAt: 2026-08-01T07:04:44.3100360Z
modifiedAt: 2026-08-01T07:05:06.8789280Z
---

Advance the coordinator's Starfall gitlink to the reviewed and validated child commit that records the approved service availability and ownership boundaries under ARCH-0004. Keep this pointer-only integration separate from the child PM/wiki commit.

Acceptance criteria:
- The canonical Starfall dependency is complete and readable.
- The starfall checkout is clean at the intended pushed child commit.
- Only the Starfall gitlink and this coordinator task's PM evidence are committed.
- Coordinator and Starfall PM validation pass.

## Notes

- 2026-08-01 07:05 UTC - Canonical dependency `pm://project/prj_pkIpzx0fzFD4URjvqBuYrGZF/task/ARCH-0004` resolves as completed in Starfall. Advance the `starfall` gitlink from `83bba0c9bbff099a90faab21a7914708bbd334f0` to reviewed child commit `224ec171346f7633a5390388538ec41a4433a8ce` (`[ARCH-0004] Define service availability boundaries`). The child checkout is clean, the commit is contained by `origin/main`, stable project identity and `pathHint: starfall` match, and coordinator/Starfall `pm doctor` validation passes.