# ChronoFall and Starfall Backlog Execution Manifest

Prepared: 2026-08-06

Status: planning artifact; PM execution is not authorized by this file

This manifest is part of one work entity with the [backlog audit](2026-08-05-chronofall-starfall-backlog-audit.md), [owner-decision addendum](2026-08-05-chronofall-starfall-backlog-audit-addendum.md), and [audit log](2026-08-05-chronofall-starfall-backlog-audit-log.md). The addendum has precedence where it refines the original audit. The audit log records every later grooming or implementation cycle attributable to this work.

The manifest translates the approved decisions into direct PM grooming and feature-execution work. It is intentionally not a task whose purpose is to create other tasks. Exact new PM IDs are allocated only during an owner-approved grooming cycle through PM's next-ID service. The symbolic names below are planning handles, not persistent PM identities.

## Authoritative planning snapshot

Fresh PM MCP readback on 2026-08-06 reported:

| Project | Stable project ID | Revision | Unfinished tasks | Resolution |
| --- | --- | --- | ---: | --- |
| ChronoFall | `prj_E7QP3LUocfY7k3PYM-EQOlqc` | `6279afd898b9dcdbbce2852a66bf198b866d2b50` | 16 | current, readable, write-trusted |
| Royale | `prj__-jXLQgm6GuD2gCKZ_bTa1m-` | `3b1bc45e4c8be76d110d8cf9613284db342db42e` | not in this manifest | child, readable, write-trusted |
| Starfall | `prj_pkIpzx0fzFD4URjvqBuYrGZF` | `96a42b250b7018bb1d19c7baa24c07eeea9757bd` | 54 | child, readable, write-trusted |

The family had zero resolution warnings. Starfall and Royale were clean. ChronoFall contained only the untracked audit work entity under `docs/reports/`; no child gitlink was changed.

This snapshot is evidence for the manifest, not a permanent readiness claim. Every later grooming or implementation cycle must refresh the family, task, dependency, trust, and worktree state.

## Execution rules

1. A milestone is a completable, independently demonstrable deliverable. It may depend on earlier deliverables; it must add an observable outcome of its own.
2. Completed tasks remain historical evidence. Do not rewrite their scope or move them merely to make old milestones look tidy.
3. Existing unfinished tasks may be narrowed, split, retitled, reassigned, or have dependencies corrected before activation. No feature task is activated during structural grooming.
4. New tasks are created directly during grooming. Do not create a grooming task whose deliverable is merely another list of tasks.
5. A dependency is an implementation prerequisite, not a citation. Historical evidence belongs in notes and wiki prose.
6. Broad non-executable ideas have no milestone and `priority: none`. They are roadmap placeholders, not implementation tasks. Focused work is derived from them only when an owner-approved deliverable activates.
7. Cross-project prerequisites use canonical PM task URIs after the owning task exists. No unavailable coordinator ID is invented in a Starfall cycle.
8. Each implementation task still receives its own owner-directed Plan-mode pass, activation, focused commit, validation, completion, and stop. This manifest does not pre-approve implementation details.
9. Starfall remains authoritative for its simulation, protocol, content, World exchange, and product presentation. ChronoFall owns only demonstrated reusable source, cooking, rendering, and family-consumption boundaries.
10. Pressure Cooker, the proper Editor-authored scene, public wings, persistence, economy, trade, crafting, PvP, and final deployment topology remain outside the active path.

## Required supersession crosswalk

| Earlier backlog implication | Manifest rule |
| --- | --- |
| Fire Arrow or Arrow Rain owns mana | Mana is its own end-to-end deliverable. Skills consume its completed state, facts, exchange, and diagnostics. |
| Fire Arrow implicitly owns shared combat-action state | Completed Basic Arrow behavior is the canonical starting point. Fire reuses it or performs one focused, source-evidenced extraction; Arrow Rain consumes the established result. |
| Physical drops precede inventory | Inventory completes first with development-injected items. Equipment and Physical Drops are sibling consumers of Inventory. |
| Basic Arrow or the first bow requires a starter/Ranger loadout | Basic Arrow and its first rendered bow are equipment-free presentation proofs. Ranger equipment follows Inventory and Equipment. |
| Basic Arrow presentation owns player respawn | Basic Arrow owns monster damage and death. Player Defeat and Town Respawn is a separate later deliverable. |
| Movement quality is an ongoing milestone | `Connected Movement Quality v1` is completable. A broader initiative remains milestone-free; prediction/reconciliation enter v1 only if evidence requires them. |
| Resolved audit questions remain open planning choices | The decisions in A-01 through A-13 are closed inputs. Only the explicit decision gates in this manifest remain open. |

## Milestone restructuring

### Existing milestone treatment

- Starfall M0 and M1 remain completed historical foundation milestones.
- Starfall M2 retains its historical name and completed membership. After unfinished work moves out, roadmap prose marks it as a legacy planning bucket superseded by the deliverable-milestone model. Do not rename it to imply that its graybox, movement, monsters, camp lifecycle, combat simulation, and player-life simulation were one coherent deliverable.
- Starfall M3's unfinished transformation/mount/companion placeholders move to no milestone with `priority: none`. If M3 becomes empty, remove or retire the configuration only through supported child-context PM tooling.
- ChronoFall's completed milestone history remains unchanged. Unfinished shared work moves only where its current milestone falsely implies an active deliverable.

### Cycle 1 milestone allocation

Cycle 1 creates only the four deliverables entering the near execution sequence. Exact PM keys are allocated during that approved grooming cycle.

| Planning handle | Proposed name | Priority | Observable exit |
| --- | --- | --- | --- |
| `SF-M-DEV` | Development Instrumentation | medium | The real Client opens concern-specific ImGui windows, sends `Ping World` through typed and console frontends using one gated development-command path, receives a correlated authoritative result, and can hide all debug UI without affecting gameplay. |
| `SF-M-BASIC` | Connected Basic Arrow | medium | A connected player targets a real connected placeholder monster, World resolves Basic Arrow, and the Client shows bow-body animation, one rendered bow and visual arrow, hit flash, monster damage/death, exact Combat diagnostics, and one owner-validated native run. The already-selected connected chain receives explicit high task priority because it is the immediate path. |
| `SF-M-MANA` | Authoritative Mana | medium | Configured integer mana initializes, consumes, clamps, regenerates, restores, serializes, exchanges, and is proven through authoritative development commands and Resource diagnostics before any spell owns it. |
| `SF-M-MOVE-V1` | Connected Movement Quality v1 | low | Non-local accepted snapshots are buffered/interpolated; local-player corrections are diagnosable without automatically adding interpolation delay; deterministic latency/loss/reordering fixtures pass; and macOS before/after validation shows the bounded quality result. |

Movement Quality v1 consumes the already-completed connected snapshot adapters and can proceed independently of Basic Arrow. It does not become a Basic prerequisite or a combat blocker.

### Symbolic future deliverables

These outcomes remain useful planning specifications, but Cycle 1 does not create their milestones or allocate their new tasks. Existing oversized tasks may be narrowed and made milestone-free with `priority: none`; their replacement task graph is allocated only when the owner intentionally activates that deliverable.

| Planning handle | Proposed name | Observable exit |
| --- | --- | --- |
| `SF-M-HUD` | Player Resource HUD | The native Client presents real authoritative player health and mana through a focused permanent viewport HUD with DPI-aware layout; ImGui is not used for the permanent HUD. |
| `SF-M-FIRE` | Fire Arrow | Key `1` sends Fire Arrow intent, authoritative mana and action state decide it, `700` internal damage resolves, and the Client distinguishes the result visually while reusing the Basic physical release animation. |
| `SF-M-RAIN` | Arrow Rain | Key `2` enters ground targeting, World resolves an ordered victim set for `500` internal damage each, and client-owned falling arrows/effects present the authoritative outcome. |
| `SF-M-LIFE` | Player Defeat and Town Respawn | A monster defeats the admitted player; after authoritative respawn resolution the same admitted player entity and gameplay-session identity return to the town anchor with restored resources, preserving the completed `SIM-0011` contract. |
| `SF-M-PROGRESSION` | XP and Level Progression | Authoritative monster kills award deterministic XP, levels advance through the approved level-20 curve, and the permanent Client UI presents current progress, awards, corrections, and level-up feedback. |
| `SF-M-INVENTORY` | Authoritative Player Inventory | One provisional fixed-slot inventory supports two or three development items, insert/move/swap, full/invalid rejection, correction, visible GUI interaction, and development injection without drops. |
| `SF-M-EQUIPMENT` | Authoritative Equipment State | One compatible item moves from inventory into one provisional equipment slot and back; an incompatible move is rejected. No statistics, effects, Ranger content, or on-character rendering is included. |
| `SF-M-DROPS` | Physical Drops and Collection | An authoritative monster death creates an exact provisional physical drop; the Client presents and collects it once, and the item appears in the completed inventory. |

The dependency structure is known without allocating it prematurely: Basic Arrow and Mana enable Fire; the authoritative action contract proven reusable by Fire enables Arrow Rain; completed health and Mana plus the viewport renderer enable the Resource HUD; Player Life consumes completed player-life behavior and Mana's lifecycle seam; Progression consumes Basic monster death; Inventory consumes only the permanent GUI rendering foundation on its Client side; Equipment and Physical Drops are sibling consumers of Inventory.

### Deferred deliverables

The following are legitimate outcomes but are not assigned active implementation milestones by this manifest:

- selected monster presentation;
- proper Editor-authored Draft 0 scene and runtime adoption;
- Ranger/modular-equipment presentation;
- pointer-intent cursors and movement-target marker;
- Balance Lab harness and its first real scenario;
- Pressure Cooker and `.cfbundle`;
- public wings, mounts, companions, and transformation work.

Their existing focused tasks remain todo with `priority: none` and no milestone until an owner-approved activation pass establishes the concrete deliverable and exact dependencies.

## Existing Starfall task grooming

### Connected Basic Arrow

| Task | Required semantic change |
| --- | --- |
| `PROTOCOL-0006` | Keep only Basic Arrow command/outcome facts: command sequence and target in the Client request; authoritative actor identity derived by World from the admitted session and present only in authoritative outcome facts; ticks, acceptance/rejection/cancellation, `300` damage, effective damage, and monster defeat. A client never supplies the acting entity. Remove player-health/defeat/respawn facts. Retain `SIM-0011` only as a real prerequisite for defeated/protected-town action rejection, not as player-life transport ownership. |
| `PROTOCOL-0007` | Serialize the narrowed Basic contract only. Remove player-life/respawn payloads. |
| `SERVER-0008` | Exchange Basic commands/outcomes only and continue using the existing monster snapshot path for health/defeat. Remove publication ownership for player health, defeat, and respawn. |
| `CLIENT-0012` | Preserve right-click target selection and the existing chain. Add explicit native Client/World acceptance for valid intent, deterministic target choice, authoritative acceptance/rejection/cancellation, hit flash, health reduction, and connected monster defeat. Do not wait for animation, bow, arrow, ImGui, or permanent HUD work. |
| `CONTENT-0011` | Retitle/refocus to the exact Basic archer-presentation selection: one base/underlayer, one bow, one arrow, and minimum compatible idle/locomotion/notch/release/aim clips. Remove Ranger outfit selection. Cite completed `EXPERIMENT-0014` as visual/architectural evidence and record exact pack-relative paths; do not add a PM dependency unless the implementation plan identifies a concrete artifact the task consumes. |
| `CLIENT-0007` | Retitle/refocus to Basic Arrow bow-body animation and locomotion. Remove player hit/death reaction ownership and documentary dependencies. Consume the connected Basic timing facts, exact `ASSET-0004` cook, and established blending/layering contracts. |
| `CLIENT-0011` | Retitle/refocus to one rendered socketed Basic bow. Own Starfall's provisional semantic hand socket, local bow transform, rendering, and native placement validation. Consume the exact selected/staged bow input and canonical `SHARED-0020`, whose shared proof uses only a harness-local technical socket transform. Remove equipment, `CONTENT-0009`, `GAME-0005`, aiming, off-hand IK, generalized grip systems, and starter-loadout wording. |
| `CLIENT-0018` | Retitle/refocus to Basic Arrow visual projectile and impact. Remove Fire dependencies and Fire presentation. The arrow is client-owned and never decides collision, success, damage, or timing. |
| `CLIENT-0019` | Retitle/refocus to the ImGui Basic Combat diagnostic and terminal native milestone proof. Show target health, `300` internal / `3` displayed damage, accepted/rejected/cancelled result, and monster death. Depend on the completed Development Instrumentation shell plus every Basic presentation leaf; remove Mana, Fire, Rain, player-life, floating-damage, and target-HUD scope. |

Move these tasks into `SF-M-BASIC`. Set the four already-selected connected behavior tasks to explicit `high` priority because they are the immediate executable path; do not normalize unrelated Basic presentation task priorities for cosmetic uniformity. Preserve the chain:

```text
PROTOCOL-0006
  -> PROTOCOL-0007
  -> SERVER-0008
  -> CLIENT-0012
```

The content/acquisition/presentation lane may progress in parallel, but `CLIENT-0019` is the terminal milestone task and cannot complete until the connected behavior, body animation, bow, arrow, feedback, diagnostics, and native owner validation all converge.

### Fire Arrow and Arrow Rain

| Task | Required semantic change |
| --- | --- |
| `SIM-0009` | Remove mana capacity/current-state/regeneration ownership. Consume the completed Mana behavior, retain Fire cost/range/facing/cadence/interruption/windup/resolve inputs, and reuse the Basic action lifecycle. |
| `PROTOCOL-0011` | Carry Fire-specific intent, timing, mana expenditure/outcome, damage, and rejection facts over the proven combat envelope. It does not define mana behavior. |
| `SERVER-0013` | Exchange Fire commands/outcomes through the proven Basic World path and completed Mana integration. |
| `CLIENT-0027` | Keep focused key-`1` Fire intent against the selected valid target. |
| new Fire presentation task | Reuse the Basic bow release/visual-arrow path and distinguish Fire through arrow/effect presentation. Basic and Fire do not require different body animations merely to express different gameplay outcomes. This is the terminal native Fire proof. |
| `SIM-0007` | Remove mana-system ownership; retain Rain cost, target/radius/cadence/interruption/timing/order. Consume completed Mana and the authoritative action contract established by Basic and proven reusable by Fire as its second simulation consumer—not Fire presentation, the permanent HUD, or the complete Fire milestone. |
| `PROTOCOL-0012` | Carry only Rain-specific ground-target, timing, mana expenditure, ordered victim, damage, and rejection facts. |
| `SERVER-0014` | Exchange Rain through the established connected combat host without creating spatial projectile entities. |
| `CLIENT-0028` | Keep key-`2` target mode, valid-point right-click, and cancellation input. |
| `CLIENT-0010` | Remain the terminal Rain targeting/effects/native-validation task. |

Do not add a comprehensive combat-action-system milestone. During the Fire Plan-mode pass, inspect the completed Basic source: either reuse it directly or add one narrowly scoped extraction task with tests before Fire-specific behavior. Do not allocate that refactor speculatively during grooming. Fire and Rain remain milestone-free with `priority: none` after Cycle 1 narrowing; their future milestones and any new presentation task are allocated only when each deliverable activates.

### Progression

| Task | Required semantic change |
| --- | --- |
| `CONTENT-0008` | Retitle/refocus to Draft 0 XP curve and reward inputs only. Remove starter equipment, Ranger, bow, and drop-table ownership. Its Plan-mode pass must explicitly freeze the deterministic 1.15 rounding rule before implementation; the current persisted half-up sequence is Draft 0 evidence, while the newly proposed ceiling rule is `nextRequirement = checked((previousRequirement * 115 + 99) / 100)` and yields `40, 46, 53, 61, 71, 82, 95, 110, 127, 147, 170, 196, 226, 260, 299, 344, 396, 456, 525`. Do not silently change the sequence without owner approval and matching task/wiki/test updates. |
| `GAME-0002` | Keep authoritative XP/level behavior and level-20 cap. |
| `PROTOCOL-0008` | Keep the bounded one-way progression facts/serialization extension. |
| `SERVER-0009` | Keep authoritative progression publication. |
| `CLIENT-0015` | Use the permanent HUD rendering foundation for current XP/requirement, awards, corrections, and level-up feedback. No final art, persistence, equipment, or drops. Own terminal native kill-to-progression proof. |

Cycle 1 narrows these tasks, removes M2 membership, and sets them to `priority: none`; it does not create `SF-M-PROGRESSION`. When Progression activates, its deliverable consumes Basic monster death but does not wait for Fire, Rain, Equipment, or Physical Drops.

### Inventory, Equipment, and Physical Drops

| Task | Required semantic change |
| --- | --- |
| `GAME-0003` | Depend on a new exact development-item/inventory-input task rather than `CONTENT-0008`. Implement one fixed provisional player inventory and item identity/ownership with insert/move/swap/full-invalid rejection and correction. |
| `PROTOCOL-0010` | Retitle/refocus to inventory facts and serialization only. Remove equipment and physical-drop dependencies. |
| `SERVER-0011` | Retitle/refocus to inventory commands/state/corrections only. Remove `SERVER-0010` and equipment dependencies. |
| `CLIENT-0014` | Retitle/refocus to the permanent inventory surface and inventory interaction only. Remove Physical Drops and Equipment dependencies. Add only the container, focus, selection, disabled/rejection, and correction UI needed by this proof. |
| `GAME-0005` | Retitle/refocus to authoritative provisional equipment state. Prove compatible equip, incompatible rejection, replacement/correction as needed, and unequip. Remove starter loadout, Ranger pieces, unlimited-arrow rules, modifiers, and all gameplay effects. |
| `GAME-0004` | Keep physical drop behavior, but add a direct dependency on exact provisional drop-table/item content. Inventory is already a real domain prerequisite. |
| `PROTOCOL-0009` | Keep focused physical-drop facts/serialization. |
| `SERVER-0010` | Keep focused physical-drop exchange. |
| `CLIENT-0013` | Keep focused world-drop presentation/collection and depend on the completed Inventory Client proof so the native acceptance can show the collected item in inventory. |

The future Inventory activation pass may allocate tasks for exact provisional slot count and two or three development items, the inventory-owned `give` handler, and terminal native validation. Only the injection/validation task depends on Development Instrumentation; Inventory simulation, stable protocol, exchange, and GUI do not depend on the console. Cycle 1 does not allocate these tasks.

The future Equipment activation pass may allocate Protocol, Server, and Client tasks that consume completed Inventory and `GAME-0005`. No equipment task renders the item on the character or applies statistics. Cycle 1 does not allocate them.

The future Physical Drops activation pass must allocate exact deterministic drop-table/item content. The first placeholder world representation and pickup gesture remain an explicit Plan-mode choice before `GAME-0004` activates. Cycle 1 allocates neither.

### Later presentation/content tasks

| Task | Disposition |
| --- | --- |
| `CONTENT-0004` | Defer until Inventory and Equipment are complete and exact Ranger inputs exist. It owns Ranger-to-slot/body-region presentation mapping, not equipment-system proof. Remove the refocused `CONTENT-0008` progression dependency; add the real selection dependency only when Ranger presentation activates. |
| `CONTENT-0009` | Defer until Equipment is complete. It maps authoritative item identities to bow/arrow attachment presentation; it is not used by the first equipment-free bow proof. |
| `CONTENT-0010` | Keep milestone-free with `priority: none`; activate only for a specific material/palette need. |
| `CONTENT-0013` + `CLIENT-0017` | Keep as a future selected monster presentation deliverable. `ASSET-0008` remains evidence-gated after selection identifies static, rigid, skeletal, or rejected inputs. |
| `CONTENT-0012`, `SERVER-0012`, `CLIENT-0016` | Remove from the active M2 path and set `priority: none`. Re-groom their future scene-selection and runtime-adoption roles only after gameplay evidence or authoring friction justifies them. |
| `EDITOR-0007`, `EDITOR-0008`, `EDITOR-0009`, `EDITOR-0010` | Remove each task from M2 and set `priority: none`; `EDITOR-0009` loses its misleading `high` priority. Do not pre-group them into one proper-scene deliverable. Re-read and design their eventual deliverables separately from actual editor needs when editor work resumes. |
| `EDITOR-0004`–`EDITOR-0006` | Remove from M2 and set `priority: none`. Build the harness immediately before the first real scenario; split the current combined combat/progression/drop/equipment scenario scopes at that time. |
| `CLIENT-0025`, `CLIENT-0026`, `CONTENT-0015` | Keep milestone-free with `priority: none`. They remain optional feedback improvements and receive the coordinator acquisition dependency only after exact Kenney selection completes. |
| `CONTENT-0005`, `SIM-0005`, `CLIENT-0008` | Remove M3 membership and retain as `priority: none` roadmap placeholders. They do not enter this executable manifest. |
| `ARCH-0005` | Keep milestone-free and `priority: none`; final topology/persistence degradation remains evidence-gated. |

## New Starfall task specifications

The names in the first column are temporary manifest handles, not PM identities.

### Allocate in Cycle 1

| Handle | Track / milestone | Exact prerequisite behavior | Focused acceptance boundary |
| --- | --- | --- | --- |
| `SF-DEV-ADOPTION` | CLIENT / `SF-M-DEV` | completed coordinator Client-consumption boundary for `SHARED-0024`; current native Client shell | Add the exact family-source reference and instantiate the caller-controlled backend in `Starfall.Client`; verify architecture allowlists, Client-only native assets, and headless isolation. Do not design windows, menus, commands, or permanent game UI. |
| `SF-DEV-SHELL` | CLIENT / `SF-M-DEV` | `SF-DEV-ADOPTION` | Provide the compact debug menu, concern-window visibility, `F12`, `--debug-ui-hidden`, input capture/suppression, and minimal in-memory window state. Local layout persistence is excluded from v1 unless it is truly trivial and explicitly approved in this task's plan. |
| `SF-DEV-PROTOCOL` | PROTOCOL / `SF-M-DEV` | established connected envelope/serialization conventions | One development-only command/result envelope with non-zero sequence, bounds, deterministic parse/encode, enablement/rejection facts, and explicit no-compatibility promise. No gameplay commands are defined here. |
| `SF-DEV-SERVER` | SERVER / `SF-M-DEV` | admitted World sessions plus `SF-DEV-PROTOCOL` | Explicit development gate, session binding, common dispatcher, feature-owned handler registration, correlated results, and harmless `Ping World`; no roles/admin/remote operations. |
| `SF-DEV-CONSOLE` | CLIENT / `SF-M-DEV` | `SF-DEV-SHELL`, `SF-DEV-PROTOCOL`, `SF-DEV-SERVER` | Dedicated console with bounded input/history/results plus typed `Ping World` button using the same command representation; native macOS proof and headless isolation. |
| `SF-MANA-CONTENT` | CONTENT / `SF-M-MANA` | completed Draft 0 numeric policy | Freeze provisional maximum/initial mana and fixed-tick regeneration rate/delay/rules. Values remain Balance Lab inputs; development-command enablement remains owned by Development Instrumentation. |
| `SF-MANA-SIM` | SIM / `SF-M-MANA` | `SF-MANA-CONTENT` and established authoritative player-state conventions | Immutable integer current/max state, checked consume, exhaustion, clamp, regeneration, refill/empty, deterministic ordering, and a life-cycle seam. Death/respawn policy is excluded. |
| `SF-MANA-PROTOCOL` | PROTOCOL / `SF-M-MANA` | proven `SF-MANA-SIM` behavior | Stable mana facts and deterministic serialization, distinct from development commands; current/max, ticks, corrections, and malformed-input rejection. |
| `SF-MANA-SERVER` | SERVER / `SF-M-MANA` | admitted player state, `SF-MANA-SIM`, `SF-MANA-PROTOCOL`, `SF-DEV-SERVER` | Own mana per gameplay session, publish facts/corrections, and register feature-owned consume-1000/empty/refill handlers through the common dispatcher. |
| `SF-MANA-CLIENT` | CLIENT / `SF-M-MANA` | `SF-DEV-CONSOLE`, `SF-MANA-PROTOCOL`, `SF-MANA-SERVER` | Resource diagnostics show only authoritative mana and invoke typed commands; native consume/empty/regenerate/refill proof, hidden-debug launch proof, no permanent HUD. |
| `SF-MOVE-REMOTE` | CLIENT / `SF-M-MOVE-V1` | completed connected snapshot adapters | Buffer and interpolate non-local authoritative presentation, initially connected monsters. Apply the same policy to remote players only when a real remote-player snapshot consumer exists; do not invent one for this task. No prediction. |
| `SF-MOVE-LOCAL` | CLIENT / `SF-M-MOVE-V1` | completed local-player snapshot/correction adapter | Add explicit local-player correction diagnostics and deterministic correction fixtures while continuing to present the newest accepted authoritative local state. Do not add interpolation delay, prediction, or reconciliation policy without evidence. |
| `SF-MOVE-FIXTURES` | CLIENT / `SF-M-MOVE-V1` | `SF-MOVE-REMOTE`, `SF-MOVE-LOCAL`, existing loopback transport harness | Representative latency, loss, reordering, and correction fixtures with explicit diagnostics and reproducible seeds/settings. |
| `SF-MOVE-PROOF` | CLIENT / `SF-M-MOVE-V1` | `SF-MOVE-REMOTE`, `SF-MOVE-LOCAL`, `SF-MOVE-FIXTURES` | macOS native before/after validation for remote monster motion and local correction behavior. Prediction/reconciliation is allocated only through a later approved task if this evidence proves it necessary. |

Cycle 1 allocates exactly these tasks and no other new Starfall work.

### Symbolic future task shapes

These rows preserve known ownership and acceptance boundaries. They are not allocated in Cycle 1 and must be re-read against the then-current source and PM graph when their deliverable activates.

| Handle | Track / future deliverable | Exact prerequisite behavior | Focused acceptance boundary |
| --- | --- | --- | --- |
| `SF-HUD-HEALTH-PROTOCOL` | PROTOCOL / `SF-M-HUD` | completed `SIM-0011` health behavior and existing player snapshot identity | Stable bounded player-health facts/serialization only; no death/respawn lifecycle events and no UI schema. |
| `SF-HUD-HEALTH-SERVER` | SERVER / `SF-M-HUD` | `SF-HUD-HEALTH-PROTOCOL`, admitted player state, completed player-health behavior | Publish authoritative current/max health and corrections without moving player-life policy into the HUD. |
| `SF-HUD-CLIENT` | CLIENT / `SF-M-HUD` | completed Mana exchange, health exchange, and coordinator viewport-text primitive | Read-only permanent health/mana HUD using text, simple panels/images, bars or values, DPI-aware deterministic layout, and native full/partial/empty/regen states. No containers or general GUI framework. |
| `SF-FIRE-PRESENTATION` | CLIENT / `SF-M-FIRE` | `CLIENT-0027`, completed Basic bow/projectile path, Fire facts/exchange | Reuse physical release animation; distinguish Fire through client-owned arrow/effect; present authoritative acceptance, resolve, damage, and mana consumption; terminal native proof. The permanent HUD is not a dependency. |
| `SF-LIFE-MANA-INTEGRATION` | SIM / `SF-M-LIFE` | completed `SIM-0011` behavior plus completed Mana life-cycle seam | Inspect and reuse the existing player damage/defeat/delay/town-respawn/full-health behavior and its preserved player entity/gameplay-session identity. Add only the missing Mana lifecycle integration and freeze any unresolved cross-resource policy. Do not reimplement or reopen completed player-life behavior. |
| `SF-LIFE-PROTOCOL` | PROTOCOL / `SF-M-LIFE` | `SF-LIFE-MANA-INTEGRATION` | Encode only the player-life facts still required for connected presentation: admitted-player defeat, respawn timing/anchor, resource restoration on the preserved authoritative player entity, and corrections. The Client never selects or replaces the respawn entity identity. |
| `SF-LIFE-SERVER` | SERVER / `SF-M-LIFE` | `SF-LIFE-MANA-INTEGRATION`, `SF-LIFE-PROTOCOL`, existing monster attack exchange | Publish connected player-life outcomes from the reused authoritative behavior. Development kill/respawn handlers are optional instrumentation allocated only if the implementation/validation plan proves they are useful. |
| `SF-LIFE-CLIENT` | CLIENT / `SF-M-LIFE` | `SF-LIFE-SERVER` | Present authoritative defeat and return-to-town outcomes using whatever debug/permanent surfaces exist at execution time. Final native proof requires a monster—not a debug command—to defeat the admitted player. |
| `SF-INVENTORY-CONTENT` | CONTENT / `SF-M-INVENTORY` | owner-approved Inventory activation inputs only | Freeze one provisional slot count and two or three exact development item definitions. No dependency on health, Mana, or HUD numeric conventions; no equipment, drops, final item schema, art, economy, or persistence. |
| `SF-INVENTORY-GIVE` | SERVER / `SF-M-INVENTORY` | completed inventory domain/exchange plus `SF-DEV-SERVER` | Register bounded `give <item-id> [quantity]` through the common dispatcher; authoritative insertion/rejection only. Inventory itself remains independent of the console. |
| `SF-INVENTORY-PROOF` | CLIENT / `SF-M-INVENTORY` | refocused `CLIENT-0014`, permanent GUI rendering foundation, `SF-INVENTORY-GIVE`, `SF-DEV-CONSOLE` | Use typed/console injection, then prove visible insert, move, swap, full/invalid rejection, correction, and fixed-slot interaction natively. Only the Client surface consumes the HUD rendering foundation. |
| `SF-EQUIPMENT-PROTOCOL` | PROTOCOL / `SF-M-EQUIPMENT` | completed Inventory plus refocused `GAME-0005` | Equipment-slot state, equip/unequip intent, compatibility/replacement/rejection/correction. No stats/effects or presentation mapping. |
| `SF-EQUIPMENT-SERVER` | SERVER / `SF-M-EQUIPMENT` | `SF-EQUIPMENT-PROTOCOL`, refocused `GAME-0005`, completed Inventory exchange | Bind admitted inventory owner, route equip/unequip, publish authoritative equipped state/corrections. |
| `SF-EQUIPMENT-CLIENT` | CLIENT / `SF-M-EQUIPMENT` | `SF-EQUIPMENT-SERVER`, completed Inventory GUI | Add only provisional equipment-slot UI needed for compatible equip, incompatible reject, observation, and unequip native proof. No on-character rendering. |
| `SF-DROP-CONTENT` | CONTENT / `SF-M-DROPS` | completed Inventory item definitions plus starter monster identities | Freeze exact provisional seeded drop tables/item identities. Do not own physical presentation or collection behavior. |
| `SF-RANGER-SELECTION` | CONTENT / deferred | completed Inventory and Equipment, exact character rig evidence | Select exact Ranger/leather pieces and pack-relative paths for later modular presentation. It does not define a starter loadout or block Basic Arrow. |

## New coordinator task specifications

`CF-DEBUG-CLIENT-ADOPTION` is allocated in Cycle 2 because it enables the near-term Development Instrumentation milestone. `CF-VIEWPORT-TEXT` remains a symbolic future task until the Resource HUD activates.

| Handle | Track / proposed deliverable | Dependencies | Focused acceptance boundary |
| --- | --- | --- | --- |
| `CF-DEBUG-CLIENT-ADOPTION` | SHARED / Starfall development debug UI enablement | `SHARED-0016`, `SHARED-0024` | Extend the approved consumer boundary from native Editor-only to Starfall.Client development instrumentation; document/validate the exact family-source allowlist, reusable backend/native boundary, caller-owned SDL/GPU lifecycle compatibility, macOS ARM64 native use, and headless exclusion. Do not own `--debug-ui-hidden`, `F12`, visibility behavior, Starfall windows, or permanent UI; those product behaviors belong exclusively to `SF-DEV-SHELL`. |
| `CF-VIEWPORT-TEXT` | SHARED / Viewport text rendering v1 | `SHARED-0016` and the exact shared SDL GPU source boundary confirmed during its Plan-mode inspection | One narrow caller-owned viewport-space text renderer with deterministic layout metrics, DPI scaling, bounded glyph/font input, tests, and macOS native proof. It does not migrate Royale text, create controls/layout containers, or define Starfall HUD styling. |

The coordinator grooming cycle must also correct dormant shared dependencies without manufacturing a Ranger task:

- keep `ASSET-0004`, `ASSET-0006`, and `SHARED-0020` on the Basic Arrow enabler path through refocused `CONTENT-0011`; `SHARED-0020` proves reusable static attachment rendering with a harness-local technical socket transform and does not define Starfall's semantic hand socket or local bow transform;
- remove the obsolete `CONTENT-0011` dependency from `SHARED-0003` and `ASSET-0005`;
- record in both dormant task contracts and matching roadmap prose that exact Ranger selection is an unresolved activation-time prerequisite; no canonical dependency is attached until Starfall allocates and completes the real selection task;
- move `SHARED-0003`, `SHARED-0004`, and `ASSET-0005` to no milestone with `priority: none` so removing the obsolete edge does not make modular-equipment work recommendable before activation;
- preserve the broader `SHARED-0007` attachment task as a future roadmap placeholder that reviews/reuses `SHARED-0020`;
- move `COORD-0005`, `SHARED-0007`, `SHARED-0013`, and `SHARED-0014` to no milestone with `priority: none`;
- move `SHARED-0005` to no milestone with `priority: none` until an exact material-variant consumer exists;
- keep `SHARED-0015` as focused low-priority debt, but remove misleading milestone membership if it is not part of an active deliverable;
- keep `SHARED-0025` and `COORD-0014` dormant, milestone-free, and `priority: none` until a documented Pressure Cooker activation trigger occurs;
- assign `ASSET-0007` and `ASSET-0008` only when their proper-scene or selected-monster deliverables activate.

Cycle 2 creates one focused Basic Arrow shared-enabler milestone for `ASSET-0004`, `ASSET-0006`, and `SHARED-0020`. Its observable result is the exact selected archer/bow cook and one rendered socketed static bow proof using a harness-local technical socket transform. Starfall's provisional semantic hand socket, local bow transform, and native placement validation remain in `CLIENT-0011`; the shared milestone must not absorb Starfall integration or gameplay. Existing task priorities are changed only when the grooming plan makes a deliberate scheduling decision; milestone assignment is not a reason to normalize them cosmetically.

## Dependency and execution view

### Immediate path

```text
Starfall Basic behavior:
PROTOCOL-0006 -> PROTOCOL-0007 -> SERVER-0008 -> CLIENT-0012

Development instrumentation:
parent CF-DEBUG-CLIENT-ADOPTION
  -> SF-DEV-ADOPTION
  -> SF-DEV-SHELL
SF-DEV-PROTOCOL -> SF-DEV-SERVER
SF-DEV-SHELL + SF-DEV-SERVER -> SF-DEV-CONSOLE

Basic presentation:
CONTENT-0011
  -> parent ASSET-0004
  -> CLIENT-0007

CONTENT-0011
  -> parent ASSET-0006
  -> parent SHARED-0020
  -> CLIENT-0011

CLIENT-0007 + CLIENT-0011 + CLIENT-0012
  -> CLIENT-0018

CLIENT-0012 + CLIENT-0007 + CLIENT-0011 + CLIENT-0018
  + SF-DEV-CONSOLE
  -> CLIENT-0019 terminal Basic proof
```

After structural grooming, `PROTOCOL-0006` remains the exact first feature task to plan. Development Instrumentation and asset selection/acquisition are independent prerequisite lanes that can be planned in their own owner-directed cycles before the terminal Basic proof.

### Resource, skill, and life path

```text
Development Instrumentation
  -> Mana

completed player health + completed Mana + parent CF-VIEWPORT-TEXT
  -> Player Resource HUD

Basic Arrow + Mana
  -> Fire Arrow authoritative behavior
  -> action contract proven by the second consumer
  -> Arrow Rain authoritative behavior

completed player-life simulation + Mana
  -> Player Defeat and Town Respawn
```

The Resource HUD and Fire can proceed independently after Mana; scheduling HUD first is a product preference, not a PM dependency. Arrow Rain consumes the authoritative action contract proven reusable by Fire, not Fire presentation or HUD work. Player Life may proceed after Mana without waiting for either spell.

### Progression, inventory, equipment, and drops

```text
Basic Arrow
  -> XP and Level Progression

Player Resource HUD
  -> Inventory
      |-> Equipment
      `-> Physical Drops and Collection
```

The permanent HUD is the rendering/layout foundation, not the inventory domain owner. Only Inventory's Client surface consumes that foundation. Inventory content, simulation, protocol, and exchange do not depend on health/Mana conventions or the console; only its item-injection/native-proof leaf consumes Development Instrumentation.

### Independent movement-quality path

```text
completed CLIENT-0009 local-player snapshot adapter
completed CLIENT-0023 connected-monster adapter
  -> SF-MOVE-REMOTE + SF-MOVE-LOCAL
  -> SF-MOVE-FIXTURES
  -> SF-MOVE-PROOF
```

Remote authoritative entities use buffered interpolation. The locally controlled player begins with correction diagnostics and the newest accepted authoritative state; v1 does not blindly add interpolation latency to local control. This lane is independent of Basic Arrow.

## Linked-project grooming cycles

No cycle below activates a feature.

Precondition: review and commit this four-file audit work entity before Cycle 1. The coordinator must be clean before a later Starfall pointer-only handoff; the grooming cycle must not absorb these report changes into a child-pointer commit.

### Cycle 1 — Starfall structural grooming

From the verified Starfall project context, using supported PM tooling:

1. refresh family state, worktrees, task readback, and PM validation;
2. create only `SF-M-DEV`, `SF-M-BASIC`, `SF-M-MANA`, and `SF-M-MOVE-V1`;
3. allocate only the Cycle 1 rows under `New Starfall task specifications` through the next-ID service;
4. directly groom/reassign existing tasks: put the immediate Basic chain and its presentation closure in `SF-M-BASIC`; remove Mana ownership from existing Fire/Rain tasks; refocus `CONTENT-0008`; separate the existing combined Inventory/Equipment contracts; and move unfinished future Editor, Balance Lab, selected-presentation, equipment/drop, and deferred-initiative work to no milestone with `priority: none` without allocating replacement graphs;
5. preserve M2's name and completed membership, and mark it in `roadmap/bootstrap` as a legacy planning bucket superseded by deliverable milestones;
6. update `roadmap/bootstrap`, `product/first-playable-zone-draft-0`, `content/draft-0-archer-kit`, and focused Development Instrumentation, Mana, and Movement Quality roadmap pages as needed;
7. record future coordinator prerequisites in prose only—do not invent canonical URIs;
8. verify no feature task is active and the family graph remains cycle-free;
9. commit only Starfall PM/wiki changes, then perform the normal pointer-only coordinator handoff and stop.

Cycle 1 does not allocate HUD, Fire-presentation, Rain replacement, Player Life, Progression replacement, Inventory, Equipment, Physical Drop, Ranger, proper-scene, selected-monster, or Balance Lab tasks. Their manifest handles remain planning notes. This is a direct backlog-grooming cycle, not a PM task whose purpose is task creation.

### Cycle 2 — Coordinator shared-boundary grooming

After Cycle 1 is pinned:

1. allocate only `CF-DEBUG-CLIENT-ADOPTION` through coordinator PM; `CF-VIEWPORT-TEXT` remains symbolic until HUD activation;
2. create the Development Instrumentation shared-boundary deliverable and the deterministic Basic Arrow shared-enabler milestone;
3. remove obsolete `CONTENT-0011` edges from dormant `SHARED-0003` and `ASSET-0005`, record the missing activation-time Ranger-selection prerequisite in prose, and attach no replacement canonical dependency;
4. apply the coordinator placeholder/milestone corrections listed above;
5. update shared-engine and Starfall-enabler wiki pages;
6. validate the coordinator and complete family dependency graph;
7. commit only coordinator PM/wiki changes and stop.

### Cycle 3 — Starfall canonical wiring continuation

After Cycle 2 commits the real coordinator IDs:

1. refresh both project revisions and family warnings;
2. attach the canonical coordinator debug-consumption task to `SF-DEV-ADOPTION`;
3. update matching Starfall roadmap prose and record mutation receipts;
4. validate every dependency and confirm all feature tasks remain todo;
5. commit only the reviewed Starfall wiring, perform the pointer-only handoff, and stop.

Do not combine these repositories into one PM transaction. Push Starfall commits before the coordinator commits that pin them.

## Remaining owner decisions

These decisions are intentionally deferred to the owning task's Plan-mode pass; they do not reopen A-01 through A-13:

- exact mana maximum, initial value, regeneration rate/delay/rules, and development enablement option;
- whether `CONTENT-0008` preserves the currently persisted nearest-integer half-up XP sequence or adopts the proposed checked integer-ceiling rule `nextRequirement = (previousRequirement * 115 + 99) / 100`; approval must include the resulting exact level 2–20 sequence;
- exact provisional inventory slot count and the two or three development item definitions;
- exact first equipment-slot subset;
- exact deterministic drop tables, physical-drop placeholder representation, selection radius/gesture, reservation, and expiry values;
- exact resource-HUD font input, visual tokens, bar-versus-value composition, and safe-area layout after the text primitive is proven;
- whether Fire evidence requires one focused Basic action-state extraction before Fire-specific code;
- exact movement-buffer delay/interpolation policy and whether v1 evidence justifies prediction/reconciliation;
- which real Balance Lab scenario first justifies activating `EDITOR-0004`;
- when gameplay evidence or authoring friction justifies the proper Editor-authored scene.

Resolved questions must not be reintroduced: Mana is independent; Basic is equipment-free; Inventory precedes Equipment and Physical Drops; Basic owns monster death only; ImGui is development instrumentation; the permanent first GUI outcome is the health/mana HUD; and milestones are completable deliverables.

## Validation, commits, and stop conditions

Every grooming cycle must:

- use PM MCP/application services or supported child-context PM tooling; never hand-edit `.pm/`;
- inspect every mutation receipt and re-read the owning task/wiki from the owning project;
- run PM validation in every mutated repository and review family warnings/readiness;
- check for missing, invalid, unavailable, or cyclic local/canonical dependencies;
- confirm no feature task became active;
- run `git diff --check`, inspect the full diff/staged file list, and confirm sibling children/gitlinks are untouched;
- create one focused owning-repository commit and a separate mechanical Starfall pointer commit where applicable;
- add one date-only entry to the audit log and check only audit items genuinely completed by that cycle;
- stop without selecting or implementing the next task.

Every feature implementation retains its task-specific automated, headless, native, and owner-visual gates. Visual checkpoints are proposed only when the result is genuinely worth preserving; movement-only or diagnostic-only evidence need not become a project-history artifact.

## Manifest checklist

- [x] Fresh authoritative project/family/task readback recorded with stable IDs and zero warnings.
- [x] A-13 supersession crosswalk included.
- [x] Current unfinished coordinator and Starfall work assigned a direct disposition.
- [x] Deliverable milestones and terminal observable outcomes proposed.
- [x] Existing tasks to refocus/split identified without changing completed history.
- [x] New focused work specified with owner, track, milestone, prerequisites, and acceptance boundary.
- [x] Cycle 1 allocation limited to Development Instrumentation, Connected Basic Arrow, Mana, and Connected Movement Quality v1; later task shapes remain symbolic.
- [x] Executable work separated from milestone-free `priority: none` roadmap placeholders.
- [x] Cross-project allocation and canonical-wiring cycles separated.
- [x] Remaining decisions isolated to owning Plan-mode passes.
- [ ] Owner approves Cycle 1 Starfall structural grooming.
- [ ] Cycle 1 completes, validates, commits, logs, checks corresponding audit items, and stops.
- [ ] Owner approves Cycle 2 coordinator grooming.
- [ ] Cycle 2 completes, validates, commits, logs, checks corresponding audit items, and stops.
- [ ] Owner approves Cycle 3 Starfall canonical wiring.
- [ ] Cycle 3 completes, validates, commits, logs, checks corresponding audit items, and stops.
- [ ] Owner enters Plan mode and selects `PROTOCOL-0006` after grooming.
