---
name: chronofall-character-rendering-animation
description: Plan, implement, or review ChronoFall skeletal character experiments and shared presentation. Use for supplied humanoid rigs, skin weights, bind poses, animation sampling, GPU skinning, skeleton debug rendering, clips, modular armour, sockets, attachments, blending, IK, aim offsets, deterministic captures, or native visual validation.
---

# Character Rendering And Animation

## Start From Evidence

Read the selected coordinator task, Quaternius inventory/wiki, actual asset files, Royale architecture/wiki, and relevant Royale rendering/asset-pipeline source. Preserve root-motion versus in-place distinctions and do not assume skeleton compatibility.

Inspect exact joints, parents, bind transforms, inverse-bind matrices, joint/weight accessors and influence count, animation targets/interpolation/timing, coordinate system, scale, and external resources before choosing inputs or a loader.

## Keep M1 Narrow

The proof needs one humanoid, one skeleton, one idle, one locomotion clip, and one compatible attack only if evidence supports it. It must demonstrate correct GPU skinning, deterministic sampling, skeleton debug visualization, multi-timestamp captures, native macOS ARM64 execution, and owner validation.

Exclude modular armour, blending, root motion, retargeting, IK, a general animation graph, and production engine architecture from M1.

## Gate Loader Decisions

Compare Royale's current SimpleMesh integration with required skin and animation data. Document exact gaps and present the smallest viable options during Plan mode. Do not silently add a large importer, native dependency, generic asset framework, permanent custom format, or retargeter.

## Preserve Authority And Dependencies

Keep pose evaluation and GPU data in client/presentation modules. Gameplay and protocol state signal actions; animation never decides authoritative events. Headless projects must not reference rendering or animation presentation.

## Promote Deliberately

Promote only validated data and GPU contracts. Add modular armour, body hiding, variants, sockets, attachments, blending, masks, grips, effect points, IK, debug tools, and previews as separate dependency-aware tasks.

Validate transforms numerically before visual checks, then use native captures and request explicit owner confirmation.
