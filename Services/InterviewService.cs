using GPTCvAssistant.Models;
using GPTCvAssistant.Services.Interfaces;

namespace GPTCvAssistant.Services
{
    /// <summary>
    /// Service for interview preparation and simulation
    /// </summary>
    public class InterviewService : IInterviewService
    {
        private readonly IAiService _aiService;
        private readonly ILogger<InterviewService> _logger;

        public InterviewService(IAiService aiService, ILogger<InterviewService> logger)
        {
            _aiService = aiService ?? throw new ArgumentNullException(nameof(aiService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<string> StartInterviewSimulationAsync(InterviewSimulationRequest request)
        {
            try
            {
                var prompt = $@"
                    Start a {request.InterviewType} interview simulation for {request.Role} position at {request.Company}.
                    Difficulty level: {request.Difficulty}
                    
                    Act as an experienced interviewer. Start with:
                    1. Welcome and brief company/role introduction
                    2. First interview question appropriate for the role and difficulty level
                    
                    For AI Solutions Architect role, focus on:
                    - Technical architecture and design patterns
                    - AI/ML implementation experience
                    - Cloud architecture (Azure/AWS)
                    - Leadership and team management
                    - Problem-solving scenarios
                    
                    Keep the question focused and realistic for a {request.Difficulty} level interview.
                    Format as HTML with clear structure.
                    No emoji characters.
                ";

                return await _aiService.AskAsync(prompt);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error starting interview simulation for {Role}", request.Role);
                throw;
            }
        }

        public async Task<string> ContinueInterviewAsync(string previousAnswer, string role)
        {
            try
            {
                var prompt = $@"
                    Continue the interview for {role} position.
                    
                    Candidate's previous answer: {previousAnswer}
                    
                    Provide:
                    1. Brief feedback on the previous answer (2-3 sentences)
                    2. Next interview question that builds on the conversation
                    3. Make the question progressively more challenging
                    
                    Focus on technical depth, real-world scenarios, and leadership capabilities.
                    Format as HTML with clear structure.
                    No emoji characters.
                ";

                return await _aiService.AskAsync(prompt);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error continuing interview for {Role}", role);
                throw;
            }
        }

        public async Task<InterviewFeedback> AnalyzeInterviewPerformanceAsync(string responses)
        {
            try
            {
                var prompt = $@"
                    Analyze the following interview responses and provide comprehensive feedback:
                    
                    {responses}
                    
                    Evaluate:
                    1. Technical knowledge demonstration
                    2. Communication clarity and structure
                    3. Problem-solving approach
                    4. Leadership and soft skills
                    5. Industry knowledge and trends awareness
                    6. Confidence and presentation
                    
                    Provide:
                    - Overall rating (Excellent/Good/Fair/Needs Improvement)
                    - Top 3 strengths demonstrated
                    - Top 3 areas for improvement
                    - Specific actionable suggestions
                    - Detailed feedback (100-150 words)
                    
                    Be constructive and specific in your feedback.
                    No emoji characters.
                ";

                var response = await _aiService.AskAsync(prompt);
                
                return new InterviewFeedback
                {
                    OverallRating = ExtractRating(response),
                    Strengths = ExtractList(response, "strengths"),
                    AreasForImprovement = ExtractList(response, "areas for improvement"),
                    Suggestions = ExtractList(response, "suggestions"),
                    DetailedFeedback = ExtractSection(response, "detailed feedback")
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error analyzing interview performance");
                throw;
            }
        }

        public async Task<List<string>> GenerateInterviewQuestionsAsync(string role, string interviewType)
        {
            try
            {
                var prompt = $@"
                    Generate 10 {interviewType} interview questions for {role} position.
                    
                    Include mix of:
                    1. Technical knowledge questions
                    2. Scenario-based problem solving
                    3. Behavioral questions (STAR method)
                    4. Industry and trends questions
                    5. Leadership and teamwork scenarios
                    
                    Make questions realistic and role-appropriate.
                    Vary difficulty from medium to challenging.
                    
                    Format as numbered list, one question per line.
                ";

                var response = await _aiService.AskAsync(prompt);
                return ParseQuestionsList(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating interview questions for {Role}", role);
                return new List<string>();
            }
        }

        private string ExtractRating(string response)
        {
            // Extract overall rating from response
            var ratingKeywords = new[] { "Excellent", "Good", "Fair", "Needs Improvement" };
            
            foreach (var keyword in ratingKeywords)
            {
                if (response.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                {
                    return keyword;
                }
            }
            
            return "Good"; // Default rating
        }

        private List<string> ExtractList(string response, string sectionName)
        {
            // Extract list items related to specific section
            var items = new List<string>();
            var lines = response.Split('\n');
            bool inSection = false;
            
            foreach (var line in lines)
            {
                var trimmed = line.Trim();
                
                if (trimmed.ToLower().Contains(sectionName.ToLower()))
                {
                    inSection = true;
                    continue;
                }
                
                if (inSection && (trimmed.StartsWith("-") || trimmed.StartsWith("•") || trimmed.StartsWith("*") || char.IsDigit(trimmed.FirstOrDefault())))
                {
                    var cleanItem = trimmed.TrimStart('-', '•', '*', '1', '2', '3', '4', '5', '6', '7', '8', '9', '0', '.', ' ').Trim();
                    if (!string.IsNullOrEmpty(cleanItem))
                    {
                        items.Add(cleanItem);
                    }
                }
                else if (inSection && string.IsNullOrWhiteSpace(trimmed))
                {
                    break; // End of section
                }
            }
            
            return items.Take(5).ToList(); // Limit to reasonable number
        }

        private string ExtractSection(string response, string sectionName)
        {
            // Extract detailed feedback section
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
            
            if (startIndex == -1) return "Detailed feedback not found";
            
            var content = new List<string>();
            for (int i = startIndex; i < lines.Length && i < startIndex + 10; i++)
            {
                if (!string.IsNullOrWhiteSpace(lines[i]) && !lines[i].StartsWith("#"))
                {
                    content.Add(lines[i].Trim());
                }
                else if (content.Any())
                {
                    break; // End of section
                }
            }
            
            return string.Join(" ", content);
        }

        private List<string> ParseQuestionsList(string response)
        {
            var questions = new List<string>();
            var lines = response.Split('\n');
            
            foreach (var line in lines)
            {
                var trimmed = line.Trim();
                
                // Check if line starts with number or bullet point
                if (char.IsDigit(trimmed.FirstOrDefault()) || trimmed.StartsWith("-") || trimmed.StartsWith("•"))
                {
                    var cleanQuestion = trimmed
                        .TrimStart('1', '2', '3', '4', '5', '6', '7', '8', '9', '0', '.', '-', '•', ' ')
                        .Trim();
                    
                    if (!string.IsNullOrEmpty(cleanQuestion) && cleanQuestion.Length > 10)
                    {
                        questions.Add(cleanQuestion);
                    }
                }
            }
            
            return questions.Take(10).ToList();
        }
    }
}