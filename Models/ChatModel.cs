using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GPTCvAssistant.Models
{
    /// <summary>
    /// Model for chat interactions
    /// </summary>
    public class ChatModel
    {
        public string? UserQuestion { get; set; }
        public List<ChatExchange> History { get; set; } = new();
        public List<string> SuggestedPrompts { get; set; } = new();
        public RouteIntent Intent { get; set; } = RouteIntent.Default;
    }

    /// <summary>
    /// Represents a single chat exchange between user and assistant
    /// </summary>
    public class ChatExchange
    {
        [Required]
        public string UserQuestion { get; set; } = string.Empty;
        
        [Required]
        public string Answer { get; set; } = string.Empty;
    }

    /// <summary>
    /// Enumeration of different types of requests/intents
    /// </summary>
    public enum RouteIntent
    {
        Default,
        SearchHistory,
        ClearHistory,
        DownloadTranscript,
        SuggestPrompts,
        PinAnswer,
        AskAgain,

        // Career-focused intents
        CareerSummary,
        SkillsHighlight,

        // Job matching intents
        JobAnalysis,
        GenerateCoverLetter,
        ATSOptimization,
        ExtractKeywords
    }

    /// <summary>
    /// Request model for job analysis operations
    /// </summary>
    public class JobAnalysisRequest
    {
        [Required]
        public string JobDescription { get; set; } = string.Empty;
        
        public string CompanyName { get; set; } = string.Empty;
        public string TargetRole { get; set; } = string.Empty;
    }
}