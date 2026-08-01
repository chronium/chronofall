---
id: EXPERIMENT-0001
title: Evaluate Royale rendering and loading against skeletal requirements
track: EXPERIMENT
milestone: M1
dependsOn:
- ASSET-0001
createdAt: 2026-08-01T05:34:31.3528510Z
modifiedAt: 2026-08-01T07:48:03.4859550Z
---

Compare Royale's checked-out SimpleMesh integration, static mesh geometry, SDL GPU renderer, shader pipeline, asset cooking, readback, debug primitives, editor capture tooling, and GPU harness with the selected skeletal requirements. Identify exact reusable patterns and missing loader/rendering capabilities without changing Royale.

Acceptance criteria:
- A coordinator wiki capability matrix maps supplied skeletal requirements to pinned SimpleMesh, Royale's current wrapper, and Royale's SDL GPU path.
- Exact reusable loading, upload, shader, cooking, readback, capture, debug, and native-test patterns are documented separately from the missing skeletal capabilities.
- Loader, animation, palette, coordinate, resource, and format decisions are handed to EXPERIMENT-0002 or EXPERIMENT-0003 without selecting a permanent solution.
- Client presentation ownership and the rendering-free headless boundary are preserved.
- Evidence records the evaluated Royale and SimpleMesh revisions and is reproducible without modifying Royale or supplied assets.

## Notes

- 2026-08-01 07:48 UTC - Published `pm://project/prj_E7QP3LUocfY7k3PYM-EQOlqc/wiki/experiments/royale-skeletal-capability-evaluation` for Royale `174fa32600887da2093bcf7cbc9ebf89dc92990f` and pinned SimpleMesh `9f46341e35fa5876fbea7b96bd021bc3abd7842d`. A disposable probe confirmed raw female-base loading fails on the supplied `T_Eye_Normal_png.png` URI; an in-memory alias then exposed three skinned geometries and one 65-bone/65-inverse-bind skin. UAL1 loaded directly with one skinned geometry, one 65-bone skin, and 43 clips, but SimpleMesh exposes only 65 translation and 65 rotation channels per clip and discards the 65 source scale channels. Documented the static-wrapper/SDL GPU gaps, reusable upload/shader/readback/debug/capture/harness patterns, headless boundary, and explicit handoffs to EXPERIMENT-0002/0003 without choosing a loader or permanent format. Validation: 29 focused rendering/contact-sheet tests passed; the native macOS ARM64 SDL GPU harness passed outside the sandbox after the sandbox denied VSTest's local socket. No Royale, Starfall, asset, source, shader, or submodule-pointer change was made.