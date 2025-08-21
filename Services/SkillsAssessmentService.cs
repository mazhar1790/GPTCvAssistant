using GPTCvAssistant.Models;
using GPTCvAssistant.Services.Interfaces;

namespace GPTCvAssistant.Services
{
    /// <summary>
    /// Service for skills gap analysis and assessment
    /// </summary>
    public class SkillsAssessmentService : ISkillsAssessmentService
    {
        private readonly IAiService _aiService;
        private readonly ILogger<SkillsAssessmentService> _logger;

        public SkillsAssessmentService(IAiService aiService, ILogger<SkillsAssessmentService> logger)
        {
            _aiService = aiService ?? throw new ArgumentNullException(nameof(aiService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<string> AnalyzeSkillsGapAsync(SkillsGapRequest request)
        {
            try
            {
                var currentSkillsList = string.Join(", ", request.CurrentSkills);
                
                var prompt = $@"
                    Conduct a comprehensive skills gap analysis for transitioning to {request.TargetRole} in {request.Industry}.
                    
                    Current Skills: {currentSkillsList}
                    Experience Level: {request.ExperienceLevel}
                    
                    Analysis should include:
                    1. Skills Match Assessment (percentage match)
                    2. Core skills already possessed
                    3. Critical skills missing for the target role
                    4. Nice-to-have skills that would strengthen candidacy
                    5. Learning priority ranking (High/Medium/Low)
                    6. Estimated time to acquire missing skills
                    7. Recommended learning paths and resources
                    8. Industry-specific considerations
                    
                    Format as detailed HTML report with clear sections and actionable insights.
                    Use tables and lists for better readability.
                    No emoji characters.
                ";

                return await _aiService.AskAsync(prompt);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error analyzing skills gap for {TargetRole}", request.TargetRole);
                throw;
            }
        }

        public async Task<List<string>> GetSkillRecommendationsAsync(string targetRole, List<string> currentSkills)
        {
            try
            {
                var currentSkillsList = string.Join(", ", currentSkills);
                
                var prompt = $@"
                    Based on current skills: {currentSkillsList}
                    Recommend the top 8-10 additional skills needed for {targetRole}.
                    
                    Prioritize:
                    1. High-demand technical skills
                    2. Emerging technologies relevant to the role
                    3. Soft skills and leadership capabilities
                    4. Industry-specific knowledge
                    
                    Return as a simple list, one skill per line.
                ";

                var response = await _aiService.AskAsync(prompt);
                return ParseSkillsList(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting skill recommendations for {TargetRole}", targetRole);
                return new List<string>();
            }
        }

        public async Task<string> GenerateSkillDevelopmentPlanAsync(string targetRole, List<string> skillsToAcquire)
        {
            try
            {
                var skillsList = string.Join(", ", skillsToAcquire);
                
                var prompt = $@"
                    Create a comprehensive skill development plan for acquiring these skills: {skillsList}
                    Target Role: {targetRole}
                    
                    Include:
                    1. Learning roadmap with timeline (3, 6, 12 months)
                    2. Recommended learning resources (courses, books, platforms)
                    3. Hands-on project suggestions
                    4. Certification opportunities
                    5. Practice and assessment strategies
                    6. Ways to demonstrate skills to employers
                    7. Budget considerations for paid resources
                    
                    Format as structured HTML with clear phases and milestones.
                    Make it actionable and specific.
                    No emoji characters.
                ";

                return await _aiService.AskAsync(prompt);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating skill development plan for {TargetRole}", targetRole);
                throw;
            }
        }

        public async Task<List<string>> GetCertificationRecommendationsAsync(string targetRole)
        {
            try
            {
                var prompt = $@"
                    Recommend the most valuable certifications for {targetRole}.
                    
                    Consider:
                    1. Industry recognition and credibility
                    2. Career advancement impact
                    3. Current market demand
                    4. Cost-benefit ratio
                    5. Prerequisites and difficulty
                    
                    List top 6-8 certifications with brief explanation of value.
                    Include both technical and business certifications where relevant.
                ";

                var response = await _aiService.AskAsync(prompt);
                return ParseCertificationsList(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting certification recommendations for {TargetRole}", targetRole);
                return new List<string>();
            }
        }

        private List<string> ParseSkillsList(string response)
        {
            // Parse the AI response to extract skills list
            var lines = response.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            var skills = new List<string>();

            foreach (var line in lines)
            {
                var cleanLine = line.Trim()
                    .TrimStart('-', '*', '•', '1', '2', '3', '4', '5', '6', '7', '8', '9', '0', '.', ' ')
                    .Trim();
                
                if (!string.IsNullOrEmpty(cleanLine) && cleanLine.Length > 2)
                {
                    skills.Add(cleanLine);
                }
            }

            return skills;
        }

        private List<string> ParseCertificationsList(string response)
        {
            // Similar parsing logic for certifications
            return ParseSkillsList(response);
        }
    }
}