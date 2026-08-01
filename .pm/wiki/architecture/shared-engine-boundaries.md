---
title: Shared Engine and Authority Boundaries
createdAt: 2026-08-01T05:44:07.0060700Z
modifiedAt: 2026-08-01T05:44:07.0060700Z
---

## Focus

ChronoFall may grow a focused shared engine for Royale and Starfall. It is not an arbitrary Unity-like framework. Reuse must be demonstrated before extraction.

Likely shared domains include native loading, SDL desktop lifecycle, SDL GPU device/targets, static and skinned rendering, skeleton/animation processing, modular equipment presentation, sockets/attachments, IK/aim presentation, text/debug rendering, Box3D wrappers, asset cooking, client/server asset separation, low-level transport, and proven editor infrastructure.

Gameplay simulation, protocol/replication, lifecycle, combat, AI, progression/economy, content schemas, product editor documents, and deployment topology remain child-owned.

## Authority

Servers own attacks, shots, casts, hits, movement transitions, equipment changes, damage, death, and persistent state. Clients present those outcomes through rendering, animation, IK, effects, cameras, and smoothing. Headless code never depends on SDL windowing/GPU, ImGui, rendering, or editor code.

## Promotion gate

The skinned-character experiment establishes the first candidate shared contracts. Promotion requires deterministic tests, correct GPU skinning, skeleton debug visualization, deterministic multi-timestamp captures, native macOS ARM64 execution, explicit owner visual confirmation, and a concrete child integration need.

Do not promote experimental loader choices, permanent skeletal formats, retargeting, generic animation graphs, or generalized scene/component frameworks without separate owner-approved contract tasks.

## Later authoring exploration

Typed authoring objects may eventually register serialization, inspector controls, validation, gizmos, icons/labels, debug drawing, and content cooking. They compile to product-specific runtime data: a Royale waypoint can compile to a compact navigation graph; a Starfall spawn object can compile to server-owned spawn data. Authoring structure must not force a reflective runtime ECS.