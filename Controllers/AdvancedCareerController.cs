using GPTCvAssistant.Models;
using GPTCvAssistant.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GPTCvAssistant.Controllers
{
    /// <summary>
    /// Controller for advanced career features
    /// </summary>
    public class AdvancedCareerController : Controller
    {
        private readonly ICareerAnalyticsService _careerAnalyticsService;
        private readonly ISkillsAssessmentService _skillsAssessmentService;
        private readonly IPersonalBrandingService _personalBrandingService;
        private readonly IInterviewService _interviewService;
        private readonly IApplicationTrackingService _applicationTrackingService;
        private readonly IResumeService _resumeService;
        private readonly ICareerPathService _careerPathService;
        private readonly ILogger<AdvancedCareerController> _logger;

        public AdvancedCareerController(
            ICareerAnalyticsService careerAnalyticsService,
            ISkillsAssessmentService skillsAssessmentService,
            IPersonalBrandingService personalBrandingService,
            IInterviewService interviewService,
            IApplicationTrackingService applicationTrackingService,
            IResumeService resumeService,
            ICareerPathService careerPathService,
            ILogger<AdvancedCareerController> logger)
        {
            _careerAnalyticsService = careerAnalyticsService ?? throw new ArgumentNullException(nameof(careerAnalyticsService));
            _skillsAssessmentService = skillsAssessmentService ?? throw new ArgumentNullException(nameof(skillsAssessmentService));
            _personalBrandingService = personalBrandingService ?? throw new ArgumentNullException(nameof(personalBrandingService));
            _interviewService = interviewService ?? throw new ArgumentNullException(nameof(interviewService));
            _applicationTrackingService = applicationTrackingService ?? throw new ArgumentNullException(nameof(applicationTrackingService));
            _resumeService = resumeService ?? throw new ArgumentNullException(nameof(resumeService));
            _careerPathService = careerPathService ?? throw new ArgumentNullException(nameof(careerPathService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost]
        public async Task<IActionResult> AnalyzeSkillsGap([FromBody] SkillsGapRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request?.TargetRole))
                {
                    return Json(new { success = false, message = "Target role is required" });
                }

                var analysis = await _skillsAssessmentService.AnalyzeSkillsGapAsync(request);
                
                return Json(new { 
                    success = true, 
                    html = analysis,
                    intent = "SkillsGapAnalysis"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in AnalyzeSkillsGap");
                return Json(new { success = false, message = $"Error analyzing skills gap: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetCareerTrends([FromBody] dynamic request)
        {
            try
            {
                string role = request.role;
                string industry = request.industry;
                string location = request.location;

                var trends = await _careerAnalyticsService.GetCareerTrendsAsync(role, industry, location);
                var insights = await _careerAnalyticsService.GenerateMarketInsightsAsync(role, location);
                
                return Json(new { 
                    success = true, 
                    trends = trends,
                    html = insights,
                    intent = "MarketIntelligence"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetCareerTrends");
                return Json(new { success = false, message = $"Error getting career trends: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreatePersonalBrand([FromBody] PersonalBrandRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request?.CurrentRole))
                {
                    return Json(new { success = false, message = "Current role is required" });
                }

                var brandStrategy = await _personalBrandingService.CreatePersonalBrandAsync(request);
                
                return Json(new { 
                    success = true, 
                    brand = brandStrategy,
                    intent = "PersonalBrandBuilder"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CreatePersonalBrand");
                return Json(new { success = false, message = $"Error creating personal brand: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> GenerateNetworkingStrategy([FromBody] NetworkingRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request?.TargetRole))
                {
                    return Json(new { success = false, message = "Target role is required" });
                }

                var strategy = await _personalBrandingService.CreateNetworkingPlanAsync(request);
                
                return Json(new { 
                    success = true, 
                    strategy = strategy,
                    intent = "NetworkingStrategy"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GenerateNetworkingStrategy");
                return Json(new { success = false, message = $"Error generating networking strategy: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> StartInterviewSimulation([FromBody] InterviewSimulationRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request?.Role))
                {
                    return Json(new { success = false, message = "Role is required" });
                }

                var interviewContent = await _interviewService.StartInterviewSimulationAsync(request);
                
                return Json(new { 
                    success = true, 
                    html = interviewContent,
                    intent = "InterviewSimulation"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in StartInterviewSimulation");
                return Json(new { success = false, message = $"Error starting interview simulation: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> GenerateFollowUp([FromBody] FollowUpRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request?.CompanyName) || string.IsNullOrWhiteSpace(request?.Position))
                {
                    return Json(new { success = false, message = "Company name and position are required" });
                }

                var followUpEmail = await _applicationTrackingService.GenerateFollowUpEmailAsync(request);
                
                return Json(new { 
                    success = true, 
                    html = followUpEmail,
                    intent = "FollowUpGeneration"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GenerateFollowUp");
                return Json(new { success = false, message = $"Error generating follow-up: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetCareerPaths([FromBody] CareerPathRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request?.CurrentRole))
                {
                    return Json(new { success = false, message = "Current role is required" });
                }

                var recommendations = await _careerPathService.GetCareerPathsAsync(request);
                
                return Json(new { 
                    success = true, 
                    recommendations = recommendations,
                    intent = "CareerPathRecommendation"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetCareerPaths");
                return Json(new { success = false, message = $"Error getting career paths: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateResumeVersion([FromBody] dynamic request)
        {
            try
            {
                string name = request.name;
                string targetRole = request.targetRole;
                string modifications = request.modifications;

                var version = await _resumeService.CreateResumeVersionAsync(name, targetRole, modifications);
                
                return Json(new { 
                    success = true, 
                    version = version,
                    intent = "ResumeVersioning"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CreateResumeVersion");
                return Json(new { success = false, message = $"Error creating resume version: {ex.Message}" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetResumeVersions()
        {
            try
            {
                var versions = await _resumeService.GetResumeVersionsAsync();
                return Json(new { success = true, versions = versions });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetResumeVersions");
                return Json(new { success = false, message = $"Error getting resume versions: {ex.Message}" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> CompareResumeVersions([FromQuery] int v1, [FromQuery] int v2)
        {
            try
            {
                var comparison = await _resumeService.CompareResumeVersionsAsync(v1, v2);
                return Json(new { success = true, comparison = comparison });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CompareResumeVersions");
                return Json(new { success = false, message = $"Error comparing resume versions: {ex.Message}" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetApplications()
        {
            try
            {
                var applications = await _applicationTrackingService.GetApplicationsAsync();
                return Json(new { success = true, applications = applications });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetApplications");
                return Json(new { success = false, message = $"Error getting applications: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveApplication([FromBody] JobApplication application)
        {
            try
            {
                var savedApplication = await _applicationTrackingService.SaveApplicationAsync(application);
                return Json(new { success = true, application = savedApplication });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in SaveApplication");
                return Json(new { success = false, message = $"Error saving application: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ContinueInterview([FromBody] dynamic request)
        {
            try
            {
                string previousAnswer = request.previousAnswer;
                string role = request.role;

                if (string.IsNullOrWhiteSpace(previousAnswer))
                {
                    return Json(new { success = false, message = "Previous answer is required" });
                }

                var interviewContent = await _interviewService.ContinueInterviewAsync(previousAnswer, role);
                
                return Json(new { 
                    success = true, 
                    html = interviewContent,
                    intent = "InterviewSimulation"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ContinueInterview");
                return Json(new { success = false, message = $"Error continuing interview: {ex.Message}" });
            }
        }
    }
}