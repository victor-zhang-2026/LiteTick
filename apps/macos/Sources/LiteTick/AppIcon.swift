import AppKit

@MainActor
enum AppIcon {
    static var image: NSImage {
        if let url = Bundle.main.url(forResource: "LiteTick", withExtension: "icns"),
           let image = NSImage(contentsOf: url) {
            return image
        }
        return NSApplication.shared.applicationIconImage
    }

    static var alertImage: NSImage {
        if let url = Bundle.main.url(forResource: "LiteTickAlertIcon", withExtension: "png"),
           let image = NSImage(contentsOf: url) {
            return image
        }
        return image
    }
}
