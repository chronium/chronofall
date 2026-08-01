---
id: COORD-0003
title: Document submodule and recursive checkout lifecycle
track: COORD
milestone: M0
createdAt: 2026-08-01T05:34:30.4134630Z
modifiedAt: 2026-08-01T05:51:04.9378120Z
---

Document how child source commits, child PM updates, parent pointer tasks, dirty worktrees, recursive checkout, and local validation remain separate. Record current pinned commits and the rule that Royale and Starfall never depend directly on one another.

Acceptance criteria:
- Child implementation and parent pointer advancement are separate tasks and commits.
- Recursive checkout and path-hint validation are documented and verified.
- Existing unrelated submodule work is preserved.

## Notes

- 2026-08-01 05:48 UTC - Documented separate child implementation/commit and parent gitlink tasks, dirty-worktree rules, recursive checkout identity checks, and kickoff pinned commits. `git submodule status` and both child worktrees were inspected before mutations; child roadmap commits and parent pointer advancement remain separately reviewable.
- 2026-08-01 05:50 UTC - Committed child PM bootstraps independently: Royale 174fa322ba44... (`[BUILD-025] Bootstrap shared character PM roadmap`) and Starfall ac1b034... (`[SF-0002] Bootstrap MMO PM roadmap`). The coordinator pointer commit will stage only both gitlinks plus this ownership/pin documentation; child worktrees are clean.
- 2026-08-01 05:51 UTC - Exact child commits pinned by the bootstrap pointer update: Royale 174fa32600887da2093bcf7cbc9ebf89dc92990f; Starfall ac1b03425da91cadd58e1cc76b1f4850dddbf76d.