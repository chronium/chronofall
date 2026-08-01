---
name: chronofall-review
description: Review ChronoFall coordinator, linked PM, shared-engine, submodule, asset, or child integration changes. Use for diff/commit review, ownership mistakes, reciprocal identity warnings, canonical dependency defects, trust/receipt violations, authority leaks, child-to-child dependencies, premature abstraction, native risk, missing validation, or PM/wiki omissions.
---

# ChronoFall Review

## Review In Ownership Context

Read the owning task with its project selector, relevant wiki, parent/child policies, linked family status, and each repository diff separately. Confirm the selected task owns the changed files and that no PM identity was interpreted through another project's configuration.

## Prioritize Findings

1. Wrong repository, identity mismatch, destructive behavior, or data loss.
2. Server-authority or headless dependency leaks.
3. Invalid/unavailable cross-project dependencies, alias persistence, missing trust, or receipt mismatch.
4. Royale-to-Starfall or Starfall-to-Royale dependencies; parent modules depending on children.
5. Loader/file-format/native ABI/resource-lifetime defects.
6. Determinism, skinning, animation, asset-cooking, or packaging regressions.
7. Mixed child/parent commits, incorrect gitlinks, or dirty-worktree loss.
8. Missing tests, owner visual validation, PM notes, wiki updates, provenance, or doctor results.
9. Premature generic engine/component/animation abstractions.

Report findings first with concrete path and line references. Distinguish confirmed defects from questions and remaining risk.

## Completion Checks

Verify mutation receipts, canonical URIs, dependency readiness, task state, PM doctor for every mutated project, repository-specific validation, clean child commits before gitlink updates, and task-scoped commits. If no findings exist, say so and list untested native, visual, platform, or child-resolution risk.
