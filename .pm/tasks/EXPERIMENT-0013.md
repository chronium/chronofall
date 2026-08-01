---
id: EXPERIMENT-0013
title: Harden SDL GPU harness window and frame diagnostics
track: EXPERIMENT
milestone: M1
dependsOn:
- EXPERIMENT-0008
createdAt: 2026-08-01T15:37:44.9469060Z
modifiedAt: 2026-08-01T15:37:48.8623040Z
---

Address the two low-severity review findings in the SDL GPU character experiment harness before deterministic capture work.

## Scope

- Keep the fixed 512 x 512 experiment camera contract by disabling window resizing, rather than adding responsive projection behavior to this deterministic harness.
- Extend clip and sample-time diagnostic context across the full late GPU frame path: swapchain acquisition, command recording, render-pass setup and drawing, submission, and relevant fence or completion handling.
- Preserve the existing shared rendering path, deterministic fingerprints, native Metal behavior, interactive controls, and BCL-only skeletal core.

## Acceptance criteria

- The native experiment window cannot be resized into an aspect ratio that distorts the character.
- Any failure after pose creation retains the active clip name and sample timestamp, while preserving the underlying exception and useful GPU-stage context.
- Focused automated coverage verifies the fixed-window contract where practical and verifies diagnostic wrapping for injectable or testable late-frame failures without requiring a new rendering abstraction.
- Existing deterministic bind-pose, animated-palette, skeleton, loop-boundary, and interactive-browser tests remain passing.
- The focused native macOS ARM64 Metal validation passes.
- The experiment wiki and task notes record the chosen fixed-size policy and full-frame diagnostic boundary.

## Exclusions

No responsive camera system, generalized GPU error framework, renderer refactor, capture implementation, new UI/text rendering, shared-engine extraction, child changes, or gitlink updates.