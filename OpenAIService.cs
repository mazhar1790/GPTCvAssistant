using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
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

        public OpenAiService(IOptions<OpenAISettings> options, IWebHostEnvironment env)
        {
            var apiKey = options.Value.ApiKey;
            var apiEndpoint = options.Value.ApiEndpoint; // Expected: "https://api.openai.com/v1/"
            _modelName = options.Value.ModelName;
            _cvPath = Path.Combine(env.ContentRootPath, "App_Data", "ExtractedCV.txt");

            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(apiEndpoint)
            };
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
        }

        public async Task<string> AskQuestionAsync(string question)
        {
            // Read CV text from local file
            string cvText = await File.ReadAllTextAsync(_cvPath);

            // Compose OpenAI chat request payload
            var payload = new
            {
                model = _modelName, // e.g., "gpt-4o"
                messages = new[]
                {
                    new { role = "system", content = "You are a professional CV assistant. Answer clearly and professionally." },
                    new { role = "user", content = $"Use the following CV to answer:\n\n{cvText}\n\nQuestion: {question}" }
                }
            };

            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            // Send request to OpenAI
            var response = await _httpClient.PostAsync("chat/completions", content);
            response.EnsureSuccessStatusCode();

            // Parse and return the assistant's reply
            await using var responseStream = await response.Content.ReadAsStreamAsync();
            using var jsonDoc = await JsonDocument.ParseAsync(responseStream);
            return jsonDoc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();
        }
    }
}
