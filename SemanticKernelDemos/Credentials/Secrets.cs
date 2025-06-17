using Microsoft.Extensions.Configuration;
using System;

namespace SemanticKernelDemos.Credentials
{
    public  class Secrets
    {
        //public static IConfiguration config = new ConfigurationBuilder().SetBasePath(AppContext.BaseDirectory)
        //      .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
        //      .Build();

        //public static readonly string? openaiKey = config["OPENAI_KEY"];
        //public static readonly string? openaiEndpoint = config["OPENAI_ENDPOINT"];
        //public static readonly string? openaiChatModel = config["OPENAI_CHAT_MODEL"];
        //public static readonly string? bingApiKey = config["BING_API_KEY"];
        //public static readonly string? clientId = config["CLIENTID"];
        //public static readonly string? tenantId = config["TENANTID"];


       
        public static readonly string? openaiKey = "3d91d6d1b40b414dbb8e3313f9cc5857";
        public static readonly string? openaiEndpoint = "https://swedencentral.api.cognitive.microsoft.com/";
        public static readonly string? openaiChatModel = "gpt-4";
        public static readonly string? bingApiKey = "bc1ee0c775bb467dab7b64bf07ad3940";
        public static readonly string? clientId = "a167980f-070e-4c6e-83df-2e6dda1655ba";
        public static readonly string? tenantId = "359e9e0b-3ad4-4089-a567-20ee36912628";
    }
}
