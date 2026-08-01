#!/usr/bin/env sh
set -eu

SCRIPT_DIR=$(CDPATH= cd -- "$(dirname -- "$0")" && pwd)
. "$SCRIPT_DIR/versions.env"

DEST="$SCRIPT_DIR/repos/SDL3-CS"
LICENSE_DIR="$SCRIPT_DIR/licenses/SDL3-CS"
NATIVE="$DEST/native/osx-arm64/libSDL3.dylib"

test -d "$DEST/.git"
test "$(git -C "$DEST" rev-parse HEAD)" = "$SDL3_CS_COMMIT"
test "$(git -C "$DEST" remote get-url origin)" = "$SDL3_CS_REPO"
git -C "$DEST" diff --check
diff -B "$DEST/LICENCE" "$LICENSE_DIR/LICENCE"
diff -B "$DEST/SDL3-CS/SDL-license-header.txt" "$LICENSE_DIR/SDL-license-header.txt"
test "$(shasum -a 256 "$NATIVE" | awk '{print $1}')" = "$SDL3_CS_OSX_ARM64_SHA256"
test "$(file -b "$NATIVE")" = "Mach-O 64-bit dynamically linked shared library arm64"

for patch in "$SCRIPT_DIR/patches/SDL3-CS"/*.patch; do
    [ -e "$patch" ] || continue
    git -C "$DEST" apply --check --reverse "$patch"
done
