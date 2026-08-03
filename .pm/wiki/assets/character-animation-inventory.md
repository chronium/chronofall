---
title: Quaternius Character and Animation Inventory
createdAt: 2026-08-01T07:17:10.6916090Z
modifiedAt: 2026-08-03T12:23:29.3051510Z
---

## Scope and provenance

This inventory covers only the supplied character and animation inputs under `assets/Quaternius/`:

- `Universal Base Characters[Standard]`: Standard/free subset, CC0 1.0, 112 payload files plus two `.DS_Store` files, 127 MB. Formats: 18 glTF + 18 external BIN, 26 FBX, 48 PNG, two TXT.
- `Universal Animation Library[Standard]`: CC0 1.0, nine payload files plus one `.DS_Store`, 61 MB. Formats: two GLB, two FBX, three PNG, two TXT.
- `Universal Animation Library 2[Standard]` (historical inspection; removed from the repository by `ASSET-0009`): CC0 1.0, 13 files, 69 MB. Formats: three GLB, three FBX, one Blender source, three PNG, three TXT.
- `Modular Character Outfits - Fantasy[Standard]`: Standard/free subset, CC0 1.0, 121 files, 292 MB. Formats: 24 glTF + 24 external BIN, 24 FBX, 46 PNG, one JPG, two TXT.

The included license files identify the models as by Quaternius and dedicate them to the public domain under CC0 1.0. Files presently retained in the repository are authoritative; no substitutes were downloaded. The UAL2 Standard measurements below are preserved historical evidence from the formerly retained snapshot, not evidence that those files remain available in the checkout. Medieval Weapons and Medieval Village MegaKit are supplied but are outside this task.

The Base Characters readme recommends glTF for rigged exports because of an FBX scaling issue in its target workflow. The animation readmes define `_RM` as root motion baked into every animation and the unsuffixed files as root motion disabled. The outfit readme states that the outfits work with Universal Base Characters and recommends omitting hidden body regions to prevent clipping.

## Exact source variants

### Base humanoids

- `Universal Base Characters[Standard]/Base Characters/Godot - UE/Superhero_Male_FullBody.gltf`
  - glTF SHA-256 `e7fcea214ecf8855afbf910b50de6f9c7d1decfb71ca28bad8a4481452dafeb4`
  - external `Superhero_Male_FullBody.bin`, 720,076 bytes, SHA-256 `459003f9745853ae562a85506a2b94dd56515c1f37728f9fa3d2ce1a3e4cd92f`
- `Universal Base Characters[Standard]/Base Characters/Godot - UE/Superhero_Female_FullBody.gltf`
  - glTF SHA-256 `adedf28000a0716f689b009a70314506fc62f827498f77ba852acb5610f3f3f4`
  - external `Superhero_Female_FullBody.bin`, 990,808 bytes, SHA-256 `3a8220a485b33d05d879115a50697728b45a151781106033afb8b8c243fca208`
- Unity variants: `Base Characters/Unity/Superhero_Male_FullBody.fbx` and `Superhero_Female_FullBody.fbx`.

Both glTFs use external BIN and PNG resources, contain 69 nodes, three meshes, one 65-joint skin, and no animation clips.

### Animation libraries

- `Universal Animation Library[Standard]/Unreal-Godot/UAL1_Standard.glb`: embedded BIN, 7,618,436 bytes, SHA-256 `69591853d817488edaa8fd9bf8fc1d821eaeaf789f8627b3cd23b41c4ed67997`.
- `Universal Animation Library[Standard]/Unreal-Godot/UAL1_Standard_RM.glb`: embedded BIN, 7,620,504 bytes, SHA-256 `be684571ed655a1b892c2c07e6e2aeca053b606c442d34004adaf1d944090d01`.
Historical UAL2 Standard evidence retained after the source files were removed by `ASSET-0009`:

- `Universal Animation Library 2[Standard]/Unreal-Godot/UAL2_Standard.glb`: embedded BIN, 8,091,444 bytes, SHA-256 `8cee20ab1bc55130092447e810e26df22dd2803eccc54f52137a7d54d7ab88a8`.
- `Universal Animation Library 2[Standard]/Unreal-Godot/UAL2_Standard_RM.glb`: embedded BIN, 8,095,936 bytes, SHA-256 `814eee878f82934992d3ea746c539df25e981487109c591f5efbb8dd03286f99`.
- The historical Unity equivalents were `UAL2_Standard[ _RM].fbx`.
- `Universal Animation Library 2[Standard]/Female Mannequin/Unreal-Godot/Mannequin_F.glb`: embedded BIN, no animations, 1,442,824 bytes, SHA-256 `2ee6cc3fe888d9b144afa8cc4b2ab7bfc5d13a0d5b7548df777f61f64ad65fa6`. Blender and Unity FBX variants were also inspected.

The retained UAL1 GLBs and the historically inspected UAL2 Standard GLBs each have 67 nodes, one mannequin mesh, one 65-joint skin, and 43 animation clips. No textures are embedded.

### Modular outfits

The pack contains four complete outfits—Female/Male Peasant and Female/Male Ranger—and 20 modular parts, each in glTF/external-BIN and FBX variants. The representative `Exports/glTF (Godot-Unreal)/Outfits/Female_Ranger.gltf` has nine meshes, one 65-joint skin, no animations, external PNG textures, and external `Female_Ranger.bin` (1,959,556 bytes, SHA-256 `d015964ba0a26eee9c9d2ea6f07aa660bba88aa536cf8bb6b67944b9e97c46df`). Its glTF SHA-256 is `9b98b2ae6af3b4a1ec22bf8928b939e0751f08b25b636de82bfa7364fb34d3e1`.

Modular armour is not part of M1; the representative inspection only tests the pack's stated rig relationship.

## Skeleton and skin evidence

All deep-inspected base characters, UAL mannequins, the female mannequin, and representative complete outfits have one skin with the same ordered 65 joint names and no parent-link mismatches:

```text
root -> pelvis
pelvis -> spine_01 -> spine_02 -> spine_03
spine_03 -> neck_01 -> Head
spine_03 -> clavicle_{l,r} -> upperarm -> lowerarm -> hand
hand -> index|middle|pinky|ring (01 -> 02 -> 03 -> 04_leaf)
hand -> thumb (01 -> 02 -> 03 -> 04_leaf)
pelvis -> thigh_{l,r} -> calf -> foot -> ball -> ball_leaf
```

The naming convention uses lowercase anatomical names, zero-padded segment numbers, `_l`/`_r` side suffixes, and `_leaf` terminal joints; `Head` and `Armature` retain capitals.

Every inspected mesh exposes only `JOINTS_0` and `WEIGHTS_0`; no second influence set is present. The measured maximum is four non-zero influences per vertex. Weight sums remain within floating-point tolerance of one:

- male base: 0.9999998771 to 1.0000001191;
- female base: 0.9999998647 to 1.0000001383;
- UAL mannequin: 0.9999998808 to 1.0000000894;
- female ranger: 0.9999998566 to 1.0000001444.

Each skin references 65 finite inverse-bind matrices through a `MAT4`, float (`componentType 5126`) accessor. The first root inverse-bind matrix is the same orientation transform in the inspected base and UAL files.

## Animation channels and timing

Both retained UAL1 variants contain the same 43 clip names, and both historically inspected UAL2 Standard variants contained the same 43 clip names. Every clip has 195 channels/samplers: translation, rotation, and scale for all 65 joints. All samplers declare `LINEAR` interpolation. Key spacing is approximately 0.0333333 seconds (30 Hz).

Evidence-bearing candidate clips in UAL1 are:

| Clip | Duration | Keys per channel | Non-RM root displacement | RM root displacement |
| --- | ---: | ---: | ---: | ---: |
| `Idle_Loop` | 2.500000 s | 76 | 0 | 0 |
| `Jog_Fwd_Loop` | 0.933333 s | 29 | 0 | 5.0 |
| `Walk_Loop` | 1.333333 s | 41 | 0 | 1.3 |
| `Sword_Attack` | 1.533333 s | 47 | 0 | 1.500506 |

UAL1 clip durations range from 0.166667 to 5.200000 seconds. The historical UAL2 Standard clip durations range from 0.433333 to 4.333333 seconds; examples include `Idle_FoldArms_Loop` (2.5 s), `Walk_Carry_Loop` (2.0 s), and `Sword_Regular_A` (0.433333 s). The non-RM forms keep measured root displacement at zero for these candidates; the RM forms carry translation as the readmes describe.

## Coordinate system and scale

The inspected files declare glTF 2.0 and were emitted by Khronos Blender glTF exporters 4.3.47 or 4.5.48. Their format contract is right-handed, Y-up, and metre-based. Observed humanoid bounds are consistent with that scale: the male base reaches approximately 1.810 m, the female base 1.767 m, and the UAL mannequin 1.829 m on Y. The base and UAL roots share the same exported orientation transform.

No coordinate or unit conversion should be committed during inventory. The experimental loader decision must state where glTF-to-renderer axis handling occurs and prove it with deterministic transform tests.

## Compatibility findings

Joint identity remains promising but is not sufficient proof of cross-file deformation compatibility:

- Male/female base, UAL1, UAL2, the female mannequin, and representative outfits all match 65/65 ordered names and hierarchy.
- The female base and complete Female Ranger outfit match local rest transforms and inverse-bind matrices exactly.
- The base-character rest transforms and inverse-bind matrices differ materially from the UAL mannequin. For example, male base versus UAL1 has 62 local-transform mismatches above `1e-5` (maximum component difference 0.146273) and 64 inverse-bind mismatches (maximum component difference 0.329977).
- Every UAL clip fully keys all three transforms for all 65 joints, but exact name/hierarchy mapping alone does not prove correct deformation across different rest poses.

`ASSET-0002` therefore avoids the unresolved cross-rig contract. The final M1 selection uses the mannequin mesh, `Armature` skin, inverse binds, and `Idle_Loop`, `Walk_Loop`, and `Sword_Attack` clips embedded together in the non-root-motion `UAL1_Standard.glb`.

Compatibility for that selection is structural: the `Mannequin` mesh node directly binds the selected 65-joint skin, and every selected clip targets exactly those same 65 joints with complete LINEAR TRS channels. The attack clip is therefore included without retargeting or cross-file mapping.

This supersedes the provisional female-base candidate. Universal Base Characters, their broken external image references, and base-to-UAL deformation compatibility are deferred rather than repaired or treated as M1 prerequisites.

The canonical selection, exact identifiers, root-motion evidence, exclusions, and downstream loader handoff are recorded at `pm://project/prj_E7QP3LUocfY7k3PYM-EQOlqc/wiki/assets/skinned-character-experiment-inputs`.

## Broken external references and conversion evidence

All external BIN/image URIs across the Base Character and Outfit glTF files were checked. Three supplied image URIs do not resolve:

- female base references `T_Eye_Normal_png.png`, while `T_Eye_Normal.png` is present;
- male base references `T_Eye_Normal_png.png`, while `T_Eye_Normal.png` is present;
- male base references `T_Hair_1_Normal_png.png`, while `T_Hair_1_Normal.png` is present.

The external BIN resources all resolve. Do not rename or rewrite supplied source files. The final M1 selection avoids these external references by using the embedded `UAL1_Standard.glb`; no URI repair or material remapping is required for the proof. Any future Universal Base Character work must make a separate, explicit correction/material-mapping decision.

The selected experiment requires embedded GLB JSON/BIN, float inverse-bind matrices, four-influence joint/weight attributes, and LINEAR TRS animation samplers. External glTF resources remain inventory evidence for later character integration, not approval for a permanent format, importer, converter, or native dependency.

## Reproduction

Run from the coordinator root:

```sh
find 'assets/Quaternius/Universal Base Characters[Standard]' -type f
find 'assets/Quaternius/Universal Animation Library[Standard]' -type f
find 'assets/Quaternius/Modular Character Outfits - Fantasy[Standard]' -type f
shasum -a 256 <source-file>
jq '{asset,nodes,meshes,skins,animations,buffers,images}' <source.gltf>
assimp info <source.glb>
```

The UAL2 Standard paths and hashes above are historical inspection evidence and are not reproducible from the current checkout after `ASSET-0009`. Do not restore or substitute that snapshot as part of reproduction. The separately owner-supplied private UAL2 Source package requires its own task-owned inventory and must remain outside the public repository.

For GLB, read the little-endian JSON chunk length at byte 12, then inspect the JSON chunk beginning at byte 20 with `jq`. Accessor values are reproduced by applying each accessor's component type/count/type and byte offset to its bufferView and buffer, honoring byte stride. Cross-check node/mesh/animation aggregates and hierarchy with `assimp info`.

Related provenance: `pm://project/prj_E7QP3LUocfY7k3PYM-EQOlqc/wiki/assets/quaternius-provenance`.