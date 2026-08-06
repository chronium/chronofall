---
title: Initial Family Roadmap
createdAt: 2026-08-01T05:44:07.0156650Z
modifiedAt: 2026-08-06T08:12:05.1912330Z
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

M2 is preserved as a completed historical planning bucket. It records the first shared character-presentation promotion and the focused shared work completed under the earlier roadmap, including skeletal cooking, sockets, blending, static rendering/cooking, capture, and the family source-consumption boundary.

Every unfinished task has moved out of M2. Later armour, equipment, broad attachment, IK/debugging, preview, and hardening work now belongs either to a concrete deliverable milestone or to a milestone-free scheduling state. Completed membership is not rewritten merely to make the earlier bucket resemble the newer deliverable-milestone model.

Royale retains ownership of its linked integrations. Future child commits continue to use automatic pointer-only coordinator handoffs; no ceremonial coordinator submodule task is created.

## Draft 0 coordinator enablers

Starfall's Draft 0 roadmap remains game-owned. Coordinator work exists only for genuinely shared presentation, cooking, native, physics, transport, and source-consumption boundaries.

The immediate Basic Arrow shared lane is now the coordinator's M5 deliverable:

- `ASSET-0004` acquires the exact selected technical humanoid/base and minimum bow-animation inputs after Starfall selection.
- `ASSET-0006` acquires the exact selected static bow and arrow inputs.
- `SHARED-0020` proves one rendered socketed bow with a harness-local technical socket and local transform.

Starfall `CLIENT-0011` owns its provisional semantic hand socket, local bow transform, rendering integration, and native placement validation. Equipment, Ranger armour, aiming, off-hand IK, and generalized grip systems do not block this shared proof.

Zone, monster, Ranger-equipment, broad attachment, preview, and typed-authoring work remain milestone-free roadmap placeholders until their concrete deliverables activate. Exact graph and source policy: `pm://project/prj_E7QP3LUocfY7k3PYM-EQOlqc/wiki/roadmap/starfall-draft-0-shared-enablers`.

## M3 — MMO bootstrap

M3 is preserved as a completed historical coordinator planning bucket. It records the coordinator work that enabled Starfall bootstrap, including source-built Box3D, shared transport, the caller-controlled SDL GPU ImGui backend, UAL2 inventory/proof evidence, and the reviewed cross-project architecture decisions.

Every unfinished task has moved out of M3. Starfall's game-specific roadmap remains entirely child-owned, and completed M3 membership remains historical evidence rather than being renamed or redistributed.

## M4 — Starfall.Client Development Instrumentation Boundary

M4 is a medium-priority coordinator deliverable containing `SHARED-0026`. It will extend the approved family-source allowlist so Starfall.Client can consume the completed caller-controlled SDL GPU ImGui backend for development instrumentation while preserving caller lifecycle ownership and complete headless exclusion.

The coordinator does not own Starfall's debug shell, windows, F12 behavior, `--debug-ui-hidden`, feature diagnostics, console, command semantics, or permanent UI. Starfall Cycle 3 attached `pm://project/prj_E7QP3LUocfY7k3PYM-EQOlqc/task/SHARED-0026` to `pm://project/prj_pkIpzx0fzFD4URjvqBuYrGZF/task/CLIENT-0029`. The child adoption task is valid but waiting and must not activate or consume shared source until `SHARED-0026` completes and `CLIENT-0029` receives its own approved implementation plan.

## M5 — Connected Basic Arrow Shared Enablers

M5 is a medium-priority coordinator deliverable containing exactly `ASSET-0004`, `ASSET-0006`, and `SHARED-0020`. Its observable result is an exact selected archer/bow cook and one rendered socketed static-bow proof through the shared caller-owned SDL GPU path.

M5 does not implement Starfall combat, equipment, item ownership, semantic socket content, projectile behavior, or client integration.