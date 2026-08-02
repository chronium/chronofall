---
id: COORD-0009
title: Groom coordinator enablers for Starfall Draft 0
track: COORD
milestone: M3
dependsOn:
- pm://project/prj_pkIpzx0fzFD4URjvqBuYrGZF/task/SF-0008
createdAt: 2026-08-02T16:18:51.1138360Z
modifiedAt: 2026-08-02T16:31:22.9357560Z
---

Represent only the coordinator-owned shared presentation and exact asset-acquisition work required by Starfall's approved Draft 0 roadmap.

Acceptance criteria:
- Create focused shared tasks for a narrow reusable static-render boundary, deterministic static-asset cooking, and one rendered socketed static bow proof.
- Create separate exact-selection acquisition/staging tasks for the archer, Ranger pieces, bow/arrows, zone assets, and evidence-gated monster inputs.
- Use canonical dependencies on the owning Starfall selection tasks so acquisitions remain blocked until selections complete.
- Leave monster acquisition dependent only on selection and stable-ID staging until selection evidence identifies a static, rigid, or skeletal path.
- Preserve SHARED-0007 and every existing consumer; make its later broad attachment work review and reuse the narrow bow proof.
- Record ownership, source-selection, provenance, generated-output, and dependency-gating rules in coordinator wiki documentation.
- Leave all feature tasks todo and do not activate or implement rendering, cooking, staging, source acquisition, asset conversion, or child integration.
- Mutate only coordinator PM/wiki data; do not change Starfall, Royale, source, assets, generated outputs, or gitlinks.
- Validate receipts, dependency integrity, family warnings, PM projects, repository diffs, and task states before completion.

## Notes

- 2026-08-02 16:22 UTC - ## Cycle 2 grooming evidence — 2026-08-02

  Created coordinator-owned Draft 0 enablers without implementing source, assets, cooking, rendering, staging, or child integration.

  Mutation receipts:
  - Every task, dependency, milestone, note, and wiki mutation identified coordinator project prj_E7QP3LUocfY7k3PYM-EQOlqc and changed only coordinator .pm paths.
  - Created M2 shared tasks SHARED-0018 through SHARED-0020.
  - Created M3 acquisition tasks ASSET-0004 through ASSET-0008.
  - Added SHARED-0020 as a prerequisite of broad SHARED-0007 and updated SHARED-0007 to require review/reuse of the narrow bow proof; all existing consumers remain unchanged.
  - Persisted canonical dependencies on Starfall CONTENT-0011, CONTENT-0012, and CONTENT-0013 using stable project prj_pkIpzx0fzFD4URjvqBuYrGZF.
  - ASSET-0008 depends only on completed SHARED-0017 and still-todo canonical CONTENT-0013; no static, rigid, or skeletal prerequisite was invented.
  - Created roadmap/starfall-draft-0-shared-enablers and updated the initial family roadmap, shared-character architecture, Quaternius provenance, and family-source staging documentation.

  Dependency readback:
  - SHARED-0018 is the only new dependency-ready feature task.
  - SHARED-0019 waits on SHARED-0018.
  - SHARED-0020 waits on SHARED-0018 and ASSET-0006.
  - Every acquisition is blocked by its owning still-todo Starfall selection; ASSET-0006 and ASSET-0007 also wait on SHARED-0019.
  - SHARED-0007 waits on SHARED-0004 and SHARED-0020.
  - No dependency is missing, invalid, unavailable, or cyclic.

  Validation:
  - Family inspection returned ChronoFall, Royale, and Starfall available, readable, write-trusted, and warning-free.
  - Family task readback found exactly COORD-0009 active and no dependency problems.
  - Coordinator MCP validation passed with zero issues.
  - pm doctor passed in ChronoFall, Starfall, and Royale.
  - git diff --check passed.
  - Starfall and Royale remain clean at pinned commits cced385420362371d6769c6cff784cf70af7bc21 and 174fa32600887da2093bcf7cbc9ebf89dc92990f.
  - No source, asset, generated output, child PM, or gitlink changed.

  Completion ends Cycle 2. The reviewed Starfall dependency wiring remains the separately bounded Cycle 3 continuation.
- 2026-08-02 16:31 UTC - ## Reviewed dependency continuation — 2026-08-02

  Reopened COORD-0009 for the three directly attributable Cycle 2 dependency findings.

  Corrections:
  - Added canonical prerequisite pm://project/prj_pkIpzx0fzFD4URjvqBuYrGZF/task/SF-0008 so Cycle 1 is the authoritative source of coordinator grooming.
  - Added completed SHARED-0017 to SHARED-0019 alongside SHARED-0018, making the static cook formally consume the fresh-checkout-safe stable-ID staging contract it intends to extend.
  - Replaced ASSET-0005's redundant direct SHARED-0002 prerequisite with SHARED-0004. The acquisition now waits for equipment slots/body hiding, which transitively waits for modular armour and skeletal cooking, while retaining SHARED-0017 and canonical Starfall CONTENT-0011.
  - Updated roadmap/starfall-draft-0-shared-enablers to match the corrected graph.

  Every mutation receipt identified coordinator project prj_E7QP3LUocfY7k3PYM-EQOlqc and changed only the expected coordinator PM/wiki paths. No feature task was activated and no source, asset, child PM, generated output, or gitlink changed.