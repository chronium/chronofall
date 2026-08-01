---
title: Kenney All-in-One Curation and Source Policy
createdAt: 2026-08-01T08:34:07.8619950Z
modifiedAt: 2026-08-01T08:34:07.8619950Z
---

# Kenney All-in-One curation

## Scope and provenance

The owner purchased and supplied Kenney All-in-One v3.6.0 as an external, read-only source library. The compilation is not committed to ChronoFall, and this documentation does not record its machine-specific absolute path.

- Official source: https://kenney.itch.io/kenney-game-assets
- Supplied bundle version: 3.6.0
- Individual pack licence: Creative Commons Zero 1.0, supported by the supplied pack `License.txt` files and https://creativecommons.org/publicdomain/zero/1.0/
- Redistribution boundary: the supplied bundle readme asks users not to redistribute the complete All-in-One compilation and instead directs them to the purchase page. Individual assets and packs remain governed by their supplied licences.
- Snapshot inventory: 264 pack directories, 88,346 non-metadata files, approximately 975.07 MiB.
- Machine metadata found but ignored: three `.DS_Store` files.
- No assets were copied, modified, converted, cooked, renamed, deleted, or added to a manifest by ASSET-0003.

The catalogue has two levels:

- Full pack catalogue: pm://project/prj_E7QP3LUocfY7k3PYM-EQOlqc/wiki/assets/kenney-pack-catalog
- Focused 112-asset candidate index: pm://project/prj_E7QP3LUocfY7k3PYM-EQOlqc/wiki/assets/kenney-candidate-index

Search the candidate index before looking outside the supplied libraries. Search the full pack catalogue when the shortlist does not cover a need.

## Usefulness labels

| Label | Meaning |
|---|---|
| Immediate | Clearly useful for an already planned task. |
| Near-term | Likely useful during the first playable work. |
| Situational | Worth remembering for a specific future need. |
| Archive | Available, but no current reason to prefer it. |

## High-value packs

| Exact pack path | Usefulness | Primary consumers | Why it matters |
|---|---|---|---|
| `2D assets/Development Essentials` | Immediate | shared/editor tooling | UV, alpha, normal, noise, gradient, and fallback diagnostics. |
| `2D assets/Prototype Textures` | Immediate | shared/editor tooling, Royale, Starfall | Readable graybox grids and semantic prototype surfaces. |
| `3D assets/Prototype Kit` | Immediate | shared/editor tooling, Royale, Starfall | Primitive geometry, markers, targets, triggers, stairs, walls, and props. |
| `Icons/Game Icons` | Near-term | shared/editor tooling | Generic save/open/import/export/settings/validation symbols. |
| `Icons/Input Prompts` | Near-term | Royale, Starfall | Keyboard, mouse, controller, handheld, touch, and generic prompts with sheets/XML. |
| `UI assets/UI Pack - Adventure` | Near-term | Starfall, shared/editor tooling | Fantasy-compatible panels, progress bars, minimap pieces, and controls. |
| `UI assets/UI Pack` | Near-term | shared/editor tooling | Broad neutral UI vocabulary and a few supplied sounds. |
| `UI assets/Cursor Pack` | Near-term | shared/editor tooling | Scalable cursor vocabulary for editors and tools. |
| `2D assets/Crosshair Pack` | Near-term | Royale | Crosshair sheets and vector source. |
| `2D assets/Particle Pack` | Near-term | Royale, Starfall | Transparent sparks, smoke, slashes, magic, rings, glow, and pickup accents. |
| `Audio/Interface Sounds` | Near-term | shared/editor tooling | Compact UI feedback vocabulary. |
| `Audio/Impact Sounds` | Situational | Royale, Starfall | Footsteps and material impacts with variation families. |
| `Audio/RPG Audio` | Near-term | Starfall | Coins, books, doors, cloth, weapon handling, and tool actions. |
| `Early access/Medieval Weapons` | Near-term | Starfall, future shared presentation | CC0 v0.1 weapon and shield candidates in GLB/FBX/OBJ with per-model previews. |
| `3D assets/Nature Kit` | Situational | Starfall | Trees, rocks, fences, bridges, camp props, and landscape fillers. |
| `3D assets/Survival Kit` | Situational | Starfall | Containers, camps, signs, workbenches, tents, and resource props. |
| `3D assets/Graveyard Kit` | Situational | Starfall | Graveyard, dungeon, fence, lantern, and crypt props. |
| `3D assets/Furniture Kit` | Situational | Starfall | Broad interior filler set; some modern styling limits direct use. |
| `3D assets/Fantasy Town Kit` | Situational | Starfall | Available for later prop comparison, not a replacement for the selected village. |
| `2D assets/Minimap Pack` | Situational | Starfall, Royale | Small fixed minimap tile vocabularies; not a map-rendering solution. |
| `Archive/*` | Archive | archive only | Discoverable historical snapshots with no present reason to prefer them. |
| `Goodies/*` | Archive | archive only | Wallpapers and promotional material, not development inputs. |

Other packs remain available through the complete pack catalogue. “Available” never means selected, imported, or approved for runtime use.

## Current Quaternius/Kenney selection policy

Quaternius remains selected for current character and initial Starfall presentation work:

- `Universal Base Characters[Standard]`: humanoid base character and canonical/reference skeleton.
- `Universal Animation Library[Standard]` and `Universal Animation Library 2[Standard]`: animation sources.
- `Modular Character Outfits - Fantasy[Standard]`: modular armour and clothing.
- `Medieval Village MegaKit[Standard]`: initial village/environment source.

Kenney character, animation, armour, clothing, and village alternatives remain merely available for later owner-approved evaluation. They do not change ASSET-0002 or the skinned-character proof assumptions.

Weapons are deliberately not exclusive: `Medieval Weapons Pack by @Quaternius` and Kenney `Early access/Medieval Weapons` are both future task-owned candidates. ASSET-0003 selects and integrates neither.

Stylistically, Kenney’s prototype/debug/UI material is neutral and can sit beside Quaternius. The shortlisted medieval weapons and environment props use a similarly readable low-poly language, but palette, texture treatment, source scale, pivot, grip/socket alignment, and scene cohesion require native visual validation before selection.

## Medieval Weapons Early Access snapshot

The actual supplied path is `Early access/Medieval Weapons`.

- Supplied licence title/version: Medieval Weapons 0.1.
- Supplied creation timestamp: 17 May 2026, 22:52.
- Bundle history: added to Early Access in the v3.5.0 changelog and retained in v3.6.0.
- Contents excluding machine metadata: 94 files, approximately 1.69 MiB.
- Model coverage: 17 GLB, 17 FBX, and 17 OBJ/MTL weapon or shield models, a shared colour map, overview, and per-model PNG previews.
- Licence evidence: supplied CC0 1.0 `License.txt`.
- Status: candidate source only; no weapon is selected or imported.

## Pack rename/search aliases

The changelog uses these historical-to-current search aliases. “Snapshot result” records what is actually present rather than assuming the rename was applied perfectly.

| Historical name | Changelog current name | v3.6.0 snapshot result |
|---|---|---|
| Isometric Vehicles #1 | Isometric Tiles Vehicles | Current name present. |
| Isometric Buildings #1 | Isometric Tiles Buildings | Current name present. |
| Isometric City | Isometric Tiles City | Current name present. |
| Isometric Landscape | Isometric Tiles Landscape | Neither name present. |
| Animated Characters 1 | Animated Characters Survivors | Current name present. |
| Animated Characters 2 | Animated Characters Protagonists | Current name present. |
| Animated Characters 3 | Animated Characters Retro | Current name present. |
| Retro Textures 1 | Retro Textures Fantasy | Current name present. |
| Retro Medieval Kit | Retro Fantasy Kit | Current name present. |
| Mini Characters 1 | Mini Characters | Current name present. |
| Toon Characters Pack 1 | Toon Characters | Current name present. |
| Puzzle Pack | Puzzle Pack 1 | Current name present. |
| 1-Bit Input Prompts Pixel 16× | Input Prompts Pixel 1-Bit | Current name present. |
| Input Prompts Pixel 16× | Input Prompts Pixel | Current name present. |
| Pattern Pack 2 | Pattern Pack Lines | Current name present. |
| Holiday Pack 2016 | Holiday Extras | Snapshot still contains `Holiday Pack 2016`; `Holiday Extras` is absent. |
| Animal Pack Redux | Animal Pack Remastered | Current name present. |
| Background Elements Redux | Background Elements Remastered | Current name present. |
| Platformer Pack Redux | Platformer Pack Remastered | Current name present. |
| Space Shooter Redux | Space Shooter Remastered | Current name present. |
| Top-down Tanks Redux | Top-down Tanks Remastered | Snapshot uses `Topdown Tanks Remastered`. |
| Platformer Art Pixel Redux | Platformer Art Pixel | Neither exact name is present; `Platformer Assets Pixel` exists. |
| Isometric Detailed Dungeon Pack | Isometric Miniature Dungeon | Current name present. |
| Isometric Detailed Farm Pack | Isometric Miniature Farm | Current name present. |
| Isometric Detailed Library Pack | Isometric Miniature Library | Current name present. |
| Isometric Detailed Overworld Pack | Isometric Miniature Overworld | Current name present. |
| Isometric Detailed Prototype Pack | Isometric Miniature Prototype | Current name present. |

## Portability and format findings

- Dominant formats are PNG, FBX, SVG, OBJ/MTL, GLB, and OGG; older packs also contain DAE, STL, glTF/BIN, SWF, Unity packages, and archives.
- No symlinks, case-insensitive filename collisions, Windows-reserved names, trailing-dot/space paths, control characters, or files at common Git hosting size limits were found.
- Five path components contain non-ASCII multiplication signs, covering 701 files. Later imports may rename selected copies only when the focused task records the mapping.
- The snapshot contains 58 ZIP archives. Comparison did not establish any as fully duplicated by expanded files, so curation does not recommend deleting or preferring an alternate representation.
- GLB is attractive for later focused static-model experiments, but loader choice, scale normalization, material interpretation, collision, and cooking remain consuming-task decisions.
- Supplied previews are source evidence only. They are not copied into this repository by ASSET-0003.

## Recommended first import task

The smallest broadly useful first import would be a focused shared/editor diagnostics selection, not a whole pack:

- `2D assets/Development Essentials/Checkerboard/checkerboard-transparent.png`
- `2D assets/Development Essentials/UV texture/uv-texture.png`
- `2D assets/Development Essentials/Normal map/default-normal.png`
- `2D assets/Development Essentials/1×1 Pixels/pixel-white.png`
- `2D assets/Prototype Textures/PNG/Dark/texture_01.png`
- `3D assets/Prototype Kit/Models/GLB format/shape-cube.glb`
- `3D assets/Prototype Kit/Models/GLB format/indicator-special-area.glb`
- `3D assets/Prototype Kit/Models/GLB format/indicator-special-arrow.glb`
- matching required textures/resources and the relevant supplied licence files

A later import task must verify actual GLB resource embedding, choose repository destination names, record hashes and any renames, and confirm the exact consumers. It must not import the compilation, unrelated pack content, or automatically add the selection to builds.

Weapon comparison, UI selection, audio selection, and child-specific imports should remain separate tasks because they require distinct ownership and validation decisions.

## Import provenance contract

Every later selected-file or separately redistributable-pack import must record:

- All-in-One bundle version 3.6.0.
- Exact pack name and Early Access status.
- Original bundle-relative source paths.
- Exact copied files and hashes when useful.
- Official source and supplied CC0 evidence.
- Every rename, removal, conversion, or generated derivative.
- Intended owning consumer.
- Required adjacent textures, metadata, and licence/readme files.
- Confirmation that only the selected files or individual pack—not the complete All-in-One compilation—is being redistributed.
- Confirmation that selection does not imply runtime integration; builds, manifests, cooking, and loaders require their own task.
