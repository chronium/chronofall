# Third-Party Dependencies

This directory contains coordinator-owned dependency-management files for demonstrated parent consumers. It does not contain committed upstream clones or generated outputs.

## SimpleMesh

| Property | Value |
| --- | --- |
| Official source | `https://github.com/CallumDev/SimpleMesh` |
| Pinned revision | `9f46341e35fa5876fbea7b96bd021bc3abd7842d` |
| License | Apache License 2.0 |
| Purpose | Provisional importer foundation for the M1 skeletal-character experiment |

The upstream license is preserved at `licenses/SimpleMesh/LICENSE`. ChronoFall applies the ordered patches under `patches/SimpleMesh/`; patched files carry a modification notice. This pin and adapter do not promote SimpleMesh into a permanent shared-engine dependency.

Fetch and patch the ignored source checkout:

```sh
sh thirdparty/fetch-simplemesh.sh
sh thirdparty/verify-simplemesh.sh
```

The resulting source is placed at `thirdparty/repos/SimpleMesh`. The fetch script resets and cleans only that explicitly ignored dependency checkout before applying the committed patch set. Parent source never references either child's dependency directory.

## SDL3-CS

| Property | Value |
| --- | --- |
| Official source | `https://github.com/ppy/SDL3-CS` |
| Pinned revision | `a0a5276a874c0c48db705696ab7e2adc8b5db0a1` |
| Binding license | MIT |
| Bundled SDL license | zlib license notice |
| Purpose | Provisional native SDL window and GPU binding for the M1 bind-pose experiment |

The binding license and generated SDL notice are preserved under `licenses/SDL3-CS/`. The macOS ARM64 native library is verified as an ARM64 Mach-O with SHA-256 `35797abd1dc9e130f8e7ca8aeee33d68f8eecbf0af479184913297aaad4760ca`. The ordered build-only patch under `patches/SDL3-CS/` selects the upstream binding's existing desktop-only switch so coordinator builds do not require Android or WebAssembly workloads; it does not modify bindings or native code.

Fetch and verify the ignored source checkout:

```sh
sh thirdparty/fetch-sdl3-cs.sh
sh thirdparty/verify-sdl3-cs.sh
```

The resulting source is placed at `thirdparty/repos/SDL3-CS`. ChronoFall pins SDL3-CS independently for a demonstrated coordinator consumer and never references Royale's dependency checkout. This experiment pin does not itself establish a permanent shared native-loading or SDL abstraction.

## StbImageWriteSharp

| Property | Value |
| --- | --- |
| Official source | `https://github.com/StbSharp/StbImageWriteSharp` |
| Pinned NuGet version | `1.16.7` |
| License | Public Domain, as stated by the official project README |
| Purpose | PNG encoding for the bounded client/tooling screenshot contract |

The package is centrally pinned and referenced only by `ChronoFall.CharacterPresentation.SdlGpu`. SDL3-CS remains compiled from its checked-out source; the PNG dependency does not enter the BCL-only character data or cooking projects. The inspected package and licence/provenance evidence are recorded at `licenses/StbImageWriteSharp/PROVENANCE.md`.

## Box3D

| Property | Value |
| --- | --- |
| Official source | `https://github.com/erincatto/box3d` |
| Pinned revision | `3fc20f5b453ba9e14cdf54ecafa87a2a4bcdf53c` |
| License | MIT |
| Purpose | Headless world, body, box/capsule, filtering and bounded mover-query runtime for family simulations |

The upstream licence is preserved at `licenses/Box3D/LICENSE`. No patch is required at this revision; `patches/Box3D/README.md` records that evidence and remains the only approved patch location. Fetching, native build products and installed artifacts stay ignored:

```sh
sh thirdparty/fetch-box3d.sh
sh thirdparty/verify-box3d.sh
sh thirdparty/build-box3d-macos.sh
# Run build-box3d-linux.sh from Linux x64.
```

Release native artifacts are installed at `thirdparty/artifacts/box3d/<rid>/lib/` and copied by `ChronoFall.Box3D` to `runtimes/<rid>/native/`. Supported development/server targets are macOS ARM64 (`libbox3d.dylib`) and Linux x64 (`libbox3d.so`); unsupported or missing runtime libraries fail explicitly. Windows and package/feed distribution remain deferred.

The managed contract deliberately excludes debug rendering, joints, meshes, height fields, general overlap/raycast APIs, character controllers, map/collision cooking and game-specific policy. Native query traversal order is not a gameplay ordering contract, and cross-platform bitwise determinism is not promised.
