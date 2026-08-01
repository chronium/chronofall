#!/usr/bin/env sh
set -eu

SCRIPT_DIR=$(CDPATH= cd -- "$(dirname -- "$0")" && pwd)
. "$SCRIPT_DIR/versions.env"

DEST="$SCRIPT_DIR/repos/SimpleMesh"
LICENSE="$SCRIPT_DIR/licenses/SimpleMesh/LICENSE"

test -d "$DEST/.git"
test "$(git -C "$DEST" rev-parse HEAD)" = "$SIMPLEMESH_COMMIT"
test "$(git -C "$DEST" remote get-url origin)" = "$SIMPLEMESH_REPO"
git -C "$DEST" diff --check
diff -B "$DEST/LICENSE" "$LICENSE"

for patch in "$SCRIPT_DIR/patches/SimpleMesh"/*.patch; do
    [ -e "$patch" ] || continue
    git -C "$DEST" apply --check --reverse "$patch"
done
