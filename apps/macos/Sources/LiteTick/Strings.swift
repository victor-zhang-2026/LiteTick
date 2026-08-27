import Foundation

enum L10n {
    private static let languageKey = "appLanguage"

    static var isChinese: Bool {
        if let stored = UserDefaults.standard.string(forKey: languageKey) {
            return stored == "zh"
        }
        return Locale.current.language.languageCode?.identifier == "zh"
    }

    static func text(_ zh: String, _ en: String) -> String { isChinese ? zh : en }

    static func toggleLanguage() {
        UserDefaults.standard.set(isChinese ? "en" : "zh", forKey: languageKey)
    }

    static var appName: String { text(ProductIdentity.chineseName, ProductIdentity.englishName) }
    static var about: String { text("关于", "About") }
    static var aboutTitle: String { text("关于\(ProductIdentity.chineseName)", "About \(ProductIdentity.englishName)") }
    static var aboutDescription: String { text("桌面悬浮待办", "Floating Desktop Checklist") }
    static var github: String { "GitHub" }
    static var privacy: String { text("隐私", "Privacy") }
    static var license: String { text("许可证", "License") }
    static var appearance: String { text("外观", "Appearance") }
    static var switchToLight: String { text("切换到浅色模式", "Switch to Light Mode") }
    static var switchToDark: String { text("切换到深色模式", "Switch to Dark Mode") }
    static var placeholder: String { text("写下要做的事…", "Write something…") }
    static var emptyTitle: String { text("现在还没有内容", "Nothing here yet") }
    static var emptyHint: String { text("写下第一件事吧", "Write your first item") }
    static var noPendingTitle: String { text("当前没有待办", "No pending items") }
    static var noPendingHint: String { text("可在已完成中查看或恢复", "View or restore items in Completed") }
    static var completed: String { text("已完成", "Completed") }
    static var history: String { text("已完成", "Completed") }
    static var search: String { text("搜索已完成", "Search completed") }
    static var noCompleted: String { text("没有已完成事项", "No completed items") }
    static var noMatches: String { text("没有匹配结果", "No matching items") }
    static var restore: String { text("恢复", "Restore") }
    static var restoreToList: String { text("恢复到列表", "Restore to List") }
    static var delete: String { text("删除", "Delete") }
    static var deleteAll: String { text("全部删除", "Delete All") }
    static var deleteResults: String { text("删除结果", "Delete Results") }
    static var deleteAllTitle: String { text("删除全部已完成事项？", "Delete all completed items?") }
    static var deleteAllMessage: String { text("删除后无法恢复。", "This action cannot be undone.") }
    static var deleteResultsTitle: String { text("删除搜索结果？", "Delete search results?") }
    static var deleteResultsMessage: String { text("只会删除当前搜索到的已完成事项，删除后无法恢复。", "Only the completed items in the current search results will be deleted. This action cannot be undone.") }
    static var deleteTitle: String { text("永久删除这条记录？", "Delete this item permanently?") }
    static var deleteMessage: String { text("删除后无法恢复。", "This action cannot be undone.") }
    static var cancel: String { text("取消", "Cancel") }
    static var undo: String { text("撤销", "Undo") }
    static var back: String { text("返回清单", "Back to List") }
    static var closePanel: String { text("收起清单", "Close Panel") }
    static var quit: String { text("退出\(ProductIdentity.chineseName)", "Quit \(ProductIdentity.englishName)") }
    static var quitTitle: String { text("退出\(ProductIdentity.chineseName)？", "Quit \(ProductIdentity.englishName)?") }
    static var quitMessage: String { text("确定要退出吗？", "Are you sure you want to quit?") }
    static var quitConfirm: String { text("退出", "Quit") }
    static var edit: String { text("编辑", "Edit") }
    static var dragToReorder: String { text("拖动排序", "Drag to reorder") }
    static var pinItem: String { text("置顶", "Pin Item") }
    static var unpinItem: String { text("取消置顶", "Unpin Item") }
    static var pinPanel: String { text("固定浮窗", "Pin Panel") }
    static var unpinPanel: String { text("取消固定", "Unpin Panel") }
}
