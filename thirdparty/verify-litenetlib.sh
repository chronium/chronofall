#!/usr/bin/env sh
set -eu

SCRIPT_DIR=$(CDPATH= cd -- "$(dirname -- "$0")" && pwd)
. "$SCRIPT_DIR/versions.env"

DEST="$SCRIPT_DIR/repos/LiteNetLib"
LICENSE="$SCRIPT_DIR/licenses/LiteNetLib/LICENSE.txt"

test -d "$DEST/.git"
test "$(git -C "$DEST" rev-parse HEAD)" = "$LITENETLIB_COMMIT"
test "$(git -C "$DEST" remote get-url origin)" = "$LITENETLIB_REPO"
git -C "$DEST" diff --check
tr -d '\r' < "$DEST/LICENSE.txt" | cmp - "$LICENSE"

for patch in "$SCRIPT_DIR/patches/LiteNetLib"/*.patch; do
    [ -e "$patch" ] || continue
    git -C "$DEST" apply --check --reverse "$patch"
done
