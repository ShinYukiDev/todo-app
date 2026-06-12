using Microsoft.EntityFrameworkCore;
using TodoApp.Data;
using TodoApp.Services;

var databasePath = Path.Combine(AppContext.BaseDirectory, "todo.db");
var options = new DbContextOptionsBuilder<TodoDbContext>()
    .UseSqlite($"Data Source={databasePath}")
    .Options;

using var dbContext = new TodoDbContext(options);
dbContext.Database.EnsureCreated();

var todoService = new TodoService(dbContext);

while (true)
{
    Console.WriteLine();
    Console.WriteLine("==== Todoアプリ ====");
    Console.WriteLine("1. タスク一覧を表示");
    Console.WriteLine("2. タスクを追加");
    Console.WriteLine("3. タスクを変更");
    Console.WriteLine("4. タスクを削除");
    Console.WriteLine("5. タスクを完了");
    Console.WriteLine("0. 終了");
    Console.Write("操作を選択してください: ");
    var menuInput = Console.ReadLine();

    try
    {
        switch (menuInput)
        {
            case "1":
                PrintTasks(todoService);
                break;
            case "2":
                AddTask(todoService);
                break;
            case "3":
                UpdateTask(todoService);
                break;
            case "4":
                DeleteTask(todoService);
                break;
            case "5":
                CompleteTask(todoService);
                break;
            case "0":
                return;
            default:
                Console.WriteLine("無効なメニュー番号です。");
                break;
        }
    }
    catch (InvalidOperationException ex)
    {
        Console.WriteLine(ex.Message);
    }
}

static void PrintTasks(TodoService todoService)
{
    var tasks = todoService.GetAllTasks();
    if (tasks.Count == 0)
    {
        Console.WriteLine("タスクはありません。");
        return;
    }

    Console.WriteLine();
    Console.WriteLine("ID | 状態 | タイトル");
    foreach (var task in tasks)
    {
        var status = task.IsCompleted ? "完了" : "未完了";
        Console.WriteLine($"{task.Id} | {status} | {task.Title}");
    }
}

static void AddTask(TodoService todoService)
{
    var title = ReadRequiredText("追加するタスク名を入力してください: ");
    var task = todoService.AddTask(title);
    Console.WriteLine($"タスクを追加しました (ID: {task.Id})");
}

static void UpdateTask(TodoService todoService)
{
    var id = ReadTaskId("変更するタスクIDを入力してください: ");
    var title = ReadRequiredText("新しいタスク名を入力してください: ");
    todoService.UpdateTask(id, title);
    Console.WriteLine("タスクを変更しました。");
}

static void DeleteTask(TodoService todoService)
{
    var id = ReadTaskId("削除するタスクIDを入力してください: ");
    todoService.DeleteTask(id);
    Console.WriteLine("タスクを削除しました。");
}

static void CompleteTask(TodoService todoService)
{
    var id = ReadTaskId("完了にするタスクIDを入力してください: ");
    todoService.CompleteTask(id);
    Console.WriteLine("タスクを完了にしました。");
}

static int ReadTaskId(string message)
{
    Console.Write(message);
    var rawValue = Console.ReadLine();
    if (!int.TryParse(rawValue, out var id) || id <= 0)
    {
        throw new InvalidOperationException("IDは1以上の数値で入力してください。");
    }

    return id;
}

static string ReadRequiredText(string message)
{
    Console.Write(message);
    var text = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(text))
    {
        throw new InvalidOperationException("タスク名は必須です。");
    }

    return text.Trim();
}
