# Release manifest

- Product: 妥了 LiteTick
- Release: `1.1.1`
- Marketing version: `1.1.1`
- Build: `21`
- Channel: `stable`
- Date: `2026-09-23`
- Platform: macOS 14 or later
- Architecture: Apple silicon (`arm64`)
- Bundle: `LiteTick.app`
- Bundle ID: `io.github.victor-zhang-2026.litetick`
- Artifact: `LiteTick-1.1.1-macOS-arm64.zip`
- Artifact size: `318256` bytes
- SHA-256: `2fd32dd4f5fa19153ebc05aef1a6c3ce60365a2ca0b3daca1d5c3905f92417c8`
- Signing: ad-hoc
- Apple notarization: not notarized

## Verification

- Release compilation completed successfully with the macOS 26.5 SDK workaround required by the installed Command Line Tools; the linker reported two missing optional Command Line Tools search paths.
- Bundle identity verification passed after signing.
- Strict deep code-signature verification passed before archiving.
- Release gate passed for `1.1.1 [stable]`.
- Packaged executable verified as `arm64`.
- Archive contents and SHA-256 were inspected locally.
- Window-level behavior was verified structurally in source; real-device interaction acceptance remains pending.
