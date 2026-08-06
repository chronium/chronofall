# ChronoFall and Starfall Backlog Audit — Owner-Decision Addendum

This addendum is part of one work entity with the [backlog audit](2026-08-05-chronofall-starfall-backlog-audit.md), [audit log](2026-08-05-chronofall-starfall-backlog-audit-log.md), and [execution manifest](2026-08-05-chronofall-starfall-backlog-execution-manifest.md).

It records owner decisions, clarifications, corrections, and changed recommendations discovered after the original audit. A later entry may refine an earlier one, but historical entries remain intact. These decisions guide future planning and backlog grooming; they do not themselves authorize PM or source mutation.

Entries appear in the order the owner supplies them. They do not correspond to finding order in the original audit and should not be rearranged merely to mirror that document.

## A-01 — Milestones represent deliverables

Date: 2026-08-06

Status: owner decided

### Decision

A milestone is an independently demonstrable deliverable. Independently demonstrable does not mean dependency-free: a milestone may build on completed earlier deliverables and consume their established contracts. It must add an observable outcome of its own that can be exercised and judged without pretending inherited capabilities were implemented by that milestone.

A milestone is not:

- an epic or broad capability bucket;
- an entire project or large project phase;
- an iteration, sprint, or development cycle;
- a label applied to loosely related work merely because it occurs in approximately the same period.

Tasks inside a milestone may span configuration/content inputs, protocol facts and serialization, authoritative simulation, World exchange and transport binding, Client intent and presentation, automated testing, and native owner validation. They belong together when those parts converge into one coherent deliverable that can be exercised and judged end to end.

### Basic Arrow reference milestone

The complete connected Basic Arrow path is the reference shape for a Starfall milestone. Its deliverable is not one protocol or simulation layer in isolation. It is a player-visible result:

1. the player targets a connected authoritative monster;
2. the Client sends Basic Arrow intent;
3. the admitted World session supplies the actor and validates the request;
4. authoritative fixed-step simulation decides timing, cancellation, damage, and death;
5. protocol facts and deterministic serialization carry the command and outcome;
6. World exchange transports those facts;
7. the Client presents the result through the existing draft monster path and the bounded Basic Arrow presentation work;
8. focused tests and a native connected run prove the entire path.

The milestone may therefore contain the relevant configuration/content, Protocol, Simulation, Server, Client, presentation, test, and validation tasks. This is not undesirable cross-track breadth: those tasks are the constituent parts of one deliverable.

The milestone is complete only when the connected authoritative path and its bounded draft presentation converge. Its native proof includes:

- connected intent through the authoritative outcome;
- the technical or selected humanoid's bow-body animation;
- one rendered bow and one client-owned visual arrow;
- visual monster hit feedback;
- authoritative monster damage and death;
- an ImGui Combat diagnostic view showing target health, `300` internal units / `3` displayed damage, accepted/rejected/cancelled result, and death outcome;
- one native end-to-end owner validation.

The game view needs the arrow, hit flash, and monster death. It does not need permanent floating damage numbers or a target-status HUD. “Displayed damage 3” is a gameplay-value description and diagnostic fact, not a commitment to floating combat text. Player defeat and respawn belong to the later player-life deliverable.

Monster replenishment belongs to the camp-lifecycle deliverable. Basic Arrow may consume and visibly exercise the already-established replenishment contract after a defeated monster leaves the world, but replenishment is not part of the attack milestone's owned acceptance boundary.

### Backlog consequence

Starfall's current M2 is too large and must be split. Treating it as an umbrella or capability bucket is no longer an acceptable alternative.

The later grooming cycle must:

- preserve completed tasks as historical evidence rather than rewriting them for cosmetic consistency;
- define smaller milestones around independently demonstrable results;
- place the unfinished connected Basic Arrow tasks and their necessary draft presentation/validation closure into one Basic Arrow deliverable milestone;
- separate Fire Arrow, Arrow Rain, progression, physical drops, inventory/equipment, selected presentation, proper scene authoring, and editor/Balance Lab work according to the distinct deliverables they produce;
- avoid equating milestone boundaries with strict calendar iterations or requiring all work to proceed serially when independent evidence lanes can progress safely;
- give each milestone explicit acceptance describing what the owner can run, observe, and validate when it is complete.

Exact milestone names, keys, task membership, priority, and ordering remain to be proposed from authoritative PM readback during the approved backlog-grooming plan. No PM mutation is authorized by this addendum alone.

### Audit effect

This decision resolves the alternative presented in F-01. The audit's first Immediate backlog repair checklist item is complete. The remaining action is implementation of the decision through a separately approved PM grooming cycle.

## A-02 — Mana is an independent end-to-end milestone

Date: 2026-08-06

Status: owner decided

### Decision

Mana is a complete Starfall deliverable and must be represented by its own milestone under A-01's milestone definition.

Mana is not owned by Fire Arrow, Arrow Rain, or another mana-consuming action. Those actions consume the established mana contract after the mana milestone is complete.

The milestone owns the bounded Draft 0 path for:

- explicit provisional configuration and owner decisions;
- authoritative maximum/current mana state in internal integer units;
- initial state, fixed-tick regeneration, consumption, restoration, clamping, exhaustion, and deterministic ordering;
- World ownership and gameplay-session integration;
- transport-neutral facts and deterministic serialization;
- connected World exchange and corrections;
- draft development presentation through the Starfall debug GUI;
- focused simulation, protocol, World, loopback, Client, and native tests;
- documentation of the seam later permanent player-facing rendering will consume.

Permanent player-facing mana rendering remains separately owned. The debug proof establishes that authoritative mana exists and works end to end; it does not define the final HUD.

### Testing mana before spells

The mana milestone does not need a fake spell. It uses an explicit development-only authoritative command surface.

The smallest proof supports actions such as:

- consume exactly 1,000 internal mana units;
- empty current mana;
- refill to configured maximum mana;
- optionally pause/advance enough fixed ticks to make regeneration evidence easy to inspect, if existing host control cannot already provide deterministic coverage.

These are requests, not local mutations. The Client resource window invokes typed mana commands through the single shared development-command envelope and dispatcher established by A-08. A console frontend may parse text into the same command, but mana does not create a second development protocol. The World binds the admitted player, validates that development commands are enabled, dispatches to the mana-owned handler, invokes the authoritative mana behavior, and publishes the resulting state/facts. The Client displays only the authoritative result.

The development command path must be impossible to mistake for production gameplay:

- disabled unless an explicit local/development host option enables it;
- rejected when disabled;
- unavailable to non-development or unsupported remote operation according to the approved host policy;
- excluded from ordinary spell/action contracts;
- deterministic and covered by malformed/unauthorized-command tests;
- incapable of changing client-side state without an authoritative World result.

Development-only mana commands carry no compatibility promise. They are durable engineering instrumentation with bounded tests and documentation, but they are not part of Starfall's stable gameplay protocol. Their message shapes, command set, channel assignment, and diagnostics may evolve with development tooling. Gameplay clients and release compatibility must never depend on them, and stable spell/resource facts must be defined separately from this instrumentation.

Automated acceptance should cover:

- initial and maximum values;
- zero, exact-boundary, and insufficient-mana consumption;
- regeneration at fixed ticks, including partial intervals and maximum clamping;
- restoration, emptying, repeated commands, and checked arithmetic;
- a clean life-cycle integration seam through which later player-life work can define death, respawn, restoration, and session-reset behavior without rewriting mana internals;
- deterministic serialization, malformed input, sequence/order, and correction behavior;
- loopback command-to-authoritative-result flow;
- headless outputs remaining free of SDL, GPU, ImGui, and presentation dependencies.

Native acceptance should launch the real World and Client with development commands enabled, use the debug window to consume 1,000 mana, empty mana, observe regeneration, refill mana, and confirm every displayed change came from an authoritative response. The same Client launched with the debug GUI initially hidden must remain fully usable and produce clean screenshots.

Death and respawn behavior does not block completion of the mana milestone. The mana milestone proves the life-cycle seam and preserves current mana state according to its own bounded contract; a later player-life deliverable decides and tests whether death, respawn, restoration, reconnect, or session replacement preserves, clears, or restores mana.

### Backlog consequence

The later grooming cycle must:

- allocate one mana deliverable milestone;
- remove mana-system ownership from `SIM-0009` and `SIM-0007` while retaining each skill's own mana cost and validation against the established mana service/state;
- create or groom focused Content/configuration, Simulation, Protocol, Server exchange, Client debug presentation, test, and validation tasks as required by the actual project boundaries;
- make Fire Arrow and Arrow Rain depend on the completed mana milestone's exact prerequisite tasks rather than on one another merely to acquire mana;
- keep final HUD/presentation as a later consumer rather than part of the debug proof.

Exact numeric maximum, initial mana, regeneration rate, regeneration delay/rules, death/respawn restoration, and command-gating options remain Plan-mode decisions unless already established elsewhere.

## A-03 — ImGui is Starfall's development debug GUI

Date: 2026-08-06

Status: owner decided

### Decision

Starfall.Client will adopt ImGui as its development-time debug GUI.

It must not repeat Royale's single-window dump of unrelated data. The Starfall debug GUI presents separate windows for separate concerns. Tabs may group closely related views within a window, but unrelated resources, networking, rendering, world state, combat, and diagnostics do not become one scrolling data dump.

The debug GUI is not:

- the permanent player-facing HUD;
- the Starfall editor shell or editor design system;
- a source of gameplay authority;
- a reason for World, Simulation, Protocol, Content, Balance Lab, or headless editor code to reference ImGui, SDL, or GPU projects;
- a generic reflection inspector or automatic dump of every runtime object.

### Interaction contract

- A compact menu bar exposes the available debug windows and their visible state.
- `F12` toggles the entire debug GUI, including its menu bar.
- A launch argument starts the debug GUI hidden so native screenshots and captures can begin without debug chrome. The proposed semantic name is `--debug-ui-hidden`; the exact spelling is frozen during the implementation plan.
- Hiding the GUI does not disable gameplay input, simulation, rendering, or connected operation.
- When ImGui captures mouse, keyboard, or text input, the Client suppresses conflicting world/gameplay input through the existing backend capture state.
- Window visibility and reasonable layout preferences may persist locally when that can be done without storing gameplay or authoritative object identity.
- Screenshots intended as clean product evidence must be possible with the complete GUI hidden.

### Concern-specific windows

The initial windows should be introduced only as their owning feature needs them. Likely examples include:

- Player Resources: authoritative health/mana values and mana development actions;
- Player Life: kill, respawn, restoration, and protected-town diagnostics;
- Combat: selected target, action sequences, acceptance/rejection, timing, and outcome facts;
- World/Session: admission, entity/session identity, tick, channel, and connection state;
- Presentation/Rendering: camera, capture, animation, and renderer diagnostics.

These are concern categories, not authorization to build every window immediately. The Development Instrumentation milestone establishes the shared shell. Basic Arrow adds only its Combat diagnostics, and Mana adds only its Resource diagnostics. Later tasks extend the GUI through their own windows or coherent tabs.

### Debug actions and authority

Buttons such as `Consume 1000 Mana`, `Empty Mana`, `Refill Mana`, `Kill Player`, and `Respawn` send explicitly gated development requests. They do not mutate Client state directly.

The World remains authoritative:

1. the debug GUI requests an action;
2. the shared A-08 development-command envelope identifies, sequences, and serializes it;
3. the common dispatcher verifies admitted-session ownership and development authorization;
4. a feature-owned handler applies or rejects authoritative Simulation/World behavior;
5. the World publishes the authoritative result through the common result envelope;
6. the GUI displays the result and any rejection diagnostics.

Kill/respawn controls belong to player-life diagnostics and need not be implemented by the mana milestone merely because the resource window establishes the shell.

### Ownership and dependency consequence

ChronoFall already owns the caller-controlled SDL GPU ImGui backend through `SHARED-0024`, but durable architecture currently allows only the native Starfall editor host to consume it. A later approved coordinator/Starfall grooming sequence must revise that allowlist and documentation for a narrowly defined Starfall.Client development-debug consumer.

Starfall owns:

- the debug menu/window organization;
- feature-specific debug views and actions;
- launch options and F12 behavior;
- mapping ImGui capture to gameplay-input suppression;
- local debug layout state;
- development command policy at the product boundary.

ChronoFall continues to own only the reusable caller-controlled backend and its native dependency/build boundary. Royale is unchanged.

### Validation consequence

The first adoption must prove:

- native macOS ARM64 rendering inside the existing Starfall Client window;
- menu-controlled independent window visibility;
- F12 complete hide/show behavior;
- hidden-at-launch screenshot behavior;
- correct input capture without stray movement, attack, or targeting commands;
- clean Client operation when all windows are closed/hidden;
- absence of ImGui/native editor dependencies from every headless output;
- deterministic tests for visibility, launch-option parsing, action routing, and command authorization where those contracts are BCL-testable.

Visual design should remain restrained and readable, but this debug GUI is an engineering instrument rather than the permanent game UI or the polished Starfall editor.

### Development Instrumentation milestone

Development Instrumentation is its own bounded deliverable milestone. It is complete when a developer can open Starfall's debug GUI, issue one harmless authoritative command through either a typed control or the console, receive the correlated World result, and hide the entire GUI without affecting gameplay.

Focused constituent tasks may cover:

- adoption of the shared caller-controlled ImGui backend by Starfall.Client;
- the menu, `F12`, hidden-at-launch behavior, and correct input capture;
- the single development-command envelope, dispatcher, result path, and diagnostics;
- the console frontend;
- a harmless `Ping World` or equivalent world-information command whose typed button and console form create the same command representation;
- macOS ARM64 native validation and headless dependency validation.

Linux native or smoke validation is not required for this milestone. Mana later registers its own feature commands against the completed dispatcher rather than extending or duplicating its protocol foundation.

## A-04 — Basic Arrow and the first rendered bow do not depend on equipment

Date: 2026-08-06

Status: owner decided

### Decision

Basic Arrow has no equipment dependency. Its authoritative simulation, protocol, World exchange, connected input, tests, and draft presentation must not wait for item identity, inventory, equipment slots, a starter loadout, Ranger armour, or visible-equipment rules.

The first rendered bow also has no equipment dependency. It is a focused presentation proof:

- pick one suitable bow asset from the already available source candidates;
- acquire/cook only that exact input through the established provenance boundary where required;
- render it on the technical or selected humanoid through the existing skeleton socket/static-rendering contracts;
- validate placement, scale, orientation, animation coexistence, and native appearance;
- keep the bow presentation client-owned and unable to grant or imply authoritative equipment state.

This proof does not require:

- an item definition or item instance;
- a starting inventory or loadout;
- equipment-slot definitions;
- Ranger equipment or body-region rules;
- equipment statistics;
- monster drops or world items;
- a final bow selection for release.

### Backlog consequence

The later grooming cycle must preserve the existing equipment-free Basic Arrow chain and remove equipment/loadout gates from the first bow presentation path.

In particular:

- `PROTOCOL-0006`, `PROTOCOL-0007`, `SERVER-0008`, and `CLIENT-0012` remain equipment-free;
- the first rendered-bow proof must not depend on `GAME-0005` or `CONTENT-0009`;
- `CONTENT-0009` remains a later item-to-attachment mapping concern after authoritative equipment exists;
- `CLIENT-0011` must be split or refocused so its first-bow/socket proof is not bundled with later equipment-aware aiming, IK, or loadout integration;
- any exact asset-pick/acquisition step stays narrow and must not become a complete archer, weapon, or equipment-selection program merely to render one bow.

The later equipment system may reuse the proven bow socket/presentation path. It does not retroactively own the proof.

## A-05 — The first player-facing GUI deliverable is the resource HUD

Date: 2026-08-06

Status: owner decided

### Decision

Starfall's first permanent player-facing GUI milestone is an authoritative player resource HUD. It precedes inventory and equipment, but it is not asked to establish a general GUI framework in advance.

The intended convergence is:

```text
Development Instrumentation
  |-- Basic Arrow Combat diagnostics
  `-- Mana Resource diagnostics

completed authoritative health + completed Mana
  -> permanent player resource HUD
```

Basic Arrow does not wait for Mana or the permanent HUD. The permanent HUD presents player health and mana only. Permanent damage-number and target-status treatment remains undecided.

This GUI is distinct from the A-03 ImGui development debug GUI. ImGui remains engineering instrumentation; it is not the permanent game HUD or inventory implementation.

The milestone consumes already-established authoritative health and completed mana state and implements only the presentation primitives required to show them:

- viewport-space text;
- simple images and panels;
- resource bars and/or numeric current/maximum values;
- basic deterministic layout;
- logical-window and DPI scaling;
- updates driven only by authoritative resource facts;
- deterministic layout/state tests and native visual validation at full, partial, empty, consuming, and regenerating/restoring resource states;
- clean separation from World/Simulation authority and every headless output.

The resource HUD is primarily a read-only presentation consumer. Its milestone does not own focus, selection, container surfaces, general controls, disabled-state vocabulary, server-rejection presentation, inventory interaction, or a generic action/result UI seam. Those capabilities arrive with the first concrete feature that requires them, likely inventory. The server does not transmit layouts, widgets, scripts, or reflection metadata. The Client owns composition and presentation; the World owns resource state.

ChronoFall may own a narrowly reusable viewport-space text/rendering primitive after a focused plan proves that boundary. Starfall owns the resource-HUD composition, styling, layout, behavior, and later game controls. Royale's existing text system is historical evidence only: it is discarded rather than migrated or made the shared foundation.

The native showcase must use real authoritative health and mana facts rather than fake widget data. It produces a visible outcome of its own under A-01 without turning the first HUD into a controls or container milestone.

### Backlog consequence

The later grooming cycle must allocate the focused resource-HUD milestone before inventory. It must identify the smallest renderer/text prerequisites from current repository evidence and must not merge the permanent HUD with editor UI, ImGui debug windows, inventory controls, generic container primitives, or final visual branding.

## A-06 — Inventory is a post-GUI deliverable milestone

Date: 2026-08-06

Status: owner decided

### Decision

Inventory is its own end-to-end deliverable milestone and builds on the completed resource-HUD rendering foundation. Inventory adds only the interaction and container primitives required by its concrete proof; the HUD milestone does not pre-build them speculatively.

The first proof has one exact bounded shape:

- bounded authoritative item identity and ownership;
- one player inventory with one fixed provisional slot count selected during its Plan-mode contract review;
- two or three deliberately minimal development item definitions;
- authoritative insert, move, swap, full-inventory rejection, invalid-operation rejection, and correction behavior;
- protocol, World exchange, corrections, and deterministic ordering;
- one visible Client inventory surface that presents authoritative contents and sends interaction intent only;
- development-time item injection through the shared A-08 command dispatcher so the native proof can run without drops or world items;
- automated and native end-to-end validation.

The inventory domain, Simulation, Protocol, World exchange, and Client GUI do not architecturally depend on the console. Only the bounded native-validation/item-injection task consumes the console frontend or its `give` syntax. A typed ImGui action or another test harness may invoke the same feature-owned insertion handler through the common development-command envelope.

Inventory does not depend on:

- monster drops;
- physical world items;
- a complete starter loadout;
- Ranger armour definitions;
- equipment slots;
- final item art, economy, trade, crafting, or persistence.

The first proof does not introduce nested inventories, multiple container kinds, bags, banks, arbitrary container graphs, or a generalized container framework. Richer selection, disabled, rejection, and interaction states are introduced only as this concrete surface requires them.

### Backlog consequence

`GAME-0003`, `PROTOCOL-0010`, `SERVER-0011`, and `CLIENT-0014` require re-grooming because the current tasks mix inventory and equipment concerns and do not depend on a completed GUI milestone. Exact task reuse/splitting must be decided from authoritative PM readback; no new IDs are implied by this addendum.

## A-07 — Equipment follows inventory as a separate deliverable milestone

Date: 2026-08-06

Status: owner decided

### Decision

Equipment is a separate end-to-end deliverable built on the completed inventory system.

The first equipment proof needs only a provisional, deliberately incomplete slot model chosen during its own Plan-mode review. The slot list is not exhaustive, is not a final schema commitment, and may change when real class/content evidence appears.

The milestone should prove:

- an authoritative item can move from inventory into one compatible equipment slot and back;
- ownership, compatibility, replacement, rejection, and correction are deterministic;
- the Client presents and manipulates the equipment slots through the established GUI system;
- development-spawned items from the console are sufficient test inputs;
- tests and a native run demonstrate the complete inventory-to-equipped-state round trip.

No statistic, damage, armour, movement, appearance, or other gameplay effect is part of this first proof. Observing the authoritative equipped state, rejecting an incompatible move, and moving the item back to inventory is the complete deliverable. Equipment effects are later consumers with their own evidence and ownership.

Equipment does not depend on:

- monster drops or physical world items;
- a complete starting loadout;
- the Ranger family or final armour layout;
- selected visual assets or on-character equipment rendering;
- economy, persistence, trade, crafting, durability, sockets/gems, or exhaustive slot taxonomy.

Visible on-character equipment is a later presentation consumer. The equipment milestone may use technical identifiers and GUI state without solving modular armour or bow attachment again.

### Backlog consequence

`GAME-0005`, `CONTENT-0004`, `CONTENT-0008`, `CONTENT-0009`, `PROTOCOL-0010`, `SERVER-0011`, `CLIENT-0014`, and later visible-equipment tasks must be separated according to this order. System proof comes before Ranger loadout/content work; drops are an independent producer of inventory items rather than a prerequisite for inventory or equipment.

## A-08 — ImGui development console and simple server commands

Date: 2026-08-06

Status: owner decided

### Decision

Starfall should provide a development console as a dedicated window in the A-03 ImGui debug GUI.

The console uses a deliberately simple development-only command protocol handled by the World. It is engineering instrumentation, not a production administration system and not part of the stable gameplay protocol.

### Single development-command envelope

Starfall must have one development-command envelope, dispatcher, result path, and diagnostic model shared by typed ImGui controls and the text console:

```text
ImGui feature button ---+
                       +--> development command dispatcher --> feature-owned handler
Console text parser ----+
```

The common boundary owns enablement/gating, admitted-session binding, request sequencing, dispatch, response association, success/rejection results, and diagnostics. A feature window may create a typed command directly; the console parses text into the same command representation. Each feature owns and tests the authoritative operation and registers its handler. The console is another frontend, not an alternate protocol or a gameplay owner.

The initial console boundary should include only what a usable proof needs:

- one command-line input;
- bounded command and output lengths;
- submission history and readable success/error output;
- a non-zero request sequence so responses can be associated with submitted commands;
- deterministic parsing and explicit unknown/invalid-command diagnostics;
- one simple server-side command dispatch boundary with feature-owned handlers;
- transport through an explicitly enabled development path;
- ImGui input capture so typing never sends movement, combat, or targeting input.

No permission roles, account administration, moderation system, remote operations plane, scripting language, arbitrary shell execution, filesystem access, command discovery service, or compatibility guarantee is required. Development-mode enablement is a host safety gate, not a user/admin permission system.

### `give` command

After the inventory milestone provides authoritative item identity and insertion, the inventory feature should register a bounded command such as:

```text
give <item-id> [quantity]
```

The command adds the exact development item to the admitted player's authoritative inventory and returns an authoritative success or rejection result. The Client never inserts the item locally.

The base console does not need to wait for inventory. It may initially prove submission, parsing, dispatch, response association, errors, and one harmless development command. Inventory later adds `give` through its own feature-owned handler rather than expanding the console into a gameplay framework. Inventory's authoritative behavior does not depend on console text parsing; only its native validation/item-injection path may use this frontend.

### Backlog consequence

The console is a bounded constituent of the Development Instrumentation milestone defined by A-03. The later grooming sequence must preserve these boundaries:

- the debug GUI shell and input capture precede the console window;
- the World-side command path remains development-only and carries no compatibility promise;
- the inventory native-validation/item-injection task may consume the completed development-command boundary and console frontend, while inventory Simulation, Protocol, World exchange, and GUI remain independently valid;
- inventory implements/registers its own `give` handler for that native proof and does not wait for drops or world items;
- command handlers remain owned by the feature they manipulate;
- no permission/admin work is created merely to support local development.

## A-09 — Connected Movement Quality v1 is a completable deliverable

Date: 2026-08-06

Status: owner decided

### Decision

Starfall should have a completable `Connected Movement Quality v1` milestone covering:

- authoritative snapshot buffering;
- interpolation between accepted authoritative states;
- explicit correction diagnostics;
- representative latency, loss, and reordering fixtures;
- deterministic tests;
- macOS native before/after validation.

This supersedes both F-07's single deferred smoothing-task recommendation and the earlier A-09 wording that described one ongoing milestone. A-01 remains authoritative: the v1 milestone has a bounded observable result and can close.

A broader movement-quality initiative may remain ongoing as a milestone-free, lowest-priority roadmap placeholder. Prediction and reconciliation enter v1 only if evidence gathered during the v1 work makes them necessary to achieve its stated outcome. Otherwise they belong to a later versioned deliverable. Unrelated protocol, transport, combat, animation, camera, or physics work does not enter merely because it affects the client and World.

### Authority and ownership boundary

- The World and fixed-step simulation remain authoritative for positions, movement validity, collision, and correction facts.
- Client smoothing and interpolation are presentation behavior and never alter authoritative state.
- Client prediction is explicitly speculative. It may improve local responsiveness but never converts an input into an accepted movement outcome.
- Reconciliation consumes authoritative facts and corrects speculative/presented state without feeding rendered transforms back into Simulation.
- Remote players and monsters may use different presentation policies from the locally controlled player when evidence requires it; they do not need local-input prediction.
- Protocol changes belong only where the approved quality task proves that existing snapshot facts are insufficient. The milestone is not standing authorization to redesign movement transport.

The first grooming pass should define the exact v1 acceptance from current evidence. Prediction and reconciliation must not be implemented speculatively or hidden inside an interpolation task.

### Initiative scheduling rule

Broad initiatives that are not yet executable should remain unassigned to any milestone and carry the project's lowest priority. A milestone assignment communicates that a task contributes to a concrete deliverable; it must not be used merely to store or schedule a future idea.

When evidence makes an initiative actionable, an owner-approved grooming cycle should split or narrow it into focused executable tasks, assign only those tasks to the deliverable milestone they actually advance, and leave any remaining broad roadmap placeholder unassigned and lowest-priority.

### Backlog consequence

The later grooming cycle must:

- create `Connected Movement Quality v1` with explicit native acceptance under representative network conditions;
- create focused snapshot-buffering, interpolation, correction-diagnostic, fixture, and validation tasks only where the current architecture requires distinct ownership;
- add prediction/reconciliation to v1 only when v1 evidence makes them necessary; otherwise retain them under the broader lowest-priority initiative until a later versioned deliverable is justified;
- audit broad initiative-like tasks across ChronoFall and Starfall, remove milestone assignments that falsely imply deliverable membership, and retain them at the lowest priority until they are decomposed;
- preserve completed movement/network tasks as historical evidence rather than rewriting them into the new milestone.

No PM mutation or movement implementation is authorized by this addendum alone.

## A-10 — Basic Arrow establishes the shared combat-action starting point

Date: 2026-08-06

Status: owner decided

### Decision

Do not introduce a comprehensive combat-action system milestone in anticipation of future skills. The existing Basic Arrow lifecycle is the canonical starting point.

The reusable contract must support at least:

- one authoritative combat action active per actor;
- deterministic windup, resolution, and recovery timing;
- explicit acceptance, busy rejection, and cancellation;
- action-specific targeting, cadence, and resource requirements;
- no client-authoritative completion.

Fire Arrow is the first second consumer. Its Plan-mode pass must inspect and reuse the Basic Arrow contract directly, or include one focused extraction/refactoring task before Fire-specific behavior when the existing implementation cannot be consumed cleanly. Arrow Rain consumes the resulting established contract.

Mana remains an independent system and milestone. Movement interruption remains a deliberate combat-policy decision for the owning action plan; a shared action-state refactor must not invent it.

### Backlog consequence

The execution manifest must remove wording that implicitly assigns shared mana or shared combat-action ownership to Fire Arrow. It must preserve Basic Arrow as historical and architectural evidence, then place only an evidence-supported focused refactor ahead of Fire Arrow if required by source inspection.

## A-11 — Player Defeat and Town Respawn is a separate milestone

Date: 2026-08-06

Status: owner decided

### Decision

Basic Arrow owns monster death only. Player defeat and respawn belong to a later `Player Defeat and Town Respawn` deliverable milestone.

Preserve already-completed simulation work as historical evidence. The milestone closes the remaining connected path for:

- authoritative player damage and the zero-health transition;
- death state;
- configured respawn delay;
- return to the configured town respawn anchor;
- full health restoration and, provisionally, full mana restoration;
- protocol facts and deterministic serialization;
- World exchange;
- debug and permanent presentation as available at execution time;
- deterministic tests and native validation.

Development `kill` and `respawn` commands are useful instrumentation but are not the final gameplay proof. Final native acceptance requires a monster to defeat the player and the authoritative connected path to return that player to town.

### Backlog consequence

Remove player defeat/respawn ownership from the Basic Arrow presentation closure. The player-life milestone consumes the established health path and Mana's lifecycle seam without blocking completion of either Basic Arrow or Mana.

## A-12 — Inventory precedes sibling Equipment and Physical Drops deliverables

Date: 2026-08-06

Status: owner decided

### Decision

The dependency shape is:

```text
Resource HUD
  -> Inventory
      |-> Equipment
      `-> Physical drops and collection
```

Equipment and Physical Drops are sibling consumers of Inventory. Neither blocks the other.

Inventory is proven with development-injected items. Physical pickup consumes the already-working inventory. Exact drop content is required when monsters generate loot, not for the underlying inventory system. A complete Physical Drops milestone proving `kill -> drop -> collect -> inventory` depends on both Inventory and exact provisional drop content.

### Backlog consequence

Reverse the stale audit/checklist ordering that placed physical drop collection before Inventory or Equipment. Remove starter-loadout ownership from the early path. Exact item/drop content, physical world representation, collection, and later item-to-character presentation remain separately owned.

## A-13 — Execution-manifest supersession and initiative rules

Date: 2026-08-06

Status: owner decided

### Initiative representation

Until PM has a distinct roadmap outcome or initiative container:

- non-executable roadmap ideas are excluded from executable manifest work;
- existing placeholder tasks receive no milestone and `priority: none`;
- the manifest labels them as roadmap placeholders, never implementation tasks;
- activation begins by deriving focused executable tasks from the placeholder through an owner-approved grooming cycle.

Activation switches would decide whether work is enabled, but would not solve the separate need to represent something broader than a task and different from a deliverable milestone. That remains a possible future PM capability rather than ChronoFall implementation work.

### Required manifest supersession crosswalk

The execution manifest must explicitly record that:

- Mana is removed from Fire Arrow and Arrow Rain ownership;
- shared combat action state is no longer implicitly owned by Fire Arrow;
- Inventory precedes Physical Drops and collection;
- the starter loadout is eliminated from the early path;
- player respawn is removed from Basic Arrow presentation;
- the earlier ongoing A-09 model is replaced by versioned movement-quality deliverables;
- clarification questions already resolved by this addendum are closed and must not be reintroduced as open manifest decisions.

The manifest must distinguish executable tasks from milestone-free roadmap placeholders and must derive its exact task inventory from a fresh authoritative PM family readback.
