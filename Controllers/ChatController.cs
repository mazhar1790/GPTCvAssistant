using Ganss.Xss;
using GPTCvAssistant.Constants;
using GPTCvAssistant.Extensions;
using GPTCvAssistant.Models;
using GPTCvAssistant.Services;
using GPTCvAssistant.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.RegularExpressions;

namespace GPTCvAssistant.Controllers
{
    /// <summary>
    /// Main controller for handling chat interactions and career-related requests
    /// </summary>
    public class ChatController : Controller
    {
        private readonly IAiService _aiService;
        private readonly OpenAiService _openAiService; // Keep for specific scenarios
        private readonly IJobMatchingService _jobMatchingService;
        private readonly HtmlSanitizer _sanitizer;
        private readonly ILogger<ChatController> _logger;

        public ChatController(
            IAiService aiService,
            OpenAiService openAiService,
            IJobMatchingService jobMatchingService,
            ILogger<ChatController> logger)
        {
            _aiService = aiService ?? throw new ArgumentNullException(nameof(aiService));
            _openAiService = openAiService ?? throw new ArgumentNullException(nameof(openAiService));
            _jobMatchingService = jobMatchingService ?? throw new ArgumentNullException(nameof(jobMatchingService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _sanitizer = ConfigureHtmlSanitizer();
        }

        [HttpGet]
        public IActionResult Index()
        {
            var model = new ChatModel
            {
                SuggestedPrompts = AppConstants.DefaultSuggestions.Prompts,
                History = HttpContext.Session.GetObjectFromJson<List<ChatExchange>>(AppConstants.SessionKeys.ChatHistory) ?? new List<ChatExchange>()
            };
            return View("Index", model);
        }

        [HttpPost]
        public async Task<IActionResult> Chat([FromBody] ChatModel request)
        {
            try
            {
                if (request == null)
                {
                    return Json(new { success = false, message = "Request is required" });
                }

                _logger.LogInformation("Received chat request: Intent={Intent}, Question={Question}", 
                    request.Intent, request.UserQuestion ?? "null");

                // Validate input based on intent
                if (!ValidateRequest(request, out var validationMessage))
                {
                    return Json(new { success = false, message = validationMessage });
                }

                // Load existing chat history
                var history = HttpContext.Session.GetObjectFromJson<List<ChatExchange>>(AppConstants.SessionKeys.ChatHistory) ?? new List<ChatExchange>();

                // Process request based on intent
                var result = await ProcessChatRequest(request, history);
                
                return Json(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Chat method");
                return Json(new { success = false, message = $"An error occurred: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AnalyzeJob([FromBody] JobAnalysisRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request?.JobDescription))
                {
                    return Json(new { success = false, message = "Job description is required" });
                }

                var analysis = await _jobMatchingService.AnalyzeJobMatchAsync(request.JobDescription);
                var cleanHtml = _sanitizer.Sanitize(analysis.RawHtml);

                // Save to session history
                SaveToHistory($"Analyze Job: {request.TargetRole ?? "Position"}", cleanHtml);

                return Json(new { 
                    success = true, 
                    analysis = analysis,
                    html = cleanHtml,
                    intent = "JobAnalysis"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in AnalyzeJob");
                return Json(new { success = false, message = $"Error analyzing job: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> GenerateCoverLetter([FromBody] JobAnalysisRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request?.JobDescription))
                {
                    return Json(new { success = false, message = "Job description is required" });
                }

                var coverLetterHtml = await _jobMatchingService.GenerateTargetedCoverLetterAsync(
                    request.JobDescription, 
                    request.CompanyName ?? "");
                var cleanHtml = _sanitizer.Sanitize(coverLetterHtml);

                // Save to session history
                SaveToHistory($"Generate Cover Letter for {request.CompanyName ?? "Position"}", cleanHtml);

                return Json(new { 
                    success = true, 
                    html = cleanHtml,
                    intent = "GenerateCoverLetter"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GenerateCoverLetter");
                return Json(new { success = false, message = $"Error generating cover letter: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> OptimizeForATS([FromBody] JobAnalysisRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request?.JobDescription))
                {
                    return Json(new { success = false, message = "Job description is required" });
                }

                var optimizedSummary = await _jobMatchingService.GenerateATSOptimizedSummaryAsync(request.JobDescription);
                var keywords = await _jobMatchingService.ExtractATSKeywordsAsync(request.JobDescription);

                var combinedHtml = $@"
                    <h3>ATS-Optimized Professional Summary</h3>
                    {optimizedSummary}
                    
                    <h3>Critical ATS Keywords</h3>
                    <p><strong>Include these in your application:</strong> {string.Join(", ", keywords)}</p>
                ";

                var cleanHtml = _sanitizer.Sanitize(combinedHtml);

                // Save to session history
                SaveToHistory("Optimize for ATS", cleanHtml);

                return Json(new { 
                    success = true, 
                    html = cleanHtml,
                    keywords = keywords,
                    intent = "ATSOptimization"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in OptimizeForATS");
                return Json(new { success = false, message = $"Error optimizing for ATS: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Interview([FromBody] string targetRole)
        {
            try
            {
                var step = HttpContext.Session.GetInt32(AppConstants.SessionKeys.InterviewStep) ?? 0;
                var prompt = step == 0
                  ? $"Start an interview for {targetRole}. Ask one question. HTML only."
                  : "Continue. Ask next question based on previous answer. Give brief feedback first. HTML only.";
                
                var html = await _aiService.AskAsync(prompt);
                HttpContext.Session.SetInt32(AppConstants.SessionKeys.InterviewStep, step + 1);
                
                return Json(new { success = true, html = _sanitizer.Sanitize(html) });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Interview");
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> GenerateDoc([FromBody] dynamic req)
        {
            try
            {
                string type = req.type;
                string role = req.role;
                
                var instruction = type switch
                {
                    "cover" => $"Write a tailored cover letter for {role}. HTML only with <h3>, <p>, <ul>.",
                    "cv" => $"Rewrite CV highlights for {role} with quantified bullets. HTML only.",
                    _ => $"Rewrite LinkedIn 'About' for {role}. HTML only."
                };
                
                var html = await _openAiService.AskAsync(instruction);
                var clean = _sanitizer.Sanitize(html);
                
                return Json(new { success = true, html = clean });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GenerateDoc");
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult ClearHistory()
        {
            try
            {
                HttpContext.Session.Remove(AppConstants.SessionKeys.ChatHistory);
                return Json(new { success = true, message = "History cleared successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing history");
                return Json(new { success = false, message = "Failed to clear history" });
            }
        }

        [HttpPost]
        public IActionResult DownloadTranscript()
        {
            try
            {
                var history = HttpContext.Session.GetObjectFromJson<List<ChatExchange>>(AppConstants.SessionKeys.ChatHistory) ?? new List<ChatExchange>();

                if (!history.Any())
                {
                    return Json(new { success = false, message = "No chat history to download" });
                }

                var transcript = GenerateTranscript(history);
                var bytes = Encoding.UTF8.GetBytes(transcript);
                
                return File(bytes, "text/plain", AppConstants.FilePaths.TranscriptFileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error downloading transcript");
                return Json(new { success = false, message = "Failed to generate transcript" });
            }
        }

        [HttpGet]
        public IActionResult GetHistory()
        {
            try
            {
                var history = HttpContext.Session.GetObjectFromJson<List<ChatExchange>>(AppConstants.SessionKeys.ChatHistory) ?? new List<ChatExchange>();
                return Json(new { success = true, history = history });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving history");
                return Json(new { success = false, message = "Failed to retrieve history" });
            }
        }

        #region Private Methods

        private async Task<object> ProcessChatRequest(ChatModel request, List<ChatExchange> history)
        {
            string rawResponse;

            switch (request.Intent)
            {
                case RouteIntent.ClearHistory:
                    HttpContext.Session.Remove(AppConstants.SessionKeys.ChatHistory);
                    return new { success = true, message = "History cleared" };

                case RouteIntent.DownloadTranscript:
                    return DownloadTranscript();

                case RouteIntent.SearchHistory:
                    var matches = SearchHistory(history, request.UserQuestion!);
                    return new { success = true, history = matches };

                case RouteIntent.SuggestPrompts:
                    return new { success = true, suggestions = AppConstants.DefaultSuggestions.Prompts };

                case RouteIntent.CareerSummary:
                    rawResponse = await _aiService.AskAsync(AppConstants.PromptTemplates.CareerSummary);
                    break;

                case RouteIntent.SkillsHighlight:
                    rawResponse = await _aiService.AskAsync(AppConstants.PromptTemplates.SkillsHighlight);
                    break;

                default:
                    rawResponse = await _aiService.AskAsync(request.UserQuestion!);
                    break;
            }

            // Process the response
            var cleanedRaw = StripMarkdownCodeBlock(rawResponse);
            var cleanHtml = _sanitizer.Sanitize(cleanedRaw);

            var newExchange = new ChatExchange
            {
                UserQuestion = request.UserQuestion ?? $"[{request.Intent}]",
                Answer = cleanHtml
            };

            history.Add(newExchange);
            HttpContext.Session.SetObjectAsJson(AppConstants.SessionKeys.ChatHistory, history);

            return new
            {
                success = true,
                exchange = newExchange,
                totalCount = history.Count,
                intent = request.Intent.ToString()
            };
        }

        private static bool ValidateRequest(ChatModel request, out string validationMessage)
        {
            var requiresUserQuestion = request.Intent != RouteIntent.ClearHistory &&
                                     request.Intent != RouteIntent.DownloadTranscript &&
                                     request.Intent != RouteIntent.SuggestPrompts;

            if (requiresUserQuestion && string.IsNullOrWhiteSpace(request.UserQuestion))
            {
                validationMessage = "Question is required";
                return false;
            }

            validationMessage = string.Empty;
            return true;
        }

        private List<ChatExchange> SearchHistory(List<ChatExchange> history, string searchTerm)
        {
            return history
                .Where(h => h.UserQuestion.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
                         || h.Answer.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        private void SaveToHistory(string question, string answer)
        {
            var history = HttpContext.Session.GetObjectFromJson<List<ChatExchange>>(AppConstants.SessionKeys.ChatHistory) ?? new();
            history.Add(new ChatExchange { UserQuestion = question, Answer = answer });
            HttpContext.Session.SetObjectAsJson(AppConstants.SessionKeys.ChatHistory, history);
        }

        private static HtmlSanitizer ConfigureHtmlSanitizer()
        {
            var sanitizer = new HtmlSanitizer();

            foreach (var tag in AppConstants.AllowedHtmlElements.Tags)
                sanitizer.AllowedTags.Add(tag);

            foreach (var attr in AppConstants.AllowedHtmlElements.Attributes)
                sanitizer.AllowedAttributes.Add(attr);

            return sanitizer;
        }

        private static string StripMarkdownCodeBlock(string input)
        {
            var regex = new Regex(@"^```(?:html)?\s*([\s\S]*?)\s*```$", RegexOptions.Multiline);
            var match = regex.Match(input);
            return match.Success ? match.Groups[1].Value.Trim() : input;
        }

        private static string GenerateTranscript(List<ChatExchange> history)
        {
            var sb = new StringBuilder();
            sb.AppendLine("CV GPT Assistant - Chat Transcript");
            sb.AppendLine($"Generated on: {DateTime.Now:MMMM dd, yyyy 'at' h:mm tt}");
            sb.AppendLine(new string('=', 50));
            sb.AppendLine();

            foreach (var item in history)
            {
                sb.AppendLine($"You: {item.UserQuestion}");
                sb.AppendLine($"Assistant: {Regex.Replace(item.Answer, "<.*?>", string.Empty)}");
                sb.AppendLine();
            }

            return sb.ToString();
        }

        #endregion
    }
}
