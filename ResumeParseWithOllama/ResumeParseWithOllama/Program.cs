using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ResumeParseWithOllama
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            string pdfPath = @"Resumes\BalajiMogadali_Profile.pdf";
            var text = new StringBuilder();

            using (var pdfReader = new PdfReader(pdfPath))
            using (var pdfDoc = new PdfDocument(pdfReader))
            {
                for (int page = 1; page <= pdfDoc.GetNumberOfPages(); page++)
                {
                    var strategy = new SimpleTextExtractionStrategy();
                    string pageText = PdfTextExtractor.GetTextFromPage(pdfDoc.GetPage(page), strategy);
                    text.AppendLine(pageText);
                }
            }

            var resume = text.ToString();
            var job = "Required: - .Net development experience  - Azure cloud experience";

            var prompt = $@"
                            You are evaluating how well a resume matches a job description.

                            Please perform the following:
                            1. Give a **match score** as a percentage from 0 to 100.
                            2. Determine the **match level** using the following guidelines:
                               - 90–100% = Excellent match
                               - 70–89% = Good match
                               - 50–69% = Fair match
                               - Below 50% = Poor match
                            3. Provide a **short explanation** for the rating.
                            4. Provide a **summary** comparing the resume's qualifications to the job requirements.

                            Return the result in **valid JSON** format like this:

                            ```json
                            {{
                            ""match_score_percent"": 0 to 100,
                              ""match_level"": ""Excellent | Good | Fair | Poor"",
                              ""summary"": ""A brief overview of how well the resume aligns with the job."",
                              ""explanation"": ""Short explanation for the score and reasoning behind the match level."",
                              ""sentiment"": ""positive | neutral | negative""
                            }}

                            Resume:
                            {resume}

                            Job Description:
                            {job}";
            var content = new
            {
                model = "llama3.1:8b", // or llama3, mistral
                messages = new[] {
                new { role = "user", content = prompt }
            },
                stream = false
            };

            var httpClient = new HttpClient();
            var requestContent = new StringContent(JsonSerializer.Serialize(content), Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync("http://localhost:11434/api/chat", requestContent);
            var responseBody = await response.Content.ReadAsStringAsync();

            //Console.WriteLine("Raw Output:");
            //Console.WriteLine(responseBody);


            // Optionally, parse the returned JSON text inside the 'message.content' field
            var result = JsonDocument.Parse(responseBody);
            string jsonAnswer = result.RootElement.GetProperty("message").GetProperty("content").ToString();

            //Console.WriteLine("Parsed Output:");
            //Console.WriteLine(jsonAnswer);

            // Clean jsonAnswer: remove code block markers and trim whitespace
            jsonAnswer = Regex.Replace(jsonAnswer, @"^```json|^```|```$", "", RegexOptions.Multiline).Trim();

            // Optionally, remove any leading/trailing backticks or whitespace
            jsonAnswer = jsonAnswer.Trim('`', ' ', '\n', '\r');


            // Parse the JSON answer
            using var doc = JsonDocument.Parse(jsonAnswer);
            string summary = doc.RootElement.GetProperty("summary").GetString() ?? "";

            // Determine sentiment and match_level based on summary content
            string sentiment;
            string matchLevel;

            // Simple keyword-based logic (customize as needed)
            if (Regex.IsMatch(summary, @"excellent|outstanding|perfect|strong match", RegexOptions.IgnoreCase))
            {
                sentiment = "positive";
                matchLevel = "Excellent";
            }
            else if (Regex.IsMatch(summary, @"good|solid|suitable|well matched", RegexOptions.IgnoreCase))
            {
                sentiment = "positive";
                matchLevel = "Good";
            }
            else if (Regex.IsMatch(summary, @"fair|adequate|somewhat|partial", RegexOptions.IgnoreCase))
            {
                sentiment = "neutral";
                matchLevel = "Fair";
            }
            else
            {
                sentiment = "negative";
                matchLevel = "Poor";
            }

            Console.WriteLine($"Sentiment: {sentiment}");
            Console.WriteLine($"Match Level: {matchLevel}");
        }
    }
}
