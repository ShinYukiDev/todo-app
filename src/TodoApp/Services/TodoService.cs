using Microsoft.EntityFrameworkCore;
using TodoApp.Data;
using TodoApp.Models;

namespace TodoApp.Services;

public sealed class TodoService(TodoDbContext dbContext)
{
    public IReadOnlyList<TodoItem> GetAllTasks()
    {
        return dbContext.Todos
            .OrderBy(x => x.IsCompleted)
            .ThenBy(x => x.Id)
            .ToList();
    }

    public TodoItem AddTask(string title)
    {
        var normalizedTitle = NormalizeTitle(title);

        var now = DateTime.UtcNow;
        var task = new TodoItem
        {
            Title = normalizedTitle,
            IsCompleted = false,
            CreatedAt = now,
            UpdatedAt = now,
        };

        dbContext.Todos.Add(task);
        dbContext.SaveChanges();
        return task;
    }

    public void UpdateTask(int id, string title)
    {
        var normalizedTitle = NormalizeTitle(title);

        var task = FindTaskOrThrow(id);
        task.Title = normalizedTitle;
        task.UpdatedAt = DateTime.UtcNow;
        dbContext.SaveChanges();
    }

    public void DeleteTask(int id)
    {
        var task = FindTaskOrThrow(id);
        dbContext.Todos.Remove(task);
        dbContext.SaveChanges();
    }

    public void CompleteTask(int id)
    {
        var task = FindTaskOrThrow(id);
        task.IsCompleted = true;
        task.UpdatedAt = DateTime.UtcNow;
        dbContext.SaveChanges();
    }

    private TodoItem FindTaskOrThrow(int id)
    {
        var task = dbContext.Todos.SingleOrDefault(x => x.Id == id);
        if (task is null)
        {
            throw new InvalidOperationException("指定したIDのタスクは存在しません。");
        }

        return task;
    }

    private static string NormalizeTitle(string title)
    {
        var normalized = title.Replace("\0", string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(normalized))
        {
            throw new InvalidOperationException("タスク名は必須です。");
        }

        return normalized;
    }
}
