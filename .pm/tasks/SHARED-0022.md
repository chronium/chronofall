---
id: SHARED-0022
title: Promote bounded SDL GPU screenshot capture
track: SHARED
milestone: M3
dependsOn:
- SHARED-0016
- EXPERIMENT-0009
- pm://project/prj__-jXLQgm6GuD2gCKZ_bTa1m-/task/EDITOR-002
createdAt: 2026-08-03T15:16:09.8324270Z
modifiedAt: 2026-08-03T15:46:06.7728050Z
---

Promote the already-proven SDL GPU screenshot boundary into the existing coordinator-owned shared SDL GPU presentation module for family clients.

Acceptance criteria:
- Add a narrow screenshot capability to ChronoFall.CharacterPresentation.SdlGpu, which already owns the approved shared SDL GPU resource operations; do not create another shared assembly.
- Continue compiling the coordinator-pinned SDL3-CS source and never depend on Royale or Starfall.
- Preserve caller ownership of windows, GPU devices, command acquisition, render passes, render scheduling and gameplay presentation. The caller explicitly supplies a completed render command and chooses when the capture helper resolves it through submission.
- Support deterministic one-shot readback of an existing RGBA8 or BGRA8 GPU texture through correctly owned download transfer buffers and fences.
- Normalize supported RGBA/BGRA formats into a tightly packed RGBA image and fail explicitly for unsupported formats, invalid dimensions and malformed pixel buffers.
- Encode exact RGBA images as PNG through an explicitly pinned, client/tooling-only dependency; do not add scene, editor, asynchronous thumbnail or general image-framework scope.
- Derive the contract from completed ChronoFall experiment capture evidence and Royale's proven screenshot implementation without adding a coordinator-to-child source dependency.
- Add deterministic managed tests for format normalization, validation and PNG output.
- Route the existing coordinator experiment readback through the promoted helper and validate it through the existing macOS ARM64 GPU harness; do not create another native harness.
- Correct the existing development-only contact-sheet compositor's pixel/backing-scale handling so equal-sized 16:9 captures tile without hidden Retina padding. Keep AppKit out of shared/runtime assemblies and raw captures outside source control.
- Document the shared ownership boundary, pending Starfall consumer and deferred Royale adoption.

## Notes

- 2026-08-03 15:46 UTC - Implemented the bounded screenshot boundary inside the existing ChronoFall.CharacterPresentation.SdlGpu assembly. Added owned RGBA normalization for RGBA8/BGRA8 UNORM and sRGB inputs, fence-backed SDL GPU texture readback, deterministic PNG encoding/writing, and exact failure validation. The existing bind-pose and static-mesh experiment paths now consume the shared helper; no new renderer application, assembly or native harness was created.

  Pinned StbImageWriteSharp 1.16.7 only in the client/tooling SDL GPU project. Preserved official-source, Public Domain licence statement and exact NuGet/NuSpec SHA-256 evidence under thirdparty/licenses/StbImageWriteSharp/. Updated family-source policy tests to continue requiring checked-out SDL3-CS while permitting exactly this approved package.

  Managed validation: Debug and Release each built with 0 warnings/errors and passed all 200 solution tests. Focused SDL GPU presentation tests passed 21/21. Opt-in native macOS ARM64 Metal validation passed 37/37 in both Debug and Release with established fingerprints retained. Focused dotnet format verification passed.

  Reworked scripts/create-contact-sheet.swift to render through an explicit NSBitmapImageRep pixel canvas. Seven 768x443 inputs in four columns produced the exact 3072x982 output, proving no hidden Retina backing-scale padding. The previous OS-window captures contain their own black lower area and remain rejected temporary evidence; no visual-history artifact was committed.

  Created architecture/shared-sdl-gpu-capture and reconciled related shared-presentation/experiment pages. PM doctor, linked-family inspection (0 warnings), git diff --check, submodule status and child worktree checks passed. Royale and Starfall sources, PM data and gitlinks were unchanged. Starfall CLIENT-0024 remains the separately owned integration consumer; Royale adoption remains deferred.