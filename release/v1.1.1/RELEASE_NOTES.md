# 妥了 LiteTick 1.1.1

macOS 窗口行为修复版本，支持 Apple 芯片 Mac 和 macOS 14 或更高版本。

—

macOS window-behavior fixes for Apple silicon Macs running macOS 14 or later.

## 修复 / Fixed

- 未锁定的清单现在遵循 macOS 普通窗口层级。切换到其他应用后，其他应用窗口可以自然覆盖清单。
- 主动开启“锁定面板”后，清单仍会保持置顶；悬浮入口也继续保持可见。
- 修正单行任务文字与勾选框的垂直对齐。

—

- The unlocked checklist now follows normal macOS window ordering, allowing another application's windows to cover it after focus changes.
- The checklist remains always on top when Panel Lock is explicitly enabled, and the floating trigger stays accessible.
- Corrected the vertical alignment between single-line task text and its completion control.

## 升级 / Upgrade

- 退出 LiteTick，下载 `LiteTick-1.1.1-macOS-arm64.zip`，解压后用新版 `LiteTick.app` 替换“应用程序”中的旧版本。
- 任务数据独立保存在 `~/Library/Application Support/LiteTick`，替换应用不会删除任务。

—

- Quit LiteTick, download `LiteTick-1.1.1-macOS-arm64.zip`, unzip it, and replace the previous `LiteTick.app` in Applications.
- Tasks remain independently stored in `~/Library/Application Support/LiteTick`; replacing the app does not remove them.

## 发布说明 / Distribution note

- 此版本采用临时本地签名，尚未经过 Apple Developer ID 签名或公证。macOS 可能阻止首次启动。请先核对压缩包 SHA-256 与 `SHA256SUMS` 一致，再按仓库安装指南操作。

—

- This release is ad-hoc signed and is not yet signed with Apple Developer ID or notarized. macOS may block the first launch. Follow the repository's install guide only after confirming that the archive SHA-256 matches `SHA256SUMS`.
