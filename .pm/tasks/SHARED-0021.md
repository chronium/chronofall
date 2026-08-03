---
id: SHARED-0021
title: Establish a bounded shared Box3D runtime boundary
track: SHARED
milestone: M3
dependsOn:
- SHARED-0016
- pm://project/prj__-jXLQgm6GuD2gCKZ_bTa1m-/task/PHYS-012
createdAt: 2026-08-03T08:13:47.3207790Z
modifiedAt: 2026-08-03T08:13:53.6678350Z
---

Establish the smallest coordinator-owned Box3D source, native-build, managed-binding, and ownership boundary needed by authoritative ground-plane simulation.

Acceptance criteria:
- Start from the audited upstream Box3D commit 3fc20f5b453ba9e14cdf54ecafa87a2a4bcdf53c proven by Royale PHYS-012; record the exact pin, upstream source, licence evidence, fetch workflow, and any focused coordinator patches without referencing Royale paths or adding a third-party Git submodule.
- Produce reproducible Release shared-library artifacts for the currently proven macOS ARM64 and Linux x64 development/server paths, with explicit runtime-native layout and resolver behavior. Windows and package/feed distribution remain deferred.
- Promote only the child-independent managed ABI and ownership subset required for fixed-step ground-plane movement: finite Box3D-native single-precision metre values, world lifecycle/stepping, bodies, transforms/velocity, box/capsule shapes, collision filtering, and the minimum bounded query/contact facts demonstrated by Starfall's movement need.
- Preserve caller-owned stable entity identity, body/shape creation order, fixed-tick scheduling, deterministic application order, and explicit sorting where Box3D query order is not guaranteed. Do not promise cross-platform bitwise physics determinism.
- Keep the shared projects headless and child-independent. They must not depend on SDL, GPU, ImGui, rendering, editor UI, Royale, Starfall, or game-specific simulation/content.
- Extend the approved ChronoFallFamilyRoot source-consumption contract with an explicit headless-safe Box3D allowlist and a coordinator-owned consumer/build probe; do not introduce NuGet feeds, packages, buildTransitive machinery, or child-relative paths.
- Add focused ABI/layout, native resolution, world/resource ownership, hello-world stepping, collision/query, malformed/non-finite input, and headless-artifact validation on supported platforms.
- Review and reuse only the proven Royale contracts needed by this scope; do not copy Royale namespaces/gameplay, migrate all Box3D APIs, add debug rendering, character controllers, map formats, collision cooking, a physics abstraction framework, or mutate either child.
- Update coordinator third-party, shared-engine, source-consumption, and validation documentation with exact supported and deferred boundaries.
- Leave Starfall integration, SIM-0008 implementation, Royale migration, child PM/source changes, and final platform packaging to separately planned child tasks.