---
title: Initial Family Roadmap
createdAt: 2026-08-01T05:44:07.0156650Z
modifiedAt: 2026-08-02T11:44:59.2588230Z
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

`SHARED-0001` promotes only validated contracts. Follow-ups cover skeletal cooking, canonical-rig armour, slots/body hiding, variants, sockets/attachments, weapons/shields/backpacks/wings, blending/layers, grip/effect/aim points, two-bone IK, debugging, and preview tooling.

Royale owns linked tasks `RENDER-012`, `GAME-018`, `RENDER-013`, and `EDITOR-030`; each uses canonical dependencies on the parent shared tasks. After a child task commit, the coordinator records the Royale gitlink in an automatic pointer-only commit owned by that child task; no separate parent PM task is created.

## M3 — MMO bootstrap

Starfall owns its independent roadmap:

- M0 foundation: `ARCH-0004` defines authority/project boundaries; `BUILD-0002` creates the repository/solution; `PROTOCOL-0002` establishes command/snapshot ownership; `EDITOR-0003` establishes editor and Balance Lab boundaries.
- M1 shared presentation: `CLIENT-0006` integrates the parent foundation; `CONTENT-0004` maps truthful armour/weapons; `CLIENT-0007` presents class actions/skills; `CLIENT-0008` later presents wings, mounts, and companions.
- M2 first playable zone: `CONTENT-0003`, `SERVER-0002`, `CLIENT-0005`, `SIM-0003`, `SIM-0004`, `GAME-0002`, and `EDITOR-0004` cover one class/zone, one world/channel, owner-approved controls, shaped monster spots, basic attack/AoE, experience/drops/visible equipment, and authoritative headless balance simulation.

The Starfall tasks use local dependencies plus canonical parent dependencies. Completed `SUBMODULE` tasks remain historical evidence of the earlier workflow, while future child commits receive automatic pointer-only coordinator commits without separate PM tasks. Broader persistence, accounts, economy, trade stands, full wings progression, territory, and complete release scope remain in the design document.

During bootstrap, the initialized Starfall project initially lacked empty task/state directories. Linked creation failed and the owning CLI consumed next-ID allocations before reporting the missing path. Running `pm doctor` supplied the supported scaffolding; linked MCP then created every task with receipts targeting `prj_pkIpzx0fzFD4URjvqBuYrGZF`. The resulting non-contiguous first IDs are service-issued and intentionally preserved.

`COORD-0005` records the later typed-authoring/compiled-runtime exploration without starting a generic component framework.