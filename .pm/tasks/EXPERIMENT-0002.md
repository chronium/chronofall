---
id: EXPERIMENT-0002
title: Decide the narrowest experimental skeletal loader approach
track: EXPERIMENT
milestone: M1
dependsOn:
- ASSET-0002
- EXPERIMENT-0001
createdAt: 2026-08-01T05:34:31.5832330Z
modifiedAt: 2026-08-01T09:11:25.5614050Z
---

Using asset evidence and the Royale evaluation, present the smallest viable loader options. Treat loader choice, a new native dependency, and any permanent format as owner-reviewed contracts. Do not silently add a large importer, generic asset framework, retargeter, or production animation graph.

Acceptance criteria:
- Record the focused SimpleMesh patch, strict-subset adapter, and new-importer options with the owner-approved choice and rationale.
- Define the exact M1 loader subset, including truthful LINEAR translation, rotation, and scale handling and deterministic unsupported-interpolation failures.
- Preserve glTF source coordinates and assign timing, looping, transform evaluation, and GPU-palette behavior to the experiment data contract.
- Record that ChronoFall acquires only dependencies consumed by parent experiments or shared modules and never references child dependency paths.
- Preserve independently useful Royale and Starfall dependency acquisition; do not bulk-promote third-party libraries.
- Create and wire one inactive follow-up task for scoped SimpleMesh acquisition, patching, adapter implementation, and focused tests.
- Do not change source, assets, third-party files, children, or gitlinks.

## Notes

- 2026-08-01 09:11 UTC - Selected a focused ChronoFall-owned patch over pinned SimpleMesh for truthful LINEAR translation, rotation, and scale import while retaining interpolation metadata; the experiment adapter will deterministically reject unsupported interpolation and malformed channels. Published `pm://project/prj_E7QP3LUocfY7k3PYM-EQOlqc/wiki/experiments/skeletal-loader-decision` and added the per-consumer third-party ownership rule to `pm://project/prj_E7QP3LUocfY7k3PYM-EQOlqc/wiki/architecture/shared-engine-boundaries`. Created inactive follow-up EXPERIMENT-0012 depending on EXPERIMENT-0003 and made EXPERIMENT-0005 wait on both EXPERIMENT-0004 and EXPERIMENT-0012. Coordinator PM validation and `pm doctor` passed with no issues; linked-family inspection reported three available/readable/trusted members and zero warnings; `git diff --check` passed; Royale and Starfall worktrees and gitlinks were unchanged. No source, assets, third-party files, importer patches, or child PM data were changed.