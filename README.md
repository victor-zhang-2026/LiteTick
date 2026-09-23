# 妥了 LiteTick

桌面悬浮待办。支持 macOS 与 Windows，本地保存、无需账号、无需联网。

Floating Desktop Checklist for macOS and Windows, with local storage, no account, and no network required.

## 当前版本 / Current release

Windows 正式版为 `1.1.0`，支持 Windows 10 及以上 x64 系统，提供免安装单文件 EXE。macOS 正式版为 `1.0.1`，支持 Apple 芯片 Mac 和 macOS 14 或更高版本。

[下载 LiteTick 1.1.0（Windows 10/11 x64 免安装版）](https://github.com/victor-zhang-2026/LiteTick/releases/download/v1.1.0/LiteTick-1.1.0-Windows-x64.exe)

[下载 LiteTick 1.0.1（Apple 芯片 Mac）](https://github.com/victor-zhang-2026/LiteTick/releases/download/v1.0.1/LiteTick-1.0.1-macOS-arm64.zip)

—

The Windows stable release is `1.1.0` for Windows 10 and later on x64, distributed as one portable EXE with no installer. The macOS stable release remains `1.0.1` for Apple silicon Macs running macOS 14 or later.

[Download LiteTick 1.1.0 for Windows 10/11 x64](https://github.com/victor-zhang-2026/LiteTick/releases/download/v1.1.0/LiteTick-1.1.0-Windows-x64.exe)

[Download LiteTick 1.0.1 for Apple silicon Mac](https://github.com/victor-zhang-2026/LiteTick/releases/download/v1.0.1/LiteTick-1.0.1-macOS-arm64.zip)

## 功能 / Features

- 鼠标移到屏幕边缘的悬浮入口即可打开清单。
- 悬浮入口可自由拖动到任意已连接屏幕，并在重启后恢复位置。
- 一张清单，不设分类、标签、日期、提醒或优先级。
- 支持完成与恢复、已完成搜索、单项置顶和拖动排序。
- 支持简体中文与英文，以及手动浅色/深色外观。
- 数据只保存在本机；无账号、同步、分析、广告或遥测。

—

- Hover over the screen-edge trigger to open the checklist.
- Move the floating trigger freely to any connected display and keep its position across launches.
- One list, without categories, tags, dates, reminders, or priorities.
- Complete and restore items, search Completed, pin one item, and drag to reorder.
- Simplified Chinese and English, with manual Light and Dark appearances.
- Local-only data with no accounts, sync, analytics, advertising, or telemetry.

## 安装 / Install

### Windows

- 下载 `LiteTick-1.1.0-Windows-x64.exe`，保存到你希望长期放置的位置后直接打开；无需安装器、管理员权限或额外运行库。
- 如果 Microsoft Defender SmartScreen 阻止首次启动，请先确认文件来自本仓库的 `v1.1.0` Release，且 SHA-256 与发布页一致，再选择“更多信息 → 仍要运行”。请勿关闭 SmartScreen，也不要忽略其他来源文件的安全警告。
- 每个 Windows 用户的数据独立保存在 `%LOCALAPPDATA%\LiteTick`，通常是 `C:\Users\<用户名>\AppData\Local\LiteTick`。按 `Win + R`，输入 `%LOCALAPPDATA%\LiteTick` 可直接打开。
- 完全删除时，先退出妥了，删除 EXE，再删除当前用户的 `%LOCALAPPDATA%\LiteTick` 文件夹；需要立即永久清除时再清空回收站。

—

- Download `LiteTick-1.1.0-Windows-x64.exe`, place it where you want to keep it, and open it directly. No installer, administrator access, or additional runtime is required.
- If Microsoft Defender SmartScreen blocks the first launch, continue through `More info` → `Run anyway` only after confirming that the file came from this repository's `v1.1.0` Release and its SHA-256 matches the published value. Do not disable SmartScreen or ignore warnings for files from other sources.
- Each Windows account stores independent data under `%LOCALAPPDATA%\LiteTick`, normally `C:\Users\<user-name>\AppData\Local\LiteTick`. Press `Win + R` and enter `%LOCALAPPDATA%\LiteTick` to open it.
- For complete removal, quit LiteTick, delete the EXE, and delete the current user's `%LOCALAPPDATA%\LiteTick` folder. Empty the Recycle Bin if immediate permanent removal is intended.

### macOS

- 点击上方下载链接获取 `LiteTick-1.0.1-macOS-arm64.zip`。不要下载 GitHub 自动提供的 `Source code`，也不要使用 `Code → Download ZIP`。
- Safari 可能自动解压；Finder 也可能只显示 `LiteTick` 而隐藏 `.app` 后缀。将该应用拖入“应用程序”。
- 尝试打开一次。如果 macOS 阻止运行，请打开“系统设置 → 隐私与安全性”，在“安全性”区域选择“仍要打开”，再次确认。
- 仅当压缩包来自本仓库，且其 SHA-256 校验值与发布清单一致时，才绕过 macOS 安全限制。参见 [Apple 安全指南](https://support.apple.com/zh-cn/102445)。
- 完整的安装、升级、数据路径、迁移和卸载说明见 [安装与数据指南](docs/install-and-data.md)。

—

- Use the download link above to get `LiteTick-1.0.1-macOS-arm64.zip`. Do not download GitHub's automatic `Source code` archives or use `Code → Download ZIP`.
- Safari may unzip it automatically, and Finder may display the app as `LiteTick` without the `.app` extension. Drag the app into Applications.
- Try to open it once. If macOS blocks it, open System Settings → Privacy & Security, choose Open Anyway in Security, and confirm.
- Only override macOS security when the archive came from this repository and its SHA-256 checksum matches the release manifest. See [Apple's safety guidance](https://support.apple.com/en-us/102445).
- See [Install and data guide](docs/install-and-data.md) for upgrades, storage, migration, and complete removal.

## 使用 / Use

- 将指针移到绿色悬浮入口上，清单会立即出现。
- 点击输入框并按 Return 添加事项。
- 点击事项文字直接编辑；使用右侧手柄拖动排序。
- 点击方形勾选框完成事项；在“已完成”中再次点击可恢复。
- 点击面板可临时保持打开；工具栏图钉可在本次运行期间固定面板。
- 右键悬浮入口可退出，退出前会再次确认。macOS 也可使用 `Command-Q`。

—

- Move the pointer over the green floating trigger to show the checklist immediately.
- Click the input field and press Return to add an item.
- Click item text to edit it; drag the handle on the right to reorder.
- Click the square checkbox to complete an item; click it again in Completed to restore it.
- Click the panel to keep it open temporarily; use the toolbar pin to keep it fixed for the current run.
- Right-click the floating trigger to quit; LiteTick asks for confirmation first. On macOS, you can also use `Command-Q`.

## 隐私 / Privacy

任务以未加密 JSON 明文保存在当前 macOS 或 Windows 用户的本地目录中。请勿保存密码、支付卡号、身份证件号码或其他高度敏感信息。完整边界见 [隐私说明](docs/privacy.md)。

—

Tasks are stored as unencrypted plaintext JSON in the current macOS or Windows user's local directory. Do not store passwords, payment-card numbers, government identifiers, or other highly sensitive information. See [Privacy](docs/privacy.md).

## 从源码构建 / Build from source

macOS 需要 macOS 14 SDK 和 Swift 6：

```bash
cd apps/macos
./scripts/build-app.sh  # reserves a new public build number
```

详细规则见 [macOS 开发说明](apps/macos/README.md)与[版本制度](docs/versioning.md)。

Windows 需要 .NET 10 SDK：

```powershell
cd apps\windows
.\scripts\build.ps1
```

详细规则见 [Windows 开发说明](apps/windows/README.md)。

—

macOS requires the macOS 14 SDK and Swift 6:

```bash
cd apps/macos
./scripts/build-app.sh  # reserves a new public build number
```

See the [macOS development guide](apps/macos/README.md) and [versioning policy](docs/versioning.md).

Windows requires the .NET 10 SDK:

```powershell
cd apps\windows
.\scripts\build.ps1
```

See the [Windows development guide](apps/windows/README.md).

## 共创 / Contributing

欢迎缺陷报告、文档修正和符合产品边界的代码贡献。请先阅读 [CONTRIBUTING.md](CONTRIBUTING.md)。安全问题请按 [SECURITY.md](SECURITY.md) 私下报告。

—

Bug reports, documentation fixes, and code contributions within the product boundary are welcome. Read [CONTRIBUTING.md](CONTRIBUTING.md) first. Report security issues privately as described in [SECURITY.md](SECURITY.md).

## License

[MIT License](LICENSE) — Copyright (c) 2026 Victor Zhang.
