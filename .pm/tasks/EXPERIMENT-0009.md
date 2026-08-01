---
id: EXPERIMENT-0009
title: Produce deterministic multi-timestamp captures
track: EXPERIMENT
milestone: M1
dependsOn:
- EXPERIMENT-0008
- EXPERIMENT-0013
createdAt: 2026-08-01T05:34:33.2084090Z
modifiedAt: 2026-08-01T16:01:37.3619200Z
---

Use the supported offscreen/readback path to capture deterministic visual evidence at multiple fixed timestamps for the selected bind pose and animation.

## Implemented scope

- Added `--capture-suite <directory>` while preserving `--capture`, `--skeleton-capture`, `--animation-capture`, and `--visible`.
- The hidden native harness writes exactly five 512 by 512 P6 PPM files:
  - `bind-pose.ppm`
  - `animation-0000ms.ppm`
  - `animation-0500ms.ppm`
  - `animation-1000ms.ppm`
  - `animation-loop-boundary.ppm`
- The selected `Walk_Loop` evidence times are 0.000, 0.500, 1.000, and its exact 1.333333-second loop boundary.
- The suite creates its target directory and overwrites only those five known paths; it does not clean the directory or delete unrelated artifacts.
- The existing SDL GPU rendering, palette upload, offscreen target, readback, analysis, and PPM writer remain the shared path.
- Existing individual capture behavior and visible browsing remain unchanged.

## Acceptance and evidence

- Bind-pose fingerprint: `408d3a4c16278bbc`.
- Animation fingerprints:
  - 0.000: `68ba446d672887a0`
  - 0.500: `a2b427aea339d460`
  - 1.000: `85c5d42b4eac399d`
  - loop boundary: `68ba446d672887a0`
- Each generated PPM is 786,447 bytes with the expected `P6\n512 512\n255\n` header.
- Two independent native Metal runs under ignored `artifacts/EXPERIMENT-0009/run-a/` and `run-b/` compared byte-for-byte with no differences.
- SHA-256 values:
  - bind pose: `68cc300230a74917925d7785a233091f0b08eb7580224e0aebb8068571a0f18a`
  - start and loop boundary: `1268476a5f5ff930e521e1e5401ba4cc043743e2e9c2bac768d558a3034138bc`
  - 0.500: `3cc9b0e6278c51a4616922fcc0ffa9ed6eb35fb8fdea807cc3a3808502a49e37`
  - 1.000: `9dc755418dc3372ab562afb845440865ff26b7c400d3c5a0fcfb510ed37c2f16`
- Review-only PNGs under ignored `artifacts/EXPERIMENT-0009/review/` showed full framing, correct orientation, distinct walk phases, no visible deformation discontinuity, and an exact start/loop match.
- No generated PPM or PNG is committed.
- The coordinator build passes with zero warnings; all 57 automated tests pass; the focused native macOS ARM64 Metal integration passes and validates the exact five-file contract.
- Royale and Starfall source, PM data, commits, and gitlinks remain unchanged.
- Explicit owner visual validation remains owned by downstream `EXPERIMENT-0010`.

## Validation commands

- `dotnet build ChronoFall.slnx -m:1 --no-restore`
- `dotnet test ChronoFall.slnx -m:1 --no-restore --no-build`
- `CHRONOFALL_GPU_TESTS=1 dotnet test tests/ChronoFall.CharacterExperiment.SdlGpu.Tests/ChronoFall.CharacterExperiment.SdlGpu.Tests.csproj -m:1 --no-restore --no-build --filter FullyQualifiedName~SdlGpuIntegrationTests`
- Two direct `--capture-suite` native runs followed by recursive byte comparison and SHA-256 inventory
- PNG conversion and agent visual inspection
- PM doctor/validation, family-warning checks, repository diff checks, and submodule status

## Exclusions preserved

No committed generated captures, owner visual sign-off, animation blending, root motion, retargeting, IK, general capture framework, new image dependency, shared-engine extraction, child changes, or gitlink updates.