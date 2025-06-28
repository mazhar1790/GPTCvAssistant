using GPTCvAssistant.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPTCvAssistant.Controllers
{
    public class ChatController : Controller
    {
        private readonly OpenAIService _openAi;

        public ChatController(OpenAIService openAi)
        {
            _openAi = openAi;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var model = new ChatModel
            {
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

                var response = await _openAi.AskQuestionAsync(request.UserQuestion);

                var newExchange = new ChatExchange
                {
                    UserQuestion = request.UserQuestion,
                    Answer = response
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
                return Json(new { success = false, message = "An error occurred while processing your request" });
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
            catch (Exception ex)
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
                    sb.AppendLine($"GPT: {item.Answer}");
                    sb.AppendLine();
                }

                var bytes = Encoding.UTF8.GetBytes(sb.ToString());
                return File(bytes, "text/plain", "CV-GPT-Transcript.txt");
            }
            catch (Exception ex)
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
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Failed to retrieve history" });
            }
        }
    }

}