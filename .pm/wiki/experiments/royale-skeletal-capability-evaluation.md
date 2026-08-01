---
title: Royale Skeletal Loading and Rendering Capability Evaluation
createdAt: 2026-08-01T07:47:50.8798410Z
modifiedAt: 2026-08-01T07:47:50.8798410Z
---

## Scope and evaluated revisions

This evaluation compares the supplied Quaternius skeletal requirements with Royale's checked-out loading, rendering, cooking, capture, debug, and native-test paths. It does not select experiment inputs, choose a loader, define permanent skeletal types, or change either child repository.

Evaluated revisions:

- Royale: `174fa32600887da2093bcf7cbc9ebf89dc92990f`.
- SimpleMesh: `9f46341e35fa5876fbea7b96bd021bc3abd7842d`.
- Royale's committed SimpleMesh patch: `thirdparty/patches/SimpleMesh/0001-support-unsigned-byte-gltf-indices.patch`, which adds unsigned-byte glTF index support.
- Asset evidence: `pm://project/prj_E7QP3LUocfY7k3PYM-EQOlqc/wiki/assets/character-animation-inventory`.
- Architecture boundary: `pm://project/prj_E7QP3LUocfY7k3PYM-EQOlqc/wiki/architecture/shared-engine-boundaries`.

The provisional female-base and UAL1 files were used only as capability probes. Exact experiment-input selection remains owned by `pm://project/prj_E7QP3LUocfY7k3PYM-EQOlqc/task/ASSET-0002`.

## Direct load probe

A disposable .NET console program referenced the pinned `SimpleMesh.csproj`, called `Model.FromFile`, recursively enumerated model nodes and geometry descriptors, and reported skins and animations. No repository or supplied asset file was changed.

The supplied female base failed raw loading:

```text
SimpleMesh.ModelLoadException:
Unable to find external resource 'T_Eye_Normal_png.png'
```

A second pass used an in-memory `IExternalResources` resolver that mapped only that broken supplied URI to the existing `T_Eye_Normal.png`. It did not rewrite the glTF or image:

```text
roots=1 nodes=69 geometries=3 skinnedGeometries=3
skinnedNodes=3 skins=1 animations=0 images=7
skin Armature: bones=65 inverseBinds=65 finiteInverseBinds=65 root=<null>
```

UAL1 non-root-motion loaded without a resource override:

```text
roots=1 nodes=67 geometries=1 skinnedGeometries=1
skinnedNodes=1 skins=1 animations=43 images=0
skin Armature: bones=65 inverseBinds=65 finiteInverseBinds=65 root=<null>
```

Every exposed UAL1 animation had 65 translation and 65 rotation channels. Examples included `Idle_Loop` at 2.5 seconds, `Walk_Loop` at 1.3333334 seconds, and `Sword_Attack` at 1.5333333 seconds. The source inventory proves each clip actually contains 195 channels—translation, rotation, and scale for all 65 joints—so SimpleMesh discards all 65 scale channels.

## Capability matrix

| Requirement | Pinned SimpleMesh core | Royale integration today | Consequence for the experiment |
| --- | --- | --- | --- |
| glTF JSON with external BIN/images | Supported through `FileResources`; missing resources fail the load | Wrapper calls `Model.FromFile` directly and has no resource-resolution seam | The supplied female base cannot load unchanged because of its broken normal-map URI; correction policy is a later contract |
| Embedded GLB | Supported | Used by the existing static loader | UAL1 loads and exposes geometry, skin, and animation metadata |
| Joint hierarchy and local node transforms | `ModelNode` preserves names, parent/child structure, and transforms | Static wrapper recursively bakes node transforms into positions/normals | The current asset path destroys the dynamic hierarchy needed for posing |
| Skin and inverse-bind matrices | `ModelNode.Skin`, `Skin.Bones`, and `Skin.InverseBindMatrices` are populated | Not copied into `StaticMeshAsset` | A skeletal asset path is required; the static asset type cannot be extended implicitly |
| Four joint indices and weights | `VertexArray` exposes `Point4<ushort>` indices and `Vector4` weights when `VertexAttributes.Joints` is present | `StaticMeshVertex` stores only position, normal, and UV | A separate reviewed skeletal vertex/data contract is required |
| Translation animation | Imported and sampled linearly between keyframes; samples clamp outside the key range | Not exposed by Royale | Looping and clip-time ownership remain undefined |
| Rotation animation | Imported and sampled with quaternion slerp; samples clamp outside the key range | Not exposed by Royale | Deterministic pose sampling remains to be defined and tested |
| Scale animation | Ignored by the glTF animation importer | Not exposed by Royale | The supplied clips' complete TRS contract is not met |
| Interpolation mode | The glTF enum is parsed but not retained or applied when channels are built | Not exposed by Royale | Current assets are LINEAR, but validation and failure behavior need an explicit contract |
| Clip duration and looping | No duration or looping abstraction; duration can only be derived from key times | Not exposed by Royale | Experiment code must not silently inherit sample behavior |
| Coordinate/root transform policy | Source node matrices are retained, but no character-specific axis/root policy exists | Static path bakes transforms and the shader receives only instance matrices | Axis conversion, skeleton root space, bind-pose equations, and root motion need deterministic decisions |
| GPU joint palette | SimpleMesh core has no SDL GPU palette contract; its OpenTK sample is not core API | No palette buffer, uniform, or storage binding exists | A 65-joint SDL GPU transport must be chosen later |
| GPU skinning | OpenTK sample demonstrates an approach with only `Bones[32]` | SDL GPU basic shader has no joint/weight inputs or skinning | The sample's limit and transform handling are unsuitable as production contracts |
| Skinned normals and bounds | No Royale implementation | Static shader transforms normals by per-instance `WorldInverse`; static bounds drive previews | Correct skinned normal handling and animated/framed bounds are missing |
| Materials | SimpleMesh imports multiple material fields | Wrapper retains only base color and base-color texture | Normal-map failure currently blocks loading even though the static renderer ignores normal maps |
| Client/server cooking | No skeletal cooker selected | Client copies render sources/resources; server output strips render data and retains collision artifacts | Any skeletal cooking must remain client-only; headless artifacts must not acquire render dependencies |

## Royale patterns that are reusable evidence

These patterns are worth carrying into later experiments, but this evaluation does not promote or extract them:

- explicit sequential vertex structs with tested byte offsets and SDL vertex descriptors;
- transfer-buffer upload, vertex/index buffer ownership, texture caching, and deterministic disposal;
- HLSL compiled by shadercross to the repository's SPIR-V and Metal outputs;
- thin SDL GPU pipeline creation and explicit uniform/sampler declarations;
- offscreen targets, asynchronous fence-backed readback, RGBA normalization, and PNG composition;
- deterministic orthographic contact-sheet framing with stable axis and diagonal views;
- debug-line primitives that already support transforms, points, bounds, boxes, circles, and capsules;
- a standalone hidden-window SDL GPU harness with nonblank-image and distinct-view assertions;
- client/server asset audiences that keep rendering data out of server packages.

The existing static path itself is not a reusable skeletal contract. `SimpleMeshStaticMeshLoader` calls `AutoselectRoot().CalculateNormals()`, flattens each node into world-space static geometry, converts indices to `ushort`, and retains only position, normal, UV, base color, and base-color texture. `StaticMeshRenderer` uploads one static vertex/index pair, while `basic.vert.hlsl` accepts only position, normal, UV, `WorldViewProjection`, and `WorldInverse`.

## Missing contracts and downstream handoff

`pm://project/prj_E7QP3LUocfY7k3PYM-EQOlqc/task/EXPERIMENT-0002` must decide the narrowest experimental loader approach using this evidence and the final asset selection. It must explicitly decide:

- whether and how to use or supplement pinned SimpleMesh;
- how the three supplied broken image URIs are corrected without rewriting provenance sources;
- whether the experimental path requires scale channels and how unsupported interpolation fails;
- where glTF coordinate conversion and skeleton-root normalization occur;
- whether any loader/cooking output is temporary or a reviewed permanent format.

`pm://project/prj_E7QP3LUocfY7k3PYM-EQOlqc/task/EXPERIMENT-0003` must define the experiment-only skeleton, skin, bind/local pose, sampled pose, clip, and GPU-palette data. It must also define matrix convention, hierarchy evaluation, inverse-bind composition, duration/looping, and deterministic error reporting.

Later rendering tasks own a skeletal vertex layout, 65-joint palette delivery, GPU skinning, skinned normal handling, animated bounds, skeleton debug lines, timestamped captures, and native owner validation. None of those are implemented here.

## Authority and dependency boundary

Skeleton loading, pose evaluation, GPU palettes, animation sampling, debug visualization, and captures are client presentation concerns. Animation may present server-authoritative actions but may never decide attacks, hits, movement transitions, equipment changes, damage, or death.

Headless simulation and server projects must remain independent of SDL windowing, SDL GPU, ImGui, shaders, textures, editor capture tooling, and animation presentation. The experiment may reuse client asset-cooking patterns, but no render asset or dependency may enter the server audience.

## Reproduction and validation

From the Royale checkout:

```sh
git rev-parse HEAD
sh thirdparty/fetch-simplemesh.sh
git -C thirdparty/repos/SimpleMesh rev-parse HEAD
dotnet restore tests/Royale.Rendering.Tests/Royale.Rendering.Tests.csproj -p:CI_DONT_TARGET_ANDROID=1
dotnet test tests/Royale.Rendering.Tests/Royale.Rendering.Tests.csproj --no-restore \
  --filter 'FullyQualifiedName~StaticMeshRenderingTests|FullyQualifiedName~ModelContactSheetFramingTests' -m:1
ROYALE_GPU_TESTS=1 dotnet test tests/Royale.Rendering.Tests/Royale.Rendering.Tests.csproj \
  -m:1 --no-restore --no-build --filter FullyQualifiedName~SdlGpuIntegrationTests
```

The focused managed run passed 29 tests. The native macOS ARM64 SDL GPU run passed its standalone harness test after running outside the sandbox so VSTest could open its local communication socket. The host was Darwin `osx-arm64` with .NET SDK 10.0.301 and `shadercross` available on `PATH`.

The load probe can be reproduced with a disposable net8.0 console project referencing pinned `SimpleMesh.csproj`. For each model, enumerate `Model.Roots`, node geometry descriptors, `Model.Skins`, and `Model.Animations`. The alias pass should implement `IExternalResources` and map only `T_Eye_Normal_png.png` to the existing `T_Eye_Normal.png`; do not modify the asset tree.