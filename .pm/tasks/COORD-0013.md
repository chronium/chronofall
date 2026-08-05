---
id: COORD-0013
title: Record deferred Pressure Cooker architecture
track: COORD
priority: none
createdAt: 2026-08-05T08:27:08.7759560Z
modifiedAt: 2026-08-05T09:33:34.4959440Z
---

## Purpose

Record the owner-reviewed deferred Pressure Cooker and `.cfbundle` direction as durable coordinator architecture and roadmap input.

## Scope

- Create the coordinator architecture page for the license-neutral, deterministic, multi-asset `.cfbundle` direction.
- Create the deferred roadmap page with explicit activation triggers and future ownership boundaries.
- Allocate only a minimal dormant future-task spine for specification and later guard planning.
- Record the current `.cfskel` and `.cfmesh` formats as the active supported path until evidence triggers a separately planned migration.
- Preserve ChronoFall, Starfall, Royale, and coordinator ownership boundaries.

## Acceptance criteria

- The architecture distinguishes bundle-local structural validation from catalog-wide dependency resolution and classification validation.
- Classification ordering and dependency-closure rules are explicit.
- Integrity, digest/cache semantics, runtime budgets, deterministic output, safe replacement, private-root policy, and future external-signing extension are documented.
- The document states that it is architectural direction rather than the final byte-level v1 grammar.
- Public coordinator assets, game-specific child assets, catalog aggregation, and native-editor versus headless ownership are distinguished.
- The roadmap records concrete activation triggers and bounded future stages without assigning this work to M2.
- Existing `.cfskel`, `.cfmesh`, and ASSET task contracts and dependencies remain unchanged.
- No source, assets, generated output, child repository, or gitlink changes are made.
- Coordinator PM validation and repository diff checks pass.

## Exclusions

No bundle implementation, cooker implementation, migration, private content inspection, content guards, hooks, CI, asset copying, signing, child mutation, or dependency rewiring.

## Notes

- 2026-08-05 08:30 UTC - Created the deferred architecture and roadmap pages:

  - pm://project/prj_E7QP3LUocfY7k3PYM-EQOlqc/wiki/architecture/pressure-cooker-and-cfbundle
  - pm://project/prj_E7QP3LUocfY7k3PYM-EQOlqc/wiki/roadmap/pressure-cooker-deferred

  Allocated the minimal dormant spine:

  - SHARED-0025, unmilestoned with priority none, dependent on COORD-0013; owns only the future reviewed byte-level specification after a recorded activation trigger.
  - COORD-0014, unmilestoned with priority none, dependent on SHARED-0025; owns only the future content-aware blob scanner.

  Every mutation receipt named owning project prj_E7QP3LUocfY7k3PYM-EQOlqc and only coordinator .pm paths. PM MCP validation and pm doctor passed with no issues. The linked family reported zero warnings. Royale and Starfall remained clean at their existing pins; no child, gitlink, source, asset, generated-output, existing ASSET task, or dependency changes were made.
- 2026-08-05 09:33 UTC - Review continuation: corrected the scanner contract after confirming that arbitrary blob bytes cannot establish proprietary status or truthful classification. COORD-0014 now enforces structurally validated .cfbundle declarations plus a tracked trusted policy of explicit SHA-256 fingerprints and reviewed minimum-classification constraints. The architecture and deferred roadmap now state the same detection limit. Mutation receipts targeted only coordinator project prj_E7QP3LUocfY7k3PYM-EQOlqc and the expected COORD-0014 and two wiki paths; no dependencies, source, assets, children, or gitlinks changed.