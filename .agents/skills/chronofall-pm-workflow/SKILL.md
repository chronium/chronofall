---
name: chronofall-pm-workflow
description: Coordinate ChronoFall PM tasks and wiki across the parent, Royale, and Starfall through PM MCP. Use for linked-family inspection, Plan-mode task selection, project selectors, family reads, canonical cross-project dependencies, write trust, mutation receipts, linked task or wiki mutations, state transitions, completion, and PM validation.
---

# ChronoFall PM Workflow

## Inspect The Family

1. Call `get_project` for the active project.
2. Call `list_linked_projects` and inspect every member and structured warning.
3. Verify stable IDs against reciprocal `linked_projects.yaml`, each `.pm/project_id.txt`, submodule path hints, and the intended checkout.
4. Treat unavailable or mismatched children as unresolved; do not fabricate identity or edit storage.
5. Run `pm doctor` in projects that will be mutated.

Do not infer identity from paths, filesystem proximity, display names, or Git remotes.

## Read With Ownership

- Use `family: true` for cross-family task/wiki search or selection. Never combine `family` and `project`.
- Use `project: current`, `parent`, an exact stable ID, or a unique alias for one-project reads.
- Preserve `project.projectId`, alias, relationship, revision, and dirty state returned with every result.
- Interpret IDs, tracks, states, milestones, and wiki paths only with the owning project's configuration.

## Select And Plan One Task

In Plan mode, call `get_next_task(readyOnly: true, family: true)` unless the owner directs one project or task. Inspect readiness, completed/waiting/missing/unavailable/invalid dependencies, priority source, ownership, and warnings. Then call `get_task` with the local ID and owning selector and read it completely.

Planning does not mutate PM, activate work, grant trust, or choose another task merely because the recommendation is blocked. Return one implementation plan and wait for approval.

## Write Canonical Relationships

Use local IDs only for same-project dependencies. Persist cross-project tasks and wiki pages as:

```text
pm://project/<stable-project-id>/task/<task-id>
pm://project/<stable-project-id>/wiki/<wiki-path>
```

Aliases are convenient selectors, not persisted identity. A linked dependency is complete only when the canonical task is readable and in its owning project's configured completed state.

## Mutate One Repository

Every linked mutation requires an explicit `project`, unique verified identity, local write trust, and a supported operation. Never call `pm project trust` autonomously. If trust is missing, report the stable ID, alias, and intended mutation, then stop that path.

After every mutation, verify the receipt's `projectId` and `changedPaths` belong to exactly one intended repository. There is no atomic family transaction. If linked configuration selection is unavailable, run supported PM tooling from the owning child context or report the limitation; never hand-edit `.pm/`.

## Execute And Complete

After plan approval, re-read the task, recheck dependencies, move only it to the owning active state, implement, validate, update durable notes/wiki, and complete only when no required work remains. Completion ends the cycle: commit in the owning repository and stop without selecting another task.
