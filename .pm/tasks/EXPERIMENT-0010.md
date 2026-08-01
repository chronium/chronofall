---
id: EXPERIMENT-0010
title: Run native macOS ARM64 visual validation
track: EXPERIMENT
milestone: M1
dependsOn:
- EXPERIMENT-0009
createdAt: 2026-08-01T05:34:33.4435780Z
modifiedAt: 2026-08-01T16:29:40.2486960Z
---

Run the proof natively on macOS ARM64, review deterministic captures, and obtain explicit owner confirmation for deformation, orientation, scale, and animation appearance. Automated screenshots support but do not replace owner validation.

## Notes

- 2026-08-01 16:29 UTC - Completed the native macOS ARM64 Metal validation on macOS 26.5.2 with .NET SDK 10.0.301. `dotnet restore ChronoFall.slnx -m:1` was current; `dotnet build ChronoFall.slnx -m:1 --no-restore` passed with zero warnings and errors; all 57 solution tests passed; and the focused `CHRONOFALL_GPU_TESTS=1` SDL GPU run passed all 22 tests. A fresh ignored `artifacts/EXPERIMENT-0010/validation` capture suite retained bind `408d3a4c16278bbc`, palette probe `4fd2e63aea97f7a3`, skeleton `c6ad39a45245afed`, animation sample `a2b427aea339d460`, and was byte-identical to `EXPERIMENT-0009/run-a`, including the exact loop-boundary/start match. The owner exercised `Idle_Loop`, `Walk_Loop`, `Sword_Attack`, pause, and the animated skeleton overlay in the visible native browser and explicitly confirmed: “everything still works correctly!” This approves deformation, upright orientation, scale/framing, animation appearance, controls, and overlay. No additional history artifact was retained because the existing contact sheet already captures this checkpoint. No source, asset, child repository, or gitlink changed.