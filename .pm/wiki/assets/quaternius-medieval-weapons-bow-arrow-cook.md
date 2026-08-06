---
title: Quaternius Medieval Weapons Bow and Arrow Cook
createdAt: 2026-08-06T17:43:33.4884970Z
modifiedAt: 2026-08-06T18:37:54.9953410Z
---

## Decision and ownership

Starfall task `pm://project/prj_pkIpzx0fzFD4URjvqBuYrGZF/task/CONTENT-0011` selected the supplied Quaternius Medieval Weapons Pack `Bow_Wooden` and `Arrow` as the provisional Draft 0 weapon inputs. Coordinator task `pm://project/prj_E7QP3LUocfY7k3PYM-EQOlqc/task/ASSET-0006` owns their exact acquisition recipe, deterministic client cook, provenance and stable-project-ID staging.

This selection is presentation input only. Starfall still owns the semantic hand socket, local bow transform, arrow nocking/release presentation and later combat integration. It does not define equipment, a ranger loadout, grip/IK, aim, authoritative projectiles, ammunition, materials or final art.

## Supplied source and licence

The authoritative source is the committed pack at `assets/Quaternius/Medieval Weapons Pack by @Quaternius/`. The preserved `License.txt` identifies the pack as Quaternius content released under CC0 1.0. The licence file SHA-256 is:

`d32abf5eb61a5d20c582525c2ee9d8d42d86401d6b3ea0a2d5283fcaecaa35b9`

Only these exact OBJ/MTL pairs are selected:

| Logical asset | Primary source | SHA-256 | External material library | SHA-256 |
| --- | --- | --- | --- | --- |
| `quaternius-medieval-weapons-bow-wooden` | `assets/Quaternius/Medieval Weapons Pack by @Quaternius/OBJ/Bow_Wooden.obj` | `788c9e72bdd839a86704113de4809a96cfedf09441bb3f98f383a7abfe751e6d` | `assets/Quaternius/Medieval Weapons Pack by @Quaternius/OBJ/Bow_Wooden.mtl` | `545318d522d6ab3f0f4942cd5fc25001fcc9c1a722cef2d04555009721847a54` |
| `quaternius-medieval-weapons-arrow` | `assets/Quaternius/Medieval Weapons Pack by @Quaternius/OBJ/Arrow.obj` | `6960c207e3a8e6f2f09cbfd31b7fe990119cd260ef692729c498738a86698bf1` | `assets/Quaternius/Medieval Weapons Pack by @Quaternius/OBJ/Arrow.mtl` | `cee901eef3fabe40154cc3a13ed3d64181aac886767fb1132382667332c6891f` |

The Blender, FBX and remaining pack contents stay authoritative supplied sources but are not selected, copied or cooked by this task.

## Recipes and conversion

The exact recipes are:

- `assets/recipes/quaternius-medieval-weapons-bow-wooden.json`;
- `assets/recipes/quaternius-medieval-weapons-arrow.json`.

Both use `0.25` metres per source unit, preserve the OBJ coordinate axes and pivot without recentering or normalization, target only the client audience and use the provisional `section-names-only` material policy.

The bow retains ordered sections `DarkWood`, `LightWood` and `White`. Its cooked bounds are:

- minimum `(-0.09612475, -0.6795755, -0.030426)` metres;
- maximum `(0.27085575, 0.6795755, 0.0304265)` metres.

Its dominant extent is approximately `1.359151` metres.

The arrow retains ordered sections `LightWood`, `Steel`, `LightSteel` and `Red`. Its cooked bounds are:

- minimum `(-0.034382, -0.0213025, -0.01055675)` metres;
- maximum `(0.03415425, 0.038266, 0.67289925)` metres.

Its dominant extent is approximately `0.683456` metres.

The source Blend files use their own Blender-oriented axes. An unsaved native Blender comparison used the exact Blend meshes at the same uniform `0.25` scale beside a 1.8 metre reference. The owner confirmed that the approximately 1.36 metre bow and 0.68 metre arrow are credible. This validates scale and proportion only; it is not a socket, grip, nocking or equipped-character proof.

The reusable native-review procedure established by this task is recorded at `pm://project/prj_E7QP3LUocfY7k3PYM-EQOlqc/wiki/assets/native-blender-asset-evaluation`.

## Deterministic outputs

The static cooker produces:

| Output | Bytes | SHA-256 |
| --- | ---: | --- |
| `quaternius-medieval-weapons-bow-wooden.cfmesh` | 43,185 | `4c0ab766e7c622c0f52ff0ade3cb1992c6d96664233a4695fc049a3a9b1d642e` |
| `quaternius-medieval-weapons-bow-wooden.provenance.json` | deterministic sidecar | `d99a010fe7f357019413e624ca1c239092475d2edb52b61a8528bc775f5bce8e` |
| `quaternius-medieval-weapons-arrow.cfmesh` | 11,492 | `4eeb80dc06e1f729b67606eb6c12110b954068cfb7ea39590706771e4c02d9c3` |
| `quaternius-medieval-weapons-arrow.provenance.json` | deterministic sidecar | `c2eafd0392f27f2e5256cb4bc07d31b216bbd991371fd44a2dbdc664dc374244` |

Focused tests cook each recipe twice and compare the complete cooked bytes and provenance bytes. They also verify stable IDs, hashes, scale, licence policy, material-section count, portable provenance and exact bounds.

The version 1 `.cfmesh` output stores geometry, normals, indices and section identities. It does not cook textures or production material properties. The source materials contain no external diffuse texture in these OBJ selections; material colour evidence remains provenance only.

## Stable-project-ID staging

From the coordinator root:

```sh
scripts/cook-character-presentation-for-client.sh \
  --project-id prj_pkIpzx0fzFD4URjvqBuYrGZF
```

The existing workflow verifies the reciprocal linked-project identity, canonical checkout and tracked gitlink before writing. It stages the two `.cfmesh` files, deterministic provenance and a single preserved Medieval Weapons `License.txt` alongside the existing selected character cook.

All generated files remain under Starfall's ignored `artifacts/chronofall/character-presentation/client/` tree. The workflow refuses symlink escapes, tracked files and unexpected existing content. It accepts neither an alias nor an arbitrary output destination. No raw OBJ, MTL, Blend, FBX or complete pack is copied, no runtime manifest is created and no child source or PM data is changed.

## Next consumer

Shared task `pm://project/prj_E7QP3LUocfY7k3PYM-EQOlqc/task/SHARED-0020` completed the narrow reusable socketed-static attachment proof using the exact acquired `Bow_Wooden` cook. The coordinator harness renders it from a technical `hand_l` socket beside the UAL1 technical humanoid, validates deterministic placement across two `Idle_Loop` samples, and renders the skinned and static geometry in one caller-owned SDL GPU pass and depth target.

The owner-validated harness transform is technical evidence only: 0.09 metres along the hand grip axis, +0.03 metres across the palm, an 80-degree twist, and a -70-degree roll. Starfall `pm://project/prj_pkIpzx0fzFD4URjvqBuYrGZF/task/CLIENT-0011` still owns its provisional semantic socket, local bow transform, gameplay-facing rendering integration, and native placement validation. Arrow nocking/release, aiming, grip/IK, equipment, and projectile presentation remain separate work. This completed shared proof authorizes none of those tasks automatically.