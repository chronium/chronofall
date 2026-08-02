---
title: Starfall Draft 0 Coordinator Enablers
createdAt: 2026-08-02T16:21:34.5039080Z
modifiedAt: 2026-08-02T16:31:11.9301930Z
---

## Purpose and ownership

This page records the coordinator-owned enablers for Starfall's provisional Draft 0 first playable zone. Starfall owns game-specific selection, content identity, composition, simulation, protocol, and presentation integration. ChronoFall owns supplied-source provenance, reusable client presentation/cooking contracts, stable-project-ID staging, and exact selected-input acquisition.

Availability is not selection. A prospective pack is not a dependency until its files are physically supplied, inventoried, licensed, and selected by the owning Starfall task. No entire pack enters a cook or runtime manifest. Generated client output remains ignored.

Starfall's design brief is `pm://project/prj_pkIpzx0fzFD4URjvqBuYrGZF/wiki/product/first-playable-zone-draft-0`. Coordinator grooming task `COORD-0009` canonically depends on completed Starfall grooming task `pm://project/prj_pkIpzx0fzFD4URjvqBuYrGZF/task/SF-0008`, which is the authoritative source of this decomposition.

## Shared capability graph

```text
SHARED-0001 completed character-presentation host boundary
  -> SHARED-0018 narrow reusable static-mesh rendering

SHARED-0017 completed fresh-checkout-safe stable-ID staging
SHARED-0018 static rendering
  -> SHARED-0019 deterministic static-asset cooking and staging extension

SHARED-0006 completed socket transform contract
SHARED-0018 static rendering
ASSET-0006 exact selected bow/arrows
  -> SHARED-0020 one rendered socketed static bow proof

SHARED-0004 equipment slots/body hiding
SHARED-0006 sockets
SHARED-0020 narrow proof
  -> SHARED-0007 broader weapons/shields/backpacks/wings
```

`SHARED-0018` and `SHARED-0019` remain narrow client presentation contracts. They do not authorize a scene graph, render graph, terrain, vegetation, streaming, general material system, asset catalogue, importer framework, or child integration.

`SHARED-0020` proves exactly one selected bow through the existing caller-owned SDL GPU lifecycle. It excludes armour, IK, projectile behavior, combat, shields, backpacks, and wings. The later broad `SHARED-0007` task must review and reuse this proof instead of recreating it independently. Existing `SHARED-0007` consumers remain unchanged.

## Exact acquisition graph

| Coordinator task | Owning selection | Known coordinator prerequisites | Result |
| --- | --- | --- | --- |
| `ASSET-0004` archer and bow animations | `pm://project/prj_pkIpzx0fzFD4URjvqBuYrGZF/task/CONTENT-0011` | `SHARED-0002`, `SHARED-0017` | Exact base/underlayer and minimum compatible bow clips |
| `ASSET-0005` Ranger equipment | `pm://project/prj_pkIpzx0fzFD4URjvqBuYrGZF/task/CONTENT-0011` | `SHARED-0004`, `SHARED-0017` | Exact selected Ranger/leather pieces after modular-armour and body-hiding contracts |
| `ASSET-0006` bow and arrows | `pm://project/prj_pkIpzx0fzFD4URjvqBuYrGZF/task/CONTENT-0011` | `SHARED-0019`, `SHARED-0017` | Exact selected static weapon inputs |
| `ASSET-0007` zone presentation | `pm://project/prj_pkIpzx0fzFD4URjvqBuYrGZF/task/CONTENT-0012` | `SHARED-0019`, `SHARED-0017` | Exact village/nature/prop/graybox inputs |
| `ASSET-0008` monster presentation | `pm://project/prj_pkIpzx0fzFD4URjvqBuYrGZF/task/CONTENT-0013` | `SHARED-0017` initially | Exact selected temporary-monster inputs, if any |

All five acquisitions depend on still-todo canonical Starfall selection tasks, so every acquisition remains blocked. Acquisition tasks consume completed selections and may stage only exact selected inputs with pack-relative paths, hashes, licence evidence, format/scale/material/rig evidence, intended consumer, and any deliberate conversion.

`ASSET-0008` deliberately has no static-render, static-cook, or skeletal-cook prerequisite yet. Starfall selection must first determine whether the candidate is static, rigidly animated, skeletal, or unsuitable. Only a later reviewed task-planning or grooming continuation may attach the smallest demonstrated prerequisite. It must never add both paths defensively or create a generic monster pipeline.

## Source direction

Established source policy remains:

- Quaternius for humanoid characters, the reference skeleton, animations, modular armour/clothing, and `Medieval Village MegaKit[Standard]`;
- Kenney and Quaternius as task-owned weapon candidates;
- Kenney prototype geometry, textures, UI/editor/debug assets, markers, and individually approved placeholders as available development sources.

Prospective Quaternius sources remain evidence-gated:

- Universal Animation Library 2 Full/Source for a minimum compatible bow clip set;
- Modular Sci-Fi MegaKit as the preferred first temporary-monster inspection;
- Ultimate Monsters as fallback;
- Stylized Nature MegaKit for a tiny exact nature subset;
- Fantasy Props MegaKit for optional landmark-only dressing;
- Ultimate RPG Pack as a deferred, unselected pickup-art candidate.

The existing UAL1 cook and its technical evidence remain unchanged. `Sword_Attack` is not an acceptable bow placeholder. No prospective source is considered supplied, compatible, selected, or dependency-ready merely because an official pack exists.

## Staging and audience

Every acquisition must resolve the consuming checkout by stable PM project identity and committed path hint. It must refuse to write unless the exact owned generated-output tree is ignored, untracked, free of tracked files and symlink escapes, and limited to the approved output set.

Raw supplied sources remain coordinator-owned. Generated cooks, provenance sidecars, and copied licence evidence are client-only and ignored. World/server, simulation, Balance Lab, protocol, and content artifacts receive no rendering payload or native dependency.

Package/feed distribution remains deferred. The canonical family checkout and `ChronoFallFamilyRoot` source boundary remain the approved development environment.

## Later Starfall wiring

Cycle 2 creates only coordinator tasks and documentation. It does not modify Starfall. After these coordinator IDs are reviewed, the approved Cycle 3 continuation may reopen Starfall `SF-0008` solely to add the exact canonical dependencies and matching roadmap corrections already planned. It must record receipts, complete the same grooming task, commit Starfall, and perform the mechanical pointer handoff before stopping.