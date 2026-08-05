---
title: Starfall editor UI foundation
createdAt: 2026-08-05T07:29:05.1859490Z
modifiedAt: 2026-08-05T07:29:05.1859490Z
---

## Decision

Starfall's editor will preserve the proven hierarchy / viewport / inspector / lower-dock layout while replacing raw ImGui assembly with a deliberate Starfall-owned design language and focused immediate-mode UI vocabulary.

ChronoFall owns only the narrow caller-controlled native ImGui prerequisite. Starfall owns its application loop, design tokens, fonts, shell, interaction state, authoring documents, inspectors and workflows. Royale remains unchanged unless a later Royale-owned task adopts the shared backend.

## Shared prerequisite

`pm://project/prj_E7QP3LUocfY7k3PYM-EQOlqc/task/SHARED-0024` owns:

- the independent coordinator ImGui.Net/cimgui and ImGuizmo pin, patches, licence evidence and reproducible native build;
- ImGui context lifetime, SDL event forwarding and SDL GPU draw preparation/submission;
- docking opt-in, font injection, DPI hooks and native artifact packaging;
- the exact future Starfall.Editor family-source allowlist.

It depends only on completed `SHARED-0016`, which owns the coordinator SDL3-CS pin and family-source boundary.

Completed Royale EDITOR-001, EDITOR-002, EDITOR-003 and EDITOR-005 and Starfall CLIENT-0020 are architectural evidence recorded in the task. They are not PM dependencies and do not make the dependency graph a bibliography.

The shared task excludes application/window/device scheduling, dock layout, theme, panels, selection, documents, authoring concepts, ImPlot, imnodes, graph editors, sequencers and Royale migration.

## Planned Starfall sequence

After the shared prerequisite is allocated, Starfall will groom its own backlog through a separate owner-directed cycle:

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