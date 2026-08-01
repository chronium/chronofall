---
title: Experimental Skeletal Loader Decision
createdAt: 2026-08-01T09:09:44.4811970Z
modifiedAt: 2026-08-01T09:09:44.4811970Z
---

## Decision

For the M1 skinned-character proof, use the evaluated SimpleMesh revision as the importer foundation and extend it through a focused ChronoFall-owned patch. The patch must preserve glTF scale channels and interpolation metadata instead of silently discarding them. The experiment adapter accepts LINEAR translation, rotation, and scale channels and maps them into the experiment-only data contract defined by `pm://project/prj_E7QP3LUocfY7k3PYM-EQOlqc/task/EXPERIMENT-0003`.

This is an experimental loader decision, not promotion of SimpleMesh as a permanent shared-engine dependency or approval of a permanent skeletal format.

## Evidence

- Selected input: `assets/Quaternius/Universal Animation Library[Standard]/Unreal-Godot/UAL1_Standard.glb`.
- Selected file SHA-256: `69591853d817488edaa8fd9bf8fc1d821eaeaf789f8627b3cd23b41c4ed67997`.
- Evaluated SimpleMesh revision: `9f46341e35fa5876fbea7b96bd021bc3abd7842d`.
- The embedded GLB contains one compatible mesh, one 65-joint skin, 65 finite inverse-bind matrices, and same-file clips.
- Each selected clip contains translation, rotation, and scale channels for all 65 joints using LINEAR interpolation.
- The evaluated SimpleMesh core exposes geometry, hierarchy, skin, inverse binds, joint/weight attributes, translation, and rotation, but discards scale channels and does not retain interpolation metadata.

Detailed evidence remains in `pm://project/prj_E7QP3LUocfY7k3PYM-EQOlqc/wiki/assets/skinned-character-experiment-inputs` and `pm://project/prj_E7QP3LUocfY7k3PYM-EQOlqc/wiki/experiments/royale-skeletal-capability-evaluation`.

## Options considered

| Option | Result | Reason |
| --- | --- | --- |
| Focused SimpleMesh patch | Selected | Reuses the already-evaluated geometry, skin, hierarchy, and animation importer while correcting the exact scale/interpolation omissions without adding another dependency. |
| Strict subset adapter around unmodified SimpleMesh | Rejected | It would require a second GLB validation path to prove scale and interpolation while still leaving the importer API incomplete and easy to misuse. |
| New glTF importer | Rejected for M1 | It adds dependency, API, integration, and maintenance decisions beyond the smallest proof. It may be reconsidered only with evidence from the experiment. |

## Supported experiment subset

The first adapter supports only the selected embedded glTF 2.0 GLB requirements:

- indexed triangle geometry with positions, normals, texture coordinates, `JOINTS_0`, and `WEIGHTS_0`;
- at most four vertex influences;
- one same-file joint hierarchy and skin with float inverse-bind matrices;
- finite translation, rotation, and scale keyframes;
- strictly increasing key times within each channel;
- LINEAR interpolation;
- node targets that resolve uniquely within the loaded model.

SimpleMesh should retain interpolation metadata on imported channels. The ChronoFall adapter, rather than a generic SimpleMesh sampler, owns the M1 subset check. STEP and CUBICSPLINE channels must fail deterministically instead of being sampled as LINEAR.

Errors must identify the source asset, clip, target node when available, channel path, and reason. Missing targets, empty or malformed channels, non-finite values, non-increasing key times, and unsupported interpolation are load failures for the experiment.

## Transform and timing ownership

The loader preserves glTF's right-handed, Y-up, metre-based source space. It does not bake a coordinate conversion, flatten the hierarchy, retarget joints, or infer root motion.

The experiment data and sampling contracts own clip duration, loop-boundary behavior, hierarchy evaluation, matrix convention, bind/local pose representation, inverse-bind composition, and the GPU palette. Those decisions belong to the following tasks rather than the third-party importer.

## Third-party ownership

ChronoFall will acquire dependencies per demonstrated parent consumer:

- a coordinator experiment or shared module owns its own pin, fetch script, license evidence, and focused patches;
- parent source must never reference `royale/thirdparty` or `starfall` dependency paths;
- Royale and Starfall retain independent acquisition needed for useful standalone checkouts;
- temporarily repeated upstream pins are acceptable until a proven shared module and distribution contract justify consolidation;
- moving a dependency pin does not itself promote the dependency or its API into the shared engine.

For the next implementation task, ChronoFall will acquire SimpleMesh only. SDL3-CS remains deferred to the first coordinator GPU task that consumes it. Box3D, ImGui.Net, BlurgText, LiteNetLib, WattleScript, and other Royale dependencies are not copied speculatively.

## Explicit non-goals

- No source, importer, patch, build, or third-party acquisition is implemented by this decision task.
- No native dependency is added.
- No asset is converted, cooked, renamed, or modified.
- No retargeting, root motion, blending, modular armour, IK, animation graph, generic asset framework, or permanent custom format is introduced.
- No Royale or Starfall source, PM data, dependency pin, commit, or gitlink is changed.

## Handoff

A separate coordinator-owned M1 task will establish the scoped SimpleMesh acquisition, patch, adapter, and focused loader tests after the experiment data contract exists. The bind-pose rendering task must wait for both deterministic transform/sampling tests and that loader-adapter task. The follow-up remains inactive until selected and planned by the owner.