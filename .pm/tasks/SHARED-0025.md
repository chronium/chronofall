---
id: SHARED-0025
title: Specify .cfbundle v1 after an activation trigger
track: SHARED
priority: none
dependsOn:
- COORD-0013
createdAt: 2026-08-05T08:27:49.0367330Z
modifiedAt: 2026-08-05T08:27:54.8854850Z
---

## Purpose

When a concrete activation trigger has been recorded, turn the deferred Pressure Cooker architecture into the separately reviewed byte-level `.cfbundle` v1 specification.

## Activation gate

Do not activate this task until at least one of these is demonstrated and recorded:

- a selected textured multi-part asset needs capabilities that `.cfskel` or `.cfmesh` cannot provide cleanly;
- the owner approves a local proprietary-wing presentation proof;
- a second real consumer makes the current single-asset formats materially obstructive;
- measured cooker, staging, loading, or validation friction justifies migration.

## Scope

- Freeze the complete binary grammar, canonical encoding, deterministic ordering and alignment, stable logical asset identities, dependency encoding, entry and bundle digests, classification fields, and future external-signing extension point.
- Define bundle-reader validation separately from catalog-wide graph and classification validation.
- Freeze hostile-input limits, codec budgets, canonical floating-point treatment, safe-output replacement, and dependency-closure digest semantics.
- Produce golden byte fixtures, malformed fixtures, and cross-platform conformance expectations.
- Groom separately reviewable implementation tasks for the reader/writer, codecs, Pressure Cooker composition, public migration, optional private integration, and later legacy cleanup.

## Acceptance boundary

- The specification is implementation-ready and validated against macOS ARM64 and Linux x64 BCL conformance expectations, Debug and Release, multiple cultures, and shuffled input enumeration.
- Classification ordering is `RepositoryTrackable < ReleaseReviewRequired < RestrictedLocalOnly`; an entry may depend only on entries of equal or lower restriction.
- Direct entry digests exclude dependency contents; raw and hydrated cache identities are unambiguous.
- SHA-256 is described as corruption detection, not authenticity.
- Bistro and later PBR work extend codecs or capabilities rather than inflating v1 or SimpleLit.
- No production implementation or child migration occurs in this task.

## Exclusions

No scene/world format, prefab/component model, streaming, patching, package manager, signing service, encryption, DRM, private-key work, whole-library cooking, child source mutation, or M2 dependency rewiring.