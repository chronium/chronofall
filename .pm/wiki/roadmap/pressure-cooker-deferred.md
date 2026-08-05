---
title: Deferred Pressure Cooker Roadmap
createdAt: 2026-08-05T08:30:12.4988930Z
modifiedAt: 2026-08-05T09:33:25.6374750Z
---

## Status

Pressure Cooker and `.cfbundle` are deliberately on the back burner.

This roadmap preserves the reviewed direction without turning it into an active program. No work in this roadmap belongs to Starfall M2. Current `.cfskel` and `.cfmesh` tasks, dependencies, cooks, staging flows, and consumers remain unchanged.

Architecture: [Pressure Cooker and deferred cooked bundles](pm://project/prj_E7QP3LUocfY7k3PYM-EQOlqc/wiki/architecture/pressure-cooker-and-cfbundle).

## Activation triggers

Before selecting SHARED-0025, a planning pass must identify and record at least one concrete trigger:

- a selected textured multi-part model cannot be represented cleanly by the current focused formats;
- the owner approves a development-only private wing proof;
- a second real consumer demonstrates that `.cfskel` materially obstructs reuse;
- measured cooker, staging, loading, validation, or artifact-management friction justifies migration.

Availability of a large source library or the theoretical usefulness of bundling is not a trigger. No task may cook a whole vendor pack merely because a multi-asset container exists.

## Allocated dormant spine

### COORD-0013 — Record deferred Pressure Cooker architecture

Coordinator-owned documentation and grooming task for this decision. It creates the durable pages and minimal dormant spine; it does not implement content infrastructure.

### SHARED-0025 — Specify .cfbundle v1 after an activation trigger

- Project: ChronoFall coordinator
- Track: SHARED
- Milestone: none
- Priority: none
- Dependency: COORD-0013
- State after this decision cycle: todo

This is the next permissible Pressure Cooker task only after a trigger is approved. It freezes the binary grammar, fixtures, conformance contract, and bounded implementation decomposition. It does not implement or migrate the format.

### COORD-0014 — Implement declared-classification and fingerprint scanner

- Project: ChronoFall coordinator
- Track: COORD
- Milestone: none
- Priority: none
- Dependency: SHARED-0025
- State after this decision cycle: todo and waiting

This first repository-safety primitive enforces structurally validated `.cfbundle` declarations and a trusted, versioned policy of explicitly known forbidden SHA-256 fingerprints and reviewed minimum-classification constraints. It scans staged trees and outgoing history, including blobs introduced and later deleted.

It does not infer provenance, ownership, confidentiality, or licensing status from arbitrary GLB, FBX, texture, archive, or other blob contents. A bundle declaration is enforceable but not self-proving; misclassification is detectable only when independent trusted policy evidence exists. Hook installation, CI adoption, protected-branch configuration, policy maintenance, and child adoption remain distinct later work.

## Unallocated future stages

The following are architectural stages, not task IDs. They must be groomed only when the active trigger and then-current source graph establish exact boundaries.

1. **Container foundation**
   - deterministic reader/writer and bundle-local validation;
   - catalog-wide dependency, classification, cycle, and closure validation;
   - stable logical lookup, raw cache identity, and dependency-closure digest;
   - golden and malformed fixtures plus cross-platform BCL conformance.

2. **Pressure Cooker composition**
   - recipe semantics and deterministic multi-asset composition;
   - importers/codecs split by actually selected formats;
   - provenance and classification emission;
   - temporary-sibling validation and atomic destination replacement.

   Do not create one task that combines every recipe, importer, codec, private-root rule, and migration.

3. **Public catalog and current-character migration**
   - coordinator-owned public shared bundles;
   - owning-repository game-specific public bundles;
   - stable-project-ID staging for coordinator assets into a child's ignored runtime output;
   - Starfall public catalog resolution and technical-character migration;
   - separate later family cleanup of legacy readers after consumers have moved.

4. **Repository safeguards**
   - declared-classification and trusted-fingerprint scanner;
   - pre-commit staged-tree integration;
   - pre-push outgoing-history integration;
   - coordinator CI;
   - Starfall and Royale adoption only when each repository schedules it.

5. **Optional private-content integration**
   - external private bundle-root discovery;
   - Starfall guard adoption;
   - precise absent/corrupt/unsupported/unresolved diagnostics;
   - visible shared or Starfall-owned ERROR fallback;
   - clean-clone validation;
   - local wing presentation proof, if the owner still wants it.

Public migration does not wait for optional private integration or the full guard rollout. Private integration does wait for the applicable guard adoption.

## Intended order

```text
concrete reviewed trigger
  -> SHARED-0025 binary grammar and fixtures
    -> bounded container and catalog tasks
      -> selected codecs and Pressure Cooker composition
        -> public catalog and current-character migration
          -> later family legacy cleanup

SHARED-0025
  -> COORD-0014 declared-classification and fingerprint scanner
    -> separate hook and CI adoption tasks
      -> guarded optional private-root integration
        -> optional Starfall prototype-wing proof
```

A selected asset may change the codec branch. Unknown monster, wing, material, or animation inputs must not produce speculative generic pipelines.

## Ownership checkpoints

- ChronoFall owns reusable format, cooking, reading, shared presentation types, and shared/curated/diagnostic assets.
- Starfall and Royale own exact product selections, game-specific bundles, bindings, fallback policy, and migration timing.
- The coordinator may aggregate and stage catalogs without taking ownership of child content.
- Only native presentation hosts may read presentation bundle configuration.
- Headless products remain independent of bundle bytes and private roots.
- Later removal of a legacy reader belongs to the repository containing that reader.

## Explicitly unchanged

This decision does not:

- modify ASSET-0004 through ASSET-0008 or any other completed/current asset task;
- add a dependency to Starfall or Royale;
- create a Starfall M2 task;
- alter generated-content staging;
- inspect or copy private source;
- add hooks, CI, signing, keys, encryption, DRM, package feeds, or runtime manifests;
- select a wing, implement wing gameplay, or merge public first-wings work with a development proof.

## Planning rule

When a trigger occurs, enter a fresh owner-directed Plan-mode cycle. Re-read the family, current formats, selected source evidence, consumers, and task graph. Select SHARED-0025 only if it remains the narrowest correct next owner. Stop for approval before activation.