# ChronoFall and Starfall Backlog Audit

Date: 2026-08-05

Snapshot:

- ChronoFall project: `prj_E7QP3LUocfY7k3PYM-EQOlqc`
- ChronoFall revision: `6279afd898b9dcdbbce2852a66bf198b866d2b50`
- Starfall project: `prj_pkIpzx0fzFD4URjvqBuYrGZF`
- Starfall revision: `96a42b250b7018bb1d19c7baa24c07eeea9757bd`
- Family resolution: three readable, write-trusted members; zero linked-project warnings
- PM validation: ChronoFall and Starfall passed `pm doctor`
- Worktrees at inspection: ChronoFall, Starfall, and Royale clean; no task active

This is a planning audit, not an approved PM mutation. It evaluates the backlog against the preferred development cadence:

> Establish one convincing, observable truth at a time; complete it across the stack; let that evidence determine the next step.

This audit, its [audit log](2026-08-05-chronofall-starfall-backlog-audit-log.md), [owner-decision addendum](2026-08-05-chronofall-starfall-backlog-audit-addendum.md), and [execution manifest](2026-08-05-chronofall-starfall-backlog-execution-manifest.md) form one work entity. Later owner decisions in the addendum refine or supersede alternatives in this original assessment without erasing the historical analysis; the manifest translates the settled decisions into direct grooming and execution boundaries.

## Executive assessment

The backlog is substantially healthier than its raw size suggests. The completed walking and connected-monster work forms a coherent foundation, and the newly groomed Basic Arrow network chain is the correct immediate path:

```text
PROTOCOL-0006
  -> PROTOCOL-0007
  -> SERVER-0008
  -> CLIENT-0012
  -> native end-to-end Basic Arrow validation
```

That chain should remain intact. It is focused, dependency-ready in the correct order, and reuses already-proven authoritative Basic Arrow behavior and connected monster presentation.

The main problems begin after that first proof:

1. Starfall M2 contains 77 tasks and behaves as an entire development program rather than a useful milestone.
2. The starter bow is unnecessarily trapped behind the complete inventory, drop, and earned-equipment chain.
3. `CONTENT-0008` combines progression, starter equipment, and drops while still leaving exact drop inputs unresolved for downstream consumers.
4. Basic Arrow presentation is held behind Fire Arrow or unrelated later presentation work in several places.
5. Fire Arrow and Arrow Rain both assume shared mana/action state, but the graph does not establish one unambiguous owner or order.
6. Combat, progression, and inventory UI tasks require text and runtime UI capabilities that no current task owns.
7. Several deferred editor/shared tasks are initiatives disguised as single implementation tasks.
8. Snapshot interpolation/smoothing is acknowledged technical debt but has no durable task owner.

No unfinished task needs to be deleted immediately. Several do need to be split, narrowed, deferred more honestly, or clarified before they are executable.

## Current factual baseline

### Backlog size

| Project | Total | Done | Unfinished | Ready unfinished |
|---|---:|---:|---:|---:|
| ChronoFall | 67 | 51 | 16 | 3 |
| Starfall | 98 | 44 | 54 | 10 |

Starfall M2 alone contains 77 tasks: 31 completed and 46 unfinished. This is the strongest evidence that M2 is an umbrella roadmap bucket, not a milestone that communicates a near-term finish line.

### Implemented Starfall evidence

The repository already proves:

- a deterministic Draft 0 graybox and isometric client;
- local and connected authoritative walking;
- signed admission and world-owned sessions;
- ten placeholder monsters with stable identities;
- authoritative camp spawning and replenishment;
- bounded monster awareness, pursuit, attack, return, and protected-town behavior;
- player damage, defeat, restoration, and town respawn;
- authoritative Basic Arrow rules, integer damage, cancellation, monster death, and replenishment;
- connected monster snapshots with health changes, hit flash, behavior presentation, defeat, and tombstones.

What is specifically absent is the connected Basic Arrow command/fact/exchange/client-input path. The PM recommendation of `PROTOCOL-0006` therefore matches the source evidence.

## Findings

### F-01 — Starfall M2 is too large to function as a useful milestone

Severity: high, roadmap clarity

M2 currently combines:

- connected Basic Arrow;
- Fire Arrow and Arrow Rain;
- combat HUD and presentation;
- progression and leveling;
- drops, inventory, and equipment;
- selected character, monster, armour, bow, arrow, and environment assets;
- proper scene authoring and editor infrastructure;
- Balance Lab infrastructure and reports.

The dependency graph can still select sensible work, but milestone progress cannot communicate what is close to playable. It also makes low-priority future work appear to be part of the same finish line as connected Basic Arrow.

Recommendation:

- Do not move completed tasks merely to rewrite history.
- Keep the completed walking/monster work as historical M2 evidence.
- Organize unfinished work into outcome-oriented checkpoints or new milestones, approximately:
  - connected Basic Arrow;
  - complete three-action combat kit;
  - progression/drop/equipment loop;
  - selected presentation and proper scene;
  - later editor/Balance Lab capability.
- If PM milestones remain capability buckets instead, say so explicitly and maintain a separate ordered vertical-slice checklist in the roadmap.

Owner decision required: new outcome milestones versus retaining M2 as an explicitly documented umbrella.

### F-02 — The starter bow is incorrectly gated by the entire equipment loop

Severity: high, vertical-slice ordering

`GAME-0005` owns both the initially equipped wooden bow and later earned Ranger equipment, but depends on `GAME-0003` inventory. `CONTENT-0009` and `CLIENT-0011` then depend on `GAME-0005`.

The result is backwards: the first class cannot visibly hold its starter weapon until item identity, inventory, physical drops, and earned-equipment behavior are built.

Addendum A-04 supersedes the starter-loadout remedy below. Basic Arrow and the first rendered bow require no equipment or loadout contract at all. The first bow is a presentation proof using one picked asset and the established socket/rendering boundaries; authoritative equipment comes later.

Recommendation:

- Create or extract a focused authoritative starter-loadout task:
  - one equipped wooden bow;
  - no equipped armour;
  - non-equipment underlayer;
  - unlimited ammunition with no ammunition inventory;
  - stable authoritative equipment fact sufficient for presentation.
- Refocus `GAME-0005` on earned-item equip/unequip, replacement, and authoritative stat effects after inventory exists.
- Make `CONTENT-0009` and the first rendered-bow integration depend on the starter-loadout contract, not the complete earned-equipment loop.
- Keep Ranger armour progression behind inventory/drop/equipment work.

### F-03 — `CONTENT-0008` combines three domains and does not supply an exact downstream drop contract

Severity: high, scope and contract completeness

`CONTENT-0008` defines progression, starter equipment, and drops in one task. It freezes the XP sequence and monster XP ranges, but explicitly leaves exact drop tables, modifiers, and gains unresolved. Later `GAME-0004` says it consumes exact seeded drop-table inputs, and `EDITOR-0006` says it reports exact drop tables.

That is both oversized and incomplete: downstream tasks require content that no task clearly promises to produce.

Addenda A-05 through A-07 further constrain the remedy: the first permanent GUI deliverable is a focused authoritative health/mana HUD; inventory and equipment follow as separate deliverable milestones. Inventory/equipment do not wait for monster drops or world items, and the first equipment proof requires no complete loadout, final slot taxonomy, or equipment effect.

Recommendation:

- Refocus one task on progression inputs: XP curve, reward ranges, cap, and integer rules.
- Give the two or three provisional Inventory development item definitions a focused content owner without creating a starter loadout.
- Give exact deterministic Draft 0 drop-table inputs and item rewards a focused content owner before `GAME-0004`.
- Keep the first Ranger item family and item-to-presentation mapping behind the completed Equipment system.
- Rewire `GAME-0002`, `GAME-0003`, `GAME-0004`, and `GAME-0005` to the exact content contracts they consume.
- Do not require final balance; all values remain deterministic Balance Lab inputs.

### F-04 — Basic Arrow presentation is delayed by later features

Severity: high, vertical-slice completion

Three task boundaries delay the first action beyond its own slice:

- `CLIENT-0018` combines Basic and Fire Arrow projectiles, so a visible Basic projectile waits for Fire Arrow.
- `CLIENT-0019` waits for Basic, Fire, and Arrow Rain before showing health, targeting, rejection, defeat, and respawn feedback.
- `CLIENT-0007` promises hit reactions and death, but `CONTENT-0011`/`ASSET-0004` select and cook only idle, locomotion, notch, release, and aim inputs.

Recommendation:

- Split Basic Arrow projectile presentation from the later Fire Arrow visual enhancement.
- Split Basic Combat diagnostics from later mana, skill-readiness, and Arrow Rain targeting feedback. Basic uses an ImGui Combat window for target health, `300` internal / `3` displayed damage, accepted/rejected/cancelled result, and monster-death outcome.
- Keep permanent floating damage numbers, target HUD treatment, and player defeat/respawn outside Basic Arrow. The game view needs the visual arrow, hit flash, and monster death.
- Narrow `CLIENT-0007` to the selected archer locomotion and technical bow-body action sequence, or explicitly add exact hit/death clip selection and acquisition. Prefer narrowing until those clips are genuinely needed.
- Keep selected monster hit flash and death on the already-proven placeholder presentation path.

### F-05 — Mana and shared action-state ownership is ambiguous

Severity: medium, authoritative contract

`SIM-0009` and `SIM-0007` are both ready. Fire Arrow explicitly owns mana capacity, regeneration, cost, cadence, interruption, windup, and resolve inputs. Arrow Rain also validates mana and action state but does not depend on the Fire task or another shared resource/action contract.

Independent execution could produce overlapping or incompatible player resource/action state.

Addenda A-02 and A-10 supersede the ownership/order alternative below: Mana is an independent end-to-end milestone, while Basic Arrow is the canonical starting point for shared combat-action lifecycle behavior.

Recommendation:

- Prefer Fire Arrow as the first mana-consuming vertical slice because it reuses Basic Arrow's selected target.
- Remove mana-system ownership from `SIM-0009` and `SIM-0007`; both consume the completed Mana contract.
- Have Fire Arrow reuse the Basic Arrow action lifecycle directly or place one focused extraction/refactor ahead of Fire-specific behavior when source evidence requires it.
- Make Arrow Rain consume the action contract proven by Basic and its first second consumer.
- Freeze exact provisional values during each task's owner-approved plan; do not leave two tasks free to invent the same state independently.

### F-06 — Runtime text and game-UI capability has no owner

Severity: medium, missing prerequisite

`CLIENT-0019`, `CLIENT-0015`, and `CLIENT-0014` require readable health, mana, targeting, XP, level, inventory, and equipment feedback. Starfall.Client currently has no text-rendering or game-UI dependency. The shared ImGui backend is editor-only and must not become the game HUD.

Addendum A-03 refines this boundary: the shared ImGui backend may be adopted by Starfall.Client as a development-only debug GUI, while remaining prohibited as the permanent player-facing HUD or gameplay UI foundation.

Addendum A-05 establishes the first permanent player-facing GUI deliverable as a focused authoritative health/mana resource HUD, not a general GUI framework. Addendum A-08 establishes one shared non-stable development-command envelope consumed by both typed ImGui actions and the console frontend.

Recommendation:

- Before the first resource HUD task activates, plan the smallest viewport-space text/rendering prerequisite.
- ChronoFall may own that narrow reusable primitive after the need is demonstrated; Starfall owns HUD composition, layout, styling, controls, and product behavior.
- Discard rather than migrate Royale's existing text system. It is evidence, not an implementation dependency or mandated shared base.
- Let the authoritative health/mana resource HUD establish only text, simple images/panels, bars or values, DPI scaling, and basic layout. Add controls, focus, selection, containers, and richer UI states with the first feature that actually needs them.
- Split `CLIENT-0014` if necessary into inventory viewing/selection and equipment interaction; it currently combines a substantial UI surface with multiple command/reconciliation paths.
- Decide whether damage numbers belong to Basic combat feedback. The Draft 0 wording calls damage “displayed,” but no task explicitly freezes numeric damage-pop presentation.

### F-07 — Presentation smoothing has no task

Severity: medium, known technical debt

The connected client intentionally presents the latest authoritative player and monster snapshots without interpolation. Native testing found no objectionable 60 Hz stepping on the current display, so deferral was correct. However, no task records the later evidence-driven movement-quality work.

Revised Addendum A-09 supersedes the narrow recommendation below. `Connected Movement Quality v1` is a completable deliverable; a broader movement-quality initiative may remain milestone-free and lowest-priority.

Recommendation:

- Establish `Connected Movement Quality v1` around buffering, interpolation, correction diagnostics, representative fixtures, deterministic tests, and macOS native comparison.
- Add prediction/reconciliation to v1 only if its evidence makes them necessary; otherwise defer them to a later versioned deliverable.
- Preserve World authority and keep rendered/speculative transforms out of Simulation.

### F-08 — Balance Lab infrastructure is ready before its first useful scenario

Severity: medium, ordering

`EDITOR-0004` is dependency-ready but produces only a harness. Its consumers wait on Fire Arrow, Arrow Rain, monster behavior, player life, and later progression/equipment rules.

Recommendation:

- Keep the task, but give it priority none or an explicit scheduling gate until `EDITOR-0005` has enough proven rules to justify immediate use.
- Build the harness immediately before its first real scenario, not as free-standing infrastructure.
- Do not add an artificial dependency merely to hide readiness; encode the scheduling gate honestly.

### F-09 — Several deferred tasks are initiatives rather than executable tasks

Severity: medium, future scope

Tasks needing re-grooming before activation include:

- `COORD-0005`: typed authoring objects, serialization, inspectors, validation, gizmos, icons, debug drawing, and cooking.
- `SHARED-0007`: weapons, shields, backpacks, wings, and “other” attachments.
- `SHARED-0013`: skeleton, socket, equipment, and IK debugging.
- `SHARED-0014`: reusable preview/contact-sheet tooling.
- `EDITOR-0007`: real document, hierarchy, picking, transforms, inspectors, commands, validation, and two compilers.
- `EDITOR-0010`: Assets, Validation, Log, status, persistence validation, and navigation.
- `CLIENT-0008`: wings, mounts, and companions.
- `COORD-0014`: scanner, trusted policy, staged-tree scanning, outgoing-history scanning, command contract, and broad Git fixtures.

Addendum A-09 establishes the scheduling rule: broad initiatives remain unassigned to milestones and at the project's lowest priority. Only focused executable tasks derived from them receive the milestone of the concrete deliverable they advance.

Recommendation:

- Keep them as roadmap placeholders while blocked/deferred.
- Do not activate them as written.
- Split them only when a concrete consumer and current architecture reveal the real implementation boundaries.
- Move `COORD-0005` to no milestone/priority none or replace its broad wording with a specific evidence-gated exploration before it can be recommended.

### F-10 — Some dependency fan-in is documentary rather than operational

Severity: low, graph maintainability

Several tasks list both a prerequisite and prerequisites already supplied transitively through it. Some direct edges are legitimate because a task directly consumes that contract. Others appear to restate history or make readiness unnecessarily brittle.

Examples worth reviewing during grooming:

- `CLIENT-0019` has nine direct dependencies, many already represented through `CLIENT-0027` and `CLIENT-0028`.
- `EDITOR-0007` has eight dependencies, with content and shared cooking repeated through `ASSET-0007` and `CONTENT-0014`.
- `CLIENT-0011` has stale grip/reference dependencies even though the corrected boundary gives `SHARED-0020` only a harness-local technical socket and makes the Client task own Starfall's provisional semantic hand socket and local bow transform.
- acquisition tasks often list both stable staging and a cooking task that already depends on staging.

Recommendation:

- Keep direct edges for contracts the implementation genuinely consumes or for independently necessary end-to-end behavior.
- Remove edges that serve only as bibliography; cite historical evidence in task notes/wiki instead.
- Do not mechanically minimize the graph based only on transitive reachability.

### F-11 — Asset acquisition order is mostly sound, with two clarification points

Severity: low, contract clarity

The selection → coordinator acquisition → client integration boundary is correct. `ASSET-0004`, `ASSET-0006`, `ASSET-0007`, and `ASSET-0008` are appropriately evidence-gated.

Clarifications needed:

- Explain how `SHARED-0003` proves modular armour from Starfall selection evidence before `ASSET-0005` stages the exact Ranger inputs, because `ASSET-0005` currently waits on `SHARED-0004`, which itself waits on `SHARED-0003`.
- After `CONTENT-0015` selects cursor/marker files, allocate the focused coordinator acquisition task and attach its canonical dependency before `CLIENT-0025` or `CLIENT-0026` activates. The absent ID is intentional today, not a defect.

### F-12 — The first connected-combat client task needs explicit native acceptance

Severity: low, validation completeness

`CLIENT-0012` describes the correct behavior but should explicitly require a native connected proof:

- start the real World host and Client;
- right-click a live connected monster;
- show an accepted Basic Arrow through authoritative health change/hit flash;
- defeat a light monster in three resolved hits and a heavy monster in seven;
- observe one rejection or cancellation path without local damage prediction;
- confirm movement and existing monster snapshots still work.

This belongs in `CLIENT-0012` acceptance and task notes, not in a separate ceremony task.

## Recommended execution order

### 1. Finish the connected behavior stage of the Basic Arrow milestone

```text
PROTOCOL-0006  define facts
  -> PROTOCOL-0007  serialize facts
  -> SERVER-0008  bind admitted actor and exchange outcomes
  -> CLIENT-0012  right-click connected target and send intent
  -> connected behavior validation
```

Do not mark the milestone complete after this stage. Do not divert into Fire Arrow, Arrow Rain, editor, asset replacement, Balance Lab, cursor polish, or Pressure Cooker before the complete Basic Arrow milestone works end to end.

### 2. Complete Development Instrumentation

Adopt the shared ImGui backend in Starfall.Client, establish the menu/F12/hidden-at-launch/input-capture shell, add the single authoritative development-command envelope and dispatcher, and prove one `Ping World` command through both a typed button and the console. This milestone must complete before Basic Combat diagnostics or Mana Resource diagnostics consume it.

### 3. Close the same Basic Arrow milestone through draft presentation

Recommended branches after the connected proof:

```text
CONTENT-0011
  -> ASSET-0004
  -> narrowed CLIENT-0007 bow-body action presentation

CONTENT-0011
  -> ASSET-0006
  -> SHARED-0020
  -> focused Starfall first-bow/socket presentation proof
     (no equipment, loadout, CONTENT-0009, or GAME-0005 dependency)
  -> Basic-only projectile presentation

Development Instrumentation
  -> Basic Combat diagnostic window
```

These branches should converge in one native proof showing the technical/selected archer holding a bow, firing one client-owned visual arrow from authoritative facts, damaging and defeating the connected placeholder monster, receiving visual hit/death feedback, and inspecting the authoritative outcome in the Combat diagnostics window.

Player defeat/respawn, permanent floating damage numbers, and a permanent target HUD are not part of this milestone.

The exact selected monster model is not required; the existing placeholder monsters are deliberately sufficient.

### 4. Complete Mana independently

Complete authoritative mana state, fixed-tick regeneration/consumption, protocol, World exchange, and the ImGui Resource diagnostic proof through feature-owned commands registered against Development Instrumentation. Mana does not need Fire Arrow, Arrow Rain, the permanent HUD, or player-death policy.

### 5. Complete the permanent player resource HUD

Use completed authoritative health and Mana facts to present player health and mana through the focused permanent HUD. Do not add target status, floating damage, inventory controls, or general GUI-framework behavior.

### 6. Add Fire Arrow as the first extension

Addendum A-02 changes the prerequisite order: the independent mana milestone must be completed before this Fire Arrow extension begins. Fire consumes the proven mana contract; it does not define it.

```text
SIM-0009
  -> PROTOCOL-0011
  -> SERVER-0013
  -> CLIENT-0027
  -> Fire-specific resource/effect presentation
  -> native validation
```

This path consumes the completed Mana contract and the Basic Arrow action lifecycle. Its Plan-mode pass may place one focused action-contract extraction/refactor ahead of Fire-specific behavior if source evidence requires it. Basic and Fire may share the same body animation while arrow/effect presentation distinguishes the outcome.

### 7. Add Arrow Rain from the proven skill seam

```text
groomed SIM-0007
  -> PROTOCOL-0012
  -> SERVER-0014
  -> CLIENT-0028
  -> CLIENT-0010 targeting/effects
  -> native validation
```

`CLIENT-0010` should own ground-target visualization and falling-arrow effects. The later combat HUD should own resources/readiness, not duplicate Arrow Rain world visualization.

### 8. Close Player Defeat and Town Respawn after Mana

This is independent of Fire Arrow and Arrow Rain. Preserve completed simulation evidence, then close the connected player-damage, death-state, configured delay, town-respawn, full-health, provisional full-mana, protocol, exchange, presentation, deterministic-test, and native-monster-defeat path. Development `kill`/`respawn` commands support diagnosis but do not replace the final gameplay proof.

### 9. Complete progression as its own end-to-end loop

```text
focused progression content
  -> GAME-0002
  -> PROTOCOL-0008
  -> SERVER-0009
  -> CLIENT-0015
  -> native kill / XP / level-up proof
```

### 10. Complete Inventory, then its Equipment and Physical Drops consumers

```text
completed resource HUD
  -> focused GAME / PROTOCOL / SERVER / CLIENT inventory proof
     using development-injected items
     |-> focused equipment proof
     |   -> later Ranger visual mapping and presentation
     `-> exact provisional drop content
         -> GAME-0004 physical drops
         -> PROTOCOL-0009 / SERVER-0010 / CLIENT-0013
         -> native kill / drop / collect / inventory proof
```

Equipment and Physical Drops are sibling consumers of Inventory and do not block one another. Do not make the initial rendered bow wait for this lane.

### 11. Use Balance Lab after real rules exist

Implement `EDITOR-0004` immediately before `EDITOR-0005`, then use `EDITOR-0006` only after progression/drop/equipment behavior exists. The first valuable result is a reproducible scenario comparison, not an empty harness.

### 12. Keep editor, proper map, Pressure Cooker, and public wings deferred

- The generated graybox is sufficient for current gameplay.
- Placeholder monsters are sufficient for current combat.
- `.cfskel`/`.cfmesh` are sufficient until a recorded Pressure Cooker trigger exists.
- The Starfall editor should resume when content-authoring friction or a proper-scene requirement is real.
- Wings, mounts, companions, persistence, economy, and final service topology remain outside the current path.

## Unfinished task disposition

The labels mean:

- **Next**: belongs to the immediate connected Basic Arrow chain.
- **Keep**: focused and useful in the stated later order.
- **Groom**: preserve the task, but clarify scope/dependencies before activation.
- **Split**: too much independent behavior for one task/commit.
- **Defer**: valid future work; prevent it from competing with the current path.

### ChronoFall

| Task | Disposition | Assessment |
|---|---|---|
| `SHARED-0015` | Defer | Real low-risk contract debt, but not needed for the current Basic Arrow path. |
| `COORD-0005` | Groom / Defer | Broad future initiative; not executable as written and currently recommendation-eligible. |
| `SHARED-0003` | Keep / Clarify | Focused canonical-rig armour proof; clarify exact selected-input availability. |
| `SHARED-0004` | Keep / Groom | Coherent boundary, but needs exact slot/body-region acceptance before execution. |
| `SHARED-0005` | Defer / Groom | Valid later material variation; no current visual need. |
| `SHARED-0007` | Split later | Multiple attachment categories; preserve `SHARED-0020` as the narrow first proof. |
| `SHARED-0013` | Split or narrow later | Four debug domains with only a one-line contract. |
| `SHARED-0014` | Groom later | Preview tooling needs a concrete consumer and exact workflow. |
| `SHARED-0020` | Keep | Correct narrow shared bow-attachment proof. |
| `ASSET-0004` | Keep | Correct exact archer/bow-animation acquisition seam. |
| `ASSET-0005` | Clarify | Correct ownership, but its relation to the modular-armour proof is circular in intent even if not a PM cycle. |
| `ASSET-0006` | Keep | Correct exact bow/arrow static acquisition seam. |
| `ASSET-0007` | Defer | Correct proper-scene acquisition; generated graybox removes urgency. |
| `ASSET-0008` | Defer | Correct evidence-gated monster acquisition; placeholders remove urgency. |
| `SHARED-0025` | Defer | Well-gated deferred `.cfbundle` specification; activate only on a real trigger. |
| `COORD-0014` | Split/groom on trigger | Valid safety direction but probably too broad for one implementation cycle. |

### Starfall

| Task | Disposition | Assessment |
|---|---|---|
| `PROTOCOL-0006` | **Next** | Correct next task: facts derived from proven Basic Arrow/player-life behavior. |
| `PROTOCOL-0007` | **Next** | Correct serialization follow-up. |
| `SERVER-0008` | **Next** | Correct admitted-session exchange boundary. |
| `CLIENT-0012` | **Next** | Correct first connected player interaction; add explicit native acceptance. |
| `CLIENT-0007` | Groom | Narrow to selected archer/bow-body action or add missing hit/death asset inputs. |
| `SIM-0009` | Groom then Keep | Fire is the correct first mana consumer; remove mana ownership and reuse/refactor the Basic action lifecycle only where evidence requires it. |
| `PROTOCOL-0011` | Keep | Focused Fire extension. |
| `SERVER-0013` | Keep | Focused Fire exchange. |
| `CLIENT-0027` | Keep | Focused Fire input reuse. |
| `SIM-0007` | Groom then Keep | Arrow Rain is focused, but must consume the established mana/action seam. |
| `PROTOCOL-0012` | Keep | Focused Arrow Rain protocol extension. |
| `SERVER-0014` | Keep | Focused Arrow Rain exchange. |
| `CLIENT-0028` | Keep | Focused targeting-mode input. |
| `CLIENT-0010` | Groom | Keep world targeting/effects here; remove overlap with general combat HUD. |
| `CLIENT-0018` | Split | Basic projectile must not wait for Fire presentation. |
| `CLIENT-0019` | Split | Basic health/status feedback must not wait for all three actions. |
| `CONTENT-0008` | Split | Progression, starter items, and exact drop inputs need separate owners. |
| `GAME-0002` | Keep | Focused deterministic XP/level behavior once progression content is exact. |
| `PROTOCOL-0008` | Keep | Appropriate combined facts/serialization for one-way progression. |
| `SERVER-0009` | Keep | Focused progression publication. |
| `CLIENT-0015` | Keep / Clarify | Focused progression feedback; requires runtime text/HUD capability. |
| `GAME-0003` | Keep | Focused item identity/ownership/inventory domain task. |
| `GAME-0004` | Keep / Clarify | Focused physical-drop behavior; add exact drop-table content dependency. |
| `PROTOCOL-0009` | Keep | Appropriate bounded bidirectional drop extension. |
| `SERVER-0010` | Keep | Focused drop exchange. |
| `CLIENT-0013` | Groom | Focused loop, but freeze placeholder representation and pickup interaction. |
| `GAME-0005` | Refocus | Own the bounded first equipment proof after Inventory; remove starter-loadout and stat/effect scope. |
| `PROTOCOL-0010` | Keep | Focused inventory/equipment serialization after domain behavior exists. |
| `SERVER-0011` | Keep | Focused inventory/equipment exchange. |
| `CLIENT-0014` | Split or narrow | Inventory UI, equipment commands, replacement, stats, and reconciliation are too broad together. |
| `CONTENT-0011` | Keep | Correct evidence-only archer/animation/equipment selection task. |
| `CONTENT-0009` | Groom / Defer | Own later item-to-attachment presentation mapping after authoritative Equipment; no starter-loadout role. |
| `CLIENT-0011` | Refocus | Own the provisional Starfall hand socket, local bow transform, rendering, and native placement validation; exclude equipment, aiming, off-hand IK, and generalized grip systems. |
| `CONTENT-0004` | Keep / Defer | Correct Ranger visual mapping after equipment loop and shared armour contracts. |
| `CONTENT-0010` | Defer | Correctly outside the critical path. |
| `CONTENT-0013` | Defer | Exact monster selection is optional while placeholders prove behavior. |
| `CLIENT-0017` | Defer | Correct later replacement of placeholders with selected assets. |
| `CONTENT-0012` | Defer | Proper-scene selection is unnecessary for current graybox gameplay. |
| `EDITOR-0007` | Split before activation | A full authoring document, interaction model, inspectors, commands, validation, and compilation is too large. |
| `SERVER-0012` | Defer | Correct later authoritative adoption of compiled map. |
| `CLIENT-0016` | Defer | Correct later presentation adoption of compiled map. |
| `EDITOR-0004` | Defer until first scenario | Avoid an unused harness. |
| `EDITOR-0005` | Keep later | Coherent combat/camp scenario suite after all three actions exist. |
| `EDITOR-0006` | Groom later | Progression/drop/equipment reporting may need smaller scenario/report slices. |
| `EDITOR-0008` | Defer | Correctly priority none; graybox gameplay remains the priority. |
| `EDITOR-0009` | Defer | Coherent interaction foundation once editor work resumes. |
| `EDITOR-0010` | Split/groom later | Multiple auxiliary surfaces; execute against real needs, not synthetic completeness. |
| `CONTENT-0015` | Defer | Good exact Kenney selection task; no need before Basic Arrow works. |
| `CLIENT-0025` | Defer | Good semantic cursor task after selection/acquisition and Basic target seam. |
| `CLIENT-0026` | Defer | Good movement-marker task after selection/acquisition. |
| `ARCH-0005` | Defer | Correct evidence-gated topology/persistence decision. |
| `CONTENT-0005` | Defer | Future wings/mounts/companions content commitment. |
| `SIM-0005` | Defer | Future authority contract, correctly presentation-independent. |
| `CLIENT-0008` | Split when scheduled | Wings, mounts, and companions are separate product capabilities, not one implementation task. |

## Clarification queue

These decisions should be answered during backlog grooming or the owning task's Plan-mode pass:

1. Closed by A-01: unfinished M2 work moves into smaller deliverable milestones.
2. Which exact picked bow asset and provisional local transform should `CLIENT-0011` validate against Starfall's semantic hand socket without creating equipment content?
3. Which task owns exact Draft 0 item identities and deterministic drop tables?
4. Closed by A-02 and A-10: Mana is independent; Basic is the action-lifecycle starting point; Fire reuses or performs one focused evidence-driven refactor.
5. Closed for Basic by A-01: numeric resolution appears in ImGui Combat diagnostics; permanent damage-number and target-HUD treatment remains undecided.
6. Is `CLIENT-0007` narrowed, or must selection/acquisition include explicit player hit/death clips?
7. What exact narrow viewport-space text/rendering primitive is required by the decided resource HUD, and is ChronoFall ownership justified without migrating Royale's discarded text system?
8. What placeholder representation is acceptable for physical drops before exact presentation assets exist?
9. Closed by A-06 and A-07: Inventory and Equipment are separate deliverable milestones and require separate focused Client ownership.
10. Closed at the manifest level by revised A-09: v1 owns buffering, interpolation, correction diagnostics, fixtures, deterministic tests, and macOS comparison; exact algorithms remain task-plan inputs.
11. How does the shared modular-armour proof access exact selected Ranger evidence before the acquisition/staging task?
12. Which high-fan-in dependency edges are actual consumed contracts versus historical citations?

## Findings checklist

### Maintaining this audit

Work performed because of this audit must be recorded in the companion [backlog audit log](2026-08-05-chronofall-starfall-backlog-audit-log.md). Use one date-only entry per coherent grooming or implementation cycle; an exact time is unnecessary. Record the affected finding/checklist items, owning PM tasks, repositories, resulting commits, validation, and any decision that changed the recommendation.

In the same cycle, change every checklist item that was genuinely completed from `[ ]` to `[x]`. Do not check an item merely because work started, a task was created, or only part of the finding was addressed. If later evidence invalidates an earlier conclusion, preserve the historical log entry and add a new dated entry explaining the revision.

### Immediate backlog repair

- [x] Generate the execution manifest from fresh authoritative family readback with the A-13 supersession crosswalk before PM grooming begins. See the companion execution manifest.
- [x] Decide whether to split unfinished M2 work into outcome milestones or document M2 as an umbrella. Owner decision: split it into deliverable milestones; see Addendum A-01.
- [ ] Preserve `PROTOCOL-0006 -> PROTOCOL-0007 -> SERVER-0008 -> CLIENT-0012` as the next path.
- [ ] Add explicit native end-to-end acceptance to `CLIENT-0012`.
- [x] Decide complete Basic Arrow milestone acceptance. Owner decision: connected authority, bow-body animation, rendered bow/arrow, hit flash, monster damage/death, Combat diagnostics, and native proof; see refined A-01.
- [ ] Split/refocus `CONTENT-0008` into exact progression and item/drop contracts.
- [x] Decide whether Basic Arrow and the first rendered bow require equipment or a starter loadout. Owner decision: they do not; see Addendum A-04.
- [ ] Groom Basic Arrow and first-bow dependencies so no equipment, inventory, loadout, Ranger, `CONTENT-0009`, or `GAME-0005` work blocks them.
- [ ] Move authoritative inventory/equipment and `CONTENT-0009` item-to-attachment mapping into their later post-GUI milestones.
- [ ] Split Basic projectile presentation from Fire Arrow presentation in `CLIENT-0018`.
- [ ] Split Basic combat feedback from later three-skill feedback in `CLIENT-0019`.
- [ ] Resolve the `CLIENT-0007` hit/death clip mismatch.
- [x] Decide mana ownership. Owner decision: mana is an independent end-to-end milestone, not part of Fire Arrow or Arrow Rain; see Addendum A-02.
- [ ] Groom the independent mana milestone and remove mana-system ownership from Fire Arrow and Arrow Rain tasks.
- [ ] Groom the Starfall ImGui debug-GUI adoption and authoritative development-command path described by Addendum A-03.
- [x] Decide Development Instrumentation milestone acceptance. Owner decision: shared ImGui adoption, shell/input behavior, one command envelope, typed and console Ping World, correlated result, macOS/headless validation; see refined A-03/A-08.
- [x] Decide shared combat-action ownership. Owner decision: Basic Arrow is canonical; Fire reuses or performs one focused evidence-driven refactor; see A-10.
- [ ] Groom Fire/Arrow Rain tasks so Mana is an independent dependency and shared combat-action ownership follows A-10.
- [x] Decide player-life ownership. Owner decision: separate Player Defeat and Town Respawn milestone; Basic owns monster death only; see A-11.
- [ ] Groom the Player Defeat and Town Respawn milestone while preserving completed simulation evidence.

### Missing capability owners

- [ ] Plan the smallest viewport-space text/rendering prerequisite for the authoritative health/mana resource HUD.
- [x] Decide the system order. Owner decision: player-facing GUI, then inventory, then equipment are separate deliverable milestones; see A-05 through A-07.
- [x] Decide the first permanent GUI outcome. Owner decision: a read-only authoritative health/mana resource HUD, not a general GUI framework; see refined A-05.
- [ ] Groom the resource-HUD milestone around text, simple panels/images, bars or values, DPI scaling, basic layout, and native authoritative-state validation.
- [x] Decide the first inventory proof. Owner decision: one player inventory, a fixed provisional slot count, two or three development items, insert/move/swap/full-invalid rejection, correction, and visible interaction; see refined A-06.
- [ ] Groom the inventory milestone with that exact proof and keep its domain path independent of drops, world items, and the console frontend.
- [x] Decide the first equipment proof. Owner decision: compatible equip, incompatible rejection, authoritative observation, and unequip; no equipment effects; see refined A-07.
- [ ] Groom the equipment milestone on inventory using provisional slots and development-spawned test items, with stat/effect application excluded.
- [x] Decide the development-console direction. Owner decision: an ImGui console uses a simple development-only server command protocol; see A-08.
- [x] Decide development-command reuse. Owner decision: typed ImGui actions and console text share one envelope, dispatcher, result path, and feature-owned handlers; see refined A-08.
- [ ] Groom the shared development-command boundary, ImGui console frontend, and later inventory-owned `give` validation integration without making inventory depend architecturally on the console.
- [x] Decide Basic damage-number treatment. Owner decision: numeric resolution appears in ImGui Combat diagnostics; no permanent floating damage or target HUD commitment; see refined A-01.
- [x] Decide smoothing/interpolation/prediction/reconciliation ownership. Owner decision: completable Connected Movement Quality v1 plus a broader milestone-free initiative; see revised A-09.
- [ ] Groom Connected Movement Quality v1 around buffering, interpolation, correction diagnostics, representative fixtures, deterministic tests, and macOS comparison.
- [x] Decide how broad initiatives are scheduled. Owner decision: no milestone and lowest priority until decomposed into executable deliverable work; see A-09.
- [ ] Audit current initiative-like tasks and remove misleading milestone assignments while preserving lowest-priority roadmap placeholders.
- [x] Decide Inventory/Equipment/Physical Drops ordering. Owner decision: Inventory precedes sibling Equipment and Physical Drops deliverables; see A-12.
- [ ] Define exact deterministic drop-table/item content before `GAME-0004` activates.
- [ ] Decide the first placeholder physical-drop representation and pickup interaction.
- [ ] Allocate the coordinator Kenney acquisition task only after `CONTENT-0015` selects exact files.

### Scope and dependency hygiene

- [ ] Set `EDITOR-0004` to an honest deferred scheduling gate until its first scenario is ready.
- [ ] Make `COORD-0005` non-recommendable and rewrite it only when a concrete authoring need exists.
- [ ] Mark `SHARED-0007`, `SHARED-0013`, `SHARED-0014`, `EDITOR-0007`, `EDITOR-0010`, and `CLIENT-0008` for re-grooming before activation.
- [ ] Review high-fan-in tasks and remove dependency edges used only as bibliography.
- [ ] Clarify the `SHARED-0003` / `SHARED-0004` / `ASSET-0005` selected-input sequence.
- [ ] Keep Pressure Cooker tasks dormant until a documented activation trigger exists.

### Execution checkpoints

- [ ] Complete and owner-validate connected Basic Arrow before beginning Fire Arrow or Arrow Rain.
- [ ] Complete and owner-validate the independent mana milestone before beginning Fire Arrow or Arrow Rain.
- [ ] Close Basic body animation, bow rendering, projectile, and readable feedback before treating Basic combat as presented.
- [ ] Complete Fire Arrow end to end using the already-proven mana system.
- [ ] Complete Arrow Rain end to end before calling the three-action combat kit proven.
- [ ] Complete XP/level progression as a separate kill-to-reward vertical slice.
- [ ] Complete Inventory using development-injected items before Physical Drops or Equipment consumes it.
- [ ] Complete Equipment and Physical Drops as independent sibling consumers of Inventory.
- [ ] Complete initial bow presentation independently of earned Ranger armour.
- [ ] Build Balance Lab immediately before using it for real scenario evidence.
- [ ] Resume editor/proper-scene work only after gameplay evidence or authoring friction justifies it.

## Recommended next action

Use one owner-approved direct backlog-grooming session—not a meta-task—to address F-02 through F-06 and the related dependency wiring. Then plan and implement `PROTOCOL-0006` as the first feature task.

Do not combine backlog repair with implementation. The grooming commit should change only Starfall PM/wiki state (plus separately planned coordinator PM work where shared prerequisites are allocated), validate the family graph, and stop before feature activation.

Before that grooming cycle, generate the execution manifest from fresh authoritative family readback. It must include the A-13 supersession crosswalk, distinguish executable tasks from milestone-free `priority: none` roadmap placeholders, and treat addendum-resolved clarification questions as closed.
