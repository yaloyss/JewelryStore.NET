using Orders.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace Orders.API.Middleware
{
	public class ExceptionHandler
	{
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandler> _logger;

        public ExceptionHandler(RequestDelegate next, ILogger<ExceptionHandler> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            HttpStatusCode status;
            string title;
            string detail = exception.Message;

            switch (exception)
            {
                case NotFoundException:
                    status = HttpStatusCode.NotFound;
                    title = "Resource Not Found";
                    _logger.LogWarning(exception, "Resource not found: {Message}", exception.Message);
                    break;

                case ValidationException:
                    status = HttpStatusCode.BadRequest;
                    title = "Validation Error";
                    _logger.LogWarning(exception, "Validation error: {Message}", exception.Message);
                    break;

                case BusinessConflictException:
                    status = HttpStatusCode.Conflict;
                    title = "Business Conflict";
                    _logger.LogWarning(exception, "Business conflict: {Message}", exception.Message);
                    break;

                default:
                    status = HttpStatusCode.InternalServerError;
                    title = "Internal Server Error";
                    detail = "An unexpected error occurred. Please try again later.";
                    _logger.LogError(exception, "Unhandled exception occurred: {Message}", exception.Message);
                    break;
            }

            var problemDetails = new ProblemDetails
            {
                Status = (int)status,
                Title = title,
                Detail = detail,
                Instance = context.Request.Path
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)status;

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails, options));
        }
    }
}

