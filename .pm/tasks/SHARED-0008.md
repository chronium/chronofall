---
id: SHARED-0008
title: Add locomotion and action blending
track: SHARED
milestone: M2
dependsOn:
- SHARED-0001
createdAt: 2026-08-01T05:34:58.0573880Z
modifiedAt: 2026-08-01T17:59:51.7637480Z
---

Add deterministic full-body pose crossfades to the coordinator-owned character-presentation core and prove them through the provisional native harness. Child presentation code remains responsible for selecting locomotion clips, consuming authoritative action signals, and owning transition policy; animation never authorizes gameplay.

## Acceptance criteria

- `SkeletonPoseBlender.Blend` operates on validated local poses using the same skeleton instance, a finite amount in `[0, 1]`, linear translation/scale interpolation, and normalized shortest-path quaternion interpolation.
- The BCL-only core remains independent of SDL, GPU, importers, children, servers, simulation, and gameplay signal policy.
- The experiment-only controller demonstrates 0.25-second Idle/Walk transitions, a child-signaled `Sword_Attack` with 0.10-second entry and 0.15-second return, deterministic interruption/retrigger behavior, and a continuously advancing locomotion return target.
- Direct 43-clip inspection, pause, restart, skeleton visualization, and complete diagnostics remain available.
- The original M1 capture suite and recorded fingerprints remain unchanged; a separate deterministic blend capture suite records locomotion and action transition evidence.
- Debug and Release builds and managed tests pass; the opt-in native macOS ARM64 Metal test passes; two blend capture suites compare byte-for-byte; and the owner explicitly validates transition appearance and controls.
- Architecture and experiment wiki document the math, authority boundary, harness policy, captures, exclusions, and downstream ownership.
- No blend tree, locomotion parameter, bone mask, layered animation, root motion, retargeting, event marker, animation graph, asset conversion/cooking, child change, or gitlink update is introduced.

## Notes

- 2026-08-01 17:59 UTC - Implemented the coordinator-owned BCL-only `SkeletonPoseBlender` with finite same-skeleton local-TRS blending, linear translation/scale, and normalized shortest-path quaternion interpolation. The provisional native harness now demonstrates 0.25-second Idle/Walk crossfades plus child-signalled `Sword_Attack` entry/body/return behavior (0.10-second in, play once, 0.15-second out), advancing locomotion return poses, retriggering from the displayed pose, locomotion retargeting during actions, unchanged direct clip browsing, pause/restart, skeleton overlay, and full diagnostics. Added a separate six-frame blend capture suite while preserving the original M1 suite and all recorded fingerprints/hashes. Debug and Release builds passed with zero warnings/errors and all 74 tests passed in each configuration; the opt-in native macOS ARM64 Metal integration passed; two independent M1 and blend suites were byte-identical. The owner exercised the native browser and confirmed the transitions work surprisingly well, then approved the six-frame 3072x2240 checkpoint sheet as-is. Preserved it at `docs/project-history/2026-08-01-animation-crossfades/contact-sheet.png` (SHA-256 `ffd916ad5af750faeddf20d9608a472ad80dc1f652b176299fe377f183d9a791`) and promoted the reusable macOS AppKit compositor to `scripts/create-contact-sheet.swift`; regenerating through that script produced a byte-identical copy of the reviewed candidate. Updated the shared presentation and experiment wiki. No child repository, asset, headless project, runtime manifest, or gitlink changed.