namespace LiteTick.Windows.Models;

public sealed class TaskItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Text { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;
    public bool IsCompleted { get; set; }
    public DateTimeOffset? CompletedAt { get; set; }
    public bool IsPinned { get; set; }
}
