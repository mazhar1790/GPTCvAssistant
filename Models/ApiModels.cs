using System.ComponentModel.DataAnnotations;

namespace GPTCvAssistant.Models
{
    /// <summary>
    /// Response model for API operations
    /// </summary>
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public string? ErrorCode { get; set; }
    }

    /// <summary>
    /// Model for error responses
    /// </summary>
    public class ErrorResponse
    {
        public string Message { get; set; } = string.Empty;
        public string? StackTrace { get; set; }
        public string? Source { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Model for analytics and usage tracking
    /// </summary>
    public class UsageMetrics
    {
        public int TotalQuestions { get; set; }
        public int JobAnalysisCount { get; set; }
        public int CoverLettersGenerated { get; set; }
        public int ATSOptimizationsCount { get; set; }
        public DateTime FirstVisit { get; set; }
        public DateTime LastVisit { get; set; }
        public List<string> PopularQuestions { get; set; } = new();
    }
}