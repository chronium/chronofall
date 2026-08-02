---
id: SUBMODULE-0002
title: Advance Starfall after family source policy
track: SUBMODULE
milestone: M3
dependsOn:
- pm://project/prj_pkIpzx0fzFD4URjvqBuYrGZF/task/SF-0006
createdAt: 2026-08-01T05:47:15.1776140Z
modifiedAt: 2026-08-02T09:57:48.5417020Z
---

Advance the coordinator's Starfall gitlink from `0ba42c2a6a5da32726cced9672294b1ac975605e` to reviewed child commit `88d08591d626caa03ef0ad7d372e7b80d8b110ca` after the family-source policy is complete.

## Acceptance criteria

- Require canonical child prerequisite `pm://project/prj_pkIpzx0fzFD4URjvqBuYrGZF/task/SF-0006`; its local dependency retains the completed repository foundation.
- Verify the stable child identity, reciprocal project declaration, submodule path, clean child worktree, linear ancestry, and exact three-commit range `SF-0005`, `BUILD-0002`, and `SF-0006`.
- Stage only the Starfall gitlink plus this coordinator task's PM state and durable note. Do not modify Starfall, Royale, coordinator source/wiki, `SHARED-0016`, or another task.
- Validate coordinator PM, family warnings, gitlink/index/HEAD values, local recursive submodule status, `git diff --check`, and the complete staged file list.
- Record that Starfall is three commits ahead of `origin/main`; no push occurs in this cycle, and any later publication must push the child before the coordinator pointer commit.
- Commit as `[SUBMODULE-0002] Advance Starfall after family source policy` and stop.

## Notes

- 2026-08-02 09:57 UTC - Advanced the staged Starfall gitlink from `0ba42c2a6a5da32726cced9672294b1ac975605e` to reviewed child commit `88d08591d626caa03ef0ad7d372e7b80d8b110ca`. The linear descendant range contains exactly `c1ccf9173408c86463e7080c24cd69d2c923a362 [SF-0005]`, `475bfeb03d54a066eb64f21fe1e8d7e3d6f1f824 [BUILD-0002]`, and `88d08591d626caa03ef0ad7d372e7b80d8b110ca [SF-0006]`. Replaced the redundant direct BUILD-0002 prerequisite with canonical completed dependency `pm://project/prj_pkIpzx0fzFD4URjvqBuYrGZF/task/SF-0006`, whose local dependency retains BUILD-0002.

  Verified the reciprocal child declaration and stable project ID `prj_pkIpzx0fzFD4URjvqBuYrGZF`, the `starfall` submodule path, clean Starfall and Royale worktrees, linear ancestry, exact index mode `160000` and target hash, local recursive submodule status, and the complete submodule log. Linked-family inspection returned all three projects available/readable/trusted with zero warnings; coordinator PM doctor and `git diff --check` passed. The child commits already carry their owning PM/build/test evidence, so this pointer-only task did not rerun product builds. No Starfall, Royale, coordinator source/wiki, SHARED-0016, or unrelated task content changed.

  After a fresh fetch, Starfall `main` remains three commits ahead of `origin/main`. Nothing is pushed by this task. A later publication must push Starfall through `88d08591d626caa03ef0ad7d372e7b80d8b110ca` before pushing the coordinator pointer commit; remote recursive checkout must not be claimed until then.