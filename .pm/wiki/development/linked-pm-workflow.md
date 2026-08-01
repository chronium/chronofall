---
title: Linked Project PM Workflow
createdAt: 2026-08-01T05:44:07.0007420Z
modifiedAt: 2026-08-01T05:44:07.0007420Z
---

## Inspect before selection

Call `get_project`, then `list_linked_projects`. Review stable IDs, aliases, relationships, resolution state/source, readability, write trust, dirty/revision state, and every warning. Verify reciprocal declarations, committed project IDs, and path hints; run `pm doctor` in each project to be mutated.

Use `family: true` for family reads and never combine it with `project`. Use an explicit project selector for one project and every linked mutation. Preserve the owning project returned by PM and interpret local IDs/states/tracks/milestones only with that owner's configuration.

## Canonical references

Persist cross-project task and wiki identity only as stable PM URIs, for example:

- `pm://project/prj_E7QP3LUocfY7k3PYM-EQOlqc/task/SHARED-0001`
- `pm://project/prj__-jXLQgm6GuD2gCKZ_bTa1m-/task/RENDER-012`
- `pm://project/prj_E7QP3LUocfY7k3PYM-EQOlqc/wiki/architecture/project-family`

Aliases select; they are not persisted identity. Plain dependency IDs are local.

## Plan-mode selection

For family work, call `get_next_task(readyOnly: true, family: true)`. For owner-directed project work, use an explicit project selector. Inspect ownership, priority, `dependenciesReady`, waiting/missing/unavailable/invalid dependencies, and warnings. Re-read the exact task with its owner selector, inspect implementation context, produce one plan, and stop for approval.

Planning never edits files or PM, activates a task, grants trust, or swaps in an unrelated task.

## Linked writes

Linked writes require explicit selection, unique verified identity, local write trust, and an allowed operation. Never grant trust autonomously. Each operation affects one repository. Inspect the mutation receipt's project ID and changed paths immediately. There is no atomic family transaction.

If linked configuration selection is not exposed, use supported PM tooling from the owning repository. If a required capability still fails, report it instead of editing `.pm/` manually.

## Execution and completion

After plan approval, recheck the worktrees/task/dependencies, activate only the owning task, implement and validate only approved scope, update owning task notes/wiki, obtain required owner validation, complete and commit in the owner, then stop. Do not automatically select another task.