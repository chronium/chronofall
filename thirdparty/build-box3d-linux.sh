#!/usr/bin/env sh
set -eu

SCRIPT_DIR=$(CDPATH= cd -- "$(dirname -- "$0")" && pwd)
SOURCE_DIR="$SCRIPT_DIR/repos/box3d"
BUILD_DIR="$SCRIPT_DIR/build/box3d/linux-x64-release"
INSTALL_DIR="$SCRIPT_DIR/artifacts/box3d/linux-x64"
EXPECTED_SO="$INSTALL_DIR/lib/libbox3d.so"

[ "$(uname -s)" = "Linux" ] || { echo "This workflow requires Linux." >&2; exit 1; }
[ "$(uname -m)" = "x86_64" ] || { echo "This workflow supports Linux x64 only." >&2; exit 1; }

sh "$SCRIPT_DIR/fetch-box3d.sh"
cmake -S "$SOURCE_DIR" -B "$BUILD_DIR" -G "Unix Makefiles" \
    -DCMAKE_BUILD_TYPE=Release \
    -DBUILD_SHARED_LIBS=ON \
    -DBOX3D_SAMPLES=OFF \
    -DBOX3D_UNIT_TESTS=OFF \
    -DBOX3D_BENCHMARKS=OFF \
    -DBOX3D_DOCS=OFF \
    -DBOX3D_PROFILE=OFF \
    -DBOX3D_VALIDATE=OFF \
    -DCMAKE_INSTALL_PREFIX="$INSTALL_DIR"
cmake --build "$BUILD_DIR" --target install
test -f "$EXPECTED_SO"
printf 'Box3D Linux x64 shared library installed to %s\n' "$EXPECTED_SO"
