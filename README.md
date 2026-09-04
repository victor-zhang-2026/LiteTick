# 妥了 LiteTick

桌面悬浮待办。macOS 原生、本地保存、无需账号、无需联网。

Floating Desktop Checklist. Native macOS, local storage, no account, and no network required.

## 当前版本 / Current release

`1.0.1` 是当前正式版本，面向 Apple 芯片 Mac，要求 macOS 14 或更高版本。当前下载包采用临时本地签名，尚未经过 Apple Developer ID 签名或公证。

[下载 LiteTick 1.0.1（Apple 芯片 Mac）](https://github.com/victor-zhang-2026/LiteTick/releases/download/v1.0.1/LiteTick-1.0.1-macOS-arm64.zip)

—

`1.0.1` is the current stable release for Apple silicon Macs running macOS 14 or later. The current download uses ad-hoc local signing and is not yet signed with Developer ID or notarized by Apple.

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
- 右键悬浮入口可退出；`Command-Q` 同样可用，退出前会再次确认。

—

- Move the pointer over the green floating trigger to show the checklist immediately.
- Click the input field and press Return to add an item.
- Click item text to edit it; drag the handle on the right to reorder.
- Click the square checkbox to complete an item; click it again in Completed to restore it.
- Click the panel to keep it open temporarily; use the toolbar pin to keep it fixed for the current run.
- Right-click the floating trigger to quit. `Command-Q` also works and always asks for confirmation.

## 隐私 / Privacy

任务以未加密 JSON 明文保存在当前 Mac 用户目录中。请勿保存密码、支付卡号、身份证件号码或其他高度敏感信息。完整边界见 [隐私说明](docs/privacy.md)。

Tasks are stored as unencrypted plaintext JSON in the current Mac user's directory. Do not store passwords, payment-card numbers, government identifiers, or other highly sensitive information. See [Privacy](docs/privacy.md).

## 从源码构建 / Build from source

需要 macOS 14 SDK 和 Swift 6。

```bash
cd apps/macos
./scripts/build-app.sh  # reserves a new public build number
```

详细规则见 [macOS 开发说明](apps/macos/README.md)与[版本制度](docs/versioning.md)。

## 共创 / Contributing

欢迎缺陷报告、文档修正和符合产品边界的代码贡献。请先阅读 [CONTRIBUTING.md](CONTRIBUTING.md)。安全问题请按 [SECURITY.md](SECURITY.md) 私下报告。

Bug reports, documentation fixes, and code contributions within the product boundary are welcome. Read [CONTRIBUTING.md](CONTRIBUTING.md) first. Report security issues privately as described in [SECURITY.md](SECURITY.md).

## License

[MIT License](LICENSE) — Copyright (c) 2026 Victor Zhang.
