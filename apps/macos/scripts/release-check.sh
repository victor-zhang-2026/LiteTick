#!/bin/zsh
set -euo pipefail

script_dir="${0:A:h}"
project_dir="${script_dir:h}"
product_dir="${project_dir:h:h}"
version_file="$project_dir/Support/Version.plist"
plist_buddy="/usr/libexec/PlistBuddy"

"$script_dir/verify-version.sh" "$project_dir/build/LiteTick.app"

marketing_version="$("$plist_buddy" -c 'Print :MarketingVersion' "$version_file")"
release_channel="$("$plist_buddy" -c 'Print :ReleaseChannel' "$version_file")"

[[ "$release_channel" != "development" ]] || {
    print -u2 "Release blocked: channel is still development"
    exit 1
}

grep -Eq "^## \[$marketing_version\] - [0-9]{4}-[0-9]{2}-[0-9]{2}$" "$product_dir/CHANGELOG.md" || {
    print -u2 "Release blocked: CHANGELOG.md has no dated [$marketing_version] section"
    exit 1
}

print "Release gate passed for LiteTick $marketing_version [$release_channel]"
