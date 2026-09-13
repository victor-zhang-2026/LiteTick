#!/bin/zsh
set -euo pipefail

script_dir="${0:A:h}"
project_dir="${script_dir:h}"
dotnet_bin="$project_dir/.tools/dotnet/dotnet"
export DOTNET_CLI_TELEMETRY_OPTOUT=1
export DOTNET_NOLOGO=1

if [[ ! -x "$dotnet_bin" ]]; then
    print -u2 "Missing project-local .NET SDK: $dotnet_bin"
    exit 1
fi

"$project_dir/scripts/version.sh" bump-build --quiet
"$dotnet_bin" publish "$project_dir/LiteTick.Windows.csproj" \
    --configuration Release \
    --runtime win-x64 \
    --self-contained true \
    --output "$project_dir/build/publish"

published_files=("$project_dir/build/publish"/*(N))
[[ ${#published_files[@]} -eq 1 && "${published_files[1]:t}" == "LiteTick.exe" ]] || {
    print -u2 "Expected exactly one published file named LiteTick.exe"
    ls -la "$project_dir/build/publish"
    exit 1
}

cp "$project_dir/build/publish/LiteTick.exe" "$project_dir/build/LiteTick.exe"
file "$project_dir/build/LiteTick.exe"
"$project_dir/scripts/test.sh"
print "$project_dir/build/LiteTick.exe"
