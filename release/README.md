# Release workspace

This directory is reserved for LiteTick release material. It must not contain ordinary source or design work.

No artifact in this directory is approved for public distribution merely because it exists here.

For each approved release, use a directory named `vMAJOR.MINOR.PATCH/` containing, as applicable:

- signed and notarized application/archive;
- SHA-256 checksum manifest;
- release notes derived from `CHANGELOG.md`;
- signing/notarization verification output;
- compatibility, migration, privacy, and uninstall checks;
- a release manifest recording version, build number, channel, date, and artifact names.

For every desktop release, explain that LiteTick data belongs to the operating-system user who runs the app, show both the shorthand and normal per-user path form, explain how to open hidden data locations, and distinguish application-only removal from permanent data deletion.

For a macOS release, document `~/Library/Application Support/LiteTick`, `~/Library/Preferences/io.github.victor-zhang-2026.litetick.plist`, their normal `/Users/<macOS-user-name>/...` form, Finder → Go → Go to Folder…, and complete removal of the app plus both current-user paths.

For a Windows release, also state that `LiteTick.exe` is a portable, installation-free application. Explain that each Windows account stores independent data under `%LOCALAPPDATA%\LiteTick`, normally `C:\Users\<Windows-user-name>\AppData\Local\LiteTick`, and give the `Win + R` shortcut for opening the hidden directory. Document complete removal as quitting LiteTick, deleting the EXE, deleting the current user's complete `%LOCALAPPDATA%\LiteTick` folder, and optionally emptying the Recycle Bin. If the unsigned EXE triggers Microsoft Defender SmartScreen, instruct users to verify the official GitHub Release source and published SHA-256 first, then use `More info` → `Run anyway` for that verified file. Do not advise disabling SmartScreen or ignoring unrelated security warnings.

Follow `docs/versioning.md` and pass `apps/macos/scripts/release-check.sh` before placing a candidate here. Publishing or creating external resources always requires explicit user approval.
