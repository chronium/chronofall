---
title: Initial Family Roadmap
createdAt: 2026-08-01T05:44:07.0156650Z
modifiedAt: 2026-08-07T19:27:26.1347630Z
---

## M0 — Coordinator foundation

`COORD-0001` through `COORD-0004` establish family identity, linked PM policy, agent/skill routing, child commit/gitlink workflow, recursive checkout, experiment/shared-source rules, and asset provenance. These are kickoff foundation tasks, not gameplay implementation.

## M1 — Skinned mesh and animation proof

The dependency graph is intentionally narrow:

1. `ASSET-0001` inventory supplied rigs/animations.
2. `ASSET-0002` select exact compatible inputs; `EXPERIMENT-0001` evaluate Royale patterns/capability gaps.
3. `EXPERIMENT-0002` make the owner-reviewed loader decision.
4. `EXPERIMENT-0003` and `EXPERIMENT-0004` define/test minimal data and sampling.
5. `EXPERIMENT-0005` bind pose; then `EXPERIMENT-0006` animated GPU skinning and `EXPERIMENT-0007` skeleton debug.
6. `EXPERIMENT-0008` diagnostics, `EXPERIMENT-0009` deterministic captures, `EXPERIMENT-0010` native owner validation, and `EXPERIMENT-0011` findings/promotion criteria.

M1 excludes modular armour, blending, root motion, retargeting, IK, general animation graphs, and production engine design.

M1 completed on 2026-08-01. The supplied Quaternius mannequin and compatible animation passed deterministic CPU sampling, native SDL GPU skinning, skeleton diagnostics, repeatable multi-timestamp captures, and explicit owner visual validation. The conclusions and bounded promotion matrix are recorded at `pm://project/prj_E7QP3LUocfY7k3PYM-EQOlqc/wiki/experiments/skinned-character-proof-findings`. Completion makes `SHARED-0001` dependency-ready but does not activate M2 work.

## M2 — Shared character presentation

M2 is formally delivered as the first focused shared character-presentation capability. Children can consume the promoted BCL presentation contracts, SDL GPU rendering, family source staging, skeletal and static cooking, sockets, action blending/layers, grip and reference-point support, and bounded IK/aim presentation without leaking presentation into headless code.

The delivery preserves its completed membership and evidence: shared test suites, fresh-checkout-safe staging, native character/static rendering proofs, architecture dependency tests, and successful Starfall source consumption. It does not establish game-specific action mapping, equipment rules, generic animation graphs, arbitrary engine abstractions, final distribution, or headless presentation dependencies.

Unfinished armour, equipment, broad attachment, debugging, preview, and hardening work remains milestone-free or belongs to a separately approved deliverable. Royale retains ownership of its linked integrations, and future child work continues through automatic pointer-only coordinator handoffs.

## Draft 0 coordinator enablers

Starfall's Draft 0 roadmap remains game-owned. Coordinator work exists only for genuinely shared presentation, cooking, native, physics, transport, and source-consumption boundaries.

The immediate Basic Arrow shared lane is now the coordinator's M5 deliverable:

- `ASSET-0004` acquires the exact selected technical humanoid/base and minimum bow-animation inputs after Starfall selection.
- `ASSET-0006` acquires the exact selected static bow and arrow inputs.
- `SHARED-0020` proves one rendered socketed bow with a harness-local technical socket and local transform.

Starfall `CLIENT-0011` owns its provisional semantic hand socket, local bow transform, rendering integration, and native placement validation. Equipment, Ranger armour, aiming, off-hand IK, and generalized grip systems do not block this shared proof.

Zone, monster, Ranger-equipment, broad attachment, preview, and typed-authoring work remain milestone-free roadmap placeholders until their concrete deliverables activate. Exact graph and source policy: `pm://project/prj_E7QP3LUocfY7k3PYM-EQOlqc/wiki/roadmap/starfall-draft-0-shared-enablers`.

## M3 — MMO bootstrap

M3 remains an undelivered historical coordinator bucket. Its completed tasks record early Starfall bootstrap work, including source-built Box3D, shared transport, the caller-controlled SDL GPU ImGui backend, UAL2 inventory/proof evidence, and reviewed cross-project architecture decisions, but they do not form one accepted capability contract.

M3 must not be used as an activation prerequisite. Starfall's game-specific roadmap remains child-owned, and the individual completed tasks, commits, and wiki pages remain the evidence for their own capabilities.

## M4 — Starfall.Client Development Instrumentation Boundary

M4 is formally delivered. Its sole coordinator task, `SHARED-0026`, approves `Starfall.Client` as a direct family-source consumer of the completed caller-controlled SDL GPU ImGui backend through `$(ChronoFallFamilyRoot)src/ChronoFall.EditorUi.SdlGpu/ChronoFall.EditorUi.SdlGpu.csproj`. The coordinator retains the backend, pinned native/source boundary, caller-owned lifecycle contract, and complete headless exclusion.

Starfall retains ownership of its debug shell, windows, menu and F12 behavior, `--debug-ui-hidden`, input capture, feature diagnostics, console, development-command semantics, and permanent product UI. Starfall Cycle 3 attached `pm://project/prj_E7QP3LUocfY7k3PYM-EQOlqc/task/SHARED-0026` to `pm://project/prj_pkIpzx0fzFD4URjvqBuYrGZF/task/CLIENT-0029`. The canonical prerequisite is complete, so `CLIENT-0029` is dependency-ready but remains todo until its own owner-approved implementation plan. No Starfall source, task state, or gitlink changed while completing M4.

## M5 — Connected Basic Arrow Shared Enablers

M5 is formally delivered. Its completed membership is `ASSET-0004`, `ASSET-0006`, `ASSET-0011`, and `SHARED-0020`; together they provide exact selected bow, arrow, and body-animation inputs, reproducible staging/cooking evidence, durable Blender evaluation guidance, and one rendered socketed static-bow proof through the shared caller-owned SDL GPU path.

M5 does not implement Starfall combat, equipment, item ownership, semantic socket content, projectile behavior, aiming/IK integration, final character art, or client integration.