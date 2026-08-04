#!/usr/bin/env sh
set -eu

SCRIPT_DIR=$(CDPATH= cd -- "$(dirname -- "$0")" && pwd)
. "$SCRIPT_DIR/versions.env"

DEST="$SCRIPT_DIR/repos/box3d"
LICENSE="$SCRIPT_DIR/licenses/Box3D/LICENSE"

test -d "$DEST/.git"
test "$(git -C "$DEST" rev-parse HEAD)" = "$BOX3D_COMMIT"
test "$(git -C "$DEST" remote get-url origin)" = "$BOX3D_REPO"
git -C "$DEST" diff --check
diff -B "$DEST/LICENSE" "$LICENSE"

for patch in "$SCRIPT_DIR/patches/Box3D"/*.patch; do
    [ -e "$patch" ] || continue
    git -C "$DEST" apply --check --reverse "$patch"
done
