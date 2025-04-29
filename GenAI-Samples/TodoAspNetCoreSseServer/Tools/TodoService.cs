using ModelContextProtocol.Server;
using System.ComponentModel;

namespace TodoAspNetCoreSseServer.Tools;

[McpServerToolType]
public sealed class TodoService
{
    private static readonly List<TodoItem> _todos = new();
    private static int _nextId = 1;

    [McpServerTool(Name = "listTodos"), Description("Lists all TODO items")]
    public static IEnumerable<TodoItem> GetAll() => _todos;

    [McpServerTool(Name = "getTodo"), Description("Gets a specific TODO item by ID")]
    public static TodoItem? GetById(
        [Description("The ID of the TODO item to retrieve")] int id)
        => _todos.FirstOrDefault(t => t.Id == id);

    [McpServerTool(Name = "createTodo"), Description("Creates a new TODO item")]
    public static TodoItem CreateTodo(
        [Description("What you need to do")] string title,
        [Description("When it needs to be done (today, tomorrow, next week, or specific date)")] string dueDate = "today",
        [Description("Time of day (e.g. '09:00', '23:59', or 'eod' for end of day)")] string timeOfDay = "eod")
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Please provide what you need to do", nameof(title));

        var todo = new TodoItem
        {
            Id = _nextId++,
            Title = title,
            DueDate = DateService.ParseFutureDate(dueDate, timeOfDay),
            IsDone = false
        };

        _todos.Add(todo);
        return todo;
    }

    [McpServerTool(Name = "updateTodo"), Description("Updates an existing TODO item")]
    public static TodoItem? Update(
        [Description("ID of the TODO to update")] int id,
        [Description("New title (optional)")] string? title = null,
        [Description("New due date (optional)")] string? dueDate = null,
        [Description("New time of day (optional)")] string? timeOfDay = null,
        [Description("Mark as done? (optional)")] bool? isDone = null)
    {
        var existing = _todos.FirstOrDefault(t => t.Id == id);
        if (existing == null) return null;

        if (title != null)
            existing.Title = title;

        if (dueDate != null)
        {
            existing.DueDate = DateService.ParseFutureDate(dueDate, timeOfDay ?? "eod");
        }

        if (isDone.HasValue)
            existing.IsDone = isDone.Value;

        return existing;
    }

    [McpServerTool(Name = "deleteTodo"), Description("Deletes a TODO item")]
    public static bool Delete(
        [Description("The ID of the TODO item to delete")] int id)
    {
        var todo = _todos.FirstOrDefault(t => t.Id == id);
        if (todo == null) return false;

        return _todos.Remove(todo);
    }

    [McpServerTool(Name = "getOverdueTodos"), Description("Lists all overdue and incomplete tasks")]
    public static IEnumerable<TodoItem> GetOverdueTasks()
    {
        return _todos.Where(t => !t.IsDone && t.DueDate < DateTime.Now);
    }

    [McpServerTool(Name = "toggleTodo"), Description("Marks a TODO item as done or not done")]
    public static bool ToggleStatus(
        [Description("The ID of the TODO item to toggle")] int id)
    {
        var todo = _todos.FirstOrDefault(t => t.Id == id);
        if (todo == null) return false;

        todo.IsDone = !todo.IsDone;
        return true;
    }
}