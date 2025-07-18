﻿
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

Console.WriteLine("Hello , MCP Server");

var builder = Host.CreateEmptyApplicationBuilder(settings: null);

// Register the MCP server and its core services in the DI container.
builder.Services.AddMcpServer()
    // Configure the server to use standard input/output (Stdio) for communication.
    .WithStdioServerTransport()
    // Automatically discover and register tools from the current assembly.
    .WithToolsFromAssembly();


var app = builder.Build();


await app.RunAsync();