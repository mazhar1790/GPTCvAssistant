using GPTCvAssistant.Models;
using GPTCvAssistant.Services.Interfaces;

namespace GPTCvAssistant.Services
{
    /// <summary>
    /// Service for application tracking and follow-up management
    /// </summary>
    public class ApplicationTrackingService : IApplicationTrackingService
    {
        private readonly IAiService _aiService;
        private readonly ILogger<ApplicationTrackingService> _logger;
        private static readonly List<JobApplication> _applications = new(); // In-memory storage for demo

        public ApplicationTrackingService(IAiService aiService, ILogger<ApplicationTrackingService> logger)
        {
            _aiService = aiService ?? throw new ArgumentNullException(nameof(aiService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<string> GenerateFollowUpEmailAsync(FollowUpRequest request)
        {
            try
            {
                var prompt = $@"
                    Generate a professional follow-up email for a job application.
                    
                    Details:
                    - Company: {request.CompanyName}
                    - Position: {request.Position}
                    - Days since application: {request.DaysSinceApplication}
                    - Last interaction: {request.LastInteraction}
                    - Interaction type: {request.InteractionType}
                    - Contact name: {request.ContactName ?? "Hiring Manager"}
                    
                    Email should:
                    1. Be professional and courteous
                    2. Reference previous interaction appropriately
                    3. Reiterate interest and qualifications briefly
                    4. Include a clear call to action
                    5. Be appropriately timed (not too pushy)
                    6. Match the company culture and tone
                    
                    Format as complete email with subject line and body.
                    Return as HTML with proper structure.
                    No emoji characters.
                ";

                return await _aiService.AskAsync(prompt);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating follow-up email for {Company} {Position}", request.CompanyName, request.Position);
                throw;
            }
        }

        public async Task<List<JobApplication>> GetApplicationsAsync()
        {
            try
            {
                // In a real implementation, this would fetch from database
                await Task.Delay(10); // Simulate async operation
                return _applications.OrderByDescending(a => a.AppliedDate).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving applications");
                return new List<JobApplication>();
            }
        }

        public async Task<JobApplication> SaveApplicationAsync(JobApplication application)
        {
            try
            {
                await Task.Delay(10); // Simulate async operation
                
                if (application.Id == 0)
                {
                    application.Id = _applications.Count + 1;
                    application.AppliedDate = DateTime.Now;
                    _applications.Add(application);
                }
                else
                {
                    var existing = _applications.FirstOrDefault(a => a.Id == application.Id);
                    if (existing != null)
                    {
                        var index = _applications.IndexOf(existing);
                        _applications[index] = application;
                    }
                }

                return application;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving application for {Company} {Position}", application.CompanyName, application.Position);
                throw;
            }
        }

        public async Task<string> GenerateApplicationInsightsAsync()
        {
            try
            {
                var applications = await GetApplicationsAsync();
                
                if (!applications.Any())
                {
                    return "<h3>Application Insights</h3><p>No applications tracked yet. Start tracking your applications to get insights!</p>";
                }

                var prompt = $@"
                    Analyze the following job application data and provide insights:
                    
                    Total Applications: {applications.Count}
                    Status Breakdown:
                    - Applied: {applications.Count(a => a.Status == ApplicationStatus.Applied)}
                    - Under Review: {applications.Count(a => a.Status == ApplicationStatus.UnderReview)}
                    - Interview Scheduled: {applications.Count(a => a.Status == ApplicationStatus.InterviewScheduled)}
                    - Interviewed: {applications.Count(a => a.Status == ApplicationStatus.Interviewed)}
                    - Rejected: {applications.Count(a => a.Status == ApplicationStatus.Rejected)}
                    
                    Recent Applications:
                    {string.Join("\n", applications.Take(5).Select(a => $"- {a.CompanyName}: {a.Position} ({a.Status})"))}
                    
                    Provide:
                    1. Application performance analysis
                    2. Success rate and conversion metrics
                    3. Recommendations for improvement
                    4. Follow-up action items
                    5. Market timing insights
                    
                    Format as detailed HTML report with actionable insights.
                    No emoji characters.
                ";

                return await _aiService.AskAsync(prompt);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating application insights");
                throw;
            }
        }
    }

    /// <summary>
    /// Service for resume versioning and optimization
    /// </summary>
    public class ResumeService : IResumeService
    {
        private readonly IAiService _aiService;
        private readonly ILogger<ResumeService> _logger;
        private static readonly List<ResumeVersion> _resumeVersions = new(); // In-memory storage for demo

        public ResumeService(IAiService aiService, ILogger<ResumeService> logger)
        {
            _aiService = aiService ?? throw new ArgumentNullException(nameof(aiService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ResumeVersion> CreateResumeVersionAsync(string name, string targetRole, string modifications)
        {
            try
            {
                await Task.Delay(10); // Simulate async operation
                
                var version = new ResumeVersion
                {
                    Id = _resumeVersions.Count + 1,
                    Name = name,
                    TargetRole = targetRole,
                    Content = $"Resume optimized for {targetRole} - {modifications}",
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now,
                    Modifications = modifications.Split(',').Select(m => m.Trim()).ToList(),
                    IsActive = true
                };

                // Deactivate other versions
                foreach (var existing in _resumeVersions)
                {
                    existing.IsActive = false;
                }

                _resumeVersions.Add(version);
                return version;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating resume version {Name} for {TargetRole}", name, targetRole);
                throw;
            }
        }

        public async Task<List<ResumeVersion>> GetResumeVersionsAsync()
        {
            try
            {
                await Task.Delay(10); // Simulate async operation
                return _resumeVersions.OrderByDescending(v => v.CreatedDate).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving resume versions");
                return new List<ResumeVersion>();
            }
        }

        public async Task<string> CompareResumeVersionsAsync(int version1Id, int version2Id)
        {
            try
            {
                var version1 = _resumeVersions.FirstOrDefault(v => v.Id == version1Id);
                var version2 = _resumeVersions.FirstOrDefault(v => v.Id == version2Id);

                if (version1 == null || version2 == null)
                {
                    throw new ArgumentException("One or both resume versions not found");
                }

                var prompt = $@"
                    Compare two resume versions and highlight the differences:
                    
                    Version 1: {version1.Name} (Target: {version1.TargetRole})
                    Modifications: {string.Join(", ", version1.Modifications)}
                    Created: {version1.CreatedDate:yyyy-MM-dd}
                    
                    Version 2: {version2.Name} (Target: {version2.TargetRole})
                    Modifications: {string.Join(", ", version2.Modifications)}
                    Created: {version2.CreatedDate:yyyy-MM-dd}
                    
                    Provide:
                    1. Key differences between versions
                    2. Target role alignment comparison
                    3. Strengths and weaknesses of each version
                    4. Recommendations for which version to use when
                    5. Suggested improvements for both versions
                    
                    Format as structured HTML comparison report.
                    No emoji characters.
                ";

                return await _aiService.AskAsync(prompt);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error comparing resume versions {Version1Id} and {Version2Id}", version1Id, version2Id);
                throw;
            }
        }

        public async Task<string> OptimizeResumeForRoleAsync(string targetRole, string currentResume)
        {
            try
            {
                var prompt = $@"
                    Optimize the following resume for {targetRole} position:
                    
                    Current Resume Content: {currentResume}
                    
                    Provide specific optimization recommendations:
                    1. Keywords to add for ATS optimization
                    2. Skills to emphasize or add
                    3. Experience descriptions to modify
                    4. Achievements to highlight
                    5. Format and structure improvements
                    6. Industry-specific customizations
                    7. Quantifiable metrics to include
                    
                    Focus on making the resume highly relevant for {targetRole}.
                    Format as detailed HTML optimization guide.
                    No emoji characters.
                ";

                return await _aiService.AskAsync(prompt);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error optimizing resume for {TargetRole}", targetRole);
                throw;
            }
        }
    }

    /// <summary>
    /// Service for career path recommendations and planning
    /// </summary>
    public class CareerPathService : ICareerPathService
    {
        private readonly IAiService _aiService;
        private readonly ILogger<CareerPathService> _logger;

        public CareerPathService(IAiService aiService, ILogger<CareerPathService> logger)
        {
            _aiService = aiService ?? throw new ArgumentNullException(nameof(aiService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<CareerPathRecommendations> GetCareerPathsAsync(CareerPathRequest request)
        {
            try
            {
                var skillsList = string.Join(", ", request.Skills);
                
                var prompt = $@"
                    Analyze career path options for a professional with the following profile:
                    
                    Current Role: {request.CurrentRole}
                    Skills: {skillsList}
                    Interests: {request.Interests}
                    Years of Experience: {request.YearsExperience}
                    Preferred Industry: {request.PreferredIndustry}
                    Career Goals: {request.CareerGoals}
                    
                    Provide 4-5 distinct career path recommendations including:
                    1. Path title and description
                    2. Required skills to develop
                    3. Timeline for transition (1-3 years, 3-5 years, 5+ years)
                    4. Salary progression expectations
                    5. Market demand rating (1-10)
                    6. Next steps to pursue this path
                    7. Recommended certifications
                    8. Learning resources and development plan
                    
                    Also provide general career advice and skill acquisition recommendations.
                    Format as structured response with clear sections.
                    No emoji characters.
                ";

                var response = await _aiService.AskAsync(prompt);
                
                return new CareerPathRecommendations
                {
                    RecommendedPaths = ParseCareerPaths(response),
                    SkillsToAcquire = ExtractList(response, "skills to acquire"),
                    RecommendedCertifications = ExtractList(response, "certifications"),
                    CareerAdvice = ExtractSection(response, "career advice")
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting career paths for {CurrentRole}", request.CurrentRole);
                throw;
            }
        }

        public async Task<string> GenerateCareerAdviceAsync(string currentRole, string targetRole, int experience)
        {
            try
            {
                var prompt = $@"
                    Provide comprehensive career advice for transitioning from {currentRole} to {targetRole}.
                    Current experience level: {experience} years
                    
                    Include:
                    1. Realistic timeline for transition
                    2. Key skills and competencies to develop
                    3. Industry trends and market outlook
                    4. Networking and relationship building strategies
                    5. Common challenges and how to overcome them
                    6. Salary negotiation considerations
                    7. Work-life balance and personal development
                    8. Actionable next steps (30, 60, 90 days)
                    
                    Make the advice specific, actionable, and realistic.
                    Format as structured HTML guide.
                    No emoji characters.
                ";

                return await _aiService.AskAsync(prompt);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating career advice for {CurrentRole} to {TargetRole}", currentRole, targetRole);
                throw;
            }
        }

        public async Task<List<string>> GetLearningResourcesAsync(string skill)
        {
            try
            {
                var prompt = $@"
                    Recommend the best learning resources for acquiring {skill}.
                    
                    Include:
                    1. Online courses (Coursera, Udemy, Pluralsight, etc.)
                    2. Books and publications
                    3. Certification programs
                    4. Hands-on projects and practice
                    5. Communities and forums
                    6. YouTube channels and podcasts
                    7. Free vs paid resources
                    8. Beginner to advanced learning path
                    
                    Provide specific resource names and brief descriptions.
                    Order by effectiveness and popularity.
                ";

                var response = await _aiService.AskAsync(prompt);
                return ParseResourcesList(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting learning resources for {Skill}", skill);
                return new List<string>();
            }
        }

        private List<CareerPath> ParseCareerPaths(string response)
        {
            // In a real implementation, parse the AI response more sophisticatedly
            return new List<CareerPath>
            {
                new CareerPath
                {
                    Title = "Senior AI Architect",
                    Description = "Lead enterprise AI solution design and implementation",
                    RequiredSkills = new List<string> { "Advanced AI/ML", "System Architecture", "Team Leadership" },
                    Timeline = "2-3 years",
                    SalaryProgression = new SalaryRange { MinSalary = 200000, MaxSalary = 350000, MedianSalary = 275000 },
                    MarketDemand = 9,
                    NextSteps = new List<string> { "Gain leadership experience", "Advanced AI certifications", "Build portfolio" }
                },
                new CareerPath
                {
                    Title = "Head of AI/CTO",
                    Description = "Executive leadership in AI strategy and technology direction",
                    RequiredSkills = new List<string> { "Strategic Planning", "Business Acumen", "Technology Leadership" },
                    Timeline = "5-7 years",
                    SalaryProgression = new SalaryRange { MinSalary = 300000, MaxSalary = 500000, MedianSalary = 400000 },
                    MarketDemand = 8,
                    NextSteps = new List<string> { "Build P&L experience", "MBA or executive education", "Board advisor roles" }
                }
            };
        }

        private List<string> ExtractList(string response, string sectionName)
        {
            // Extract list items from response
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
            
            return items.Take(10).ToList();
        }

        private string ExtractSection(string response, string sectionName)
        {
            // Extract specific section from response
            var lines = response.Split('\n');
            var content = new List<string>();
            bool inSection = false;
            
            foreach (var line in lines)
            {
                if (line.ToLower().Contains(sectionName.ToLower()))
                {
                    inSection = true;
                    continue;
                }
                
                if (inSection)
                {
                    if (string.IsNullOrWhiteSpace(line) && content.Any())
                    {
                        break;
                    }
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        content.Add(line.Trim());
                    }
                }
            }
            
            return string.Join(" ", content);
        }

        private List<string> ParseResourcesList(string response)
        {
            var resources = new List<string>();
            var lines = response.Split('\n');
            
            foreach (var line in lines)
            {
                var trimmed = line.Trim();
                if (trimmed.StartsWith("-") || trimmed.StartsWith("•") || trimmed.StartsWith("*") || char.IsDigit(trimmed.FirstOrDefault()))
                {
                    var cleanResource = trimmed.TrimStart('-', '•', '*', '1', '2', '3', '4', '5', '6', '7', '8', '9', '0', '.', ' ').Trim();
                    if (!string.IsNullOrEmpty(cleanResource))
                    {
                        resources.Add(cleanResource);
                    }
                }
            }
            
            return resources.Take(15).ToList();
        }
    }
}