using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol.Transport;

var clientTransport = new StdioClientTransport(new StdioClientTransportOptions
{
    Name = "Local-MCP-Server",
    Command = "dotnet",
    Arguments =
                [
                    "run",
                    "--project",
                    "C:\\Users\\balaj\\source\\repos\\LocalMCPServer",
                    "--no-build"
                ],
});

var mcpClient = await McpClientFactory.CreateAsync(clientTransport);

var tools = await mcpClient.ListToolsAsync().ConfigureAwait(false);

foreach (var tool in tools)
{
    Console.WriteLine($"{tool.Name}: {tool.Description}");
}

var kernel = CreateBuilder();

#pragma warning disable SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
kernel.Plugins.AddFromFunctions("LocalMCPServer",
    tools.Select(aiFunction => aiFunction.AsKernelFunction()));
#pragma warning restore SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

#pragma warning disable SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
OpenAIPromptExecutionSettings executionSettings = new()
{
    Temperature = 0,
    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(options: new() { RetainArgumentTypes = true })
};
#pragma warning restore SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

var prompt = "list the details of books and weather information about Thanjavur";
var result = await kernel.InvokePromptAsync(prompt, new(executionSettings)).ConfigureAwait(false);
Console.WriteLine($"\n\n{prompt}\n{result}");

Console.Read();





static Kernel CreateBuilder()
{
    Config config = new Config();

    var builder = Kernel.CreateBuilder();
    builder.Services.AddAzureOpenAIChatCompletion(config.DeploymentOrModelId, config.Endpoint, config.ApiKey);
    Kernel kernel = builder.Build();
    return kernel;
}


