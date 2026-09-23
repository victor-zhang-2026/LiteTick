# 安装与数据 / Install and Data

## 系统要求 / Requirements

- Apple 芯片 Mac。
- macOS 14 或更高版本。

—

- Apple silicon Mac.
- macOS 14 or later.

## 安装 / Install

- 从 [GitHub Release](https://github.com/victor-zhang-2026/LiteTick/releases/tag/v1.0.0) 下载 `LiteTick-1.0.0-macOS-arm64.zip`。不要下载 GitHub 自动提供的 `Source code`，也不要使用 `Code → Download ZIP`。
- LiteTick 不使用安装器。Safari 可能自动解压，Finder 也可能隐藏 `.app` 后缀。将显示为 `LiteTick` 或 `LiteTick.app` 的应用拖入 `/Applications` 即可。
- `1.0.0` 尚未使用 Apple Developer ID 签名或公证。首次尝试打开后，如 macOS 阻止运行，请按以下步骤处理：
- 打开“系统设置 → 隐私与安全性”。
- 在“安全性”区域选择“仍要打开”。
- 再次确认打开。
- 仅当压缩包来自本仓库且 SHA-256 校验值与发布清单一致时，才继续绕过 macOS 安全提示。见 [Apple Support](https://support.apple.com/en-us/102445)。

—

- Download `LiteTick-1.0.0-macOS-arm64.zip` from the [GitHub Release](https://github.com/victor-zhang-2026/LiteTick/releases/tag/v1.0.0). Do not download GitHub's automatic `Source code` archives or use `Code → Download ZIP`.
- LiteTick does not use an installer. Safari may unzip the archive automatically, and Finder may hide the `.app` extension. Drag the app shown as `LiteTick` or `LiteTick.app` into `/Applications`.
- `1.0.0` is not yet signed with Apple Developer ID or notarized. After the first blocked launch attempt, do this:
- Open System Settings → Privacy & Security.
- In Security, choose Open Anyway.
- Confirm Open.
- Only override macOS security when the archive came from this repository and its SHA-256 checksum matches the release manifest. See [Apple Support](https://support.apple.com/en-us/102445).

## 数据位置 / Data locations

- 正式版 / Public LiteTick:

```text
~/Library/Application Support/LiteTick/items.json
~/Library/Application Support/LiteTick/items.backup.json
~/Library/Preferences/io.github.victor-zhang-2026.litetick.plist
```

- 任务文件是未加密 JSON。偏好设置包含语言、外观和悬浮入口位置。删除或替换 `.app` 不会自动删除这些文件。
- `~` 代表当前 macOS 用户的个人目录。完整数据目录通常是 `/Users/<macOS 用户名>/Library/Application Support/LiteTick`；每个 macOS 用户拥有独立数据。
- 用户资源库默认隐藏。在 Finder 中选择“前往 → 前往文件夹…”，输入 `~/Library/Application Support/LiteTick`，即可打开当前用户的数据目录。

—

- Public LiteTick:

```text
~/Library/Application Support/LiteTick/items.json
~/Library/Application Support/LiteTick/items.backup.json
~/Library/Preferences/io.github.victor-zhang-2026.litetick.plist
```

- Task files are unencrypted JSON. Preferences include language, appearance, and floating-trigger position. Replacing or deleting the `.app` does not automatically remove these files.
- `~` means the current macOS user's home directory. The full data directory is normally `/Users/<macOS-user-name>/Library/Application Support/LiteTick`; each macOS user has independent data.
- Because the user Library is hidden by default, choose Finder → Go → Go to Folder…, enter `~/Library/Application Support/LiteTick`, and open the current user's data directory.

## 升级 / Upgrade

- 退出 LiteTick。
- 备份 `~/Library/Application Support/LiteTick`。
- 将新的 `LiteTick.app` 拖入“应用程序”并替换旧应用。
- 打开新版并检查版本和任务。

—

- Quit LiteTick.
- Back up `~/Library/Application Support/LiteTick`.
- Drag the new `LiteTick.app` into Applications and replace the old app.
- Open it and verify the version and tasks.

## 完整卸载 / Complete uninstall

- 先退出妥了。删除 `/Applications/LiteTick.app` 只会删除程序，不会删除任务。
- 如果确定不再需要数据，在 Finder 中使用“前往 → 前往文件夹…”分别打开并删除：

```text
~/Library/Application Support/LiteTick
~/Library/Preferences/io.github.victor-zhang-2026.litetick.plist
```

- 需要立即永久清除时，再清空废纸篓。
- 只删除 `.app` 会保留数据；只删除上述用户目录会清除当前 macOS 用户的数据和偏好，但不会删除应用。

—

- Quit LiteTick first. Removing `/Applications/LiteTick.app` removes only the application.
- To erase all tasks, backups, and preferences, use Finder → Go → Go to Folder… to open and delete both paths above, then empty Trash if immediate permanent removal is intended.
- Deleting only the `.app` preserves data; deleting only the two user paths removes the current macOS user's data and preferences while preserving the application.

## Windows 免安装版 / Windows portable builds

Windows 版是可直接打开的单个 EXE，不使用安装器，也不把任务放在 EXE 旁边：

```text
%LOCALAPPDATA%\LiteTick\items.json
%LOCALAPPDATA%\LiteTick\items.backup.json
%LOCALAPPDATA%\LiteTick\settings.json
```

- `items.json` 是当前任务；`items.backup.json` 是最后一次有效备份；`settings.json` 保存语言、外观和悬浮入口位置。
- `%LOCALAPPDATA%` 会根据当前 Windows 用户分别解析。完整路径通常是 `C:\Users\<Windows 用户名>\AppData\Local\LiteTick`；例如用户 `Dell` 对应 `C:\Users\Dell\AppData\Local\LiteTick`。每个 Windows 用户拥有独立数据。
- `AppData` 默认隐藏。按 `Win + R`，输入 `%LOCALAPPDATA%\LiteTick` 并回车，可以直接打开当前用户的实际数据目录。
- Windows 10 及以上版本使用同一目录和数据格式。移动、重命名、替换或删除 EXE 不会删除待办。
- 升级时先退出 LiteTick，可选备份整个 `%LOCALAPPDATA%\LiteTick` 文件夹，再用新 EXE 替换旧 EXE，并重新打开。
- 完全删除：先退出妥了，删除 `LiteTick.exe`，再通过 `Win + R` 打开 `%LOCALAPPDATA%` 并删除整个 `LiteTick` 文件夹；需要立即永久清除时再清空回收站。
- 只删除 EXE 会保留数据；只删除 `%LOCALAPPDATA%\LiteTick` 会清除当前 Windows 用户的数据但保留程序文件。

—

The Windows edition is a directly opened single EXE with no installer. It never stores tasks beside the executable.

- `items.json` contains current tasks; `items.backup.json` is the last-valid backup; `settings.json` stores language, appearance, and floating-trigger position.
- `%LOCALAPPDATA%` resolves independently for the current Windows account. The full path is normally `C:\Users\<Windows-user-name>\AppData\Local\LiteTick`; for example, account `Dell` uses `C:\Users\Dell\AppData\Local\LiteTick`. Each Windows account has separate data.
- Because `AppData` is hidden by default, press `Win + R`, enter `%LOCALAPPDATA%\LiteTick`, and press Enter to open the current user's actual data directory.
- Windows 10 and later builds use this directory and data schema. Moving, renaming, replacing, or deleting the EXE does not delete tasks.
- To upgrade, quit LiteTick, optionally back up the complete `%LOCALAPPDATA%\LiteTick` folder, replace the old EXE, then reopen it.
- For complete removal, quit LiteTick, delete `LiteTick.exe`, open `%LOCALAPPDATA%` through `Win + R`, and delete the complete `LiteTick` folder. Empty the Recycle Bin if immediate permanent removal is intended.
- Deleting only the EXE preserves data; deleting only `%LOCALAPPDATA%\LiteTick` removes the current Windows account's data while preserving the program file.
