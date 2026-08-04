---
title: Shared Box3D Runtime Boundary
createdAt: 2026-08-04T12:20:28.5879190Z
modifiedAt: 2026-08-04T12:20:28.5879190Z
---

## Decision

ChronoFall owns a bounded, child-independent Box3D source/native/managed runtime for authoritative ground-plane simulation. The supported source pin is official Box3D commit `3fc20f5b453ba9e14cdf54ecafa87a2a4bcdf53c` under the MIT licence. Ignored upstream source and build products are obtained only through the coordinator workflows in `thirdparty/`.

## Project and dependency boundary

`ChronoFall.Box3D.Bindings` owns the narrow raw C ABI and focused native resolver. `ChronoFall.Box3D` owns managed world, body and shape lifetime, validation, box/capsule construction, filtering and bounded mover queries. Parent projects do not depend on either game.

A child headless simulation may directly reference only:

```text
$(ChronoFallFamilyRoot)src/ChronoFall.Box3D/ChronoFall.Box3D.csproj
```

The bindings are transitive. No child references the raw bindings directly. This source contract adds no package, feed, `buildTransitive` target or child-relative path.

## Supported native layout

- macOS ARM64: `runtimes/osx-arm64/native/libbox3d.dylib`
- Linux x64: `runtimes/linux-x64/native/libbox3d.so`

Missing artifacts and unsupported platforms fail explicitly. Windows and independent package distribution remain deferred. Native binaries, source checkouts and build directories remain ignored and reproducible from the committed pin and scripts.

## Managed contract

The promoted surface is limited to finite Box3D-native single-precision metres, world lifecycle and stepping, static/kinematic/dynamic bodies, transforms and linear velocity, box and capsule shapes, collision filters, mover casts and copied collision-plane facts.

Worlds recursively own bodies and shapes. Disposal is idempotent; use after disposal fails. Time steps, dimensions and radii must be finite and positive; transforms and velocities must be finite; rotations must be normalized.

Mover-contact callback data is copied before returning, exposed immutably and sorted by native shape ID fields followed by plane values. Native traversal order is not authoritative. Callers still own stable game identity, creation order, fixed-tick scheduling, mapping from native shape IDs, and final gameplay application order. Cross-platform bitwise physics determinism is not promised.

## Authority and exclusions

The shared runtime supplies mechanics, not gameplay policy. Starfall and Royale retain simulation rules, entities, content conversion, collision layers, movement decisions and outcomes. Headless projects remain free of SDL, GPU, ImGui, rendering, editor and presentation assets.

This contract excludes debug drawing, character controllers, map formats, collision cooking, meshes, height fields, joints, general ray/overlap APIs, contact/event systems and a general physics abstraction. Starfall integration remains owned by `pm://project/prj_pkIpzx0fzFD4URjvqBuYrGZF/task/SIM-0008` after this prerequisite completes and that task receives its own approved plan.

## Validation

The boundary is validated by pinned upstream Box3D tests with native validation enabled, ABI sizes/offsets, native resolver behavior, managed ownership and malformed-input tests, hello-world stepping, box/capsule mover collision and filtering, immutable deterministic contact ordering, and a separate headless `ChronoFallFamilyRoot` consumer on macOS ARM64 and Linux x64.