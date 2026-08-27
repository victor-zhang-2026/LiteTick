#!/bin/zsh
set -euo pipefail

script_dir="${0:A:h}"
project_dir="${script_dir:h}"
version_file="$project_dir/Support/Version.plist"
bundle_path="${1:-$project_dir/build/LiteTick.app}"
bundle_info="$bundle_path/Contents/Info.plist"
plist_buddy="/usr/libexec/PlistBuddy"

[[ -f "$version_file" ]] || { print -u2 "Missing $version_file"; exit 1; }
[[ -f "$bundle_info" ]] || { print -u2 "Missing $bundle_info"; exit 1; }

"$script_dir/version.sh" validate >/dev/null

expected_version="$("$plist_buddy" -c 'Print :MarketingVersion' "$version_file")"
expected_build="$("$plist_buddy" -c 'Print :BuildNumber' "$version_file")"
expected_channel="$("$plist_buddy" -c 'Print :ReleaseChannel' "$version_file")"
actual_version="$("$plist_buddy" -c 'Print :CFBundleShortVersionString' "$bundle_info")"
actual_build="$("$plist_buddy" -c 'Print :CFBundleVersion' "$bundle_info")"
actual_channel="$("$plist_buddy" -c 'Print :LiteTickReleaseChannel' "$bundle_info")"

[[ "$actual_version" == "$expected_version" ]] || { print -u2 "Version mismatch: expected $expected_version, found $actual_version"; exit 1; }
[[ "$actual_build" == "$expected_build" ]] || { print -u2 "Build mismatch: expected $expected_build, found $actual_build"; exit 1; }
[[ "$actual_channel" == "$expected_channel" ]] || { print -u2 "Channel mismatch: expected $expected_channel, found $actual_channel"; exit 1; }

codesign --verify --deep --strict "$bundle_path"
print "Verified LiteTick $actual_version ($actual_build) [$actual_channel]"
