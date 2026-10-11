# Release manifest

- Product: 妥了 LiteTick
- Release: `1.2.0`
- Marketing version: `1.2.0`
- Build: `23`
- Channel: `stable`
- Date: `2026-10-11`
- Platform: macOS 14 or later only
- Architecture: Apple silicon (`arm64`)
- Bundle: `LiteTick.app`
- Bundle ID: `io.github.victor-zhang-2026.litetick`
- Artifact: `LiteTick-1.2.0-macOS-arm64.zip`
- Artifact size: `322738` bytes
- SHA-256: `3f2748dcceb351c117ae1e9928628f7bb3e02e89f82fd2c77a13ae69160cdd29`
- Signing: ad-hoc
- Apple notarization: not notarized
- Windows release: unchanged at `1.1.0`

## Verification

- Release build, bundle identity, and strict deep code signature passed.
- Isolated task-store checks covered add, complete, restore, delete, zero count, and persistence.
- Trigger rendering was inspected for 0, 2, 12, 99, and `99+`; two-digit text fits its badge.
- `/Applications/LiteTick.app` was updated and visually showed the current unfinished count of 2.
- The release ZIP was extracted and its bundle identity, signature, and executable hash matched the tested build.
