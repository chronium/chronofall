---
id: COORD-0012
title: Groom the shared editor UI foundation roadmap
track: COORD
milestone: M3
createdAt: 2026-08-05T07:28:09.7765510Z
modifiedAt: 2026-08-05T07:30:17.1699140Z
---

Record the approved ownership and dependency shape for Starfall's editor design-language work without implementing the native backend or child editor.

Acceptance criteria:
- Allocate one coordinator-owned shared task for the narrow caller-controlled SDL GPU ImGui/ImGuizmo backend.
- Make only completed SHARED-0016 a dependency; cite completed Royale and Starfall tasks as architectural evidence in notes rather than dependency edges.
- Record the shared/backend versus Starfall/application ownership boundary and platform-validation policy.
- Add a coordinator roadmap page for the future Starfall editor enabler and link it from shared-engine architecture.
- Do not mutate either child, implement native integration, add third-party source, change family source allowlists, advance gitlinks or activate the new feature task.
- Validate PM, family resolution, diffs and repository cleanliness, complete this grooming task, commit and stop before the Starfall grooming cycle.

## Notes

- 2026-08-05 - Completed the approved coordinator-only editor UI roadmap grooming cycle.
  - Allocated `SHARED-0024` in todo at high priority with only completed `SHARED-0016` as its dependency.
  - Recorded Royale EDITOR-001/002/003/005 and Starfall CLIENT-0020 as architectural evidence in task prose rather than PM dependency edges.
  - Added `pm://project/prj_E7QP3LUocfY7k3PYM-EQOlqc/wiki/roadmap/starfall-editor-ui-foundation` and linked the planned backend from shared-engine architecture.
  - Preserved the narrow ownership boundary: ChronoFall owns the caller-controlled native ImGui/ImGuizmo backend; Starfall will own its later executable, design language, interaction state, documents and surfaces.
  - Mutation receipts all targeted `prj_E7QP3LUocfY7k3PYM-EQOlqc` and only coordinator PM/wiki paths.
  - PM MCP validation and `pm doctor` passed; SHARED-0024 reread is dependency-ready with no missing, invalid or unavailable dependencies; family inspection returned three available/readable/write-trusted members and zero warnings.
  - `git diff --check` passed. Royale and Starfall worktrees and both gitlinks remain unchanged.
  - No source, third-party dependency, native artifact, family allowlist, child PM data or feature state changed. SHARED-0024 remains todo and was not activated.