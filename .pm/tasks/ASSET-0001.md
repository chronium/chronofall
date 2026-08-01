---
id: ASSET-0001
title: Inventory supplied character and animation assets
track: ASSET
milestone: M1
createdAt: 2026-08-01T05:34:30.8893020Z
modifiedAt: 2026-08-01T07:17:31.6553560Z
---

Inventory only the supplied Quaternius character and animation packs. Record exact filenames and formats, embedded/external resources, skeleton hierarchy and joint count, bone naming, weights and influence count, inverse-bind matrices, animation channels/interpolation/timing, coordinate system and scale, skeleton compatibility evidence, and required conversion steps. Do not process the whole collection or invent retargeting.

Acceptance criteria:
- Evidence is reproducible from the checked-in files.
- One candidate humanoid, idle, locomotion, and compatible attack are identified only if supported.
- Any incompatibility is reported with the smallest follow-up experiment.

## Notes

- 2026-08-01 07:17 UTC - Published `pm://project/prj_E7QP3LUocfY7k3PYM-EQOlqc/wiki/assets/character-animation-inventory` from the checked-in Quaternius files. Deep inspection found a shared ordered 65-joint hierarchy, four normalized influences, 65 finite float MAT4 inverse binds, and 43 clips per UAL library with complete LINEAR TRS channels at approximately 30 Hz. UAL1 provides provisional non-RM `Idle_Loop`, `Walk_Loop`, and `Sword_Attack` candidates, but base-versus-mannequin rest/inverse-bind differences mean deformation compatibility is not yet proven; the documented smallest follow-up applies one clip by exact joint identity without retargeting. Also recorded three broken supplied normal-map URIs. No asset, loader, conversion, rendering, or child-repository change was made.