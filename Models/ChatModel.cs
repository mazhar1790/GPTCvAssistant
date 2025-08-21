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
        ExtractKeywords,

        // Advanced features
        SkillsGapAnalysis,
        CareerPathRecommendation,
        InterviewSimulation,
        VideoInterviewAnalysis,
        PersonalBrandBuilder,
        NetworkingStrategy,
        MarketIntelligence,
        ApplicationTracking,
        FollowUpGeneration,
        ResumeVersioning
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

    /// <summary>
    /// Request model for skills gap analysis
    /// </summary>
    public class SkillsGapRequest
    {
        [Required]
        public string TargetRole { get; set; } = string.Empty;
        
        public List<string> CurrentSkills { get; set; } = new();
        public string Industry { get; set; } = string.Empty;
        public string ExperienceLevel { get; set; } = string.Empty;
    }

    /// <summary>
    /// Model for career analytics and trends
    /// </summary>
    public class CareerAnalyticsModel
    {
        public string Role { get; set; } = string.Empty;
        public string Industry { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public List<TrendData> Trends { get; set; } = new();
        public List<SalaryRange> SalaryData { get; set; } = new();
        public List<SkillDemand> SkillDemands { get; set; } = new();
    }

    public class TrendData
    {
        public string Category { get; set; } = string.Empty;
        public decimal Value { get; set; }
        public string Trend { get; set; } = string.Empty; // "increasing", "stable", "decreasing"
        public string Period { get; set; } = string.Empty;
    }

    public class SalaryRange
    {
        public string Role { get; set; } = string.Empty;
        public decimal MinSalary { get; set; }
        public decimal MaxSalary { get; set; }
        public decimal MedianSalary { get; set; }
        public string ExperienceLevel { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
    }

    public class SkillDemand
    {
        public string SkillName { get; set; } = string.Empty;
        public int DemandLevel { get; set; } // 1-10 scale
        public decimal GrowthRate { get; set; }
        public List<string> RelatedRoles { get; set; } = new();
    }

    /// <summary>
    /// Model for personal branding recommendations
    /// </summary>
    public class PersonalBrandRequest
    {
        public string CurrentRole { get; set; } = string.Empty;
        public string TargetRole { get; set; } = string.Empty;
        public string Industry { get; set; } = string.Empty;
        public List<string> KeySkills { get; set; } = new();
        public List<string> Achievements { get; set; } = new();
        public string PersonalityType { get; set; } = string.Empty;
    }

    public class PersonalBrandStrategy
    {
        public string BrandMessage { get; set; } = string.Empty;
        public string ValueProposition { get; set; } = string.Empty;
        public List<string> KeyThemes { get; set; } = new();
        public List<SocialMediaPost> SuggestedPosts { get; set; } = new();
        public LinkedInOptimization LinkedInStrategy { get; set; } = new();
    }

    public class SocialMediaPost
    {
        public string Platform { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string PostType { get; set; } = string.Empty; // "Article", "Update", "Question", "Achievement"
        public List<string> Hashtags { get; set; } = new();
    }

    public class LinkedInOptimization
    {
        public string Headline { get; set; } = string.Empty;
        public string Summary { get; set; } = string.Empty;
        public List<string> SkillKeywords { get; set; } = new();
        public List<string> ContentStrategy { get; set; } = new();
    }

    /// <summary>
    /// Model for networking strategy
    /// </summary>
    public class NetworkingRequest
    {
        public string TargetRole { get; set; } = string.Empty;
        public string Industry { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string NetworkingGoal { get; set; } = string.Empty;
    }

    public class NetworkingStrategy
    {
        public List<string> PeopleToConnect { get; set; } = new();
        public List<string> RecommendedEvents { get; set; } = new();
        public List<string> BestPlatforms { get; set; } = new();
        public List<string> ConversationStarters { get; set; } = new();
        public string NetworkingPlan { get; set; } = string.Empty;
    }

    /// <summary>
    /// Model for interview simulation
    /// </summary>
    public class InterviewSimulationRequest
    {
        public string Role { get; set; } = string.Empty;
        public string Company { get; set; } = string.Empty;
        public string InterviewType { get; set; } = string.Empty; // "Technical", "Behavioral", "Case Study"
        public string Difficulty { get; set; } = string.Empty; // "Beginner", "Intermediate", "Advanced"
    }

    public class InterviewFeedback
    {
        public string OverallRating { get; set; } = string.Empty;
        public List<string> Strengths { get; set; } = new();
        public List<string> AreasForImprovement { get; set; } = new();
        public List<string> Suggestions { get; set; } = new();
        public string DetailedFeedback { get; set; } = string.Empty;
    }

    /// <summary>
    /// Model for application tracking
    /// </summary>
    public class JobApplication
    {
        public int Id { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public DateTime AppliedDate { get; set; }
        public ApplicationStatus Status { get; set; }
        public string Notes { get; set; } = string.Empty;
        public List<InteractionLog> Interactions { get; set; } = new();
        public DateTime? FollowUpDate { get; set; }
        public string JobUrl { get; set; } = string.Empty;
        public string ContactPerson { get; set; } = string.Empty;
        public decimal? SalaryRange { get; set; }
    }

    public enum ApplicationStatus
    {
        Applied,
        UnderReview,
        InterviewScheduled,
        Interviewed,
        SecondRound,
        FinalRound,
        Offered,
        Negotiating,
        Rejected,
        Withdrawn,
        Accepted
    }

    public class InteractionLog
    {
        public DateTime Date { get; set; }
        public string Type { get; set; } = string.Empty; // "Email", "Call", "Interview", "Follow-up"
        public string Description { get; set; } = string.Empty;
        public string Contact { get; set; } = string.Empty;
    }

    /// <summary>
    /// Model for follow-up generation
    /// </summary>
    public class FollowUpRequest
    {
        public string CompanyName { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public int DaysSinceApplication { get; set; }
        public string LastInteraction { get; set; } = string.Empty;
        public string InteractionType { get; set; } = string.Empty;
        public string ContactName { get; set; } = string.Empty;
    }

    /// <summary>
    /// Model for resume versioning
    /// </summary>
    public class ResumeVersion
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string TargetRole { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public List<string> Modifications { get; set; } = new();
        public bool IsActive { get; set; }
    }

    /// <summary>
    /// Model for career path recommendations
    /// </summary>
    public class CareerPathRequest
    {
        public string CurrentRole { get; set; } = string.Empty;
        public List<string> Skills { get; set; } = new();
        public string Interests { get; set; } = string.Empty;
        public int YearsExperience { get; set; }
        public string PreferredIndustry { get; set; } = string.Empty;
        public string CareerGoals { get; set; } = string.Empty;
    }

    public class CareerPathRecommendations
    {
        public List<CareerPath> RecommendedPaths { get; set; } = new();
        public List<string> SkillsToAcquire { get; set; } = new();
        public List<string> RecommendedCertifications { get; set; } = new();
        public string CareerAdvice { get; set; } = string.Empty;
    }

    public class CareerPath
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<string> RequiredSkills { get; set; } = new();
        public string Timeline { get; set; } = string.Empty;
        public SalaryRange SalaryProgression { get; set; } = new();
        public int MarketDemand { get; set; } // 1-10 scale
        public List<string> NextSteps { get; set; } = new();
    }
}