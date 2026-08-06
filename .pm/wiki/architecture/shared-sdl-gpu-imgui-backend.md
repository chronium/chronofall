---
title: Shared SDL GPU ImGui Backend
createdAt: 2026-08-05T10:06:17.9136640Z
modifiedAt: 2026-08-06T11:51:06.0619590Z
---

## Decision

ChronoFall owns a narrow caller-controlled immediate-mode editor UI backend in `src/ChronoFall.EditorUi.SdlGpu/`. It is shared native presentation infrastructure, not an editor application or UI toolkit.

The backend owns:

- one managed Dear ImGui context and its SDL3 / SDL_GPU backend lifetime;
- SDL event forwarding and capture-state reads;
- frame metrics, framebuffer-scale propagation and caller-injected font-atlas configuration;
- Dear ImGui draw-data finalization, SDL GPU upload preparation and recording into a supplied render pass;
- a single coordinator-built native library containing the approved cimgui, Dear ImGui, cimguizmo, ImGuizmo and SDL backend source.

The caller retains its SDL window and GPU-device lifetime, application/event loop, command buffers, swapchain or offscreen targets, render-pass begin/end, submission/cancellation, dock layout, theme, fonts, panels, documents, selection and workflows.

## Public contract

`SdlGpuImGuiBackend.Create` receives caller-owned `SDL_Window*`, `SDL_GPUDevice*`, color-target format and MSAA sample count. Creation may opt into docking, provide an explicit ini path and configure the font atlas. Ini persistence is disabled when no path is supplied.

Each visible frame follows this order:

~~~text
caller acquires window metrics
  -> backend.BeginFrame(metrics)
  -> caller emits raw ImGui / ImGuizmo UI
  -> caller acquires command buffer and swapchain/target
  -> backend.PrepareDrawData(commandBuffer)
  -> caller begins render pass
  -> backend.RecordDrawData(commandBuffer, renderPass)
  -> caller ends render pass and submits command buffer
~~~

Preparation must occur before the SDL GPU render pass because the upstream renderer uploads dynamic vertex, index and font-atlas data through the command buffer. The backend never acquires, begins, ends, submits or cancels these caller-owned resources.

A caller may use `EndFrameWithoutRendering` before preparation or `DiscardPreparedDrawData` after preparation. The explicit managed state machine rejects invalid ordering before unsafe native work. Backend calls are creating-thread-affine; calls always select the owned ImGui context first.

`SdlGpuImGuiFrameMetrics` requires positive logical/pixel sizes and positive finite delta time. It sets ImGui display size, delta time and framebuffer scale. The shared backend exposes this scale but never chooses product font sizes, style scaling or DPI policy.

`ImGuiCaptureState` reports mouse, keyboard and text-input capture. The caller remains responsible for applying that state to its own input routing.

## Window and docking boundary

Docking inside the one caller-owned SDL window is an explicit option. Platform multi-viewport is unsupported and rejected because the upstream feature creates secondary platform windows and render scheduling outside the caller-controlled contract.

No shared DockBuilder layout, editor panel model, viewport adapter or application shell is provided.

## Pinned source and native build

ChronoFall independently pins ImGui.Net revision `1f97beecfc9b83e1549e9782757cf85b1777cb9d` and the exact cimgui, Dear ImGui, cimguizmo and ImGuizmo submodule revisions recorded in `thirdparty/versions.env`.

The reproducible macOS ARM64 build includes only:

- Dear ImGui core, draw, widgets and tables;
- cimgui;
- the Dear ImGui SDL3 and SDL_GPU backends;
- cimguizmo and ImGuizmo;
- the small `chronofall_imgui` C ABI bridge.

ImPlot, imnodes, demos, graph editors, curves, gradients and sequencers are excluded. The build uses the exact official SDL headers recorded by the SDL3-CS `External/SDL` gitlink rather than machine-installed headers.

A reproducible patch removes excluded generated bindings and upstream prebuilt cimgui runtime files. The approved upstream `Evergine.Mathematics` version remains as the generated bindings' ABI vector dependency. All inspected licence snapshots and package hashes live under `thirdparty/licenses/`.

The ignored output is:

~~~text
thirdparty/artifacts/imgui/osx-arm64/lib/libchronofall_imgui.dylib
~~~

The shared project copies it and the pinned SDL3 library to `runtimes/osx-arm64/native/`. Both generated binding imports named `cimgui` and the ChronoFall bridge import named `chronofall_imgui` resolve to that one library. Unsupported platforms and missing artifacts fail with an explicit RID and expected path.

## Family source allowlist

The only approved child direct reference is:

~~~text
$(ChronoFallFamilyRoot)src/ChronoFall.EditorUi.SdlGpu/ChronoFall.EditorUi.SdlGpu.csproj
~~~

The allowlist has two distinct audiences, each requiring a separately approved child task:

- the native `Starfall.Editor` host established by completed `SHARED-0024`;
- the `Starfall.Client` composition root for development-only instrumentation, established by completed `SHARED-0026` and adopted only by `pm://project/prj_pkIpzx0fzFD4URjvqBuYrGZF/task/CLIENT-0029`.

SDL3-CS, ImGui.Net, Evergine.Mathematics and the native libraries remain transitive implementation details; Starfall must not reference their coordinator project or checkout paths directly.

Starfall World, Simulation, Protocol, Content, Balance Lab and the headless editor document/compiler remain outside this allowlist. The native editor host may compose the backend with separately approved presentation projects, but authoritative editor documents remain presentation-free.

Starfall-specific tokens, fonts, UI primitives, shell behavior, input policy, debug windows, commands, documents, inspectors and workflows stay in Starfall. Royale remains unchanged. Any future Royale adoption requires a Royale-owned plan and may reuse this proof without forcing an editor rewrite.

## Platform validation

Current support requires macOS ARM64 native build, load and interactive rendering validation. The C++ bridge and managed API contain no AppKit or Metal-specific application code; platform differences are isolated to the native build artifact and resolver.

Linux x64 build/load or native smoke is not yet required. Windows validation begins only when Windows becomes a supported family target. No unsupported artifact is implied.

## Non-goals

This contract does not provide a theme, font choice, UI primitive kit, dock layout, inspector, asset browser, reflection system, scene/document format, application framework, general docking framework, ImPlot, imnodes, editor migration or gameplay UI.