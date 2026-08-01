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

## Preserve Authority

Servers own gameplay outcomes and persistent state. Clients own rendering, animation, IK, effects, cameras, and smoothing. Animation consumes events/state; it never produces authoritative attacks, hits, movement, equipment, damage, or death.

Keep headless projects free of SDL, GPU, ImGui, renderer, editor, and presentation assets.

## Promote Only Proven Contracts

Keep the first skinned-character work as a coordinator experiment. Promote a contract only after deterministic tests, native GPU execution, visual evidence, owner confirmation, and a concrete need in at least one child. Do not extract Royale code because it looks reusable.

Reject premature render graphs, scene frameworks, generic ECS/component runtimes, retargeting systems, permanent custom skeletal formats, or production animation graphs.

## Keep Authoring Separate

Typed authoring objects may register serialization, inspector controls, validation, gizmos, icons, debug drawing, and cooking. Compile them into game-specific runtime data such as Royale navigation graphs or Starfall spawn definitions. Do not make runtime simulation reflective merely to resemble the authoring model.

Treat dependency, project, authority, protocol, file-format, and third-party-loader changes as owner decision gates and update the owning wiki after approval.
