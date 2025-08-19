using System.ComponentModel.DataAnnotations;

namespace GPTCvAssistant.Configuration
{
    /// <summary>
    /// Configuration settings for Google Gemini service with validation
    /// </summary>
    public class GeminiSettings : IValidatableObject
    {
        public const string SectionName = "Gemini";

        [Required(ErrorMessage = "Gemini API Key is required")]
        [MinLength(20, ErrorMessage = "API Key must be at least 20 characters long")]
        public string ApiKey { get; set; } = string.Empty;

        [Required(ErrorMessage = "Gemini Base URL is required")]
        [Url(ErrorMessage = "Base URL must be a valid URL")]
        public string BaseUrl { get; set; } = string.Empty;

        [Required(ErrorMessage = "Model Name is required")]
        public string ModelName { get; set; } = string.Empty;

        [Range(0.0, 2.0, ErrorMessage = "Temperature must be between 0.0 and 2.0")]
        public double Temperature { get; set; } = 0.7;

        [Range(1, 100, ErrorMessage = "TopK must be between 1 and 100")]
        public int TopK { get; set; } = 40;

        [Range(0.0, 1.0, ErrorMessage = "TopP must be between 0.0 and 1.0")]
        public double TopP { get; set; } = 0.95;

        [Range(1, 8192, ErrorMessage = "Max output tokens must be between 1 and 8192")]
        public int MaxOutputTokens { get; set; } = 2048;

        [Range(30, 300, ErrorMessage = "Timeout must be between 30 and 300 seconds")]
        public int TimeoutSeconds { get; set; } = 120;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            if (!string.IsNullOrEmpty(ApiKey) && ApiKey.Contains("your-api-key"))
            {
                results.Add(new ValidationResult(
                    "Please replace the placeholder API key with your actual Gemini API key",
                    new[] { nameof(ApiKey) }));
            }

            if (!string.IsNullOrEmpty(BaseUrl) && !BaseUrl.Contains("googleapis.com"))
            {
                results.Add(new ValidationResult(
                    "Base URL does not appear to be a valid Google API endpoint",
                    new[] { nameof(BaseUrl) }));
            }

            if (!string.IsNullOrEmpty(ModelName) && !ModelName.StartsWith("gemini"))
            {
                results.Add(new ValidationResult(
                    "Model name should start with 'gemini' for Gemini models",
                    new[] { nameof(ModelName) }));
            }

            return results;
        }
    }
}