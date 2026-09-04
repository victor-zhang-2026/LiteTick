# Release manifest

- Product: 妥了 LiteTick
- Release: `1.0.1`
- Marketing version: `1.0.1`
- Build: `17`
- Channel: `stable`
- Date: `2026-09-04`
- Platform: macOS 14 or later
- Architecture: Apple silicon (`arm64`)
- Bundle: `LiteTick.app`
- Bundle ID: `io.github.victor-zhang-2026.litetick`
- Artifact: `LiteTick-1.0.1-macOS-arm64.zip`
- Artifact size: `326134` bytes
- SHA-256: `24f6e7c7e31e60d366827f00d45a19d149c271ee00c4ac354bc3432bdf899b7c`
- Signing: ad-hoc
- Apple notarization: not notarized

## Verification

- Release build completed without compiler warnings.
- Cross-display dragging, destination-display panel placement, and saved secondary-display position restoration passed user acceptance.
- Bundle identity verification passed after signing.
- Strict deep code-signature verification passed before and after ZIP extraction.
- Release gate passed for `1.0.1 [stable]`.
- Extracted executable verified as `arm64`.
- Public archive contains only the LiteTick application identity.
