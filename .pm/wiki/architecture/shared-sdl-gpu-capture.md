---
title: Shared SDL GPU Capture
createdAt: 2026-08-03T15:40:07.3544860Z
modifiedAt: 2026-08-03T15:45:54.3195890Z
---

## Decision

`ChronoFall.CharacterPresentation.SdlGpu` owns a bounded client/tooling screenshot boundary because both the coordinator experiment and Royale have demonstrated the same SDL GPU readback lifecycle and Starfall has a concrete deterministic-capture need. The capability remains inside the already-approved shared SDL GPU assembly; it is not a new renderer, window host, scene API, capture application or general image framework.

The assembly remains coordinator-owned and child-independent. Starfall and Royale may consume it only through their own approved child tasks. The coordinator never references child source.

## Contract

A caller retains ownership of SDL initialization, its window and GPU device, textures and render targets, cameras, render-pass recording, presentation state and scheduling. After recording a completed frame into a caller-owned command buffer, the caller may explicitly pass that command and an existing 8-bit RGBA or BGRA texture to `SdlGpuTextureReadback.Submit`.

Submission transfers the command buffer to the helper. The helper records one download copy, submits once with a fence, and returns an owned `SdlGpuReadbackRequest`. The request may be polled or waited, maps only after the fence is resolved, normalizes supported RGBA/BGRA UNORM and sRGB byte orders into an owned tightly packed `RgbaImage`, and releases its fence and transfer buffer exactly once. Invalid dimensions, malformed buffers, unsupported formats and SDL failures remain explicit.

`PngImageWriter` encodes only validated RGBA images through the centrally pinned client/tooling dependency `StbImageWriteSharp` 1.16.7. Its official project is `https://github.com/StbSharp/StbImageWriteSharp`; the upstream README states Public Domain, and the exact package hashes and licence evidence are preserved under `thirdparty/licenses/StbImageWriteSharp/`. File output requires a `.png` path and uses a temporary sibling plus replacement. The dependency does not enter the BCL-only character data/cooking assemblies or any child headless output.

## Proven evidence

The existing coordinator bind-pose and static-mesh offscreen paths consume the promoted helper through `ChronoFall.CharacterExperiment.SdlGpu`. The established macOS ARM64 Metal harness remains the native validation host; no second harness or application is introduced. Existing fingerprint assertions ensure promotion does not change rendered results.

Managed contract tests cover dimensions, buffer ownership and length, both RGBA and BGRA UNORM/sRGB normalization, unsupported formats, deterministic PNG output and atomic replacement.

The development-only AppKit compositor at `scripts/create-contact-sheet.swift` consumes completed PNG files only. Its canvas is an explicit pixel bitmap, so macOS backing scale cannot add hidden Retina padding. AppKit does not enter any .NET runtime or shared assembly. Raw captures and temporary sheets remain ignored unless the owner separately approves one curated project-history artifact.

### Promotion validation

Debug and Release each build with zero warnings and errors and pass all 200 coordinator solution tests. The opt-in macOS ARM64 Metal suite passes all 37 tests in both configurations, retaining the established bind-pose, animation, blending, IK/Aim and static-mesh fingerprints through the promoted readback helper. Focused formatting verification passes for every changed .NET project.

The AppKit compositor was exercised against seven equal 768 by 443 PNG inputs in four columns. It produced the exact 3,072 by 982 pixel contract: four source widths and two rows of one source height plus the 48-pixel label strip. This proves the compositor adds no hidden Retina backing scale. The old OS-window captures still contain their own black lower area; they remain rejected temporary evidence and are not committed.

Coordinator PM validation, family resolution, repository diff checks and submodule checks pass. Royale and Starfall remain unchanged.

## Consumers and exclusions

Starfall task `pm://project/prj_pkIpzx0fzFD4URjvqBuYrGZF/task/CLIENT-0024` owns deterministic Draft 0 F1-F7 capture integration through the established family source boundary. It must reuse Starfall's own render path and retain Starfall ownership of its camera presets, scene composition and capture timing.

Royale already contains the evidence source for this boundary, but migration back to the shared helper is intentionally deferred to a later Royale-owned task. No Royale source changes are part of this promotion.

This task does not add asynchronous thumbnail queues, image decoding, window capture, OS automation, editor catalogues, scene ownership, runtime manifests, video capture or project-history retention policy.