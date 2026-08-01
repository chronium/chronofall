---
id: SHARED-0001
title: Promote proven character presentation contracts into shared modules
track: SHARED
milestone: M2
dependsOn:
- EXPERIMENT-0011
createdAt: 2026-08-01T05:34:56.3230610Z
modifiedAt: 2026-08-01T17:15:52.6966950Z
---

Promote the M1-proven character presentation contracts into focused parent-owned modules. The shared core remains BCL-only and the SDL GPU layer records work into a caller-owned native frame lifecycle. Shared modules remain independent of Royale and Starfall and do not become a general engine.

## Acceptance criteria

- `ChronoFall.CharacterPresentation` owns immutable skeleton, skin, mesh, animation, pose, and palette contracts plus deterministic sampling and evaluation.
- The core project has no SDL, GPU, SimpleMesh, editor, server, child-game, or simulation dependency.
- `ChronoFall.CharacterPresentation.SdlGpu` owns the reviewed four-influence GPU ABI, shader contract, immutable mesh upload, per-instance palette resources, and draw recording.
- Callers own SDL initialization, device/window/target lifetime, command buffers, render passes, submission, cameras, and gameplay-to-animation mapping.
- CPU matrices use the established System.Numerics row-vector convention; the SDL GPU layer owns the single transpose at shader upload.
- The experiment SimpleMesh adapter remains provisional and maps into the promoted core without becoming a shared loader dependency.
- The existing experiment harness consumes the shared modules and retains deterministic managed tests, native Metal execution, skeleton diagnostics, controls, captures, and recorded fingerprints.
- No cooking format, material system, animated bounds, retargeting, root motion, blending, equipment, IK, scene framework, child integration, package publication, or gitlink update is introduced.
- Architecture documentation records resource ownership, authority boundaries, internal ABI decisions, and deferred child distribution.
- Coordinator PM validation, repository diff checks, and explicit owner visual confirmation pass before completion.

## Notes

- 2026-08-01 17:12 UTC - Implemented the approved promotion without child or gitlink changes. `ChronoFall.CharacterPresentation` now owns the BCL-only immutable skeletal mesh, skeleton, skin, pose, animation sampling/evaluation and CPU palette contracts. `ChronoFall.CharacterPresentation.SdlGpu` owns the reviewed internal 48-byte vertex ABI, shader/pipeline resources, caller-command-buffer mesh and palette uploads, the single palette transpose, and section or whole-mesh draws against caller-owned render passes. The SimpleMesh loader, SDL window/device host, camera, skeleton overlay, controls, targets, readback and capture suite remain provisional experiment consumers.

  Validation on native macOS ARM64: scoped `dotnet format --verify-no-changes` passed for all ChronoFall-owned projects (the whole-solution formatter continues to report pre-existing upstream checkout style issues); Debug and Release solution builds passed with 0 warnings and 0 errors; all 59 managed tests passed in Debug and Release; the opt-in SDL GPU integration test passed; two independently generated MSL capture suites compared byte-for-byte identical. Retained fingerprints: bind `408d3a4c16278bbc`, translated palette `4fd2e63aea97f7a3`, skeleton `c6ad39a45245afed`, animation start/loop `68ba446d672887a0`, 0.5 s `a2b427aea339d460`, 1.0 s `85c5d42b4eac399d`. Retained PPM SHA-256: bind `68cc300230a74917925d7785a233091f0b08eb7580224e0aebb8068571a0f18a`, start/loop `1268476a5f5ff930e521e1e5401ba4cc043743e2e9c2bac768d558a3034138bc`, 0.5 s `3cc9b0e6278c51a4616922fcc0ffa9ed6eb35fb8fdea807cc3a3808502a49e37`, 1.0 s `9dc755418dc3372ab562afb845440865ff26b7c400d3c5a0fcfb510ed37c2f16`. Owner visual confirmation remains required before completion.
- 2026-08-01 17:15 UTC - Owner visually validated the promoted shared renderer on native macOS ARM64 Metal on 2026-08-01. The retained browser rendered correctly while switching to `Sword_Attack`, toggling the 65-joint skeleton, pausing and resuming, restarting, and stepping through multiple sample times; the harness closed cleanly with the expected bind, probe, skeleton and animation fingerprints. No new history artifact was requested because this is a contract-preserving promotion of the existing M1 checkpoint rather than a new visual milestone.