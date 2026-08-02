---
id: SHARED-0018
title: Add a narrow reusable static-mesh rendering boundary
track: SHARED
milestone: M2
dependsOn:
- SHARED-0001
createdAt: 2026-08-02T16:19:37.5313310Z
modifiedAt: 2026-08-02T17:19:16.2142920Z
---

Add a focused coordinator-owned SDL GPU static-mesh presentation boundary for exact child-selected props and attachments.

Acceptance boundary:
- Reuse the established caller-owned SDL device, command buffer, render pass, target, camera, scheduling, resource-lifetime, source-consumption, and headless-isolation contracts.
- Define only the minimum immutable static geometry/material inputs, upload resources, and draw recording needed by one selected bow and bounded Draft 0 environment props.
- Keep the deterministic/non-native data boundary independent of SDL where practical and keep all GPU code client-only.
- Support only deliberately approved material inputs; do not assume engine-specific vegetation/wind shaders.
- Prove deterministic managed behavior and native macOS ARM64 rendering before completion.
- Do not add a scene graph, render graph, terrain, vegetation, streaming, model catalogue, general material system, child integration, or asset acquisition.

## Notes

- 2026-08-02 17:16 UTC - 2026-08-02 implementation and automated validation - Added the approved BCL-only immutable static geometry contract (`StaticVertex`, `StaticMeshSection`, `StaticMeshDefinition`) and the client-only SDL GPU static renderer (`SdlGpuStaticMeshRenderer`, uploaded mesh resources, section/whole-mesh draw recording, shader set and `StaticMeshDraw`). The reviewed internal ABI is 24 bytes: position float3 at byte 0 and normal float3 at byte 12, with 32-bit indices. Sections preserve opaque source material names only. Rendering is opaque flat RGB plus directional light and accepts translation, rotation and positive uniform scale while rejecting reflection, shear and non-uniform scale. No UV, texture, sampler, PBR, alpha, asset acquisition/cooking, scene, terrain, vegetation, child source or gitlink work was introduced.

  The isolated `--static-proof` native harness uses deterministic synthetic two-section geometry and proves baseline/transformed/repeated rendering without loading a supplied asset or changing the existing character capture path. Native macOS ARM64 MSL evidence: baseline `247198b9ff0e2862`, transformed `7d2c37c52e46fb19`, repeated baseline `247198b9ff0e2862`; 41,514 rendered pixels; section pixels 12,959/28,555; capture SHA-256 `5c45a75532678dc94a69334d6d693b08d0f4544c247a92177d893acc690f0b43` (786,447-byte ignored PPM). Two independent subprocess captures compared byte-for-byte. The full opt-in native suite passed both static and unchanged character regression tests.

  Validation completed so far: coordinator restore current; Debug and Release builds passed with zero warnings/errors; all 161 managed solution tests passed in both configurations; scoped format verification passed; Starfall restored, built with zero warnings/errors and passed 23 architecture tests while World, Simulation and BalanceLab outputs remained presentation/SDL/shader-free; PM MCP validation and `pm doctor` passed; family inspection returned all three projects readable/trusted with zero warnings; `git diff --check` passed; Royale, Starfall and both gitlinks remain unchanged. Wiki contracts were updated through PM with coordinator-only receipts. Explicit owner native visual confirmation and preserve/reframe/skip choice remain pending before completion.
- 2026-08-02 17:19 UTC - 2026-08-02 owner validation and project-history checkpoint - The owner inspected the fixed native macOS ARM64 Metal window and confirmed: “we have an orange and a blue box!” This approves the visible section colours, lighting, geometry, framing and stable native presentation. The owner explicitly requested preservation. The curated baseline is tracked at `docs/project-history/2026-08-02-static-mesh-rendering/static-mesh.png` (512 by 512, SHA-256 `6bd6e1be6a75a5fe4c8bda7bb5156a14c0d9e0c0399ba5ef2cf6c8bfc40a1624`) with a dated README recording canonical task/wiki ownership, coordinator-authored synthetic provenance, generation, meaning and limitations. The project-history timeline and native experiment wiki link this checkpoint. Raw PPM and duplicate validation captures remain ignored.