# 妥了 LiteTick for macOS

Native Swift 6, SwiftUI, and AppKit implementation with no third-party runtime dependencies. The public build is local-only and uses the Bundle ID `io.github.victor-zhang-2026.litetick`.

## Requirements

- Apple silicon Mac
- macOS 14 SDK or later
- Swift 6

## Application build

```bash
./scripts/build-app.sh
open build/LiteTick.app
```

The build uses `io.github.victor-zhang-2026.litetick` and `Application Support/LiteTick`. Every packaged build reserves a new monotonically increasing build number, injects version metadata, signs ad hoc, and verifies bundle identity. Failed builds may leave intentional build-number gaps.

On a Command Line Tools-only Mac where the macOS 27 SDK lacks the SwiftUI macro plugin, use the installed macOS 26.5 SDK: `LITETICK_MACOS_SDK=/Library/Developer/CommandLineTools/SDKs/MacOSX26.5.sdk ./scripts/build-app.sh`.

## Release gate

Set the version and channel only through `Support/Version.plist` using `scripts/version.sh`, create the matching dated changelog section, build the final candidate, and run:

```bash
./scripts/release-check.sh
```

See [versioning](../../docs/versioning.md), [privacy](../../docs/privacy.md), and [install/data](../../docs/install-and-data.md).
