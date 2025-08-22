using GPTCvAssistant.Configuration;
using GPTCvAssistant.Services.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GPTCvAssistant.Services
{
    /// <summary>
    /// Service for interacting with Google's Gemini AI API with enhanced error handling and caching
    /// </summary>
    public class GeminiService : IAiService
    {
        private readonly HttpClient _httpClient;
        private readonly GeminiSettings _settings;
        private readonly IMemoryCache _cache;
        private readonly ILogger<GeminiService> _logger;
        private readonly string _cvPath;

        public string ServiceName => "Gemini";

        public GeminiService(
            IOptions<GeminiSettings> options, 
            IWebHostEnvironment env, 
            HttpClient httpClient,
            IMemoryCache cache,
            ILogger<GeminiService> logger)
        {
            _settings = options.Value ?? throw new ArgumentNullException(nameof(options));
            _cvPath = Path.Combine(env.ContentRootPath, "App_Data", "ExtractedCV.txt");
            _httpClient = httpClient;
            _cache = cache;
            _logger = logger;
            
            _httpClient.BaseAddress = new Uri(_settings.BaseUrl);
        }

        public async Task<string> AskAsync(string userQuestion, CancellationToken cancellationToken = default)
        {
            return await AskWithContextAsync(userQuestion, string.Empty, cancellationToken);
        }

        public async Task<string> AskWithContextAsync(string userQuestion, string context, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(userQuestion))
                throw new ArgumentException("Question cannot be empty", nameof(userQuestion));

            // Create cache key for frequently asked questions
            var cacheKey = $"gemini_{userQuestion.GetHashCode()}_{context?.GetHashCode() ?? 0}";
            
            if (_cache.TryGetValue(cacheKey, out string? cachedResponse))
            {
                _logger.LogInformation("Returning cached response for question hash: {Hash}", userQuestion.GetHashCode());
                return cachedResponse;
            }

            try
            {
                if (!File.Exists(_cvPath))
                {
                    _logger.LogError("CV file not found at {Path}", _cvPath);
                    throw new FileNotFoundException($"CV file not found at {_cvPath}");
                }

                // Read CV content from file
                var cvText = await File.ReadAllTextAsync(_cvPath, cancellationToken);

                // Create the prompt with proper system instructions
                var fullPrompt = CreateSystemPrompt(cvText, userQuestion, context);

                var request = new
                {
                    contents = new[]
                    {
                        new
                        {
                            parts = new[]
                            {
                                new { text = fullPrompt }
                            }
                        }
                    },
                    generationConfig = new
                    {
                        temperature = 0.7,
                        topK = 40,
                        topP = 0.95,
                        maxOutputTokens = 2048
                    }
                };

                var requestJson = JsonSerializer.Serialize(request);
                _logger.LogDebug("Sending request to Gemini: {RequestSize} characters", requestJson.Length);

                var content = new StringContent(requestJson, Encoding.UTF8, "application/json");
                
                using var response = await _httpClient.PostAsync(
                    $"models/{_settings.ModelName}:generateContent?key={_settings.ApiKey}", 
                    content, 
                    cancellationToken);
                
                response.EnsureSuccessStatusCode();

                var responseStream = await response.Content.ReadAsStreamAsync(cancellationToken);
                using var jsonDoc = await JsonDocument.ParseAsync(responseStream, cancellationToken: cancellationToken);

                var result = ExtractResponseText(jsonDoc);
                var cleanedResult = CleanUpResponse(result);

                // Cache the response for 10 minutes
                _cache.Set(cacheKey, cleanedResult, TimeSpan.FromMinutes(10));

                _logger.LogInformation("Successfully processed Gemini request. Response length: {Length}", cleanedResult.Length);
                return cleanedResult;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP request failed for Gemini service");
                throw new InvalidOperationException("Failed to communicate with Gemini AI service", ex);
            }
            catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
            {
                _logger.LogError(ex, "Gemini request timed out");
                throw new TimeoutException("Gemini AI service request timed out", ex);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to parse Gemini response");
                throw new InvalidOperationException("Invalid response format from Gemini AI service", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in Gemini service");
                throw;
            }
        }

        public async Task<bool> IsHealthyAsync()
        {
            try
            {
                const string healthCheckQuestion = "Hello";
                var response = await AskAsync(healthCheckQuestion, CancellationToken.None);
                return !string.IsNullOrEmpty(response);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Gemini health check failed");
                return false;
            }
        }

        private static string CreateSystemPrompt(string cvText, string userQuestion, string additionalContext = "")
        {
            var contextSection = !string.IsNullOrEmpty(additionalContext) 
                ? $"\n\nAdditional Context:\n{additionalContext}\n" 
                : "";

            return $"""
                   You are a professional, human-like career assistant who is familiar with Mazhar Hayat's career and achievements. You've read his professional information thoroughly.

                   ?? Behavior Rules:
                   - Respond as if you're speaking from personal familiarity ? not reading a file.
                   - If the user asks "show everything", instead summarize the key highlights, categories, or suggest follow-ups.
                   - If the user question is broad (e.g., "tell me everything"), provide a concise overview and guide them to ask about specific areas such as experience, projects, skills, or education.
                   - DO NOT dump the full content unless the user explicitly asks for a section (e.g., "show all experience").
                   - Avoid saying things like "in the profile" or "according to the CV".
                   - Keep the tone polished and thoughtful.
                   - Always reply in the same language used by the user.
                   - Return semantic HTML (h3, ul, li, p, strong), close all tags properly, no script/style tags.
                   - You MUST return your answers in valid HTML.
                   - Do NOT use Markdown (e.g., no *, no ###).
                   - Use proper HTML tags: <h3>, <p>, <ul>, <li>, <strong>.
                   - Every tag must be closed properly.
                   - Do NOT return Markdown code blocks or wrap the HTML in triple backticks (```html). Just return raw HTML.
                   - Your output will be directly rendered as HTML on a website.
                   - IMPORTANT: Do not use emoji characters in your response. Use text labels instead.

                   Professional Info:
                   {cvText}
                   {contextSection}
                   User:
                   {userQuestion}

                   Assistant:
                   """;
        }

        private static string ExtractResponseText(JsonDocument jsonDoc)
        {
            try
            {
                return jsonDoc.RootElement
                    .GetProperty("candidates")[0]
                    .GetProperty("content")
                    .GetProperty("parts")[0]
                    .GetProperty("text")
                    .GetString() ?? string.Empty;
            }
            catch (KeyNotFoundException)
            {
                throw new InvalidOperationException("Unexpected response format from Gemini API");
            }
        }

        private static string CleanUpResponse(string response)
        {
            if (string.IsNullOrEmpty(response))
                return response;

            // Remove leading/trailing Markdown code block markers
            response = System.Text.RegularExpressions.Regex.Replace(
                response,
                @"^```(?:html)?[\r\n]+|```$",
                string.Empty,
                System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.Multiline
            );

            // Fix common encoding issues
            return response
                .Replace("??", "") // Remove question mark artifacts
                .Replace("'", "'") // Fix apostrophe encoding
                .Replace("\u201C", "\"") // Fix left quote encoding
                .Replace("\u201D", "\"") // Fix right quote encoding
                .Replace("•", "•") // Fix bullet point encoding
                .Trim();
        }
    }
}