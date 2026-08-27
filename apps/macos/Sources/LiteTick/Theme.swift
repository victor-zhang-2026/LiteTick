import AppKit
import SwiftUI

enum AppearanceMode: String, CaseIterable {
    case light
    case dark

    var colorScheme: ColorScheme {
        switch self {
        case .light: .light
        case .dark: .dark
        }
    }

    var symbolName: String {
        switch self {
        case .light: "sun.max"
        case .dark: "moon"
        }
    }
}

extension NSColor {
    static let liteTickGreen = NSColor(name: nil) { appearance in
        appearance.bestMatch(from: [.darkAqua, .aqua]) == .darkAqua
            ? NSColor(red: 24 / 255, green: 201 / 255, blue: 110 / 255, alpha: 1)
            : NSColor(red: 7 / 255, green: 193 / 255, blue: 96 / 255, alpha: 1)
    }
}

extension Color {
    static let liteTickGreen = Color(nsColor: .liteTickGreen)
    static let liteTickDeepGreen = Color(nsColor: NSColor(name: nil) { appearance in
        appearance.bestMatch(from: [.darkAqua, .aqua]) == .darkAqua
            ? NSColor(red: 57 / 255, green: 217 / 255, blue: 137 / 255, alpha: 1)
            : NSColor(red: 5 / 255, green: 138 / 255, blue: 69 / 255, alpha: 1)
    })
    static let liteTickSoftGreen = Color(nsColor: NSColor(name: nil) { appearance in
        appearance.bestMatch(from: [.darkAqua, .aqua]) == .darkAqua
            ? NSColor(red: 23 / 255, green: 77 / 255, blue: 48 / 255, alpha: 1)
            : NSColor(red: 214 / 255, green: 245 / 255, blue: 227 / 255, alpha: 1)
    })

    static let liteTickSurface = Color(nsColor: .windowBackgroundColor)
    static let liteTickElevated = Color(nsColor: .controlBackgroundColor)
    static let liteTickSeparator = Color(nsColor: .separatorColor)
    static let liteTickPrimaryText = Color(nsColor: .labelColor)
    static let liteTickSecondaryText = Color(nsColor: .secondaryLabelColor)
}
