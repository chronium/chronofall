---
id: SHARED-0026
title: Enable Starfall.Client development instrumentation through the shared ImGui backend
track: SHARED
milestone: M4
dependsOn:
- SHARED-0016
- SHARED-0024
createdAt: 2026-08-06T07:22:48.3166220Z
modifiedAt: 2026-08-06T07:22:51.1685860Z
---

Extend the approved coordinator family-source boundary so Starfall.Client may later consume the completed caller-controlled SDL GPU ImGui backend for development-only instrumentation.

Ownership:
- ChronoFall owns the exact Starfall.Client source allowlist, reusable backend/native boundary, SDL/GPU lifecycle compatibility, pinned native dependencies, macOS ARM64 integration evidence, and architecture enforcement that keeps headless projects presentation-free.
- Starfall owns every product behavior: debug windows and their organization, menu items, F12 visibility, `--debug-ui-hidden`, input-capture policy, diagnostics, command frontends, persistence choices, and permanent game UI.

Acceptance criteria:
- Depend on completed SHARED-0016 and SHARED-0024.
- Extend the approved audience-specific family-source allowlist only to Starfall.Client development instrumentation through `ChronoFallFamilyRoot`.
- Preserve caller ownership of the SDL window, GPU device, command buffers, swapchain/render targets, render-pass scheduling, submission, application loop, and gameplay presentation.
- Reuse the existing `ChronoFall.EditorUi.SdlGpu` backend and its independently pinned native inputs; do not create a second ImGui bridge or a shared application shell.
- Prove the exact reference graph and ensure World, Simulation, Protocol, Content, Balance Lab, and headless editor document/compiler projects remain free of SDL, GPU, ImGui, renderer, and presentation dependencies.
- Require deterministic managed/native-boundary validation and macOS ARM64 native use when implemented.
- Update the shared-engine and family-source documentation with the distinct Starfall.Client audience and ownership limits.

Exclusions:
- No Starfall source or PM mutation in this task.
- No debug shell, windows, menu, F12 behavior, hidden-at-launch option, feature diagnostics, console, development-command protocol, permanent HUD, editor implementation, Royale migration, package/feed distribution, or gameplay work.