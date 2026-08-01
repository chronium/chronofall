---
id: COORD-0002
title: Establish linked PM and agent workflow
track: COORD
milestone: M0
createdAt: 2026-08-01T05:34:30.1729980Z
modifiedAt: 2026-08-01T05:48:47.0102130Z
---

Add coordinator policy and focused repository-local skills for Plan-mode selection, linked ownership, canonical references, write trust, mutation receipts, one-task execution, validation, review, and completion. Document that planning never activates work and completion ends the cycle.

Acceptance criteria:
- `AGENTS.md` encodes the owner-approved cadence and authority boundaries.
- Skills contain repository-specific procedures rather than repeating policy.
- PM mutations remain MCP-first and preserve owning projects.

## Notes

- 2026-08-01 05:48 UTC - Added coordinator AGENTS.md, seven focused repository-local skills with generated UI metadata, linked PM workflow wiki, and canonical selection/execution gates. Official quick_validate could not start because PyYAML is absent; Ruby YAML parsing passed for every SKILL.md frontmatter and openai.yaml, and no scaffold TODOs remain.