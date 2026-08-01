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
