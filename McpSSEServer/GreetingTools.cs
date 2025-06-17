using ModelContextProtocol.Server;
using System.ComponentModel;
namespace McpSSEServer
{
    [McpServerToolType]
    public sealed class GreetingTools
    {
        public GreetingTools()
        {
        }
        [McpServerTool, Description("Says Hello to a user")]
        public static string Echo(string username)
        {
            return "Hello " + username;
        }
    }
}
