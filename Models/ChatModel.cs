using System.Collections.Generic;

namespace GPTCvAssistant.Models
{
    public class ChatModel
    {
        public string UserQuestion { get; set; }
        public List<ChatExchange> History { get; set; } = new();
        public List<string> SuggestedPrompts { get; set; } = new();
        public RouteIntent Intent { get; set; } = RouteIntent.Default;
    }

    public class ChatExchange
    {
        public string UserQuestion { get; set; }
        public string Answer { get; set; }
    }

    public enum RouteIntent
    {
        Default,
        SearchHistory,
        ClearHistory,
        DownloadTranscript,
        SuggestPrompts,
        PinAnswer,
        AskAgain,

        // Existing intents
        CareerSummary,
        SkillsHighlight,

        // New Job Matching intents
        JobAnalysis,
        GenerateCoverLetter,
        ATSOptimization,
        ExtractKeywords
    }

    public class JobAnalysisRequest
    {
        public string JobDescription { get; set; }
        public string CompanyName { get; set; } = "";
        public string TargetRole { get; set; } = "";
    }
}