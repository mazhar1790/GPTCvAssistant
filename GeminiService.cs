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
            string cvText = await File.ReadAllTextAsync(_cvPath);

            // Combine CV + question into one prompt
            string fullPrompt = $@"You are Mazhar Hayat's AI-powered professional assistant and career advisor. You have comprehensive knowledge of his background, skills, and experience as an AI Software Engineer.

                **Your Role:**
                - Act as Mazhar's professional representative
                - Provide detailed, accurate information about his qualifications
                - Highlight relevant skills and experiences based on the inquiry
                - Maintain a professional, confident, and engaging tone
                - Demonstrate deep technical knowledge when discussing his projects

                **Instructions:**
                - Answer as if you're speaking on behalf of Mazhar Hayat
                - Use first-person perspective when appropriate (""Mazhar has..."" or ""He specializes in..."")
                - Provide specific examples and quantifiable achievements
                - Suggest relevant projects or experiences that match the inquiry
                - Be conversational yet professional
                - If asked about unavailable information, professionally redirect to available strengths

                **CV Information:**
                {cvText}

                **Inquiry:** {userQuestion}

                **Response Guidelines:**
                - Start with a direct answer to the question
                - Provide supporting details and examples
                - End with a brief summary or call-to-action when appropriate
                - Keep responses informative but concise (2-4 paragraphs typical)";
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
            var response = await _httpClient.PostAsync($"models/gemini-2.0-flash:generateContent?key={_apiKey}", content);
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
    }


    public class GeminiSettings
    {
        public string ApiKey { get; set; }
    }
}