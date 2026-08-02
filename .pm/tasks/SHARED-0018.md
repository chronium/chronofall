---
id: SHARED-0018
title: Add a narrow reusable static-mesh rendering boundary
track: SHARED
milestone: M2
dependsOn:
- SHARED-0001
createdAt: 2026-08-02T16:19:37.5313310Z
modifiedAt: 2026-08-02T16:20:04.8233840Z
---

Add a focused coordinator-owned SDL GPU static-mesh presentation boundary for exact child-selected props and attachments.

Acceptance boundary:
- Reuse the established caller-owned SDL device, command buffer, render pass, target, camera, scheduling, resource-lifetime, source-consumption, and headless-isolation contracts.
- Define only the minimum immutable static geometry/material inputs, upload resources, and draw recording needed by one selected bow and bounded Draft 0 environment props.
- Keep the deterministic/non-native data boundary independent of SDL where practical and keep all GPU code client-only.
- Support only deliberately approved material inputs; do not assume engine-specific vegetation/wind shaders.
- Prove deterministic managed behavior and native macOS ARM64 rendering before completion.
- Do not add a scene graph, render graph, terrain, vegetation, streaming, model catalogue, general material system, child integration, or asset acquisition.