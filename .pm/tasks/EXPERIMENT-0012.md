---
id: EXPERIMENT-0012
title: Establish coordinator skeletal loader adapter and scoped dependency acquisition
track: EXPERIMENT
milestone: M1
dependsOn:
- EXPERIMENT-0003
createdAt: 2026-08-01T09:10:26.9854900Z
modifiedAt: 2026-08-01T09:10:31.3981480Z
---

Establish coordinator-local acquisition for the approved SimpleMesh revision, add the focused LINEAR TRS/interpolation patch, and map loaded data into the experiment-only skeletal contract. Add deterministic selected-asset and failure tests. Do not add SDL, rendering, a new importer, a permanent format, or child dependencies.

Acceptance criteria:
- Pin SimpleMesh revision `9f46341e35fa5876fbea7b96bd021bc3abd7842d` in coordinator-local dependency-management files and preserve Apache-2.0 license evidence.
- Fetch into an ignored coordinator-local source directory; do not commit an upstream clone or reference `royale/thirdparty`.
- Add a focused, reproducible patch that exposes scale channels and retains interpolation metadata for translation, rotation, and scale.
- Map imported geometry, hierarchy, skin, inverse binds, joints/weights, and LINEAR TRS channels into the experiment-only data contract.
- Deterministically reject unsupported interpolation, unresolved targets, empty or malformed channels, non-finite values, and non-increasing key times with contextual errors.
- Load the selected UAL1 GLB without modifying it and verify the selected idle, locomotion, and attack clips retain all 65 translation, rotation, and scale channels.
- Add focused patch-application, selected-asset, and failure-path tests.
- Keep SDL/GPU, headless integration, root motion, retargeting, blending, permanent formats, new importers, and child changes out of scope.