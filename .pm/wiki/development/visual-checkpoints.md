---
title: Owner-Curated Visual Checkpoints
createdAt: 2026-08-01T16:17:30.3180850Z
modifiedAt: 2026-08-01T16:17:30.3180850Z
---

## Purpose

ChronoFall should remember meaningful visual progress without turning the repository into a screenshot dump. Agents must actively notice candidate checkpoints and offer them to the owner before they disappear into ignored artifacts.

Owning policy task: `pm://project/prj_E7QP3LUocfY7k3PYM-EQOlqc/task/COORD-0007`.

## Candidate gate

Offer the strongest image or compact visual artifact when it records:

- a first working capability;
- milestone closure;
- a major before/after;
- an explanatory architecture or debug view;
- another clear project transition.

Do not prompt for routine regression screenshots, near-duplicates, temporary noise, or raw capture collections.

Show the candidate and ask the owner to choose:

1. preserve it as-is;
2. revise camera, framing, crop, overlays, labels, timestamps, frame selection, or composition first;
3. skip it.

Visual acceptance and permanent retention are separate decisions. Nothing is committed automatically.

## Retention contract

Raw captures and intermediate conversions stay ignored. An approved coordinator artifact is reduced to the smallest useful curated derivative and stored at `docs/project-history/<YYYY-MM-DD>-<slug>/` with:

- a dated timeline entry;
- canonical PM task/wiki ownership;
- source and licence provenance;
- generation and reproduction notes;
- an explanation of what it records;
- a content hash.

Curated history is documentation, never runtime content or a build-manifest input.

## Family ownership

An artifact belongs to the repository that produced the milestone. Royale and Starfall candidates must be preserved through their owning child's PM, documentation, validation, and commit workflow. The coordinator must not copy child artifacts or advance gitlinks incidentally. A family-level coordinator artifact requires an explicit owner decision and links back to child-owned evidence.

## Future wiki timeline

The dated repository-relative layout is deliberately stable for future PM wiki image and timeline support. Until that capability has its own approved task, the Git-tracked history index remains authoritative and wiki pages may record paths without assuming they render images.