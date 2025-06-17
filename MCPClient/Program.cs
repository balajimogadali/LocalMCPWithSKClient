using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using ModelContextProtocol;
using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol.Transport;
using System.Text.Json;

Console.WriteLine("MCP Client Started!");

// MCP Client Options
var clientOptions = new McpClientOptions
{
    ClientInfo = new() { Name = "demo-client", Version = "1.0.0" }
};

// MCP Server Configuration
var serverConfig = new McpServerConfig
{
    Id = "demo-server",
    Name = "Demo Server",
    TransportType = TransportTypes.StdIo,
    TransportOptions = new Dictionary<string, string>
    {
        ["command"] = @"C:\Users\balaj\source\repos\LocalMCPServer\bin\Debug\net9.0\LocalMCPServer.exe"
    }
};

// Logger
using var loggerFactory = LoggerFactory.Create(builder =>
    builder.AddConsole().SetMinimumLevel(LogLevel.Information));

// Create MCP Client
await using var mcpClient =
    await McpClientFactory.CreateAsync(serverConfig, clientOptions, loggerFactory: loggerFactory);


var playwrightConfig = new McpServerConfig
{
    Id = "Playwright",
    Name = "Playwright",
    TransportType = TransportTypes.StdIo,
    TransportOptions = new Dictionary<string, string>
    {
        ["command"] = "npx",
        ["arguments"] = "-y @playwright/mcp@latest",
    }
};


// Create MCP Client
await using var mcpPlayWrightClient =
    await McpClientFactory.CreateAsync(playwrightConfig, clientOptions, loggerFactory: loggerFactory);

// Get available tools from MCP Server
var mcpPlayWrightTools = await mcpPlayWrightClient.ListToolsAsync();



var airbnbConfig = new McpServerConfig
{
    Id = "airbnb",
    Name = "airbnb",
    TransportType = TransportTypes.StdIo,
    TransportOptions = new Dictionary<string, string>
    {
        ["command"] = "npx",
        ["arguments"] = "-y @openbnb/mcp-server-airbnb",
    }
};

// Create MCP Client
await using var mcpAirBnbClient =
    await McpClientFactory.CreateAsync(airbnbConfig, clientOptions, loggerFactory: loggerFactory);

// Get available tools from MCP Server
var mcpAirBnbTools = await mcpAirBnbClient.ListToolsAsync();

// Configure Ollama LLM Client
var ollamaChatClient = new OllamaChatClient(
    new Uri("http://localhost:11434/"),
    "llama3.2:3b"
);

var chatClient = new ChatClientBuilder(ollamaChatClient)
    .UseLogging(loggerFactory)
    .UseFunctionInvocation()
    .Build();
// Get available tools from MCP Server
var mcpTools = await mcpClient.ListToolsAsync();

var allTools = mcpTools.Concat(mcpPlayWrightTools).ToList();
allTools.AddRange(mcpAirBnbTools);

var toolsJson = JsonSerializer.Serialize(allTools, new JsonSerializerOptions { WriteIndented = true });
Console.WriteLine("\nAvailable Tools:\n" + toolsJson);

await Task.Delay(100);

// Prompt loop
Console.WriteLine("Type your message below (type 'exit' to quit):");


while (true)
{
    Console.Write("\n You: ");
    var userInput = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(userInput))
        continue;

    if (userInput.Trim().ToLower() == "exit")
    {
        Console.WriteLine("Exiting chat...");
        break;
    }

    var messages = new List<ChatMessage>
    {
        new(ChatRole.System, "You are a helpful assistant."),
        new(ChatRole.User, userInput)
    };

    try
    {
        var response = await chatClient.GetResponseAsync(
            messages,
            new ChatOptions { Tools = [.. allTools] });

        var assistantMessage = response.Messages.LastOrDefault(m => m.Role == ChatRole.Assistant);

        if (assistantMessage != null)
        {
            var textOutput = string.Join(" ", assistantMessage.Contents.Select(c => c.ToString()));
            Console.WriteLine("\n AI: " + textOutput);
        }
        else
        {
            Console.WriteLine("\n AI: (no assistant message received)");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"\n Error: {ex.Message}");
    }
}