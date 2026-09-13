# Release manifest

- Product: 妥了 LiteTick
- Release: `1.1.0`
- Marketing version: `1.1.0`
- Build: `12`
- Channel: `stable`
- Date: `2026-09-14`
- Platform: Windows 10 or later
- Architecture: x64
- Artifact: `LiteTick-1.1.0-Windows-x64.exe`
- Artifact size: `71178792` bytes
- SHA-256: `b1bfab8038186e1ed074e9e3201fdfc94d0c42c393452f1e60a23ed4e5ba6927`
- Packaging: self-contained portable single EXE; no installer or additional runtime
- Windows code signing: unsigned

## Verification

- User acceptance completed on Windows for the final source behavior before release preparation.
- Final stable build changes release identity to `1.1.0 build 12 stable` and uses the accepted source behavior.
- Release compilation completed without warnings or errors.
- Core persistence and ordering smoke tests passed.
- PE verification passed for AMD64 Windows GUI identity, embedded icon, manifest, version, build, and stable channel.
- The artifact is limited to Windows 10 and later; Windows 7 is not supported or released.
- SHA-256 was recorded for download verification.
