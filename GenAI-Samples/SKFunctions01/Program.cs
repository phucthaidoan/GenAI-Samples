#pragma warning disable SKEXP0070 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Ollama;
using OllamaFunctionCalling;
using SKFunctions01.Plugins;

var builder = Kernel.CreateBuilder();
var modelId = "llama3.1";
var endpoint = new Uri("http://localhost:11434");

builder.Services.AddOllamaChatCompletion(modelId, endpoint);

builder.Plugins
    .AddFromType<MyTimePlugin>()
    .AddFromObject(new MyLightPlugin(turnedOn: true))
    .AddFromObject(new MyAlarmPlugin("11"))
    .AddFromType<CityTemperaturePlugIn>();

var kernel = builder.Build();
var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();
var settings = new OllamaPromptExecutionSettings { FunctionChoiceBehavior = FunctionChoiceBehavior.Auto() };

Console.WriteLine("""
    Ask questions or give instructions to the copilot such as:
    - Change the alarm to 8
    - What is the current alarm set?
    - Is the light on?
    - Turn the light off please.
    - Set an alarm for 6:00 am.
    """);

Console.Write("> ");

string? input = null;
var history = new ChatHistory();

while (true)
{
    Console.Write("Q: ");
    var userQ = Console.ReadLine();
    if (string.IsNullOrEmpty(userQ))
    {
        break;
    }
    history.AddUserMessage(userQ);

    var result = await chatCompletionService.GetChatMessageContentAsync(
        history,
        executionSettings: settings,
        kernel: kernel);
    Console.WriteLine(result.Content);
    history.AddAssistantMessage(result?.Content ?? string.Empty);
}
