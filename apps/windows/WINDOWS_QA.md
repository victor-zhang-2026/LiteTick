# LiteTick Windows native acceptance

Run this checklist on the named native Windows systems before any public release. A macOS cross-build does not satisfy this gate.

## Support matrix

| System | Artifact | Runtime | Positioning expectation |
| --- | --- | --- | --- |
| Windows 11 x64 | `LiteTick.exe` | Embedded; no install | Per-monitor DPI and multi-display |
| Windows 10 22H2 x64 | `LiteTick.exe` | Embedded; no install | Per-monitor DPI and multi-display |

Windows 7 is outside LiteTick's maintained and released product scope. Passing this checklist establishes LiteTick compatibility with the tested Windows 10/11 systems only.

## 测试准备 / Setup

- Copy only the artifact named for the test system to a temporary folder.
- Record the Windows version, display count/scaling, executable version, file size, SHA-256, and any SmartScreen warning.
- Back up `%LOCALAPPDATA%\LiteTick` if it already exists.

## 验收清单 / Acceptance checklist

- [ ] The EXE launches without a console window, installer, administrator prompt, or separately downloaded runtime.
- [ ] Starting the EXE twice leaves only one LiteTick process and does not corrupt data.
- [ ] The circular trigger is transparent outside its white ring, stays above ordinary windows, and opens the checklist immediately on Hover.
- [ ] Clicking the trigger or interacting with the checklist keeps it open; Close hides only the checklist; right-click Quit and normal Windows close commands show confirmation.
- [ ] The trigger drags smoothly to every connected display, remains fully visible when released, and restores its saved secondary-display position after restart.
- [ ] The checklist remains inside the trigger's display work area at left, right, top, and bottom placements. Validate mixed per-monitor scaling on Windows 10/11.
- [ ] New tasks appear first below a fixed top item; blank submissions are ignored; consecutive Return submissions keep input focus.
- [ ] Single-click editing, Return save, Escape cancel, click-away save, Cut/Copy/Paste, and the context-menu Edit path work.
- [ ] Completion moves an item to Completed; its checked square restores it; completed dates and search results are correct in Chinese and English.
- [ ] Exactly one unfinished item can be fixed at the top; replacing and removing that state preserves the documented order.
- [ ] Reordering starts only from the six-dot handle, never crosses above the fixed item, and does not export task IDs to other applications.
- [ ] Individual deletion, Delete All, and search-scoped Delete Results require confirmation and affect only the stated items.
- [ ] Chinese/English and Light/Dark changes apply immediately and persist after restart.
- [ ] About displays the Windows version/build/channel and opens repository, privacy, and license links only when selected.
- [ ] `%LOCALAPPDATA%\LiteTick\items.json`, `items.backup.json`, and `settings.json` contain the expected local plaintext data; replacing the EXE preserves them.
- [ ] With a valid backup present, a deliberately invalid primary task file recovers the prior valid list without deleting the backup.
- [ ] Keyboard Tab navigation and screen-reader names identify input, completion, pin, reorder, navigation, appearance, language, Close, and delete controls.

## Release gate

After every item passes:

1. Record defects and fixes in `bug.md` and `progress.md`.
2. Choose the release SemVer and a non-development channel in `Support/Version.props` without reusing a build number.
3. Rebuild the Windows 10/11 x64 EXE on Windows with `scripts\build.ps1` and repeat identity, checksum, malware, and interaction checks on that exact file.
4. Prepare release notes and obtain explicit approval before committing, tagging, uploading, or publishing.
