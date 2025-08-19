using System.ComponentModel.DataAnnotations;

namespace GPTCvAssistant.Configuration
{
    /// <summary>
    /// Configuration settings for OpenAI service with validation
    /// </summary>
    public class OpenAISettings : IValidatableObject
    {
        public const string SectionName = "OpenAI";

        [Required(ErrorMessage = "OpenAI API Key is required")]
        [MinLength(10, ErrorMessage = "API Key must be at least 10 characters long")]
        public string ApiKey { get; set; } = string.Empty;

        [Required(ErrorMessage = "OpenAI API Endpoint is required")]
        [Url(ErrorMessage = "API Endpoint must be a valid URL")]
        public string ApiEndpoint { get; set; } = string.Empty;

        [Required(ErrorMessage = "Model Name is required")]
        public string ModelName { get; set; } = string.Empty;

        public string EmbeddingModel { get; set; } = string.Empty;

        [Range(0.0, 2.0, ErrorMessage = "Temperature must be between 0.0 and 2.0")]
        public double Temperature { get; set; } = 0.7;

        [Range(1, 4096, ErrorMessage = "Max tokens must be between 1 and 4096")]
        public int MaxTokens { get; set; } = 2048;

        [Range(30, 300, ErrorMessage = "Timeout must be between 30 and 300 seconds")]
        public int TimeoutSeconds { get; set; } = 120;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            if (!string.IsNullOrEmpty(ApiKey) && ApiKey.Contains("your-api-key"))
            {
                results.Add(new ValidationResult(
                    "Please replace the placeholder API key with your actual OpenAI API key",
                    new[] { nameof(ApiKey) }));
            }

            if (!string.IsNullOrEmpty(ApiEndpoint) && !ApiEndpoint.Contains("openai"))
            {
                results.Add(new ValidationResult(
                    "API Endpoint does not appear to be a valid OpenAI endpoint",
                    new[] { nameof(ApiEndpoint) }));
            }

            return results;
        }
    }
}