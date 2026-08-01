---
id: EXPERIMENT-0003
title: Define minimal skeleton, skin, pose, and animation data
track: EXPERIMENT
milestone: M1
dependsOn:
- EXPERIMENT-0002
createdAt: 2026-08-01T05:34:31.8121280Z
modifiedAt: 2026-08-01T09:29:19.7540740Z
---

Define experiment-only data needed for the selected assets: joint hierarchy, local bind transforms, inverse-bind matrices, vertex joints and weights, sampled pose, clip channels, interpolation, and GPU palette. Keep headless simulation independent of rendering.

Acceptance criteria:
- Bootstrap a minimal coordinator .NET 10 solution with an experiment-only character data library and focused test project.
- Define immutable validated skeleton, skin, four-influence, pose, complete LINEAR TRS animation, explicit playback-mode, and CPU palette types.
- Use System.Numerics row-vector matrices with local `Scale * Rotation * Translation`, child global `local * parentGlobal`, and palette `inverseBind * posedGlobal`.
- Preserve the selected glTF right-handed, Y-up, metre-based identity Armature space without conversion, flattening, retargeting, or implicit root motion.
- Keep CPU matrices untransposed and leave sampling, hierarchy evaluation, asset loading, GPU ABI, SDL, rendering, and native work to their existing tasks.
- Add deterministic contract/invariant tests and durable coordinator wiki documentation.
- Add no runtime package, child-project, headless, third-party source, asset, or gitlink dependency.

## Notes

- 2026-08-01 09:29 UTC - Bootstrapped `ChronoFall.slnx` on pinned .NET SDK 10.0.301 with the BCL-only `ChronoFall.CharacterExperiment` library and an xUnit-only test project. Implemented immutable validated joint transforms, parent-first skeletons, inverse-bind skins, four-lane influences, local poses, complete LINEAR TRS clips, explicit Clamp/Loop playback modes, and CPU skinning palettes. The contract uses System.Numerics row-vector composition (`Scale * Rotation * Translation`, `local * parentGlobal`, `inverseBind * posedGlobal`) and preserves the selected glTF identity Armature space. Published `pm://project/prj_E7QP3LUocfY7k3PYM-EQOlqc/wiki/experiments/skeletal-data-contract` and linked it from the loader decision. Validation: `dotnet restore ChronoFall.slnx` passed; Debug and Release builds passed with zero warnings/errors; `dotnet test ChronoFall.slnx --no-restore --no-build -c Release` passed 10/10 tests; `dotnet format ChronoFall.slnx --verify-no-changes --no-restore`, `git diff --check`, PM MCP validation, and `pm doctor` passed. Package/reference inspection confirmed the runtime library has no package or project references; only the test project references xUnit/Test SDK and the experiment library. Family inspection returned all three projects available/readable/trusted with zero warnings. Royale and Starfall worktrees and both gitlinks were unchanged. No asset, loader, SimpleMesh, SDL/GPU, child, or native work was performed.