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
    /// Service for interacting with OpenAI API with enhanced error handling and caching
    /// </summary>
    public class OpenAiService : IAiService
    {
        private readonly HttpClient _httpClient;
        private readonly OpenAISettings _settings;
        private readonly IMemoryCache _cache;
        private readonly ILogger<OpenAiService> _logger;
        private readonly string _cvPath;

        public string ServiceName => "OpenAI";

        public OpenAiService(
            IOptions<OpenAISettings> options, 
            IWebHostEnvironment env, 
            HttpClient httpClient,
            IMemoryCache cache,
            ILogger<OpenAiService> logger)
        {
            _settings = options.Value ?? throw new ArgumentNullException(nameof(options));
            _cvPath = Path.Combine(env.ContentRootPath, "App_Data", "ExtractedCV.txt");
            _httpClient = httpClient;
            _cache = cache;
            _logger = logger;

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("api-key", _settings.ApiKey);
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "GPTCvAssistant/2.0");
        }

        public async Task<string> AskAsync(string question, CancellationToken cancellationToken = default)
        {
            return await AskWithContextAsync(question, string.Empty, cancellationToken);
        }

        public async Task<string> AskWithContextAsync(string question, string context, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(question))
                throw new ArgumentException("Question cannot be empty", nameof(question));

            // Create cache key for frequently asked questions
            var cacheKey = $"openai_{question.GetHashCode()}_{context?.GetHashCode() ?? 0}";
            
            if (_cache.TryGetValue(cacheKey, out string? cachedResponse))
            {
                _logger.LogInformation("Returning cached response for question hash: {Hash}", question.GetHashCode());
                return cachedResponse;
            }

            try
            {
                if (!File.Exists(_cvPath))
                {
                    _logger.LogError("CV file not found at {Path}", _cvPath);
                    throw new FileNotFoundException($"CV file not found at {_cvPath}");
                }

                // Read CV text from local file
                var cvText = await File.ReadAllTextAsync(_cvPath, cancellationToken);

                // Create the system prompt
                var fullPrompt = CreateSystemPrompt(cvText, question, context);

                var requestBody = new
                {
                    model = _settings.ModelName,
                    messages = new[]
                    {
                        new { role = "system", content = "You are a professional career assistant." },
                        new { role = "user", content = fullPrompt }
                    },
                    max_tokens = 2048,
                    temperature = 0.7,
                    top_p = 0.95,
                    frequency_penalty = 0.1,
                    presence_penalty = 0.1
                };

                var requestJson = JsonSerializer.Serialize(requestBody);
                _logger.LogDebug("Sending request to OpenAI: {RequestSize} characters", requestJson.Length);

                var content = new StringContent(requestJson, Encoding.UTF8, "application/json");
                
                using var response = await _httpClient.PostAsync(_settings.ApiEndpoint, content, cancellationToken);
                
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
                using var jsonDoc = JsonDocument.Parse(responseContent);
                
                var result = ExtractResponseText(jsonDoc);

                // Cache the response for 10 minutes
                _cache.Set(cacheKey, result, TimeSpan.FromMinutes(10));

                _logger.LogInformation("Successfully processed OpenAI request. Response length: {Length}", result.Length);
                return result;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP request failed for OpenAI service");
                throw new InvalidOperationException("Failed to communicate with OpenAI service", ex);
            }
            catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
            {
                _logger.LogError(ex, "OpenAI request timed out");
                throw new TimeoutException("OpenAI service request timed out", ex);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to parse OpenAI response");
                throw new InvalidOperationException("Invalid response format from OpenAI service", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in OpenAI service");
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
                _logger.LogWarning(ex, "OpenAI health check failed");
                return false;
            }
        }

        private static string CreateSystemPrompt(string cvText, string question, string additionalContext = "")
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
                   {question}

                   Assistant:
                   """;
        }

        private static string ExtractResponseText(JsonDocument jsonDoc)
        {
            try
            {
                return jsonDoc.RootElement
                    .GetProperty("choices")[0]
                    .GetProperty("message")
                    .GetProperty("content")
                    .GetString() ?? "No response received";
            }
            catch (KeyNotFoundException)
            {
                throw new InvalidOperationException("Unexpected response format from OpenAI API");
            }
        }
    }
}