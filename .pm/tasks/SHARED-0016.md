---
id: SHARED-0016
title: Establish family source consumption for shared presentation
track: SHARED
milestone: M2
dependsOn:
- SHARED-0001
- SHARED-0002
createdAt: 2026-08-02T07:50:10.8973880Z
modifiedAt: 2026-08-02T10:31:15.5596650Z
---

Establish the canonical coordinator-family source-consumption boundary for shared character presentation. Child clients may reference only the approved coordinator-owned ChronoFall.CharacterPresentation, ChronoFall.CharacterPresentation.Cooking, and ChronoFall.CharacterPresentation.SdlGpu projects through one ChronoFallFamilyRoot MSBuild property. The SDL GPU module must continue compiling the checked-out, coordinator-pinned SDL3-CS project from source.

Provide an explicit coordinator workflow that cooks the selected Quaternius UAL1 client asset and stages it into a linked child's ignored generated-content tree. Resolve the destination from a stable project ID and committed linked-project path hint; verify the child's stable ID, reciprocal parent declaration, gitlink, canonical checkout path, ignored output boundary, absence of tracked files, and absence of symlink escapes before writing. Preserve portable deterministic provenance and CC0 evidence.

Acceptance criteria:
- A child-like smoke consumer builds the three approved shared projects from source through ChronoFallFamilyRoot.
- SDL3-CS remains a checked-out source ProjectReference; no NuGet package, feed, package version, buildTransitive target, source mapping, or content-package contract is introduced.
- The client staging command accepts a stable project ID, refuses aliases/arbitrary destinations and unsafe output trees, and writes only the selected .cfskel, deterministic provenance, License.txt, and README.txt beneath artifacts/chronofall/character-presentation/client/.
- Two cooks are byte-identical and retain the established cooked SHA-256.
- No raw source GLB, server content, runtime manifest, child source/PM change, or gitlink change is produced.
- Coordinator policy and wiki distinguish repository ownership from the canonical family-checkout build environment and defer standalone package distribution until integration or release evidence requires it.
- Coordinator PM, builds/tests, linked-family warnings, diffs, submodules, and child cleanliness are validated.

This task owns coordinator source, workflow, tests, PM, and documentation only. Starfall integration remains CLIENT-0006; Royale integration remains child-owned. Completion ends the cycle.

## Notes

- 2026-08-02 10:31 UTC - Implemented the approved family source-consumption boundary.

  - Added conditional coordinator ChronoFallFamilyRoot and a child-like smoke consumer referencing exactly ChronoFall.CharacterPresentation, ChronoFall.CharacterPresentation.Cooking, and ChronoFall.CharacterPresentation.SdlGpu through that property.
  - Preserved the direct checked-out SDL3-CS ProjectReference; thirdparty/verify-sdl3-cs.sh passed and no SDL/presentation package or feed configuration was added.
  - Added stable-ID scripts/cook-character-presentation-for-client.sh. It verifies the committed path hint, reciprocal PM IDs, .gitmodules/gitlink, canonical checkout, ignored/untracked destination, symlinks, and known output set before staging.
  - Added deterministic cooker provenance JSON and copied only the cooked asset plus Quaternius License.txt/README.txt. No raw GLB, runtime manifest, or server output is produced.
  - Created development/family-source-consumption and reconciled shared-engine, shared-character, and skeletal-cooking wiki contracts plus coordinator policy/skills.

  Validation:
  - Debug: 148 managed tests passed (10 cooker, 29 experiment SDL GPU, 10 SimpleMesh adapter, 9 cooking format, 6 shared SDL GPU, 84 shared core/policy).
  - Release: the same 148 managed tests passed.
  - Debug and Release FamilySourceConsumer builds succeeded with zero warnings/errors while compiling SDL3-CS and its source generator from the checked-out source; both runs read the staged asset successfully (65 joints, 3 clips).
  - Two stable-ID Starfall stages produced a 1,278,301-byte .cfskel with SHA-256 37d2ecd2c614a4cc74fe359906c84408432100f0338b86d7ce4f4dddb6b585d3 and deterministic provenance SHA-256 bbe46b17fa0882e3ba5cdc46093a67df3224b6d5892aa5463ee6d386fce9d8c9.
  - Alias, unknown stable ID, and unexpected-output-file probes failed closed before cooking/writing.
  - Starfall contains exactly four ignored generated files under artifacts/chronofall/character-presentation/client/. Both child worktrees remain clean and gitlinks remain Royale 174fa32600887da2093bcf7cbc9ebf89dc92990f and Starfall 88d08591d626caa03ef0ad7d372e7b80d8b110ca.
  - Coordinator, Starfall, and Royale pm doctor passed. MCP validation passed; family inspection returned all three available/readable/write-trusted with zero warnings even after child-context startup.
  - git diff --check and formatting verification limited to every changed C# file passed. Full-solution dotnet format remains noisy only for the existing ignored SimpleMesh/SDL3-CS source checkouts; none was modified.
  - No native visual validation or project-history artifact was required because rendering behavior and output were unchanged.