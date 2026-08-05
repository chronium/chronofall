#!/usr/bin/env sh
set -eu

SCRIPT_DIR=$(CDPATH= cd -- "$(dirname -- "$0")" && pwd)
SOURCE_DIR="$SCRIPT_DIR/repos/ImGui.Net/NativeLibraries"
SDL_SOURCE_DIR="$SCRIPT_DIR/repos/SDL3-CS/External/SDL"
BUILD_DIR="$SCRIPT_DIR/build/imgui/osx-arm64"
INSTALL_DIR="$SCRIPT_DIR/artifacts/imgui/osx-arm64"
EXPECTED_DYLIB="$INSTALL_DIR/lib/libchronofall_imgui.dylib"

if [ "$(uname -s)" != "Darwin" ]; then
    echo "build-imgui-macos.sh requires macOS (Darwin)." >&2
    exit 1
fi

if [ "$(uname -m)" != "arm64" ]; then
    echo "build-imgui-macos.sh currently builds only macOS ARM64 artifacts." >&2
    exit 1
fi

sh "$SCRIPT_DIR/fetch-sdl3-cs.sh"
sh "$SCRIPT_DIR/fetch-imgui-net.sh"

mkdir -p "$BUILD_DIR" "$INSTALL_DIR/lib"

CIMGUI_DIR="$SOURCE_DIR/cimgui"
IMGUI_DIR="$CIMGUI_DIR/imgui"
CIMGUIZMO_DIR="$SOURCE_DIR/cimguizmo"
IMGUIZMO_DIR="$CIMGUIZMO_DIR/ImGuizmo"

clang++ \
    -std=c++17 \
    -arch arm64 \
    -dynamiclib \
    -undefined dynamic_lookup \
    -fvisibility=hidden \
    -install_name "@rpath/libchronofall_imgui.dylib" \
    -DIMGUI_DISABLE_OBSOLETE_FUNCTIONS=1 \
    -DCIMGUI_VARGS0 \
    -I"$SDL_SOURCE_DIR/include" \
    -I"$CIMGUI_DIR" \
    -I"$IMGUI_DIR" \
    -I"$IMGUI_DIR/backends" \
    -I"$CIMGUIZMO_DIR" \
    -I"$IMGUIZMO_DIR" \
    "$CIMGUI_DIR/cimgui.cpp" \
    "$IMGUI_DIR/imgui.cpp" \
    "$IMGUI_DIR/imgui_draw.cpp" \
    "$IMGUI_DIR/imgui_widgets.cpp" \
    "$IMGUI_DIR/imgui_tables.cpp" \
    "$IMGUI_DIR/backends/imgui_impl_sdl3.cpp" \
    "$IMGUI_DIR/backends/imgui_impl_sdlgpu3.cpp" \
    "$CIMGUIZMO_DIR/cimguizmo.cpp" \
    "$IMGUIZMO_DIR/ImGuizmo.cpp" \
    "$SCRIPT_DIR/chronofall_imgui/chronofall_imgui.cpp" \
    -o "$EXPECTED_DYLIB"

if [ ! -f "$EXPECTED_DYLIB" ]; then
    echo "Expected ImGui shared library was not produced: $EXPECTED_DYLIB" >&2
    exit 1
fi

test "$(file -b "$EXPECTED_DYLIB")" = "Mach-O 64-bit dynamically linked shared library arm64"
printf 'ChronoFall ImGui macOS ARM64 shared library installed to %s\n' "$EXPECTED_DYLIB"
