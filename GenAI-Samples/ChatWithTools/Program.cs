﻿using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol.Transport;
using Microsoft.Extensions.AI;
using OpenTelemetry;
using OpenTelemetry.Trace;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;

using var tracerProvider = Sdk.CreateTracerProviderBuilder()
    .AddHttpClientInstrumentation()
    .AddSource("*")
    .AddOtlpExporter()
    .Build();
using var metricsProvider = Sdk.CreateMeterProviderBuilder()
    .AddHttpClientInstrumentation()
    .AddMeter("*")
    .AddOtlpExporter()
    .Build();
using var loggerFactory = LoggerFactory.Create(builder => builder.AddOpenTelemetry(opt => opt.AddOtlpExporter()));

// Connect to an MCP server
Console.WriteLine("Connecting client to MCP 'everything' server");

var ollamaEndpoint = "http://localhost:11434";
var chatModel = "llama3.2";

using IChatClient samplingClient = new OllamaChatClient(
    endpoint: ollamaEndpoint,
    modelId: chatModel)
    .AsBuilder()
    .UseOpenTelemetry(loggerFactory: loggerFactory, configure: o => o.EnableSensitiveData = true)
    .Build();


var mcpClient = await McpClientFactory.CreateAsync(
    new StdioClientTransport(new()
    {
        Command = "dotnet",
        Arguments = ["run", "--project", "C:\\Workspace\\git\\GenAI-Samples\\GenAI-Samples\\EverythingServer\\EverythingServer.csproj"],
        Name = "Everything",
    }),
    clientOptions: new()
    {
        Capabilities = new() { Sampling = new() { SamplingHandler = samplingClient.CreateSamplingHandler() } },
    },
    loggerFactory: loggerFactory);

// Get all available tools
Console.WriteLine("Tools available:");
var tools = await mcpClient.ListToolsAsync();
foreach (var tool in tools)
{
    Console.WriteLine($"  {tool}");
}

Console.WriteLine();

// Create an IChatClient that can use the tools.
using IChatClient chatClient = new OllamaChatClient(
        endpoint: ollamaEndpoint,
        modelId: chatModel)
    .AsBuilder()
    .UseFunctionInvocation()
    .UseOpenTelemetry(loggerFactory: loggerFactory, configure: o => o.EnableSensitiveData = true)
    .Build();

// Have a conversation, making all tools available to the LLM.
List<ChatMessage> messages = [];
while (true)
{
    Console.Write("Q: ");
    messages.Add(new(ChatRole.User, Console.ReadLine()));

    List<ChatResponseUpdate> updates = [];
    await foreach (var update in chatClient.GetStreamingResponseAsync(messages, new() { Tools = [.. tools] }))
    {
        Console.Write(update);
        updates.Add(update);
    }
    Console.WriteLine();

    messages.AddMessages(updates);
}