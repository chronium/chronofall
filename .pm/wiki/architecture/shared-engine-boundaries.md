---
title: Shared Engine and Authority Boundaries
createdAt: 2026-08-01T05:44:07.0060700Z
modifiedAt: 2026-08-01T17:05:28.4058140Z
---

## Focus

ChronoFall may grow a focused shared engine for Royale and Starfall. It is not an arbitrary Unity-like framework. Reuse must be demonstrated before extraction.

Likely shared domains include native loading, SDL desktop lifecycle, SDL GPU device/targets, static and skinned rendering, skeleton/animation processing, modular equipment presentation, sockets/attachments, IK/aim presentation, text/debug rendering, Box3D wrappers, asset cooking, client/server asset separation, low-level transport, and proven editor infrastructure.

Gameplay simulation, protocol/replication, lifecycle, combat, AI, progression/economy, content schemas, product editor documents, and deployment topology remain child-owned.

## Third-party dependency ownership

ChronoFall acquires a third-party dependency only when a parent-owned experiment or shared module consumes it. The coordinator then owns its pin, fetch workflow, license evidence, and focused patches. Parent source must never reference dependency paths inside Royale or Starfall.

Each child retains the dependency acquisition needed for an independently useful checkout. Repeating an upstream pin across repositories is acceptable until a validated shared module and an explicit distribution contract justify consolidation. Do not bulk-copy child dependency sets or treat a relocated pin as shared-engine promotion.

The M1 loader decision is recorded at `pm://project/prj_E7QP3LUocfY7k3PYM-EQOlqc/wiki/experiments/skeletal-loader-decision`.

## Authority

Servers own attacks, shots, casts, hits, movement transitions, equipment changes, damage, death, and persistent state. Clients present those outcomes through rendering, animation, IK, effects, cameras, and smoothing. Headless code never depends on SDL windowing/GPU, ImGui, rendering, or editor code.

## Promotion gate

The M1 skinned-character proof satisfied its evidence gate, and `pm://project/prj_E7QP3LUocfY7k3PYM-EQOlqc/task/SHARED-0001` now owns the first deliberate promotion.

The resulting module and public-resource boundaries are recorded at `pm://project/prj_E7QP3LUocfY7k3PYM-EQOlqc/wiki/architecture/shared-character-presentation`. `ChronoFall.CharacterPresentation` preserves the proven BCL-only skeletal and deterministic animation semantics. `ChronoFall.CharacterPresentation.SdlGpu` owns the reviewed hidden GPU ABI and records mesh uploads, palette updates and draws into a caller-owned SDL GPU lifecycle.

The promotion does not make SimpleMesh permanent, freeze a cooked format, establish child package distribution, or approve materials, animated bounds, cross-rig animation, retargeting, root motion, blending, equipment, IK, animation graphs, render graphs, scenes or generalized components. Those remain separately approved contracts.

## Later authoring exploration

Typed authoring objects may eventually register serialization, inspector controls, validation, gizmos, icons/labels, debug drawing, and content cooking. They compile to product-specific runtime data: a Royale waypoint can compile to a compact navigation graph; a Starfall spawn object can compile to server-owned spawn data. Authoring structure must not force a reflective runtime ECS.