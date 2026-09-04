# 妥了 LiteTick 1.0.1

跨屏拖动修复版本，支持 Apple 芯片 Mac 和 macOS 14 或更高版本。

—

Cross-display dragging fix for Apple silicon Macs running macOS 14 or later.

## 修复 / Fixed

- 悬浮入口现在可以自由拖动到任意已连接屏幕的任意可见位置。
- 清单会在悬浮入口所在的屏幕内正确展开。
- 副屏位置会在退出后保留，并在下次启动时恢复；如果该屏幕已经断开，入口会回到最近的可用屏幕。

—

- The floating trigger can now be moved freely to any visible position on any connected display.
- The checklist opens within the display containing the trigger.
- Secondary-display placement persists across launches; if that display is disconnected, the trigger recovers onto the nearest available display.

## 升级 / Upgrade

- 退出 LiteTick，下载 `LiteTick-1.0.1-macOS-arm64.zip`，解压后用新版 `LiteTick.app` 替换“应用程序”中的旧版本。
- 任务数据独立保存在 `~/Library/Application Support/LiteTick`，替换应用不会删除任务。

—

- Quit LiteTick, download `LiteTick-1.0.1-macOS-arm64.zip`, unzip it, and replace the previous `LiteTick.app` in Applications.
- Tasks remain independently stored in `~/Library/Application Support/LiteTick`; replacing the app does not remove them.

## 发布说明 / Distribution note

- 此版本采用临时本地签名，尚未经过 Apple Developer ID 签名或公证。macOS 可能阻止首次启动。请先核对压缩包 SHA-256 与 `SHA256SUMS` 一致，再按仓库安装指南操作。

—

- This release is ad-hoc signed and is not yet signed with Apple Developer ID or notarized. macOS may block the first launch. Follow the repository's install guide only after confirming that the archive SHA-256 matches `SHA256SUMS`.
