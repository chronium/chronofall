---
id: EXPERIMENT-0014
title: Prove a technical UAL2 bow-body-animation sequence
track: EXPERIMENT
milestone: M3
priority: medium
dependsOn:
- ASSET-0010
createdAt: 2026-08-03T12:29:27.1809660Z
modifiedAt: 2026-08-03T13:27:26.3544210Z
---

Cook and visually prove a technical Universal Animation Library 2 bow-body-animation sequence on the established Quaternius technical humanoid.

Acceptance criteria:

- Consume the private UAL2 Source inventory and selected non-root-motion inputs from ASSET-0010 without copying or committing source-equivalent exports or an absolute source path.
- Add the approved narrow recipe-root/private-source-root separation to the existing character cooker while preserving current callers.
- Compare proven UAL1 `Idle_Loop` against UAL2 `Idle_No_Loop` before freezing either a seven-clip UAL2 evaluation recipe or a six-clip UAL2 bow-body recipe paired with the established UAL1 idle.
- Reuse the current deterministic animation, blending, SDL GPU, technical-humanoid, capture, and native-viewer paths; do not introduce a production multi-source cooker, retargeter, animation graph, combat controller, socket, weapon, or projectile.
- Demonstrate neutral idle, notch/draw, held aim, release, recovery, repeated shots, generic locomotion, and the bounded Arrow Rain candidates `Bow_Aim_Up` and `Bow_RapidShoot_Loop`.
- Inspect every 30 Hz sample of `Bow_Shoot` and `Bow_RapidShoot_Loop`; record owner-confirmed visual body-release frame/time markers and any uncertainty caused by the absent bow/string. These are presentation evidence, never authoritative combat timing.
- Produce deterministic ignored cooks/provenance and multi-timestamp captures, plus a continuous native 1920x1080 review sequence with clip/frame/time diagnostics.
- Obtain explicit owner visual validation and keep any project-history preservation as a separate owner decision.
- Update the owning task/wiki evidence, validate focused tests/native macOS ARM64 behavior/PM/diffs, commit only this task, and stop before Starfall selection or integration.

## Notes

- 2026-08-03 13:11 UTC - Implemented and owner-validated the bounded technical UAL2 bow-body sequence.

  - Added a backward-compatible `--recipe-root` character-cooker boundary so a committed recipe and licence evidence remain separate from an invocation-supplied private source root. Generated provenance records portable paths only.
  - Froze `assets/recipes/quaternius-ual2-source-bow-body.json` with exactly six non-root-motion UAL2 clips: `Walk_Fwd_Loop`, `Bow_Notch`, `Bow_Aim_Neutral`, `Bow_Shoot`, `Bow_Aim_Up`, and `Bow_RapidShoot_Loop`.
  - Compared UAL2 `Idle_No_Loop` against proven UAL1 `Idle_Loop`. Native review established that `Idle_No_Loop` is a one-shot head-shake “no” interaction gesture, with separate UAL2 `Yes` as a nod candidate. Both remain available for future NPC presentation but are excluded from this cook. Neutral/recovery uses UAL1 `Idle_Loop` through an experiment-only bit-exact 65-joint skeleton rebind; no retargeting was introduced.
  - Added a fixed task-specific sequence/controller and 1920x1080 native viewer with full-sequence, `Bow_Shoot` frame, rapid-shot frame, pause, restart, frame-step and skeleton diagnostics. It reuses the existing sampling, blending, GPU skinning, capture and readback path and introduces no animation graph, combat controller, socket, bow or projectile.
  - Two independent six-clip cooks were byte-identical: 1,636,937 bytes, SHA-256 `9b8daaf2ed481bcb14e553be8159f7be908a70fcd441bfd58a3e5e5dab9f7484`; generated provenance was also byte-identical.
  - Two native Metal capture suites were byte-identical across 11 key stages, all 21 `Bow_Shoot` samples and all 14 `Bow_RapidShoot_Loop` samples.
  - Owner validated the coherent idle/notch/aim/release/recovery/repeated-shot/locomotion/upward-aim/rapid-shot sequence. Owner frame review selected `Bow_Shoot` frame 3 at 100 ms as the first fully released body pose: frame 0 retains implied string contact and frame 1 is partially released. Rapid-shot release timing remains unresolved without a socketed bow/string/arrow. These are presentation markers, never authoritative combat timing.
  - Basic Arrow and Fire Arrow can share the same physical body sequence. `Bow_Aim_Up` and `Bow_RapidShoot_Loop` remain bounded Arrow Rain candidates for later Starfall selection.
  - Owner approved the curated contact sheet, preserved under `docs/project-history/2026-08-03-ual2-bow-body-sequence/` with canonical task/wiki ownership, CC0 provenance, generation details, dimensions and SHA-256.
  - Validation: Debug and Release builds succeeded with zero warnings; all 190 solution tests passed in both configurations; all 36 opt-in native GPU tests passed in Release; formatter verification passed for all five touched coordinator-owned projects. The solution-wide formatter reports only pre-existing pinned SimpleMesh/SDL3-CS source formatting.
  - No private absolute path, source-equivalent UAL2 asset, cooked output or raw capture is tracked. Royale and Starfall remained untouched with unchanged gitlinks.
- 2026-08-03 13:27 UTC - Review continuation: corrected the interactive rapid-shot frame inspector so both frame-inspection modes clamp at their clip endpoints instead of looping. The final `Bow_RapidShoot_Loop` sample now remains visible as frame 13; the existing offline capture path was already clamped and its preserved evidence is unchanged. Added an exact regression for frame 13. Validation passed: focused Debug tests 37/37, Release solution build with zero warnings/errors, full Release tests 191/191, touched-project format checks, and opt-in native GPU tests 37/37. Royale, Starfall, and both gitlinks remained unchanged.