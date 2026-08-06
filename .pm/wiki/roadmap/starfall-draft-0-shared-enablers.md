---
title: Starfall Draft 0 Coordinator Enablers
createdAt: 2026-08-02T16:21:34.5039080Z
modifiedAt: 2026-08-06T07:25:36.5068140Z
---

## Purpose and ownership

This page records the coordinator-owned enablers for Starfall's provisional Draft 0 first playable zone. Starfall owns game-specific selection, content identity, composition, simulation, protocol, and presentation integration. ChronoFall owns supplied-source provenance, reusable client presentation/cooking contracts, stable-project-ID staging, and exact selected-input acquisition.

Availability is not selection. A prospective pack is not a dependency until its files are physically supplied, inventoried, licensed, and selected by the owning Starfall task. No entire pack enters a cook or runtime manifest. Generated client output remains ignored.

Starfall's design brief is `pm://project/prj_pkIpzx0fzFD4URjvqBuYrGZF/wiki/product/first-playable-zone-draft-0`. Coordinator grooming task `COORD-0009` canonically depends on completed Starfall grooming task `pm://project/prj_pkIpzx0fzFD4URjvqBuYrGZF/task/SF-0008`, which is the authoritative source of this decomposition.

## Shared capability graph

The near coordinator deliverables are intentionally narrow:

~~~text
M4 Starfall.Client Development Instrumentation Boundary

SHARED-0016 completed family-source policy
SHARED-0024 completed caller-controlled SDL GPU ImGui backend
  -> SHARED-0026 approved Starfall.Client development-instrumentation boundary
  -> later separately approved Starfall canonical wiring
~~~

`SHARED-0026` owns only the exact family-source allowlist, reusable backend/native boundary, caller-lifecycle compatibility, macOS ARM64 evidence, and headless exclusion. Starfall owns debug-window organization, menu/F12/hidden-at-launch behavior, input capture, diagnostics, command frontends, and permanent UI.

~~~text
M5 Connected Basic Arrow Shared Enablers

Starfall CONTENT-0011 exact Basic archer/bow/animation selection
SHARED-0002 completed skeletal cooking
SHARED-0017 completed stable-ID staging
EXPERIMENT-0014 completed technical bow-body evidence
  -> ASSET-0004 exact selected humanoid/base and minimum bow-animation inputs

Starfall CONTENT-0011 exact Basic bow/arrow selection
SHARED-0019 completed static cooking
SHARED-0017 completed stable-ID staging
  -> ASSET-0006 exact selected bow and arrow inputs

SHARED-0006 completed socket transform contract
SHARED-0018 completed static rendering
ASSET-0006
  -> SHARED-0020 one rendered socketed static-bow proof
~~~

`SHARED-0020` uses a harness-local technical socket and local transform. Starfall `CLIENT-0011` owns the provisional semantic hand socket, local bow transform, rendering integration, and native placement validation. Equipment, aiming, off-hand IK, projectile behavior, and generalized grip systems are excluded.

The later broad `SHARED-0007` attachment task remains milestone-free and must review and reuse `SHARED-0020` rather than recreating it.

Modular armour is separately deferred:

~~~text
future completed Starfall Ranger/leather selection
SHARED-0002 completed skeletal cooking
  -> re-groomed SHARED-0003 canonical-rig modular-armour proof
  -> SHARED-0004 equipment slots and body-region hiding
  -> ASSET-0005 exact Ranger acquisition and stable-ID staging
~~~

No Ranger-selection task identity is currently allocated. `SHARED-0003` and `ASSET-0005` deliberately carry no fabricated canonical dependency; the real edge is attached only in an owner-approved activation cycle after Starfall allocates and completes that selection. These and the later material, attachment, debugging, preview, zone, monster, and authoring tasks remain milestone-free scheduling placeholders until concrete deliverables activate them.

## Exact acquisition graph

| Coordinator task | Owning selection | Coordinator prerequisites | Scheduling/result |
| --- | --- | --- | --- |
| `ASSET-0004` archer and bow animations | `pm://project/prj_pkIpzx0fzFD4URjvqBuYrGZF/task/CONTENT-0011` | `SHARED-0002`, `SHARED-0017`, `EXPERIMENT-0014` | M5; exact selected technical humanoid/base and minimum compatible bow clips |
| `ASSET-0006` bow and arrows | `pm://project/prj_pkIpzx0fzFD4URjvqBuYrGZF/task/CONTENT-0011` | `SHARED-0019`, `SHARED-0017` | M5; exact selected static weapon inputs |
| `ASSET-0005` Ranger equipment | Future task-owned Starfall Ranger/leather selection; not allocated | `SHARED-0004`, `SHARED-0017` plus the later canonical selection edge | Milestone-free and priority none until post-Inventory equipment work activates |
| `ASSET-0007` zone presentation | `pm://project/prj_pkIpzx0fzFD4URjvqBuYrGZF/task/CONTENT-0012` | `SHARED-0019`, `SHARED-0017` | Milestone-free and priority none until proper-scene work activates |
| `ASSET-0008` monster presentation | `pm://project/prj_pkIpzx0fzFD4URjvqBuYrGZF/task/CONTENT-0013` | `SHARED-0017` initially | Milestone-free and priority none until selected-monster presentation activates |

`ASSET-0004` and `ASSET-0006` remain blocked on the still-todo Basic selection in `CONTENT-0011`; M5 assignment does not make selection complete. Acquisitions consume exact selections and may stage only approved inputs with pack-relative paths, hashes, licence evidence, format/scale/material/rig evidence, intended consumer, and deliberate conversions.

`ASSET-0005` no longer depends on `CONTENT-0011` because that task does not own Ranger/equipment selection. The future real selection is an explicit activation-time prerequisite rather than an alias, prose-only identity presented as a dependency, or prematurely allocated task.

`ASSET-0008` deliberately has no static-render, static-cook, or skeletal-cook prerequisite yet. Starfall selection must first determine whether the candidate is static, rigidly animated, skeletal, or unsuitable. Only a later reviewed planning or grooming cycle may attach the smallest demonstrated prerequisite; it must never add both paths defensively or create a generic monster pipeline.

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

Starfall's provisional graybox and authoritative movement remain game-owned. ChronoFall `SHARED-0021` now establishes the reusable Box3D source/native/managed boundary from the independent coordinator pin and the audited Royale `PHYS-012` evidence.

```text
parent SHARED-0016 family-source policy
Royale PHYS-012 audited evidence
  -> parent SHARED-0021 bounded shared Box3D runtime
  -> Starfall SIM-0008 approved integration plan
```

The coordinator runtime is deliberately limited to world stepping, body/shape ownership, transforms/velocity, boxes/capsules, filtering, mover casts and immutable sorted collision-plane facts. Starfall continues to own entity identity, fixed-tick scheduling, content conversion, movement rules, collision layers and gameplay outcomes.

The headless direct-reference allowlist contains only `$(ChronoFallFamilyRoot)src/ChronoFall.Box3D/ChronoFall.Box3D.csproj`; bindings are transitive. Source consumption adds no SDL/GPU payload and no package/feed machinery.

Starfall Cycle 3 already attached the canonical dependency through `pm://project/prj_pkIpzx0fzFD4URjvqBuYrGZF/task/SF-0009`. Once `SHARED-0021` completes, `SIM-0008` becomes dependency-ready, but it must remain todo until its own owner-approved plan. This coordinator task does not implement or activate Starfall simulation.

## Connected walking transport prerequisite

Starfall `SERVER-0005` proves the connected-walking exchange in-process, while `CLIENT-0009` deliberately requires a real transport before activation. ChronoFall `SHARED-0023` now owns the bounded shared opaque-packet transport, independently pinned from the audited Royale `NET-001` evidence.

```text
parent SHARED-0016 family-source policy
Royale NET-001 audited transport evidence
  -> parent SHARED-0023 shared low-level network transport
  -> future Starfall Client/World adoption task
  -> Starfall CLIENT-0009 connected walking implementation
```

The shared boundary owns only source-built LiteNetLib transport contracts and copied packet delivery. Starfall retains admission framing, join tickets, gameplay sessions, command/snapshot codecs, channel use, connection policy and Client/World composition.

`SHARED-0023` does not mutate Starfall or make `CLIENT-0009` executable by itself. A later owner-directed Starfall cycle must allocate the focused adoption task, attach the canonical coordinator dependency, update the exact Client/World family-source allowlist, and wire `CLIENT-0009` behind that completed adoption.

## Historical asset-enabler wiring (SF-0008)

An earlier asset-enabler Cycle 3 completed through Starfall task `pm://project/prj_pkIpzx0fzFD4URjvqBuYrGZF/task/SF-0008`. Starfall commit `d88ab2e8e6bf9757bda8c37a1d10cf71724ff7d8` added the reviewed canonical asset, cooking and presentation dependencies plus matching roadmap corrections while leaving every feature task todo.

Coordinator pointer commit `166ec61255addf97da44a4594a810427e9424188` pins that exact Starfall commit and records the canonical task URI and stable Starfall project ID. That earlier COORD-0009/SF-0008 asset-enabler continuation is complete and has no further wiring under its scope.

It is distinct from the now-completed Box3D Cycle 3 described above. Both reviewed wiring continuations are complete; the Box3D implementation and Starfall source-consumption gate remain separately owned by `SHARED-0021` and the later approved `SIM-0008` implementation plan.
