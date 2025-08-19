using System.Threading.Tasks;

namespace GPTCvAssistant.Services.Interfaces
{
    /// <summary>
    /// Common interface for AI services to enable polymorphism and testability
    /// </summary>
    public interface IAiService
    {
        Task<string> AskAsync(string question);
    }
}