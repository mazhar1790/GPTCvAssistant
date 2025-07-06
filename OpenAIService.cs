using AngleSharp.Dom;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GPTCvAssistant
{
    public class OpenAiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _modelName;
        private readonly string _cvPath;
        private readonly string _apiEndpoint;

        public OpenAiService(IOptions<OpenAISettings> options, IWebHostEnvironment env, HttpClient httpClient)
        {
            _httpClient = httpClient;
            var apiKey = options.Value.ApiKey;
            _apiEndpoint = options.Value.ApiEndpoint;

            _modelName = options.Value.ModelName;
            _cvPath = Path.Combine(env.ContentRootPath, "App_Data", "ExtractedCV.txt");

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("api-key", apiKey);
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "AzureOpenAITest");
        }

        public async Task<string> AskQuestionAsync(string question)
        {
            // Read CV text from local file
            var cvText = await File.ReadAllTextAsync(_cvPath);

            // Refined system prompt
            var fullPrompt = $"""
                              You are a professional, human-like career assistant who is familiar with Mazhar Hayat's career and achievements. You've read his professional information thoroughly.

                              🧠 Behavior Rules:
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

            var requestBody = new
            {
                messages = new[]
                {
                    new { role = "user", content = fullPrompt }
                },
                temperature = 0.7,
                max_tokens = 300
            };

            var chatEndpoint = $"{_apiEndpoint}openai/deployments/{_modelName}/chat/completions?api-version=2024-02-15-preview";

            var response = await _httpClient.PostAsJsonAsync(chatEndpoint, requestBody);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                using var doc = JsonDocument.Parse(responseBody);
                var content = doc.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();
                return content;
            }
            
            // If response is not successful, return error message
            return $"Error: {response.StatusCode} - {responseBody}";
        }

        public async Task<string> TestChat()
        {
            string chatApiKey = "9rCqSjTo24z4dLOxqMlv3r94Sn3zCqH6Vo0k38TnFRXSEretLBprJQQJ99BDACHYHv6XJ3w3AAAAACOGFQW5";
            string chatEndpoint = "https://rushi-m9gm4fle-eastus2.openai.azure.com/";
            string chatDeployment = "gpt-4o";

            var url = $"{chatEndpoint}openai/deployments/{chatDeployment}/chat/completions?api-version=2024-02-15-preview";

            var requestBody = new
            {
                messages = new[]
                {
                    new { role = "user", content = "Who is USA president?" }
                },
                temperature = 0.7,
                max_tokens = 300
            };

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("api-key", chatApiKey);
            client.DefaultRequestHeaders.Add("User-Agent", "AzureOpenAITest");

            var response = await client.PostAsJsonAsync(url, requestBody);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                using var doc = JsonDocument.Parse(responseBody);
                var content = doc.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();
                return content;
            }
            return null;
        }
    }
}
