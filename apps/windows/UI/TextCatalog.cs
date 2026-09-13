namespace LiteTick.Windows.UI;

internal sealed class TextCatalog
{
    internal bool IsChinese { get; }

    internal TextCatalog(bool isChinese)
    {
        IsChinese = isChinese;
    }

    private string Pick(string chinese, string english) => IsChinese ? chinese : english;

    internal string ProductName => Pick("妥了", "LiteTick");
    internal string AddPlaceholder => Pick("记录一件事…", "Add a task...");
    internal string NewListEmpty => Pick("写下第一件事", "Add your first task");
    internal string NoPending => Pick("目前没有待办事项", "No pending tasks");
    internal string Completed => Pick("已完成", "Completed");
    internal string NoCompleted => Pick("还没有已完成事项", "No completed tasks yet");
    internal string NoResults => Pick("没有匹配的结果", "No matching results");
    internal string SearchCompleted => Pick("搜索已完成…", "Search completed...");
    internal string Language => Pick("Switch to English", "切换到中文");
    internal string SwitchToDark => Pick("切换深色外观", "Switch to Dark");
    internal string SwitchToLight => Pick("切换浅色外观", "Switch to Light");
    internal string ShowCompleted => Pick("查看已完成", "Show Completed");
    internal string BackToList => Pick("返回清单", "Back to List");
    internal string PinPanel => Pick("固定面板", "Pin Panel");
    internal string UnpinPanel => Pick("取消固定", "Unpin Panel");
    internal string ClosePanel => Pick("收起清单", "Close Panel");
    internal string PinItem => Pick("置顶事项", "Move item to top");
    internal string UnpinItem => Pick("取消置顶", "Remove item from top");
    internal string CompleteItem => Pick("完成事项", "Complete item");
    internal string RestoreItem => Pick("恢复事项", "Restore item");
    internal string DragToReorder => Pick("拖动排序", "Drag to reorder");
    internal string Edit => Pick("编辑", "Edit");
    internal string Delete => Pick("删除", "Delete");
    internal string DeleteAll => Pick("全部删除", "Delete All");
    internal string DeleteResults => Pick("删除结果", "Delete Results");
    internal string DeleteItemTitle => Pick("永久删除此事项？", "Delete this item permanently?");
    internal string DeleteItemMessage => Pick("删除后无法恢复。", "This cannot be undone.");
    internal string DeleteAllTitle => Pick("永久删除所有已完成事项？", "Delete all completed tasks permanently?");
    internal string DeleteResultsTitle => Pick("永久删除这些搜索结果？", "Delete these search results permanently?");
    internal string DeleteManyMessage(int count) => Pick($"将永久删除 {count} 项，且无法恢复。", $"This will permanently delete {count} item(s) and cannot be undone.");
    internal string ConfirmDelete => Pick("删除", "Delete");
    internal string Cancel => Pick("取消", "Cancel");
    internal string Yes => Pick("是", "Yes");
    internal string No => Pick("否", "No");
    internal string Quit => Pick("退出妥了", "Quit LiteTick");
    internal string QuitTitle => Pick("退出妥了？", "Quit LiteTick?");
    internal string QuitMessage => Pick("悬浮入口将关闭，任务仍会保存在本机。", "The floating trigger will close. Your tasks remain stored locally.");
    internal string About => Pick("关于妥了", "About LiteTick");
    internal string Description => Pick("桌面悬浮待办", "Floating Desktop Checklist");
    internal string LocalOnly => Pick("本地保存 · 无需账号 · 无需联网", "Local storage · No account · No network");
    internal string Repository => Pick("GitHub 仓库", "GitHub Repository");
    internal string Privacy => Pick("隐私说明", "Privacy");
    internal string License => Pick("开源许可", "License");
}
