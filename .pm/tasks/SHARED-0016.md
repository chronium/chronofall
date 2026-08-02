---
id: SHARED-0016
title: Establish independent child acquisition for shared presentation
track: SHARED
milestone: M2
dependsOn:
- SHARED-0001
- SHARED-0002
createdAt: 2026-08-02T07:50:10.8973880Z
modifiedAt: 2026-08-02T07:50:16.4913880Z
---

Define and provide the smallest reproducible mechanism by which independently buildable child repositories acquire the coordinator-owned character-presentation binaries and selected cooked client content. Decide package boundaries, version identity, publication or local-development feed workflow, integrity/provenance metadata, and deterministic child restore without parent-relative project references. Preserve parent-to-child dependency direction and client/server audience separation. This task owns coordinator packaging/distribution only; it does not integrate Royale or Starfall, advance a gitlink, stabilize the provisional cooked format, or publish unrelated engine modules.