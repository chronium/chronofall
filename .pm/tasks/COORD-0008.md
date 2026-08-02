---
id: COORD-0008
title: Streamline automatic child gitlink handoff
track: COORD
milestone: M0
dependsOn:
- COORD-0003
createdAt: 2026-08-02T11:43:22.1354030Z
modifiedAt: 2026-08-02T11:46:16.3608120Z
---

Replace separate coordinator PM tasks for mechanical child gitlink advancement with an automatic pointer-only coordinator commit in the same owner-approved child execution cycle.

Acceptance criteria:
- The completed child task remains the sole PM owner of its implementation and pointer handoff.
- Child implementation and coordinator gitlink advancement remain two focused Git commits in their owning repositories.
- After the child task is completed and committed, the coordinator automatically verifies stable identity, reciprocal declarations, path hint/gitlink ownership, clean child and sibling worktrees, expected child HEAD and ancestry, then creates a pointer-only commit without a coordinator PM task.
- The pointer commit subject uses the child task ID and its body persists the canonical child task URI, stable child project ID, and pinned commit.
- Unexpected coordinator changes, identity/ancestry mismatches, dirty children, or unavailable linked projects stop the automatic follow-up without creating a ceremonial backlog task.
- Pushing remains owner-directed and ordered child first, coordinator second.
- Coordinator AGENTS, source-control skill metadata, and relevant wiki pages encode the new workflow.
- Obsolete todo SUBMODULE-0001 and SUBMODULE-0003 are removed through PM; completed pointer tasks remain historical evidence.
- The policy task commit excludes gitlinks. After it is committed, the new workflow is applied mechanically to completed Starfall BUILD-0005 at commit 5092c53.
- PM validation, family warnings, diffs, staged scope, and submodule state are verified.

## Notes

- 2026-08-02 11:46 UTC - Implemented the owner-approved automatic child gitlink handoff policy.

  - Updated coordinator AGENTS, linked PM completion guidance, the source-control/submodule skill, and its interface prompt.
  - Preserved two focused Git commits while making the completed child task the sole PM owner. The pointer-only coordinator commit now happens automatically in the same approved cycle with canonical child task URI, stable project ID, and pinned commit recorded in its body.
  - Added fail-closed checks for family identity, reciprocal declarations, path hint/gitlink ownership, expected HEAD and ancestry, clean child/sibling trees, and unrelated coordinator changes.
  - Kept pushing owner-directed and ordered child before coordinator.
  - Updated architecture/project-family, roadmap/initial-family-roadmap, development/family-source-consumption, architecture/shared-character-presentation, and experiments/skinned-character-proof-findings.
  - Removed obsolete todo SUBMODULE-0001 and SUBMODULE-0003 after confirming they had no dependents. Completed SUBMODULE tasks remain historical evidence.
  - No child repository, coordinator source/runtime, dependency, asset, or gitlink was included in this policy task commit.

  Validation:
  - Every PM mutation receipt targeted coordinator project prj_E7QP3LUocfY7k3PYM-EQOlqc and only expected .pm paths.
  - Coordinator pm doctor and git diff --check passed.
  - Family inspection returned all three projects available, readable, and write-trusted with zero warnings.
  - SUBMODULE track now contains only four completed historical tasks and no todo task.
  - Skill frontmatter and agents/openai.yaml parsed successfully with system Ruby; name, description, interface fields, and routed default prompt are valid. The supplied quick_validate.py could not start because host Python lacks PyYAML; no dependency was installed.
  - Starfall is clean at completed BUILD-0005 commit 5092c53f14238cdbb06b6fe348e511e31f3b8ddc; Royale is clean at 174fa32600887da2093bcf7cbc9ebf89dc92990f.
  - The policy commit will deliberately exclude the currently advanced Starfall working-tree gitlink. After this task commit, the new mechanical follow-up will record that pointer separately.