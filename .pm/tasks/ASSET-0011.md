---
id: ASSET-0011
title: Document native Blender asset evaluation workflow
track: ASSET
milestone: M5
createdAt: 2026-08-06T17:50:37.5772830Z
modifiedAt: 2026-08-06T17:52:29.7286540Z
---

Turn the successful ASSET-0006 scale review into a reusable coordinator workflow for focused native asset evaluation.

Acceptance boundary:
- Document how to verify Blender availability through the connected Blender MCP before a review.
- If Blender is unavailable, stop the native-review path and ask the owner to open Blender with the MCP add-on enabled; do not launch or install Blender autonomously.
- Inspect the current Blender scene before changing it and preserve existing user work.
- Build only an unsaved, temporary comparison scene from exact task-selected source assets, explicit scale and simple dimensional references.
- Record measured dimensions, source identity, transforms and the exact question requiring owner confirmation.
- Never save over supplied source files, export replacements, mutate source assets or treat a visual comparison as socket, gameplay or final-art proof.
- After owner confirmation, record durable evidence and leave preservation of screenshots/history artifacts as a separate explicit choice.
- Add repository-local asset-skill routing so future agents follow this workflow.

## Notes

- 2026-08-06 17:52 UTC - Completion evidence: created the durable assets/native-blender-asset-evaluation wiki workflow; linked the ASSET-0006 bow/arrow evidence; updated the repository-local asset-provenance skill with the Blender MCP availability gate, current-session preservation, unsaved comparison-scene procedure, metric measurement, owner decision, proof exclusions, and screenshot-preservation boundary. PM validation and linked-family inspection passed with zero structured warnings; git diff --check passed; Starfall and Royale remained clean and their gitlinks unchanged. Documentation-only scope required no product build or native launch.