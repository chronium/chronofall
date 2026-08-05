---
id: COORD-0014
title: Implement declared-classification and fingerprint scanner
track: COORD
priority: none
dependsOn:
- SHARED-0025
createdAt: 2026-08-05T08:28:09.2452580Z
modifiedAt: 2026-08-05T09:32:54.8587950Z
---

## Purpose

After `.cfbundle` v1 is specified and restricted-content work is actually scheduled, implement the coordinator-owned scanner that enforces declared bundle classifications and a trusted policy of explicitly known blob fingerprints.

## Detection boundary

The scanner does not infer provenance, ownership, licensing status, or confidentiality from arbitrary bytes. A GLB, FBX, texture, archive, or other opaque blob is prohibited only when its exact digest is present in the trusted policy. A `.cfbundle` classification is an untrusted declaration that the scanner can validate and enforce; the declaration is not proof that the content was classified truthfully.

Deliberate or accidental bundle misclassification is detectable only when the trusted policy supplies an independent constraint, such as a minimum classification for a canonical bundle or asset identity or an exact known-forbidden digest.

## Scope

- Read classification only from structurally validated `.cfbundle` content, never from filenames or extensions.
- Consume a tracked, versioned coordinator policy containing explicit forbidden SHA-256 blob fingerprints and, where reviewed evidence exists, minimum classifications for canonical bundle or asset identities.
- Scan an explicitly supplied staged tree against declared bundle classifications and the trusted policy.
- Scan every new blob introduced by explicit outgoing commit ranges, including content committed and deleted before the range tip.
- Report exact commits, blob identities, paths where available, the factual detection basis, and actionable remediation.
- Keep scanning deterministic, bounded, non-destructive, and independent of child game code.
- Define the stable command contract that later pre-commit, pre-push, and CI adoption tasks may invoke.

## Acceptance boundary

- Renaming a `.cfbundle` cannot evade enforcement of its embedded declared classification.
- Renaming an explicitly fingerprinted forbidden blob cannot evade its policy match.
- Unknown arbitrary source blobs are not reported as proprietary or redistributable merely from format or contents.
- A bundle whose declaration conflicts with a trusted minimum-classification rule is rejected; without such a rule, the scanner makes no claim that the declaration is truthful.
- Policy fixtures distinguish declared classification, trusted fingerprint evidence, and minimum-classification evidence in diagnostics.
- Staged-tree and outgoing-history fixtures cover additions, renames, copies, deletions after introduction, malformed bundles, false-positive boundaries, and multiple ranges.
- The policy contains digests and canonical identities rather than private source paths, and the scanner never reads private-source roots merely to perform repository validation.
- Client hooks and CI are explicitly separate consumers; this task does not claim they are unbypassable.
- No general license classifier, heuristic proprietary-content detector, server-side pre-receive hook, signing, encryption, repository rewrite, or child hook adoption is included.

## Activation gate

Do not activate merely because the deferred architecture exists. Activate only after SHARED-0025 establishes the binary classification contract and a planned private or release-review-required workflow creates a concrete enforcement need.