import AppKit
import SwiftUI

extension Notification.Name {
    static let liteTickPanelWillClose = Notification.Name("LiteTickPanelWillClose")
    static let liteTickPanelDidOpen = Notification.Name("LiteTickPanelDidOpen")
}

@MainActor
final class FloatingWindowController: NSObject, NSWindowDelegate {
    private let store: TaskStore
    private var triggerPanel: FloatingPanel!
    private var listPanel: FloatingPanel!
    private var openWorkItem: DispatchWorkItem?
    private var closeWorkItem: DispatchWorkItem?
    private var isTriggerHovered = false
    private var isListHovered = false
    private var isPinned = false
    private var mouseMonitor: Any?
    private var outsideClickMonitor: Any?
    private var isAppearanceTransitioning = false
    private var aboutWindowController: AboutWindowController?
    private(set) var isOpen = false
    private(set) var isPanelLocked = false

    // The visible ball remains 42 pt; the extra transparent space prevents its circular shadow from clipping.
    private let triggerSize = NSSize(width: 48, height: 48)
    private let panelSize = NSSize(width: 360, height: 480)
    private let panelTransitionDuration: TimeInterval = 0.12
    // Some conferencing speaker overlays sit above the screen-saver level. Use the
    // system's high assistive-overlay level without entering cursor-reserved levels.
    static let alwaysOnTopLevel = NSWindow.Level(
        rawValue: Int(CGWindowLevelForKey(.assistiveTechHighWindow))
    )

    init(store: TaskStore) {
        self.store = store
        super.init()
        // Pinning is deliberately session-only. Clear preferences left by earlier prototype builds.
        UserDefaults.standard.removeObject(forKey: "panelLocked")
        configurePanels()
    }

    func showTrigger() {
        positionTrigger(at: savedTriggerOrigin())
        triggerPanel.orderFrontRegardless()
    }

    func togglePanel() {
        isOpen ? dismissPanel() : openPanel(activate: true)
    }

    func dismissPanel() {
        if isPanelLocked {
            isPanelLocked = false
        }
        isPinned = false
        closePanel()
    }

    func openPinned() {
        isPinned = true
        openPanel(activate: true)
    }

    func pinPanel() {
        guard isOpen else { return }
        isPinned = true
        cancelClose()
    }

    @discardableResult
    func togglePanelLock() -> Bool {
        isPanelLocked.toggle()
        if isPanelLocked {
            isPinned = true
            cancelClose()
        } else if !isTriggerHovered && !isListHovered {
            scheduleClose()
        }
        return isPanelLocked
    }

    func scheduleOpen() {
        isTriggerHovered = true
        cancelClose()
        guard !isOpen else { return }
        openWorkItem?.cancel()
        openPanel(activate: true)
    }

    func triggerExited() {
        isTriggerHovered = false
        openWorkItem?.cancel()
        scheduleClose()
    }

    func listHoverChanged(_ hovering: Bool) {
        isListHovered = hovering
        hovering ? cancelClose() : scheduleClose()
    }

    func moveTrigger(by translation: CGSize, ended: Bool) {
        guard let screen = screenContainingTrigger() ?? NSScreen.main else { return }
        let frame = screen.visibleFrame
        let current = triggerPanel.frame.origin
        let proposed = NSPoint(x: current.x + translation.width, y: current.y - translation.height)
        let clamped = NSPoint(
            x: min(max(proposed.x, frame.minX + 8), frame.maxX - triggerSize.width - 8),
            y: min(max(proposed.y, frame.minY + 8), frame.maxY - triggerSize.height - 8)
        )
        triggerPanel.setFrameOrigin(clamped)
        if ended {
            UserDefaults.standard.set(Double(clamped.x), forKey: "triggerX")
            UserDefaults.standard.set(Double(clamped.y), forKey: "triggerY")
            if isOpen { positionListPanel() }
        }
    }

    func quit() { NSApplication.shared.terminate(nil) }

    func showAbout() {
        if aboutWindowController == nil {
            aboutWindowController = AboutWindowController()
        }
        aboutWindowController?.present()
    }

    func refreshAbout() {
        aboutWindowController?.refresh()
    }

    func transitionAppearance(_ change: @escaping @MainActor () -> Void) {
        guard !isAppearanceTransitioning, let contentView = listPanel.contentView else { return }
        let bounds = contentView.bounds
        guard bounds.width > 0, bounds.height > 0, let oldImage = snapshot(of: contentView) else {
            change()
            return
        }

        let oldSnapshot = NSImageView(frame: bounds)
        oldSnapshot.image = oldImage
        oldSnapshot.imageScaling = .scaleAxesIndependently
        contentView.addSubview(oldSnapshot)

        isAppearanceTransitioning = true
        change()

        DispatchQueue.main.async { [weak self, weak oldSnapshot] in
            guard let self, let oldSnapshot else { return }
            NSAnimationContext.runAnimationGroup({ context in
                context.duration = 0.26
                context.timingFunction = CAMediaTimingFunction(name: .easeInEaseOut)
                oldSnapshot.animator().alphaValue = 0
            }, completionHandler: {
                Task { @MainActor in
                    oldSnapshot.removeFromSuperview()
                    self.isAppearanceTransitioning = false
                }
            })
        }
    }

    private func snapshot(of view: NSView) -> NSImage? {
        let bounds = view.bounds
        guard let bitmap = view.bitmapImageRepForCachingDisplay(in: bounds) else { return nil }
        view.cacheDisplay(in: bounds, to: bitmap)
        let image = NSImage(size: bounds.size)
        image.addRepresentation(bitmap)
        return image
    }

    private func configurePanels() {
        triggerPanel = FloatingPanel(
            contentRect: NSRect(origin: .zero, size: triggerSize),
            styleMask: [.borderless, .nonactivatingPanel],
            backing: .buffered,
            defer: false
        )
        prepare(triggerPanel)
        triggerPanel.becomesKeyOnlyIfNeeded = true
        triggerPanel.hasShadow = false
        let triggerView = FloatingBallView(controller: self)
        triggerView.wantsLayer = true
        triggerView.layer?.backgroundColor = NSColor.clear.cgColor
        triggerView.layer?.isOpaque = false
        triggerPanel.contentView = triggerView

        listPanel = FloatingPanel(
            contentRect: NSRect(origin: .zero, size: panelSize),
            styleMask: [.borderless],
            backing: .buffered,
            defer: false
        )
        prepare(listPanel)
        listPanel.hasShadow = true
        listPanel.delegate = self
        listPanel.contentView = NSHostingView(rootView: ChecklistView(store: store, controller: self))
        mouseMonitor = NSEvent.addLocalMonitorForEvents(matching: [.leftMouseDown, .rightMouseDown]) { [weak self] event in
            guard let self else { return event }
            if event.window === self.listPanel {
                self.isPinned = true
                if event.type == .leftMouseDown, let editor = self.listPanel.firstResponder as? NSTextView {
                    let pointInEditor = editor.convert(event.locationInWindow, from: nil)
                    if !editor.bounds.contains(pointInEditor) {
                        self.listPanel.makeFirstResponder(nil)
                    }
                }
            }
            return event
        }
        outsideClickMonitor = NSEvent.addGlobalMonitorForEvents(matching: [.leftMouseDown, .rightMouseDown]) { [weak self] _ in
            Task { @MainActor in
                guard let self, self.isPinned, !self.isPanelLocked else { return }
                self.listPanel.makeFirstResponder(nil)
                self.dismissPanel()
            }
        }
    }

    private func prepare(_ panel: NSPanel) {
        panel.level = Self.alwaysOnTopLevel
        panel.collectionBehavior = [.canJoinAllSpaces, .fullScreenAuxiliary, .stationary]
        panel.animationBehavior = .none
        panel.isOpaque = false
        panel.backgroundColor = .clear
        panel.hidesOnDeactivate = false
        panel.isReleasedWhenClosed = false
    }

    private func openPanel(activate: Bool) {
        openWorkItem?.cancel()
        isOpen = true
        positionListPanel()
        listPanel.alphaValue = 0
        listPanel.orderFrontRegardless()
        if activate {
            NSApplication.shared.activate(ignoringOtherApps: true)
            listPanel.makeKey()
            listPanel.makeFirstResponder(nil)
            NotificationCenter.default.post(name: .liteTickPanelDidOpen, object: nil)
        }
        NSAnimationContext.runAnimationGroup { context in
            context.duration = panelTransitionDuration
            context.timingFunction = CAMediaTimingFunction(name: .easeInEaseOut)
            listPanel.animator().alphaValue = 1
        }
    }

    private func closePanel() {
        openWorkItem?.cancel()
        closeWorkItem?.cancel()
        NotificationCenter.default.post(name: .liteTickPanelWillClose, object: nil)
        listPanel.makeFirstResponder(nil)
        isOpen = false
        NSAnimationContext.runAnimationGroup({ context in
            context.duration = panelTransitionDuration
            context.timingFunction = CAMediaTimingFunction(name: .easeInEaseOut)
            listPanel.animator().alphaValue = 0
        }, completionHandler: { [weak self] in
            Task { @MainActor in
                guard let self, !self.isOpen else { return }
                self.listPanel.orderOut(nil)
                self.listPanel.alphaValue = 1
            }
        })
    }

    private func scheduleClose() {
        cancelClose()
        let work = DispatchWorkItem { [weak self] in
            guard let self, !self.isPanelLocked, !self.isPinned, !self.isTriggerHovered, !self.isListHovered else { return }
            self.closePanel()
        }
        closeWorkItem = work
        DispatchQueue.main.asyncAfter(deadline: .now() + 0.3, execute: work)
    }

    private func cancelClose() {
        closeWorkItem?.cancel()
        closeWorkItem = nil
    }

    private func positionTrigger(at origin: NSPoint) {
        triggerPanel.setFrameOrigin(origin)
    }

    private func positionListPanel() {
        guard let screen = screenContainingTrigger() ?? NSScreen.main else { return }
        let visible = screen.visibleFrame
        let trigger = triggerPanel.frame
        let spaceOnLeft = trigger.minX - visible.minX
        let x = spaceOnLeft >= panelSize.width
            ? trigger.minX - panelSize.width
            : trigger.maxX
        let centeredY = trigger.midY - panelSize.height / 2
        let y = min(max(centeredY, visible.minY + 12), visible.maxY - panelSize.height - 12)
        listPanel.setFrame(NSRect(x: x, y: y, width: panelSize.width, height: panelSize.height), display: true)
    }

    private func screenContainingTrigger() -> NSScreen? {
        let center = NSPoint(x: triggerPanel.frame.midX, y: triggerPanel.frame.midY)
        return NSScreen.screens.first { NSMouseInRect(center, $0.frame, false) }
    }

    private func savedTriggerOrigin() -> NSPoint {
        guard let screen = NSScreen.main else { return NSPoint(x: 800, y: 300) }
        let frame = screen.visibleFrame
        if UserDefaults.standard.object(forKey: "triggerX") != nil {
            let x = CGFloat(UserDefaults.standard.double(forKey: "triggerX"))
            let y = CGFloat(UserDefaults.standard.double(forKey: "triggerY"))
            return NSPoint(
                x: min(max(x, frame.minX + 8), frame.maxX - triggerSize.width - 8),
                y: min(max(y, frame.minY + 8), frame.maxY - triggerSize.height - 8)
            )
        }
        return NSPoint(x: frame.maxX - triggerSize.width - 10, y: frame.midY - triggerSize.height / 2)
    }
}

final class FloatingPanel: NSPanel {
    override var canBecomeKey: Bool { true }
    override var canBecomeMain: Bool { false }
}
