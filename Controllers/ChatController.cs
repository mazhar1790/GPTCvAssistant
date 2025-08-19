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
            var model = new ChatModel
            {
                SuggestedPrompts = GetSuggestions(),
                History = HttpContext.Session.GetObjectFromJson<List<ChatExchange>>("ChatHistory") ?? new List<ChatExchange>()
            };
            return View("Index", model);
        }

        [HttpPost]
        public async Task<IActionResult> Chat([FromBody] ChatModel request)
        {
            try
            {
                // First check if request itself is null
                if (request == null)
                {
                    return Json(new { success = false, message = "Request is required" });
                }

                // Log the incoming request for debugging
                Console.WriteLine($"Received request: Intent={request.Intent}, Question={request.UserQuestion ?? "null"}");

                // Check if UserQuestion is null or empty only for intents that require it
                var requiresUserQuestion = request.Intent != GPTCvAssistant.Models.RouteIntent.ClearHistory &&
                                         request.Intent != GPTCvAssistant.Models.RouteIntent.DownloadTranscript &&
                                         request.Intent != GPTCvAssistant.Models.RouteIntent.SuggestPrompts;

                if (requiresUserQuestion && string.IsNullOrWhiteSpace(request.UserQuestion))
                {
                    return Json(new { success = false, message = "Question is required" });
                }

                // Load existing chat history
                var history = HttpContext.Session.GetObjectFromJson<List<ChatExchange>>("ChatHistory") ?? new List<ChatExchange>();

                string rawResponse;

                // RouteIntent handling
                switch (request.Intent)
                {
                    case GPTCvAssistant.Models.RouteIntent.ClearHistory:
                        HttpContext.Session.Remove("ChatHistory");
                        return Json(new { success = true, message = "History cleared" });

                    case GPTCvAssistant.Models.RouteIntent.DownloadTranscript:
                        return DownloadTranscript();

                    case GPTCvAssistant.Models.RouteIntent.SearchHistory:
                        if (string.IsNullOrWhiteSpace(request.UserQuestion))
                            return Json(new { success = false, message = "Search term is required" });
                            
                        var matches = history
                            .Where(h => h.UserQuestion.Contains(request.UserQuestion, StringComparison.OrdinalIgnoreCase)
                                     || h.Answer.Contains(request.UserQuestion, StringComparison.OrdinalIgnoreCase))
                            .ToList();
                        return Json(new { success = true, history = matches });

                    case GPTCvAssistant.Models.RouteIntent.SuggestPrompts:
                        return Json(new { success = true, suggestions = GetSuggestions() });

                    case GPTCvAssistant.Models.RouteIntent.CareerSummary:
                        rawResponse = await _geminiService.AskAsync(@"
                                        Act as a Career Narrator.
                                        Return valid HTML only (<h3>, <p>, <ul>, <li>, <strong>).
                                        Summarize Mazhar Hayat's career as an AI Solutions Architect in Abu Dhabi.
                                        Focus on leadership, AI solutions, .NET, LLM, RAG, and Azure expertise.
                                        ");
                        break;

                    case GPTCvAssistant.Models.RouteIntent.SkillsHighlight:
                        rawResponse = await _geminiService.AskAsync(@"
                                        Act as a Technical Skills Highlighter.
                                        Return valid HTML only (<h3>, <p>, <ul>, <li>, <strong>).
                                        Highlight Mazhar Hayat's strongest technical skills:
                                        - .NET ecosystem
                                        - Large Language Models (LLM)
                                        - Retrieval-Augmented Generation (RAG)
                                        - Azure Cloud
                                        - AI-powered enterprise solutions
                                        ");
                        break;

                    default:
                        // Normal Q&A flow - UserQuestion already validated above for this case
                        if (string.IsNullOrWhiteSpace(request.UserQuestion))
                        {
                            return Json(new { success = false, message = "Question is required for default intent" });
                        }
                        
                        rawResponse = await _geminiService.AskAsync(request.UserQuestion);
                        break;
                }

                // Common post-processing for any response
                var cleanedRaw = StripMarkdownCodeBlock(rawResponse);
                var cleanHtml = _sanitizer.Sanitize(cleanedRaw);

                var newExchange = new ChatExchange
                {
                    UserQuestion = request.UserQuestion ?? $"[{request.Intent}]", // Use intent name if no question
                    Answer = cleanHtml
                };

                history.Add(newExchange);
                HttpContext.Session.SetObjectAsJson("ChatHistory", history);

                // Enhanced debugging information in response
                var intentString = request.Intent.ToString();
                Console.WriteLine($"Responding with intent: {intentString}");

                return Json(new
                {
                    success = true,
                    exchange = newExchange,
                    totalCount = history.Count,
                    intent = intentString, // Explicitly using ToString() for clarity
                    debug = new { 
                        intentType = request.Intent.GetType().FullName,
                        intentValue = (int)request.Intent
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Chat method: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return Json(new { success = false, message = $"An error occurred: {ex.Message}" });
            }
        }





        [HttpPost]
        public async Task<IActionResult> AnalyzeJD([FromBody] string jobDescription)
        {
            var cvHtml = await _geminiService.AskAsync($@"
                        Act as a Job Match Agent.
                        Return valid HTML only.
                        Tasks:
                        - Extract core requirements from this JD.
                        - Compare with Mazhar's CV.
                        - Output: <h3>Match Summary</h3>, <ul>Strengths</ul>, <ul>Gaps</ul>, <h3>ATS Keywords</h3>, and a short <p>Pitch</p>.
                        JD:
                        {jobDescription}");
            var clean = _sanitizer.Sanitize(cvHtml); // sanitizer already configured for h3, ul, li, p, strong :contentReference[oaicite:15]{index=15}
                                                     // Save to session history like Chat()
            var history = HttpContext.Session.GetObjectFromJson<List<ChatExchange>>("ChatHistory") ?? new();
            history.Add(new ChatExchange { UserQuestion = "Analyze this JD", Answer = clean });
            HttpContext.Session.SetObjectAsJson("ChatHistory", history); // :contentReference[oaicite:16]{index=16}
            return Json(new { success = true, html = clean });
        }

        [HttpPost]
        public async Task<IActionResult> Interview([FromBody] string targetRole)
        {
            // Pull progress from session
            var step = HttpContext.Session.GetInt32("InterviewStep") ?? 0;
            var prompt = step == 0
              ? $"Start an interview for {targetRole}. Ask one question. HTML only."
              : "Continue. Ask next question based on previous answer. Give brief feedback first. HTML only.";
            var html = await _geminiService.AskAsync(prompt); // LLM returns HTML by design:contentReference[oaicite:17]{index=17}
            HttpContext.Session.SetInt32("InterviewStep", step + 1);
            return Json(new { success = true, html = _sanitizer.Sanitize(html) });
        }

        [HttpPost]
        public async Task<IActionResult> GenerateDoc([FromBody] dynamic req) // { type:'cover'|'cv'|'linkedin', role:'...' }
        {
            string type = req.type; string role = req.role;
            var instruction = type switch
            {
                "cover" => $"Write a tailored cover letter for {role}. HTML only with <h3>, <p>, <ul>.",
                "cv" => $"Rewrite CV highlights for {role} with quantified bullets. HTML only.",
                _ => $"Rewrite LinkedIn 'About' for {role}. HTML only."
            };
            var html = await _openAi.AskQuestionAsync(instruction); // OpenAI path also returns HTML per rules:contentReference[oaicite:18]{index=18}
            var clean = _sanitizer.Sanitize(html); // :contentReference[oaicite:19]{index=19}
            return Json(new { success = true, html = clean });
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

        // Intent router: decide which agent behavior to trigger
        private string DetermineIntent(string q)
        {
            if (string.IsNullOrWhiteSpace(q)) return "qa";

            var s = q.ToLowerInvariant();

            // Job Description / Matching
            if (Regex.IsMatch(s, @"\b(jd|job\s*description|role\s*requirements|match\s*this\s*job|ats)\b"))
                return "jd";

            // Interview simulation
            if (Regex.IsMatch(s, @"\b(interview\s*me|mock\s*interview|ask\s*me\s*questions|practice\s*interview)\b"))
                return "interview";

            // Document generation (cover letter / CV variant / LinkedIn)
            if (Regex.IsMatch(s, @"\b(cover\s*letter|tailored\s*cv|cv\s*variant|linkedin|about\s*section|summary\s*for\s*linkedin)\b"))
                return "gen";

            // Default: normal Q&A
            return "qa";
        }

        private List<string> GetSuggestions()
        {
            return new List<string>
                {
                    "Summarize Mazhar’s career as an AI Solutions Architect.",
                    "Highlight Mazhar’s expertise with .NET, LLM, RAG, and Azure.",
                    "What leadership and team roles has Mazhar taken in Abu Dhabi?",
                    "Explain Mazhar’s experience in building AI-powered enterprise solutions.",
                    "Generate a quick overview of Mazhar’s projects in data and AI.",
                    "How does Mazhar apply RAG techniques in real-world systems?",
                    "What makes Mazhar a strong fit for AI architect roles?"
               };
        }



    }
}
