---
title: Shared Engine and Authority Boundaries
createdAt: 2026-08-01T05:44:07.0060700Z
modifiedAt: 2026-08-06T11:50:37.9193500Z
---

## Focus

ChronoFall may grow a focused shared engine for Royale and Starfall. It is not an arbitrary Unity-like framework. Reuse must be demonstrated before extraction.

Likely shared domains include native loading, SDL desktop lifecycle, SDL GPU device/targets, static and skinned rendering, skeleton/animation processing, modular equipment presentation, sockets/attachments, IK/aim presentation, text/debug rendering, Box3D wrappers, asset cooking, client/server asset separation, low-level transport, and proven editor infrastructure.

Gameplay simulation, protocol/replication, lifecycle, combat, AI, progression/economy, content schemas, product editor documents, and deployment topology remain child-owned.

## Third-party dependency ownership

ChronoFall acquires a third-party dependency only when a parent-owned experiment or shared module consumes it. The coordinator then owns its pin, fetch workflow, licence evidence, and focused patches. Parent source must never reference dependency paths inside Royale or Starfall.

The canonical full-client development environment is the shallow coordinator family checkout. Approved child clients may consume a narrow coordinator source allowlist through the single `ChronoFallFamilyRoot` property while retaining independent PM, source, product architecture, build-policy, and release ownership. Full client build isolation outside that family checkout is not currently required.

The shared SDL GPU project continues to compile the checked-out coordinator SDL3-CS pin from source. Children consume that dependency transitively through the approved shared project; they do not directly reference or independently package it for this path.

NuGet packages, feeds, versions, `buildTransitive` targets, source mapping, and content packages remain deferred until real Royale and Starfall integrations or independent CI/release requirements demonstrate the need. The complete source and generated-content contract is recorded at `pm://project/prj_E7QP3LUocfY7k3PYM-EQOlqc/wiki/development/family-source-consumption`.

The M1 loader decision is recorded at `pm://project/prj_E7QP3LUocfY7k3PYM-EQOlqc/wiki/experiments/skeletal-loader-decision`.

## Shared Box3D runtime

Coordinator task `pm://project/prj_E7QP3LUocfY7k3PYM-EQOlqc/task/SHARED-0021` implements the first bounded shared Box3D source, native-build and managed-runtime boundary. It begins from Royale's audited evidence at `pm://project/prj__-jXLQgm6GuD2gCKZ_bTa1m-/task/PHYS-012` but owns an independent official pin, MIT licence snapshot, fetch/build workflow, namespaces and child-independent source.

The promoted surface is finite Box3D-native single-precision metre values, world lifecycle and fixed stepping, body/shape ownership, transforms and velocity, boxes/capsules, collision filtering, mover casts and copied collision-plane facts. Callback facts are immutable and explicitly sorted; stable game identity, creation/application order, fixed-tick scheduling and final gameplay ordering remain caller-owned. Cross-platform bitwise physics determinism is not promised.

The approved headless direct-reference allowlist contains only `src/ChronoFall.Box3D/ChronoFall.Box3D.csproj`; raw bindings are transitive. macOS ARM64 and Linux x64 native layouts are explicit. SDL, GPU, ImGui, debug rendering, character controllers, gameplay, maps, collision cooking, generalized physics abstractions, packages/feeds and child migration remain excluded.

The complete contract is `pm://project/prj_E7QP3LUocfY7k3PYM-EQOlqc/wiki/architecture/shared-box3d-runtime`. Starfall `SIM-0008` has a valid dependency on this task but still requires its own approved plan before it may activate or consume the source.

## Shared low-level network transport

Coordinator task `pm://project/prj_E7QP3LUocfY7k3PYM-EQOlqc/task/SHARED-0023` promotes the proven opaque-packet boundary from Royale `pm://project/prj__-jXLQgm6GuD2gCKZ_bTa1m-/task/NET-001` into child-independent source. `ChronoFall.Network.Transport` is BCL-only; `ChronoFall.Network.Transport.LiteNetLib` is the sole consumer of the independently pinned LiteNetLib source.

The boundary owns endpoints, ephemeral peer IDs, five delivery modes, channels, connection/disconnection/error/latency events, copied packet memory and optional peer statistics. Children retain framing, protocol, admission, sessions, gameplay, connection policy and runtime composition. Transport acceptance is not player admission.

The approved future direct reference is only the LiteNetLib adapter through `ChronoFallFamilyRoot`, and only from product process/composition roots that perform network I/O. Starfall Client/World adoption and Royale Client/Server migration require separate child-owned tasks. The complete contract is `pm://project/prj_E7QP3LUocfY7k3PYM-EQOlqc/wiki/architecture/shared-network-transport`.

## Authority

Servers own attacks, shots, casts, hits, movement transitions, equipment changes, damage, death, and persistent state. Clients present those outcomes through rendering, animation, IK, effects, cameras, and smoothing. Headless code never depends on SDL windowing/GPU, ImGui, rendering, or editor code.

## Promotion gate

The M1 skinned-character proof satisfied its evidence gate, and `pm://project/prj_E7QP3LUocfY7k3PYM-EQOlqc/task/SHARED-0001` now owns the first deliberate promotion.

The resulting module and public-resource boundaries are recorded at `pm://project/prj_E7QP3LUocfY7k3PYM-EQOlqc/wiki/architecture/shared-character-presentation`. `ChronoFall.CharacterPresentation` preserves the proven BCL-only skeletal and deterministic animation semantics. `ChronoFall.CharacterPresentation.SdlGpu` owns the reviewed hidden GPU ABI and records mesh uploads, palette updates and draws into a caller-owned SDL GPU lifecycle.

The promotion does not make SimpleMesh permanent, freeze a cooked format, establish child package distribution, or approve materials, animated bounds, cross-rig animation, retargeting, root motion, blending, equipment, IK, animation graphs, render graphs, scenes or generalized components. Those remain separately approved contracts.

## Later authoring exploration

Typed authoring objects may eventually register serialization, inspector controls, validation, gizmos, icons/labels, debug drawing, and content cooking. They compile to product-specific runtime data: a Royale waypoint can compile to a compact navigation graph; a Starfall spawn object can compile to server-owned spawn data. Authoring structure must not force a reflective runtime ECS.

## Shared editor UI backend

Coordinator task `pm://project/prj_E7QP3LUocfY7k3PYM-EQOlqc/task/SHARED-0024` is the completed caller-controlled SDL GPU ImGui backend. It owns `ChronoFall.EditorUi.SdlGpu`, the independent ImGui.Net/cimgui and ImGuizmo pins, reproducible native build, licence evidence, and macOS ARM64 artifact packaging.

The backend owns ImGui context lifetime, SDL event forwarding, framebuffer-scale propagation, caller-injected font-atlas configuration, draw-data preparation, and recording into a caller-supplied SDL GPU render pass. Every consumer continues to own its SDL window/device, command buffer, swapchain/target, pass begin/end, submission, application loop, layout, state, and product semantics.

The completed `SHARED-0024` contract established the native `Starfall.Editor` host audience. Editor work remains deferred and is not activated by later consumers.

Coordinator task `pm://project/prj_E7QP3LUocfY7k3PYM-EQOlqc/task/SHARED-0026` completes the distinct Starfall.Client development-instrumentation audience boundary. The only approved direct reference is:

```text
$(ChronoFallFamilyRoot)src/ChronoFall.EditorUi.SdlGpu/ChronoFall.EditorUi.SdlGpu.csproj
```

Only the Starfall.Client composition root may adopt that reference, through its separately planned `pm://project/prj_pkIpzx0fzFD4URjvqBuYrGZF/task/CLIENT-0029`. SDL3-CS, ImGui.Net, Evergine.Mathematics, the coordinator-built native libraries, and coordinator checkout paths remain transitive implementation details.

`SHARED-0026` does not own Starfall debug windows, menu organization, F12, `--debug-ui-hidden`, input-capture policy, feature diagnostics, the console, development-command semantics, permanent UI, or gameplay. Those remain Starfall-owned. World, Simulation, Protocol, Content, Balance Lab, and the headless editor document/compiler remain presentation-free.

Docking remains optional inside one caller-owned window. Platform multi-viewport is rejected because it would transfer secondary-window and rendering control into the backend. Neither task creates a shared application shell, theme, font choice, design-token system, UI primitives, editor framework, or Royale migration.