#!/bin/zsh
set -euo pipefail

script_dir="${0:A:h}"
project_dir="${script_dir:h}"
version_file="$project_dir/Support/Version.props"

fail() {
    print -u2 "version: $1"
    exit 1
}

read_value() {
    sed -n "s:.*<$1>\(.*\)</$1>.*:\1:p" "$version_file"
}

validate_version() {
    [[ "$1" =~ '^(0|[1-9][0-9]*)\.(0|[1-9][0-9]*)\.(0|[1-9][0-9]*)$' ]] || \
        fail "marketing version must be SemVer core format MAJOR.MINOR.PATCH"
}

validate_build() {
    [[ "$1" =~ '^[1-9][0-9]*$' ]] || fail "build number must be a positive integer"
}

validate_channel() {
    case "$1" in
        development|alpha|beta|rc|stable) ;;
        *) fail "release channel must be development, alpha, beta, rc, or stable" ;;
    esac
}

replace_value() {
    local key="$1"
    local value="$2"
    perl -0pi -e "s|<$key>[^<]+</$key>|<$key>$value</$key>|" "$version_file"
}

show_version() {
    local marketing_version build_number release_channel
    marketing_version="$(read_value MarketingVersion)"
    build_number="$(read_value BuildNumber)"
    release_channel="$(read_value ReleaseChannel)"
    validate_version "$marketing_version"
    validate_build "$build_number"
    validate_channel "$release_channel"
    print "$marketing_version ($build_number) [$release_channel]"
}

command="${1:-show}"
case "$command" in
    show)
        show_version
        ;;
    validate)
        show_version >/dev/null
        print "Version metadata is valid: $(show_version)"
        ;;
    bump-build)
        current_build="$(read_value BuildNumber)"
        validate_build "$current_build"
        replace_value BuildNumber "$((current_build + 1))"
        [[ "${2:-}" == "--quiet" ]] || show_version
        ;;
    set)
        [[ $# -eq 4 ]] || fail "usage: version.sh set MAJOR.MINOR.PATCH BUILD_NUMBER CHANNEL"
        validate_version "$2"
        validate_build "$3"
        validate_channel "$4"
        current_build="$(read_value BuildNumber)"
        (( $3 > current_build )) || fail "new build number must be greater than current build $current_build"
        replace_value MarketingVersion "$2"
        replace_value BuildNumber "$3"
        replace_value ReleaseChannel "$4"
        show_version
        ;;
    set-channel)
        [[ $# -eq 2 ]] || fail "usage: version.sh set-channel CHANNEL"
        validate_channel "$2"
        replace_value ReleaseChannel "$2"
        show_version
        ;;
    *)
        fail "unknown command '$command'; use show, validate, bump-build, set, or set-channel"
        ;;
esac
