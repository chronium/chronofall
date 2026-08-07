#!/bin/sh
set -eu

fail()
{
    printf '%s\n' "CHARACTER_CLIENT_STAGE_FAILURE: $*" >&2
    exit 1
}

project_id=
ual2_source_root=
while [ "$#" -gt 0 ]; do
    option=$1
    shift
    [ "$#" -gt 0 ] || fail "$option requires a value"
    value=$1
    shift
    case "$value" in
        --*) fail "$option requires a value" ;;
    esac
    case "$option" in
        --project-id)
            [ -z "$project_id" ] || fail "--project-id may be supplied only once"
            project_id=$value
            ;;
        --ual2-source-root)
            [ -z "$ual2_source_root" ] || fail "--ual2-source-root may be supplied only once"
            ual2_source_root=$value
            ;;
        *)
            fail "unknown option $option; usage: $0 --project-id <stable-project-id> [--ual2-source-root <private-package-root>]"
            ;;
    esac
done

[ -n "$project_id" ] || fail "usage: $0 --project-id <stable-project-id> [--ual2-source-root <private-package-root>]"
case "$project_id" in
    prj_*) ;;
    *) fail "the project selector must be a stable project ID, not an alias or path" ;;
esac

script_directory=$(CDPATH= cd -- "$(dirname -- "$0")" && pwd -P)
coordinator_root=$(CDPATH= cd -- "$script_directory/.." && pwd -P)
coordinator_id=$(tr -d '\r\n' < "$coordinator_root/.pm/project_id.txt")
manifest="$coordinator_root/.pm/linked_projects.yaml"

if [ -n "$ual2_source_root" ]; then
    [ ! -L "$ual2_source_root" ] || fail "the UAL2 source root must not be a symlink"
    [ -d "$ual2_source_root" ] || fail "the UAL2 source root is not an available directory"
    ual2_source_root=$(CDPATH= cd -- "$ual2_source_root" && pwd -P)
    case "$ual2_source_root/" in
        "$coordinator_root/"*) fail "the private UAL2 source root must remain outside the coordinator family worktree" ;;
    esac
    [ ! -L "$ual2_source_root/Unreal-Godot" ] || fail "the UAL2 Unreal-Godot source directory must not be a symlink"
    [ ! -L "$ual2_source_root/Unreal-Godot/UAL2.glb" ] || fail "the UAL2 source GLB must not be a symlink"
    [ -f "$ual2_source_root/Unreal-Godot/UAL2.glb" ] || fail "the UAL2 source GLB was not found"
fi

path_hint=$(awk -v wanted="$project_id" '
    /^- projectId: / {
        active = substr($0, 14) == wanted
        next
    }
    /^  pathHint: / && active {
        count += 1
        value = substr($0, 13)
    }
    END {
        if (count != 1 || value == "")
            exit 2
        print value
    }
' "$manifest") || fail "project $project_id does not have one committed child path hint"

case "$path_hint" in
    /*|..|../*|*/../*|*/..) fail "linked-project path hint escapes the coordinator" ;;
esac

declared_submodule=$(git -C "$coordinator_root" config --file .gitmodules --get-regexp '^submodule\..*\.path$' |
    awk -v wanted="$path_hint" '$2 == wanted { count += 1 } END { if (count == 1) print wanted }')
[ "$declared_submodule" = "$path_hint" ] || fail "path hint $path_hint is not exactly one declared submodule"

gitlink=$(git -C "$coordinator_root" ls-files --stage -- "$path_hint")
case "$gitlink" in
    "160000 "*"	$path_hint") ;;
    *) fail "path hint $path_hint is not the coordinator's tracked gitlink" ;;
esac

[ ! -L "$coordinator_root/$path_hint" ] || fail "linked-project checkout is a symlink"
[ -d "$coordinator_root/$path_hint" ] || fail "linked-project checkout is unavailable"
consumer_root=$(CDPATH= cd -- "$coordinator_root/$path_hint" && pwd -P)
[ "$consumer_root" = "$coordinator_root/$path_hint" ] || fail "linked-project checkout did not resolve to its declared canonical path"

actual_project_id=$(tr -d '\r\n' < "$consumer_root/.pm/project_id.txt")
[ "$actual_project_id" = "$project_id" ] || fail "resolved checkout has stable project ID $actual_project_id, expected $project_id"

parent_id=$(awk '
    /^parent:/ { in_parent = 1; next }
    /^children:/ { in_parent = 0 }
    /^  projectId: / && in_parent {
        count += 1
        value = substr($0, 14)
    }
    END {
        if (count != 1 || value == "")
            exit 2
        print value
    }
' "$consumer_root/.pm/linked_projects.yaml") || fail "resolved checkout has no unambiguous parent declaration"
[ "$parent_id" = "$coordinator_id" ] || fail "resolved checkout does not reciprocally declare coordinator $coordinator_id"

relative_output="artifacts/chronofall/character-presentation/client"
output_root="$consumer_root/$relative_output"
current="$consumer_root"
for component in artifacts chronofall character-presentation client; do
    current="$current/$component"
    test ! -L "$current" || fail "output path contains symlink $current"
done

git -C "$consumer_root" check-ignore -q -- "$relative_output/" ||
    fail "the exact generated output tree is not ignored by the consumer"
[ -z "$(git -C "$consumer_root" ls-files -- "$relative_output")" ] ||
    fail "the generated output tree contains tracked files"

if [ -d "$output_root" ]; then
    output_symlink=$(find "$output_root" -type l -print -quit)
    [ -z "$output_symlink" ] || fail "the owned output tree contains a symlink: $output_symlink"
    unexpected=$(find "$output_root" -mindepth 1 \
        ! -path "$output_root/quaternius-ual1-standard.cfskel" \
        ! -path "$output_root/quaternius-ual1-standard.provenance.json" \
        ! -path "$output_root/quaternius-medieval-weapons-bow-wooden.cfmesh" \
        ! -path "$output_root/quaternius-medieval-weapons-bow-wooden.provenance.json" \
        ! -path "$output_root/quaternius-medieval-weapons-arrow.cfmesh" \
        ! -path "$output_root/quaternius-medieval-weapons-arrow.provenance.json" \
        ! -path "$output_root/quaternius-ual2-source-bow-shot-body.cfskel" \
        ! -path "$output_root/quaternius-ual2-source-bow-shot-body.provenance.json" \
        ! -path "$output_root/licenses" \
        ! -path "$output_root/licenses/quaternius-ual1-standard" \
        ! -path "$output_root/licenses/quaternius-ual1-standard/License.txt" \
        ! -path "$output_root/licenses/quaternius-ual1-standard/README.txt" \
        ! -path "$output_root/licenses/quaternius-medieval-weapons" \
        ! -path "$output_root/licenses/quaternius-medieval-weapons/License.txt" \
        ! -path "$output_root/licenses/quaternius-ual2-source" \
        ! -path "$output_root/licenses/quaternius-ual2-source/License.txt" \
        ! -path "$output_root/licenses/quaternius-ual2-source/README.txt" \
        -print -quit)
    [ -z "$unexpected" ] || fail "the owned output tree contains unexpected content: $unexpected"
fi

staging_root=$(mktemp -d "${TMPDIR:-/tmp}/chronofall-character-client.XXXXXX")
trap 'rm -r "$staging_root"' EXIT HUP INT TERM
mkdir -p "$staging_root/licenses/quaternius-ual1-standard"
mkdir -p "$staging_root/licenses/quaternius-medieval-weapons"
if [ -n "$ual2_source_root" ]; then
    mkdir -p "$staging_root/licenses/quaternius-ual2-source"
fi

dotnet restore "$coordinator_root/tools/ChronoFall.CharacterCooker/ChronoFall.CharacterCooker.csproj" \
    --disable-build-servers
dotnet restore "$coordinator_root/tools/ChronoFall.StaticMeshCooker/ChronoFall.StaticMeshCooker.csproj" \
    --disable-build-servers
dotnet build "$coordinator_root/tools/ChronoFall.CharacterCooker/ChronoFall.CharacterCooker.csproj" \
    -c Release -m:1 --no-restore --disable-build-servers
dotnet build "$coordinator_root/tools/ChronoFall.StaticMeshCooker/ChronoFall.StaticMeshCooker.csproj" \
    -c Release -m:1 --no-restore --disable-build-servers
dotnet run --project "$coordinator_root/tools/ChronoFall.CharacterCooker/ChronoFall.CharacterCooker.csproj" \
    -c Release --no-restore --no-build -- \
    --source-root "$coordinator_root" \
    --recipe "$coordinator_root/assets/recipes/quaternius-ual1-standard.json" \
    --output "$staging_root/quaternius-ual1-standard.cfskel" \
    --provenance-output "$staging_root/quaternius-ual1-standard.provenance.json" \
    --audience client
if [ -n "$ual2_source_root" ]; then
    dotnet run --project "$coordinator_root/tools/ChronoFall.CharacterCooker/ChronoFall.CharacterCooker.csproj" \
        -c Release --no-restore --no-build -- \
        --source-root "$ual2_source_root" \
        --recipe-root "$coordinator_root" \
        --recipe "$coordinator_root/assets/recipes/quaternius-ual2-source-bow-shot-body.json" \
        --output "$staging_root/quaternius-ual2-source-bow-shot-body.cfskel" \
        --provenance-output "$staging_root/quaternius-ual2-source-bow-shot-body.provenance.json" \
        --audience client
fi
dotnet run --project "$coordinator_root/tools/ChronoFall.StaticMeshCooker/ChronoFall.StaticMeshCooker.csproj" \
    -c Release --no-restore --no-build -- \
    --source-root "$coordinator_root" \
    --recipe "$coordinator_root/assets/recipes/quaternius-medieval-weapons-bow-wooden.json" \
    --output "$staging_root/quaternius-medieval-weapons-bow-wooden.cfmesh" \
    --provenance-output "$staging_root/quaternius-medieval-weapons-bow-wooden.provenance.json" \
    --audience client
dotnet run --project "$coordinator_root/tools/ChronoFall.StaticMeshCooker/ChronoFall.StaticMeshCooker.csproj" \
    -c Release --no-restore --no-build -- \
    --source-root "$coordinator_root" \
    --recipe "$coordinator_root/assets/recipes/quaternius-medieval-weapons-arrow.json" \
    --output "$staging_root/quaternius-medieval-weapons-arrow.cfmesh" \
    --provenance-output "$staging_root/quaternius-medieval-weapons-arrow.provenance.json" \
    --audience client

cp "$coordinator_root/assets/Quaternius/Universal Animation Library[Standard]/License.txt" \
    "$staging_root/licenses/quaternius-ual1-standard/License.txt"
cp "$coordinator_root/assets/Quaternius/Universal Animation Library[Standard]/README.txt" \
    "$staging_root/licenses/quaternius-ual1-standard/README.txt"
cp "$coordinator_root/assets/Quaternius/Medieval Weapons Pack by @Quaternius/License.txt" \
    "$staging_root/licenses/quaternius-medieval-weapons/License.txt"
if [ -n "$ual2_source_root" ]; then
    cp "$coordinator_root/assets/provenance/Quaternius/Universal Animation Library 2 Source/License.txt" \
        "$staging_root/licenses/quaternius-ual2-source/License.txt"
    cp "$coordinator_root/assets/provenance/Quaternius/Universal Animation Library 2 Source/README.txt" \
        "$staging_root/licenses/quaternius-ual2-source/README.txt"
fi

mkdir -p "$output_root/licenses/quaternius-ual1-standard"
mkdir -p "$output_root/licenses/quaternius-medieval-weapons"
cp "$staging_root/quaternius-ual1-standard.cfskel" "$output_root/quaternius-ual1-standard.cfskel"
cp "$staging_root/quaternius-ual1-standard.provenance.json" "$output_root/quaternius-ual1-standard.provenance.json"
cp "$staging_root/quaternius-medieval-weapons-bow-wooden.cfmesh" \
    "$output_root/quaternius-medieval-weapons-bow-wooden.cfmesh"
cp "$staging_root/quaternius-medieval-weapons-bow-wooden.provenance.json" \
    "$output_root/quaternius-medieval-weapons-bow-wooden.provenance.json"
cp "$staging_root/quaternius-medieval-weapons-arrow.cfmesh" \
    "$output_root/quaternius-medieval-weapons-arrow.cfmesh"
cp "$staging_root/quaternius-medieval-weapons-arrow.provenance.json" \
    "$output_root/quaternius-medieval-weapons-arrow.provenance.json"
cp "$staging_root/licenses/quaternius-ual1-standard/License.txt" "$output_root/licenses/quaternius-ual1-standard/License.txt"
cp "$staging_root/licenses/quaternius-ual1-standard/README.txt" "$output_root/licenses/quaternius-ual1-standard/README.txt"
cp "$staging_root/licenses/quaternius-medieval-weapons/License.txt" \
    "$output_root/licenses/quaternius-medieval-weapons/License.txt"

if [ -n "$ual2_source_root" ]; then
    mkdir -p "$output_root/licenses/quaternius-ual2-source"
    cp "$staging_root/quaternius-ual2-source-bow-shot-body.cfskel" \
        "$output_root/quaternius-ual2-source-bow-shot-body.cfskel"
    cp "$staging_root/quaternius-ual2-source-bow-shot-body.provenance.json" \
        "$output_root/quaternius-ual2-source-bow-shot-body.provenance.json"
    cp "$staging_root/licenses/quaternius-ual2-source/License.txt" \
        "$output_root/licenses/quaternius-ual2-source/License.txt"
    cp "$staging_root/licenses/quaternius-ual2-source/README.txt" \
        "$output_root/licenses/quaternius-ual2-source/README.txt"
    ual2_status=staged
else
    rm -f "$output_root/quaternius-ual2-source-bow-shot-body.cfskel"
    rm -f "$output_root/quaternius-ual2-source-bow-shot-body.provenance.json"
    rm -f "$output_root/licenses/quaternius-ual2-source/License.txt"
    rm -f "$output_root/licenses/quaternius-ual2-source/README.txt"
    rmdir "$output_root/licenses/quaternius-ual2-source" 2>/dev/null || true
    ual2_status=absent
fi

printf '%s\n' "CHARACTER_CLIENT_STAGE_SUCCESS project=$project_id output=$output_root ual2=$ual2_status"
