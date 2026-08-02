---
id: SHARED-0017
title: Make client character staging fresh-checkout safe
track: SHARED
milestone: M2
dependsOn:
- SHARED-0016
createdAt: 2026-08-02T11:23:14.9343680Z
modifiedAt: 2026-08-02T11:26:17.9309010Z
---

Remove the hidden pre-restored-build prerequisite from the coordinator-owned stable-ID character-presentation staging workflow.

Acceptance criteria:
- The documented staging command works from a fresh canonical family checkout after normal submodule acquisition, without requiring the owner to restore the cooker manually first.
- The script performs an explicit restore for the focused ChronoFall.CharacterCooker project, then retains the existing --no-restore build and --no-build run phases.
- Stable-ID resolution, reciprocal linked identity, gitlink/path validation, ignored and untracked output protections, symlink checks, deterministic cook outputs, and the exact client-only output set remain unchanged.
- Focused policy tests cover the restore/build/run command sequence.
- The staging workflow is exercised successfully for Starfall and produces the established deterministic output hash.
- No child source, PM data, tracked generated content, gitlink, package/feed contract, asset format, or runtime integration changes are made.
- Coordinator PM, tests, diffs, submodule state, and family warnings are validated.

## Notes

- 2026-08-02 11:26 UTC - Implemented the fresh-checkout staging usability fix.

  - The stable-ID staging script now explicitly restores only ChronoFall.CharacterCooker before retaining the existing Release build with --no-restore and run with --no-restore --no-build.
  - Added a policy regression test that requires restore, build, and run in that order and preserves the no-restore/no-build phase boundaries.
  - Updated scripts/README.md to state that the advertised command needs no separate restore.
  - All stable-ID, reciprocal ownership, gitlink/path, ignore/tracked-file, symlink, output allowlist, provenance, and client-only boundaries remain unchanged.

  Validation:
  - Focused ChronoFall.CharacterPresentation.Tests passed 85/85.
  - Full ChronoFall.slnx Debug test run passed 149/149.
  - The documented Starfall staging command completed successfully and reproduced the 1,278,301-byte cooked SHA-256 37d2ecd2c614a4cc74fe359906c84408432100f0338b86d7ce4f4dddb6b585d3 and provenance SHA-256 bbe46b17fa0882e3ba5cdc46093a67df3224b6d5892aa5463ee6d386fce9d8c9.
  - Output remained exactly the established four ignored files.
  - Coordinator and Starfall pm doctor passed; family inspection returned three available/readable/write-trusted members with zero warnings.
  - git diff --check passed. Both child tracked worktrees are clean; gitlinks remain Royale 174fa32600887da2093bcf7cbc9ebf89dc92990f and Starfall 88d08591d626caa03ef0ad7d372e7b80d8b110ca.
  - No native or visual validation was required because runtime output and rendering were unchanged.