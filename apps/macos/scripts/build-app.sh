#!/bin/zsh
set -euo pipefail

script_dir="${0:A:h}"
project_dir="${script_dir:h}"
bundle_dir="$project_dir/build/LiteTick.app"
contents_dir="$bundle_dir/Contents"
version_file="$project_dir/Support/Version.plist"
plist_buddy="/usr/libexec/PlistBuddy"

cd "$project_dir"
"$project_dir/scripts/version.sh" bump-build --quiet
marketing_version="$("$plist_buddy" -c 'Print :MarketingVersion' "$version_file")"
build_number="$("$plist_buddy" -c 'Print :BuildNumber' "$version_file")"
release_channel="$("$plist_buddy" -c 'Print :ReleaseChannel' "$version_file")"
swift build -c release

mkdir -p "$contents_dir/MacOS" "$contents_dir/Resources"
cp ".build/release/LiteTick" "$contents_dir/MacOS/LiteTick"
cp "Support/Info.plist" "$contents_dir/Info.plist"
cp "Support/LiteTick.icns" "$contents_dir/Resources/LiteTick.icns"
cp "Support/LiteTickAlertIcon.png" "$contents_dir/Resources/LiteTickAlertIcon.png"
"$plist_buddy" -c "Add :CFBundleShortVersionString string $marketing_version" "$contents_dir/Info.plist"
"$plist_buddy" -c "Add :CFBundleVersion string $build_number" "$contents_dir/Info.plist"
"$plist_buddy" -c "Add :LiteTickReleaseChannel string $release_channel" "$contents_dir/Info.plist"
chmod +x "$contents_dir/MacOS/LiteTick"

codesign --force --deep --sign - "$bundle_dir"
"$project_dir/scripts/verify-version.sh" "$bundle_dir"
echo "$bundle_dir"
