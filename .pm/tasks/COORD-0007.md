---
id: COORD-0007
title: Add owner-curated visual checkpoint workflow
track: COORD
createdAt: 2026-08-01T16:16:03.0470210Z
modifiedAt: 2026-08-01T16:18:18.3123940Z
---

Make visually meaningful milestones discoverable and owner-curated instead of relying on an agent to remember them incidentally.

## Implemented scope

- Added a coordinator `AGENTS.md` policy requiring agents to actively assess visual work for project-history candidates.
- Defined candidate heuristics: first working capability, milestone closure, meaningful before/after, explanatory architecture/debug view, or another clear project transition.
- Defined exclusions: routine regression screenshots, near-duplicates, temporary noise, and raw capture dumps.
- Required an explicit owner choice before retention:
  1. preserve as-is;
  2. revise camera, framing, crop, overlays, labels, timestamps, selected frames, or composition;
  3. skip preservation.
- Clarified that visual acceptance and permanent retention are separate decisions.
- Updated the existing build-validation skill with the operational notification and preservation gate; no ceremonial skill was added.
- Expanded `docs/project-history/README.md` with candidate selection, owner approval, provenance, and repository-ownership rules.
- Created `pm://project/prj_E7QP3LUocfY7k3PYM-EQOlqc/wiki/development/visual-checkpoints` as the durable workflow page.
- Kept raw captures ignored; approved coordinator derivatives use `docs/project-history/<YYYY-MM-DD>-<slug>/`.
- Routed Royale and Starfall artifacts through their producing child repositories and child-owned PM/commit workflows. The coordinator must notify the owner but cannot silently mutate a child, copy its artifact, or advance a gitlink.
- Recorded the stable dated layout as input to a future PM wiki image/timeline task without implementing that capability.

## Validation

- PM project and linked family resolve with zero warnings.
- PM doctor and MCP validation pass.
- `chronofall-build-validation` frontmatter parses with exactly the required `name` and `description` fields.
- The prescribed `quick_validate.py` could not start because its environment lacks the Python `yaml` module; no dependency was installed. Equivalent YAML and structural validation passed with Ruby's built-in YAML parser.
- Markdown and `git diff --check` validation pass.
- Royale and Starfall files, PM data, commits, and gitlinks remain unchanged.
- No family task other than `COORD-0007` was activated.

## Completion contract

Commit only the coordinator task/wiki, `AGENTS.md`, existing build-validation skill, and project-history convention. No screenshot is automatically retained by this policy, and no PM wiki image/timeline support is implemented.