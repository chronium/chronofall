#!/usr/bin/env sh
set -eu

SCRIPT_DIR=$(CDPATH= cd -- "$(dirname -- "$0")" && pwd)
. "$SCRIPT_DIR/versions.env"

DEST="$SCRIPT_DIR/repos/ImGui.Net"
LICENSE_DIR="$SCRIPT_DIR/licenses/ImGui.Net"

test -d "$DEST/.git"
test "$(git -C "$DEST" rev-parse HEAD)" = "$IMGUI_NET_COMMIT"
test "$(git -C "$DEST" remote get-url origin)" = "$IMGUI_NET_REPO"
test "$(git -C "$DEST/NativeLibraries/cimgui" rev-parse HEAD)" = "$CIMGUI_COMMIT"
test "$(git -C "$DEST/NativeLibraries/cimgui/imgui" rev-parse HEAD)" = "$IMGUI_COMMIT"
test "$(git -C "$DEST/NativeLibraries/cimguizmo" rev-parse HEAD)" = "$CIMGUIZMO_COMMIT"
test "$(git -C "$DEST/NativeLibraries/cimguizmo/ImGuizmo" rev-parse HEAD)" = "$IMGUIZMO_COMMIT"

git -C "$DEST" diff --check
git -C "$DEST/NativeLibraries/cimgui" diff --check
git -C "$DEST/NativeLibraries/cimgui/imgui" diff --check
git -C "$DEST/NativeLibraries/cimguizmo" diff --check
git -C "$DEST/NativeLibraries/cimguizmo/ImGuizmo" diff --check

diff -B "$DEST/LICENSE" "$LICENSE_DIR/ImGui.Net-LICENSE"
diff -B "$DEST/NativeLibraries/cimgui/LICENSE" "$LICENSE_DIR/cimgui-LICENSE"
diff -B "$DEST/NativeLibraries/cimgui/imgui/LICENSE.txt" "$LICENSE_DIR/Dear-ImGui-LICENSE.txt"
diff -B "$DEST/NativeLibraries/cimguizmo/LICENSE" "$LICENSE_DIR/cimguizmo-LICENSE"
diff -B "$DEST/NativeLibraries/cimguizmo/ImGuizmo/LICENSE" "$LICENSE_DIR/ImGuizmo-LICENSE"

for patch in "$SCRIPT_DIR/patches/ImGui.Net"/*.patch; do
    [ -e "$patch" ] || continue
    git -C "$DEST" apply --check --reverse "$patch"
done

test ! -d "$DEST/NativeLibraries/cimplot/.git"
test ! -d "$DEST/NativeLibraries/cimnodes/.git"
