import Foundation
import Observation

@MainActor
@Observable
final class TaskStore {
    private(set) var items: [TaskItem] = []
    var recentlyCompletedID: UUID?

    private let fileURL: URL

    init(fileURL: URL? = nil) {
        self.fileURL = fileURL ?? Self.defaultFileURL()
        load()
    }

    var unfinished: [TaskItem] {
        items.filter { !$0.isCompleted }.sorted { lhs, rhs in
            if lhs.isPinned != rhs.isPinned { return lhs.isPinned }
            if lhs.sortOrder == rhs.sortOrder { return lhs.createdAt > rhs.createdAt }
            return lhs.sortOrder < rhs.sortOrder
        }
    }

    var completed: [TaskItem] {
        items.filter(\.isCompleted).sorted {
            ($0.completedAt ?? .distantPast) > ($1.completedAt ?? .distantPast)
        }
    }

    func add(_ rawText: String) {
        let text = rawText.trimmingCharacters(in: .whitespacesAndNewlines)
        guard !text.isEmpty else { return }
        let nextOrder = (unfinished.map(\.sortOrder).min() ?? 0) - 1
        items.append(TaskItem(text: text, sortOrder: nextOrder))
        normalizeOrders()
        save()
    }

    func toggle(_ id: UUID) {
        guard let index = items.firstIndex(where: { $0.id == id }) else { return }
        items[index].isCompleted.toggle()
        if items[index].isCompleted {
            items[index].isPinned = false
            items[index].completedAt = .now
            recentlyCompletedID = id
        } else {
            items[index].completedAt = nil
            items[index].sortOrder = (unfinished.map(\.sortOrder).min() ?? 0) - 1
            recentlyCompletedID = nil
            normalizeOrders()
        }
        save()
    }

    func restore(_ id: UUID) {
        guard let index = items.firstIndex(where: { $0.id == id }) else { return }
        items[index].isCompleted = false
        items[index].completedAt = nil
        items[index].sortOrder = (unfinished.map(\.sortOrder).min() ?? 0) - 1
        normalizeOrders()
        save()
    }

    func delete(_ id: UUID) {
        items.removeAll { $0.id == id }
        if recentlyCompletedID == id { recentlyCompletedID = nil }
        normalizeOrders()
        save()
    }

    func deleteCompleted(_ ids: Set<UUID>) {
        guard !ids.isEmpty else { return }
        items.removeAll { $0.isCompleted && ids.contains($0.id) }
        recentlyCompletedID = nil
        normalizeOrders()
        save()
    }

    func updateText(_ id: UUID, text rawText: String) {
        let text = rawText.trimmingCharacters(in: .whitespacesAndNewlines)
        guard !text.isEmpty, let index = items.firstIndex(where: { $0.id == id }) else { return }
        items[index].text = text
        save()
    }

    func togglePinned(_ id: UUID) {
        guard let index = items.firstIndex(where: { $0.id == id && !$0.isCompleted }) else { return }

        if items[index].isPinned {
            var updatedItems = items
            updatedItems[index].isPinned = false
            items = updatedItems
            save()
            return
        }

        let currentOrder = unfinished.map(\.id)
        let nextOrder = [id] + currentOrder.filter { $0 != id }
        let orderByID = Dictionary(uniqueKeysWithValues: nextOrder.enumerated().map { ($1, $0) })

        var updatedItems = items
        for itemIndex in updatedItems.indices where !updatedItems[itemIndex].isCompleted {
            let itemID = updatedItems[itemIndex].id
            updatedItems[itemIndex].isPinned = itemID == id
            if let order = orderByID[itemID] {
                updatedItems[itemIndex].sortOrder = order
            }
        }
        items = updatedItems
        save()
    }

    func move(_ draggedID: UUID, before targetID: UUID) {
        var ordered = unfinished
        guard
            let sourceIndex = ordered.firstIndex(where: { $0.id == draggedID }),
            let targetIndex = ordered.firstIndex(where: { $0.id == targetID }),
            !ordered[sourceIndex].isPinned,
            sourceIndex != targetIndex
        else { return }

        let item = ordered.remove(at: sourceIndex)
        let adjustedTarget = sourceIndex < targetIndex ? targetIndex - 1 : targetIndex
        let firstMovableIndex = ordered.first?.isPinned == true ? 1 : 0
        ordered.insert(item, at: max(firstMovableIndex, adjustedTarget))
        applyOrder(ordered)
    }

    func moveToEnd(_ draggedID: UUID) {
        var ordered = unfinished
        guard let index = ordered.firstIndex(where: { $0.id == draggedID }), !ordered[index].isPinned else { return }
        ordered.append(ordered.remove(at: index))
        applyOrder(ordered)
    }

    private func applyOrder(_ ordered: [TaskItem]) {
        let orderByID = Dictionary(uniqueKeysWithValues: ordered.enumerated().map { ($1.id, $0) })
        for index in items.indices where !items[index].isCompleted {
            if let order = orderByID[items[index].id] { items[index].sortOrder = order }
        }
        save()
    }

    private func normalizeOrders() {
        let ids = unfinished.map(\.id)
        let orderByID = Dictionary(uniqueKeysWithValues: ids.enumerated().map { ($1, $0) })
        for index in items.indices where !items[index].isCompleted {
            if let order = orderByID[items[index].id] { items[index].sortOrder = order }
        }
    }

    private func load() {
        if let savedItems = decodeItems(at: fileURL) {
            items = savedItems
            return
        }

        if let backupItems = decodeItems(at: backupFileURL) {
            items = backupItems
            NSLog("LiteTick 已从本地备份恢复数据")
            return
        }

        items = []
    }

    private func save() {
        do {
            try FileManager.default.createDirectory(
                at: fileURL.deletingLastPathComponent(),
                withIntermediateDirectories: true
            )
            try preserveValidBackup()
            let data = try JSONEncoder.liteTick.encode(items)
            try data.write(to: fileURL, options: .atomic)
        } catch {
            NSLog("LiteTick 保存失败: %@", error.localizedDescription)
        }
    }

    private var backupFileURL: URL {
        fileURL.deletingLastPathComponent().appending(path: "items.backup.json")
    }

    private func decodeItems(at url: URL) -> [TaskItem]? {
        guard
            let data = try? Data(contentsOf: url),
            let decoded = try? JSONDecoder.liteTick.decode([TaskItem].self, from: data)
        else { return nil }
        return decoded
    }

    private func preserveValidBackup() throws {
        guard
            let data = try? Data(contentsOf: fileURL),
            (try? JSONDecoder.liteTick.decode([TaskItem].self, from: data)) != nil
        else {
            return
        }
        try data.write(to: backupFileURL, options: .atomic)
    }

    private static func defaultFileURL() -> URL {
        let base = FileManager.default.urls(for: .applicationSupportDirectory, in: .userDomainMask).first!
        return base.appending(path: ProductIdentity.dataDirectoryName, directoryHint: .isDirectory)
            .appending(path: "items.json")
    }

}

private extension JSONEncoder {
    static var liteTick: JSONEncoder {
        let encoder = JSONEncoder()
        encoder.outputFormatting = [.prettyPrinted, .sortedKeys]
        encoder.dateEncodingStrategy = .iso8601
        return encoder
    }
}

private extension JSONDecoder {
    static var liteTick: JSONDecoder {
        let decoder = JSONDecoder()
        decoder.dateDecodingStrategy = .iso8601
        return decoder
    }
}
