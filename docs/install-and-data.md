# 安装与数据 / Install and Data

## 系统要求 / Requirements

- Apple 芯片 Mac / Apple silicon Mac
- macOS 14 或更高版本 / macOS 14 or later

## 安装 / Install

LiteTick 不使用安装器。解压 Release ZIP，将 `LiteTick.app` 拖入 `/Applications` 即可。技术上可以从其他目录运行，但放入“应用程序”更便于升级和管理。

LiteTick does not use an installer. Unzip the release archive and drag `LiteTick.app` into `/Applications`. It can run elsewhere, but Applications is recommended for predictable upgrades and management.

### 未公证候选版 / Unnotarized release candidate

`1.0.0-rc.1` 尚未使用 Apple Developer ID 签名或公证。首次尝试打开后，如 macOS 阻止运行：

1. 打开“系统设置 → 隐私与安全性”。
2. 在“安全性”区域选择“仍要打开”。
3. 再次确认打开。

`1.0.0-rc.1` is not yet signed with Apple Developer ID or notarized. After the first blocked launch attempt:

1. Open System Settings → Privacy & Security.
2. In Security, choose Open Anyway.
3. Confirm Open.

Apple warns that bypassing this protection carries risk. Only proceed for an archive downloaded from the official repository whose SHA-256 checksum matches the release manifest. See [Apple Support](https://support.apple.com/en-us/102445).

## 数据位置 / Data locations

正式版 / Public LiteTick:

```text
~/Library/Application Support/LiteTick/items.json
~/Library/Application Support/LiteTick/items.backup.json
~/Library/Preferences/io.github.victor-zhang-2026.litetick.plist
```

任务文件是未加密 JSON。偏好设置包含语言、外观和悬浮入口位置。删除或替换 `.app` 不会自动删除这些文件。

Task files are unencrypted JSON. Preferences include language, appearance, and floating-trigger position. Replacing or deleting the `.app` does not automatically remove these files.

## 升级 / Upgrade

1. 退出 LiteTick。
2. 备份 `~/Library/Application Support/LiteTick`。
3. 将新的 `LiteTick.app` 拖入“应用程序”并替换旧应用。
4. 打开新版并检查版本和任务。

1. Quit LiteTick.
2. Back up `~/Library/Application Support/LiteTick`.
3. Drag the new `LiteTick.app` into Applications and replace the old app.
4. Open it and verify the version and tasks.

## 完整卸载 / Complete uninstall

先退出 LiteTick。删除 `/Applications/LiteTick.app` 只会删除程序，不会删除任务。如果确定不再需要数据，再分别删除：

```text
~/Library/Application Support/LiteTick
~/Library/Preferences/io.github.victor-zhang-2026.litetick.plist
```

Quit LiteTick first. Removing `/Applications/LiteTick.app` removes only the application. Delete the two paths above separately only if you also intend to erase all tasks, backups, and preferences. Empty Trash if permanent removal is intended.
