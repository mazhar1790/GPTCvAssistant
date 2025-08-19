namespace GPTCvAssistant.Configuration
{
    /// <summary>
    /// Configuration settings for Gemini service
    /// </summary>
    public class GeminiSettings
    {
        public const string SectionName = "Gemini";
        
        public string ApiKey { get; set; } = string.Empty;
        public string BaseUrl { get; set; } = "https://generativelanguage.googleapis.com/v1beta/";
        public string ModelName { get; set; } = "gemini-2.0-flash";
    }
}