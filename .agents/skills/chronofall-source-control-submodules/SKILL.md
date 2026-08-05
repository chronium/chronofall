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

After the child task is complete and committed, return to the coordinator in the same approved cycle. Do not create, select, activate, or complete a coordinator PM task for a mechanical pointer advance. Confirm:

- the submodule path is the declared linked project;
- its stable project ID and reciprocal parent declaration match;
- its committed path hint and tracked gitlink identify the checkout;
- `HEAD` is the intended reviewed child commit;
- the child commit descends from the recorded pin;
- the child tree is clean;
- every sibling tree is clean;
- the coordinator has no unrelated staged or unstaged changes.

Stage only the intended gitlink and inspect the complete staged submodule diff. Commit with the child task ID in the subject. Persist the canonical child task URI, stable child project ID, and pinned commit in the commit body. Do not mutate coordinator PM or wiki data for the pointer.

If any check fails, stop and report the blocker. Resume this mechanical follow-up after resolution without creating a `SUBMODULE` task. Pushing remains owner-directed and must publish the child before the coordinator.

For an explicitly owner-approved taskless child backlog-grooming commit, use a `[PM]` pointer subject instead of inventing a task ID. Record the stable child project ID, pinned commit, and concise grooming purpose in the body; do not fabricate a canonical task URI. Preserve every other pointer-only identity, ancestry, cleanliness, validation, and publish-order rule.

When an owner-approved child grooming task is reopened solely for reviewed canonical dependency wiring after the planned coordinator cycle, its second child commit and pointer-only handoff use the original grooming task ID. Keep that continuation limited to recorded dependency receipts and matching roadmap corrections; never absorb feature implementation or unrelated grooming.

## Validate Checkout

Check `.gitmodules`, path hints, `git submodule status`, and the recorded gitlink. Validate a recursive checkout with the repository's documented command or a non-destructive status/sync check. Never treat a matching remote as identity proof.

Report child commit, pointer commit, old/new gitlink, canonical child PM ownership, and any uncommitted work separately.
