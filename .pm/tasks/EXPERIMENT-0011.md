---
id: EXPERIMENT-0011
title: Document findings and shared-promotion criteria
track: EXPERIMENT
milestone: M1
dependsOn:
- EXPERIMENT-0010
createdAt: 2026-08-01T05:34:33.6713070Z
modifiedAt: 2026-08-01T16:37:16.4267290Z
---

Record loader/format findings, asset compatibility, GPU contract, validation evidence, limitations, and the specific contracts that are sufficiently proven for promotion. M1 completes only when the supplied humanoid and at least one supplied animation render correctly with GPU skinning, deterministic tests, debug skeletons, captures, native execution, and owner validation.

## Notes

- 2026-08-01 16:37 UTC - Closed the M1 evidence review by publishing `pm://project/prj_E7QP3LUocfY7k3PYM-EQOlqc/wiki/experiments/skinned-character-proof-findings`, updating the shared-engine promotion gate, and recording M1 completion in the family roadmap. The decision promotes validated skeletal/skin/pose semantics, explicit deterministic animation sampling, pose/palette evaluation, the debug-global-pose boundary, and GPU-skinning behavior into `SHARED-0001` design. It deliberately does not freeze experiment assemblies, public type names, SimpleMesh as a permanent dependency, the exact GPU vertex/shader ABI, harness/camera/capture tooling, cooking formats, materials, animated bounds, cross-rig animation, retargeting, root motion, blending, equipment, IK, or animation graphs. Concrete demand resolves through Royale `pm://project/prj__-jXLQgm6GuD2gCKZ_bTa1m-/task/RENDER-012` and Starfall `pm://project/prj_pkIpzx0fzFD4URjvqBuYrGZF/task/CLIENT-0006`, both canonically waiting on `SHARED-0001`. No source, dependency, asset, child repository, or gitlink changed; the existing contact sheet remains the M1 visual checkpoint, so no duplicate artifact was created. Documentation-only validation uses PM family resolution, canonical task rereads, PM doctor, receipts, and repository diff checks rather than repeating the already recorded M1 binary test suite.