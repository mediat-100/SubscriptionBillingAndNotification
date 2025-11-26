using SubscriptionBillingAndNotificationCore.Dtos.Responses;
using System.Net;
using System.Text.Json;
using static SubscriptionBillingAndNotificationCore.Utilities.CustomExceptions;

namespace SubscriptionBillingAndNotification.Middlewares
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;
        private readonly IWebHostEnvironment _environment;

        public ErrorHandlingMiddleware(
            RequestDelegate next,
            ILogger<ErrorHandlingMiddleware> logger,
            IWebHostEnvironment environment)
        {
            _next = next;
            _logger = logger;
            _environment = environment;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                // Log the full exception details regardless of environment
                _logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);

                await HandleExceptionAsync(context, ex, _environment.IsDevelopment());
            }
        }

        private static async Task HandleExceptionAsync(
            HttpContext context,
            Exception exception,
            bool isDevelopment)
        {
            var response = context.Response;
            response.ContentType = "application/json";
            var errorResponse = new ErrorResponse();

            switch (exception)
            {
                case ValidationException ex:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    errorResponse.Message = ex.Message ?? "Validation failed";
                    break;

                case NotFoundException ex:
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    errorResponse.Message = ex.Message ?? "Resource not found";
                    break;

                case UnauthorizedException ex:
                    response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    errorResponse.Message = ex.Message ?? "Unauthorized access";
                    break;

                case ForbiddenException ex:
                    response.StatusCode = (int)HttpStatusCode.Forbidden;
                    errorResponse.Message = ex.Message ?? "Access forbidden";
                    break;

                case ArgumentException ex:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    errorResponse.Message = ex.Message ?? "Invalid argument";
                    break;

                case InvalidOperationException ex:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    errorResponse.Message = ex.Message ?? "Invalid operation";
                    break;

                default:
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    errorResponse.Message = "An internal server error occurred. Please try again later";
                    break;
            }

            errorResponse.StatusCode = response.StatusCode;
            errorResponse.Timestamp = DateTime.UtcNow;

            // Add additional debugging info in development
            if (isDevelopment || exception.InnerException != null)
            {
                errorResponse.Details = new ErrorDetails
                {
                    InnerException = exception.InnerException?.Message,
                    StackTrace = exception.StackTrace                   
                };
                
            }

            var jsonResponse = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = isDevelopment 
            });

            await response.WriteAsync(jsonResponse);
        }
    }
}