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

—

- Public LiteTick:

```text
~/Library/Application Support/LiteTick/items.json
~/Library/Application Support/LiteTick/items.backup.json
~/Library/Preferences/io.github.victor-zhang-2026.litetick.plist
```

- Task files are unencrypted JSON. Preferences include language, appearance, and floating-trigger position. Replacing or deleting the `.app` does not automatically remove these files.

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

- 先退出 LiteTick。删除 `/Applications/LiteTick.app` 只会删除程序，不会删除任务。
- 如果确定不再需要数据，再分别删除：

```text
~/Library/Application Support/LiteTick
~/Library/Preferences/io.github.victor-zhang-2026.litetick.plist
```

—

- Quit LiteTick first. Removing `/Applications/LiteTick.app` removes only the application.
- Delete the two paths above separately only if you also intend to erase all tasks, backups, and preferences. Empty Trash if permanent removal is intended.
