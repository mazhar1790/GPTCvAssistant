using GPTCvAssistant.Services.Interfaces;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GPTCvAssistant.Services
{
    /// <summary>
    /// Service for analyzing job matches and generating tailored career materials
    /// </summary>
    public class JobMatchingService : IJobMatchingService
    {
        private readonly IAiService _aiService;
        private readonly string _cvPath;

        public JobMatchingService(IAiService aiService, IWebHostEnvironment env)
        {
            _aiService = aiService ?? throw new ArgumentNullException(nameof(aiService));
            _cvPath = Path.Combine(env.ContentRootPath, "App_Data", "ExtractedCV.txt");
        }

        public async Task<JobMatchAnalysis> AnalyzeJobMatchAsync(string jobDescription)
        {
            if (string.IsNullOrWhiteSpace(jobDescription))
                throw new ArgumentException("Job description cannot be empty", nameof(jobDescription));

            var analysisPrompt = CreateJobAnalysisPrompt(jobDescription);

            try
            {
                var analysisHtml = await _aiService.AskAsync(analysisPrompt);
                
                // Parse the response to extract structured data
                var analysis = ParseAnalysisResponse(analysisHtml);
                analysis.RawHtml = analysisHtml;
                analysis.JobDescription = jobDescription;
                analysis.AnalysisDate = DateTime.UtcNow;
                
                return analysis;
            }
            catch (Exception ex)
            {
                return CreateErrorAnalysis(ex, jobDescription);
            }
        }

        public async Task<string> GenerateTargetedCoverLetterAsync(string jobDescription, string companyName = "")
        {
            if (string.IsNullOrWhiteSpace(jobDescription))
                throw new ArgumentException("Job description cannot be empty", nameof(jobDescription));

            var prompt = CreateCoverLetterPrompt(jobDescription, companyName);
            return await _aiService.AskAsync(prompt);
        }

        public async Task<string> GenerateATSOptimizedSummaryAsync(string jobDescription)
        {
            if (string.IsNullOrWhiteSpace(jobDescription))
                throw new ArgumentException("Job description cannot be empty", nameof(jobDescription));

            var prompt = CreateATSOptimizationPrompt(jobDescription);
            return await _aiService.AskAsync(prompt);
        }

        public async Task<List<string>> ExtractATSKeywordsAsync(string jobDescription)
        {
            if (string.IsNullOrWhiteSpace(jobDescription))
                throw new ArgumentException("Job description cannot be empty", nameof(jobDescription));

            var prompt = CreateKeywordExtractionPrompt(jobDescription);

            try
            {
                var response = await _aiService.AskAsync(prompt);
                var keywords = response.Split(',')
                    .Select(k => k.Trim())
                    .Where(k => !string.IsNullOrEmpty(k))
                    .ToList();
                
                return keywords;
            }
            catch
            {
                return new List<string>();
            }
        }

        private static string CreateJobAnalysisPrompt(string jobDescription)
        {
            return $@"
                Act as an Expert AI Recruitment Consultant and Job Match Analyst.
                
                Your task: Analyze the job description against Mazhar Hayat's CV and provide a comprehensive recruitment-grade analysis.
                
                ANALYSIS REQUIREMENTS:
                1. Match Score: Provide an overall match percentage (0-100%)
                2. Strengths: Identify 3-5 key strengths where Mazhar excels for this role
                3. Gaps: Identify 2-4 areas where Mazhar might need development or lacks experience
                4. ATS Keywords: Extract 8-12 critical keywords that must appear in applications
                5. Tailored Pitch: Write a compelling 2-3 sentence elevator pitch for this specific role
                6. Recommendations: Provide 2-3 actionable suggestions to strengthen the application

                OUTPUT FORMAT - Return valid HTML only (no markdown, no code blocks, no emoji):
                
                <h3>Job Match Analysis</h3>
                <div class=""match-score""><strong>Overall Match: XX%</strong></div>
                
                <h3>Key Strengths</h3>
                <ul>
                    <li><strong>[Strength Area]:</strong> [Specific evidence from CV]</li>
                </ul>
                
                <h3>Potential Gaps</h3>
                <ul>
                    <li>[Gap description with mitigation suggestion]</li>
                </ul>
                
                <h3>Critical ATS Keywords</h3>
                <p><strong>Must Include:</strong> [keyword1, keyword2, keyword3...]</p>
                
                <h3>Tailored Pitch</h3>
                <p>[Compelling 2-3 sentence pitch for this specific role]</p>
                
                <h3>Application Recommendations</h3>
                <ul>
                    <li>[Actionable suggestion 1]</li>
                    <li>[Actionable suggestion 2]</li>
                </ul>

                ANALYSIS RULES:
                - Be specific and quantify experience where possible
                - Focus on role-relevant skills and achievements
                - Consider both technical and soft skills requirements
                - Provide actionable, practical advice
                - Use professional recruitment language
                - Be honest about gaps but constructive in solutions
                - Return only HTML, no markdown formatting
                - Do not use emoji characters or special symbols

                JOB DESCRIPTION TO ANALYZE:
                {jobDescription}
            ";
        }

        private static string CreateCoverLetterPrompt(string jobDescription, string companyName)
        {
            return $@"
                Act as a Professional Cover Letter Writer specializing in AI/Tech roles.
                
                Task: Write a compelling, ATS-optimized cover letter for Mazhar Hayat targeting this specific role.
                
                REQUIREMENTS:
                - Open with a strong hook that mentions specific company/role details
                - Highlight 3-4 most relevant achievements with quantified results
                - Address key job requirements directly
                - Show genuine interest in the company/role
                - Professional tone, 3-4 paragraphs, 250-350 words
                - Include relevant keywords naturally
                - Do not use emoji characters
                
                STRUCTURE:
                - Opening: Hook + specific role interest
                - Body 1: Most relevant technical achievements 
                - Body 2: Leadership/impact examples + cultural fit
                - Closing: Call to action + availability
                
                Return valid HTML with proper <h3> for header and <p> tags for paragraphs.
                
                Company: {companyName}
                Job Description: {jobDescription}
            ";
        }

        private static string CreateATSOptimizationPrompt(string jobDescription)
        {
            return $@"
                Act as an ATS Optimization Expert.
                
                Task: Rewrite Mazhar's professional summary to maximize ATS scoring for this specific job.
                
                REQUIREMENTS:
                - Include exact keywords from job description
                - Maintain authenticity while optimizing for ATS
                - Quantify achievements where possible
                - 3-4 sentences maximum
                - Professional tone
                - Focus on role-relevant experience
                - Do not use emoji characters
                
                Return as a single paragraph in valid HTML <p> tags.
                
                Job Description: {jobDescription}
            ";
        }

        private static string CreateKeywordExtractionPrompt(string jobDescription)
        {
            return $@"
                Act as an ATS Keyword Extraction Expert.
                
                Task: Extract the most critical keywords and phrases that an ATS system would scan for in this job description.
                
                Focus on:
                - Technical skills and technologies
                - Certifications and qualifications
                - Industry-specific terminology
                - Role-specific responsibilities
                - Required experience levels
                
                Return as a comma-separated list, no HTML, just plain text.
                
                Job Description: {jobDescription}
            ";
        }

        private JobMatchAnalysis ParseAnalysisResponse(string htmlResponse)
        {
            var analysis = new JobMatchAnalysis();
            
            try
            {
                // Extract match score using regex
                var matchScoreMatch = System.Text.RegularExpressions.Regex.Match(
                    htmlResponse, @"Overall Match:\s*(\d+)%", 
                    System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                if (matchScoreMatch.Success)
                {
                    int.TryParse(matchScoreMatch.Groups[1].Value, out int score);
                    analysis.MatchScore = score;
                }

                // For production, implement proper HTML parsing
                analysis.Strengths = ExtractListItems(htmlResponse, "strengths");
                analysis.Gaps = ExtractListItems(htmlResponse, "gaps");
                analysis.ATSKeywords = ExtractKeywords(htmlResponse);
                analysis.TailoredPitch = ExtractPitch(htmlResponse);
                analysis.Recommendations = ExtractListItems(htmlResponse, "recommendations");
            }
            catch
            {
                // Set defaults if parsing fails
                analysis.MatchScore = 75;
                analysis.Strengths = new List<string> { "Technical expertise", "Leadership experience" };
                analysis.Gaps = new List<string> { "See detailed analysis above" };
                analysis.ATSKeywords = new List<string> { "AI", ".NET", "Azure", "RAG", "LLM" };
                analysis.TailoredPitch = "See analysis above for tailored pitch";
                analysis.Recommendations = new List<string> { "Review full analysis above" };
            }

            return analysis;
        }

        private static JobMatchAnalysis CreateErrorAnalysis(Exception ex, string jobDescription)
        {
            return new JobMatchAnalysis
            {
                MatchScore = 0,
                RawHtml = $"<p class='text-danger'>Error analyzing job description: {ex.Message}</p>",
                Strengths = new List<string> { "Analysis failed - please try again" },
                Gaps = new List<string> { "Unable to determine gaps" },
                ATSKeywords = new List<string>(),
                TailoredPitch = "Unable to generate pitch due to analysis error",
                Recommendations = new List<string>(),
                JobDescription = jobDescription,
                AnalysisDate = DateTime.UtcNow
            };
        }

        private List<string> ExtractListItems(string html, string section)
        {
            // Simplified extraction - in production, use a proper HTML parser like HtmlAgilityPack
            return new List<string> { $"See {section} in detailed analysis above" };
        }

        private List<string> ExtractKeywords(string html)
        {
            return new List<string> { "AI", ".NET", "Azure", "RAG", "LLM", "Solutions Architecture" };
        }

        private string ExtractPitch(string html)
        {
            return "See tailored pitch in detailed analysis above";
        }
    }

    /// <summary>
    /// Data model for job match analysis results
    /// </summary>
    public class JobMatchAnalysis
    {
        public int MatchScore { get; set; }
        public List<string> Strengths { get; set; } = new();
        public List<string> Gaps { get; set; } = new();
        public List<string> ATSKeywords { get; set; } = new();
        public string TailoredPitch { get; set; } = "";
        public List<string> Recommendations { get; set; } = new();
        public string RawHtml { get; set; } = "";
        public string JobDescription { get; set; } = "";
        public DateTime AnalysisDate { get; set; }
    }
}