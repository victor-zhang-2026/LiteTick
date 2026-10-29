import AppKit

/// Display-local positions survive clamshell mode and changes to the desktop origin.
@MainActor
final class FloatingPlacement {
    private let defaults: UserDefaults
    private let key = "floatingPlacementV1"
    private var pending: DispatchWorkItem?
    init(defaults: UserDefaults = .standard) { self.defaults = defaults }

    private func identifier(_ screen: NSScreen) -> String {
        let number = screen.deviceDescription[NSDeviceDescriptionKey("NSScreenNumber")] as? NSNumber
        if let number, let uuid = CGDisplayCreateUUIDFromDisplayID(number.uint32Value)?.takeRetainedValue() {
            return CFUUIDCreateString(nil, uuid) as String
        }
        return number?.stringValue ?? screen.localizedName
    }

    private func bounds(_ screen: NSScreen, size: NSSize) -> NSRect {
        let visible = screen.visibleFrame.insetBy(dx: 8, dy: 8)
        return NSRect(x: visible.minX, y: visible.minY,
                      width: max(0, visible.width - size.width), height: max(0, visible.height - size.height))
    }

    func remember(_ window: NSWindow) {
        let center = NSPoint(x: window.frame.midX, y: window.frame.midY)
        guard let screen = NSScreen.screens.first(where: { $0.frame.contains(center) }) ?? window.screen else { return }
        let area = bounds(screen, size: window.frame.size)
        let id = identifier(screen)
        var state = defaults.dictionary(forKey: key) ?? [:]
        var positions = state["positions"] as? [String: [String: Double]] ?? [:]
        positions[id] = ["x": Double(min(1, max(0, (window.frame.minX - area.minX) / max(1, area.width)))),
                         "y": Double(min(1, max(0, (window.frame.minY - area.minY) / max(1, area.height))))]
        state["positions"] = positions; state["preferred"] = id
        defaults.set(state, forKey: key)
    }

    func restore(_ window: NSWindow, fallback: NSPoint) {
        let screens = NSScreen.screens
        guard !screens.isEmpty else { return }
        guard let state = defaults.dictionary(forKey: key), let preferred = state["preferred"] as? String,
              let positions = state["positions"] as? [String: [String: Double]], let source = positions[preferred] else {
            window.setFrameOrigin(fallback)
            remember(window)
            return
        }
        let screen = screens.first(where: { identifier($0) == preferred }) ?? NSScreen.main ?? screens[0]
        let position = positions[identifier(screen)] ?? source
        guard let x = position["x"], let y = position["y"], x.isFinite, y.isFinite else { return }
        let area = bounds(screen, size: window.frame.size)
        window.setFrameOrigin(NSPoint(x: area.minX + CGFloat(min(1, max(0, x))) * area.width,
                                      y: area.minY + CGFloat(min(1, max(0, y))) * area.height))
        // Automatic relocation never overwrites the user's preferred display.
    }

    func afterScreenChange(_ action: @escaping @MainActor () -> Void) {
        pending?.cancel()
        let work = DispatchWorkItem { action() }
        pending = work
        DispatchQueue.main.asyncAfter(deadline: .now() + 0.4, execute: work)
    }
}
