---
title: Shared Engine and Authority Boundaries
createdAt: 2026-08-01T05:44:07.0060700Z
modifiedAt: 2026-08-03T08:47:13.2675400Z
---

## Focus

ChronoFall may grow a focused shared engine for Royale and Starfall. It is not an arbitrary Unity-like framework. Reuse must be demonstrated before extraction.

Likely shared domains include native loading, SDL desktop lifecycle, SDL GPU device/targets, static and skinned rendering, skeleton/animation processing, modular equipment presentation, sockets/attachments, IK/aim presentation, text/debug rendering, Box3D wrappers, asset cooking, client/server asset separation, low-level transport, and proven editor infrastructure.

Gameplay simulation, protocol/replication, lifecycle, combat, AI, progression/economy, content schemas, product editor documents, and deployment topology remain child-owned.

## Third-party dependency ownership

ChronoFall acquires a third-party dependency only when a parent-owned experiment or shared module consumes it. The coordinator then owns its pin, fetch workflow, licence evidence, and focused patches. Parent source must never reference dependency paths inside Royale or Starfall.

The canonical full-client development environment is the shallow coordinator family checkout. Approved child clients may consume a narrow coordinator source allowlist through the single `ChronoFallFamilyRoot` property while retaining independent PM, source, product architecture, build-policy, and release ownership. Full client build isolation outside that family checkout is not currently required.

The shared SDL GPU project continues to compile the checked-out coordinator SDL3-CS pin from source. Children consume that dependency transitively through the approved shared project; they do not directly reference or independently package it for this path.

NuGet packages, feeds, versions, `buildTransitive` targets, source mapping, and content packages remain deferred until real Royale and Starfall integrations or independent CI/release requirements demonstrate the need. The complete source and generated-content contract is recorded at `pm://project/prj_E7QP3LUocfY7k3PYM-EQOlqc/wiki/development/family-source-consumption`.

The M1 loader decision is recorded at `pm://project/prj_E7QP3LUocfY7k3PYM-EQOlqc/wiki/experiments/skeletal-loader-decision`.

## Pending shared Box3D promotion

Coordinator task `pm://project/prj_E7QP3LUocfY7k3PYM-EQOlqc/task/SHARED-0021` owns the first bounded shared Box3D acquisition and managed-runtime boundary. It exists because two independent facts now meet: Royale has proven and updated its focused Box3D integration through `pm://project/prj__-jXLQgm6GuD2gCKZ_bTa1m-/task/PHYS-012`, and Starfall's approved walking slice needs authoritative ground-plane physics.

The task begins from Royale's audited upstream commit `3fc20f5b453ba9e14cdf54ecafa87a2a4bcdf53c` as evidence, but ChronoFall must own its own pin, licence record, fetch/build workflow, native artifacts, namespaces and child-independent source. Parent code must never reference Royale paths or absorb Royale gameplay.

The initial promotable surface is intentionally small: Box3D-native finite single-precision metre values, world lifecycle and fixed stepping, body/shape ownership, transforms and velocity, boxes/capsules, collision filtering, and only the bounded query/contact facts needed by authoritative ground-plane movement. Stable game identity, creation/application order, fixed-tick scheduling and sorting of unordered query results remain caller responsibilities. The shared boundary does not promise cross-platform bitwise physics determinism.

`SHARED-0021` remains allocated and todo. Starfall Box3D Cycle 3 completed through `pm://project/prj_pkIpzx0fzFD4URjvqBuYrGZF/task/SF-0009`: Starfall commit `84b1c94d3d3413954a20b09ea1d0445dfeb748f7` attached the canonical `SHARED-0021` URI to `SIM-0008`, and coordinator pointer commit `7530f552044b5888d44df7123c66996612c4655e` pins that reviewed child commit. `SIM-0008` now has a valid-but-waiting dependency on `SHARED-0021`. Source consumption and `SIM-0008` activation remain blocked until `SHARED-0021` completes and `SIM-0008` receives its own approved implementation plan. The approved family-source allowlist remains client-presentation-only until that implementation establishes its headless boundary, and the completed dependency wiring does not transfer gameplay or simulation ownership to the coordinator.

The task excludes debug rendering, character controllers, gameplay, map formats, collision cooking, editor integration, generalized physics abstractions, package feeds and child migration.

## Authority

Servers own attacks, shots, casts, hits, movement transitions, equipment changes, damage, death, and persistent state. Clients present those outcomes through rendering, animation, IK, effects, cameras, and smoothing. Headless code never depends on SDL windowing/GPU, ImGui, rendering, or editor code.

## Promotion gate

The M1 skinned-character proof satisfied its evidence gate, and `pm://project/prj_E7QP3LUocfY7k3PYM-EQOlqc/task/SHARED-0001` now owns the first deliberate promotion.

The resulting module and public-resource boundaries are recorded at `pm://project/prj_E7QP3LUocfY7k3PYM-EQOlqc/wiki/architecture/shared-character-presentation`. `ChronoFall.CharacterPresentation` preserves the proven BCL-only skeletal and deterministic animation semantics. `ChronoFall.CharacterPresentation.SdlGpu` owns the reviewed hidden GPU ABI and records mesh uploads, palette updates and draws into a caller-owned SDL GPU lifecycle.

The promotion does not make SimpleMesh permanent, freeze a cooked format, establish child package distribution, or approve materials, animated bounds, cross-rig animation, retargeting, root motion, blending, equipment, IK, animation graphs, render graphs, scenes or generalized components. Those remain separately approved contracts.

## Later authoring exploration

Typed authoring objects may eventually register serialization, inspector controls, validation, gizmos, icons/labels, debug drawing, and content cooking. They compile to product-specific runtime data: a Royale waypoint can compile to a compact navigation graph; a Starfall spawn object can compile to server-owned spawn data. Authoring structure must not force a reflective runtime ECS.