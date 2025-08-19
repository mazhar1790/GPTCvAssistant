using GPTCvAssistant.Configuration;
using GPTCvAssistant.Services.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GPTCvAssistant.Services
{
    /// <summary>
    /// Service for interacting with OpenAI API
    /// </summary>
    public class OpenAiService : IAiService
    {
        private readonly HttpClient _httpClient;
        private readonly OpenAISettings _settings;
        private readonly string _cvPath;

        public OpenAiService(IOptions<OpenAISettings> options, IWebHostEnvironment env, HttpClient httpClient)
        {
            _settings = options.Value ?? throw new ArgumentNullException(nameof(options));
            _cvPath = Path.Combine(env.ContentRootPath, "App_Data", "ExtractedCV.txt");
            _httpClient = httpClient;

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("api-key", _settings.ApiKey);
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "GPTCvAssistant");
        }

        public async Task<string> AskAsync(string question)
        {
            return await AskQuestionAsync(question);
        }

        public async Task<string> AskQuestionAsync(string question)
        {
            if (string.IsNullOrWhiteSpace(question))
                throw new ArgumentException("Question cannot be empty", nameof(question));

            if (!File.Exists(_cvPath))
                throw new FileNotFoundException($"CV file not found at {_cvPath}");

            // Read CV text from local file
            var cvText = await File.ReadAllTextAsync(_cvPath);

            // Create the system prompt
            var fullPrompt = CreateSystemPrompt(cvText, question);

            var requestBody = new
            {
                model = _settings.ModelName,
                messages = new[]
                {
                    new { role = "system", content = "You are a professional career assistant." },
                    new { role = "user", content = fullPrompt }
                },
                max_tokens = 1500,
                temperature = 0.7
            };

            var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(_settings.ApiEndpoint, content);
            
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            using var jsonDoc = JsonDocument.Parse(responseContent);
            
            return jsonDoc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString() ?? "No response received";
        }

        public async Task<string> TestChat()
        {
            return await AskQuestionAsync("Hello, can you briefly introduce yourself?");
        }

        private static string CreateSystemPrompt(string cvText, string question)
        {
            return $"""
                   You are a professional, human-like career assistant who is familiar with Mazhar Hayat's career and achievements. You've read his professional information thoroughly.

                   ?? Behavior Rules:
                   - Respond as if you're speaking from personal familiarity — not reading a file.
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

                   Professional Info:
                   {cvText}

                   User:
                   {question}

                   Assistant:
                   """;
        }
    }
}