---
title: Family Source Consumption
createdAt: 2026-08-02T10:26:40.3003240Z
modifiedAt: 2026-08-06T17:44:20.0578130Z
---

## Decision

ChronoFall's canonical full-client development environment is the shallow coordinator family checkout:

```text
ChronoFall/
├── royale/
└── starfall/
```

Royale and Starfall remain independently owned products with their own PM data, source histories, architecture, gameplay, protocol, content, build decisions, and releases. That ownership does not require every full client build to succeed without the coordinator checkout.

Approved child clients may consume explicitly approved coordinator projects from source through one MSBuild property, `ChronoFallFamilyRoot`. Literal parent traversal, absolute checkout paths, arbitrary external roots, imported coordinator build policy, and child-to-child references are forbidden.

NuGet packages, feeds, package versions, `buildTransitive` targets, source mapping, and a content-package contract are deferred until real Royale and Starfall integrations or independent release/CI requirements demonstrate their value.

## Approved source boundary

The initial client-only allowlist is:

- `src/ChronoFall.CharacterPresentation/ChronoFall.CharacterPresentation.csproj`;
- `src/ChronoFall.CharacterPresentation.Cooking/ChronoFall.CharacterPresentation.Cooking.csproj`;
- `src/ChronoFall.CharacterPresentation.SdlGpu/ChronoFall.CharacterPresentation.SdlGpu.csproj`.

A child reference uses `$(ChronoFallFamilyRoot)src/...`. The child's conditional default resolves the coordinator root in the canonical checkout; an override must identify an equivalent coordinator root.

`ChronoFall.CharacterPresentation` remains deterministic and BCL-only. `ChronoFall.CharacterPresentation.Cooking` remains BCL-only and depends only on the core. `ChronoFall.CharacterPresentation.SdlGpu` owns GPU upload/draw recording and directly references the checked-out coordinator pin at `thirdparty/repos/SDL3-CS/SDL3-CS/SDL3-CS.csproj`. SDL3-CS is compiled from source; children do not acquire or pin it independently for this shared path.

Only a child client may consume this allowlist. Headless simulation, world/server, Balance Lab, protocol, content, and editor projects remain free of SDL/GPU and shared presentation dependencies unless a later approved contract says otherwise.

`ChronoFall.CharacterPresentation.SdlGpu` also exposes the bounded screenshot readback and PNG-writing contract recorded at `pm://project/prj_E7QP3LUocfY7k3PYM-EQOlqc/wiki/architecture/shared-sdl-gpu-capture`. This is part of the existing client-only allowlisted project, not an additional reference. A child must still own its capture scheduling, camera, scene, output policy and task approval; the shared capability does not authorize automatic child adoption.

## Headless Box3D source boundary

The source model now has separate audience allowlists. Client presentation retains the three character-presentation projects above. Headless simulation may directly reference only:

```text
$(ChronoFallFamilyRoot)src/ChronoFall.Box3D/ChronoFall.Box3D.csproj
```

`ChronoFall.Box3D.Bindings` is a transitive implementation dependency and must not be directly referenced by a child. The coordinator family-source consumer proves the managed source reference, native runtime copy and absence of SDL, GPU, ImGui and presentation payloads.

The boundary uses the same single `ChronoFallFamilyRoot` property and adds no literal parent traversal, arbitrary root, package/feed machinery, imported coordinator build policy or child reference. Its full runtime contract is `pm://project/prj_E7QP3LUocfY7k3PYM-EQOlqc/wiki/architecture/shared-box3d-runtime`.

Starfall `SIM-0008` may adopt this reference only after `SHARED-0021` completes and `SIM-0008` receives its own approved implementation plan. Dependency completion permits planning; it does not activate child work or transfer gameplay ownership.

## Network transport source boundary

Low-level network I/O has a separate process-host allowlist. A child process that actually owns socket composition may directly reference only:

```text
$(ChronoFallFamilyRoot)src/ChronoFall.Network.Transport.LiteNetLib/ChronoFall.Network.Transport.LiteNetLib.csproj
```

`ChronoFall.Network.Transport` and the checked-out LiteNetLib source are transitive; a child must not directly reference either one. The permitted future consumers are Starfall Client/World and Royale Client/Server, each only after a separately planned child adoption or migration task.

Content, Protocol, Simulation, Editor and Balance Lab remain outside this allowlist. Protocol codecs remain transport-independent. The shared adapter supplies opaque copied packets and connection facts; each child owns framing, admission, sessions, gameplay exchange, connection policy and runtime wiring.

The source boundary uses the existing `ChronoFallFamilyRoot` property and adds no package/feed machinery. Its complete contract is `pm://project/prj_E7QP3LUocfY7k3PYM-EQOlqc/wiki/architecture/shared-network-transport`. Completion of `SHARED-0023` permits a Starfall adoption plan but does not activate `CLIENT-0009` or mutate either child.

## Native editor UI source boundary

Editor-host presentation has its own audience-specific allowlist. `SHARED-0024` establishes this boundary. A child may use it only after its own adoption task receives owner approval; the only permitted direct reference is:

~~~text
$(ChronoFallFamilyRoot)src/ChronoFall.EditorUi.SdlGpu/ChronoFall.EditorUi.SdlGpu.csproj
~~~

The first planned consumer is the native `Starfall.Editor` host. SDL3-CS, ImGui.Net, Evergine.Mathematics and the coordinator-built native libraries are transitive implementation details; a child must not reference their coordinator project or checkout paths directly.

Starfall's World, Simulation, Protocol, Content, Balance Lab and headless editor document/compiler are excluded. A native editor host may compose this UI backend with separately approved client-side presentation projects, but its authoritative document model must remain presentation-free.

The source boundary uses only `ChronoFallFamilyRoot`. It adds no package/feed distribution, imported coordinator build policy, product theme, docking layout or editor framework. Royale remains unchanged unless a later Royale-owned task adopts the backend. The complete contract is `pm://project/prj_E7QP3LUocfY7k3PYM-EQOlqc/wiki/architecture/shared-sdl-gpu-imgui-backend`.

## Starfall.Client development-instrumentation source boundary

Completed coordinator task `pm://project/prj_E7QP3LUocfY7k3PYM-EQOlqc/task/SHARED-0026` extends the existing caller-controlled SDL GPU ImGui boundary to the Starfall.Client composition root for development-only instrumentation. The only approved direct reference is:

~~~text
$(ChronoFallFamilyRoot)src/ChronoFall.EditorUi.SdlGpu/ChronoFall.EditorUi.SdlGpu.csproj
~~~

SDL3-CS, ImGui.Net, Evergine.Mathematics, native pins, and coordinator checkout paths remain transitive implementation details. Starfall.Client must not reference those implementation projects or paths directly. World, Simulation, Protocol, Content, Balance Lab, and the headless editor document/compiler remain outside this allowlist.

ChronoFall owns the reusable backend/native integration, caller-lifecycle compatibility, pinned dependencies, macOS ARM64 evidence, and this exact audience boundary. Starfall owns debug windows, menu organization, F12 and `--debug-ui-hidden`, input capture, feature diagnostics, the console, development commands, persistence choices, and permanent game UI.

The existing child-like family-source consumer proves that the single direct reference builds from `ChronoFallFamilyRoot` and receives only the expected transitive managed and native implementation payload. It is not a Starfall application shell or a substitute for child integration.

Starfall `pm://project/prj_pkIpzx0fzFD4URjvqBuYrGZF/task/CLIENT-0029` now has a completed canonical prerequisite and is dependency-ready. It remains todo and requires its own owner-approved plan before Starfall source consumption begins. This boundary does not activate the Starfall editor or confer a general child ImGui entitlement.

## Generated client content

From the coordinator root, stage the selected Quaternius character, bow and arrow cooks for a declared child by stable project ID:

```sh
scripts/cook-character-presentation-for-client.sh \
  --project-id prj_pkIpzx0fzFD4URjvqBuYrGZF
```

The command does not accept an alias, filesystem destination or server audience. It resolves the committed child `pathHint`, canonicalizes the checkout and verifies:

- the child stable ID in `.pm/project_id.txt`;
- the reciprocal parent stable ID;
- the coordinator `.gitmodules` entry and tracked gitlink;
- the exact canonical checkout path;
- that no output path component or existing output is a symlink;
- that `artifacts/chronofall/character-presentation/client/` is ignored;
- that no file under that tree is tracked;
- that an existing tree contains only the known workflow-owned files.

The fixed generated layout is:

```text
artifacts/chronofall/character-presentation/client/
├── quaternius-ual1-standard.cfskel
├── quaternius-ual1-standard.provenance.json
├── quaternius-medieval-weapons-bow-wooden.cfmesh
├── quaternius-medieval-weapons-bow-wooden.provenance.json
├── quaternius-medieval-weapons-arrow.cfmesh
├── quaternius-medieval-weapons-arrow.provenance.json
└── licenses/
    ├── quaternius-ual1-standard/
    │   ├── License.txt
    │   └── README.txt
    └── quaternius-medieval-weapons/
        └── License.txt
```

The command restores and builds only the focused skeletal and static cookers, cooks into a temporary directory, then replaces only these known files. It copies no raw GLB, OBJ, MTL, Blend or FBX, changes no child source or PM data and creates no runtime manifest.

## Provenance and determinism

The selected UAL1 character cook remains the client-only recipe at `assets/recipes/quaternius-ual1-standard.json`. It selects `Mannequin`, the `Armature` skeleton and `Idle_Loop`, `Walk_Loop` and `Sword_Attack` from Quaternius `Universal Animation Library[Standard]`.

ASSET-0006 adds two exact static client recipes:

- `assets/recipes/quaternius-medieval-weapons-bow-wooden.json`;
- `assets/recipes/quaternius-medieval-weapons-arrow.json`.

They select only the Quaternius Medieval Weapons Pack OBJ/MTL pairs approved by Starfall CONTENT-0011, record every source/resource/licence SHA-256 and cook at `0.25` metres per source unit through the provisional `section-names-only` material contract.

Every deterministic JSON sidecar records schema and audience, portable recipe/source paths and hashes, CC0 identifier and evidence paths, cooked filename, byte count and SHA-256. It contains no timestamp or absolute checkout path.

Established output hashes are:

- `quaternius-ual1-standard.cfskel`: `37d2ecd2c614a4cc74fe359906c84408432100f0338b86d7ce4f4dddb6b585d3`;
- `quaternius-medieval-weapons-bow-wooden.cfmesh`: `4c0ab766e7c622c0f52ff0ade3cb1992c6d96664233a4695fc049a3a9b1d642e`;
- `quaternius-medieval-weapons-arrow.cfmesh`: `4eeb80dc06e1f729b67606eb6c12110b954068cfb7ea39590706771e4c02d9c3`.

The formats remain provisional. Generated output stays ignored and must not be committed or placed in headless artifacts. Exact bow/arrow provenance, bounds and scale evidence are recorded at `pm://project/prj_E7QP3LUocfY7k3PYM-EQOlqc/wiki/assets/quaternius-medieval-weapons-bow-arrow-cook`.

## Future exact-selection staging

The Draft 0 coordinator roadmap extends this boundary only through focused acquisition tasks. Each acquisition consumes a completed canonical Starfall selection, verifies exact pack-relative paths, hashes and licence evidence, and stages only an approved client output set. It may not accept an alias or arbitrary destination, modify a child, import a whole pack or add a runtime manifest.

ASSET-0006 realizes the first static extension with the exact selected `Bow_Wooden` and `Arrow` recipes and ignored outputs. It does not authorize sockets, equipment, combat or automatic child integration.

ASSET-0004 and ASSET-0005 retain their known skeletal prerequisites. ASSET-0007 remains the owner of exact selected zone inputs. ASSET-0008 monster acquisition deliberately begins with only its canonical selection and the established stable-ID staging boundary. Static, rigid or skeletal prerequisites are attached only after selection evidence identifies the actual representation.

Generated outputs remain ignored and client-only. Package/feed distribution remains deferred.

## Ownership and next consumers

Coordinator task `pm://project/prj_E7QP3LUocfY7k3PYM-EQOlqc/task/SHARED-0016` owns the original source-consumption and staging boundary. Completed audience-specific extensions own only their exact additional consumers:

- `SHARED-0021`: headless Starfall simulation access to `ChronoFall.Box3D`;
- `SHARED-0023`: process-host access to the LiteNetLib transport adapter;
- `SHARED-0024`: native Starfall.Editor access to the caller-controlled SDL GPU ImGui backend;
- `SHARED-0026`: approved Starfall.Client development-instrumentation access to that same backend.

Each audience remains deny-by-default and requires its own child-owned adoption task and architecture tests. Completion permits planning and canonical dependency wiring; it never activates child work.

Starfall character integration remains `pm://project/prj_pkIpzx0fzFD4URjvqBuYrGZF/task/CLIENT-0006`. Starfall development instrumentation uses its separately allocated child tasks; `CLIENT-0029` carries the completed canonical `SHARED-0026` edge and is dependency-ready but remains todo until its own owner-approved plan. Royale integrations remain Royale-owned.

No child adoption may redesign coordinator source, add a feed, copy raw source assets, edit the parent repository, grant unrelated projects access, or leak presentation dependencies into headless outputs. Child commits receive the normal pointer-only coordinator handoff after review.