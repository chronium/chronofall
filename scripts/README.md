# Coordinator Scripts

This directory contains small coordinator-owned documentation and workflow helpers. Scripts here must not become runtime, asset-cooking, or child-repository dependencies.

## Character Presentation Client Cook

`cook-character-presentation-for-client.sh` resolves one declared child from its stable PM project ID, verifies the reciprocal linked-project and Git-submodule identity, restores and builds the focused cooker project, and stages the selected Quaternius UAL1 cook plus portable provenance and CC0 evidence into the child's ignored `artifacts/chronofall/character-presentation/client/` tree. No separate restore step is required.

From the coordinator root:

```sh
scripts/cook-character-presentation-for-client.sh \
  --project-id prj_pkIpzx0fzFD4URjvqBuYrGZF
```

Aliases, arbitrary destinations, non-ignored trees, tracked content, symlink escapes, and unexpected existing files are rejected. The output is client-only generated content; it is not a runtime manifest or a committed package.

## Contact-Sheet Compositor

`create-contact-sheet.swift` is a macOS AppKit utility for composing equally sized captures into the labeled 2x PNG sheets used by `docs/project-history/`. It preserves each source image's exact pixel dimensions and framing without cropping or scaling, adds a 48-pixel label strip, and supports an arbitrary number of `--item <path> <label>` pairs. AppKit decoding and redraw do not promise byte-identical decoded RGB samples.

Run `scripts/create-contact-sheet.swift --help` for the complete syntax. For example:

```sh
scripts/create-contact-sheet.swift \
  --output /tmp/contact-sheet.png \
  --columns 3 \
  --item /tmp/idle.ppm "Idle" \
  --item /tmp/walk.ppm "Walk"
```

The source captures remain generated evidence. Commit a composed sheet only after the owner approves it through the visual-checkpoint workflow documented in `AGENTS.md` and `docs/project-history/README.md`.
