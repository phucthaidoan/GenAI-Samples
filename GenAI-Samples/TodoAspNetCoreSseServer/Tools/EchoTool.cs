﻿using ModelContextProtocol.Server;
using System.ComponentModel;

namespace TodoAspNetCoreSseServer.Tools;

[McpServerToolType]
public sealed class EchoTool
{
    [McpServerTool, Description("Echoes the input back to the client.")]
    public static string Echo(string message)
    {
        return "hello " + message;
    }
}
