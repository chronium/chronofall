# ChronoFall and Starfall Backlog Audit Log

Companion to the [ChronoFall and Starfall Backlog Audit](2026-08-05-chronofall-starfall-backlog-audit.md), its [owner-decision addendum](2026-08-05-chronofall-starfall-backlog-audit-addendum.md), and its [execution manifest](2026-08-05-chronofall-starfall-backlog-execution-manifest.md). The four files form one work entity.

Use this file to preserve date-only records of grooming and implementation performed because of the audit. Each entry should identify:

- the affected finding and checklist items;
- the owning PM task or approved direct-grooming cycle;
- the repositories changed;
- the resulting commits and child-pointer handoff where applicable;
- validation and owner-visible evidence;
- remaining work or a reason the audit recommendation changed.

Update the audit checklist from `[ ]` to `[x]` in the same cycle for every item that was genuinely completed. Never mark partial, merely scheduled, or task-created work as complete.

## Entries

### 2026-08-06 — Correct remaining Cycle 3 roadmap wording

- Finding/checklist: corrected two coordinator roadmap passages that still described completed Cycle 3 canonical wiring as future work. No checklist state changed.
- Durable documentation: `roadmap/initial-family-roadmap` and `roadmap/starfall-draft-0-shared-enablers` now record that Cycle 3 attached `SHARED-0026` to Starfall `CLIENT-0029`.
- Readiness boundary: both pages preserve that `CLIENT-0029` remains valid but waiting and cannot activate or consume shared source until todo `SHARED-0026` completes and the child task receives its own approved implementation plan.
- Scope: coordinator wiki and this audit log only. No task, dependency, priority, milestone, feature state, source, asset, child repository, or gitlink changed.

### 2026-08-06 — Complete Cycle 3 Starfall canonical wiring

- Finding/checklist: completed the approved Starfall canonical-wiring cycle and closed the shared development-command, ImGui console and later inventory-owned `give` validation grooming item. No implementation item was marked complete.
- Canonical dependency: added `pm://project/prj_E7QP3LUocfY7k3PYM-EQOlqc/task/SHARED-0026` to Starfall `CLIENT-0029`, preserved completed local prerequisite `CLIENT-0020`, and cleared the temporary `priority: none` override.
- Readiness: `CLIENT-0029` now inherits M4 medium priority and is valid but waiting on todo `SHARED-0026`. `CLIENT-0030` remains transitively blocked, no task is active, and `PROTOCOL-0006` remains Starfall's high-priority dependency-ready feature recommendation.
- Durable documentation: updated the Starfall Development Instrumentation roadmap and coordinator family-source-consumption contract from pending Cycle 3 language to the completed canonical wiring and retained implementation gates.
- Repositories/commits: Starfall commit `d765ccb6bdaed01da4c4e0bd511b720ced8458b9`; pointer-only coordinator commit `da595e72b06a374ed28a12ce2091c4684bcbdb8c`.
- Validation: both PM projects passed doctor; all 407 family tasks have no missing, invalid or unavailable dependencies; family warnings are empty; `git diff --check` passed; Royale remained unchanged; and no source, assets, generated content or feature state changed.
- Remaining work: `SHARED-0026` requires its own owner-directed Plan-mode implementation cycle before `CLIENT-0029` can activate. Cycle 3 does not select or implement either task.

### 2026-08-06 — Correct Cycle 2 durable-documentation review findings

- Finding/checklist: corrected three documentation inconsistencies found after Cycle 2 approval. No checklist state changed because the PM graph, milestone structure, and implementation readiness were already correct.
- Bow-animation evidence: recorded `ASSET-0010` and `EXPERIMENT-0014` as complete, preserved the experiment as technical body-animation and release-marker evidence rather than an equipped-bow proof, and clarified that Starfall `CONTENT-0011` cites it without a dependency unless a later implementation plan identifies a concrete consumed artifact.
- Cycle 3 wiring: clarified that Cycle 3 attaches the canonical todo `SHARED-0026` dependency immediately; the Starfall adoption task then remains blocked until `SHARED-0026` completes.
- Acquisition ownership: corrected the exact mapping to `ASSET-0006` bow and arrow, `ASSET-0007` zone, and `ASSET-0008` monster.
- Scope: coordinator wiki and this audit log only. No PM task, state, milestone, priority, dependency, source, asset, child repository, or gitlink changed.

### 2026-08-06 — Complete Cycle 2 coordinator shared-boundary grooming

- Finding/checklist: completed the approved coordinator-only grooming cycle; made `COORD-0005` non-recommendable, completed the shared/child placeholder re-grooming gate, and clarified the modular-armour selected-input sequence.
- Milestones and allocation: created M4 `Starfall.Client Development Instrumentation Boundary`, M5 `Connected Basic Arrow Shared Enablers`, and exactly one new coordinator task, `SHARED-0026`. `CF-VIEWPORT-TEXT` and all later symbolic work remain unallocated.
- Basic Arrow boundary: moved only `ASSET-0004`, `ASSET-0006`, and `SHARED-0020` into M5. `SHARED-0020` now uses a harness-local technical socket; Starfall `CLIENT-0011` retains semantic hand-socket, local-transform, rendering-integration, and native-placement ownership.
- Dependency correction: removed the obsolete canonical Starfall `CONTENT-0011` edges from `SHARED-0003` and `ASSET-0005`. No replacement Ranger task or fabricated canonical dependency was created; the real selection remains an activation-time prerequisite.
- Scheduling correction: removed every unfinished task from legacy M2 and M3. Deferred modular armour, equipment, material variants, broad attachments, debugging, preview, zone, monster, and typed-authoring work is milestone-free and `priority: none`; focused `SHARED-0015` remains explicit low-priority debt. Pressure Cooker tasks remain milestone-free and `priority: none` without mutation.
- Durable documentation: updated the initial roadmap, Draft 0 shared-enabler graph, shared-engine boundary, family-source-consumption contract, and editor roadmap. M2 and M3 are preserved as completed historical buckets; M4 and M5 are concrete deliverables; Cycle 3 remains a separate Starfall canonical-wiring cycle.
- Repository/commit: coordinator commit `59970a601926cbc74376238161927e30b98db3c3`. No source, assets, Starfall, Royale, feature state, generated content, or gitlink changed.
- Validation: coordinator `pm doctor` and MCP project validation passed; all 407 family tasks have no missing, invalid, unavailable, or cyclic dependencies; no task is active; M4 contains only ready `SHARED-0026`; M5 contains exactly the three blocked Basic Arrow enablers; M2 and M3 have no unfinished members; linked-family warnings are empty; and `git diff --check` passed.
- Remaining work: Cycle 3 must attach the canonical `pm://project/prj_E7QP3LUocfY7k3PYM-EQOlqc/task/SHARED-0026` dependency to the Starfall adoption task and update matching Starfall roadmap prose. No implementation is authorized by this cycle.

### 2026-08-06 — Correct Cycle 1 dependency and durable-roadmap review findings

- Finding/checklist: completed the focused review continuation without allocating work, activating a feature, or changing implementation source.
- Dependency correction: added `CLIENT-0014` to `CLIENT-0013`, so physical-drop presentation and its terminal collected-item proof cannot become ready before the Inventory Client surface.
- Progression correction: `GAME-0002` now consumes the exact owner-approved level sequence frozen by `CONTENT-0008` and no longer independently mandates half-up arithmetic or its historical sequence.
- Durable roadmap correction: replaced stale Mana, Fire/Rain, Basic projectile/diagnostic, socketed-bow and Editor milestone claims in the Starfall bootstrap; corrected Mana ownership in the Draft 0, World lifecycle and starter-flyer/camp pages; and reframed Draft 0 as the umbrella roadmap outcome with M2 as its completed legacy bucket.
- Repositories/commits: Starfall commit `8c3afb2bfdfb34352cb79555db9ef20e704b0f6a`; pointer-only coordinator commit `077748af19d4f8b8b4cb37c41b98aee101570c1b`.
- Validation: Starfall `pm doctor` passed; 112 tasks have no missing, invalid or unavailable dependencies; no task is active; linked-family warnings are empty; superseded wiki phrases no longer appear; and `git diff --check` passed.

### 2026-08-06 — Complete Cycle 1 Starfall structural grooming

- Finding/checklist: completed the approved direct Starfall grooming cycle for deliverable milestones, Basic Arrow, Mana, Development Instrumentation, Movement Quality v1, deferred placeholders, and the related dependency/scope repairs.
- Milestones: created M4 Development Instrumentation, M5 Connected Basic Arrow, M6 Authoritative Mana, and M7 Connected Movement Quality v1. Preserved M2's name and 31 completed historical members while moving every unfinished task out.
- Allocation: created exactly 14 near-sequence tasks through PM's track-scoped next-ID service: `CLIENT-0029`–`CLIENT-0036`, `CONTENT-0016`, `SIM-0012`, `PROTOCOL-0013`–`PROTOCOL-0014`, and `SERVER-0015`–`SERVER-0016`. No HUD, Fire/Rain presentation, Player Life, Progression replacement, Inventory, Equipment, Physical Drop, Ranger, selected-monster, proper-scene, or Balance Lab replacement graph was allocated.
- Grooming: preserved the high-priority `PROTOCOL-0006 -> PROTOCOL-0007 -> SERVER-0008 -> CLIENT-0012` path; made the Basic contract Basic-only; separated its body, bow, visual projectile, Combat diagnostics, and terminal proof; removed equipment/loadout/Ranger gates; made Mana independent; rewired Fire/Rain to consume Mana; narrowed Progression, Inventory, Equipment, Drops, and Ranger placeholders; and deferred Editor, Balance Lab, proper-scene, selected-monster, and transformation work with no milestone and `priority: none`.
- Durable documentation: updated the Starfall bootstrap, product direction, Draft 0, archer, starter-flyer/camp, World lifecycle, and editor-language pages; added focused Development Instrumentation, Authoritative Mana, and Connected Movement Quality v1 roadmaps.
- Repositories/commits: Starfall commit `a2e4f138bcce206dde4f01d87ce045705a8616e9`; pointer-only coordinator commit `7aaeef57ed6ceb8720ae5018e3e5348dfdb431be` pins that child revision. No source, assets, Royale, feature state, or generated content changed.
- Validation: Starfall `pm doctor` passed; 112 tasks read back with no missing, invalid, or unavailable dependencies; no task is active; M2 has zero unfinished members; family resolution has zero warnings; `git diff --check` passed; Starfall and Royale worktrees were clean at handoff.
- Remaining work: Cycle 2 must allocate the coordinator-owned Starfall Client ImGui-consumption boundary. Cycle 3 then attaches its canonical URI to `CLIENT-0029`. Until then, `CLIENT-0029` remains `priority: none` and must not activate.

### 2026-08-06 — Correct Cycle 1 ownership boundaries after baseline review

- Finding/checklist: corrected one Cycle 1 blocker and two smaller manifest boundaries; no PM grooming or implementation checklist item was marked complete.
- Bow ownership: `SHARED-0020` now uses only a harness-local technical socket transform, while `CLIENT-0011` owns Starfall's provisional semantic hand socket, local bow transform, rendering, and native placement validation. Equipment, aiming, off-hand IK, and generalized grip systems remain excluded.
- Player Life: preserved completed `SIM-0011` evidence that respawn retains player entity and gameplay-session identity; entity replacement is no longer presented as an unresolved implementation choice.
- Debug ownership: coordinator adoption owns the source allowlist, reusable backend/native boundary, lifecycle compatibility, macOS native use, and headless exclusion. `SF-DEV-SHELL` alone owns `--debug-ui-hidden`, `F12`, and visibility behavior.
- Editor deferral: `EDITOR-0007` through `EDITOR-0010` are removed from M2 and set to `priority: none` individually; the manifest does not pre-group them into one future proper-scene deliverable.
- PM/source effect: documentation only. No PM task, state, dependency, wiki, source, child repository, or gitlink changed.

### 2026-08-06 — Bound execution-manifest allocation after review

- Finding/checklist: revised the execution manifest after review; manifest generation remains complete, while no PM grooming or implementation item was checked.
- Allocation boundary: Cycle 1 now allocates only Development Instrumentation, Connected Basic Arrow, Mana and Connected Movement Quality v1. HUD, Fire, Rain, Player Life, Progression, Inventory, Equipment, Physical Drops, Ranger, selected-monster, proper-scene and Balance Lab task shapes remain unallocated planning handles until activation.
- Dependency corrections: removed HUD as a Fire/Rain prerequisite; made Movement Quality independent of Basic; removed health/Mana conventions from Inventory content; made Ranger selection an activation-time prerequisite rather than a fabricated canonical edge; and kept `EXPERIMENT-0014` as evidence unless a concrete artifact dependency is later demonstrated.
- Historical treatment: M2 retains its name and completed membership and will be documented as a legacy planning bucket rather than cosmetically renamed.
- Scope corrections: split Client ImGui adoption from debug-shell behavior; made remote interpolation distinct from local correction diagnostics; narrowed Player Life to completed-behavior/Mana integration; left respawn entity reuse versus replacement open; and made kill/respawn commands optional instrumentation.
- Progression decision: recorded the current half-up sequence and the proposed checked integer-ceiling rule as an explicit `CONTENT-0008` Plan-mode decision rather than silently changing either.
- Canonical files: confirmed that the repository contains only the latest date-prefixed audit, addendum, log and execution-manifest filenames; no unsuffixed older duplicate exists under `docs/reports/`.
- PM/source effect: documentation only. No PM task, state, dependency, wiki, source, child repository, gitlink, commit or push changed.

### 2026-08-06 — Produce the backlog execution manifest

- Finding/checklist: completed the execution-manifest prerequisite under Immediate backlog repair; no PM grooming or implementation item was marked complete.
- Readback: refreshed the authoritative linked family with zero warnings, 16 unfinished coordinator tasks, 54 unfinished Starfall tasks, and stable ownership for all three family members.
- Output: added the companion execution manifest with the A-13 supersession crosswalk, deliverable milestone exits, exact existing-task dispositions, bounded new-task specifications, linked-project grooming cycles, remaining task-owned decisions, and validation/stop gates.
- Workflow: the manifest performs direct backlog grooming rather than creating a task whose purpose is to create tasks. Symbolic handles are not PM identities; real IDs must come from the owning PM next-ID service during an approved cycle.
- PM/source effect: documentation only. No PM task, state, dependency, wiki, source, child repository, gitlink, commit, or push changed.
- Validation: companion links, checklist state, Markdown structure, repository diff, and `git diff --check` reviewed; Starfall and Royale remained clean.

### 2026-08-06 — Milestones are deliverables

- Finding/checklist: resolved the decision branch in F-01 and checked the first Immediate backlog repair item.
- Owner decision: a milestone is an independently demonstrable deliverable, not an epic, project bucket, iteration, or development cycle.
- Direction: split Starfall's mile-long M2. The complete connected Basic Arrow vertical slice is the model for one milestone because it includes configuration/content inputs, protocol, authoritative simulation, World exchange/transport, connected Client intent, draft presentation, tests, and native validation while producing one coherent player-visible result.
- PM/source effect: none yet. Exact milestone allocation and task reassignment require a later approved backlog-grooming cycle.
- Durable record: Addendum A-01.

### 2026-08-06 — Mana is an independent milestone

- Finding/checklist: resolved the ownership branch in F-05 and checked the mana-ownership decision item.
- Owner decision: mana is a complete end-to-end deliverable and must not be owned by Fire Arrow or Arrow Rain.
- Direction: the milestone covers explicit inputs, authoritative state and fixed-tick regeneration/consumption, protocol, World exchange, a Starfall debug presentation, automated tests, and native validation before either spell consumes it.
- Test seam: development-only authoritative mana commands allow consume, empty, and refill scenarios without inventing a spell.
- PM/source effect: none yet. A later grooming cycle must allocate the milestone/tasks and remove overlapping ownership from the existing spell tasks.
- Durable record: Addendum A-02.

### 2026-08-06 — ImGui is the Starfall debug GUI

- Finding/checklist: refined F-06 and added an explicit grooming item for debug-GUI adoption and authoritative development commands.
- Owner decision: Starfall.Client will use ImGui for development diagnostics, organized into concern-specific windows rather than one Royale-style data dump.
- Direction: a menu bar controls window visibility, tabs may group coherent sub-concerns, F12 toggles the complete GUI, and a launch argument starts it hidden for clean screenshots.
- Authority: debug buttons request explicitly gated World-owned development actions; they never mutate gameplay state locally.
- Boundary: the debug GUI is not the permanent player HUD and remains absent from headless products.
- PM/source effect: none yet. Adoption requires later coordinator/Starfall architecture and task grooming because the shared backend currently allows only the native Starfall editor host.
- Durable record: Addendum A-03.

### 2026-08-06 — Tighten milestone and mana boundaries

- Addendum A-01: clarified that demonstrable milestones may depend on completed deliverables; each must contribute its own observable outcome.
- Basic Arrow: removed ownership of monster replenishment. It may exercise the already-established camp-lifecycle behavior, but death is the attack milestone's terminal authoritative outcome.
- Addendum A-02: made death/respawn policy a later player-life consumer of a clean mana life-cycle seam rather than a mana completion gate.
- Development commands: explicitly carry no stable gameplay-protocol compatibility promise despite being durable, tested engineering instrumentation.
- PM/source/checklist effect: none. These changes refine already-recorded decisions and do not complete an implementation checklist item.

### 2026-08-06 — Basic Arrow and first bow are equipment-free

- Finding/checklist: superseded F-02's proposed starter-loadout prerequisite and checked the corresponding ownership decision.
- Owner decision: Basic Arrow and the first rendered bow do not depend on items, inventory, equipment slots, starter/Ranger loadouts, drops, or world items.
- Direction: pick and render one exact bow through the socket/static-rendering path as a presentation proof. Actual item-to-attachment mapping follows the later equipment system.
- PM/source effect: none yet. Later grooming must remove the current `GAME-0005`/`CONTENT-0009` gates from the first bow path.
- Durable record: Addendum A-04.

### 2026-08-06 — GUI, inventory, and equipment are sequential deliverables

- Finding/checklist: resolved system ordering and added separate unchecked grooming items.
- Owner decision: permanent player-facing GUI is a milestone; inventory is a later milestone built on GUI; equipment is a later milestone built on inventory.
- Direction: inventory/equipment proofs use minimal development item definitions and do not wait for monster drops, world items, complete loadouts, Ranger content, or final assets.
- Boundary: ImGui remains the development debug GUI rather than the permanent player-facing GUI.
- PM/source effect: none yet. Existing combined content/game/protocol/server/client tasks require a later approved split and dependency rewrite.
- Durable record: Addenda A-05, A-06, and A-07.

### 2026-08-06 — Add an ImGui development console

- Finding/checklist: resolved the console direction and added an unchecked console-grooming item.
- Owner decision: add a concern-specific ImGui console backed by a simple development-only World command protocol, with no permission/admin system.
- Direction: the base console proves bounded request/response, parsing, dispatch, history, diagnostics, and input capture. Inventory later registers `give <item-id> [quantity]` to inject authoritative test items.
- Boundary: commands carry no stable gameplay compatibility promise and cannot execute arbitrary shell/filesystem operations.
- PM/source effect: none yet.
- Durable record: Addendum A-08.

### 2026-08-06 — Narrow GUI, inventory, equipment, and development commands

- Finding/checklist: refined F-03 and F-06 and checked four owner-decision items; no grooming or implementation item was marked complete.
- GUI decision: the first permanent player-facing GUI deliverable is the authoritative health/mana resource HUD, limited to viewport text, simple images/panels, bars or values, DPI scaling, basic layout, and native validation. Focus, selection, containers, richer controls, and rejection states wait for a concrete consumer.
- Ownership: ChronoFall may own a narrow reusable viewport-space text/rendering primitive after focused review; Starfall owns HUD composition and behavior. Royale's existing text system is discarded rather than migrated.
- Development tooling: typed ImGui actions and console text share one development-command envelope, dispatcher, result path, diagnostics, and feature-owned handlers. Mana does not create a parallel development protocol.
- Inventory decision: the first proof has one player inventory, one fixed provisional slot count, two or three development items, insert/move/swap/full-invalid rejection, authoritative correction, and visible interaction. Only native item injection may consume the console frontend.
- Equipment decision: the first proof equips a compatible item, rejects an incompatible move, exposes authoritative equipped state, and moves the item back. No statistic or gameplay effect belongs to this deliverable.
- PM/source effect: none. These decisions constrain later grooming and implementation plans.
- Durable record: refined Addenda A-02, A-03, and A-05 through A-08.

### 2026-08-06 — World-client movement quality and initiative scheduling

- Finding/checklist: superseded F-07's single smoothing-task recommendation and checked the movement-quality ownership and initiative-scheduling decisions; grooming and implementation remain unchecked.
- Owner decision: smoothing, interpolation, prediction, and reconciliation form one ongoing world-client movement-quality deliverable milestone.
- Boundary: World and fixed-step simulation remain authoritative. Smoothing/interpolation are presentation, prediction is speculative, and reconciliation corrects presentation from authoritative facts without feeding rendered transforms into Simulation.
- Execution rule: each task remains focused and evidence-gated. Start with the smallest demonstrated smoothing/interpolation need; do not hide prediction or reconciliation inside it.
- Initiative rule: broad non-executable initiatives remain unassigned to milestones and at the project's lowest priority. Only derived executable tasks receive a concrete deliverable milestone.
- PM/source effect: none. Later approved grooming must allocate the milestone, define its first task, and audit existing initiative-like assignments.
- Durable record: Addendum A-09.

### 2026-08-06 — Close execution-manifest decision gaps

- Finding/checklist: closed Basic milestone acceptance, diagnostic damage treatment, Development Instrumentation acceptance, shared combat-action ownership, player-life ownership, Inventory/Equipment/Physical Drops ordering, and versioned movement-quality semantics. No grooming or implementation item was marked complete.
- Basic Arrow: completion now requires the connected authoritative path, bow-body animation, rendered bow/arrow, hit flash, monster damage/death, ImGui Combat diagnostics, and native end-to-end proof. Numeric resolution is diagnostic evidence, not a floating-combat-text commitment.
- Instrumentation/HUD: Development Instrumentation is a separate milestone with shared ImGui adoption, one command envelope/dispatcher, typed and console `Ping World`, correlated result, hide/show behavior, macOS validation, and headless isolation. Basic and Mana consume its diagnostic surfaces; the permanent health/mana HUD follows completed Mana.
- Combat action state: Basic Arrow is canonical. Fire reuses it or performs one focused source-evidenced refactor before Fire behavior; Arrow Rain consumes the established result. Mana and movement-interruption policy remain separate.
- Player life: Player Defeat and Town Respawn is a later milestone. Basic owns monster death only; development kill/respawn commands do not replace a native monster-defeats-player proof.
- Inventory ordering: Inventory precedes sibling Equipment and Physical Drops deliverables. Development injection proves Inventory; the complete physical-drop proof consumes Inventory and exact provisional drop content.
- Movement quality: replaced the earlier ongoing milestone with completable `Connected Movement Quality v1`; prediction/reconciliation enter only if v1 evidence requires them, otherwise they wait for a later versioned deliverable.
- Manifest rule: broad placeholders remain milestone-free with `priority: none`; the future execution manifest must contain the A-13 supersession crosswalk and treat resolved clarification questions as closed.
- PM/source effect: none. These decisions prepare—but do not authorize—the future manifest or PM grooming cycles.
- Durable record: refined A-01, A-03, A-05, A-08, and A-09; added A-10 through A-13.
