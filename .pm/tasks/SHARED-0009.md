---
id: SHARED-0009
title: Add bone masks and layered animation
track: SHARED
milestone: M2
dependsOn:
- SHARED-0008
createdAt: 2026-08-01T05:34:58.2855500Z
modifiedAt: 2026-08-01T18:41:09.6202500Z
---

Add bounded binary joint masks and stateless layered-pose composition to the coordinator-owned BCL-only character-presentation core, then prove the contract with the selected Quaternius `Walk_Loop` and `Sword_Attack`. Child presentation remains responsible for choosing clips, masks, clocks, signals, and transition policy; animation never authorizes gameplay.

## Acceptance criteria

- An immutable joint mask is bound to one skeleton instance, copies its binary membership, validates exact joint count, and can construct a deterministic subtree mask from a valid joint index.
- A stateless layer operation requires base pose, layer pose, and mask to share the same skeleton instance plus a finite amount in `[0, 1]`; unmasked local transforms remain exactly from the base pose while masked local translation/scale and normalized shortest-path rotation use the approved interpolation contract.
- The shared core remains BCL-only and contains no clip selection, clocks, transition state, anatomical-name policy, renderer, importer, child, server, or gameplay dependency.
- The provisional harness demonstrates `Sword_Attack` layered over continuously advancing `Walk_Loop` through the exact `spine_01` subtree of the selected 65-joint rig: 53 upper-body joints are masked while root, pelvis, and both complete leg chains remain locomotion-owned.
- Key `3` retains the existing full-body attack and key `4` signals the layered comparison; direct clip browsing, locomotion crossfades, pause/restart, skeleton visualization, and complete diagnostics remain available.
- The original M1 and full-body blend capture suites and recorded fingerprints remain unchanged; a separate deterministic layered capture suite records the new evidence and two independent suites compare byte-for-byte.
- Core and selected-asset tests prove mask validation and immutability, subtree membership, exact unmasked preservation, masked endpoints/interpolation, finite global poses/palettes, interruption/retrigger behavior, and advancing lower-body locomotion.
- Debug and Release builds/tests pass, the opt-in native macOS ARM64 Metal integration passes, and the owner explicitly validates layered transition appearance and controls.
- Shared-presentation and experiment wiki document semantics, ownership, selected-mask evidence, captures, validation, and exclusions.
- No weighted per-joint mask, blend tree, normalized locomotion parameter, shared transition player, root motion, retargeting, event marker, animation graph, shader/renderer change, asset conversion/cooking, child change, or gitlink update is introduced.

## Notes

- 2026-08-01 18:41 UTC - Implemented and validated the bounded layered-animation contract. Added immutable skeleton-specific binary masks with deterministic subtree construction, stateless masked local-TRS composition, shared interpolation reuse, selected-asset evidence for the exact 53-of-65-joint `spine_01` subtree, and a harness comparison between key `3` full-body and key `4` upper-body `Sword_Attack` over continuously advancing `Walk_Loop`. Debug and Release builds succeeded with zero warnings; each configuration passed 93 tests (51 core, 28 experiment, 8 SimpleMesh adapter, 6 SDL GPU). Focused formatting verification passed for every changed coordinator-owned project. The opt-in native macOS ARM64 Metal integration test passed. Two independent complete M1, blend, and layered capture runs compared byte-for-byte; existing M1/blend fingerprints remained unchanged and the layered fingerprints/hashes match the experiment wiki. The owner exercised full-body/layered comparison, repeated layered signals, locomotion changes, pause/restart, and existing diagnostics, and confirmed that the result looks great. The owner reviewed the six-frame sheet and chose not to preserve it as project history; no capture artifact was committed. Royale and Starfall remained clean at their pinned commits, with no child or gitlink changes.