---
name: chronofall-architecture-boundaries
description: Place ChronoFall architecture and source in the correct repository. Use for parent versus child ownership, server/client authority, shared-engine promotion, dependency direction, experiments, project boundaries, authoring versus runtime data, or game-object/component proposals.
---

# ChronoFall Architecture Boundaries

## Choose The Owner

- Coordinator: product family roadmap, cross-project contracts, experiments, proven shared modules, and pinned child commits.
- Royale: battle-royale simulation, protocol, content, presentation integration, build, and release.
- Starfall: MMO simulation, protocol, content, presentation integration, editor/Balance Lab, build, and release.

Never make Royale and Starfall depend directly on each other. Parent shared modules may be consumed by children but must remain child-independent.

The canonical full-client environment is the coordinator family checkout. An approved child may reference an audience-specific coordinator source allowlist through `ChronoFallFamilyRoot`: character-presentation projects are client-only, while the headless-safe `ChronoFall.Box3D` project may be consumed by simulation with bindings only transitively. This does not transfer repository, build-policy, gameplay, or release ownership to the parent. Do not require package distribution merely to preserve independent product ownership.

## Preserve Authority

Servers own gameplay outcomes and persistent state. Clients own rendering, animation, IK, effects, cameras, and smoothing. Animation consumes events/state; it never produces authoritative attacks, hits, movement, equipment, damage, or death.

Keep headless projects free of SDL, GPU, ImGui, renderer, editor, and presentation assets. A headless child may consume the bounded shared Box3D world/body/shape/mover-query contract, but retains fixed-tick scheduling, stable entity identity, gameplay ordering, and all game-specific collision policy.

## Promote Only Proven Contracts

Keep the first skinned-character work as a coordinator experiment. Promote a contract only after deterministic tests, native GPU execution, visual evidence, owner confirmation, and a concrete need in at least one child. Do not extract Royale code because it looks reusable.

Reject premature render graphs, scene frameworks, generic ECS/component runtimes, retargeting systems, permanent custom skeletal formats, or production animation graphs.

Do not preserve backward compatibility merely because an older path exists. After an approved replacement is complete, remove obsolete readers, writers, shims, fallbacks, dual paths and speculative migrations. Keep or migrate an old contract only for a demonstrated current consumer, stored data, staged deployment or explicit owner decision, with a named owner and removal condition.

## Keep Authoring Separate

Typed authoring objects may register serialization, inspector controls, validation, gizmos, icons, debug drawing, and cooking. Compile them into game-specific runtime data such as Royale navigation graphs or Starfall spawn definitions. Do not make runtime simulation reflective merely to resemble the authoring model.

Treat dependency, project, authority, protocol, file-format, and third-party-loader changes as owner decision gates and update the owning wiki after approval.
