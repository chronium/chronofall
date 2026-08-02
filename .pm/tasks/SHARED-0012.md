---
id: SHARED-0012
title: Add two-bone IK and aim offsets
track: SHARED
milestone: M2
dependsOn:
- SHARED-0010
- SHARED-0011
createdAt: 2026-08-01T05:34:58.9644050Z
modifiedAt: 2026-08-02T06:33:58.6310100Z
---

Add coordinator-owned BCL-only two-bone IK and bounded one-joint aim-offset operations using the approved grip and Aim-reference contracts. The operations consume presentation inputs only; children retain anatomy, target selection, blending policy, and gameplay authority.

## Acceptance criteria

- `TwoBoneIkChain` binds one skeleton to a direct root -> middle -> end hierarchy and rejects invalid indices or topology.
- `TwoBoneIkSolver.ApplyModelSpace` consumes a same-skeleton local pose, finite off-hand target model frame, finite pole position, and finite amount in `[0, 1]`.
- The full solve preserves all local translations/scales and unrelated local transforms, rotates only the three chain joints, reaches a reachable target position, aligns the end-joint orientation, and clamps unreachable positions to current minimum/maximum reach.
- Pole degeneracy falls back first to the current bend plane and then to a deterministic axis; zero-length chains and unsupported non-rigid transform data fail explicitly.
- `AimOffsetEvaluator` consumes a finite Aim reference model frame, finite non-zero model-space direction, and symmetric finite yaw/pitch limits. It uses `+Z` forward, `+Y` up, positive yaw toward `+X`, and positive pitch toward `+Y`.
- `AimOffsetApplier` applies the roll-free bounded model-space delta to exactly one caller-selected joint with finite amount in `[0, 1]`, preserving every local translation/scale and unrelated transform.
- Deterministic tests cover validation, reachable/clamped solves, pole behavior, amount endpoints, exact preservation, aim conventions/limits, and direct composition with `WeaponGripEvaluator` and `AttachmentReferencePointEvaluator`.
- The selected `UAL1_Standard.glb` proof uses test-only `spine_03`, `upperarm_l -> lowerarm_l -> hand_l`, and `hand_r` mappings without making those shared policy.
- The native harness adds independent Aim and IK toggles plus a deterministic base/aim/IK/combined capture suite using synthetic metadata and no weapon asset; existing suites and fingerprints remain unchanged.
- Focused formatting, Debug and Release builds/tests, native macOS ARM64 Metal validation, two byte-identical new capture runs, and explicit owner visual validation pass.
- The shared-presentation wiki documents spaces, ordering, limits, failure behavior, authority, and exclusions.
- No weapon asset selection/import, weighted spine policy, multi-joint aim distribution, general constraint graph, renderer/shader change, serialization/cooking format, protocol/gameplay change, child change, gitlink update, or automatic history artifact is introduced.

## Notes

- 2026-08-02 06:33 UTC - Implemented the approved coordinator-owned BCL-only IK/Aim boundary. Added direct-hierarchy TwoBoneIkChain validation, bounded model-space two-bone solving with target orientation, reach clamping, pole/current-plane/deterministic fallbacks, exact non-rotation preservation, near-rigid transform validation, bounded direction-based AimOffset evaluation, and one caller-selected-joint application. Added deterministic synthetic, grip/reference integration, folded-chain, and selected UAL1_Standard tests. The provisional native harness now exposes 5=Aim and 6=off-hand IK, composes them after the existing sampled/blended/layered pose, and records a separate base/aim/IK/combined suite. Initial Metal fingerprints are b8ad9c8aa7d18175/9e2df0bd4b37fd65/189c992928f1ef01/11f48674481b3770; IK-only and combined errors record 0.000000 metres, two suites compare byte-for-byte, and all prior fingerprints remain unchanged. Focused formatting passed. Debug and Release builds completed with zero warnings/errors and each passed 125 managed tests (80 core, 29 experiment SDL GPU, 10 SimpleMesh adapter, 6 SDL GPU presentation). The final opt-in macOS ARM64 Metal integration passed. The owner exercised Aim, IK, combined modes, skeleton overlay, and all 43 clips and confirmed the controls/deformation work correctly and every animation still looks great. The temporary four-frame sheet and raw captures were not committed. PM validation, pm doctor, and git diff --check passed; family warnings were empty; Royale and Starfall stayed clean at their pinned commits; no child or gitlink changed.