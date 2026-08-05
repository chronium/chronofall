---
id: SHARED-0024
title: Provide a caller-controlled SDL GPU ImGui backend
track: SHARED
milestone: M3
priority: high
dependsOn:
- SHARED-0016
createdAt: 2026-08-05T07:28:31.4283210Z
modifiedAt: 2026-08-05T07:28:38.4045050Z
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