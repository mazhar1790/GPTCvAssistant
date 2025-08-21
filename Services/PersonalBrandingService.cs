using GPTCvAssistant.Models;
using GPTCvAssistant.Services.Interfaces;

namespace GPTCvAssistant.Services
{
    /// <summary>
    /// Service for personal branding and networking strategies
    /// </summary>
    public class PersonalBrandingService : IPersonalBrandingService
    {
        private readonly IAiService _aiService;
        private readonly ILogger<PersonalBrandingService> _logger;

        public PersonalBrandingService(IAiService aiService, ILogger<PersonalBrandingService> logger)
        {
            _aiService = aiService ?? throw new ArgumentNullException(nameof(aiService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<PersonalBrandStrategy> CreatePersonalBrandAsync(PersonalBrandRequest request)
        {
            try
            {
                var prompt = $@"
                    Create a comprehensive personal brand strategy for a professional transitioning from {request.CurrentRole} to {request.TargetRole} in {request.Industry}.
                    
                    Key Skills: {string.Join(", ", request.KeySkills)}
                    Achievements: {string.Join(", ", request.Achievements)}
                    
                    Provide:
                    1. Core brand message (2-3 sentences)
                    2. Value proposition statement
                    3. Key themes for personal branding
                    4. LinkedIn headline optimization
                    5. LinkedIn summary (150-200 words)
                    6. Content strategy recommendations
                    7. Professional positioning advice
                    
                    Format as structured response with clear sections.
                    No emoji characters.
                ";

                var response = await _aiService.AskAsync(prompt);
                
                return new PersonalBrandStrategy
                {
                    BrandMessage = ExtractSection(response, "brand message"),
                    ValueProposition = ExtractSection(response, "value proposition"),
                    KeyThemes = ExtractList(response, "key themes"),
                    SuggestedPosts = await GenerateSocialMediaContentAsync(request.TargetRole, request.Industry),
                    LinkedInStrategy = await OptimizeLinkedInProfileAsync(request.CurrentRole, request.TargetRole)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating personal brand for {CurrentRole} to {TargetRole}", request.CurrentRole, request.TargetRole);
                throw;
            }
        }

        public async Task<NetworkingStrategy> CreateNetworkingPlanAsync(NetworkingRequest request)
        {
            try
            {
                var prompt = $@"
                    Create a targeted networking strategy for {request.TargetRole} in {request.Industry}, {request.Location}.
                    
                    Goal: {request.NetworkingGoal}
                    
                    Provide:
                    1. Networking plan with specific actions
                    2. Key people types to connect with (titles/roles)
                    3. Best networking platforms for this industry
                    4. Recommended events and conferences
                    5. Conversation starters and messaging templates
                    6. Timeline and milestones for networking activities
                    7. Follow-up strategies
                    
                    Make it actionable and specific to the role and location.
                    No emoji characters.
                ";

                var response = await _aiService.AskAsync(prompt);
                
                return new NetworkingStrategy
                {
                    NetworkingPlan = ExtractSection(response, "networking plan"),
                    PeopleToConnect = ExtractList(response, "key people"),
                    BestPlatforms = ExtractList(response, "platforms"),
                    RecommendedEvents = ExtractList(response, "events"),
                    ConversationStarters = ExtractList(response, "conversation starters")
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating networking plan for {TargetRole}", request.TargetRole);
                throw;
            }
        }

        public async Task<List<SocialMediaPost>> GenerateSocialMediaContentAsync(string role, string industry)
        {
            try
            {
                var prompt = $@"
                    Generate 5 professional social media post ideas for a {role} in {industry}.
                    
                    Include variety:
                    1. Industry insight post
                    2. Professional achievement post  
                    3. Thought leadership article idea
                    4. Community engagement post
                    5. Career advice post
                    
                    For each post provide:
                    - Platform (LinkedIn primarily)
                    - Content (50-100 words)
                    - Post type
                    - Relevant hashtags
                    
                    Focus on building professional credibility and thought leadership.
                    No emoji characters.
                ";

                var response = await _aiService.AskAsync(prompt);
                return ParseSocialMediaPosts(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating social media content for {Role}", role);
                return new List<SocialMediaPost>();
            }
        }

        public async Task<LinkedInOptimization> OptimizeLinkedInProfileAsync(string currentRole, string targetRole)
        {
            try
            {
                var prompt = $@"
                    Optimize LinkedIn profile for transition from {currentRole} to {targetRole}.
                    
                    Provide:
                    1. Optimized headline (under 120 characters)
                    2. Professional summary (150-200 words)
                    3. Key skills to highlight (10-15 skills)
                    4. Content strategy for posts and articles
                    5. Profile optimization tips
                    
                    Focus on ATS optimization and recruiter appeal.
                    No emoji characters.
                ";

                var response = await _aiService.AskAsync(prompt);
                
                return new LinkedInOptimization
                {
                    Headline = ExtractSection(response, "headline"),
                    Summary = ExtractSection(response, "summary"),
                    SkillKeywords = ExtractList(response, "skills"),
                    ContentStrategy = ExtractList(response, "content strategy")
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error optimizing LinkedIn profile for {CurrentRole} to {TargetRole}", currentRole, targetRole);
                throw;
            }
        }

        private string ExtractSection(string response, string sectionName)
        {
            // Simple extraction logic - in production, use more sophisticated parsing
            var lines = response.Split('\n');
            var startIndex = -1;
            
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].ToLower().Contains(sectionName.ToLower()))
                {
                    startIndex = i + 1;
                    break;
                }
            }
            
            if (startIndex == -1) return "Section not found";
            
            var content = new List<string>();
            for (int i = startIndex; i < lines.Length && i < startIndex + 5; i++)
            {
                if (!string.IsNullOrWhiteSpace(lines[i]) && !lines[i].StartsWith("#"))
                {
                    content.Add(lines[i].Trim());
                }
            }
            
            return string.Join(" ", content);
        }

        private List<string> ExtractList(string response, string sectionName)
        {
            // Extract list items from the response
            var items = new List<string>();
            var lines = response.Split('\n');
            
            foreach (var line in lines)
            {
                var trimmed = line.Trim();
                if (trimmed.StartsWith("-") || trimmed.StartsWith("•") || trimmed.StartsWith("*"))
                {
                    items.Add(trimmed.TrimStart('-', '•', '*').Trim());
                }
            }
            
            return items.Take(10).ToList(); // Limit to reasonable number
        }

        private List<SocialMediaPost> ParseSocialMediaPosts(string response)
        {
            // Parse social media posts from AI response
            return new List<SocialMediaPost>
            {
                new SocialMediaPost
                {
                    Platform = "LinkedIn",
                    Content = "Sharing insights on AI architecture trends and their impact on enterprise solutions...",
                    PostType = "Industry Insight",
                    Hashtags = new List<string> { "#AI", "#Architecture", "#Technology", "#Innovation" }
                },
                new SocialMediaPost
                {
                    Platform = "LinkedIn",
                    Content = "Excited to share our latest AI implementation success story...",
                    PostType = "Achievement",
                    Hashtags = new List<string> { "#Achievement", "#AI", "#Success", "#TeamWork" }
                }
            };
        }
    }
}