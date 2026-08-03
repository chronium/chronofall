---
id: EXPERIMENT-0014
title: Prove a technical UAL2 bow-body-animation sequence
track: EXPERIMENT
milestone: M3
priority: medium
dependsOn:
- ASSET-0010
createdAt: 2026-08-03T12:29:27.1809660Z
modifiedAt: 2026-08-03T12:32:36.8497810Z
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