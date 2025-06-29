using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GPTCvAssistant
{
    public class GeminiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _cvPath;

        public GeminiService(IOptions<GeminiSettings> options, IWebHostEnvironment env)
        {
            _apiKey = options.Value.ApiKey;
            _cvPath = Path.Combine(env.ContentRootPath, "App_Data", "ExtractedCV.txt");

            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://generativelanguage.googleapis.com/v1beta/")
            };
        }

        public async Task<string> AskAsync(string userQuestion)
        {
            // Read CV content from file
            var cvText = await File.ReadAllTextAsync(_cvPath);

            // Refined system prompt
            var fullPrompt = $"""
                              You are a professional, human-like career assistant who is familiar with Mazhar Hayat’s career and achievements. You’ve read his professional information thoroughly.

                              🧠 Behavior Rules:
                              - Respond as if you're speaking from personal familiarity — not reading a file.
                              - If the user asks "show everything", instead summarize the key highlights, categories, or suggest follow-ups.
                              - If the user question is broad (e.g., “tell me everything”), provide a concise overview and guide them to ask about specific areas such as experience, projects, skills, or education.
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
                              {userQuestion}

                              Assistant:
                              """;



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
                }
            };

            var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
            var response =
                await _httpClient.PostAsync($"models/gemini-2.0-flash:generateContent?key={_apiKey}", content);
            response.EnsureSuccessStatusCode();

            var responseStream = await response.Content.ReadAsStreamAsync();
            using var jsonDoc = await JsonDocument.ParseAsync(responseStream);

            return jsonDoc.RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString();
        }



        public class GeminiSettings
        {
            public string ApiKey { get; set; }
        }
    }
}