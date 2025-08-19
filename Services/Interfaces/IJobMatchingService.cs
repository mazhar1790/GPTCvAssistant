using System.Collections.Generic;
using System.Threading.Tasks;
using GPTCvAssistant.Services;

namespace GPTCvAssistant.Services.Interfaces
{
    /// <summary>
    /// Interface for job matching and analysis services
    /// </summary>
    public interface IJobMatchingService
    {
        Task<JobMatchAnalysis> AnalyzeJobMatchAsync(string jobDescription);
        Task<string> GenerateTargetedCoverLetterAsync(string jobDescription, string companyName = "");
        Task<string> GenerateATSOptimizedSummaryAsync(string jobDescription);
        Task<List<string>> ExtractATSKeywordsAsync(string jobDescription);
    }
}