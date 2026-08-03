---
title: Family Source Consumption
createdAt: 2026-08-02T10:26:40.3003240Z
modifiedAt: 2026-08-03T15:40:16.5253530Z
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

## Pending headless Box3D source boundary

The current allowlist above remains client-presentation-only. Coordinator task `pm://project/prj_E7QP3LUocfY7k3PYM-EQOlqc/task/SHARED-0021` is the separately owned prerequisite for extending this model to an explicit headless-safe Box3D source allowlist.

When implemented, that task must expose only coordinator-owned child-independent Box3D projects through the existing `ChronoFallFamilyRoot` property, prove a headless family-source consumer, and keep native runtime artifacts free of SDL, GPU, ImGui and presentation payloads. It must not add literal parent traversal, package/feed machinery, imported coordinator build policy, or direct child references.

No child may consume the pending boundary merely because the task exists. Starfall Box3D Cycle 3 completed through `pm://project/prj_pkIpzx0fzFD4URjvqBuYrGZF/task/SF-0009`: Starfall commit `84b1c94d3d3413954a20b09ea1d0445dfeb748f7` attached the canonical dependency and coordinator pointer commit `7530f552044b5888d44df7123c66996612c4655e` pins it. `SIM-0008` now has a valid-but-waiting dependency on `SHARED-0021`; source consumption and activation remain blocked until `SHARED-0021` completes and `SIM-0008` receives its own approved implementation plan.

## Generated client content

From the coordinator root, stage the selected Quaternius character cook for a declared child by stable project ID:

```sh
scripts/cook-character-presentation-for-client.sh \
  --project-id prj_pkIpzx0fzFD4URjvqBuYrGZF
```

The command does not accept an alias, filesystem destination, or server audience. It resolves the committed child `pathHint`, canonicalizes the checkout, and verifies:

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
└── licenses/quaternius-ual1-standard/
    ├── License.txt
    └── README.txt
```

The command cooks into a temporary directory, then replaces only these known files. It copies no raw GLB, changes no child source or PM data, and creates no runtime manifest.

## Provenance and determinism

The cook remains the client-only recipe at `assets/recipes/quaternius-ual1-standard.json`. It selects `Mannequin`, the `Armature` skeleton, and `Idle_Loop`, `Walk_Loop`, and `Sword_Attack` from Quaternius `Universal Animation Library[Standard]`.

The deterministic JSON sidecar records schema and audience, portable recipe/source paths and hashes, CC0 identifier and evidence paths, selected clips, cooked filename, byte count, and SHA-256. It contains no timestamp or absolute checkout path.

The established `.cfskel` output is 1,278,301 bytes with SHA-256 `37d2ecd2c614a4cc74fe359906c84408432100f0338b86d7ce4f4dddb6b585d3`. The format remains provisional. Generated output stays ignored and must not be committed or placed in headless artifacts.

## Future exact-selection staging

The Draft 0 coordinator roadmap extends this boundary only through focused acquisition tasks. Each acquisition must consume a completed canonical Starfall selection, verify exact pack-relative paths, hashes and licence evidence, and stage only an approved client output set. It may not accept an alias or arbitrary destination, modify a child, import a whole pack, or add a runtime manifest.

`ASSET-0004` through `ASSET-0007` have known skeletal or static prerequisites. `ASSET-0008` monster acquisition deliberately begins with only the canonical still-todo selection and the established stable-ID staging boundary. Static, rigid, or skeletal prerequisites are attached only after selection evidence identifies the actual representation.

Generated outputs remain ignored and client-only. Package/feed distribution remains deferred.

`pm://project/prj_E7QP3LUocfY7k3PYM-EQOlqc/task/SHARED-0019` completes the provisional deterministic static-mesh cooking capability and exact hashed source/resource/licence evidence boundary. It deliberately adds no child allowlist, recipe, generated output or runtime manifest entry.

`ASSET-0006` and `ASSET-0007` remain the owners of the first real bow/monster and zone exact-selection recipes and staged outputs after their canonical Starfall selection dependencies complete. The existing character staging workflow and selected UAL1 cook remain unchanged.

## Ownership and next consumers

Coordinator task `pm://project/prj_E7QP3LUocfY7k3PYM-EQOlqc/task/SHARED-0016` owns this source-consumption and staging boundary.

Starfall integration remains `pm://project/prj_pkIpzx0fzFD4URjvqBuYrGZF/task/CLIENT-0006`. It owns only Starfall client references, content consumption, and runtime mapping. Royale integration remains child-owned. Neither child integration may redesign coordinator source, add a feed, copy raw source assets, or edit the parent repository. After the child task is complete and committed, the coordinator automatically records only the reviewed gitlink in a pointer-only commit owned by the canonical child task.

Completing `SHARED-0016` satisfies only the coordinator dependency of Starfall `CLIENT-0006`; Starfall `BUILD-0003` remains independently required.