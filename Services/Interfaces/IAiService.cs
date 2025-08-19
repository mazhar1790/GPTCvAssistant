using System.Threading;
using System.Threading.Tasks;

namespace GPTCvAssistant.Services.Interfaces
{
    /// <summary>
    /// Common interface for AI services to enable polymorphism and testability
    /// </summary>
    public interface IAiService
    {
        /// <summary>
        /// Sends a question to the AI service and returns the response
        /// </summary>
        /// <param name="question">The question or prompt to send</param>
        /// <param name="cancellationToken">Cancellation token for the request</param>
        /// <returns>The AI service response</returns>
        Task<string> AskAsync(string question, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends a question with additional context to the AI service
        /// </summary>
        /// <param name="question">The question or prompt to send</param>
        /// <param name="context">Additional context or system messages</param>
        /// <param name="cancellationToken">Cancellation token for the request</param>
        /// <returns>The AI service response</returns>
        Task<string> AskWithContextAsync(string question, string context, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the current service health status
        /// </summary>
        /// <returns>True if the service is healthy, false otherwise</returns>
        Task<bool> IsHealthyAsync();

        /// <summary>
        /// Gets the service name for logging and monitoring
        /// </summary>
        string ServiceName { get; }
    }
}