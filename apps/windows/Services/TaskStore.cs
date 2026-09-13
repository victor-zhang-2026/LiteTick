using System.Text.Json;
using LiteTick.Windows.Models;

namespace LiteTick.Windows.Services;

public sealed class TaskStore
{
    private readonly List<TaskItem> _items;

    public TaskStore()
    {
        _items = LoadItems();
        NormalizePins();
    }

    public IReadOnlyList<TaskItem> Pending => _items
        .Where(item => !item.IsCompleted)
        .OrderByDescending(item => item.IsPinned)
        .ThenBy(item => item.SortOrder)
        .ThenByDescending(item => item.CreatedAt)
        .ToList();

    public IReadOnlyList<TaskItem> Completed => _items
        .Where(item => item.IsCompleted)
        .OrderByDescending(item => item.CompletedAt ?? item.CreatedAt)
        .ToList();

    public void Add(string text)
    {
        var cleaned = text.Trim();
        if (cleaned.Length == 0)
        {
            return;
        }

        var nextOrder = (Pending.Select(item => item.SortOrder).DefaultIfEmpty(0).Min()) - 1;
        _items.Add(new TaskItem { Text = cleaned, SortOrder = nextOrder });
        NormalizeOrders();
        Save();
    }

    public void Update(Guid id, string text)
    {
        var item = Find(id);
        var cleaned = text.Trim();
        if (item is null || cleaned.Length == 0)
        {
            return;
        }
        item.Text = cleaned;
        Save();
    }

    public void Complete(Guid id)
    {
        var item = Find(id);
        if (item is null)
        {
            return;
        }
        item.IsCompleted = true;
        item.CompletedAt = DateTimeOffset.Now;
        item.IsPinned = false;
        Save();
    }

    public void Restore(Guid id)
    {
        var item = Find(id);
        if (item is null)
        {
            return;
        }
        item.IsCompleted = false;
        item.CompletedAt = null;
        item.SortOrder = (Pending.Select(candidate => candidate.SortOrder).DefaultIfEmpty(0).Min()) - 1;
        NormalizeOrders();
        Save();
    }

    public void TogglePin(Guid id)
    {
        var item = Find(id);
        if (item is null || item.IsCompleted)
        {
            return;
        }

        if (item.IsPinned)
        {
            item.IsPinned = false;
            Save();
            return;
        }

        var nextOrder = new[] { item.Id }
            .Concat(Pending.Where(candidate => candidate.Id != item.Id).Select(candidate => candidate.Id))
            .ToList();
        var orderById = nextOrder.Select((idValue, index) => (idValue, index))
            .ToDictionary(pair => pair.idValue, pair => pair.index);
        foreach (var candidate in _items.Where(candidate => !candidate.IsCompleted))
        {
            candidate.IsPinned = candidate.Id == item.Id;
            candidate.SortOrder = orderById[candidate.Id];
        }
        Save();
    }

    public void Move(Guid draggedId, Guid targetId)
    {
        var ordered = Pending.ToList();
        var sourceIndex = ordered.FindIndex(item => item.Id == draggedId);
        var targetIndex = ordered.FindIndex(item => item.Id == targetId);
        if (sourceIndex < 0 || targetIndex < 0 || ordered[sourceIndex].IsPinned || sourceIndex == targetIndex)
        {
            return;
        }

        var dragged = ordered[sourceIndex];
        ordered.RemoveAt(sourceIndex);
        var adjustedTarget = sourceIndex < targetIndex ? targetIndex - 1 : targetIndex;
        var firstMovableIndex = ordered.FirstOrDefault()?.IsPinned == true ? 1 : 0;
        ordered.Insert(Math.Max(firstMovableIndex, adjustedTarget), dragged);
        ApplyOrder(ordered);
    }

    public void MoveToEnd(Guid draggedId)
    {
        var ordered = Pending.ToList();
        var index = ordered.FindIndex(item => item.Id == draggedId);
        if (index < 0 || ordered[index].IsPinned)
        {
            return;
        }
        ordered.Add(ordered[index]);
        ordered.RemoveAt(index);
        ApplyOrder(ordered);
    }

    public void Delete(Guid id)
    {
        var item = Find(id);
        if (item is null)
        {
            return;
        }
        _items.Remove(item);
        NormalizeOrders();
        Save();
    }

    public void DeleteCompleted(IEnumerable<Guid> ids)
    {
        var selected = ids.ToHashSet();
        _items.RemoveAll(item => item.IsCompleted && selected.Contains(item.Id));
        NormalizeOrders();
        Save();
    }

    private TaskItem? Find(Guid id) => _items.FirstOrDefault(item => item.Id == id);

    private void Save()
    {
        JsonFile.WriteAtomically(LocalPaths.ItemsFile, _items, IsValidItemsFile);
    }

    private static List<TaskItem> LoadItems()
    {
        foreach (var path in new[] { LocalPaths.ItemsFile, LocalPaths.BackupFile })
        {
            try
            {
                if (!File.Exists(path))
                {
                    continue;
                }
                var items = JsonSerializer.Deserialize<List<TaskItem>>(File.ReadAllText(path), JsonFile.Options);
                if (items is not null && Validate(items))
                {
                    return items;
                }
            }
            catch
            {
                // Try the backup before starting with an empty list.
            }
        }
        return [];
    }

    private static bool IsValidItemsFile(string path)
    {
        try
        {
            var items = JsonSerializer.Deserialize<List<TaskItem>>(File.ReadAllText(path), JsonFile.Options);
            return items is not null && Validate(items);
        }
        catch
        {
            return false;
        }
    }

    private static bool Validate(List<TaskItem> items)
    {
        return items.All(item => item.Id != Guid.Empty && !string.IsNullOrWhiteSpace(item.Text))
            && items.Select(item => item.Id).Distinct().Count() == items.Count;
    }

    private void NormalizePins()
    {
        var pinnedPending = _items.Where(item => !item.IsCompleted && item.IsPinned).ToList();
        foreach (var extra in pinnedPending.Skip(1))
        {
            extra.IsPinned = false;
        }
        foreach (var completed in _items.Where(item => item.IsCompleted))
        {
            completed.IsPinned = false;
        }
        NormalizeOrders();
    }

    private void NormalizeOrders()
    {
        var orderedIds = Pending.Select(item => item.Id).ToList();
        var orderById = orderedIds.Select((id, index) => (id, index))
            .ToDictionary(pair => pair.id, pair => pair.index);
        foreach (var item in _items.Where(item => !item.IsCompleted))
        {
            item.SortOrder = orderById[item.Id];
        }
    }

    private void ApplyOrder(List<TaskItem> ordered)
    {
        for (var index = 0; index < ordered.Count; index++)
        {
            ordered[index].SortOrder = index;
        }
        Save();
    }
}
