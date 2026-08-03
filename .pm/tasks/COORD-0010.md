---
id: COORD-0010
title: Groom shared Box3D prerequisite for Starfall walking
track: COORD
milestone: M3
dependsOn:
- pm://project/prj_pkIpzx0fzFD4URjvqBuYrGZF/task/SF-0009
createdAt: 2026-08-03T08:12:46.7111100Z
modifiedAt: 2026-08-03T08:47:47.4101010Z
---

Groom the coordinator-owned Box3D prerequisite required by Starfall's approved walking-slice roadmap without implementing physics or mutating a child.

Acceptance criteria:
- Depend canonically on completed Starfall grooming task pm://project/prj_pkIpzx0fzFD4URjvqBuYrGZF/task/SF-0009.
- Confirm no existing coordinator task owns the exact shared Box3D acquisition and managed integration boundary.
- Use completed Royale task pm://project/prj__-jXLQgm6GuD2gCKZ_bTa1m-/task/PHYS-012 as implementation evidence, not as permission to copy Royale-owned source or mutate Royale.
- Allocate one focused shared task for coordinator-owned Box3D pin/fetch/licence/native-build ownership plus the minimum child-independent managed binding and ownership surface required by authoritative ground-plane movement.
- Preserve finite single-precision metre state, fixed-tick calling policy, stable identity/order responsibilities, headless isolation, parent-to-child dependency direction, and the ChronoFallFamilyRoot source-consumption model.
- Keep the shared implementation task todo and record the separately approved Cycle 3 continuation that will attach its canonical URI to Starfall SIM-0008.
- Update only coordinator task/wiki roadmap and architecture documentation.
- Do not change Box3D source, pins, patches, builds, managed projects, Starfall or Royale PM/source, gitlinks, or feature states.

## Notes

- 2026-08-03 08:15 UTC - Implemented the approved coordinator-only Cycle 2 grooming pass.

  - Authoritative readback confirmed no existing coordinator task owned the exact shared Box3D acquisition and managed runtime boundary.
  - Allocated SHARED-0021, Establish a bounded shared Box3D runtime boundary, in coordinator M3 and left it todo.
  - SHARED-0021 depends on completed SHARED-0016 plus canonical Royale evidence pm://project/prj__-jXLQgm6GuD2gCKZ_bTa1m-/task/PHYS-012; reread reports both dependencies complete.
  - Bounded the future implementation to coordinator-owned pin/licence/fetch/native builds, a minimal child-independent headless binding/ownership surface for ground-plane movement, and explicit ChronoFallFamilyRoot consumption.
  - Excluded generic physics abstractions, debug rendering, gameplay, map formats, collision cooking, packages/feeds, child migration and child mutation.
  - Updated architecture/shared-engine-boundaries, development/family-source-consumption and roadmap/starfall-draft-0-shared-enablers.
  - Recorded that Cycle 3 must reopen Starfall SF-0009 solely to attach SHARED-0021 canonically to SIM-0008 and correct matching Starfall prose.

  Mutation receipts:
  - COORD-0010 creation/state/metadata changed only coordinator task and state paths.
  - SHARED-0021 creation/dependency metadata changed only coordinator task and todo-state paths.
  - Each wiki mutation identified coordinator project prj_E7QP3LUocfY7k3PYM-EQOlqc and exactly its intended page.

  Validation:
  - Coordinator and family reads report 56 and 365 tasks respectively.
  - No missing, invalid or unavailable dependencies and no local or family cycles.
  - Exactly COORD-0010 was active during grooming; SHARED-0021 and every feature task remained todo.
  - The linked family has three available/readable/write-trusted members and zero warnings.
  - Coordinator pm doctor and git diff --check passed.
  - Starfall and Royale worktrees are clean; both gitlinks are unchanged.
  - No source, third-party files, pins, patches, builds, assets, generated content, child PM/wiki, or feature implementation changed.
  - No build, native or visual validation was required for this task/wiki-only grooming cycle.
- 2026-08-03 08:28 UTC - Reopened as a directly attributable Cycle 2 documentation review continuation.

  - Corrected architecture/shared-engine-boundaries to distinguish canonical dependency wiring from implementation readiness: Cycle 3 attaches allocated SHARED-0021 to Starfall SIM-0008, producing a valid waiting dependency; SIM-0008 cannot activate or consume shared Box3D source until SHARED-0021 completes.
  - Reframed roadmap/starfall-draft-0-shared-enablers so the completed SF-0008 Cycle 3 is explicitly historical asset-enabler wiring under COORD-0009, distinct from the pending Box3D SF-0009 Cycle 3.
  - Qualified the historical no-further-wiring statement to its original asset-enabler scope and preserved the pending Box3D continuation.
  - Mutation receipts identified coordinator project prj_E7QP3LUocfY7k3PYM-EQOlqc and only the two intended wiki paths.
  - No tasks, dependencies, source, assets, children, gitlinks or feature states changed beyond the required COORD-0010 reopen/complete lifecycle.

  Validation:
  - Coordinator pm doctor and git diff --check passed.
  - Family readback reports 365 tasks with no missing, invalid or unavailable dependencies and zero warnings.
  - COORD-0010 was the only active task during correction; SHARED-0021 remained dependency-ready and todo with unchanged dependencies.
  - Starfall and Royale worktrees and both gitlinks remained unchanged.
  - No build, native or visual validation was required for this documentation-only correction.
- 2026-08-03 08:47 UTC - Reopened as a directly attributable Cycle 3 documentation review continuation.

  - Updated the coordinator Box3D roadmap and source-boundary prose to record that Starfall Cycle 3 completed through pm://project/prj_pkIpzx0fzFD4URjvqBuYrGZF/task/SF-0009.
  - Recorded Starfall commit 84b1c94d3d3413954a20b09ea1d0445dfeb748f7 and coordinator pointer commit 7530f552044b5888d44df7123c66996612c4655e.
  - Replaced future dependency-wiring language with the completed state: SIM-0008 now has a valid-but-waiting canonical dependency on SHARED-0021.
  - Preserved the implementation gate: source consumption and SIM-0008 activation remain blocked until SHARED-0021 completes and SIM-0008 receives its own approved implementation plan.
  - Mutation receipts identified coordinator project prj_E7QP3LUocfY7k3PYM-EQOlqc and only architecture/shared-engine-boundaries, development/family-source-consumption, and roadmap/starfall-draft-0-shared-enablers.
  - No tasks, dependencies, source, assets, children, gitlinks, or feature states changed beyond the required COORD-0010 reopen/complete lifecycle.

  Validation:
  - Family readback reports 365 tasks with no missing, invalid, or unavailable dependencies and no cycles.
  - Exactly COORD-0010 was active during the correction; SHARED-0021 and SIM-0008 remain todo.
  - SIM-0008 resolves SHARED-0021 as a valid waiting dependency, and linked-family inspection reports no warnings.
  - Coordinator pm doctor and git diff --check passed.
  - Starfall and Royale worktrees and both gitlinks remained unchanged.
  - No build, native, or visual validation was required for this coordinator PM/wiki-only correction.