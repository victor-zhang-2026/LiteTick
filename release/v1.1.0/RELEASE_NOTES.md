# 妥了 LiteTick 1.1.0

首次 Windows 正式版，支持 Windows 10 及以上 x64 系统。免安装、单文件、本地保存、无需账号、无需联网。

—

The first stable Windows release for x64 PCs running Windows 10 or later. Portable, single-file, local-only, account-free, and network-free.

## Windows 功能 / Windows features

- 屏幕边缘悬浮入口，鼠标移入立即展开，可跨显示器拖动并记住位置。
- 单一清单，支持新增、编辑、完成与恢复、单项置顶和拖动排序。
- 已完成事项支持搜索、单项删除、删除搜索结果和全部删除。
- 简体中文与英文、浅色与深色外观，界面状态与确认弹窗跟随应用语言。
- 任务、备份和设置只保存在当前 Windows 用户的本地目录中。

—

- Screen-edge floating trigger with immediate Hover opening, cross-display dragging, and saved placement.
- One checklist with capture, editing, completion and restore, one pinned item, and drag reordering.
- Completed-item search, individual deletion, search-result deletion, and complete deletion.
- Simplified Chinese and English plus Light and Dark appearances; app-owned controls and confirmations follow the selected app language.
- Tasks, backup, and settings stay only in the current Windows user's local directory.

## 下载与首次启动 / Download and first launch

- 下载 `LiteTick-1.1.0-Windows-x64.exe`，放到希望长期保存的位置后直接打开；无需安装器、管理员权限或额外运行库。
- 此版本尚未使用受信任的 Windows 代码签名证书。Microsoft Defender SmartScreen 可能阻止首次启动。
- 仅当 EXE 来自本仓库的 `v1.1.0` Release，且 SHA-256 与本页一致时，选择“更多信息 → 仍要运行”。不要关闭 SmartScreen，也不要对其他来源的文件忽略安全警告。

—

- Download `LiteTick-1.1.0-Windows-x64.exe`, keep it in a permanent location, and open it directly. No installer, administrator access, or additional runtime is required.
- This release is not yet signed with a trusted Windows code-signing certificate. Microsoft Defender SmartScreen may block the first launch.
- Use `More info` → `Run anyway` only after confirming that the EXE came from this repository's `v1.1.0` Release and its SHA-256 matches this page. Do not disable SmartScreen or ignore warnings for files from other sources.

## 数据位置 / Data location

每个 Windows 用户的数据彼此独立：

```text
%LOCALAPPDATA%\LiteTick
C:\Users\<用户名>\AppData\Local\LiteTick
```

按 `Win + R`，输入 `%LOCALAPPDATA%\LiteTick` 并回车，可直接打开当前用户的数据目录。目录包含 `items.json`、`items.backup.json` 和 `settings.json`。替换或删除 EXE 不会删除这些文件。

—

Each Windows account has independent data:

```text
%LOCALAPPDATA%\LiteTick
C:\Users\<user-name>\AppData\Local\LiteTick
```

Press `Win + R`, enter `%LOCALAPPDATA%\LiteTick`, and press Enter to open the current user's data directory. It contains `items.json`, `items.backup.json`, and `settings.json`. Replacing or deleting the EXE does not remove these files.

## 彻底删除 / Complete removal

1. 从悬浮入口右键菜单退出妥了。
2. 删除 `LiteTick-1.1.0-Windows-x64.exe`。
3. 按 `Win + R`，输入 `%LOCALAPPDATA%`，删除整个 `LiteTick` 文件夹。
4. 如需立即永久清除，再清空回收站。

—

1. Quit LiteTick from the floating trigger's context menu.
2. Delete `LiteTick-1.1.0-Windows-x64.exe`.
3. Press `Win + R`, enter `%LOCALAPPDATA%`, and delete the complete `LiteTick` folder.
4. Empty the Recycle Bin if immediate permanent removal is intended.

## SHA-256

```text
b1bfab8038186e1ed074e9e3201fdfc94d0c42c393452f1e60a23ed4e5ba6927  LiteTick-1.1.0-Windows-x64.exe
```
