using System.ComponentModel;

namespace TestServerWithHosting.Tools;

[System.Serializable]
public class TodoItem
{
    [Description("The unique identifier of the TODO item")]
    public int Id { get; set; }

    [Description("The title of the TODO item")]
    public string Title { get; set; } = string.Empty;

    [Description("The due date of the TODO item")]
    public DateTime DueDate { get; set; }

    [Description("Whether the TODO item is completed")]
    public bool IsDone { get; set; }
}