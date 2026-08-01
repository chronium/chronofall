---
title: SDL GPU Bind-Pose Experiment
createdAt: 2026-08-01T14:24:15.9309170Z
modifiedAt: 2026-08-01T17:59:39.2963670Z
---

## Status and ownership

This page records the coordinator-owned SDL GPU proof begun by `pm://project/prj_E7QP3LUocfY7k3PYM-EQOlqc/task/EXPERIMENT-0005` and retained as the native regression harness after `SHARED-0001`.

`ChronoFall.CharacterPresentation.SdlGpu` now owns the hidden 48-byte vertex ABI, palette transport, skinned shaders, graphics pipeline, mesh resources, per-instance palette resources and draw recording. It consumes the BCL-only `ChronoFall.CharacterPresentation` contract.

`ChronoFall.CharacterExperiment.SdlGpu` remains the diagnostic host and owns bounds framing, its window and SDL device, skeleton overlay, offscreen targets, readback, captures, interactive controls and error context. `ChronoFall.CharacterExperiment.GpuHarness` composes that host with the provisional `ChronoFall.CharacterExperiment.SimpleMesh` adapter.

Neither child references these projects yet. No SDL, GPU, shader, SimpleMesh, asset or presentation dependency enters headless code. The promoted ownership boundary is documented at `pm://project/prj_E7QP3LUocfY7k3PYM-EQOlqc/wiki/architecture/shared-character-presentation`.

## Authoritative input

The unchanged input is:

`assets/Quaternius/Universal Animation Library[Standard]/Unreal-Godot/UAL1_Standard.glb`

The selected mesh contains 8,546 vertices, 41,232 32-bit indices, sections `M_Main` and `M_Joints`, and one 65-joint skin. The adapter remains responsible for loading and model-space validation; the renderer does not depend on SimpleMesh.

## SDL3-CS acquisition

| Property | Value |
| --- | --- |
| Official source | `https://github.com/ppy/SDL3-CS` |
| Pinned revision | `a0a5276a874c0c48db705696ab7e2adc8b5db0a1` |
| Binding license | MIT |
| Bundled SDL notice | zlib license |
| macOS ARM64 dylib SHA-256 | `35797abd1dc9e130f8e7ca8aeee33d68f8eecbf0af479184913297aaad4760ca` |

The coordinator fetches SDL3-CS into ignored `thirdparty/repos/SDL3-CS`; parent code never references Royale's dependency checkout. A build-only patch sets the upstream `CI_DONT_TARGET_ANDROID` switch so coordinator restore and build select desktop `net8.0` without Android or WebAssembly workloads. It does not change generated bindings or native behavior.

Use:

```sh
sh thirdparty/fetch-sdl3-cs.sh
sh thirdparty/verify-sdl3-cs.sh
```

## GPU data contract

`ChronoFall.CharacterPresentation.SdlGpu` owns this internal vertex ABI:

| Location | Field | Format | Offset |
| --- | --- | --- | --- |
| 0 | Position | `float3` | 0 |
| 1 | Normal | `float3` | 12 |
| 2 | Joint indices | `ushort4` | 24 |
| 3 | Weights | `float4` | 32 |

The source UV remains deliberately omitted because the validated path uses deterministic diagnostic colors rather than a material or texture framework. Indices remain 32-bit. Section order and index ranges are preserved.

The selected 65-matrix palette occupies 4,160 bytes. The shared renderer uploads it to an SDL GPU buffer with `SDL_GPU_BUFFERUSAGE_GRAPHICS_STORAGE_READ`, binds it to vertex storage slot 0 and owns the single CPU-to-shader transpose. The vertex shader declares one storage buffer; the draw transform occupies vertex uniform slot 0.

The renderer records uploads and draws into caller-owned command buffers and render passes. It does not own SDL initialization, the GPU device, windows, targets, camera policy, submission, captures or host lifecycle. Those responsibilities remain in the diagnostic host until another demonstrated consumer justifies a narrower shared contract.

## Matrix and shader boundary

CPU data keeps the established `System.Numerics` row-vector convention:

```text
local             = Scale * Rotation * Translation
posedGlobal[j]    = local[j] * posedGlobal[parent[j]]
palette[j]        = inverseBind[j] * posedGlobal[j]
```

CPU matrices remain untransposed. Each palette matrix and the view-projection matrix transpose exactly once when packed for HLSL. The vertex shader blends four joint matrices by the four source weights, skins positions with `w = 1`, skins normals with `w = 0`, and then applies the view-projection matrix.

The committed HLSL compiles through `shadercross` to MSL and SPIR-V. Native macOS ARM64 selects MSL with entry point `main0`; SPIR-V uses `main`. The pipeline uses back-face counter-clockwise culling, triangle lists, D32 depth testing/writes, two flat diagnostic section colors, and simple directional lighting.

## Skeleton debug overlay

`EXPERIMENT-0007` adds an experiment-only line overlay generated from the evaluated `SkeletonGlobalPose`. It does not infer joints from skinned vertices or reconstruct them from palette matrices.

For the selected 65-joint hierarchy, the builder emits:

- 64 yellow parent-to-child links;
- 65 red local X axes;
- 65 green local Y axes;
- 65 blue local Z axes;
- 259 lines and 518 vertices in total.

Axis length is 4% of the selected mesh-bound radius. Each line vertex is 28 bytes: a `float3` model-space position followed by a `float4` color. Links are emitted first in parent-first joint order, followed by X, Y, and Z axes for each joint. The builder is deterministic, validates positive finite axis length, and rejects non-finite transformed endpoints.

Dedicated `skeleton-debug` HLSL shaders consume the model-space position, color, and the existing transposed view-projection matrix. SDL GPU renders the vertices as a line list after the skinned mesh. The overlay disables depth testing and depth writes, making the complete hierarchy visible through the mannequin as an explicit x-ray diagnostic. It is not a promoted generic debug renderer.

The hidden harness retains the original mesh-only bind-pose and translated-palette probes, then renders a third bind-pose frame with the overlay. The overlay frame must differ from the baseline and contain meaningful yellow-link and green-axis pixel coverage. `--capture` remains the mesh-only capture; `--skeleton-capture <path>` writes the overlay frame. `--visible` displays the x-ray overlay for owner inspection.

## Harness and validation contract

The standalone harness initializes Cocoa and SDL on `Program.Main`, loads the selected GLB, computes the bind palette, and renders to a 512 by 512 offscreen target. Readback requires:

- every pixel to remain opaque;
- a nonblank rendered region;
- both diagnostic section colors;
- centered bounds with a clear unclipped margin;
- a stable frame fingerprint.

A second render post-multiplies every CPU palette matrix by a small translation before GPU packing. A distinct fingerprint and a material centroid shift prove the shader consumes the joint indices, weights, and storage-buffer palette instead of accidentally rendering the input positions directly.

Run automated validation:

```sh
dotnet restore ChronoFall.slnx -m:1
dotnet build ChronoFall.slnx -m:1 --no-restore
dotnet test ChronoFall.slnx -m:1 --no-restore --no-build
CHRONOFALL_GPU_TESTS=1 dotnet test tests/ChronoFall.CharacterExperiment.SdlGpu.Tests/ChronoFall.CharacterExperiment.SdlGpu.Tests.csproj -m:1 --no-restore --no-build
```

Run direct hidden capture or visible inspection:

```sh
dotnet tests/ChronoFall.CharacterExperiment.GpuHarness/bin/Debug/net10.0/ChronoFall.CharacterExperiment.GpuHarness.dll --capture artifacts/EXPERIMENT-0005/bind-pose.ppm
dotnet tests/ChronoFall.CharacterExperiment.GpuHarness/bin/Debug/net10.0/ChronoFall.CharacterExperiment.GpuHarness.dll --visible
```

The first native Metal capture produced 24,803 rendered pixels, section classifications 22,672 and 2,131, unclipped bounds `111,62-463,449`, bind fingerprint `408d3a4c16278bbc`, and a 19.39-pixel palette-probe centroid shift with fingerprint `4fd2e63aea97f7a3`.

Automated and agent capture inspection passed. On 2026-08-01, the owner viewed the native Metal window and explicitly confirmed seeing the character; the bind-pose visual gate is satisfied.

The first native Metal skeleton-debug frame emitted 259 lines, changed 2,076 pixels from the mesh-only bind pose, classified 872 yellow hierarchy-link pixels and 349 green Y-axis pixels, and produced fingerprint `c6ad39a45245afed`. The unchanged baseline remained `408d3a4c16278bbc`, and the translated-palette probe remained `4fd2e63aea97f7a3`.

Automated native validation and agent capture inspection passed. On 2026-08-01, the owner viewed the native Metal overlay and confirmed: “that looks like a skeleton, fingers and all.” The skeleton and joint visualization gate is satisfied.

## Looping animation playback

`pm://project/prj_E7QP3LUocfY7k3PYM-EQOlqc/task/EXPERIMENT-0006` extends the same provisional harness with the exact ordinally selected `Walk_Loop` clip from the unchanged UAL1 Standard GLB. The clip duration is 1.333333 seconds. It is an in-place source clip: the experiment does not extract or apply root motion.

For every animation frame, the presentation path performs the established sequence:

```text
Loop sample -> parent-first global pose -> inverseBind * posedGlobal
            -> transpose at GPU boundary -> 65-matrix palette upload -> GPU skinning
```

The visible loop uses SDL's performance counter at normal speed. A persistent upload transfer buffer is mapped and cycled for each 4,160-byte palette update; the mesh, shaders, vertex ABI, palette storage buffer, and render pipeline remain unchanged. The visible animation is mesh-only. Dynamic skeleton visualization remains owned by the later animated-debug task rather than being folded into this loop.

The hidden native harness renders mesh-only frames at time zero, the fixed 0.5-second sample, and the exact clip duration under `AnimationPlaybackMode.Loop`. It requires the start and exact-duration GPU fingerprints to match, the 0.5-second fingerprint to differ from start and bind pose, and all earlier bind-pose, palette-probe, and static skeleton diagnostics to continue passing.

Use `--animation-capture <path>` to write the deterministic 0.5-second frame. Existing `--capture` and `--skeleton-capture` behavior is unchanged.

On native macOS ARM64 Metal, start and exact duration both produced `68ba446d672887a0`; the 0.5-second sample produced `a2b427aea339d460`; bind pose remained `408d3a4c16278bbc`. The focused native integration test passed, and the saved 512 by 512 capture was inspected. On 2026-08-01, the owner viewed the normal-speed native loop and confirmed that it works correctly. The deformation and loop-continuity visual gate is satisfied.

## Interactive clip diagnostics

`pm://project/prj_E7QP3LUocfY7k3PYM-EQOlqc/task/EXPERIMENT-0008` turns the visible native harness into a narrow diagnostic browser over all 43 structurally compatible clips in source order. This availability does not change the M1 evidence selection: `Idle_Loop`, `Walk_Loop`, and `Sword_Attack` remain the selected idle, locomotion, and attack clips.

Controls are:

| Input | Action |
| --- | --- |
| Left / Right | Select the previous or next clip with wrapping and restart it at zero. |
| 1 / 2 / 3 | Select `Idle_Loop`, `Walk_Loop`, or `Sword_Attack`. |
| Space | Pause or resume without changing the current sample. |
| R | Restart the current clip at zero. |
| D | Toggle the animated skeleton and joint-axis overlay. |
| Escape | Close the harness. |

Clip changes preserve the playing or paused state. The window title reports source index and count, exact clip name, resolved sample and duration, playing state, skeleton state, joint count, and palette count using invariant formatting. The console prints the control legend and the same state after every interactive change. Startup, skeleton-identity, timing, joint-count, palette-count, and GPU failures remain fail-fast and include clip/sample context through `GPU_HARNESS_FAILURE`.

Each frame is sampled once. Its parent-first `SkeletonGlobalPose` feeds both the skinning palette and, when enabled, `SkeletonDebugGeometry`. The diagnostic never reconstructs joints from inverse-bind palette matrices. Palette and skeleton vertices use separate persistent cycled SDL upload transfer buffers. When the overlay is disabled, no dynamic skeleton upload is performed.

The browser adds no text renderer or UI framework: live text uses the SDL window title and console. It does not add capture orchestration; deterministic multi-timestamp evidence remains owned by `EXPERIMENT-0009`.

Automated coverage proves navigation, wrapping, shortcuts, pause/resume, restart, skeleton toggling, invariant diagnostics, validation errors, and source ordering. All 43 clips were sampled at a representative timestamp and produced a finite 65-matrix, 4,160-byte GPU palette on the selected skeleton. The coordinator build passed with zero warnings, 54 tests passed, and the focused native macOS ARM64 Metal integration passed.

On 2026-08-01, the owner exercised the native controls. The console evidence includes all three selected shortcuts, pause/resume, animated skeleton on/off, and successful exit. The owner confirmed that it works correctly, satisfying the task's visual and interaction gate.


## Fixed-window and full-frame diagnostics

`pm://project/prj_E7QP3LUocfY7k3PYM-EQOlqc/task/EXPERIMENT-0013` hardens the interactive browser before deterministic capture work. The experiment camera remains a fixed 512 by 512 contract: visible windows use no resize flag, and hidden validation windows remain hidden and non-resizable. Responsive projection and general camera behavior remain outside this harness.

At the start of each visible frame, the browser captures the exact clip and sample timestamp used for evaluation. One diagnostic boundary now covers pose evaluation, palette and optional skeleton uploads, title refresh, command-buffer and swapchain acquisition, visible-depth preparation, render-pass recording, and command submission. Failures add invariant clip, sample, and joint-count context while retaining the original operation-specific exception as the inner failure.

Focused policy tests cover visible and hidden window flags, successful diagnostic execution, and preservation of a simulated late GPU submission failure. The coordinator build passes with zero warnings; 57 tests pass across the core, SimpleMesh adapter, and SDL GPU projects; and the focused native macOS ARM64 Metal integration passes. The native run retained bind-pose `408d3a4c16278bbc`, palette-probe `4fd2e63aea97f7a3`, skeleton `c6ad39a45245afed`, and animation `a2b427aea339d460` fingerprints. On 2026-08-01, the owner confirmed that the window did not resize and that the controls and animations still looked correct.


## Deterministic multi-timestamp capture suite

`pm://project/prj_E7QP3LUocfY7k3PYM-EQOlqc/task/EXPERIMENT-0009` adds `--capture-suite <directory>` to the hidden native harness without changing the earlier individual capture flags. The suite writes exactly five 512 by 512 P6 PPM files through the established SDL GPU offscreen/readback path:

| File | Evidence | GPU fingerprint | SHA-256 |
| --- | --- | --- | --- |
| `bind-pose.ppm` | selected bind pose | `408d3a4c16278bbc` | `68cc300230a74917925d7785a233091f0b08eb7580224e0aebb8068571a0f18a` |
| `animation-0000ms.ppm` | `Walk_Loop` at 0.000 seconds | `68ba446d672887a0` | `1268476a5f5ff930e521e1e5401ba4cc043743e2e9c2bac768d558a3034138bc` |
| `animation-0500ms.ppm` | `Walk_Loop` at 0.500 seconds | `a2b427aea339d460` | `3cc9b0e6278c51a4616922fcc0ffa9ed6eb35fb8fdea807cc3a3808502a49e37` |
| `animation-1000ms.ppm` | `Walk_Loop` at 1.000 seconds | `85c5d42b4eac399d` | `9dc755418dc3372ab562afb845440865ff26b7c400d3c5a0fcfb510ed37c2f16` |
| `animation-loop-boundary.ppm` | exact 1.333333-second loop boundary | `68ba446d672887a0` | `1268476a5f5ff930e521e1e5401ba4cc043743e2e9c2bac768d558a3034138bc` |

Each PPM is 786,447 bytes: a deterministic 15-byte header followed by 512 by 512 RGB pixels. Two independent native macOS ARM64 Metal runs under `artifacts/EXPERIMENT-0009/run-a/` and `run-b/` compared byte-for-byte with no differences. Review-only PNG conversions live under the ignored `artifacts/EXPERIMENT-0009/review/` directory. The raw PPM suite, individual review PNGs, and duplicate run remain ignored. After task completion, the owner selected the labeled 3072 by 2240 contact sheet as a curated project-history artifact at `docs/project-history/2026-08-01-skinned-character-proof/contact-sheet.png`; its SHA-256 is `709b7633adcb37055338740749a90ad17d980ff570763a5d5798641f76492f44`. The dated path is intended for later use by a PM wiki image/timeline feature.

Agent visual inspection found the character fully framed with correct orientation and no visible deformation discontinuity. The three unique animation phases differ as expected, while the exact loop boundary reproduces the start image. Explicit owner review of deformation, scale, orientation, and animation appearance remains owned by `pm://project/prj_E7QP3LUocfY7k3PYM-EQOlqc/task/EXPERIMENT-0010`.

Run the suite with:

```sh
dotnet tests/ChronoFall.CharacterExperiment.GpuHarness/bin/Debug/net10.0/ChronoFall.CharacterExperiment.GpuHarness.dll --capture-suite artifacts/EXPERIMENT-0009/run-a
```

## Native macOS ARM64 owner validation

`pm://project/prj_E7QP3LUocfY7k3PYM-EQOlqc/task/EXPERIMENT-0010` closes the proof's explicit human-validation gate on native macOS ARM64 Metal. The coordinator solution built with zero warnings and errors, all 57 deterministic tests passed, and the focused native SDL GPU suite passed all 22 tests.

A fresh five-frame `--capture-suite` run reproduced the documented bind-pose and `Walk_Loop` fingerprints and was byte-identical to the preserved `EXPERIMENT-0009` evidence. The exact loop-boundary frame remained identical to time zero. The ignored validation files stay under `artifacts/EXPERIMENT-0010/`; the already approved contact sheet remains the sole curated project-history artifact because this validation did not produce a materially different checkpoint.

The owner then exercised the visible native browser with `Idle_Loop`, `Walk_Loop`, `Sword_Attack`, and the animated skeleton overlay. On 2026-08-01, the owner confirmed that deformation, upright orientation, scale and framing, animation appearance, controls, and overlay still worked correctly. Automated captures supported this decision but did not replace it. No source, asset, runtime manifest, child repository, or headless dependency changed during validation.

## Focused locomotion and action blending

`pm://project/prj_E7QP3LUocfY7k3PYM-EQOlqc/task/SHARED-0008` retains this host as a provisional proof for the promoted stateless pose blender.

The interactive path keeps Left/Right as direct inspection over all 43 clips. Key `1` requests `Idle_Loop`, key `2` requests `Walk_Loop`, and key `3` simulates receipt of a child-owned authoritative action event for `Sword_Attack`. Idle/Walk requests use a 0.25-second full-body crossfade. The attack uses a 0.10-second entry, clamps and plays once, then returns over 0.15 seconds to the continuously advancing selected locomotion loop. Repeated attack signals begin from the currently displayed pose. Pause, restart and skeleton visualization still operate over the evaluated blended pose.

These timings and interruption rules are harness policy only. Animation does not decide whether an attack happened, when it hit, or whether gameplay can resume.

The original `--capture-suite` files and M1 fingerprints remain unchanged. `--blend-capture-suite <directory>` adds six separate PPM files for idle, locomotion midpoint, walk, action entry, action body and action return. The initial Metal fingerprints are:

| Frame | Fingerprint |
| --- | --- |
| Idle source | `247702bbf7799ca9` |
| Locomotion midpoint | `620021052adb3084` |
| Walk destination | `a2b427aea339d460` |
| Action entry | `8d03eaf0fe5dd28e` |
| Action body | `b8ad9c8aa7d18175` |
| Action return | `771344f116121af7` |

Two independently generated suites must compare byte-for-byte before completion. Native owner visual confirmation remains the acceptance gate for transition appearance and controls.

### Native validation and visual checkpoint

Debug and Release each pass all 74 solution tests, including 36 BCL-only presentation tests, 24 experiment tests, 8 SimpleMesh adapter tests, and 6 SDL GPU presentation tests. The opt-in native macOS ARM64 Metal integration test also passes. Two independent runs of both the original M1 suite and the six-frame blend suite compare byte-for-byte; all M1 fingerprints and SHA-256 hashes remain unchanged.

The owner exercised Idle/Walk crossfades, one-shot and repeatedly signalled `Sword_Attack`, locomotion changes during the action, pause/restart, direct clip browsing, and the skeleton overlay in the visible native browser and confirmed that the behavior works surprisingly well. The owner selected the six-frame sheet at `docs/project-history/2026-08-01-animation-crossfades/contact-sheet.png` for permanent preservation without revision. Its SHA-256 is `ffd916ad5af750faeddf20d9608a472ad80dc1f652b176299fe377f183d9a791`; the reusable macOS AppKit compositor is `scripts/create-contact-sheet.swift`.

## Explicit exclusions

This experiment does not add a rendered text/UI system, general capture orchestration, blend tree, normalized locomotion parameter, shared transition player, bone mask, layered animation, root motion, retargeting, event marker, IK, general animation graph, textures, material framework, modular equipment, cooking, general camera, production renderer, child integration, child source changes or gitlink updates.