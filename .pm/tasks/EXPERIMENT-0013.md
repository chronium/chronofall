---
id: EXPERIMENT-0013
title: Harden SDL GPU harness window and frame diagnostics
track: EXPERIMENT
milestone: M1
dependsOn:
- EXPERIMENT-0008
createdAt: 2026-08-01T15:37:44.9469060Z
modifiedAt: 2026-08-01T15:48:10.3045110Z
---

Address the two low-severity review findings in the SDL GPU character experiment harness before deterministic capture work.

## Implemented scope

- Visible experiment windows use no resizable flag; hidden validation windows remain hidden and non-resizable.
- The fixed 512 by 512 camera contract is preserved without adding responsive projection or a general camera.
- Each visible frame captures its exact clip and sample timestamp once.
- One diagnostic boundary covers pose evaluation, palette and optional skeleton uploads, title refresh, command-buffer and swapchain acquisition, visible-depth preparation, rendering, and command submission.
- Diagnostic failures format clip, sample, and joint-count context invariantly and preserve the original operation-specific exception.
- No rendering abstraction, capture implementation, child change, or gitlink update was added.

## Acceptance and evidence

- Focused tests prove visible and hidden window flags, successful diagnostic execution, and preservation of a simulated late GPU submission failure.
- The coordinator build passes with zero warnings and errors.
- All 57 automated tests pass: 27 core, 8 SimpleMesh adapter, and 22 SDL GPU.
- The focused native macOS ARM64 Metal integration passes.
- Deterministic fingerprints remain unchanged:
  - bind pose: `408d3a4c16278bbc`
  - palette probe: `4fd2e63aea97f7a3`
  - skeleton debug: `c6ad39a45245afed`
  - animation sample: `a2b427aea339d460`
- The visible native run exited successfully after clip switching and pause/resume interaction.
- On 2026-08-01, the owner confirmed that the window did not resize and that the controls and animations still looked correct.
- Royale and Starfall source, PM data, commits, and gitlinks remain unchanged.

## Validation commands

- `dotnet build ChronoFall.slnx -m:1 --no-restore`
- `dotnet test ChronoFall.slnx -m:1 --no-restore --no-build`
- `CHRONOFALL_GPU_TESTS=1 dotnet test tests/ChronoFall.CharacterExperiment.SdlGpu.Tests/ChronoFall.CharacterExperiment.SdlGpu.Tests.csproj -m:1 --no-restore --no-build --filter FullyQualifiedName~SdlGpuIntegrationTests`
- Native visible harness with `--visible`
- `pm doctor`, PM validation, linked-family inspection, repository diff checks, and submodule status

## Exclusions preserved

No responsive camera system, generalized GPU error framework, renderer refactor, capture implementation, new UI/text rendering, shared-engine extraction, child changes, or gitlink updates.