# Release manifest

- Product: 妥了 LiteTick
- Release: `1.0.0-rc.1`
- Marketing version: `1.0.0`
- Build: `14`
- Channel: `rc`
- Date: `2026-08-27`
- Platform: macOS 14 or later
- Architecture: Apple silicon (`arm64`)
- Bundle: `LiteTick.app`
- Bundle ID: `io.github.victor-zhang-2026.litetick`
- Artifact: `LiteTick-1.0.0-rc.1-macOS-arm64.zip`
- Artifact size: `324202` bytes
- SHA-256: `15644231781b6b84abe952f2f4889f669c89937ca5ff035c574420fb7cd42d52`
- Signing: ad-hoc
- Apple notarization: not notarized

## Verification

- Release build completed without compiler warnings.
- Bundle identity verification passed after signing.
- Strict deep code-signature verification passed before and after ZIP extraction.
- Release gate passed for `1.0.0 [rc]`.
- Extracted executable verified as `arm64`.
- Source scan found no network API references.
- Local-only data storage and backup recovery were verified before packaging.
