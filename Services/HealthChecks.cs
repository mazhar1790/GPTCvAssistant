using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using GPTCvAssistant.Services.Interfaces;

namespace GPTCvAssistant.Services
{
    /// <summary>
    /// Health check for Gemini AI service
    /// </summary>
    public class GeminiHealthCheck : IHealthCheck
    {
        private readonly IAiService _aiService;
        private readonly ILogger<GeminiHealthCheck> _logger;

        public GeminiHealthCheck(IAiService aiService, ILogger<GeminiHealthCheck> logger)
        {
            _aiService = aiService;
            _logger = logger;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                // Simple health check with a minimal request
                var response = await _aiService.AskAsync("Hello", cancellationToken);
                
                if (!string.IsNullOrEmpty(response))
                {
                    return HealthCheckResult.Healthy("Gemini service is responding");
                }
                
                return HealthCheckResult.Degraded("Gemini service returned empty response");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Gemini health check failed");
                return HealthCheckResult.Unhealthy("Gemini service is not available", ex);
            }
        }
    }

    /// <summary>
    /// Health check for OpenAI service
    /// </summary>
    public class OpenAiHealthCheck : IHealthCheck
    {
        private readonly OpenAiService _openAiService;
        private readonly ILogger<OpenAiHealthCheck> _logger;

        public OpenAiHealthCheck(OpenAiService openAiService, ILogger<OpenAiHealthCheck> logger)
        {
            _openAiService = openAiService;
            _logger = logger;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _openAiService.AskAsync("Hello", cancellationToken);
                
                if (!string.IsNullOrEmpty(response))
                {
                    return HealthCheckResult.Healthy("OpenAI service is responding");
                }
                
                return HealthCheckResult.Degraded("OpenAI service returned empty response");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "OpenAI health check failed");
                return HealthCheckResult.Unhealthy("OpenAI service is not available", ex);
            }
        }
    }
}