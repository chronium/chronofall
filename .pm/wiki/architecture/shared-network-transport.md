---
title: Shared Low-Level Network Transport
createdAt: 2026-08-04T15:55:20.4923310Z
modifiedAt: 2026-08-04T15:55:20.4923310Z
---

## Decision

Coordinator task `pm://project/prj_E7QP3LUocfY7k3PYM-EQOlqc/task/SHARED-0023` owns a reusable low-level network transport boundary. It promotes only the opaque-packet transport demonstrated by Royale `pm://project/prj__-jXLQgm6GuD2gCKZ_bTa1m-/task/NET-001` and required by Starfall before connected walking.

The public source is split deliberately:

- `ChronoFall.Network.Transport` contains BCL-only contracts.
- `ChronoFall.Network.Transport.LiteNetLib` contains the sole source-built LiteNetLib adapter.
- children own every protocol, frame, message, admission, session, gameplay and runtime-composition decision.

ChronoFall does not depend on either child. Royale and Starfall never depend on each other.

## Public contract

`INetworkTransport` exposes start, connect, send, disconnect and caller-owned polling. Its payloads are opaque bytes. `INetworkEventHandler` receives connected, disconnected, copied packet, socket-error and latency facts synchronously while the caller polls.

Stable contract values are:

- `NetworkEndpoint`: non-empty host and remote port 1 through 65535;
- `NetworkPeerId`: non-negative, transport-instance-local and never a gameplay identity;
- `NetworkDelivery`: unreliable, reliable unordered, sequenced, reliable ordered and reliable sequenced;
- transport channels 0 through 63;
- bounded disconnect reasons with `Unknown` fallback;
- immutable peer statistics through the optional `INetworkTransportDiagnostics` interface.

Received payload memory is copied before it is reported and may be retained by the caller. The transport adapter is a single-caller, non-concurrent object. Callbacks occur only during `Poll`; no shared background callback or game loop is introduced.

## LiteNetLib adapter

LiteNetLib is independently pinned by ChronoFall at commit `37cbf5ab608a4dbd0e491c528a0c14c1e09f1cba` from `https://github.com/RevenantX/LiteNetLib`. The ignored checkout is compiled directly from `thirdparty/repos/LiteNetLib/LiteNetLib/LiteNetLib.csproj`. The MIT licence snapshot, fetch/verify workflow and no-patch evidence are coordinator-owned.

The adapter:

- accepts `Start(0)` for an ephemeral local listen port and fixed ports 1 through 65535;
- starts once, fails explicitly before start or after disposal, and disposes idempotently;
- accepts incoming transport connections so product admission can run above it;
- enables peer statistics and maps every supported delivery/disconnect value;
- copies received packet bytes before the LiteNetLib reader is recycled.

No NuGet package or feed is introduced, and coordinator builds suppress the upstream project's package-on-build default.

## Ownership above transport

The following remain child-owned and must not enter either shared project:

- protocol facts, deterministic serialization and framing;
- authentication, signed join tickets, admission and gameplay sessions;
- product channel assignments and connection lifecycle policy;
- retries, timeouts, encryption, NAT traversal and deployment topology;
- commands, snapshots, prediction, reconciliation and simulation;
- simulated impairment, telemetry presentation and operational policy.

Transport acceptance does not admit a player. Starfall's World consumes its join-ticket protocol and creates its gameplay session above this boundary. Once admitted, gameplay availability remains independent of identity, chat and operations services.

## Family source consumption

A child process/composition project may directly reference only:

```text
$(ChronoFallFamilyRoot)src/ChronoFall.Network.Transport.LiteNetLib/ChronoFall.Network.Transport.LiteNetLib.csproj
```

The BCL contracts project and LiteNetLib source are transitive. Direct child references to the contract project or upstream LiteNetLib checkout are not approved.

Potential audiences are only the actual network-I/O composition roots:

- Starfall Client and World after a separately planned adoption task;
- Royale Client and Server after a separately planned migration task.

Content, Protocol, Simulation, Editor and Balance Lab projects must not reference this boundary. Protocol remains transport-independent even though process hosts may compose protocol codecs with the transport.

## Validation evidence

The coordinator proves the boundary with:

- contract and lifecycle validation;
- all delivery and disconnect mappings;
- a real bidirectional UDP loopback using distinct delivery modes and channels;
- retained packet memory after later receives;
- peer statistics and disconnect behavior;
- architecture tests for child independence and graphics-free source;
- a `ChronoFallFamilyRoot` consumer that directly references only the adapter;
- exact source-pin, origin, clean-checkout and MIT licence verification.

Starfall adoption, CLIENT-0009 implementation and Royale migration are separate owner-directed cycles.