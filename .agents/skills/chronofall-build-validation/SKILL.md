---
name: chronofall-build-validation
description: Validate ChronoFall coordinator and child work. Use for PM doctor, linked mutation verification, repository diffs, submodule checkout, child build/test selection, native macOS ARM64 rendering checks, deterministic captures, server artifact boundaries, or completion evidence.
---

# ChronoFall Build And Validation

## Validate PM And Git First

Run `pm doctor` in each mutated project. Re-read linked family warnings and cross-project dependency readiness. Confirm every linked receipt targets the expected stable ID and only expected paths.

Inspect coordinator and child diffs independently, then run `git diff --check`. Check `git submodule status` whenever a child commit or gitlink changed.

## Choose Proportionate Product Checks

- Policy/wiki/task-only: skill metadata validation, PM doctor, link/dependency reads, diff checks, and repository status.
- Coordinator experiment code: focused deterministic tests plus build for affected parent projects.
- Royale code: load Royale's build-validation skill and run its documented solution/focused/native commands.
- Starfall code: load its repository policy and documented commands once its build lifecycle exists.
- Shared contracts: build/test the shared module plus every affected child adapter; inspect headless dependency graphs and artifacts.

Do not invent commands that a repository does not configure.

## Validate Rendering And Animation

Require deterministic transform, hierarchy, inverse-bind, interpolation, looping, and timestamp tests. Use the supported SDL GPU native path on macOS ARM64, deterministic multi-timestamp captures, skeleton/joint diagnostics, and explicit owner visual confirmation.

Screenshots establish evidence but do not replace human validation of deformation, animation, controls, camera, UI, or feel.

## Protect Headless Outputs

Inspect server/simulation project references and packaged output. They must exclude SDL windowing/GPU, ImGui, shaders, textures, presentation code, and editor-only assets.

Report exact commands, outcomes, skipped platforms, sandbox limitations, human validation, PM status, commits, and remaining risk.
