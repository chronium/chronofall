---
title: Starfall editor UI foundation
createdAt: 2026-08-05T07:29:05.1859490Z
modifiedAt: 2026-08-05T10:14:23.2029740Z
---

## Decision

Starfall's editor will preserve the proven hierarchy / viewport / inspector / lower-dock layout while replacing raw ImGui assembly with a deliberate Starfall-owned design language and focused immediate-mode UI vocabulary.

ChronoFall owns only the narrow caller-controlled native ImGui prerequisite. Starfall owns its application loop, design tokens, fonts, shell, interaction state, authoring documents, inspectors and workflows. Royale remains unchanged unless a later Royale-owned task adopts the shared backend.

## Shared prerequisite

`pm://project/prj_E7QP3LUocfY7k3PYM-EQOlqc/task/SHARED-0024` is the completed coordinator prerequisite. It implements the caller-controlled `ChronoFall.EditorUi.SdlGpu` project and owns:

- the independent coordinator ImGui.Net/cimgui and ImGuizmo pin, reproducible patch/build workflow and licence evidence;
- one managed ImGui context, SDL event forwarding, capture state, docking opt-in, font-atlas injection and DPI/framebuffer-scale inputs;
- SDL GPU draw-data preparation before a caller-owned pass and recording into that supplied pass;
- one macOS ARM64 native artifact containing only the approved neutral ImGui/ImGuizmo surface;
- the exact future allowlist permitting only the native `Starfall.Editor` host to reference the shared project.

It depends only on completed `SHARED-0016`, which owns the coordinator SDL3-CS pin and family-source boundary. Its complete contract is `pm://project/prj_E7QP3LUocfY7k3PYM-EQOlqc/wiki/architecture/shared-sdl-gpu-imgui-backend`.

Completed Royale EDITOR-001, EDITOR-002, EDITOR-003 and EDITOR-005 and Starfall CLIENT-0020 remain architectural evidence, not PM dependencies.

The backend does not acquire windows, GPU devices, command buffers, targets or swapchains; begin/end render passes; submit work; or own application layout, theme, fonts, panels, selection, documents or authoring. Multi-viewport, ImPlot, imnodes, graph editors, sequencers and Royale migration are excluded.

## Planned Starfall sequence

With the shared prerequisite complete, Starfall may groom its own backlog through a separate owner-directed cycle:

~~~text
SHARED-0024
  -> Starfall editor UI foundation
  -> Starfall editor interaction foundation
  -> Starfall EDITOR-0007 proper Draft 0 authoring
     ├── SERVER-0012 / CLIENT-0016
     └── auxiliary Assets / Validation / Log / status polish
~~~

The UI foundation owns the native Starfall executable, proposed design tokens, fonts, thin UI primitives, static shell and explicitly synthetic visual showcase. It does not depend on the Draft 0 graybox catalog.

The interaction foundation owns one selection/action-routing state, keyboard focus and shortcut suppression, transform-tool and cancellation state, dock/tab/panel/tool persistence, focus requests and generic command history. Generic command history owns execution, undo, redo and dirty checkpoints; concrete commands and mutation rules remain with EDITOR-0007.

EDITOR-0007 remains the first real Draft 0 document. It owns actual hierarchy concepts, picking, transforms, inspector, concrete commands, inline validation and deterministic authoritative/presentation compilation. Auxiliary surface polish follows EDITOR-0007 and consumes its real adapters without blocking the first proper scene or its downstream runtime consumers.

## Platform validation

The shared backend requires macOS ARM64 native build, load, rendering and owner visual validation. Source and build policy must avoid gratuitous macOS-only APIs.

Linux x64 build/load or native smoke validation is not currently required. Windows validation begins only when Windows becomes a supported ChronoFall-family target. Unsupported platforms are documented rather than implied.

## Visual validation

The first Starfall UI task will render deterministic, explicitly labelled synthetic showcase states for no selection, a selected transformable model, a spawn-like authoring object, validation errors, populated and empty assets, warning/error rows, active transform tools, keyboard focus, popup/modal/tooltip treatments and expanded/collapsed lower docks.

Palette, font and density values remain proposed until native showcase review. Architecture approval does not prevent focused visual tuning after evidence exists.

## Non-goals

This roadmap does not authorize native implementation, child PM mutation, a docking framework, scene format, reflective inspector, ECS, Pressure Cooker work, runtime UI, gameplay, final icons, Royale reskinning or automatic task activation.