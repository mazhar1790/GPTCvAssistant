using GPTCvAssistant.Models;

namespace GPTCvAssistant.Services.Interfaces
{
    /// <summary>
    /// Service for advanced career analytics and market intelligence
    /// </summary>
    public interface ICareerAnalyticsService
    {
        Task<CareerAnalyticsModel> GetCareerTrendsAsync(string role, string industry, string location);
        Task<List<SalaryRange>> GetSalaryTrendsAsync(string role, string experience, string location);
        Task<List<SkillDemand>> GetInDemandSkillsAsync(string industry);
        Task<string> GenerateMarketInsightsAsync(string role, string location);
    }

    /// <summary>
    /// Service for skills gap analysis and assessment
    /// </summary>
    public interface ISkillsAssessmentService
    {
        Task<string> AnalyzeSkillsGapAsync(SkillsGapRequest request);
        Task<List<string>> GetSkillRecommendationsAsync(string targetRole, List<string> currentSkills);
        Task<string> GenerateSkillDevelopmentPlanAsync(string targetRole, List<string> skillsToAcquire);
        Task<List<string>> GetCertificationRecommendationsAsync(string targetRole);
    }

    /// <summary>
    /// Service for personal branding and networking
    /// </summary>
    public interface IPersonalBrandingService
    {
        Task<PersonalBrandStrategy> CreatePersonalBrandAsync(PersonalBrandRequest request);
        Task<NetworkingStrategy> CreateNetworkingPlanAsync(NetworkingRequest request);
        Task<List<SocialMediaPost>> GenerateSocialMediaContentAsync(string role, string industry);
        Task<LinkedInOptimization> OptimizeLinkedInProfileAsync(string currentRole, string targetRole);
    }

    /// <summary>
    /// Service for interview preparation and simulation
    /// </summary>
    public interface IInterviewService
    {
        Task<string> StartInterviewSimulationAsync(InterviewSimulationRequest request);
        Task<string> ContinueInterviewAsync(string previousAnswer, string role);
        Task<InterviewFeedback> AnalyzeInterviewPerformanceAsync(string responses);
        Task<List<string>> GenerateInterviewQuestionsAsync(string role, string interviewType);
    }

    /// <summary>
    /// Service for application tracking and follow-up
    /// </summary>
    public interface IApplicationTrackingService
    {
        Task<string> GenerateFollowUpEmailAsync(FollowUpRequest request);
        Task<List<JobApplication>> GetApplicationsAsync();
        Task<JobApplication> SaveApplicationAsync(JobApplication application);
        Task<string> GenerateApplicationInsightsAsync();
    }

    /// <summary>
    /// Service for resume versioning and optimization
    /// </summary>
    public interface IResumeService
    {
        Task<ResumeVersion> CreateResumeVersionAsync(string name, string targetRole, string modifications);
        Task<List<ResumeVersion>> GetResumeVersionsAsync();
        Task<string> CompareResumeVersionsAsync(int version1Id, int version2Id);
        Task<string> OptimizeResumeForRoleAsync(string targetRole, string currentResume);
    }

    /// <summary>
    /// Service for career path recommendations
    /// </summary>
    public interface ICareerPathService
    {
        Task<CareerPathRecommendations> GetCareerPathsAsync(CareerPathRequest request);
        Task<string> GenerateCareerAdviceAsync(string currentRole, string targetRole, int experience);
        Task<List<string>> GetLearningResourcesAsync(string skill);
    }
}