# Coordinator Scripts

This directory contains small coordinator-owned documentation and workflow helpers. Scripts here must not become runtime, asset-cooking, or child-repository dependencies.

## Contact-Sheet Compositor

`create-contact-sheet.swift` is a macOS AppKit utility for composing equally sized captures into the labeled 2x PNG sheets used by `docs/project-history/`. It preserves source framing, adds a 48-point label strip, and supports an arbitrary number of `--item <path> <label>` pairs.

Run `scripts/create-contact-sheet.swift --help` for the complete syntax. For example:

```sh
scripts/create-contact-sheet.swift \
  --output /tmp/contact-sheet.png \
  --columns 3 \
  --item /tmp/idle.ppm "Idle" \
  --item /tmp/walk.ppm "Walk"
```

The source captures remain generated evidence. Commit a composed sheet only after the owner approves it through the visual-checkpoint workflow documented in `AGENTS.md` and `docs/project-history/README.md`.
