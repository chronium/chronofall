#!/bin/sh
set -eu

fail()
{
    printf '%s\n' "CHARACTER_CLIENT_STAGE_FAILURE: $*" >&2
    exit 1
}

if [ "$#" -ne 2 ] || [ "$1" != "--project-id" ]; then
    fail "usage: $0 --project-id <stable-project-id>"
fi

project_id=$2
case "$project_id" in
    prj_*) ;;
    *) fail "the project selector must be a stable project ID, not an alias or path" ;;
esac

script_directory=$(CDPATH= cd -- "$(dirname -- "$0")" && pwd -P)
coordinator_root=$(CDPATH= cd -- "$script_directory/.." && pwd -P)
coordinator_id=$(tr -d '\r\n' < "$coordinator_root/.pm/project_id.txt")
manifest="$coordinator_root/.pm/linked_projects.yaml"

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
        ! -path "$output_root/licenses" \
        ! -path "$output_root/licenses/quaternius-ual1-standard" \
        ! -path "$output_root/licenses/quaternius-ual1-standard/License.txt" \
        ! -path "$output_root/licenses/quaternius-ual1-standard/README.txt" \
        -print -quit)
    [ -z "$unexpected" ] || fail "the owned output tree contains unexpected content: $unexpected"
fi

staging_root=$(mktemp -d "${TMPDIR:-/tmp}/chronofall-character-client.XXXXXX")
trap 'rm -r "$staging_root"' EXIT HUP INT TERM
mkdir -p "$staging_root/licenses/quaternius-ual1-standard"

dotnet restore "$coordinator_root/tools/ChronoFall.CharacterCooker/ChronoFall.CharacterCooker.csproj" \
    --disable-build-servers
dotnet build "$coordinator_root/tools/ChronoFall.CharacterCooker/ChronoFall.CharacterCooker.csproj" \
    -c Release -m:1 --no-restore --disable-build-servers
dotnet run --project "$coordinator_root/tools/ChronoFall.CharacterCooker/ChronoFall.CharacterCooker.csproj" \
    -c Release --no-restore --no-build -- \
    --source-root "$coordinator_root" \
    --recipe "$coordinator_root/assets/recipes/quaternius-ual1-standard.json" \
    --output "$staging_root/quaternius-ual1-standard.cfskel" \
    --provenance-output "$staging_root/quaternius-ual1-standard.provenance.json" \
    --audience client

cp "$coordinator_root/assets/Quaternius/Universal Animation Library[Standard]/License.txt" \
    "$staging_root/licenses/quaternius-ual1-standard/License.txt"
cp "$coordinator_root/assets/Quaternius/Universal Animation Library[Standard]/README.txt" \
    "$staging_root/licenses/quaternius-ual1-standard/README.txt"

mkdir -p "$output_root/licenses/quaternius-ual1-standard"
cp "$staging_root/quaternius-ual1-standard.cfskel" "$output_root/quaternius-ual1-standard.cfskel"
cp "$staging_root/quaternius-ual1-standard.provenance.json" "$output_root/quaternius-ual1-standard.provenance.json"
cp "$staging_root/licenses/quaternius-ual1-standard/License.txt" "$output_root/licenses/quaternius-ual1-standard/License.txt"
cp "$staging_root/licenses/quaternius-ual1-standard/README.txt" "$output_root/licenses/quaternius-ual1-standard/README.txt"

printf '%s\n' "CHARACTER_CLIENT_STAGE_SUCCESS project=$project_id output=$output_root"
