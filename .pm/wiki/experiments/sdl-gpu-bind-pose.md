---
title: SDL GPU Bind-Pose Experiment
createdAt: 2026-08-01T14:24:15.9309170Z
modifiedAt: 2026-08-01T14:30:04.1603340Z
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

## Explicit exclusions

This task does not add animation playback, textures, a material framework, modular equipment, retargeting, cooking, a general camera, a production renderer, shared-engine promotion, child integration, child source changes, or gitlink updates.