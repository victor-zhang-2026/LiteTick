import Foundation

struct TaskItem: Identifiable, Codable, Equatable, Sendable {
    let id: UUID
    var text: String
    var isCompleted: Bool
    var sortOrder: Int
    let createdAt: Date
    var completedAt: Date?
    var isPinned: Bool

    init(
        id: UUID = UUID(),
        text: String,
        isCompleted: Bool = false,
        sortOrder: Int,
        createdAt: Date = .now,
        completedAt: Date? = nil,
        isPinned: Bool = false
    ) {
        self.id = id
        self.text = text
        self.isCompleted = isCompleted
        self.sortOrder = sortOrder
        self.createdAt = createdAt
        self.completedAt = completedAt
        self.isPinned = isPinned
    }

    private enum CodingKeys: String, CodingKey {
        case id, text, isCompleted, sortOrder, createdAt, completedAt, isPinned
    }

    init(from decoder: Decoder) throws {
        let values = try decoder.container(keyedBy: CodingKeys.self)
        id = try values.decode(UUID.self, forKey: .id)
        text = try values.decode(String.self, forKey: .text)
        isCompleted = try values.decode(Bool.self, forKey: .isCompleted)
        sortOrder = try values.decode(Int.self, forKey: .sortOrder)
        createdAt = try values.decode(Date.self, forKey: .createdAt)
        completedAt = try values.decodeIfPresent(Date.self, forKey: .completedAt)
        isPinned = try values.decodeIfPresent(Bool.self, forKey: .isPinned) ?? false
    }
}
