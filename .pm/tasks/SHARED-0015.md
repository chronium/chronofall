---
id: SHARED-0015
title: Harden character-presentation edge contracts
track: SHARED
milestone: M2
priority: low
dependsOn:
- SHARED-0008
createdAt: 2026-08-01T18:16:02.3289170Z
modifiedAt: 2026-08-01T18:16:06.0279450Z
---

Resolve the two low-severity contract edges identified after the first shared character-presentation promotion without widening the shared-engine scope.

## Acceptance criteria

- Decide and durably document the supported world-transform contract for `SdlGpuSkinnedCharacterRenderer`; until a proper normal-matrix contract is explicitly approved, non-uniform scale is either rejected with a clear diagnostic or documented and tested as unsupported.
- Decide the experiment-controller contract for repeated requests targeting the current locomotion destination while a blend is active; repeated requests become idempotent, or an edge-triggered caller requirement is made explicit and covered by tests.
- Add focused regression coverage for the selected behavior and update the shared-character-presentation or experiment wiki where the contract belongs.
- Preserve caller ownership of camera, scheduling, gameplay mapping, and transition policy; do not promote `CharacterPlaybackController` into the shared core.
- Do not introduce a normal-matrix pipeline, blend tree, animation graph, child source change, or gitlink update unless separately planned and approved.