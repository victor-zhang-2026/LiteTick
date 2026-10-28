# 妥了 LiteTick 1.2.2 — macOS

本次仅更新 Apple 芯片 Mac 版本，支持 macOS 14 或更高版本。Windows 版继续使用 [1.1.0](https://github.com/victor-zhang-2026/LiteTick/releases/tag/v1.1.0)。

This update is for Apple silicon Macs running macOS 14 or later. The Windows release remains [1.1.0](https://github.com/victor-zhang-2026/LiteTick/releases/tag/v1.1.0).

## 修复 / Fixed

- Mac 删除确认框中的“取消”和“删除”支持点击整个按钮区域，包括文字周围的留白。
- 中英文界面同步修复，适用于单条删除、全部已完成删除和搜索结果删除。
- The full Cancel and Delete button areas now respond to clicks, including the space around their labels.
- Fixed in Simplified Chinese and English for single-item, all-completed, and search-result deletion.

## 升级 / Upgrade

- 退出 LiteTick，下载 `LiteTick-1.2.2-macOS-arm64.zip`，解压后替换“应用程序”中的旧版 `LiteTick.app`。替换应用不会删除当前用户的任务。
- Quit LiteTick, download `LiteTick-1.2.2-macOS-arm64.zip`, unzip it, and replace the old `LiteTick.app` in Applications. Replacing the app preserves the current user's tasks.

## 数据与卸载 / Data and removal

- 当前 macOS 用户的任务和备份在 `~/Library/Application Support/LiteTick/`，通常对应 `/Users/<用户名>/Library/Application Support/LiteTick/`；偏好设置在 `~/Library/Preferences/io.github.victor-zhang-2026.litetick.plist`。Finder 中使用“前往 → 前往文件夹…”可打开这些隐藏路径。完整卸载需先退出应用，再删除应用及当前用户的上述两个位置。
- The current macOS user's tasks and backup are in `~/Library/Application Support/LiteTick/`, normally `/Users/<user-name>/Library/Application Support/LiteTick/`; preferences are in `~/Library/Preferences/io.github.victor-zhang-2026.litetick.plist`. Use Finder → Go → Go to Folder… to open these hidden paths. For complete removal, quit the app and delete the app plus both current-user locations.

## 发布说明 / Distribution note

- 此版本采用临时本地签名，尚未经过 Apple Developer ID 签名或公证。首次启动如被 macOS 阻止，请先核对压缩包 SHA-256 与 `SHA256SUMS` 一致，再按仓库安装指南操作。
- This release is ad-hoc signed and is not notarized. If macOS blocks its first launch, verify the archive against `SHA256SUMS` before following the repository's install guide.

## SHA-256

```text
91898840d4aefb80a200be078aeaa9b5f849d005327418b1801ded5edf71ce8d  LiteTick-1.2.2-macOS-arm64.zip
```
