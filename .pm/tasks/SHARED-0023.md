---
id: SHARED-0023
title: Establish shared low-level network transport
track: SHARED
milestone: M3
dependsOn:
- SHARED-0016
- pm://project/prj__-jXLQgm6GuD2gCKZ_bTa1m-/task/NET-001
createdAt: 2026-08-04T15:47:15.4022830Z
modifiedAt: 2026-08-04T15:58:38.7276570Z
---

Establish a coordinator-owned, source-built LiteNetLib transport boundary proven by Royale NET-001 and required by Starfall before CLIENT-0009.

Implement two focused shared libraries: a BCL-only ChronoFall.Network.Transport contract and a ChronoFall.Network.Transport.LiteNetLib adapter that is the sole consumer of coordinator-pinned LiteNetLib source. Preserve the proven low-level surface: endpoints and ephemeral peer identities; all five delivery modes; connect, disconnect, send and caller-owned polling; connection, packet, network-error, latency and disconnect-reason events; and optional immutable peer statistics.

Acceptance criteria:
- Pin LiteNetLib at Royale's proven commit 37cbf5ab608a4dbd0e491c528a0c14c1e09f1cba with reproducible fetch/verify workflow, MIT licence/provenance evidence and no unrecorded patches.
- Compile the checked-out LiteNetLib project directly. Do not introduce a package, feed or package-distribution contract.
- Keep payloads opaque and copy received data before callbacks return. Preserve 64 channels, all five delivery mappings, auto-accepted transport connections, explicit lifecycle failures, idempotent disposal and synchronous caller-owned Poll semantics.
- Keep framing, protocol facts, join tickets, gameplay sessions, retries, encryption, NAT traversal, impairment simulation, snapshots, reconciliation and product connection policy out of shared source.
- Prove the contracts project is BCL-only, only the adapter references LiteNetLib, neither shared project references a child, and no graphics dependency enters either project.
- Provide focused lifecycle/mapping/diagnostic tests, a real bidirectional UDP loopback test, and a ChronoFallFamilyRoot smoke consumer that references only the adapter directly.
- Document shared transport ownership, permitted future child audiences and source-consumption policy. Starfall Client/World adoption and Royale migration remain separate child-owned tasks.
- Validate coordinator PM, family warnings, third-party pin, Debug/Release builds and tests, family-source consumption, diffs, submodules and child cleanliness.

This task owns coordinator source, third-party pin, tests, PM and wiki only. It does not mutate either child, advance gitlinks, activate CLIENT-0009 or push. Completion ends the cycle.

## Notes

- 2026-08-04 15:58 UTC - Implemented the coordinator-owned shared low-level network transport boundary.

  Implementation:
  - Added BCL-only ChronoFall.Network.Transport contracts and the isolated ChronoFall.Network.Transport.LiteNetLib adapter.
  - Preserved the five proven delivery modes, 64 channels, copied receive buffers, bounded disconnect reasons, caller-owned polling, latency facts and optional immutable peer statistics.
  - Pinned LiteNetLib independently at 37cbf5ab608a4dbd0e491c528a0c14c1e09f1cba with fetch/verify scripts, MIT licence snapshot, explicit no-patch record and package-on-build suppression.
  - Added a direct-adapter ChronoFallFamilyRoot consumer. Protocol, admission, sessions, gameplay exchange, connection policy and child composition remain excluded.
  - Created architecture/shared-network-transport and updated shared-engine, family-source and Draft 0 enabler documentation.
  - No Starfall, Royale, gitlink, package/feed or product runtime change was made.

  Validation:
  - LiteNetLib origin, exact HEAD, clean ignored checkout, licence snapshot and no-patch state verified; no .nupkg was produced.
  - Focused network project formatting check passed.
  - Debug and Release ChronoFall solution builds passed with zero warnings/errors.
  - Debug and Release solution tests passed: 252 tests each, including 41 network transport tests and real bidirectional UDP loopback.
  - Debug and Release family-source consumer runs passed through one direct adapter reference; output contained only the consumer, shared contracts, adapter, LiteNetLib and ordinary .NET host files.
  - PM MCP validation and pm doctor passed. Family inspection returned all three projects available/readable/write-trusted with zero warnings.
  - git diff --check passed; both children remained clean and both gitlinks remained unchanged at Royale 3b1bc45e4c8be76d110d8cf9613284db342db42e and Starfall 129a2adadb8329e18b46326e912797eb00e05a28.
  - No native visual validation or project-history artifact was required because this task has no visual output.