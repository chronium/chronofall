---
id: COORD-0014
title: Implement restricted-content blob scanner
track: COORD
priority: none
dependsOn:
- SHARED-0025
createdAt: 2026-08-05T08:28:09.2452580Z
modifiedAt: 2026-08-05T08:28:12.3741340Z
---

## Purpose

After `.cfbundle` v1 is specified and restricted-content work is actually scheduled, implement the coordinator-owned blob scanner that underpins later family Git safeguards.

## Scope

- Recognize bundle classification from validated blob contents rather than names or extensions.
- Scan an explicitly supplied staged tree for prohibited private source and restricted cooked content.
- Scan every new blob introduced by explicit outgoing commit ranges, including content committed and deleted before the range tip.
- Report exact commits, blob identities, paths where available, classification evidence, and actionable remediation.
- Keep scanning deterministic, bounded, non-destructive, and independent of child game code.
- Define the stable command contract that later pre-commit, pre-push, and CI adoption tasks may invoke.

## Acceptance boundary

- Renaming a restricted bundle cannot evade classification.
- Staged-tree and outgoing-history fixtures cover additions, renames, copies, deletions after introduction, malformed bundles, false-positive boundaries, and multiple ranges.
- The scanner never reads private-source roots merely to perform repository validation.
- Client hooks and CI are explicitly separate consumers; this task does not claim they are unbypassable.
- No server-side pre-receive, signing, encryption, repository rewrite, or child hook adoption is included.

## Activation gate

Do not activate merely because the deferred architecture exists. Activate only after SHARED-0025 establishes the binary classification contract and a planned private or release-review-required workflow creates a concrete enforcement need.