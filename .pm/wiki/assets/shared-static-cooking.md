---
title: Shared Static Asset Cooking
createdAt: 2026-08-02T17:49:59.5767640Z
modifiedAt: 2026-08-06T17:43:52.1372840Z
---

## Decision and ownership

Coordinator task `pm://project/prj_E7QP3LUocfY7k3PYM-EQOlqc/task/SHARED-0019` owns a provisional deterministic client-only static-mesh cook for the narrow shared renderer. Starfall and Royale still own game-specific asset identity, selection, composition and presentation mapping. This task selected no Quaternius or Kenney game asset.

The cook is implemented by `ChronoFall.StaticMeshCooker`. The BCL-only `ChronoFall.CharacterPresentation.Cooking` assembly owns the readable format; the provisional `ChronoFall.CharacterExperiment.SimpleMesh` adapter remains build-time only. No importer or native dependency enters the core/cooking assembly or a headless project.

## Version 1 format

A `.cfmesh` version 1 file contains:

- fixed magic and version;
- stable asset ID;
- one primary source path and hash;
- an ordered bounded list of external resource paths and hashes;
- CC0 identifier and hashed licence-evidence paths;
- positive metres-per-source-unit conversion evidence;
- the fixed `section-names-only` material policy;
- one immutable `StaticMeshDefinition` with positions, normalized normals, 32-bit triangle indices and ordered contiguous sections.

Readers reject bad magic/version, malformed UTF-8, truncation, trailing bytes and data beyond the fixed limits of 2,000,000 vertices, 6,000,000 indices, 4,096 sections, 64 external/evidence files and 16 KiB strings. Version 1 is provisional evidence, not a compatibility or distribution promise.

## Recipe, provenance and conversion

A recipe selects exactly one portable primary model plus every external resource it references. Every source and licence-evidence file carries a SHA-256. Directories, globs, archives, absolute/escaping paths, symlinked path components, undeclared resources and unused declared resources are rejected. The command accepts only `--audience client`, writes through temporary files and rechecks all protected inputs before replacement.

The importer accepts only OBJ, glTF and GLB through the already pinned SimpleMesh source. It fails on importer warnings, skins, animations, non-triangle geometry, missing/generated normals, invalid index groups, non-finite transforms, singular transforms and reflections. Scene hierarchy transforms are baked in deterministic preorder; positions receive the approved uniform metres conversion, normals use inverse transpose, and source orientation and pivot remain unchanged.

Material names and observed diffuse/PBR evidence are retained in deterministic provenance. Runtime output preserves only section diagnostic identities. UVs, textures, colours, alpha, two-sided behavior, PBR properties and engine-specific shaders are neither cooked nor silently claimed as supported.

SimpleMesh patch `0002-use-invariant-culture-for-obj-floats.patch` makes OBJ/MTL multi-value floating parsing invariant. This prevents decimal-comma machines from turning `-1.25` into `-125`.

## Reproduction evidence

The test fixture under `tests/fixtures/static-cooking/` is coordinator-authored synthetic CC0 geometry, not selected game art. It contains two exact OBJ sections plus one MTL and licence file.

```sh
dotnet run --project tools/ChronoFall.StaticMeshCooker/ChronoFall.StaticMeshCooker.csproj \
  -c Release -- \
  --source-root . \
  --recipe tests/fixtures/static-cooking/two-boxes.recipe.json \
  --output /tmp/chronofall-static-two-boxes.cfmesh \
  --provenance-output /tmp/chronofall-static-two-boxes.provenance.json \
  --audience client
```

The validated cook is 1,967 bytes with SHA-256 `c04d5071091d36a1b18edc187854e29395f107e0529cbaa8c63e1c8c592b78c2`. Its deterministic provenance hash is `20dc5e30892bdc8dc4edbce13bc93c4f289c8e85d43c1ad205078111e0ee3312`.

The GPU harness accepts `--static-proof --cooked-static-asset <path>`. The cooked fixture reproduces the approved direct renderer fingerprints `247198b9ff0e2862 / 7d2c37c52e46fb19 / 247198b9ff0e2862` and byte-identical PPM SHA-256 `5c45a75532678dc94a69334d6d693b08d0f4544c247a92177d893acc690f0b43`.

## Selection and staging boundary

SHARED-0019 deliberately added no real static recipe or child output. Starfall selection task `pm://project/prj_pkIpzx0fzFD4URjvqBuYrGZF/task/CONTENT-0011` later selected the exact Quaternius Medieval Weapons Pack `Bow_Wooden` and `Arrow` inputs.

Coordinator acquisition task `pm://project/prj_E7QP3LUocfY7k3PYM-EQOlqc/task/ASSET-0006` is the first bounded real consumer. It adds exactly two recipes at `0.25` metres per source unit:

- `assets/recipes/quaternius-medieval-weapons-bow-wooden.json`;
- `assets/recipes/quaternius-medieval-weapons-arrow.json`.

The stable-project-ID staging workflow now permits only their two cooked `.cfmesh` files, deterministic provenance sidecars and one preserved Medieval Weapons licence in addition to the existing selected UAL1 character cook. Generated files remain ignored and client-only. No raw model, entire pack, runtime manifest, production material or child source enters the boundary.

Exact source hashes, bounds, output hashes and owner scale validation are recorded at `pm://project/prj_E7QP3LUocfY7k3PYM-EQOlqc/wiki/assets/quaternius-medieval-weapons-bow-arrow-cook`.

Future real selections remain separately task-owned. ASSET-0007 may later add only exact Starfall-selected zone inputs. ASSET-0008 remains evidence-gated until Starfall selects a monster representation and proves whether static, rigid or skeletal acquisition is actually required.

## Deferred work

This contract does not provide textures, production materials, alpha, two-sided rendering, bounds, collision, LOD, compression, streaming, terrain, vegetation, scene/render graphs, asset catalogues, package distribution or child integration. Any such need requires later task-owned evidence and approval.