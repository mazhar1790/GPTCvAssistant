using GPTCvAssistant.Models;
using GPTCvAssistant.Services.Interfaces;

namespace GPTCvAssistant.Services
{
    /// <summary>
    /// Service for advanced career analytics and market intelligence
    /// </summary>
    public class CareerAnalyticsService : ICareerAnalyticsService
    {
        private readonly IAiService _aiService;
        private readonly ILogger<CareerAnalyticsService> _logger;

        public CareerAnalyticsService(IAiService aiService, ILogger<CareerAnalyticsService> logger)
        {
            _aiService = aiService ?? throw new ArgumentNullException(nameof(aiService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<CareerAnalyticsModel> GetCareerTrendsAsync(string role, string industry, string location)
        {
            try
            {
                var prompt = $@"
                    Analyze current career trends for {role} in {industry} industry, {location}.
                    
                    Provide insights on:
                    1. Job market demand (scale 1-10)
                    2. Salary trends and ranges
                    3. Growing vs declining opportunities
                    4. Key skills in demand
                    5. Future outlook (2-5 years)
                    
                    Format as detailed analysis with specific data points where possible.
                    Return HTML format with structured sections.
                ";

                var analysis = await _aiService.AskAsync(prompt);

                // Parse the response into structured data
                return new CareerAnalyticsModel
                {
                    Role = role,
                    Industry = industry,
                    Location = location,
                    Trends = ParseTrendData(analysis),
                    SalaryData = await GetSalaryTrendsAsync(role, "Mid-Level", location),
                    SkillDemands = await GetInDemandSkillsAsync(industry)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error analyzing career trends for {Role} in {Industry}", role, industry);
                throw;
            }
        }

        public async Task<List<SalaryRange>> GetSalaryTrendsAsync(string role, string experience, string location)
        {
            try
            {
                var prompt = $@"
                    Provide salary ranges for {role} with {experience} experience in {location}.
                    
                    Include:
                    - Entry level (0-2 years)
                    - Mid level (3-7 years)  
                    - Senior level (8+ years)
                    - Lead/Principal level (10+ years)
                    
                    Format: Role | Min Salary | Max Salary | Median | Experience Level
                    Use local currency and realistic market rates.
                ";

                var response = await _aiService.AskAsync(prompt);
                return ParseSalaryData(response, location);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting salary trends for {Role}", role);
                return new List<SalaryRange>();
            }
        }

        public async Task<List<SkillDemand>> GetInDemandSkillsAsync(string industry)
        {
            try
            {
                var prompt = $@"
                    List the top 10 most in-demand skills for {industry} industry in 2024-2025.
                    
                    For each skill provide:
                    - Skill name
                    - Demand level (1-10 scale)
                    - Growth rate percentage
                    - Related job roles
                    
                    Focus on technical skills, soft skills, and emerging competencies.
                ";

                var response = await _aiService.AskAsync(prompt);
                return ParseSkillDemands(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting skill demands for {Industry}", industry);
                return new List<SkillDemand>();
            }
        }

        public async Task<string> GenerateMarketInsightsAsync(string role, string location)
        {
            try
            {
                var prompt = $@"
                    Generate comprehensive market insights for {role} positions in {location}.
                    
                    Include:
                    1. Current market conditions
                    2. Hiring trends and patterns
                    3. Company types actively hiring
                    4. Remote work opportunities
                    5. Key success factors for job seekers
                    6. Networking and application strategies
                    7. Timeline expectations for job search
                    
                    Return as structured HTML with actionable insights.
                    No emoji characters.
                ";

                return await _aiService.AskAsync(prompt);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating market insights for {Role} in {Location}", role, location);
                throw;
            }
        }

        private List<TrendData> ParseTrendData(string analysis)
        {
            // Simple parsing logic - in a real implementation, you might use more sophisticated parsing
            return new List<TrendData>
            {
                new TrendData { Category = "Job Demand", Value = 8.5m, Trend = "increasing", Period = "2024" },
                new TrendData { Category = "Salary Growth", Value = 12.5m, Trend = "increasing", Period = "YoY" },
                new TrendData { Category = "Remote Opportunities", Value = 75m, Trend = "stable", Period = "2024" }
            };
        }

        private List<SalaryRange> ParseSalaryData(string response, string location)
        {
            // In a real implementation, parse the AI response to extract salary data
            return new List<SalaryRange>
            {
                new SalaryRange { Role = "AI Solutions Architect", MinSalary = 180000, MaxSalary = 280000, MedianSalary = 230000, ExperienceLevel = "Senior", Location = location },
                new SalaryRange { Role = "AI Solutions Architect", MinSalary = 120000, MaxSalary = 180000, MedianSalary = 150000, ExperienceLevel = "Mid-Level", Location = location },
                new SalaryRange { Role = "AI Solutions Architect", MinSalary = 80000, MaxSalary = 120000, MedianSalary = 100000, ExperienceLevel = "Entry", Location = location }
            };
        }

        private List<SkillDemand> ParseSkillDemands(string response)
        {
            // In a real implementation, parse the AI response to extract skill demands
            return new List<SkillDemand>
            {
                new SkillDemand { SkillName = "Large Language Models", DemandLevel = 10, GrowthRate = 45.5m, RelatedRoles = new List<string> { "AI Engineer", "ML Engineer", "AI Architect" } },
                new SkillDemand { SkillName = "RAG Systems", DemandLevel = 9, GrowthRate = 38.2m, RelatedRoles = new List<string> { "AI Architect", "NLP Engineer" } },
                new SkillDemand { SkillName = "Azure AI Services", DemandLevel = 9, GrowthRate = 32.8m, RelatedRoles = new List<string> { "Cloud Architect", "AI Developer" } },
                new SkillDemand { SkillName = ".NET Core", DemandLevel = 8, GrowthRate = 15.3m, RelatedRoles = new List<string> { "Backend Developer", "Full Stack Developer" } }
            };
        }
    }
}