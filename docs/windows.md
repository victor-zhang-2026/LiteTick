# LiteTick Windows architecture and release boundary

## Product contract

The Windows application reproduces LiteTick's established desktop behavior rather than creating a separate task product:

- one uncategorized local checklist;
- floating always-on-top trigger with Hover opening, click engagement, free multi-display placement, and saved position;
- capture, direct editing, completion and restoration, one pinned item, handle-based reordering, Completed search, and confirmed permanent deletion;
- Simplified Chinese and English plus Light and Dark appearances;
- no account, network, synchronization, analytics, advertising, telemetry, reminders, dates, tags, or other task-management expansion.

Windows uses its own local data directory and does not synchronize with macOS.

## Architecture

- Maintained UI and behavior: WPF with direct Win32 monitor integration.
- Maintained build: .NET 10 LTS, self-contained `LiteTick.exe` for Windows 10 and later on x64.
- Native integration: Win32 monitor/work-area and window-position APIs for the trigger and panel.
- Distribution: one unpackaged application EXE that runs directly without an installer, administrator access, or separately installed runtime.
- Data: plaintext JSON under `%LOCALAPPDATA%\LiteTick`, with atomic replacement and one last-valid backup.
- Legacy boundary: retained Windows 7 project files are historical source only and are excluded from builds, QA, fixes, compatibility claims, and releases.

## Supported Windows boundary

Windows 11 x64 is the primary release baseline, and Windows 10 22H2 x64 is the compatibility baseline. Windows 7 and earlier releases are outside LiteTick's maintained and released product scope.

## Version identity

`apps/windows/Support/Version.props` is the sole editable Windows version source:

- `MarketingVersion` — SemVer core version;
- `BuildNumber` — positive and monotonically increasing for every packaged EXE;
- `ReleaseChannel` — `development`, `alpha`, `beta`, `rc`, or `stable`.

Both Windows build entry points reserve one new Windows build number, then build the modern x64 artifact. Failed builds may leave gaps, and a build number is never reused. A Windows public release requires a non-development channel, a dated changelog entry, native acceptance for every claimed system, checksum and PE identity verification, and explicit user approval.

## Build trust boundary

macOS can cross-compile the modern x64 inspection candidate and verify its PE GUI identity, icon, manifest, version resources, and packaging. Cross-compilation cannot prove Windows interaction behavior. The final public artifact must therefore be rebuilt and tested on native Windows 11 x64 and Windows 10 22H2 x64 before those compatibility claims are released.

Until a trusted Windows code-signing certificate is configured, builds are unsigned and Windows SmartScreen may warn. GitHub release notes must label the EXE as portable and installation-free, publish its SHA-256, and explain that users should proceed through `More info` → `Run anyway` only after confirming the file came from the official LiteTick GitHub Release and matches that checksum. Never instruct users to disable SmartScreen or generally ignore security warnings.
