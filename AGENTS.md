# Repository Instructions

## Operating Model

ChronoFall coordinates two independently useful game repositories:

- `royale`: the existing server-authoritative battle royale.
- `starfall`: the MU-inspired MMORPG.

The coordinator owns the family roadmap, cross-project decisions, parent initiatives, shared-technology experiments, proven shared modules, and pinned child commits. Each child owns its PM project, source history, product architecture, simulation, protocol, content, build, and release lifecycle.

Inspect the active PM project, linked family, worktrees, relevant wiki pages, source, tests, and supplied assets before proposing work. Ask the owner before changing a dependency, authority boundary, file format, protocol, renderer contract, physics behavior, platform policy, or product rule that is not already established.

## Architecture And Authority

- Both games are server-authoritative. Clients express intent and present authoritative events.
- Rendering, animation, IK, particles, audiovisual feedback, cameras, and presentation smoothing are client-owned. Animation never decides attacks, hits, casts, movement transitions, equipment changes, damage, or death.
- Headless server and simulation projects must not depend on SDL windowing, SDL GPU, ImGui, rendering, editor, or other graphical code.
- Parent-owned shared modules may be consumed by either child but must not depend on either child.
- The canonical full-client development environment is the shallow coordinator family checkout. Children may consume explicitly approved parent projects from source through the single `ChronoFallFamilyRoot` property; independent repository ownership does not require every full client build to work without the coordinator checkout.
- `royale` and `starfall` must never depend directly on one another.
- Do not extract code merely because it looks reusable. Promote only contracts demonstrated by both a focused experiment and a concrete child need.
- Do not build a general Unity-like engine, generic runtime component framework, retargeter, animation graph, or general asset framework without an approved task and explicit contract decision.

Authoring objects may later use typed components and registered tooling, but they must compile into compact game-specific runtime data. Authoring representation is not runtime simulation representation.

## PM And Wiki

Use PM MCP for ordinary task, state, metadata, dependency, ordering, milestone, priority, and wiki mutations. Never hand-edit `.pm/`. If linked configuration selection is not exposed, use supported PM tooling from the owning repository or report the missing capability.

At the start of linked work:

1. Call `get_project` for the active project.
2. Call `list_linked_projects`.
3. Review every warning and record stable project ID, alias, relationship, status, readability, local write trust, and resolution source.
4. Verify reciprocal declarations, `.pm/project_id.txt`, path hints, and intended checkout. A path or Git remote is not proof of identity.
5. Run `pm doctor` in every project that will be mutated.

Preserve the owning project returned by PM. Task IDs, states, milestones, tracks, and wiki paths are project-local. Use `family: true` only for family reads and never combine it with `project`. Use an explicit `project` selector for one-project reads and every linked mutation.

Persist cross-project identity only as:

```text
pm://project/<stable-project-id>/task/<task-id>
pm://project/<stable-project-id>/wiki/<wiki-path>
```

Plain dependency IDs are local to their owning project. Never persist aliases, display names, paths, Git URLs, or web URLs as PM identity.

Linked writes require unique resolution, matching stable identity, local write trust, and a supported mutation. Never grant trust autonomously. Inspect every linked mutation receipt: its `projectId`, changed paths, and repository must match exactly one intended child. One PM operation mutates at most one repository; there is no cross-repository transaction.

The PM wiki is the durable source of truth for architecture, ownership, formats, setup, experiments, provenance, and workflow. Update it in the owning project with the same task that changes a contract.

## Plan-Mode Approval Gate

When the owner enters Plan mode and asks for the next task:

1. Call `get_project` and `list_linked_projects`.
2. Resolve or report family warnings relevant to readiness.
3. Call `get_next_task(readyOnly: true, family: true)` for family-wide selection, or use an explicit `project` selector for owner-directed project work.
4. Preserve the returned owning project and inspect `dependenciesReady`, `waitingOnDependencies`, unavailable/missing/invalid dependencies, priority, and warnings.
5. Call `get_task` with both the local ID and owning project selector.
6. Read the complete task, relevant source, tests, assets, wiki, and nearby implementation.
7. Identify authority, dependency, file-format, asset, native, and submodule contracts.
8. Produce one concrete plan covering behavior, owning repository, files/projects, decisions, tests, native/visual validation, PM/wiki notes, commits, pointer implications, and owner questions.
9. Stop for approval.

Planning never edits implementation files or `.pm/`, activates a task, grants trust, plans unrelated tasks, or substitutes a different task merely because the selected task is blocked.

## Execution After Approval

Only after the owner approves the plan:

1. Recheck parent and relevant child worktrees.
2. Re-read the selected task from its owning project and confirm dependencies are still ready.
3. Confirm the exact PM mutation target and write authority.
4. Move only that task to the owning project's active state.
5. Implement only approved scope and follow the owning repository's `AGENTS.md` and skills.
6. Validate according to the plan.
7. Update durable task notes and relevant owning-project wiki pages.
8. Obtain explicit owner validation when visuals, animation, UI, controls, camera, audio, or gameplay feel are acceptance criteria.
9. Complete the task only after implementation, validation, documentation, and required owner validation.
10. Commit the focused change in the owning repository with the task ID.
11. For a child-owned task executed from the verified family checkout, complete the automatic pointer-only coordinator follow-up described below, then stop.

Do not automatically select or begin another task after completion. Every task starts with a new owner-directed Plan-mode pass.

## Git And Submodules

Check `git status --short` in the coordinator and every relevant child before work.

- Clean: proceed.
- One obvious coherent existing change: report and handle it deliberately.
- Mixed, surprising, or ambiguous changes: stop and ask.

Never discard, rewrite, absorb, or hide unrelated work. Keep parent source, child source, PM changes in different repositories, and gitlink changes in distinct commits.

Child source lifecycle:

1. Select, plan, approve, activate, implement, validate, document, complete, and commit the child task in the child.
2. Return to the coordinator.
3. Without creating or activating a coordinator PM task, verify the child stable ID, reciprocal declarations, path hint, tracked gitlink, clean child and sibling worktrees, expected child `HEAD`, and ancestry from the recorded pin.
4. Stage only that child's gitlink, validate the recursive checkout and complete staged diff, then create a pointer-only coordinator commit whose subject begins with the child task ID and whose body records the canonical child task URI, stable project ID, and pinned commit.
5. Stop. Pushing remains owner-directed; publish the child commit before the coordinator commit.

The pointer commit is a mechanical continuation of the approved child task, not a second PM task or implementation scope. If the coordinator contains unrelated changes, the child is dirty, identity or ancestry is wrong, or the linked project is unavailable, stop and report the blocker. Resume the same mechanical follow-up after resolution; do not create a ceremonial `SUBMODULE` task.

Task commits begin with the owning task ID, for example `[RENDER-012] Integrate shared character presentation`. The corresponding pointer-only commit may use `[RENDER-012] Pin Royale child commit` and persists the canonical child task URI in its body.

## Experiments, Shared Source, And Assets

Keep experimental source explicitly provisional and parent-owned. The skinned-character proof must establish the first real shared contracts before any shared-engine promotion.

Approved child source consumption remains narrow and client-only. Use `ChronoFallFamilyRoot`; never scatter literal parent traversal, absolute checkout paths, arbitrary external roots, or child-to-child references. SDL3-CS remains coordinator-pinned source compiled transitively through the shared SDL GPU project. Do not replace this with packages or feeds without later task-owned evidence.

Generate selected client content only through the coordinator workflow for the consuming child's stable project ID. The workflow must verify reciprocal identity, resolved checkout and gitlink, then refuse to write unless the exact owned output tree is ignored, untracked, and free of symlink escapes or unrelated files. Generated cooks and provenance remain outside source control and never enter server or simulation outputs.

Treat files under `assets/Quaternius/` as authoritative. Do not download substitutes or process the entire collection. Preserve Quaternius CC0 provenance and select only one humanoid, one skeleton, one idle, one locomotion clip, and one compatible attack when evidence supports it.

Loader selection is a contract decision. Inspect the actual formats and Royale's SimpleMesh capability first. If rigs are incompatible, record evidence and plan the smallest resolving experiment; do not invent retargeting.

## Validation

Run `pm doctor` or equivalent PM validation for every mutated PM project. Inspect mutation receipts, task readiness, `git diff --check`, repository diffs, submodule status, and staged changes.

Select build/test/native validation by owning repository and risk. Documentation-only work does not require a full game test run. Rendering and animation require deterministic transform/sampling tests, supported GPU/native execution, captured evidence, and explicit owner visual confirmation. Server outputs must remain free of client graphics dependencies and assets.

## Visual Checkpoints And Project History

Actively look for a project-history candidate whenever work produces meaningful visual output. Good candidates include a first working capability, milestone closure, a major before/after, an explanatory architecture or debug view, or an image that clearly captures a project transition. Routine regression screenshots, near-duplicates, noisy debug output, and raw capture dumps are not candidates.

Before completing or handing off visually meaningful work, show the strongest candidate to the owner and explicitly ask whether to:

1. preserve it as-is;
2. revise framing, camera, crop, overlays, labels, timestamps, or contact-sheet composition first; or
3. skip preservation.

Do not assume visual acceptance also authorizes permanent retention, and do not commit screenshots automatically. Keep raw captures and intermediate conversions ignored. Once the owner approves a coordinator artifact, preserve only the curated derivative under `docs/project-history/<YYYY-MM-DD>-<slug>/` with a dated index entry, canonical PM ownership, provenance and licence evidence, generation notes, and a content hash.

Preserve a child-owned milestone in the child repository that produced it. When Royale or Starfall work yields a candidate, notify the owner but route any permanent child policy, PM, documentation, and commit through a child-owned task. Do not silently copy a child artifact into the coordinator or advance a gitlink. A family-level coordinator artifact requires an explicit owner choice and must link back to its child-owned evidence.

The repository-relative history layout is intended to feed a future PM wiki image/timeline feature. Do not implement or assume that capability until its own approved task exists.

The durable workflow is documented at `pm://project/prj_E7QP3LUocfY7k3PYM-EQOlqc/wiki/development/visual-checkpoints`.

## Skill Routing

Load the smallest relevant set:

- `chronofall-pm-workflow`: linked family inspection, selection, dependencies, trust, receipts, task lifecycle, and wiki.
- `chronofall-source-control-submodules`: dirty trees, child commits, gitlinks, recursive checkout, and commit scope.
- `chronofall-architecture-boundaries`: parent/child ownership, dependency direction, authority, shared promotion, and authoring/runtime boundaries.
- `chronofall-build-validation`: PM validation, coordinator checks, child build/test, native validation, and evidence.
- `chronofall-character-rendering-animation`: skeletal proof, GPU skinning, animation, IK, attachments, and visual gates.
- `chronofall-asset-pipeline-provenance`: supplied-asset inspection, licenses, formats, cooking, audience separation, and selection.
- `chronofall-review`: cross-project review for identity, authority, dependencies, native risk, validation, PM/wiki, and commit boundaries.

When working inside a child, also load that child's own policy and relevant skills. Coordinator guidance does not replace child guidance.
