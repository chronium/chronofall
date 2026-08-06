---
title: Native Blender Asset Evaluation Workflow
createdAt: 2026-08-06T17:51:38.8854580Z
modifiedAt: 2026-08-06T17:51:38.8854580Z
---

## Purpose

ChronoFall uses Blender as an owner-visible native review surface when a focused asset task needs confirmation of scale, silhouette, orientation, pivot implications or relative proportions. This workflow is evidence gathering, not asset authoring.

The first proven use was `pm://project/prj_E7QP3LUocfY7k3PYM-EQOlqc/task/ASSET-0006`: exact Quaternius `Bow_Wooden` and `Arrow` source meshes were shown at `0.25` metres per source unit beside a 1.8 metre reference. The owner confirmed that the approximately 1.36 metre bow and 0.68 metre arrow were credible.

## Availability gate

Before planning Blender interaction, probe the connected Blender MCP with a read-only scene or blend-file summary.

If the probe shows that Blender is closed, its MCP add-on is not connected or the session cannot be reached:

1. tell the owner that Blender is needed;
2. ask the owner to open Blender and enable its MCP add-on;
3. stop only the native-review path until the owner confirms it is ready.

Agents do not install, launch, reconfigure or replace Blender autonomously. Repository inspection, hashing and deterministic cooker tests may continue when they do not depend on the visual decision.

## Preserve the current session

Inspect before changing anything:

- current blend-file path;
- dirty/unsaved state;
- active scene and mode;
- scene unit system;
- object and collection summary;
- source objects relevant to the review.

Never assume the open file is disposable. Do not reset Blender, load factory settings, reload a source file, clear the scene or discard existing objects to make room for a review.

Prefer a new, clearly named temporary scene or collection:

```text
<TASK-ID> Scale Review (UNSAVED)
```

Append an exact source object or duplicate it into the temporary collection. Keep existing source scenes and source datablocks intact. If safe cleanup cannot be proven, leave the temporary scene open and remind the owner not to save the supplied source file.

## Construct the smallest useful comparison

Use only task-selected source files. A similarly named or visually close substitute is not valid evidence.

The review scene should normally contain:

- metric scene units;
- the exact approved uniform scale;
- the exact source objects being compared;
- a neutral ground plane;
- a known-height or known-size reference;
- a small ruler or dimensional ticks where useful;
- concise object and measurement labels;
- a neutral, easily inspected viewport framing.

Compute and report transformed world-space bounds in metres. Keep the durable source orientation and pivot unchanged unless the approved task explicitly owns a conversion.

Source variants may use different authoring axes. For example, a supplied Blend and the selected OBJ cook can express the same physical object with different axis conventions. Record that distinction; do not silently convert one and then claim its axes or pivot describe the other.

Diagnostic materials, labels, camera framing and reference objects exist only to make the decision legible. They are not runtime assets and are not cooked or committed.

## Ask one explicit question

The agent must state the exact owner decision being requested. Examples include:

- Does this scale look credible beside the reference character?
- Is the pivot suitable for the next socket experiment?
- Is the silhouette readable enough for a temporary monster role?
- Do two independently supplied skeletons appear plausibly aligned before structural comparison?

State the evidence numerically and state its limits. A scale comparison does not prove:

- a socket or local attachment transform;
- hand grip, string contact or IK;
- skeleton compatibility;
- gameplay collision or authority;
- animation quality;
- runtime material correctness;
- renderer integration;
- final-art suitability.

Structural questions still require file inspection and deterministic tests. Native appearance never overrides authoritative cooker or source evidence.

## Saving, exporting and provenance

The review scene remains unsaved unless a separately approved task explicitly owns a reusable diagnostic fixture.

Never:

- save over a supplied `.blend`;
- export replacement OBJ, FBX, GLB or Blend source;
- mutate the supplied source mesh;
- perform an unapproved conversion;
- process a complete pack;
- persist an owner's absolute machine path;
- treat a screenshot as licence or provenance evidence.

After owner confirmation, the owning task and wiki record:

- exact source identity and pack-relative path;
- source and licence hashes;
- scale and transforms used;
- measured dimensions or bounds;
- diagnostic references;
- owner conclusion;
- what the review did not establish.

## Visual-checkpoint boundary

A useful Blender comparison is not automatically a project-history checkpoint. Ask separately whether to preserve, revise or skip a screenshot. Raw captures remain ignored. Permanent retention follows the coordinator visual-checkpoint workflow and its own owner approval.

## ASSET-0006 example

The ASSET-0006 scene preserved the already-open Quaternius Arrow source scene and added a separate `ASSET-0006 Scale Review (UNSAVED)` scene. It used the exact `Bow_Wooden.blend` and `Arrow.blend` meshes, uniform scale `0.25`, a neutral ground plane, ruler and 1.8 metre reference.

The comparison measured:

- bow dominant extent: approximately `1.359151` metres;
- arrow dominant extent: approximately `0.683456` metres.

The owner confirmed the sizes and proportions. The review deliberately did not claim an equipped bow, hand socket, grip, nocking, release, projectile or gameplay proof. Exact cooked OBJ identities, hashes and bounds remain recorded at `pm://project/prj_E7QP3LUocfY7k3PYM-EQOlqc/wiki/assets/quaternius-medieval-weapons-bow-arrow-cook`.