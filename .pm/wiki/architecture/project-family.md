---
title: Coordinator and Child Project Topology
createdAt: 2026-08-01T05:44:06.9895610Z
modifiedAt: 2026-08-02T11:44:59.1532330Z
---

## Stable family

```text
ChronoFall (prj_E7QP3LUocfY7k3PYM-EQOlqc)
├── royale (prj__-jXLQgm6GuD2gCKZ_bTa1m-)
└── starfall (prj_pkIpzx0fzFD4URjvqBuYrGZF)
```

The coordinator declaration and both child back-references are reciprocal. Each declared ID matches the owning checkout's `.pm/project_id.txt`. Submodule hints resolve to `royale/` and `starfall/`; identity does not derive from path proximity or Git remote.

At kickoff, PM reported all three projects available, readable, locally write-trusted, registry-resolved, and warning-free. Local trust is machine-only authority and is never inherited or committed.

## Ownership

ChronoFall owns the family roadmap, cross-project contracts, experiments, proven shared modules, and pinned child commits. Royale and Starfall each own their product architecture, gameplay simulation, protocol, content, PM configuration/wiki, source history, and build/release lifecycle.

Parent-owned shared modules may be consumed by either child but must not depend on either. Children never depend on one another.

## Pinned checkouts at kickoff

Initial checked-out commits before bootstrap:

- Royale: `5feafe2cf1fe6484fd4fc9d5d8ceeb13c331db51`
- Starfall: `20c25eebc60b1c72d5503c8a81e79cb683631208`

Independently committed PM bootstraps selected for the coordinator pointers:

- Royale: `174fa32600887da2093bcf7cbc9ebf89dc92990f` (`BUILD-025`)
- Starfall: `ac1b03425da91cadd58e1cc76b1f4850dddbf76d` (`SF-0002`)

A child implementation commit and its coordinator gitlink update remain separate focused Git commits, but they share one PM owner and one approved execution cycle. After the child task is complete and committed, the coordinator verifies identity, ancestry, and clean worktrees, then records a pointer-only commit automatically without creating a parent PM task. The pointer commit body preserves the canonical child task URI, stable project ID, and pinned commit.