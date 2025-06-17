using Azure.AI.OpenAI;
using Azure;
using Microsoft.SemanticKernel.Plugins.Web.Bing;
using Microsoft.SemanticKernel.Plugins.Web;
using Microsoft.SemanticKernel;
using SemanticKernelDemos.Credentials;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemanticKernelDemos.Plugins
{
    public class WebSearchPlugin
    {
        [KernelFunction, Description("To search for information/answers to the user query on the web such as news, articles, weather report and much more. To find information that the GPT would not have access to.")]
        public async Task<string> WebSearchFunction([Description("the user query whose answer needs to be searched on the web")] string userQuery)
        {
            var builder = Kernel.CreateBuilder();
            builder.AddAzureOpenAIChatCompletion(Secrets.openaiChatModel!, Secrets.openaiEndpoint!, Secrets.openaiKey!);
            var kernel = builder.Build();

            var bingConnector = new BingConnector(Secrets.bingApiKey!);
            kernel.ImportPluginFromObject(new WebSearchEnginePlugin(bingConnector), "bing");

            var function = kernel.Plugins["bing"]["search"];

            var bingResult = await kernel.InvokeAsync(function, new() { ["query"] = userQuery });
            var stringResult = bingResult.ToString();
            stringResult = stringResult.Replace("[", " ");
            stringResult = stringResult.Replace("]", " ");

            var systemPrompt = $@"you will be provided with some jumbled information. Your task is to unjumble the information and provide a summary of the information. 
        so the thing is that the jumbled information is retrieved from the bing websearcher plugin in semantic kernel SDK and becuae the information is all jumbled up,
        we cannot simply provide it to the user, so you need to extract relevant information from the jumbled information and provide a summary of the information.
        you will be provided with both the jumbled information and the user query that was sent to the web searcher plugin of the semantic kernel SDK that caused such a jumbled information to be retrieved in the first place.
        ==============================================================
        Important Points:
        1)The user shouldn't get the jumbled information directly.
        2)The user should be able to understand the information.
        3) the user should not get to know that we used semantic kernel SDK to retrieve the information. it should appear as if you are the one that is providing the information.
        4) You should not include anything like ' Based on the user's query about the current stock prices of NVIDIA, the jumbled information from various sources can be summarized as follows:'
        ==============================================================
        the user query is : {userQuery}
        the jumbled information is :{stringResult}  ";

            var systemMessage = "you are a helpful AI assistant";

            OpenAIClient client = new OpenAIClient(new Uri(Secrets.openaiEndpoint!), new AzureKeyCredential(Secrets.openaiKey!));

            ChatCompletionsOptions chatCompletionsOptions = new ChatCompletionsOptions()
            {
                Messages =
            {
                new ChatRequestSystemMessage(systemMessage),
                new ChatRequestUserMessage(systemPrompt),
            },
                MaxTokens = 400,
                Temperature = 0.7f,
                DeploymentName = Secrets.openaiChatModel
            };

            ChatCompletions finalResponse = client.GetChatCompletions(chatCompletionsOptions);
            string completion = finalResponse.Choices[0].Message.Content;

            return completion;


        }

    }
}
