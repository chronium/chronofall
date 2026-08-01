---
title: Coordinator and Child Project Topology
createdAt: 2026-08-01T05:44:06.9895610Z
modifiedAt: 2026-08-01T05:44:06.9895610Z
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

- Royale: `5feafe2cf1fe6484fd4fc9d5d8ceeb13c331db51`
- Starfall: `20c25eebc60b1c72d5503c8a81e79cb683631208`

A child implementation commit and a coordinator gitlink update are separate tasks and commits. The parent pointer is advanced only after the child task is complete and committed.