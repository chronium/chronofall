---
title: Pressure Cooker and Deferred Cooked Bundles
createdAt: 2026-08-05T08:30:12.5053670Z
modifiedAt: 2026-08-05T08:30:12.5053670Z
---

## Decision status

This page records an approved **deferred architectural direction**. It does not establish a byte-level format, authorize implementation, or replace the current cooked formats.

The working product name is **Pressure Cooker**. Its eventual engine-native container is referred to as `.cfbundle`. The current `.cfskel` and `.cfmesh` readers, writers, staging flows, tests, and task dependencies remain authoritative until a concrete activation trigger is reviewed and a migration is separately planned.

The durable objective is a deterministic, license-neutral cooked container that can hold a curated set of complete presentation assets and multiple logical assets without turning ChronoFall into a scene format, archive filesystem, or package manager.

## Ownership

ChronoFall owns:

- the reusable `.cfbundle` specification, deterministic reader and writer, Pressure Cooker composition contracts, reusable presentation asset types, and shared loading capabilities;
- shared or coordinator-curated cooked assets;
- genuinely shared diagnostic content, such as a future `chronofall/diagnostic/error-model`, if evidence shows that both games need it.

Starfall owns:

- the identities and selection of Starfall-specific assets;
- logical presentation bindings, wing attachment and animation integration;
- optional private-bundle discovery and missing-content behavior;
- Starfall-specific cooked bundles and fallbacks;
- the native presentation-facing editor host that may consume the catalog and renderer.

Royale owns its own selections, bindings, bundles, and migration decisions.

The coordinator may aggregate or stage catalog inputs through stable linked-project identity, but aggregation does not transfer ownership of a child bundle to ChronoFall. Parent-owned shared modules must not depend on either child, and the children must never depend directly on one another.

World, Simulation, Protocol, Content, Balance Lab, and headless editor document/compilation code remain independent of presentation bundles, renderers, native UI, and private roots. A native editor host may use client-side presentation systems without leaking those dependencies into its authoritative document model or compiler.

## Stable logical identity

Every cooked asset has a canonical logical identity that is independent of:

- source filename or absolute path;
- vendor or source family;
- bundle filename;
- table ordinal, payload offset, or physical bundle location.

An asset may move between bundles without changing a Starfall or Royale logical reference. Source provenance records exact pack-relative source inputs separately from runtime identity.

Candidate bundle names such as `kenney-draft0-graybox.cfbundle` or `quaternius-draft0-character.cfbundle` are operational examples, not permanent identity or mandatory bundle boundaries.

## Intended asset envelope

A later reviewed v1 grammar may represent typed entries for:

- static meshes;
- skinned meshes;
- skeletons;
- animation clips;
- textures;
- materials;
- complete models referencing their mesh, material, texture, skeleton, and animation dependencies;
- narrowly scoped presentation metadata such as bounds or attachment information where an established shared contract requires it.

Bundles remain curated. Supporting multiple assets never authorizes cooking or redistributing an entire purchased library.

The initial material envelope should remain deliberately small: a SimpleLit-style model with the minimum proven base-color and alpha behavior. Bistro and later PBR benchmarks must extend the bundle through new codec versions or declared asset capabilities; they must not inflate `.cfbundle` v1 or SimpleLit merely to anticipate distant renderer work.

## Reader and catalog boundaries

A single-bundle reader validates only what one file can establish:

- magic, schema version, canonical bundle identity, table encoding, stable entry identities, kinds, dependency-list encoding, offsets, lengths, alignment, metadata, payload bounds, and integrity digests;
- duplicate identities within the bundle;
- codec-specific structural limits and canonical numeric values.

The reader does **not** claim that dependencies in other bundles exist or that a cross-bundle graph is acyclic.

Catalog assembly validates the complete accepted set of bundles:

- global duplicate logical identities;
- missing dependencies;
- cycles across bundle boundaries;
- dependency classification;
- deterministic lookup and dependency closure.

Pressure Cooker may accept declared external dependencies, but it validates them only against an explicitly supplied external catalog. It must not infer dependency availability from filenames or ambient directories.

Catalog traversal must be iterative or depth-safe. Dependency closures are validated as complete, acyclic, and classification-safe before closure digests or hydrated assets are produced.

## Classification

Classification records repository and release policy; it is not a legal conclusion or proof of redistribution rights.

The ordered classifications are:

```text
RepositoryTrackable
  < ReleaseReviewRequired
  < RestrictedLocalOnly
```

Each entry declares a classification. A bundle inherits the maximum classification of its entries.

An entry may depend only on entries with equal or lower restriction. Restricted content may depend on public content; an apparently public entry may not acquire a restricted material, texture, skeleton, or other dependency. Catalog validation enforces this rule across bundles.

Source confidentiality and cooked-output policy are separate. A private source package may produce a tightly curated cooked artifact that repository policy permits, while a purchased model may remain `RestrictedLocalOnly`.

## Determinism and integrity

The eventual container must have:

- fixed magic and schema version;
- canonical bundle identity;
- a deterministic asset table ordered by stable logical identity;
- explicit kinds and dependency identities;
- bounded offsets and lengths;
- canonical metadata encoding;
- deterministic payload alignment;
- per-entry and whole-bundle SHA-256 digests;
- direct indexed lookup without eagerly decoding every payload.

An entry digest covers the entry's canonical metadata, dependency identities, and payload; it does not incorporate dependency contents. It fingerprints the direct entry only.

A raw-entry cache therefore uses at least `(bundle digest, asset identity)`. A hydrated model or other dependent asset uses a separately computed dependency-closure digest after the catalog graph has passed validation. Changing a texture or skeleton beneath the same logical identity must invalidate the hydrated result.

SHA-256 provides corruption and accidental-change detection, not authenticity. Anyone able to replace a bundle can recompute its hashes. Authority comes from reviewed repository content, catalog policy, and, if later justified, an external signing system. A future external signing system may consume bundle identity, schema version, and canonical digest without changing the v1 container. No key custody or signing service belongs to this architecture.

BCL reader, writer, and cooker conformance must prove byte-identical output across macOS ARM64 and Linux x64, Debug and Release, multiple cultures, repeated runs, and varied input enumeration order. Native rendering proof may initially remain macOS ARM64.

## Hostile-input and runtime budgets

A valid file-size ceiling never authorizes an equivalent allocation.

The final grammar must freeze bounded maxima for:

- entries, dependencies, strings, metadata, individual payloads, and total file size;
- texture dimensions and decoded bytes;
- model parts and sections;
- joints, clips, channels, and keyframes;
- cumulative hydrated memory and graph work.

Hashing and structural inspection stream payloads where practical. Codecs apply stricter decoded-memory and element-count budgets than the outer file container when appropriate. All cumulative sizes use checked arithmetic.

Canonical floating-point payloads must be finite. The writer canonicalizes negative zero to positive zero; the reader rejects negative-zero bit patterns, NaN, and infinity where canonical encoding requires a unique representation.

Pressure Cooker writes to a temporary sibling, closes and flushes it, reopens it through production validation, and atomically replaces the destination only after success. A failed cook preserves the previous canonical artifact.

These are architectural invariants, not final numeric ceilings. Exact limits belong to the separately reviewed v1 binary specification.

## Current formats and migration

The present `.cfskel` artifact owns the proven technical humanoid's mesh, skin, skeleton, and animation data. The present `.cfmesh` path owns focused static-mesh data. Their current cooks, staging, readers, validation, native presentation, and historical capture evidence remain valid.

No compatibility reader is required merely because `.cfbundle` is planned.

When an activation trigger exists, migration should prefer one reviewed family sequence:

1. specify and prove the deterministic bundle container;
2. express the current technical humanoid and animations as ordinary typed bundle entries;
3. add required model, material, texture, and composition codecs only for selected inputs;
4. move public catalog staging and the current-character consumer to stable logical resolution;
5. preserve existing validation, animation behavior, native presentation, and historical evidence;
6. remove direct legacy staging/loading in a later family migration cycle owned by the repository containing each reader.

Public migration and optional private-root integration are separate stages. Public content must not wait on the complete restricted-content guard rollout.

## Private source and restricted cooked content

Private source packages remain outside every repository. No committed file may contain a machine-specific private path.

Optional local discovery may later use:

- `CHRONOFALL_PRIVATE_ASSET_ROOT` for private source available to approved cooking tasks;
- `CHRONOFALL_PRIVATE_BUNDLE_ROOT` for canonical restricted cooked bundles;
- ignored local configuration only where a reviewed workflow needs it.

Restricted canonical bundles should remain outside worktrees. A stable-ID staging workflow may copy only approved runtime inputs into an exact ignored output tree after validating ownership, repository identity, tracked-file absence, and symlink safety.

Clean clones must build and test without private content. Starfall's native Client may resolve a missing optional presentation asset to a visible tracked or generated diagnostic ERROR model. Absence of a wing or other private presentation asset must not change authoritative equipment, progression, simulation, protocol, or headless behavior.

A future local wing proof may validate complete textured or animated model loading, deterministic animation sampling, and attachment presentation. It does not implement flight, wing gameplay, unlocks, final selection, public release packaging, or progression.

## Repository safeguards

The intended protection is layered:

1. keep private source and restricted canonical outputs outside worktrees;
2. ignore exact local staging paths;
3. scan staged blob contents and bundle classification before commit;
4. scan every new blob in outgoing commit ranges before push, including content committed and later deleted;
5. run equivalent validation in coordinator and later child CI;
6. use protected branches where hosted repository policy supports them.

Names and extensions are insufficient: renaming a restricted bundle must not evade scanning.

`.gitignore`, local hooks, and pre-push checks are guardrails and can be bypassed. CI protects accepted branches but runs only after a remote receives a pushed branch. Without a controlled server-side pre-receive hook, keeping canonical private inputs and outputs outside the worktree is the strongest default protection.

Hook installation, CI wiring, and each child repository's adoption remain separately owned future work.

## Explicit non-goals

This direction is not:

- a scene, world, prefab, ECS, component, editor-document, or reflective object format;
- a terrain or streaming system;
- a patch service, archive filesystem, package manager, or private feed;
- an asset-library mirroring license;
- encryption, DRM, authenticated download, extraction resistance, or local key management;
- permission to cook whole Kenney, Quaternius, or commercial libraries;
- permission to introduce a permanent second content path;
- a reason to interrupt Starfall M2.

## PM ownership

- [COORD-0013](pm://project/prj_E7QP3LUocfY7k3PYM-EQOlqc/task/COORD-0013) records this decision.
- [SHARED-0025](pm://project/prj_E7QP3LUocfY7k3PYM-EQOlqc/task/SHARED-0025) owns the future reviewed byte-level specification after an activation trigger.
- [COORD-0014](pm://project/prj_E7QP3LUocfY7k3PYM-EQOlqc/task/COORD-0014) owns the first bounded repository-safety implementation after the specification and an actual enforcement need.

See [Deferred Pressure Cooker roadmap](pm://project/prj_E7QP3LUocfY7k3PYM-EQOlqc/wiki/roadmap/pressure-cooker-deferred).