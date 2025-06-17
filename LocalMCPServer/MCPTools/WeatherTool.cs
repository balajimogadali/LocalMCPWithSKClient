using ModelContextProtocol.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LocalMCPServer.MCPTools
{
    [McpServerToolType]
    public static class WeatherTool
    {
        static readonly HttpClient httpClient = new();

        [McpServerTool, Description("Get the current weather in a city")]
        public static async Task<string> GetWeatherAsync(string city)
        {
            var apiKey = "602802ab119b47928e1160452251606";
            var url = $"http://api.weatherapi.com/v1/current.json?key={apiKey}&q={Uri.EscapeDataString(city)}";

            try
            {
                var json = await httpClient.GetStringAsync(url);
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                var tempC = root.GetProperty("current").GetProperty("temp_c").GetDouble();
                var condition = root.GetProperty("current").GetProperty("condition").GetProperty("text").GetString();

                return $"It's {tempC}°C and {condition?.ToLower()} in {city}.";
            }
            catch (Exception ex)
            {
                return $"Sorry, I couldn't fetch the weather for {city}. ({ex.Message})";
            }
        }
    }
}
