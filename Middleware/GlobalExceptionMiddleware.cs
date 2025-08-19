using System.Text.Json;

namespace GPTCvAssistant.Middleware
{
    /// <summary>
    /// Middleware for global exception handling
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
                _logger.LogError(exception, "An unhandled exception occurred");
                await HandleExceptionAsync(context, exception);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = 500;

            var env = context.RequestServices.GetRequiredService<IWebHostEnvironment>();
            var message = "An error occurred while processing your request.";
            var details = env.IsDevelopment() ? exception.Message : "Please try again later.";

            switch (exception)
            {
                case ArgumentNullException:
                    context.Response.StatusCode = 400;
                    message = "Invalid request parameters.";
                    break;
                case ArgumentException:
                    context.Response.StatusCode = 400;
                    message = "Invalid request parameters.";
                    break;
                case UnauthorizedAccessException:
                    context.Response.StatusCode = 401;
                    message = "Unauthorized access.";
                    break;
                case FileNotFoundException:
                    context.Response.StatusCode = 404;
                    message = "Required resource not found.";
                    break;
            }

            var response = new
            {
                success = false,
                message = message,
                details = details
            };

            var jsonResponse = JsonSerializer.Serialize(response);
            await context.Response.WriteAsync(jsonResponse);
        }
    }
}