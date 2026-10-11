# Release manifest

- Product: 妥了 LiteTick
- Release: `1.2.1`
- Marketing version: `1.2.1`
- Build: `32`
- Channel: `stable`
- Date: `2026-10-11`
- Platform: macOS 14 or later only
- Architecture: Apple silicon (`arm64`)
- Bundle: `LiteTick.app`
- Bundle ID: `io.github.victor-zhang-2026.litetick`
- Artifact: `LiteTick-1.2.1-macOS-arm64.zip`
- Artifact size: `332708` bytes
- SHA-256: `6e2ccff90cab8a314f88e3b0419855f90ddf69048dac10eed74206941dc4c823`
- Signing: ad-hoc
- Apple notarization: not notarized
- Windows release: unchanged at `1.1.0`

## Verification

- Release build, bundle identity, and strict deep code signature passed.
- Native light and dark panel colors were compared with the previous installed Mac app.
- Native light and dark deletion confirmations stayed within the panel's rounded corners; Escape canceled without deleting, and the input focus outline cleared.
- The unfinished-task badge stayed visible through repeated panel openings and closings on the installed app; isolated count transitions and 0/2/12/99/100 rendering passed.
- The release ZIP was extracted and its bundle identity, signature, architecture, and executable hash matched the tested installed app.
