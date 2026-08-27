// swift-tools-version: 6.0
import PackageDescription

let package = Package(
    name: "LiteTick",
    platforms: [.macOS(.v14)],
    products: [.executable(name: "LiteTick", targets: ["LiteTick"])],
    targets: [
        .executableTarget(
            name: "LiteTick",
            path: "Sources/LiteTick"
        )
    ]
)
