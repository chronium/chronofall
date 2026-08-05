---
id: SHARED-0024
title: Provide a caller-controlled SDL GPU ImGui backend
track: SHARED
milestone: M3
priority: high
dependsOn:
- SHARED-0016
createdAt: 2026-08-05T07:28:31.4283210Z
modifiedAt: 2026-08-05T10:14:37.9972510Z
---

Create the narrow coordinator-owned native UI prerequisite for the Starfall editor without creating a shared editor application.

Ownership:
- ChronoFall owns an independent ImGui.Net/cimgui and ImGuizmo pin, reproducible patches/build, licence evidence, native artifact packaging, ImGui context lifetime, SDL event forwarding, SDL GPU draw preparation/submission, docking opt-in, font injection and DPI hooks.
- Callers retain SDL window and GPU-device lifetime, command buffers, swapchain/render-target scheduling, application loop, dock layout, theme, panels, selection, documents and authoring concepts.
- Starfall owns its editor application, design language and product workflows. Royale remains unchanged unless a later Royale-owned task adopts the backend.

Acceptance criteria:
- Depend only on completed SHARED-0016, which owns the coordinator SDL3-CS pin and family-source boundary.
- Build only the minimum neutral ImGui, SDL3/SDL_GPU backend and ImGuizmo surface; exclude ImPlot, imnodes, graph-editor, sequencer and generic application-toolkit scope.
- Remain independent of Royale, Starfall, gameplay, content, editor documents and product UI.
- Establish the exact future Starfall.Editor family-source allowlist without granting unrelated child projects access.
- Require macOS ARM64 native build, load, rendering and owner visual validation.
- Keep source/build policy free of gratuitous macOS-only APIs. Linux x64 build/load or native smoke is not yet required; Windows validation begins only when Windows becomes a supported family target.
- Add deterministic managed/native boundary tests, licence/provenance documentation and explicit failure reporting.
- Do not implement a dock layout, theme, font choice, UI primitive kit, inspector, asset browser, scene/document format or Royale migration.

Architectural evidence, not dependencies:
- Royale EDITOR-001, EDITOR-002, EDITOR-003 and EDITOR-005 prove SDL desktop lifecycle, SDL GPU ImGui docking and ImGuizmo interaction.
- Starfall CLIENT-0020 proves the concrete family consumer already owns native SDL GPU presentation and would otherwise duplicate the native ImGui bridge/build for its editor.

## Notes

- 2026-08-05 10:14 UTC - Implemented the coordinator-owned caller-controlled SDL GPU ImGui boundary.

  - Pinned ImGui.Net `1f97beecfc9b83e1549e9782757cf85b1777cb9d`, cimgui `715802490eabca2fc86cf25b41b83aa7c5d6060d`, Dear ImGui `2a1b69f05748ad909f03acf4533447cac1331611`, cimguizmo `77e8ff47dc16a688edb06526b2f19c845b653bc7`, ImGuizmo `b10e91756d32395f5c1fefd417899b657ed7cb88` and the SDL source gitlink `f0e99e7c7f9aa90d5ce2e3b8a69f72c23faf257e`.
  - Added reproducible fetch, patch, verification and macOS ARM64 native-build workflows. The ignored `libchronofall_imgui.dylib` is a Mach-O ARM64 library with SHA-256 `5a5def7d998b1706cbf3b997be98dc470197e8f623d4629e0bffec50b511ce52`; required cimgui, ImGuizmo and ChronoFall bridge exports were verified.
  - Preserved upstream Evergine.Mathematics `2025.10.21.3204` with package SHA-256 `d417512a72fef6239c6736b5efee06ca4b54cd3b453e5c23ab3902035979d499` and committed licence/provenance evidence.
  - Added `ChronoFall.EditorUi.SdlGpu`, deterministic call-order/platform/native-boundary tests, a family-source consumer and a neutral caller-owned SDL GPU harness.
  - Debug and Release solution builds completed with zero warnings/errors. All 275 tests passed in both configurations; the focused Editor UI suite passed 23 tests after a fresh native rebuild.
  - The standalone family-source consumer built from the approved project reference, copied only the expected ARM64 SDL3 and ChronoFall ImGui native libraries, and reported deterministic 2.0x2.0 framebuffer scale.
  - Owner native validation on macOS ARM64 confirmed crisp rendering/resizing, interactive checkbox and slider, visible/draggable ImGuizmo translation control and clean close/Escape behavior. This neutral backend proof is not a visual-history checkpoint.
  - PM doctor and MCP validation passed; the linked family resolved all three projects with zero warnings. Royale, Starfall and both gitlinks remained unchanged.