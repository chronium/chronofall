---
title: Skinned-Character Proof Findings and Promotion Criteria
createdAt: 2026-08-01T16:36:40.6943920Z
modifiedAt: 2026-08-02T11:44:59.5852120Z
---

## Decision

M1 proves that the supplied Quaternius humanoid and compatible same-file animation data can be loaded, sampled deterministically, inspected, and rendered correctly through native SDL GPU skinning. That proof authorized the focused promotion in `pm://project/prj_E7QP3LUocfY7k3PYM-EQOlqc/task/SHARED-0001`.

The promotion preserves the demonstrated semantics and boundaries in `ChronoFall.CharacterPresentation` and `ChronoFall.CharacterPresentation.SdlGpu`. `ChronoFall.CharacterExperiment.SimpleMesh`, `ChronoFall.CharacterExperiment.SdlGpu`, and the GPU harness remain provisional validation consumers rather than shared loader, window, camera, capture or scene APIs.

## M1 acceptance evidence

| Requirement | Result | Durable evidence |
| --- | --- | --- |
| Supplied humanoid and compatible animation | Passed | The unchanged `UAL1_Standard.glb` supplies the mannequin, 65-joint skin, and same-file `Idle_Loop`, `Walk_Loop`, and `Sword_Attack` clips. See `pm://project/prj_E7QP3LUocfY7k3PYM-EQOlqc/wiki/assets/skinned-character-experiment-inputs`. |
| Deterministic transforms and sampling | Passed | Complete LINEAR TRS sampling, explicit clamp/loop time mapping, parent-first pose evaluation, inverse-bind palettes, exact loop boundaries, and selected-asset fixtures are covered by `pm://project/prj_E7QP3LUocfY7k3PYM-EQOlqc/wiki/experiments/skeletal-data-contract`. |
| Correct GPU skinning | Passed | Joint indices, weights, and a 65-matrix palette deform positions and normals. A translated-palette probe proves the shader consumes the skinning data. |
| Skeleton and joint debugging | Passed | The evaluated global pose produces the hierarchy links and local axes; debugging does not reconstruct joints from inverse-bind palette matrices. |
| Deterministic captures | Passed | Five native 512 by 512 captures reproduce stable hashes at bind pose and multiple `Walk_Loop` timestamps; the exact duration is byte-identical to time zero. |
| Native macOS ARM64 execution | Passed | The SDL GPU path, MSL shaders, offscreen readback, visible browser, and focused native test all pass on macOS ARM64 Metal. |
| Explicit owner validation | Passed | The owner approved deformation, orientation, scale and framing, animation appearance, controls, and the animated skeleton overlay in `pm://project/prj_E7QP3LUocfY7k3PYM-EQOlqc/task/EXPERIMENT-0010`. |
| Headless separation | Passed | The BCL-only skeletal data and animation assembly has no SDL, GPU, SimpleMesh, child-game, server, or simulation dependency. No child or headless project references the experiment. |

The full GPU contract, fingerprints, capture hashes, controls, and owner evidence are recorded at `pm://project/prj_E7QP3LUocfY7k3PYM-EQOlqc/wiki/experiments/sdl-gpu-bind-pose`. The preserved project-history contact sheet remains the visual checkpoint for this proof.

## Proven input envelope

The proof establishes one intentionally narrow source envelope:

- glTF 2.0 GLB with embedded resources;
- one compatible mesh, skin, and animation set in the same file;
- one parent-first 65-joint hierarchy and one inverse-bind matrix per joint;
- at most four influences per vertex;
- complete finite translation, rotation, and scale tracks for every joint;
- strictly increasing key times using LINEAR interpolation;
- right-handed, Y-up, metre-based source space;
- identity relationship between the selected mannequin and `Armature` space;
- in-place animation with no root translation.

The loader deterministically rejects unsupported interpolation, unresolved or duplicate targets, incomplete TRS, malformed keys, invalid hierarchy or skin data, and invalid triangle geometry. This is evidence for strict validation and useful diagnostics; it is not evidence that the loader supports arbitrary glTF character assets.

## Promotion matrix

| Area | Classification | M2 implication |
| --- | --- | --- |
| Skeleton, skin, pose, and influence semantics | Ready for shared-module design | Preserve immutable validated data, parent-first hierarchy, exact skeleton identity, finite inverse binds, normalized four-lane influences, and explicit failures. Current namespaces and type names are not frozen. |
| Animation clips and sampling | Ready for shared-module design | Preserve complete TRS tracks, explicit playback mode, Euclidean looping, endpoint holding, LINEAR vectors, shortest-path normalized quaternion interpolation, and deterministic duration. |
| Pose and palette evaluation | Ready for shared-module design | Preserve `Scale * Rotation * Translation`, parent-first globals, `inverseBind * posedGlobal`, finite matrices, and no CPU transposition. |
| Debug-pose boundary | Ready for shared-module design | Debug hierarchy and axes consume evaluated global transforms; skinning palettes remain a separate vertex-deformation representation. |
| GPU skinning behavior | Proven, exact ABI not frozen | Retain joint/weight-driven position and normal deformation, palette-buffer delivery, and exactly one transpose at the GPU boundary. Re-evaluate vertex stride, attributes, storage slots, shader entry points, and material inputs. |
| SDL GPU lifecycle and readback | Proven implementation evidence | The parent-owned SDL3-CS acquisition, explicit resource ownership, offscreen targets, readback, and native harness are useful inputs. They are not yet a permanent public wrapper or render framework. |
| SimpleMesh adapter and patch | Experiment-only | The selected input loads reproducibly, but SimpleMesh is not approved as a permanent dependency or shared loader API. Its focused interpolation/scale patch remains evidence for required importer behavior. |
| Source loading and cooked format | Deferred | `SHARED-0002` must decide client-only cooking and output boundaries without committing to a permanent format before evidence. |
| Harness, fixed camera, captures, and browser controls | Experiment-only | Preserve as validation evidence and possible tooling patterns, not runtime shared APIs. |
| Materials, textures, UV1, and animated bounds | Unproven | The diagnostic renderer deliberately omits production material and bounds contracts. |
| Cross-rig animation and production character features | Deferred | Retargeting, root motion, blending, masks, modular armour, sockets, attachments, equipment, IK, aim offsets, and animation graphs remain separate M2 decisions. |

## Concrete family demand

Promotion has two explicit child consumers:

- Royale: `pm://project/prj__-jXLQgm6GuD2gCKZ_bTa1m-/task/RENDER-012`;
- Starfall: `pm://project/prj_pkIpzx0fzFD4URjvqBuYrGZF/task/CLIENT-0006`.

Both depend canonically on `pm://project/prj_E7QP3LUocfY7k3PYM-EQOlqc/task/SHARED-0001`. Neither child may depend on the other. Their adapters, gameplay events, protocols, content, build, validation, and release lifecycle remain child-owned.

## SHARED-0001 handoff

The promotion task must:

1. design focused parent-owned modules that remain independent of Royale and Starfall;
2. keep reusable skeletal data and deterministic animation math free of SDL, GPU, importer, editor, and child dependencies;
3. keep SDL GPU rendering and native lifetime management in client-only presentation modules;
4. preserve server authority: child state and events select presentation, while animation never decides gameplay outcomes;
5. choose final module names and API shapes deliberately instead of renaming the experiment assemblies mechanically;
6. treat SimpleMesh, SDL3-CS, vertex packing, shader bindings, and resource ownership as reviewed dependency or ABI decisions;
7. retain deterministic managed tests and native Metal evidence while avoiding a general renderer, scene system, ECS, or Unity-like engine;
8. leave cooking and child integration to their owning tasks; after a child task commit, record gitlink advancement as its automatic pointer-only coordinator follow-up rather than a separate PM task.

Completing this M1 decision makes `SHARED-0001` dependency-ready. It does not activate that task or authorize extraction.

### Promotion result

The approved promotion is implemented by `ChronoFall.CharacterPresentation` and `ChronoFall.CharacterPresentation.SdlGpu`; the durable contract is `pm://project/prj_E7QP3LUocfY7k3PYM-EQOlqc/wiki/architecture/shared-character-presentation`.

The experiment loader and diagnostic host remain provisional consumers. Native Metal validation retains the M1 fingerprints, proving that the promoted path—not duplicate harness rendering code—still consumes four joint lanes, weights, palettes and deterministic poses. Child integration and distribution remain separately owned.