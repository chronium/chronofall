---
id: EXPERIMENT-0005
title: Render one correct bind pose through SDL GPU
track: EXPERIMENT
milestone: M1
dependsOn:
- EXPERIMENT-0004
- EXPERIMENT-0012
createdAt: 2026-08-01T05:34:32.2761550Z
modifiedAt: 2026-08-01T14:30:53.7224160Z
---

Render the selected supplied Quaternius humanoid in a correct bind pose through SDL GPU using GPU skinning infrastructure.

Acceptance criteria:

- The authoritative input remains `assets/Quaternius/Universal Animation Library[Standard]/Unreal-Godot/UAL1_Standard.glb`.
- A provisional coordinator experiment assembly owns SDL GPU rendering and depends on the headless character core, while the standalone harness composes it with the SimpleMesh adapter.
- SDL3-CS is independently pinned, fetched, integrity-checked, and license-documented in the coordinator; coordinator code never references Royale's third-party checkout.
- The skeletal vertex ABI is position float3, normal float3, joint indices ushort4, and weights float4 with a 48-byte stride and 32-bit indices.
- The 65-matrix joint palette is transposed only at GPU upload and bound as a vertex-stage graphics-storage buffer; camera constants use a small vertex uniform.
- HLSL is compiled to MSL and SPIR-V, with deterministic diagnostic section colors, back-face CCW culling, D32 depth, and simple directional lighting.
- A main-thread macOS ARM64 harness supports hidden offscreen validation/readback and visible owner inspection.
- Automated validation proves a nonblank, opaque, centered, unclipped image with both mesh sections and a translated-palette probe proves the shader consumes joints, weights, and palette.
- Managed tests cover vertex layout and offsets, mesh conversion, palette packing/transposition, section conversion, and deterministic framing.
- SDL, GPU, shader, SimpleMesh, and presentation dependencies do not enter the headless core.
- The experiment contract, dependency provenance, commands, native evidence, and visual result are documented in the coordinator wiki.
- Explicit owner visual confirmation is required before completion.

Out of scope: animation playback, modular equipment, material or texture frameworks, asset conversion or cooking, shared-engine promotion, child changes, gitlink updates, retargeting, and general camera or rendering architecture.

## Notes

- 2026-08-01 14:30 UTC - Completed the coordinator-owned native bind-pose proof.

  Implementation:
  - Added provisional `ChronoFall.CharacterExperiment.SdlGpu` with a 48-byte position/normal/ushort4-joints/float4-weights vertex ABI, 32-bit indices, a 4,160-byte vertex storage-buffer palette, HLSL-to-MSL/SPIR-V shaders, D32 depth, diagnostic section colors, deterministic bounds framing, offscreen readback, and explicit native resource ownership.
  - Added the standalone main-thread `ChronoFall.CharacterExperiment.GpuHarness` and environment-gated xUnit launcher.
  - Independently pinned SDL3-CS at `a0a5276a874c0c48db705696ab7e2adc8b5db0a1`; verified `libSDL3.dylib` SHA-256 `35797abd1dc9e130f8e7ca8aeee33d68f8eecbf0af479184913297aaad4760ca`, preserved MIT and SDL zlib notices, and applied the reversible desktop-only build-selection patch.
  - Documented the GPU contract at `pm://project/prj_E7QP3LUocfY7k3PYM-EQOlqc/wiki/experiments/sdl-gpu-bind-pose` and linked it from the skeletal data contract.

  Validation:
  - `sh thirdparty/verify-simplemesh.sh`: passed.
  - `sh thirdparty/verify-sdl3-cs.sh`: passed, including pin, licenses, reversible patch, ARM64 Mach-O type, and native hash.
  - `dotnet build ChronoFall.slnx -m:1 --no-restore`: passed with 0 warnings and 0 errors.
  - `dotnet test ChronoFall.slnx -m:1 --no-restore --no-build`: 42/42 passed.
  - `CHRONOFALL_GPU_TESTS=1 dotnet test tests/ChronoFall.CharacterExperiment.SdlGpu.Tests/ChronoFall.CharacterExperiment.SdlGpu.Tests.csproj -m:1 --no-restore --no-build`: 7/7 passed, including the native Metal harness.
  - Native bind render: MSL, 24,803 rendered pixels, section classifications 22,672/2,131, unclipped bounds `111,62-463,449`, fingerprint `408d3a4c16278bbc`.
  - Palette transport probe: 19.39-pixel centroid shift, fingerprint `4fd2e63aea97f7a3`.
  - Coordinator PM validation passed; linked family returned zero warnings; `git diff --check` passed.
  - The BCL-only core has no project references. Royale and Starfall remained clean, and no gitlink changed.
  - On 2026-08-01 the owner viewed the native Metal window and explicitly confirmed: “I see a character!” The required visual gate is satisfied.