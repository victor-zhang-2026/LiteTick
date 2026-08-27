# Release manifest

- Product: 妥了 LiteTick
- Release: `1.0.0`
- Marketing version: `1.0.0`
- Build: `15`
- Channel: `stable`
- Date: `2026-08-27`
- Platform: macOS 14 or later
- Architecture: Apple silicon (`arm64`)
- Bundle: `LiteTick.app`
- Bundle ID: `io.github.victor-zhang-2026.litetick`
- Artifact: `LiteTick-1.0.0-macOS-arm64.zip`
- Artifact size: `324207` bytes
- SHA-256: `1d0d1dd909f22a269efd4aa586cc5abb0dfe6802b062f877835af9bd94daed1a`
- Signing: ad-hoc
- Apple notarization: not notarized

## Verification

- Release build completed without compiler warnings.
- Bundle identity verification passed after signing.
- Strict deep code-signature verification passed before and after ZIP extraction.
- Release gate passed for `1.0.0 [stable]`.
- Extracted executable verified as `arm64`.
- Public archive contains only the LiteTick application identity.
- Local-only data storage and backup recovery were verified before packaging.
