---
id: ASSET-0002
title: Select exact compatible experiment inputs
track: ASSET
milestone: M1
dependsOn:
- ASSET-0001
createdAt: 2026-08-01T05:34:31.1172840Z
modifiedAt: 2026-08-01T05:35:27.2181210Z
---

Select the smallest supplied CC0 input set for the skinned-character proof. Persist exact repository paths, embedded object identifiers, compatibility evidence, and the root-motion choice without copying, converting, or repairing unrelated assets.

## Acceptance criteria

- One exact repository-relative source path and its SHA-256 are recorded with committed Quaternius CC0 provenance.
- The selected humanoid, skin, idle, locomotion, and compatible attack identifiers are recorded.
- Compatibility is demonstrated from the selected mesh/skin binding and complete animation targets, not inferred from names.
- Root-motion versus in-place behavior is verified across every root-translation sample.
- Explicit exclusions prevent cross-rig work, retargeting, bulk processing, source repair, or a permanent loader decision.
- The selection and downstream loader handoff are durable in the coordinator wiki.

## Selection result

Use only:

`assets/Quaternius/Universal Animation Library[Standard]/Unreal-Godot/UAL1_Standard.glb`

SHA-256: `69591853d817488edaa8fd9bf8fc1d821eaeaf789f8627b3cd23b41c4ed67997`.

Embedded proof inputs:

- humanoid node/mesh: `Mannequin`;
- skeleton/skin: `Armature`, 65 joints and 65 finite inverse-bind matrices;
- idle: `Idle_Loop`, 2.500000000 seconds;
- locomotion: `Walk_Loop`, 1.333333373 seconds;
- attack: `Sword_Attack`, 1.533333302 seconds.

The mesh node binds this exact skin. Each selected clip has 195 LINEAR channels—translation, rotation, and scale for every selected joint—and targets exactly the skin's 65-joint set. This same-file relationship demonstrates compatibility without cross-rig mapping or retargeting.

The unsuffixed file is the in-place variant. Full accessor decoding found zero root translation at every key: 76 samples for idle, 41 for walk, and 47 for attack. Scale remains identity within exporter noise, with a maximum measured deviation of `4.768371582e-7`.

## Provenance and format notes

The pack's committed `License.txt` and `README.txt` identify Quaternius and CC0 1.0 Universal / public-domain dedication. The selected 7,618,436-byte glTF 2.0 GLB contains embedded binary data, no external images or textures, two triangle primitives, at most four non-zero influences per vertex, and normalized weights within floating-point tolerance.

Canonical decision record: `pm://project/prj_E7QP3LUocfY7k3PYM-EQOlqc/wiki/assets/skinned-character-experiment-inputs`.

Source inventory: `pm://project/prj_E7QP3LUocfY7k3PYM-EQOlqc/wiki/assets/character-animation-inventory`.

## Boundaries and handoff

The selection excludes the `_RM` variant, Universal Base Characters, UAL2, outfits, armour, equipment, external-resource repair, cross-rig deformation, retargeting, conversion, and permanent skeletal formats or importers.

Pinned SimpleMesh loads this GLB and exposes its skinned geometry, skin, and animations, but omits source scale channels. The selected clips' scale values happen to be effectively identity; that does not authorize discarding scale permanently. `EXPERIMENT-0002` owns the reviewed loader decision and failure behavior.

No asset, source, shader, child PM record, or submodule pointer is changed by this task.