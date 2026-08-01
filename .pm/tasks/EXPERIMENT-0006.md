---
id: EXPERIMENT-0006
title: Sample and render one looping animation with GPU skinning
track: EXPERIMENT
milestone: M1
dependsOn:
- EXPERIMENT-0005
createdAt: 2026-08-01T05:34:32.5121390Z
modifiedAt: 2026-08-01T05:35:27.2671240Z
---

Sample one supplied compatible clip deterministically and render it as a loop through GPU skinning. Root motion, blending, retargeting, and a general animation graph are excluded.

## Implemented scope

- Exact ordinal clip selection: `Walk_Loop` from `assets/Quaternius/Universal Animation Library[Standard]/Unreal-Godot/UAL1_Standard.glb`.
- Explicit `AnimationPlaybackMode.Loop` sampling at time zero, fixed 0.5 seconds, exact duration, and SDL performance-counter time for the visible loop.
- Parent-first global evaluation, `inverseBind * posedGlobal` palette creation, one GPU-boundary transpose, and per-frame GPU skinning.
- Persistent cycled SDL upload transfer buffer for the 65-matrix, 4,160-byte palette.
- Mesh-only visible playback at normal speed with no root-motion application.
- Deterministic `--animation-capture <path>` while retaining the bind-pose and static-skeleton capture modes.
- Harness/session names broadened from bind-pose-only names without changing the established shader or vertex ABI.

## Acceptance and evidence

- CPU tests prove the 0.5-second palette is finite, 4,160 bytes, and distinct from bind pose.
- CPU and native GPU tests prove exact duration reproduces time zero under looping.
- Native macOS ARM64 Metal fingerprints: start `68ba446d672887a0`, 0.5 seconds `a2b427aea339d460`, exact duration `68ba446d672887a0`, bind pose `408d3a4c16278bbc`.
- Existing palette probe `4fd2e63aea97f7a3` and static skeleton diagnostic `c6ad39a45245afed` remain passing.
- The 512 by 512 deterministic 0.5-second capture was inspected and shows a correctly deformed walking pose.
- On 2026-08-01, the owner viewed the native normal-speed loop and confirmed: “that works correctly, very nice.”

## Validation

- `dotnet build ChronoFall.slnx --no-restore`: passed with zero warnings and errors.
- Core tests: 27 passed.
- SimpleMesh adapter tests: 8 passed.
- SDL GPU tests: 12 passed.
- Focused native Metal integration: 1 passed with `CHRONOFALL_GPU_TESTS=1`.
- No Royale or Starfall source, PM data, commits, or gitlinks changed.

## Exclusions preserved

No dynamic animated-skeleton overlay, clip controls, blending, root motion, retargeting, IK, animation graph, shared-engine extraction, asset conversion, child integration, or gitlink update.
