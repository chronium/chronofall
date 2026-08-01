---
title: SDL GPU Bind-Pose Experiment
createdAt: 2026-08-01T14:24:15.9309170Z
modifiedAt: 2026-08-01T15:09:53.8885080Z
---

## Status and ownership

This page records the coordinator-owned, provisional SDL GPU bind-pose experiment implemented by `pm://project/prj_E7QP3LUocfY7k3PYM-EQOlqc/task/EXPERIMENT-0005`.

`ChronoFall.CharacterExperiment.SdlGpu` owns experiment-only vertex packing, GPU palette transport, shader layout, bounds-based framing, SDL GPU resources, offscreen readback, and native validation. It consumes the BCL-only `ChronoFall.CharacterExperiment` data contract. The standalone `ChronoFall.CharacterExperiment.GpuHarness` is the composition root that loads the selected GLB through `ChronoFall.CharacterExperiment.SimpleMesh`.

This is not a promoted shared-engine module. Neither child references it, and no SDL, GPU, shader, SimpleMesh, asset, or presentation dependency enters the headless core.

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

The experiment packs each vertex as 48 bytes:

| Location | Field | Format | Offset |
| --- | --- | --- | --- |
| 0 | Position | `float3` | 0 |
| 1 | Normal | `float3` | 12 |
| 2 | Joint indices | `ushort4` | 24 |
| 3 | Weights | `float4` | 32 |

The source UV is deliberately omitted because this task uses deterministic diagnostic colors rather than a material or texture framework. Indices remain 32-bit. Section order and index ranges are preserved.

The selected 65-matrix palette occupies 4,160 bytes. It is uploaded to an SDL GPU buffer with `SDL_GPU_BUFFERUSAGE_GRAPHICS_STORAGE_READ` and bound to vertex storage slot 0. The vertex shader declares one storage buffer; a small camera matrix occupies vertex uniform slot 0. This avoids treating the full palette as pushed uniform data and establishes a transport usable by the next animation experiment without promoting it as a permanent API.

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

## Explicit exclusions

This experiment does not add dynamic animated-skeleton visualization, clip controls, blending, root motion, retargeting, IK, a general animation graph, textures, a material framework, modular equipment, cooking, a general camera, a production renderer, shared-engine promotion, child integration, child source changes, or gitlink updates.