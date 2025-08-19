using System.Text.Json;
using System.Net;

namespace GPTCvAssistant.Middleware
{
    /// <summary>
    /// Enhanced middleware for global exception handling with detailed logging and response mapping
    /// </summary>
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception exception)
            {
                var requestId = context.TraceIdentifier;
                _logger.LogError(exception, 
                    "Unhandled exception occurred. RequestId: {RequestId}, Path: {Path}, Method: {Method}", 
                    requestId, context.Request.Path, context.Request.Method);
                
                await HandleExceptionAsync(context, exception, requestId);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception, string requestId)
        {
            var env = context.RequestServices.GetRequiredService<IWebHostEnvironment>();
            
            // Determine response details based on exception type
            var (statusCode, message, details) = GetErrorResponse(exception, env.IsDevelopment());
            
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var response = new ErrorResponse
            {
                Success = false,
                Message = message,
                Details = details,
                RequestId = requestId,
                Timestamp = DateTime.UtcNow
            };

            // Add correlation ID for tracking
            if (!context.Response.Headers.ContainsKey("X-Correlation-ID"))
            {
                context.Response.Headers.Add("X-Correlation-ID", requestId);
            }

            var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(jsonResponse);
        }

        private static (HttpStatusCode statusCode, string message, string details) GetErrorResponse(Exception exception, bool isDevelopment)
        {
            return exception switch
            {
                ArgumentNullException => (
                    HttpStatusCode.BadRequest,
                    "Invalid request parameters",
                    isDevelopment ? exception.Message : "One or more required parameters are missing"
                ),
                ArgumentException => (
                    HttpStatusCode.BadRequest,
                    "Invalid request parameters",
                    isDevelopment ? exception.Message : "The provided parameters are not valid"
                ),
                UnauthorizedAccessException => (
                    HttpStatusCode.Unauthorized,
                    "Unauthorized access",
                    isDevelopment ? exception.Message : "You don't have permission to access this resource"
                ),
                FileNotFoundException => (
                    HttpStatusCode.NotFound,
                    "Required resource not found",
                    isDevelopment ? exception.Message : "The requested resource could not be found"
                ),
                TimeoutException => (
                    HttpStatusCode.RequestTimeout,
                    "Request timeout",
                    isDevelopment ? exception.Message : "The request took too long to complete. Please try again"
                ),
                InvalidOperationException when exception.Message.Contains("AI service") => (
                    HttpStatusCode.ServiceUnavailable,
                    "AI service temporarily unavailable",
                    isDevelopment ? exception.Message : "The AI service is currently unavailable. Please try again later"
                ),
                HttpRequestException => (
                    HttpStatusCode.BadGateway,
                    "External service error",
                    isDevelopment ? exception.Message : "An error occurred while communicating with external services"
                ),
                TaskCanceledException => (
                    HttpStatusCode.RequestTimeout,
                    "Request cancelled",
                    isDevelopment ? exception.Message : "The request was cancelled due to timeout"
                ),
                JsonException => (
                    HttpStatusCode.BadRequest,
                    "Invalid data format",
                    isDevelopment ? exception.Message : "The request contains invalid data format"
                ),
                _ => (
                    HttpStatusCode.InternalServerError,
                    "An unexpected error occurred",
                    isDevelopment ? $"{exception.GetType().Name}: {exception.Message}" : "Please try again later or contact support if the problem persists"
                )
            };
        }
    }

    /// <summary>
    /// Standard error response model
    /// </summary>
    public class ErrorResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Details { get; set; } = string.Empty;
        public string RequestId { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
    }
}