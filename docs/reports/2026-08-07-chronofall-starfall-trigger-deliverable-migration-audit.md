# ChronoFall and Starfall Trigger/Deliverable Migration Audit

Date: 2026-08-07

Status: read-only planning audit; no milestone delivery, trigger, dependency, task, or wiki mutation is authorized by this file

This audit and its [companion log](2026-08-07-chronofall-starfall-trigger-deliverable-migration-log.md) form one work entity. Later owner decisions may add an addendum. A separately reviewed execution manifest should translate the accepted findings into exact PM mutations; understanding a future outcome does not authorize allocating or activating it.

The earlier [backlog audit](2026-08-05-chronofall-starfall-backlog-audit.md), [owner-decision addendum](2026-08-05-chronofall-starfall-backlog-audit-addendum.md), [execution manifest](2026-08-05-chronofall-starfall-backlog-execution-manifest.md), and [audit log](2026-08-05-chronofall-starfall-backlog-audit-log.md) remain authoritative for product ordering and task scope. This audit addresses only how PM milestone deliverables and activation triggers should represent those decisions.

## Snapshot

- Coordinator project: `prj_E7QP3LUocfY7k3PYM-EQOlqc`
- Coordinator revision: `db638944efda9a53eab132c09fea92210f6500e8`
- Starfall project: `prj_pkIpzx0fzFD4URjvqBuYrGZF`
- Starfall revision: `36d6f8fd0d08869486cc017e614f20ecefc9e77b`
- Royale project: `prj__-jXLQgm6GuD2gCKZ_bTa1m-` (read-only family context)
- Branches: coordinator and Starfall `pm-exploration`; Royale `main`
- Family resolution: three available, readable, write-trusted members; zero warnings
- PM validation: coordinator passed; Starfall passed with only the existing empty-M3 warning
- Worktrees: coordinator, Starfall, and Royale clean after the schema commits and pointer handoff
- Active tasks: none
- Activation triggers: none in either audited project
- Milestone descriptions: empty in both audited projects
- Delivery records: none in either audited project

The schema migration itself is committed separately:

- Starfall `36d6f8fd0d08869486cc017e614f20ecefc9e77b` — `[PM] Migrate milestone configuration schema`
- Coordinator `9e7ee4b` — `[PM] Migrate milestone configuration schema`
- Coordinator `db63894` — `[PM] Pin Starfall milestone schema migration`

No source, wiki, task, task state, dependency, trigger, delivery record, asset, Royale file, or other gitlink changed in those commits.

## Executive assessment

The new PM model fits the product decisions substantially better than the current task-only dependency graph.

The current graph is valid, but it asks task dependencies to perform three different jobs:

1. order concrete implementation work;
2. assert that a broader capability already exists;
3. cite historical evidence.

Only the first is inherently a task dependency. A delivered milestone plus a latched activation trigger is the correct representation for the second. Notes, task contracts, and wiki prose are the correct representation for the third and for naming the precise seams supplied by a delivered capability.

Preventing combinatorial cross-task fan-out is the primary purpose of this migration. The Draft 0 graph was reasonable before activation triggers existed: explicit task dependencies were the only available way to ensure prerequisite capabilities. Under the new PM model, a consumer milestone should name each prerequisite capability once. Once that trigger is active, every task in the milestone may rely on the delivered capability and its documented implementation seams without adding parallel Simulation-to-Simulation, Protocol-to-Protocol, World-to-World, and Client-to-Client edges.

The migration should not attempt to erase the history that produced the current graph. Completed task edges remain historical evidence. The first execution pass should instead:

- describe every milestone as a deliverable or explicitly identify it as a legacy bucket;
- formally deliver only coherent, accepted completed milestones;
- create four narrowly named Starfall capability triggers;
- attach those triggers only where the whole consumer milestone must wait;
- remove six now-redundant dependencies from todo tasks;
- deliberately retain one narrow cross-milestone overlap dependency for the Basic Arrow terminal diagnostic proof;
- leave all cross-project canonical dependencies intact;
- leave dormant, milestone-free initiatives unchanged until their deliverables activate.

This is a small migration with a large prospective benefit. It reduces only six current edges because completed history is preserved and one real overlap remains, but it establishes the rule that prevents Fire Arrow, the permanent HUD, Player Life, Progression, Inventory, Equipment, Physical Drops, and later combat work from rebuilding large prerequisite fans when their milestones are allocated.

## Current graph

### Size and state

| Project | Tasks | Done | Todo | Active | Local edges | Canonical edges | Total edges |
| --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| ChronoFall | 69 | 56 | 13 | 0 | 72 | 13 | 85 |
| Starfall | 118 | 59 | 59 | 0 | 256 | 27 | 283 |
| Combined audited graph | 187 | 115 | 72 | 0 | 328 | 40 | 368 |

ChronoFall has 27 local cross-milestone edges. Starfall has 94. Those numbers are not seven-times too large by themselves: many belong to completed historical work or exact source contracts. They show where semantic review matters.

### Coordinator milestones

| Milestone | Title | Done / total | Current lifecycle interpretation |
| --- | --- | ---: | --- |
| M0 | Coordinator foundation | 8 / 8 | coherent, ready to deliver |
| M1 | Skinned mesh and animation proof | 15 / 15 | coherent, ready to deliver |
| M2 | Shared character presentation | 12 / 12 | coherent, ready to deliver |
| M3 | MMO bootstrap | 12 / 12 | completed legacy bucket, not one deliverable |
| M4 | Starfall.Client Development Instrumentation Boundary | 1 / 1 | coherent, ready to deliver |
| M5 | Connected Basic Arrow Shared Enablers | 4 / 4 | coherent, ready to deliver |

### Starfall milestones

| Milestone | Title | Done / total | Current lifecycle interpretation |
| --- | --- | ---: | --- |
| M0 | Repository foundation | 11 / 11 | coherent, ready to deliver |
| M1 | Shared character presentation | 1 / 1 | coherent, ready to deliver |
| M2 | First playable zone | 31 / 31 | completed legacy planning bucket, not one deliverable |
| M3 | Deferred transformations and companions | 0 / 0 | empty initiative-shaped milestone |
| M4 | Development Instrumentation | 5 / 5 | coherent, ready to deliver |
| M5 | Connected Basic Arrow | 10 / 15 | active deliverable; authoritative-projectile closure remains |
| M6 | Authoritative Mana | 0 / 5 | active deliverable |
| M7 | Connected Movement Quality v1 | 0 / 4 | active deliverable |

## Representation rules

### A milestone is an accepted result

A milestone is a completable, independently demonstrable deliverable. Independently demonstrable does not mean dependency-free. It may build on earlier accepted outcomes, but it must add its own observable result.

Its description uses:

- **Outcome:** what becomes usable or accepted?
- **Scope:** what belongs in this delivery?
- **Exclusions:** what is intentionally deferred?
- **Evidence:** how is completion demonstrated?

A delivery record is an owner acceptance event, not an automatic consequence of every assigned task reaching done. A completed legacy bucket may therefore remain undelivered when its membership never represented one coherent result.

### A trigger is a stable capability promise

An activation trigger answers: “What must already be accepted before this milestone becomes eligible?”

- Requirements use AND semantics.
- Requirements may point to delivered milestones or completed tasks.
- Reconciliation latches an eligible trigger into a persisted activation record.
- The latch intentionally survives later historical reopening unless explicitly reset under PM's rules.
- A manual-only trigger is reserved for a real owner activation decision, not used to hide missing requirements.
- Trigger keys name durable capabilities, not task IDs, calendar phases, or implementation details.
- A task-backed trigger is appropriate when a useful capability seam is proven before a larger milestone completes.

### A task dependency orders work, not delivered capabilities

Within a milestone, keep task dependencies that express real implementation order: facts before codecs, simulation and protocol before World exchange, and World exchange before the terminal Client consumer.

Across milestones, the fact that a task consumes a concrete API, adapter, dispatcher, codec, or presentation seam is not by itself sufficient reason to retain a task dependency. Milestone delivery guarantees that the capability and its required seams are available. The consumer task description and durable wiki contract identify the exact seam it uses.

For every cross-milestone edge, ask:

> Could this downstream task become eligible before the prerequisite capability is guaranteed to exist?

- If the downstream milestone is inactive until the prerequisite milestone is delivered, remove the task dependency.
- If the milestones intentionally overlap and this particular task must wait while other tasks may proceed, retain the narrow dependency.
- If the trigger does not guarantee the required capability, correct the trigger or retain the dependency explicitly.
- Never retain an edge merely as architectural documentation.

Keep task dependencies when they deliberately coordinate overlapping milestones, order work inside one milestone, or preserve canonical cross-project ownership that local triggers cannot express.

Examples:

- Content inputs before the simulation that consumes them;
- facts before their deterministic codec;
- simulation and protocol before World exchange;
- World exchange before the terminal Client consumer;
- a selected asset before its acquisition/cook task within the same deliverable or an intentionally overlapping acquisition lane;
- a shared source task referenced canonically by a child integration task.

Do not keep a cross-milestone task edge merely to say “the platform already supports connected worlds,” “debug instrumentation exists,” “Mana was completed,” or “this task calls an API produced by that milestone.” Those are trigger promises plus task/wiki contract documentation.

### Documentation is not a dependency

Historical experiments, reviews, captures, and earlier architectural evidence belong in task notes or wiki prose unless the new task consumes a concrete artifact they produced. Do not use either dependencies or triggers as a bibliography.

### Cross-project dependencies stay canonical

The current PM surface does not establish linked-project trigger consumption as a replacement for canonical cross-project task dependencies. Preserve all 40 canonical edges unless a later PM capability and owner-approved family contract explicitly replace them. A local trigger must not pretend to prove a child or parent capability it cannot authoritatively resolve.

## Proposed milestone descriptions and delivery treatment

### ChronoFall M0 — Coordinator foundation

- **Outcome:** The shallow family checkout resolves the coordinator, Royale, and Starfall by stable reciprocal PM identity and supports safe owner-planned child work with mechanical gitlink handoff.
- **Scope:** linked topology, PM/agent workflow, recursive checkout and submodule policy, experiment/shared-source conventions, and asset ownership/provenance rules.
- **Exclusions:** child feature implementation, shared-engine extraction, gameplay, and release work.
- **Evidence:** passing coordinator doctor, zero linked-family warnings, verified reciprocal IDs/path hints/gitlinks, clean recursive checkout, and durable workflow documentation.
- **Recommendation:** preview and deliver normally.

### ChronoFall M1 — Skinned mesh and animation proof

- **Outcome:** One supplied Quaternius humanoid renders a correct bind pose and looping animation through GPU skinning with deterministic sampling and inspectable skeleton evidence.
- **Scope:** source inventory/selection, narrow loader decision, BCL skeletal contracts, deterministic tests, GPU bind/animation proof, debug skeleton, captures, and native validation.
- **Exclusions:** modular armour, retargeting, IK, animation graphs, final character content, and child integration.
- **Evidence:** deterministic tests/captures, native macOS ARM64 proof, skeleton diagnostics, documented findings, and owner visual acceptance.
- **Recommendation:** preview and deliver normally.

### ChronoFall M2 — Shared character presentation

- **Outcome:** Children can consume focused shared character-presentation, skeletal/static cooking, rendering, socket, blending, layering, and IK contracts without leaking presentation into headless code.
- **Scope:** promoted BCL presentation contracts, SDL GPU rendering, family source consumption/staging, skeletal/static cooking, sockets, action blending/layers, grips, reference points, and bounded IK/aim support.
- **Exclusions:** game-specific action mapping, equipment rules, generic animation graphs, arbitrary engine abstractions, final distribution, and headless presentation dependencies.
- **Evidence:** shared test suites, fresh-checkout-safe staging, native character/static rendering proofs, architecture dependency tests, and successful Starfall source consumption.
- **Recommendation:** preview and deliver normally.

### ChronoFall M3 — MMO bootstrap

- **Outcome:** This milestone records completed historical coordinator work gathered during early Starfall bootstrap; it establishes no single activation contract.
- **Scope:** only the task history already assigned to M3.
- **Exclusions:** interpreting the bucket as one accepted deliverable or using it as a prerequisite for future work.
- **Evidence:** the individual completed tasks, commits, and wiki pages remain their own evidence.
- **Recommendation:** set the description, preserve membership/history, but do not deliver it and do not derive a trigger from it.

### ChronoFall M4 — Starfall.Client Development Instrumentation Boundary

- **Outcome:** Starfall.Client is an approved consumer of the shared caller-controlled ImGui backend while all headless products remain presentation-free.
- **Scope:** family-source allowlist, native/backend lifecycle compatibility, macOS ARM64 validation, and dependency-isolation evidence.
- **Exclusions:** Starfall debug-window behavior, F12/launch policy, feature commands, permanent HUD/UI, and Royale migration.
- **Evidence:** shared/backend tests, source-reference architecture checks, native Starfall use, and clean headless outputs.
- **Recommendation:** preview and deliver normally.

### ChronoFall M5 — Connected Basic Arrow Shared Enablers

- **Outcome:** The exact selected bow/arrow/body-animation inputs can be staged and one socketed static bow is proven through the shared presentation boundary.
- **Scope:** exact curated acquisition, reproducible staging/cooking evidence, Blender evaluation guidance, and the reusable technical socketed-bow proof.
- **Exclusions:** Starfall semantic grip placement, authoritative combat, projectile simulation, equipment systems, aiming/IK integration, and final character art.
- **Evidence:** exact provenance/manifests, deterministic cook/stage validation, native socketed-bow proof, and owner visual acceptance.
- **Recommendation:** preview and deliver normally.

### Starfall M0 — Repository foundation

- **Outcome:** Starfall has an independently owned, runnable, headless-safe project foundation with defined authority/service boundaries and approved coordinator-family source consumption.
- **Scope:** repository/project graph, architecture tests, client/world host shells, admission contract, family source policy, and automatic pointer-handoff policy.
- **Exclusions:** connected gameplay features, final service topology, persistence implementation, content production, and presentation systems.
- **Evidence:** passing solution/architecture tests, runnable Client and World shells, admission contract tests, documented boundaries, and clean family handoffs.
- **Recommendation:** preview and deliver normally.

### Starfall M1 — Shared character presentation

- **Outcome:** Starfall.Client renders the technical humanoid through the approved coordinator-owned presentation projects while every headless project remains independent of presentation code.
- **Scope:** the bounded child integration, generated-content staging/validation, native preview, and dependency-isolation tests.
- **Exclusions:** selected final character content, gameplay animation mapping, equipment, combat presentation, and independent package distribution.
- **Evidence:** native macOS preview, non-graphical content probe, exact source-reference allowlist, and headless output inspection.
- **Recommendation:** preview and deliver normally.

### Starfall M2 — First playable zone

- **Outcome:** This milestone preserves historical evidence accumulated by the former broad first-playable planning bucket; it is not one accepted end-to-end deliverable.
- **Scope:** only its existing completed graybox, movement, connected-monster, camp, combat-simulation, protected-town, and grooming history.
- **Exclusions:** using M2 delivery as a prerequisite, adding unfinished work, or claiming those capabilities formed one milestone under the current deliverable model.
- **Evidence:** each completed task and its native/deterministic validation remain the authoritative evidence for its specific capability.
- **Recommendation:** set the description, preserve completed membership, do not deliver, and expose only deliberately selected task-backed triggers from its real seams.

### Starfall M3 — Deferred transformations and companions

- **Outcome:** None; the broad future ideas remain milestone-free roadmap placeholders until an owner activates a concrete deliverable.
- **Scope:** no executable work.
- **Exclusions:** wings, mounts, companions, transformations, progression, or presentation implementation merely because the historical key exists.
- **Evidence:** not applicable until a focused deliverable is approved.
- **Recommendation:** remove the empty milestone through supported PM tooling after confirming no wiki identity depends on the key. Do not create a trigger or replacement milestone yet.

### Starfall M4 — Development Instrumentation

- **Outcome:** A developer can show/hide concern-specific ImGui instrumentation, issue `ping` through the typed/debug-console path, and receive the correlated authoritative World result without affecting gameplay or headless isolation.
- **Scope:** shared backend adoption, debug shell/input capture, development command envelope, admitted-session dispatch, console frontend, and World/session diagnostics.
- **Exclusions:** permanent gameplay UI, stable gameplay-protocol compatibility, feature-specific Mana/combat commands, admin permissions, and observability infrastructure.
- **Evidence:** deterministic command/codec/dispatch tests, architecture tests, native macOS validation of menu/F12/hidden launch/input capture/`ping`, and owner confirmation.
- **Recommendation:** preview and deliver first; it becomes the requirement for `development_instrumentation_available`.

### Starfall M5 — Connected Basic Arrow

- **Outcome:** A connected player issues Basic Arrow against an authoritative monster; the World resolves a frozen-aim, first-contact straight projectile and the Client presents the rendered socketed bow/body action, authoritative spawn/termination, hit feedback, monster damage/death, and Combat diagnostics end to end.
- **Scope:** the already-completed connected intent/action/presentation evidence plus straight-projectile inputs, deterministic authority/collision, replacement protocol facts/codecs, World lifecycle/exchange, authoritative event-driven Client presentation, and terminal native validation.
- **Exclusions:** Fire Arrow, Arrow Rain, Mana, permanent damage numbers/target HUD, equipment/inventory, generalized projectile frameworks, and Arrow Rain spatial projectiles.
- **Evidence:** focused Content/Simulation/Protocol/World/Client tests, deterministic collision/order fixtures, clean headless outputs, and one owner-validated native connected run.
- **Recommendation:** do not gate all of M5 on Development Instrumentation. Preserve exact internal task order and the narrow `CLIENT-0019 -> CLIENT-0031` overlap edge; deliver only after all five remaining tasks and owner validation complete.

### Starfall M6 — Authoritative Mana

- **Outcome:** Connected integer Mana initializes, consumes, clamps, regenerates, restores, serializes, exchanges, and is inspectable through authoritative development commands before any spell owns it.
- **Scope:** explicit inputs, fixed-tick authoritative state, transport-neutral facts/codecs, admitted World exchange, resource diagnostics, development-command handlers, and lifecycle seam.
- **Exclusions:** Fire Arrow, Arrow Rain, permanent HUD, death/respawn Mana policy, stable development-command compatibility, and floating-point resources.
- **Evidence:** deterministic resource/codec/exchange tests, headless isolation, and a native World/Client proof of consume, empty, regeneration, refill, rejection, and authoritative correction.
- **Recommendation:** require Development Instrumentation, gameplay-protocol-v1, and connected-world triggers; keep only internal implementation dependencies afterward.

### Starfall M7 — Connected Movement Quality v1

- **Outcome:** Remote authoritative movement is visibly smoother under representative network conditions and local-player corrections are diagnosable without silently adding prediction or local interpolation delay.
- **Scope:** remote snapshot buffering/interpolation, local correction diagnostics, deterministic latency/loss/reordering fixtures, and native before/after comparison.
- **Exclusions:** mandatory prediction/reconciliation, gameplay authority changes, simulation interpolation, protocol redesign without evidence, and a never-ending quality bucket.
- **Evidence:** deterministic fixtures/tests, unchanged authoritative outcomes, headless isolation, macOS native comparison, and an explicit decision on whether v1 evidence justifies later prediction/reconciliation.
- **Recommendation:** require the connected-snapshot-presentation trigger and keep the remaining internal dependency chain.

## Proposed trigger registry

### First migration pass

| Trigger key | Title | Requirements | Consuming milestones | Promise |
| --- | --- | --- | --- | --- |
| `development_instrumentation_available` | Development instrumentation available | milestone `M4` delivered | `M6` | Concern-specific ImGui shell, admitted development-command dispatch, console, and correlated result path are accepted. |
| `gameplay_protocol_v1_available` | Gameplay protocol v1 negotiation available | task `PROTOCOL-0015` done | `M6` | Admission negotiates the single gameplay protocol version and gameplay codecs may use the accepted v1 layout contract. |
| `connected_world_available` | Connected World exchange available | tasks `SERVER-0005` and `CLIENT-0009` done | `M6` | An admitted session exchanges authoritative World state with the connected Client through the established host path. |
| `connected_snapshot_presentation_available` | Connected snapshot presentation available | tasks `CLIENT-0009` and `CLIENT-0023` done | `M7` | The local player and remote monster snapshot adapters are proven consumers of authoritative connected state. |

These triggers are intentionally project-local. Reconcile them only after their requirements read satisfied. Attach the already-active triggers to milestones before removing replaced task dependencies so no current work loses eligibility.

### Create when their producer or first consumer is ready

| Trigger key | Requirement | Likely consumers | Creation point |
| --- | --- | --- | --- |
| `connected_basic_arrow_available` | delivered Starfall `M5` | Fire Arrow, Progression, later combat consumers | when M5 is delivered or the first consumer milestone is allocated |
| `authoritative_mana_available` | delivered Starfall `M6` | Fire Arrow, Resource HUD, Player Life, Arrow Rain | when M6 is delivered or the first consumer milestone is allocated |
| `movement_quality_v1_available` | delivered Starfall `M7` | a later evidence-driven movement-quality deliverable | only when a real consumer exists |
| `combat_action_contract_available` | exact future Fire task(s) proving the second action consumer | Arrow Rain | during Fire planning, only if source evidence identifies a real reusable seam |
| `player_resource_hud_available` | delivered future Resource HUD milestone | Inventory Client surface and later permanent resource UI consumers | when that milestone exists |
| `inventory_available` | delivered future Inventory milestone | Equipment and Physical Drops | when Inventory is delivered or either consumer is allocated |

Do not allocate these triggers merely because their names are understood. A trigger with neither a completed/active producer nor a real consuming milestone is another dormant placeholder.

### Manual future activation

Pressure Cooker remains deferred. When a concrete need justifies allocating its implementation milestone, a manual-only coordinator trigger such as `pressure_cooker_needed` may record the owner activation decision. Do not create it now, do not attach it directly to dormant task `SHARED-0025`, and do not treat the task title “after an activation trigger” as an existing trigger definition.

## Exact first-pass dependency migration

The original seven audited edges have been re-evaluated using the cross-milestone eligibility test. Six become redundant after the stated trigger is active and attached to the owning milestone. One remains as a deliberate overlap dependency.

| Todo task | Remove dependency | Replacement milestone trigger | Reason |
| --- | --- | --- | --- |
| `PROTOCOL-0014` | `PROTOCOL-0015` | M6 requires `gameplay_protocol_v1_available` | Mana codec work consumes the negotiated gameplay-version capability, not the historical task as an ordering step. |
| `SERVER-0016` | `SERVER-0005` | M6 requires `connected_world_available` | The connected World host path is a pre-existing capability. |
| `SERVER-0016` | `SERVER-0015` | M6 requires `development_instrumentation_available` | Mana registers feature-owned handlers against the delivered development dispatcher. |
| `CLIENT-0032` | `CLIENT-0031` | M6 requires `development_instrumentation_available` | Resource diagnostics consume the accepted debug shell/console capability. |
| `CLIENT-0033` | `CLIENT-0023` | M7 requires `connected_snapshot_presentation_available` | Remote interpolation consumes the proven remote snapshot adapter capability. |
| `CLIENT-0034` | `CLIENT-0009` | M7 requires `connected_snapshot_presentation_available` | Local correction diagnostics consume the proven local snapshot adapter capability. |

Retain this narrow cross-milestone dependency:

| Todo task | Retain dependency | Why milestone activation does not replace it |
| --- | --- | --- |
| `CLIENT-0019` | `CLIENT-0031` | M5 and M4 intentionally overlapped. Most Basic Arrow work was valid before Development Instrumentation completed, while only the terminal connected Combat diagnostic proof requires the debug shell/console seam. Gating all M5 on M4 would make the milestone model less accurate. |

Expected immediate graph change:

- Starfall edges: `283 -> 277`
- Starfall local edges: `256 -> 250`
- Starfall local cross-milestone edges: `94 -> 88`
- Combined audited edges: `368 -> 362`
- New Starfall triggers: `0 -> 4`
- New milestone/trigger requirements: four (`M6` three, `M7` one)

Do not remove the six exact dependencies until switchboard readback proves the replacement trigger active and the consumer milestone eligible. Do not attach `development_instrumentation_available` to M5 merely to remove the retained overlap edge.

## Dependencies that remain tasks

### Connected Basic Arrow

Preserve the unfinished internal chain:

```text
CONTENT-0017 -> SIM-0013
SIM-0013 + PROTOCOL-0015 + PROTOCOL-0007 -> PROTOCOL-0016
SIM-0013 + PROTOCOL-0016 + SERVER-0008 -> SERVER-0017
CLIENT-0018 + PROTOCOL-0016 + SERVER-0017 -> CLIENT-0037
CLIENT-0037 + the exact Basic presentation leaves -> CLIENT-0019
```

Those edges order concrete contracts and implementations inside one deliverable. A Basic Arrow trigger cannot gate its own milestone without creating a semantic cycle.

### Authoritative Mana

Preserve the internal chain:

```text
CONTENT-0016 -> SIM-0012
SIM-0012 -> PROTOCOL-0014
SIM-0012 + PROTOCOL-0014 -> SERVER-0016
PROTOCOL-0014 + SERVER-0016 -> CLIENT-0032
```

Keep `CONTENT-0016 -> CONTENT-0003` until Mana planning confirms whether it consumes the established neutral gameplay-resource scale. If it is merely historical context, remove it during that task's Plan-mode review; do not replace it with a broad trigger speculatively.

### Connected Movement Quality v1

After the two external adapter edges migrate to the trigger, preserve:

```text
CLIENT-0033 + CLIENT-0034 -> CLIENT-0035 -> CLIENT-0036
```

These edges describe the v1 evidence sequence, not a broad capability gate.

### Cross-project source and asset contracts

Keep canonical dependencies such as Starfall Client consumption of `SHARED-0026`, character/static rendering, selected-input acquisition, Box3D, transport, and screenshot capture. They resolve exact owning-project artifacts and remain necessary until PM supports a reviewed linked-trigger contract with equivalent identity and readiness semantics.

## Future graph discipline

Every future milestone grooming pass must start with capability triggers before adding cross-milestone task dependencies.

### Fire Arrow and Mana

The intended representation is:

```text
delivered M6 -> authoritative_mana_available -> Fire Arrow milestone

Fire content -> Fire simulation -> Fire protocol/World -> Fire client proof
```

Do not recreate:

```text
Fire simulation -> Mana simulation
Fire protocol   -> Mana protocol
Fire World      -> Mana World
Fire client     -> Mana client
```

Fire task contracts should identify the exact Mana state/service, facts/codecs, World integration, and diagnostics they consume. Because the entire Fire milestone remains inactive until Mana is delivered, those per-layer cross-milestone edges add no readiness guarantee.

The same rule applies to:

- Arrow Rain consuming delivered Mana and the established combat-action capability;
- the permanent HUD consuming delivered Mana;
- Player Life consuming delivered Mana's lifecycle seam;
- Progression consuming delivered Basic Arrow/monster-death capability;
- Equipment and Physical Drops consuming delivered Inventory;
- later combat actions consuming delivered targeting, action, projectile, or resource capabilities.

A narrow cross-milestone dependency remains appropriate when producer and consumer milestones deliberately overlap and only one downstream task must wait. `CLIENT-0019 -> CLIENT-0031` is the current reference case.

### Migration success criteria

The eventual execution manifest must measure all of the following:

- capability prerequisites represented once at milestone level;
- redundant cross-milestone task dependencies removed;
- narrow overlap dependencies retained with an explicit reason;
- internal task ordering preserved;
- completed history preserved;
- task and wiki contracts updated to name the precise consumed seams;
- future grooming instructed not to recreate per-layer fans already guaranteed by activation;
- before/after task-edge and milestone-trigger counts recorded from authoritative readback.

## Historical dependency treatment

Do not rewrite dependencies on completed tasks simply to lower the edge count. Their files are historical evidence of the order and assumptions under which the work was completed.

This specifically means:

- M0/M1/M2 completed chains stay intact;
- completed M4 instrumentation edges stay intact;
- the completed portion of M5 stays intact;
- coordinator experimental/promotion history stays intact;
- previous pointer/grooming canonical edges stay intact;
- documentary edges are corrected only when a current task is reviewed or a concrete false readiness problem remains.

The migration success metric is clearer eligibility and less repeated future fan-out, not the smallest possible graph.

## Partial-deliverable trigger opportunities

The task-backed trigger capability should be used sparingly and deliberately.

### Approved first use: gameplay protocol v1

`PROTOCOL-0015` is complete inside unfinished M5 and exposes a stable, independently useful protocol-negotiation seam. `gameplay_protocol_v1_available` allows Mana to proceed without falsely waiting for the whole Basic Arrow milestone.

### Approved first use: connected snapshot presentation

`CLIENT-0009` and `CLIENT-0023` are completed terminal evidence inside legacy M2. Their combined trigger exposes the exact local/remote snapshot adapters needed by M7 without pretending M2 itself is a deliverable.

### Future candidate: combat action contract

Fire Arrow is the first second consumer of Basic's action lifecycle. If Fire planning identifies a focused extraction/refactor task, that task may latch `combat_action_contract_available` before the Fire milestone's presentation proof completes, allowing Arrow Rain's authoritative lane to begin. Do not preselect the task or trigger requirements before that source inspection.

### Rejected patterns

- “first three tasks complete” without a named usable capability;
- a trigger sourced from a task inside the same consuming milestone when it creates self-activation;
- a trigger whose requirements merely restate every task in a milestone;
- a trigger used as a citation;
- a trigger for an initiative with no allocated deliverable;
- a local trigger pretending to resolve a linked-project artifact.

## Cycle and lifecycle review

The proposed first pass is acyclic:

```text
delivered M4 -> development_instrumentation_available -> M6
done PROTOCOL-0015 -> gameplay_protocol_v1_available -> M6
done SERVER-0005 + CLIENT-0009 -> connected_world_available -> M6
done CLIENT-0009 + CLIENT-0023 -> connected_snapshot_presentation_available -> M7
```

M5 remains independently eligible and overlaps with M4 through the narrow `CLIENT-0019 -> CLIENT-0031` edge. M6 and M7 do not source their prerequisites. Future outcome triggers point from delivered producer milestones to later consumer milestones.

The safe mutation order is:

1. set milestone descriptions;
2. preview and deliver coherent completed milestones;
3. create trigger definitions;
4. dry-run reconciliation and inspect impact;
5. reconcile satisfied triggers;
6. attach only active triggers to current consumer milestones;
7. re-read switchboard eligibility;
8. remove the six replaced task edges and verify the one deliberate overlap edge remains;
9. validate tasks, switchboard, warnings, and cycles again;
10. update matching wiki roadmap text and audit log;
11. commit one owning repository at a time with the usual Starfall pointer handoff.

Attaching an inactive trigger before its requirements are understood could make eligible work disappear. Redefining an active trigger may also revoke eligibility and therefore requires preview/revision confirmation. The execution manifest must preserve these gates.

## Proposed execution-cycle boundaries

No cycle is authorized by this audit alone.

### Cycle A — Coordinator descriptions and historical deliveries

- set descriptions for M0-M5;
- deliver M0, M1, M2, M4, and M5 normally after preview;
- leave M3 undelivered as a legacy bucket;
- create no coordinator trigger yet;
- update only matching coordinator roadmap/wiki prose;
- validate and commit `[PM]`; stop.

### Cycle B — Starfall descriptions and coherent completed deliveries

- set descriptions for M0-M7;
- deliver M0, M1, and M4 normally after preview;
- leave M2 undelivered as a legacy bucket;
- remove empty M3 only after checking durable references;
- do not deliver M5-M7;
- update matching Starfall roadmap/wiki prose;
- validate, commit `[PM]`, perform pointer handoff, stop.

### Cycle C — Starfall current trigger migration

- create and reconcile the four first-pass triggers;
- attach them only to M6 and M7 exactly as specified;
- verify eligibility before removing dependencies;
- remove only the six enumerated redundant task edges;
- retain `CLIENT-0019 -> CLIENT-0031` and record why M5 intentionally overlaps M4;
- update task wording only where it still names the removed edge rather than the consumed capability;
- update durable task/wiki contracts to name the concrete seams now guaranteed by activation;
- validate counts, readiness, cycles, PM doctor, receipts, and family warnings;
- commit `[PM]`, perform pointer handoff, stop.

Later producer/consumer triggers are created in the owner-approved grooming cycle that allocates or activates their real consumer milestone. Do not batch future Fire, HUD, Player Life, Progression, Inventory, Equipment, Drop, Editor, or Pressure Cooker graphs into Cycle C.

## Findings

### TD-01 — Completed coherent milestones have no accepted delivery record

Severity: high

Five coordinator milestones and three Starfall milestones are coherent, complete, and supported by durable evidence, but PM currently records only task completion. Their descriptions and delivery records should become the accepted capability boundaries from which future triggers derive.

### TD-02 — Legacy buckets must not become false contracts

Severity: high

Coordinator M3 and Starfall M2 are completed but not coherent deliverables. Delivering them would turn historical grouping into an architectural promise. Preserve them as described legacy buckets and use task-backed triggers for real seams.

### TD-03 — Starfall M3 is an initiative disguised as a milestone

Severity: medium

M3 has no tasks and produces an `empty_milestone` warning. Wings, transformations, mounts, and companions remain broad future outcomes. Remove the empty milestone rather than inventing scope or tasks to populate it.

### TD-04 — Development Instrumentation demonstrates both trigger and overlap cases

Severity: high

M4 is complete. M6 requires Development Instrumentation as a whole and should consume it through `development_instrumentation_available`, removing its per-layer `SERVER-0015`/`CLIENT-0031` edges. M5 intentionally overlapped M4: only terminal task `CLIENT-0019` needs `CLIENT-0031`. Retain that narrow edge instead of globally gating M5.

### TD-05 — Mana is accidentally coupled to Basic through a completed task

Severity: high

`PROTOCOL-0014` depends on `PROTOCOL-0015`, which currently belongs to unfinished M5. The dependency is satisfied today, but the model suggests Mana consumes Basic Arrow rather than the already-proven connection-level protocol contract. A task-backed protocol trigger expresses the real seam and preserves Mana's independent milestone.

### TD-06 — Movement Quality repeats adapter prerequisites

Severity: medium

M7's two parallel tasks each point to the completed adapter they consume. A single task-backed trigger requiring both adapters more accurately gates the deliverable, while the internal fixture/proof sequence remains task dependencies.

### TD-07 — Cross-project triggers are not yet a family contract

Severity: high

Parent/child source, asset, Box3D, transport, screenshot, and debug-backend dependencies remain canonical task URIs. Do not replace them with aliases, prose, or local triggers.

### TD-08 — Future symbolic deliverables should not produce dormant trigger clutter

Severity: medium

The future trigger names are planning handles. Create them only alongside a real producer delivery or consumer milestone. This preserves the earlier rule that understanding future work does not mean allocating its whole graph.

### TD-09 — Pressure Cooker needs an owner activation, not a fabricated prerequisite

Severity: medium

The deferred architecture is durable, but neither `SHARED-0025` nor its title defines a real activation trigger. Keep the work milestone-free and `priority: none`. When a concrete need appears, allocate the deliverable and record the owner decision through a manual trigger.

### TD-10 — Completed dependency history should not be cosmetically normalized

Severity: medium

Most of the 368 edges are completed history or exact internal order. Rewriting completed edges would obscure evidence and generate risk for little scheduling value. For todo and future work, however, the old graph is not presumptively correct: apply the cross-milestone eligibility test and remove per-layer fans whenever milestone activation already guarantees the capability.

## Decisions required before an execution manifest

1. Approve or revise the proposed milestone descriptions.
2. Approve normal delivery of coordinator M0/M1/M2/M4/M5 and Starfall M0/M1/M4.
3. Confirm coordinator M3 and Starfall M2 remain undelivered legacy buckets.
4. Confirm empty Starfall M3 should be removed rather than retained as an initiative placeholder.
5. Approve the four first-pass trigger keys, requirements, promises, and consuming milestones, with no M5-wide Development Instrumentation gate.
6. Approve the six exact task-edge removals and deliberate retention of `CLIENT-0019 -> CLIENT-0031`.
7. Confirm completed task dependencies remain historical and are not mass-rewritten.
8. Confirm canonical cross-project task dependencies remain unchanged.
9. Confirm future milestone activation replaces per-layer dependency fans unless a documented overlap requires a narrow edge.
10. Confirm future outcome triggers are allocated only with a real producer/consumer cycle.
11. Confirm Pressure Cooker remains dormant and receives no trigger until an owner activation decision.

## Audit checklist

### Audit preparation

- [x] Migrate the coordinator and Starfall PM milestone schemas with focused commits and a pointer-only handoff.
- [x] Verify stable project IDs, reciprocal resolution, write trust, and zero family warnings.
- [x] Run PM validation in both audited projects.
- [x] Record current task, state, milestone, and dependency counts.
- [x] Confirm no task is active and no trigger or delivery record exists.
- [x] Draft Outcome/Scope/Exclusions/Evidence for every current milestone.
- [x] Separate coherent completed milestones from legacy buckets.
- [x] Classify current dependencies as exact order, capability gate, historical evidence, or canonical linked ownership.
- [x] Propose task-backed partial-capability triggers without introducing cycles.
- [x] Define the first-pass graph delta and safe mutation order.
- [x] Re-evaluate all seven candidate edges using the cross-milestone eligibility test.
- [x] Add prospective anti-fan-out rules and measurable manifest success criteria.

### Owner decisions

- [ ] Approve/revise milestone descriptions and delivery treatment.
- [ ] Approve/revise first-pass trigger definitions and consumers.
- [ ] Approve/revise the six exact dependency replacements and one retained overlap edge.
- [ ] Approve/revise empty Starfall M3 removal.
- [ ] Approve preserving legacy buckets and completed dependency history.
- [ ] Approve preserving canonical cross-project task dependencies.
- [ ] Approve the anti-fan-out rule for every future milestone grooming pass.
- [ ] Approve producing the trigger/deliverable execution manifest.

### Execution

- [ ] Produce a reviewed execution manifest from fresh PM readback.
- [ ] Complete Cycle A coordinator descriptions/deliveries; log and validate it.
- [ ] Complete Cycle B Starfall descriptions/deliveries; log and validate it.
- [ ] Complete Cycle C Starfall trigger/edge migration; log and validate it.
- [ ] Re-read switchboards and prove every current milestone has the intended lifecycle.
- [ ] Verify M5/M6/M7 eligibility is unchanged except where explicitly approved.
- [ ] Verify the resulting task and trigger graph is acyclic.
- [ ] Record before/after edge counts and confirm the retained overlap edge is documented.
- [ ] Verify no task became active during grooming.
- [ ] Check off every completed item in this audit during the owning cycle.

## Recommended next action

Review the eleven decisions above. Once settled, produce the execution manifest with the anti-fan-out test as a mandatory dependency-review rule; do not mutate milestones, deliveries, triggers, dependencies, or wiki text directly from this audit.
