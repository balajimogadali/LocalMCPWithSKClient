using System;
using Azure;
using Azure.AI.OpenAI;
using Microsoft.SemanticKernel;
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel.Plugins.Web;
using Microsoft.SemanticKernel.Plugins.Web.Bing;
using Microsoft.SemanticKernel.Plugins.Core;
using Kernel = Microsoft.SemanticKernel.Kernel;
using Microsoft.SemanticKernel.Planning.Handlebars;
using SemanticKernelDemos.Authentication;
using SemanticKernelDemos.DTOs;
using SemanticKernelDemos.Plugins;
namespace SemanticKernelDemos
{
    public static class Program
    {
        public class Globals
        {
            
            public static string key = "e4a4dccc201c423f824a6408a3c65835";
            public static string endpoint = "https://trainingopenaiazure.openai.azure.com/";
            public static string model = "GPT-4";
            public static string bingApiKey = "bc1ee0c775bb467dab7b64bf07ad3940";

        }

        public static  async Task Main(string[] args)
        {
            //initialize the kernel
            var builder = Kernel.CreateBuilder();
            builder.AddAzureOpenAIChatCompletion(Globals.model!, Globals.endpoint!, Globals.key!);
            var kernel = builder.Build();

            //import the default bing websearcher plugin into the kernel
            var bingConnector = new BingConnector(Globals.bingApiKey!);
            kernel.ImportPluginFromObject(new WebSearchEnginePlugin(bingConnector), "bing");
            var function = kernel.Plugins["bing"]["search"];

            //create token for Graph api
            Auth auth = new Auth();
            TokenManager.AccessToken = auth.GetAccessToken();

            //Import GraphPlugin into kernal
            var graphPlugin = kernel.ImportPluginFromType<GraphPlugin>();

            var planner = new HandlebarsPlanner(new HandlebarsPlannerOptions() { AllowLoops = true });

            //take input from the user
            Console.WriteLine("Enter the search query: ");
            string userQuery = Console.ReadLine();

            Console.WriteLine("Sending Message To Chat Completions Model");

            try
            {
                var plan = await planner.CreatePlanAsync(kernel, userQuery);

                var serializedPlan = plan.ToString();



                var result = await plan.InvokeAsync(kernel);

                var chatResponse = result.ToString();

                var stringSerializedPlan = serializedPlan.ToString();

                Console.WriteLine(chatResponse);


            }
            catch (Exception e) { Console.WriteLine(e.Message); }
        
        }
    }
}
