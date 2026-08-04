---
id: SHARED-0021
title: Establish a bounded shared Box3D runtime boundary
track: SHARED
milestone: M3
dependsOn:
- SHARED-0016
- pm://project/prj__-jXLQgm6GuD2gCKZ_bTa1m-/task/PHYS-012
createdAt: 2026-08-03T08:13:47.3207790Z
modifiedAt: 2026-08-04T12:30:05.9430100Z
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

## Notes

- 2026-08-04 12:30 UTC - Implemented the bounded coordinator-owned Box3D runtime.

  - Source: official https://github.com/erincatto/box3d at `3fc20f5b453ba9e14cdf54ecafa87a2a4bcdf53c`; MIT licence snapshot matches upstream; no patch required.
  - Native workflows: Release shared builds use samples/tests/benchmarks/docs/profile/validation disabled for runtime artifacts. macOS ARM64 installs `libbox3d.dylib`; Linux x64 installs `libbox3d.so`. Both source/build/artifact trees remain ignored.
  - Managed boundary: `ChronoFall.Box3D.Bindings` owns only the required raw ABI and explicit resolver; `ChronoFall.Box3D` owns validated recursive world/body/shape lifetime, transforms/velocity, box/capsule shapes, filters, mover casts and immutable explicitly sorted collision-plane facts. The separate family consumer directly references only the managed project through `ChronoFallFamilyRoot`.
  - macOS ARM64 evidence: pinned upstream Debug static suite passed with `BOX3D_VALIDATE=ON` (all Box3D tests, 6.06 s); Release dylib is Mach-O ARM64, links only libSystem, and exports the required lifecycle/shape/mover symbols. Coordinator Debug and Release solution builds passed with zero warnings; 211 tests passed in each configuration, including 11 focused Box3D tests. Release headless consumer passed at mover fraction 0.375 and its output contains only the two Box3D assemblies, native dylib and ordinary .NET host files.
  - Linux x64 evidence: Release shared build passed under the local amd64 .NET 10 SDK container; `file`/ldd confirmed x86-64 ELF with only libc/libm/loader dependencies. The 11 focused Release tests and the headless consumer passed at fraction 0.375. The pinned upstream Debug suite also passed completely with `BOX3D_VALIDATE=ON` (10.74 s). GCC emitted non-fatal upstream maybe-uninitialized warnings in recording/world-snapshot implementation files outside the promoted API.
  - Durable architecture, family-source, third-party, validation and Starfall-enabler documentation now records the implemented headless-safe allowlist and its authority limits.

  Deferred exactly as planned: Starfall SIM-0008 integration, Royale migration, Windows, packages/feeds, debug rendering, controllers, map/collision cooking, meshes/height fields, joints, general query/event APIs and game-specific policy. No child source, PM data or gitlink changed.