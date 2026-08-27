import AppKit

@MainActor
final class FloatingBallView: NSView {
    private weak var controller: FloatingWindowController?
    private var trackingAreaRef: NSTrackingArea?
    private var isHovering = false
    private var mouseDownPoint: NSPoint?
    private var lastDragPoint: NSPoint?
    private var didDrag = false

    init(controller: FloatingWindowController) {
        self.controller = controller
        super.init(frame: .zero)
        wantsLayer = true
        layer?.backgroundColor = NSColor.clear.cgColor
        layer?.isOpaque = false
    }

    @available(*, unavailable)
    required init?(coder: NSCoder) { fatalError("init(coder:) has not been implemented") }

    override var isOpaque: Bool { false }
    override var acceptsFirstResponder: Bool { true }
    override func acceptsFirstMouse(for event: NSEvent?) -> Bool { true }

    override func updateTrackingAreas() {
        super.updateTrackingAreas()
        if let trackingAreaRef { removeTrackingArea(trackingAreaRef) }
        let area = NSTrackingArea(
            rect: bounds,
            options: [.mouseEnteredAndExited, .activeAlways, .inVisibleRect],
            owner: self,
            userInfo: nil
        )
        addTrackingArea(area)
        trackingAreaRef = area
    }

    override func draw(_ dirtyRect: NSRect) {
        NSColor.clear.setFill()
        dirtyRect.fill()

        let outerInset: CGFloat = 3
        let outerCircle = NSBezierPath(ovalIn: bounds.insetBy(dx: outerInset, dy: outerInset))
        let shadow = NSShadow()
        shadow.shadowColor = NSColor.black.withAlphaComponent(isHovering ? 0.20 : 0.14)
        shadow.shadowBlurRadius = isHovering ? 5 : 4
        shadow.shadowOffset = NSSize(width: 0, height: -1)
        NSGraphicsContext.saveGraphicsState()
        shadow.set()
        NSColor.white.setFill()
        outerCircle.fill()
        NSGraphicsContext.restoreGraphicsState()

        let innerInset: CGFloat = isHovering ? 6 : 7
        let circleRect = bounds.insetBy(dx: innerInset, dy: innerInset)
        let circle = NSBezierPath(ovalIn: circleRect)
        NSColor(calibratedRed: isHovering ? 5/255 : 7/255, green: isHovering ? 154/255 : 193/255, blue: isHovering ? 75/255 : 96/255, alpha: 1).setFill()
        circle.fill()

        let check = NSBezierPath()
        check.lineWidth = 4.2
        check.lineCapStyle = .round
        check.lineJoinStyle = .round
        check.move(to: NSPoint(x: bounds.width * 0.30, y: bounds.height * 0.52))
        check.line(to: NSPoint(x: bounds.width * 0.44, y: bounds.height * 0.38))
        check.line(to: NSPoint(x: bounds.width * 0.70, y: bounds.height * 0.66))
        NSColor.white.setStroke()
        check.stroke()
    }

    override func mouseEntered(with event: NSEvent) {
        isHovering = true
        needsDisplay = true
        controller?.scheduleOpen()
    }

    override func mouseExited(with event: NSEvent) {
        isHovering = false
        needsDisplay = true
        controller?.triggerExited()
    }

    override func mouseDown(with event: NSEvent) {
        mouseDownPoint = event.locationInWindow
        lastDragPoint = NSEvent.mouseLocation
        didDrag = false
    }

    override func mouseDragged(with event: NSEvent) {
        guard let previous = lastDragPoint else { return }
        let current = NSEvent.mouseLocation
        let translation = CGSize(width: current.x - previous.x, height: previous.y - current.y)
        if hypot(current.x - previous.x, current.y - previous.y) > 0.5 { didDrag = true }
        controller?.moveTrigger(by: translation, ended: false)
        lastDragPoint = current
    }

    override func mouseUp(with event: NSEvent) {
        if didDrag { controller?.moveTrigger(by: .zero, ended: true) }
        else { controller?.openPinned() }
        mouseDownPoint = nil
        lastDragPoint = nil
        didDrag = false
    }

    override func rightMouseDown(with event: NSEvent) {
        let menu = NSMenu()
        let quit = NSMenuItem(title: L10n.quit, action: #selector(quitApplication), keyEquivalent: "")
        quit.target = self
        menu.addItem(quit)
        NSMenu.popUpContextMenu(menu, with: event, for: self)
    }

    @objc private func quitApplication() { controller?.quit() }
}
