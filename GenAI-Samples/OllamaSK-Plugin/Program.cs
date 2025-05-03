#pragma warning disable SKEXP0001, SKEXP0070

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Ollama;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using System.ComponentModel;

var modelId = "llama3.1";//"phi4-mini";
var uri = "http://localhost:11434/";


var builder = Kernel
    .CreateBuilder()
    .AddOllamaChatCompletion(modelId, new Uri(uri))
    ;
var kernel = builder.Build();
var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

// Add the plugin to the kernel
kernel.Plugins.AddFromType<TaskManagementPlugin>("TaskManagement");

var settings = new OllamaPromptExecutionSettings { FunctionChoiceBehavior = FunctionChoiceBehavior.Auto() };

// Create a history store the conversation
var history = new ChatHistory();
history.AddUserMessage("What are all of the critical tasks?");

// Get the response from the AI
var result = await chatCompletionService.GetChatMessageContentAsync(
   history,
   executionSettings: settings,
   kernel: kernel);

// Print the results
Console.WriteLine("Assistant: " + result);

public class TaskManagementPlugin
{
    // Mock data for the tasks
    private readonly List<TaskModel> tasks = new()
    {
        new TaskModel { Id = 1, Title = "Design homepage", Description = "Create a modern homepage layout", Status = "In Progress", Priority = "High" },
        new TaskModel { Id = 2, Title = "Fix login bug", Description = "Resolve the issue with login sessions timing out", Status = "To Do", Priority = "Critical" },
        new TaskModel { Id = 3, Title = "Update documentation", Description = "Improve API reference for developers", Status = "Completed", Priority = "Medium" }
    };

    [KernelFunction("complete_task")]
    [Description("Updates the status of the specified task to Completed")]
    [return: Description("The updated task; will return null if the task does not exist")]
    public TaskModel? CompleteTask(int id)
    {
        var task = tasks.FirstOrDefault(task => task.Id == id);

        if (task == null)
        {
            return null;
        }

        task.Status = "Completed";

        return task;
    }

    [KernelFunction("get_critical_tasks")]
    [Description("Gets a list of all tasks marked as 'Critical' priority")]
    [return: Description("A list of critical tasks")]
    public List<TaskModel> GetCriticalTasks()
    {
        // Filter tasks with "Critical" priority
        return tasks.Where(task => task.Priority.Equals("Critical", StringComparison.OrdinalIgnoreCase)).ToList();
    }
}

public class TaskModel
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Status { get; set; }
    public string Priority { get; set; }
}
