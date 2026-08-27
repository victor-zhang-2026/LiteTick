import AppKit
import SwiftUI

@MainActor
final class AboutWindowController: NSWindowController, NSWindowDelegate {
    init() {
        let window = NSWindow(
            contentRect: NSRect(x: 0, y: 0, width: 360, height: 340),
            styleMask: [.titled, .closable],
            backing: .buffered,
            defer: false
        )
        window.title = L10n.aboutTitle
        window.isReleasedWhenClosed = false
        window.level = FloatingWindowController.alwaysOnTopLevel
        window.collectionBehavior = [.canJoinAllSpaces, .fullScreenAuxiliary]
        window.contentView = NSHostingView(rootView: AboutView())
        super.init(window: window)
        window.delegate = self
    }

    @available(*, unavailable)
    required init?(coder: NSCoder) { nil }

    func present() {
        guard let window else { return }
        refresh()
        window.center()
        NSApplication.shared.activate(ignoringOtherApps: true)
        window.makeKeyAndOrderFront(nil)
    }

    func refresh() {
        guard let window else { return }
        window.title = L10n.aboutTitle
        window.contentView = NSHostingView(rootView: AboutView())
    }
}

private struct AboutView: View {
    private var version: String {
        Bundle.main.object(forInfoDictionaryKey: "CFBundleShortVersionString") as? String ?? "—"
    }

    private var build: String {
        Bundle.main.object(forInfoDictionaryKey: "CFBundleVersion") as? String ?? "—"
    }

    private var channel: String {
        Bundle.main.object(forInfoDictionaryKey: "LiteTickReleaseChannel") as? String ?? "development"
    }

    private let repositoryURL = URL(string: "https://github.com/victor-zhang-2026/litetick")!
    private let privacyURL = URL(string: "https://github.com/victor-zhang-2026/litetick/blob/main/docs/privacy.md")!
    private let licenseURL = URL(string: "https://github.com/victor-zhang-2026/litetick/blob/main/LICENSE")!

    var body: some View {
        VStack(spacing: 12) {
            Image(nsImage: AppIcon.alertImage)
                .resizable()
                .interpolation(.high)
                .frame(width: 88, height: 88)

            VStack(spacing: 3) {
                if L10n.isChinese {
                    Text(ProductIdentity.chineseName)
                        .font(.system(size: 22, weight: .semibold))
                    Text(ProductIdentity.englishName)
                        .font(.system(size: 14, weight: .medium))
                        .foregroundStyle(.secondary)
                } else {
                    Text(ProductIdentity.englishName)
                        .font(.system(size: 22, weight: .semibold))
                }
            }

            Text(L10n.aboutDescription)
                .font(.system(size: 13))
                .foregroundStyle(.secondary)

            Text("\(version) (\(build)) · \(channel.uppercased())")
                .font(.system(size: 12, design: .monospaced))
                .foregroundStyle(.secondary)

            HStack(spacing: 18) {
                Link(L10n.github, destination: repositoryURL)
                Link(L10n.privacy, destination: privacyURL)
                Link(L10n.license, destination: licenseURL)
            }
            .font(.system(size: 12))
        }
        .frame(maxWidth: .infinity, maxHeight: .infinity)
        .padding(28)
    }
}
