# LiteTick for Windows

Native Windows counterpart to the macOS LiteTick application.

## Product boundary

- Windows 11 x64 is the primary release baseline; Windows 10 22H2 x64 is the compatibility baseline. Windows 7 is outside maintenance and release scope.
- No account, network, synchronization, analytics, or telemetry.
- One local checklist with the same core behavior and bilingual identity as macOS.
- `LiteTick.exe` is the self-contained Windows 10/11 build and needs no separate runtime.
- Windows-native validation is mandatory before a public release.

## Distribution notice

- The Windows release is a portable, installation-free single EXE.
- GitHub release notes must state that unsigned builds may trigger Microsoft Defender SmartScreen.
- Tell users to continue only when the EXE came from the official LiteTick GitHub Release and its SHA-256 matches the published checksum. On that verified file, Windows 10 users can choose `More info` and then `Run anyway` if SmartScreen blocks the first launch.
- Do not tell users to disable SmartScreen or broadly ignore computer security warnings.

## Local data

```text
%LOCALAPPDATA%\LiteTick\items.json
%LOCALAPPDATA%\LiteTick\items.backup.json
%LOCALAPPDATA%\LiteTick\settings.json
```

`%LOCALAPPDATA%` resolves separately for the Windows account currently running LiteTick. The full directory is normally:

```text
C:\Users\<Windows-user-name>\AppData\Local\LiteTick
```

For example, the Windows account `Dell` stores LiteTick data under `C:\Users\Dell\AppData\Local\LiteTick`. The `AppData` folder is hidden by default; users can press `Win + R`, enter `%LOCALAPPDATA%\LiteTick`, and press Enter to open the correct directory for their own account.

The files are local plaintext JSON and are deliberately stored outside the EXE. Each Windows account has its own independent LiteTick data. Replacing, renaming, moving, or deleting the EXE does not remove the tasks.

### Safe upgrade

1. Quit LiteTick.
2. Optionally copy `%LOCALAPPDATA%\LiteTick` to another folder as a manual backup.
3. Replace the old EXE with the new EXE. Do not copy or move data beside the executable.
4. Open the new EXE and verify the task list.

### Complete removal

1. Quit LiteTick from the floating trigger's context menu.
2. Delete `LiteTick.exe` from wherever it was saved.
3. Press `Win + R`, enter `%LOCALAPPDATA%`, and press Enter.
4. Delete the complete `LiteTick` folder only when all tasks, the last-valid backup, and preferences should be erased permanently.
5. Empty the Recycle Bin if immediate permanent removal is intended.

Deleting only the EXE removes the application while preserving user data. Deleting only `%LOCALAPPDATA%\LiteTick` removes the current Windows account's LiteTick data while leaving the EXE available.

## Build

The maintained project targets .NET 10 LTS with WPF. A project-local SDK may be installed under `.tools/dotnet`. Legacy Windows 7 project files remain in the repository as historical source and are excluded from the maintained workflow.

From macOS, create a cross-built inspection candidate:

```bash
./scripts/build-cross.sh
```

From Windows PowerShell, create the native acceptance candidate:

```powershell
.\scripts\build.ps1
```

Both scripts reserve one monotonically increasing build number and produce:

```text
build/LiteTick.exe   Windows 10/11 x64, self-contained
```

Only an executable rebuilt and tested on Windows 11 x64 and Windows 10 22H2 x64 is eligible as a public release candidate.

`Support/Version.props` is the sole editable source for the Windows marketing version, build number, and release channel. The current `development` channel is not eligible for public distribution.

Complete `WINDOWS_QA.md` on Windows 11 and Windows 10 22H2 before changing the release channel or publishing the EXE.
