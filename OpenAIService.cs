using System.IO;
using System.Net.Http;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

namespace GPTCvAssistant
{
    public class OpenAIService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _apiEndpoint;
        private readonly string _modelName;
        private readonly string _cvPath;

        public OpenAIService(IOptions<OpenAISettings> options, IWebHostEnvironment env)
        {
            _apiKey = options.Value.ApiKey;
            _apiEndpoint = options.Value.ApiEndpoint;
            _modelName = options.Value.ModelName;
            _cvPath = Path.Combine(env.ContentRootPath, "App_Data", "ExtractedCV.txt");

            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
        }

        public async Task<string> AskQuestionAsync(string question)
        {
            // Read CV text from file
            string cvText = await File.ReadAllTextAsync(_cvPath);

            // Prepare payload for the API request
            var payload = new
            {
                model = _modelName,
                messages = new[]
                {
                    new { role = "system", content = "You are a professional CV assistant." },
                    new { role = "user", content = $"Use the following CV to answer:\n\n{cvText}\n\nQuestion: {question}" }
                }
            };

            // Serialize payload and send HTTP POST request
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(_apiEndpoint + "chat/completions", content);
            response.EnsureSuccessStatusCode();

            // Parse response and extract the answer
            using var responseStream = await response.Content.ReadAsStreamAsync();
            using var jsonDoc = await JsonDocument.ParseAsync(responseStream);
            return jsonDoc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();
        }
    }
}
