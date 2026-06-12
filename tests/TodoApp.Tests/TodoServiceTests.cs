using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using TodoApp.Data;
using TodoApp.Services;

namespace TodoApp.Tests;

public sealed class TodoServiceTests
{
    [Fact]
    public void AddTask_有効なタイトルを渡すとタスクが保存される()
    {
        using var testScope = CreateTestScope();

        var created = testScope.Service.AddTask("買い物");

        var reloaded = testScope.Context.Todos.Single(x => x.Id == created.Id);
        Assert.Equal("買い物", reloaded.Title);
        Assert.False(reloaded.IsCompleted);
    }

    [Fact]
    public void UpdateTask_存在するタスクを更新できる()
    {
        using var testScope = CreateTestScope();
        var created = testScope.Service.AddTask("古いタイトル");

        testScope.Service.UpdateTask(created.Id, "新しいタイトル");

        var reloaded = testScope.Context.Todos.Single(x => x.Id == created.Id);
        Assert.Equal("新しいタイトル", reloaded.Title);
    }

    [Fact]
    public void DeleteTask_存在するタスクを削除できる()
    {
        using var testScope = CreateTestScope();
        var created = testScope.Service.AddTask("削除対象");

        testScope.Service.DeleteTask(created.Id);

        Assert.Empty(testScope.Context.Todos);
    }

    [Fact]
    public void CompleteTask_存在するタスクを完了にできる()
    {
        using var testScope = CreateTestScope();
        var created = testScope.Service.AddTask("完了対象");

        testScope.Service.CompleteTask(created.Id);

        var reloaded = testScope.Context.Todos.Single(x => x.Id == created.Id);
        Assert.True(reloaded.IsCompleted);
    }

    [Fact]
    public void GetAllTasks_未完了が先でID順に返る()
    {
        using var testScope = CreateTestScope();
        var completed = testScope.Service.AddTask("後で表示");
        var active = testScope.Service.AddTask("先に表示");
        testScope.Service.CompleteTask(completed.Id);

        var list = testScope.Service.GetAllTasks();

        Assert.Collection(
            list,
            item => Assert.Equal(active.Id, item.Id),
            item => Assert.Equal(completed.Id, item.Id));
    }

    [Fact]
    public void UpdateTask_存在しないIDなら例外()
    {
        using var testScope = CreateTestScope();

        var action = () => testScope.Service.UpdateTask(999, "更新");

        var ex = Assert.Throws<InvalidOperationException>(action);
        Assert.Equal("指定したIDのタスクは存在しません。", ex.Message);
    }

    [Fact]
    public void AddTask_空白タイトルなら例外()
    {
        using var testScope = CreateTestScope();

        var action = () => testScope.Service.AddTask("   ");

        var ex = Assert.Throws<InvalidOperationException>(action);
        Assert.Equal("タスク名は必須です。", ex.Message);
    }

    [Fact]
    public void AddTask_Null文字のみなら例外()
    {
        using var testScope = CreateTestScope();

        var action = () => testScope.Service.AddTask("\0\0");

        var ex = Assert.Throws<InvalidOperationException>(action);
        Assert.Equal("タスク名は必須です。", ex.Message);
    }

    private static TestScope CreateTestScope()
    {
        var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<TodoDbContext>()
            .UseSqlite(connection)
            .Options;

        var context = new TodoDbContext(options);
        context.Database.EnsureCreated();

        var service = new TodoService(context);
        return new TestScope(connection, context, service);
    }

    private sealed class TestScope(SqliteConnection connection, TodoDbContext context, TodoService service) : IDisposable
    {
        public SqliteConnection Connection { get; } = connection;
        public TodoDbContext Context { get; } = context;
        public TodoService Service { get; } = service;

        public void Dispose()
        {
            Context.Dispose();
            Connection.Dispose();
        }
    }
}
