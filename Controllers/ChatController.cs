using Ganss.Xss;
using GPTCvAssistant.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GPTCvAssistant.Controllers
{
    public class ChatController : Controller
    {
        private readonly OpenAiService _openAi;
        private readonly GeminiService _geminiService;
        private readonly HtmlSanitizer _sanitizer;

        public ChatController(OpenAiService openAi, GeminiService geminiService)
        {
            _openAi = openAi;
            _geminiService = geminiService;

            _sanitizer = new HtmlSanitizer();
            _sanitizer.AllowedTags.Add("h1");
            _sanitizer.AllowedTags.Add("h2");
            _sanitizer.AllowedTags.Add("h3");
            _sanitizer.AllowedTags.Add("ul");
            _sanitizer.AllowedTags.Add("li");
            _sanitizer.AllowedTags.Add("strong");
            _sanitizer.AllowedTags.Add("em");
            _sanitizer.AllowedTags.Add("p");
            _sanitizer.AllowedTags.Add("br");
        }

        [HttpGet]
        public IActionResult Index()
        {

            var suggestions = new List<string>
            {
                "Summarize Mazhar professional experience.",
                "What are Mazhar strongest technical skills?",
                "What types of roles best match Mazhar's background?",
                "List key projects Mazhar has worked on.",
                "Describe Mazhar experience with AI or GPT technologies.",
                "What leadership or team roles has Mazhar taken?",
                "How is Mazhar experienced in software architecture?",
                "Highlight Mazhar full stack development experience.",
                "What industries has Mazhar worked in?",
                "Give a quick overview of Mazhar's career."
            };

            var model = new ChatModel
            {
                SuggestedPrompts = suggestions,
                History = HttpContext.Session.GetObjectFromJson<List<ChatExchange>>("ChatHistory") ?? new List<ChatExchange>()
            };
            return View("Index", model);
        }

        [HttpPost]
        public async Task<IActionResult> Chat([FromBody] ChatModel request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request?.UserQuestion))
                {
                    return Json(new { success = false, message = "Question is required" });
                }

                var history = HttpContext.Session.GetObjectFromJson<List<ChatExchange>>("ChatHistory") ?? new List<ChatExchange>();

                var rawResponse = await _geminiService.AskAsync(request.UserQuestion);

                // ⬇️ Remove triple backticks if accidentally added by Gemini
                var cleanedRaw = StripMarkdownCodeBlock(rawResponse);

                // ⬇️ Sanitize for safe rendering
                var cleanHtml = _sanitizer.Sanitize(cleanedRaw);


                var newExchange = new ChatExchange
                {
                    UserQuestion = request.UserQuestion,
                    Answer = cleanHtml
                };

                history.Add(newExchange);
                HttpContext.Session.SetObjectAsJson("ChatHistory", history);

                return Json(new
                {
                    success = true,
                    exchange = newExchange,
                    totalCount = history.Count
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult ClearHistory()
        {
            try
            {
                HttpContext.Session.Remove("ChatHistory");
                return Json(new { success = true, message = "History cleared successfully" });
            }
            catch
            {
                return Json(new { success = false, message = "Failed to clear history" });
            }
        }

        [HttpPost]
        public IActionResult DownloadTranscript()
        {
            try
            {
                var history = HttpContext.Session.GetObjectFromJson<List<ChatExchange>>("ChatHistory") ?? new List<ChatExchange>();

                if (!history.Any())
                {
                    return Json(new { success = false, message = "No chat history to download" });
                }

                var sb = new StringBuilder();
                sb.AppendLine("CV GPT Assistant - Chat Transcript");
                sb.AppendLine($"Generated on: {DateTime.Now:MMMM dd, yyyy 'at' h:mm tt}");
                sb.AppendLine(new string('=', 50));
                sb.AppendLine();

                foreach (var item in history)
                {
                    sb.AppendLine($"You: {item.UserQuestion}");
                    sb.AppendLine($"Assistant: {System.Text.RegularExpressions.Regex.Replace(item.Answer, "<.*?>", string.Empty)}");
                    sb.AppendLine();
                }

                var bytes = Encoding.UTF8.GetBytes(sb.ToString());
                return File(bytes, "text/plain", "CV-GPT-Transcript.txt");
            }
            catch
            {
                return Json(new { success = false, message = "Failed to generate transcript" });
            }
        }

        [HttpGet]
        public IActionResult GetHistory()
        {
            try
            {
                var history = HttpContext.Session.GetObjectFromJson<List<ChatExchange>>("ChatHistory") ?? new List<ChatExchange>();
                return Json(new { success = true, history = history });
            }
            catch
            {
                return Json(new { success = false, message = "Failed to retrieve history" });
            }
        }

        private string StripMarkdownCodeBlock(string input)
        {
            var regex = new Regex(@"^```(?:html)?\s*([\s\S]*?)\s*```$", RegexOptions.Multiline);
            var match = regex.Match(input);
            return match.Success ? match.Groups[1].Value.Trim() : input;
        }

    }
}
