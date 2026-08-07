# ChronoFall and Starfall Trigger/Deliverable Migration Execution Manifest

Date: 2026-08-07

Status: owner-review approval surface; no PM, wiki, task, milestone, trigger, delivery, dependency, source, gitlink, commit, or push mutation is authorized by this document alone

This manifest, the [migration audit](2026-08-07-chronofall-starfall-trigger-deliverable-migration-audit.md), and the [companion log](2026-08-07-chronofall-starfall-trigger-deliverable-migration-log.md) are one work entity. The audit owns the design rationale and exact milestone descriptions. This manifest owns execution order, mutation scope, validation gates, and commit boundaries.

## Execution model

One owner approval launches one migration. The migration does not stop for another Plan-mode pass between the coordinator and Starfall stages.

It produces exactly three focused commits:

1. coordinator milestone descriptions, historical deliveries, and matching coordinator wiki;
2. Starfall milestone descriptions, historical deliveries, empty-M3 removal, triggers, attachments, six dependency removals, seam wording, and matching Starfall wiki;
3. pointer-only coordinator handoff pinning the reviewed Starfall commit.

Internal readback gates provide safety. A failed gate stops the launched migration as blocked work; it does not authorize improvisation, direct `.pm/` edits, an exceptional milestone delivery, trigger override, unrelated grooming, or a different task.

No task is activated. No source, asset, generated content, Royale file, or implementation contract changes.

## Fresh authoritative snapshot

The manifest was generated from readback on 2026-08-07:

| Project | Stable ID | Revision | Tasks | Done / todo / active | Edges | Resolution |
| --- | --- | --- | ---: | --- | ---: | --- |
| ChronoFall | `prj_E7QP3LUocfY7k3PYM-EQOlqc` | `31a56cb944ae56e9a15f6cb3aca513d44020bac6` | 69 | 56 / 13 / 0 | 85 | current, readable, write-trusted |
| Starfall | `prj_pkIpzx0fzFD4URjvqBuYrGZF` | `36d6f8fd0d08869486cc017e614f20ecefc9e77b` | 118 | 59 / 59 / 0 | 283 | child, readable, write-trusted |
| Royale | `prj__-jXLQgm6GuD2gCKZ_bTa1m-` | `3b1bc45e4c8be76d110d8cf9613284db342db42e` | read-only | not mutated | not mutated | child, readable, write-trusted |

Family inspection returned zero warnings. Coordinator and Starfall have no activation triggers or delivery records. Coordinator's six milestones are ready to deliver. Starfall validation passes with only `empty_milestone` for M3.

The seven audited Starfall edges remain exactly present. Six are removal candidates; `CLIENT-0019 -> CLIENT-0031` is the deliberate overlap edge.

This snapshot is a precondition, not a permanent readiness claim. Refresh it before the first mutation.

## Linked-project execution path

The live PM MCP exposes an explicit `project` selector for switchboard reads and every milestone/trigger operation required by this manifest: milestone-description mutation, delivery preview and delivery, milestone removal, trigger creation and reconciliation, and trigger attachment.

The entire approved migration can therefore run from the coordinator MCP context:

- use `project: current` for coordinator reads and mutations;
- use `project: starfall` or the exact stable ID `prj_pkIpzx0fzFD4URjvqBuYrGZF` for every Starfall read and mutation;
- never omit the selector on a linked-project write;
- verify the owning `project` metadata on every linked read;
- verify every mutation receipt identifies the expected stable project, changed paths, and repository.

Live readback proved that `get_activation_switchboard(project: starfall)` resolves the exact Starfall stable ID and revision, reports a clean project, returns all eight milestones and zero triggers, and produces no linked-project warnings. The only returned validation issue is the already-recorded empty M3 warning.

If selector resolution, ownership metadata, write trust, or receipt ownership differs during execution, stop. Never work around a linked-operation failure by editing `.pm/` manually or by relying on filesystem proximity.

## Representation rule

Milestone triggers represent capability prerequisites between milestones. A delivered milestone guarantees its capability and required implementation seams. Task dependencies order work within a milestone or deliberately coordinate overlapping milestones.

For every cross-milestone task edge, ask:

> Could this downstream task become eligible before the prerequisite capability is guaranteed to exist?

- If the downstream milestone is inactive until the capability trigger is active, remove the task edge.
- If milestones intentionally overlap and one task must wait, retain the narrow edge.
- If the trigger does not guarantee the required capability, repair the trigger or retain the dependency.
- Never retain a dependency as documentation. Name consumed APIs, adapters, dispatchers, codecs, and presentation seams in task/wiki contracts.

Completed dependencies and all canonical cross-project dependencies remain historical/ownership evidence and are not rewritten.

## Preflight gate

Before mutation:

1. Read `get_project` and `list_linked_projects` from the coordinator context.
2. Confirm the three exact stable IDs, zero family warnings, and Starfall write trust.
3. Verify reciprocal declarations, `.pm/project_id.txt`, path hints, gitlinks, and the checked-out revisions.
4. Run `pm doctor` in coordinator and Starfall.
5. Confirm coordinator, Starfall, and Royale worktrees are clean and submodules match the recorded pins.
6. Re-read both switchboards with explicit `project: current` and `project: starfall` selectors, then re-read all tasks affected by dependency changes with their owning project selector.
7. Recompute task/edge counts and confirm no task is `in-progress`.
8. Confirm Starfall M3 still has zero assigned tasks and no trigger consumes it.
9. Search Starfall wiki for `M3` and `Deferred transformations and companions`; permit only the references explicitly replaced below.
10. Confirm no delivery requires `allowExceptional` or `--yes`.

Any mismatch stops before mutation.

## Commit boundary 1 — Coordinator descriptions and historical deliveries

### Milestone descriptions

Set M0-M5 to the exact Outcome/Scope/Exclusions/Evidence Markdown blocks under **Proposed milestone descriptions and delivery treatment** in the audit. Do not paraphrase them during execution.

| Milestone | Delivery treatment |
| --- | --- |
| M0 Coordinator foundation | preview, require 8/8 done, deliver normally |
| M1 Skinned mesh and animation proof | preview, require 15/15 done, deliver normally |
| M2 Shared character presentation | preview, require 12/12 done, deliver normally |
| M3 MMO bootstrap | set legacy-bucket description; preserve membership; do not deliver; create no trigger |
| M4 Starfall.Client Development Instrumentation Boundary | preview, require 1/1 done, deliver normally |
| M5 Connected Basic Arrow Shared Enablers | preview, require 4/4 done, deliver normally |

For each delivered milestone:

1. call `preview_milestone_delivery`;
2. verify zero unfinished tasks and normal mode;
3. pass its exact preview revision to `deliver_milestone` without exceptional confirmation;
4. inspect the receipt and resulting switchboard before continuing.

### Coordinator wiki

Update only:

- `roadmap/initial-family-roadmap` — record M0/M1/M2/M4/M5 as formally delivered capabilities and M3 as an undelivered historical bucket that must not source an activation contract;
- `roadmap/starfall-draft-0-shared-enablers` — record that the coordinator enabler milestones are delivered capability boundaries while canonical child dependencies remain unchanged.

Do not create coordinator triggers, alter tasks/dependencies, or reinterpret Starfall ownership.

### Coordinator validation and commit

- Re-read the coordinator switchboard: M0/M1/M2/M4/M5 `delivered`; M3 described but undelivered; zero triggers/issues.
- Run PM validation and `pm doctor`.
- Inspect every receipt for coordinator project ID and coordinator-only paths.
- Run `git diff --check`; inspect the complete staged list.
- Commit only coordinator PM/wiki changes as:

```text
[PM] Record coordinator milestone deliveries
```

Continue directly to the Starfall boundary under the same approval. Do not push yet.

## Commit boundary 2 — Starfall delivery and activation migration

### A. Describe milestones

Set M0-M2 and M4-M7 to the exact Outcome/Scope/Exclusions/Evidence Markdown blocks in the audit. Do not write a ceremonial M3 description immediately before removal; preserve its reviewed empty/non-deliverable rationale in the audit and matching wiki update.

### B. Deliver coherent history

Preview and normally deliver, in this order:

1. M0 Repository foundation — require 11/11 done;
2. M1 Shared character presentation — require 1/1 done;
3. M4 Development Instrumentation — require 5/5 done.

Do not deliver:

- M2, the completed legacy planning bucket;
- M5, which remains 10/15 done;
- M6, which remains 0/5 done;
- M7, which remains 0/4 done.

No exceptional delivery is permitted.

### C. Remove empty M3

After wiki-reference readback confirms M3 has no task or trigger ownership:

- update `product/design-direction` so transformations, wings, mounts, and companions remain milestone-free roadmap inputs rather than “M3” content;
- remove M3 through the supported owning-project operation;
- verify no task, trigger, or wiki identity becomes invalid.

Do not allocate a replacement milestone or trigger.

### D. Create and reconcile triggers

Create exactly:

| Key | Title | Requirements |
| --- | --- | --- |
| `development_instrumentation_available` | Development instrumentation available | `milestone:M4` |
| `gameplay_protocol_v1_available` | Gameplay protocol v1 negotiation available | `task:PROTOCOL-0015` |
| `connected_world_available` | Connected World exchange available | `task:SERVER-0005`, `task:CLIENT-0009` |
| `connected_snapshot_presentation_available` | Connected snapshot presentation available | `task:CLIENT-0009`, `task:CLIENT-0023` |

Then:

1. run dry-run reconciliation;
2. require all four requirements sets to be satisfied and no task to lose eligibility;
3. reconcile for real;
4. verify each trigger has an automatic activation record and is active;
5. attach active triggers only as follows:

```text
M6 requires development_instrumentation_available
M6 requires gameplay_protocol_v1_available
M6 requires connected_world_available
M7 requires connected_snapshot_presentation_available
```

Do not attach Development Instrumentation to M5. Do not create future Basic, Mana, HUD, Inventory, Fire, Rain, Life, Progression, Editor, or Pressure Cooker triggers.

### E. Eligibility checkpoint before edge removal

Read the Starfall switchboard and ready-task view. Require:

- M0, M1, M4 delivered;
- M2 described and undelivered;
- M3 absent;
- M5 active without required triggers;
- M6 active with three satisfied active triggers;
- M7 active with one satisfied active trigger;
- no switchboard issue or cycle;
- no task changed state or became ineligible;
- `CLIENT-0019 -> CLIENT-0031` remains present.

If any requirement fails, stop without removing dependencies.

### F. Remove six redundant edges and name consumed seams

Apply these exact final dependency lists:

| Task | Final `dependsOn` |
| --- | --- |
| `PROTOCOL-0014` | `SIM-0012` |
| `SERVER-0016` | `SIM-0012`, `PROTOCOL-0014` |
| `CLIENT-0032` | `PROTOCOL-0014`, `SERVER-0016` |
| `CLIENT-0033` | empty |
| `CLIENT-0034` | empty |

Preserve `CLIENT-0019` exactly:

```text
CLIENT-0012
CLIENT-0007
CLIENT-0011
CLIENT-0037
CLIENT-0031
```

Update the five changed task descriptions only to make these consumption contracts explicit; do not add new behavior:

- `PROTOCOL-0014` consumes the delivered connection-level gameplay-protocol-v1 negotiation contract and does not define a Basic-specific compatibility layer.
- `SERVER-0016` consumes the delivered admitted connected-World/session exchange and common development-command dispatcher.
- `CLIENT-0032` consumes the delivered debug shell, console, and correlated-result path while retaining Mana-owned diagnostics.
- `CLIENT-0033` consumes the delivered remote connected-snapshot-to-presentation adapter.
- `CLIENT-0034` consumes the delivered local connected-snapshot/correction adapter.

Do not rewrite completed tasks or canonical cross-project dependencies.

### G. Starfall wiki

Update only the sections necessary to make lifecycle and seam ownership durable:

- `roadmap/bootstrap` — record M0/M1/M4 delivery, M2 legacy-undelivered treatment, M3 removal, active M5/M6/M7 lifecycle, four trigger definitions/consumers, six removed edges, and the retained M5/M4 overlap;
- `product/design-direction` — replace M3 wording and state the anti-fan-out rule for later Fire, Rain, HUD, Life, Progression, Inventory, Equipment, and Drops;
- `roadmap/development-instrumentation` — record delivered M4 and its trigger promise for M6;
- `roadmap/authoritative-mana` — record the three milestone activation prerequisites and the concrete delivered seams consumed by M6 tasks;
- `roadmap/connected-movement-quality-v1` — record its one activation prerequisite and the preserved internal chain.

Do not treat dependencies/triggers as citations and do not allocate future work.

### H. Starfall validation and commit

Required final readback:

- 118 tasks: 59 done, 59 todo, zero active;
- 277 total edges, 250 local edges, 27 canonical edges;
- 88 local cross-milestone edges;
- four active triggers and four milestone-trigger requirements;
- retained `CLIENT-0019 -> CLIENT-0031`;
- zero missing, invalid, unavailable, or cyclic dependencies;
- zero switchboard issues;
- no `empty_milestone` warning after M3 removal.

Run PM validation, `pm doctor`, family warning readback, `git diff --check`, and staged-file inspection. Review every receipt for Starfall's exact stable project ID and paths. Confirm no source, asset, generated output, coordinator PM/wiki, Royale file, or gitlink is staged.

Commit as:

```text
[PM] Migrate Starfall milestone activation
```

Continue directly to the pointer handoff. Do not push yet.

## Commit boundary 3 — Pointer-only coordinator handoff

After the Starfall commit:

1. verify Starfall's stable ID, reciprocal parent declaration, path hint, and tracked gitlink;
2. require Starfall and Royale worktrees clean;
3. require Starfall `HEAD` to equal the reviewed activation-migration commit and descend from `36d6f8fd0d08869486cc017e614f20ecefc9e77b`;
4. require the coordinator to contain no change except the expected Starfall gitlink after boundary 1 is committed;
5. stage only `starfall` and inspect the staged submodule diff;
6. run `git diff --cached --check` and recursive submodule status;
7. commit:

```text
[PM] Pin Starfall activation migration
```

The body records:

- stable project ID `prj_pkIpzx0fzFD4URjvqBuYrGZF`;
- the exact pinned Starfall commit;
- “milestone descriptions, historical deliveries, M3 removal, activation triggers, and anti-fan-out dependency cleanup.”

No canonical task URI is fabricated. The commit is pointer-only.

Stop after this commit. Pushing remains separately owner-directed and must publish Starfall before the coordinator.

## Mutation receipts

For every operation, record:

- operation and owning project;
- receipt `projectId`;
- changed paths;
- switchboard impact, including automatic activation;
- immediate readback result.

There is no atomic cross-repository transaction. The commit boundaries are recovery points inside one approved migration, not separate planning cycles.

## Validation matrix

| Gate | Required result |
| --- | --- |
| Family | three available/readable/trusted members; zero warnings |
| PM identity | exact stable IDs, reciprocal declarations, correct path hints/gitlinks |
| Coordinator lifecycle | M0/M1/M2/M4/M5 delivered; M3 described and undelivered; no triggers/issues |
| Starfall lifecycle | M0/M1/M4 delivered; M2 undelivered legacy; M3 absent; M5/M6/M7 active |
| Triggers | four active automatic latches; consumers only M6/M7 as specified |
| Dependencies | six removals only; retained overlap; all internal and canonical edges preserved |
| Counts | Starfall 283 -> 277 edges; local 256 -> 250; local cross-milestone 94 -> 88; combined 368 -> 362 |
| Tasks | 118 total; 59 done; 59 todo; zero active; no task state mutation |
| PM validation | coordinator valid; Starfall valid with no empty-M3 warning |
| Git | three focused commits; child clean before gitlink; final pointer commit changes only `starfall` |
| Product/source | no source, build, test, asset, generated content, runtime, or visual change |

No build, gameplay, native, or visual validation is required because the migration changes only PM lifecycle/roadmap representation.

## Anti-fan-out acceptance

The migration is successful only if:

- each capability prerequisite is represented once at milestone level;
- no removed task dependency is recreated as documentation;
- the one narrow overlap edge remains deliberate and visible;
- internal implementation order remains intact;
- completed history remains intact;
- future roadmap prose instructs grooming not to recreate per-layer fans already covered by milestone activation.

The canonical example after this migration is:

```text
delivered Authoritative Mana
  -> authoritative_mana_available
  -> future Fire Arrow milestone

Fire content -> Fire simulation -> Fire protocol/World -> Fire client proof
```

Do not add Fire-Simulation-to-Mana-Simulation, Fire-Protocol-to-Mana-Protocol, Fire-World-to-Mana-World, or Fire-Client-to-Mana-Client edges when the Fire milestone is already inactive until Mana is delivered.

## Stop conditions

Stop without improvising if:

- the Starfall selector does not resolve uniquely to the expected readable, write-trusted project;
- linked read ownership metadata identifies the wrong project or revision, or a mutation receipt identifies the wrong project, changed path, or repository;
- trust, stable identity, reciprocal declarations, or gitlinks mismatch;
- a milestone preview requires exceptional delivery;
- trigger reconciliation does not activate all four triggers automatically;
- attaching a trigger changes eligibility outside the approved M6/M7 scope;
- any of the six dependency removals would occur before its replacement trigger is active;
- a task, completed dependency, canonical dependency, or retained overlap would change unexpectedly;
- any mutation receipt names the wrong project or path;
- a repository contains unrelated changes;
- validation reveals a cycle or new warning other than the pre-removal M3 warning.

Do not grant trust, override a trigger, use exceptional delivery, manually edit `.pm/`, allocate future milestones/triggers/tasks, activate feature work, or push automatically.

## Approval checklist

- [ ] Approve one Plan-mode decision and one launched migration.
- [ ] Approve the exact milestone descriptions and delivery treatment from the audit.
- [ ] Approve Starfall M3 removal with no replacement.
- [ ] Approve the four trigger definitions and exact M6/M7 attachments.
- [ ] Approve the six dependency removals and retained `CLIENT-0019 -> CLIENT-0031` overlap.
- [ ] Approve the task/wiki seam-wording updates.
- [ ] Approve the three commit boundaries and final stop before push.

After approval, execute this manifest exactly and stop after the pointer-only coordinator commit.
