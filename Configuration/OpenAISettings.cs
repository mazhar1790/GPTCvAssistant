namespace GPTCvAssistant.Configuration
{
    /// <summary>
    /// Configuration settings for OpenAI service
    /// </summary>
    public class OpenAISettings
    {
        public const string SectionName = "OpenAI";
        
        public string ApiKey { get; set; } = string.Empty;
        public string ApiEndpoint { get; set; } = string.Empty;
        public string ModelName { get; set; } = "gpt-4o";
        public string EmbeddingModel { get; set; } = "text-embedding-ada-002";
    }
}