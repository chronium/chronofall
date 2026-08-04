---
title: Starfall Draft 0 Coordinator Enablers
createdAt: 2026-08-02T16:21:34.5039080Z
modifiedAt: 2026-08-04T07:16:52.6925130Z
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

Modular-armour work is deliberately evidence-gated:

```text
SHARED-0002 completed shared skeletal cooking
Starfall CONTENT-0011 exact archer/underlayer/Ranger selection
  -> SHARED-0003 one canonical-rig modular-armour presentation proof
  -> SHARED-0004 equipment slots and body-region hiding
  -> ASSET-0005 exact Ranger acquisition and stable-ID staging
```

The canonical `CONTENT-0011` dependency makes `SHARED-0003` wait until Starfall identifies the exact concrete need and compatibility evidence. It does not transfer Starfall asset, item, equipment or composition ownership into the parent, and it does not make parent source depend on Starfall. Once the selection completes, `SHARED-0003` retains its inherited M2 priority and can be planned from evidence rather than speculation.

Coordinator `COORD-0011` subsequently reviewed and ratified the already-pushed `00c9224` dependency change under an active grooming/review lifecycle. The review preserved history, confirmed the canonical edge is valid and cycle-free, and left `SHARED-0003` todo and waiting; it did not activate or implement modular armour.

## Exact acquisition graph

| Coordinator task | Owning selection | Known coordinator prerequisites | Result |
| --- | --- | --- | --- |
| `ASSET-0004` archer and bow animations | `pm://project/prj_pkIpzx0fzFD4URjvqBuYrGZF/task/CONTENT-0011` | `SHARED-0002`, `SHARED-0017`, `EXPERIMENT-0014` | Exact base/underlayer and minimum compatible bow clips |
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

- the owner-supplied private Universal Animation Library 2 Source package, inventoried by `ASSET-0010`, for a minimum compatible bow clip set after the `EXPERIMENT-0014` technical bow-body visual proof;
- Modular Sci-Fi MegaKit as the preferred first temporary-monster inspection;
- Ultimate Monsters as fallback;
- Stylized Nature MegaKit for a tiny exact nature subset;
- Fantasy Props MegaKit for optional landmark-only dressing;
- Ultimate RPG Pack as a deferred, unselected pickup-art candidate.

The existing UAL1 cook and its technical evidence remain unchanged. `Sword_Attack` is not an acceptable bow placeholder. Physical availability alone never establishes compatibility, selection, or dependency readiness: the private UAL2 Source package remains outside the repository, its inventory is complete under `ASSET-0010`, and its technical proof remains todo under `EXPERIMENT-0014`; the other prospective packs remain unverified until physically supplied and reviewed.

## Bow-animation evidence lane

```text
ASSET-0010 private Source inventory and bounded candidate selection
  -> EXPERIMENT-0014 technical bow-body-animation sequence proof

Starfall CONTENT-0011 final character/bow/animation selection
EXPERIMENT-0014 completed proof
  -> ASSET-0004 exact production acquisition and client staging
```

`EXPERIMENT-0014` is todo and must not be activated without its own approved plan. Its output is body-animation evidence on the technical mannequin, not an equipped bow proof. A later Starfall-owned selection cycle may add the completed experiment's canonical URI to `CONTENT-0011`; this coordinator task does not mutate Starfall.

## Staging and audience

Every acquisition must resolve the consuming checkout by stable PM project identity and committed path hint. It must refuse to write unless the exact owned generated-output tree is ignored, untracked, free of tracked files and symlink escapes, and limited to the approved output set.

Raw supplied sources remain coordinator-owned. Generated cooks, provenance sidecars, and copied licence evidence are client-only and ignored. World/server, simulation, Balance Lab, protocol, and content artifacts receive no rendering payload or native dependency.

Package/feed distribution remains deferred. The canonical family checkout and `ChronoFallFamilyRoot` source boundary remain the approved development environment.

## Authoritative walking physics prerequisite

Starfall's provisional graybox and authoritative movement remain game-owned, but the reusable Box3D source/native/managed boundary belongs to ChronoFall:

```text
parent SHARED-0016 family-source policy
Royale PHYS-012 proven audited Box3D integration
  -> parent SHARED-0021 bounded shared Box3D runtime
  -> canonical waiting dependency from Starfall SIM-0008
```

`SHARED-0021` is dependency-ready and todo. It canonically consumes Royale `pm://project/prj__-jXLQgm6GuD2gCKZ_bTa1m-/task/PHYS-012` as evidence, without creating a Starfall-to-Royale dependency or authorizing coordinator changes in Royale. The parent task owns an independent pin, licence evidence, fetch/native-build workflow, minimum child-independent binding/ownership surface, and the explicit headless-safe `ChronoFallFamilyRoot` source boundary.

The initial surface is limited to authoritative ground-plane movement needs: world lifecycle/stepping, bodies, transforms/velocity, boxes/capsules, filtering, and bounded collision/query facts. Starfall continues to own entity identity, fixed-tick simulation policy, content conversion, movement rules and gameplay outcomes. Rendering, debug presentation, maps, collision cooking, generalized physics abstractions and child integration remain outside the coordinator task.

Cycle 2 allocated and documented the parent task without mutating Starfall. The separately owner-directed Starfall Box3D Cycle 3 then completed through `pm://project/prj_pkIpzx0fzFD4URjvqBuYrGZF/task/SF-0009`. Starfall commit `84b1c94d3d3413954a20b09ea1d0445dfeb748f7` attached the canonical `SHARED-0021` dependency to `SIM-0008` and corrected the matching Starfall roadmap prose; coordinator pointer commit `7530f552044b5888d44df7123c66996612c4655e` pins that reviewed child commit. `SIM-0008` now has a valid-but-waiting dependency on `SHARED-0021`. Source consumption and `SIM-0008` activation remain blocked until `SHARED-0021` completes and `SIM-0008` receives its own approved implementation plan.

## Historical asset-enabler wiring (SF-0008)

An earlier asset-enabler Cycle 3 completed through Starfall task `pm://project/prj_pkIpzx0fzFD4URjvqBuYrGZF/task/SF-0008`. Starfall commit `d88ab2e8e6bf9757bda8c37a1d10cf71724ff7d8` added the reviewed canonical asset, cooking and presentation dependencies plus matching roadmap corrections while leaving every feature task todo.

Coordinator pointer commit `166ec61255addf97da44a4594a810427e9424188` pins that exact Starfall commit and records the canonical task URI and stable Starfall project ID. That earlier COORD-0009/SF-0008 asset-enabler continuation is complete and has no further wiring under its scope.

It is distinct from the now-completed Box3D Cycle 3 described above. Both reviewed wiring continuations are complete; the Box3D implementation and Starfall source-consumption gate remain separately owned by `SHARED-0021` and the later approved `SIM-0008` implementation plan.
