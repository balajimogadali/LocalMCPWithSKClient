using ModelContextProtocol.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCPServer.MCPTools
{
    [McpServerToolType]
    public static class DayOfWeekTool
    {
        [McpServerTool, Description("Get the current day of the week")]
        public static string GetDayOfWeek()
        {
            var today = DateTime.Now.ToString("dddd");
            return $"Today is {today}.";
        }
    }
}
