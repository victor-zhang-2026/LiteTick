#!/bin/zsh
set -euo pipefail

script_dir="${0:A:h}"
project_dir="${script_dir:h}"
dotnet_bin="$project_dir/.tools/dotnet/dotnet"
export DOTNET_CLI_TELEMETRY_OPTOUT=1
export DOTNET_NOLOGO=1

"$dotnet_bin" run --project "$project_dir/tests/CoreSmoke/CoreSmoke.csproj" --configuration Release
marketing_version="$("$project_dir/scripts/version.sh" show | sed -E 's/^([^ ]+) .*/\1/')"
build_number="$(sed -n 's:.*<BuildNumber>\(.*\)</BuildNumber>.*:\1:p' "$project_dir/Support/Version.props")"
release_channel="$(sed -n 's:.*<ReleaseChannel>\(.*\)</ReleaseChannel>.*:\1:p' "$project_dir/Support/Version.props")"
"$dotnet_bin" run --project "$project_dir/tests/PeVerify/PeVerify.csproj" --configuration Release -- \
    "$project_dir/build/LiteTick.exe" "$marketing_version" "$build_number" "$release_channel" modern
