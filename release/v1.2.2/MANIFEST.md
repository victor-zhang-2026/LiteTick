# Release manifest

- Product: 妥了 LiteTick
- Release: `1.2.2`
- Marketing version: `1.2.2`
- Build: `33`
- Channel: `stable`
- Date: `2026-10-28`
- Platform: macOS 14 or later only
- Architecture: Apple silicon (`arm64`)
- Bundle: `LiteTick.app`
- Bundle ID: `io.github.victor-zhang-2026.litetick`
- Artifact: `LiteTick-1.2.2-macOS-arm64.zip`
- Artifact size: `333268` bytes
- SHA-256: `91898840d4aefb80a200be078aeaa9b5f849d005327418b1801ded5edf71ce8d`
- Signing: ad-hoc
- Apple notarization: not notarized
- Windows release: unchanged at `1.1.0`

## Verification

- Release compilation, version gate, arm64 identity, and strict deep code signature passed.
- A native QA host compiled the production views and store directly, with separate preferences and recoverable test fixtures. User tasks were not used for deletion tests.
- Chinese all-completed confirmation: clicking Cancel's left whitespace preserved the fixture; clicking Delete's right whitespace removed it.
- English single-item confirmation opened from the context menu: clicking Cancel's right whitespace preserved the fixture; clicking Delete's left whitespace removed it.
- The same confirmation labels and rectangular hit regions serve single-item, all-completed, and filtered-result deletion; filtered-result interaction was not separately exercised.
- Installed `/Applications/LiteTick.app` identity and executable checksum match the release build; startup displayed the floating trigger.
