---
name: chronofall-source-control-submodules
description: Manage ChronoFall source control and submodules. Use for dirty-worktree triage, child implementation commits, coordinator gitlink updates, recursive checkout validation, pinned commit review, task-scoped commits, or preventing parent and child changes from being mixed.
---

# ChronoFall Source Control And Submodules

## Establish State

Inspect coordinator and relevant children:

```sh
git status --short
git submodule status
git -C royale status --short
git -C starfall status --short
```

Preserve existing work. Stop on mixed or ambiguous changes. Never reset, clean, discard, or absorb unrelated child edits.

## Commit Child Work

Use a task owned by that child. Follow its policy, update its PM/wiki through supported tooling, validate, and commit inside the child before touching the parent gitlink. The child commit subject begins with its local task ID.

Do not include coordinator source, another child, or a parent pointer in the child commit.

## Advance A Gitlink

Use a separate coordinator task after the child commit exists. Confirm:

- the submodule path is the declared linked project;
- its stable project ID matches the declaration;
- `HEAD` is the intended reviewed child commit;
- the child tree is clean;
- no sibling or parent source changed accidentally.

Stage only the intended gitlink and any coordinator task/wiki evidence owned by the pointer task. Commit with the parent task ID.

## Validate Checkout

Check `.gitmodules`, path hints, `git submodule status`, and the recorded gitlink. Validate a recursive checkout with the repository's documented command or a non-destructive status/sync check. Never treat a matching remote as identity proof.

Report child commit, parent commit, old/new gitlink, PM task ownership, and any uncommitted work separately.
