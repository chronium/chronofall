---
id: EXPERIMENT-0008
title: Add interactive clip selection and diagnostics
track: EXPERIMENT
milestone: M1
dependsOn:
- EXPERIMENT-0006
- EXPERIMENT-0007
createdAt: 2026-08-01T05:34:32.9781060Z
modifiedAt: 2026-08-01T05:35:27.2789490Z
---

Add experiment-only controls for choosing compatible clips and inspecting timing, current sample, joint/palette counts, and validation errors without becoming a general editor.

## Implemented scope

- Browse all 43 same-skeleton clips in source order, starting on `Walk_Loop`.
- Left/Right select with wrapping; 1/2/3 select `Idle_Loop`, `Walk_Loop`, and `Sword_Attack`.
- Space pauses/resumes, R restarts at zero, D toggles the animated skeleton, and Escape closes.
- Clip changes restart at zero while preserving the playing or paused state.
- The SDL window title reports clip index/name, sample/duration, playback state, skeleton state, joint count, and palette count with invariant formatting.
- The console prints the control legend and every state change; failures retain clip/sample and contract context.
- Each frame evaluates one global pose for both GPU skinning and optional skeleton geometry.
- Palette matrices and skeleton vertices use separate persistent cycled SDL upload transfer buffers.
- No rendered text/UI framework or deterministic capture orchestration was added.

## Acceptance and evidence

- All 43 browser clips share the selected 65-joint skeleton and produce a finite 65-matrix, 4,160-byte GPU palette at a representative timestamp.
- Automated tests cover navigation, wrapping, exact shortcuts, loop advancement, pause/resume, restart, skeleton toggling, invariant diagnostics, and validation failures.
- Existing bind-pose, palette-probe, static skeleton, loop-boundary, and deterministic animation fingerprints remain passing.
- Native console evidence records all three selected shortcuts, pause/resume, animated skeleton on/off, and a successful exit.
- On 2026-08-01, the owner exercised the native browser and confirmed: “works correctly.”

## Validation

- `dotnet build ChronoFall.slnx --no-restore`: passed with zero warnings and errors.
- Core tests: 27 passed.
- SimpleMesh adapter tests: 8 passed.
- SDL GPU tests: 19 passed.
- Total automated tests: 54 passed.
- Focused native macOS ARM64 Metal integration: 1 passed with `CHRONOFALL_GPU_TESTS=1`.
- No Royale or Starfall source, PM data, commits, or gitlinks changed.

## Exclusions preserved

No rendered text/UI system, multi-timestamp capture suite, blending, root motion, retargeting, IK, animation graph, shared-engine extraction, asset conversion, child integration, or gitlink update.
