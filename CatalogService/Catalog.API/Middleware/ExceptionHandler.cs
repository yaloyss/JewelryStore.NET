using System.Diagnostics;
using System.Net;
using System.Text.Json;
using Catalog.BLL.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Serilog.Context;

namespace Catalog.API.Middleware
{
    public class ExceptionHandler
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandler> _logger;
        private readonly IHostEnvironment _environment;

        public ExceptionHandler(
            RequestDelegate next,
            ILogger<ExceptionHandler> logger,
            IHostEnvironment environment)
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
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var traceId = Activity.Current?.Id ?? context.TraceIdentifier;

            using (LogContext.PushProperty("TraceId", traceId))
            using (LogContext.PushProperty("RequestPath", context.Request.Path))
            {
                HttpStatusCode status;
                string title;
                string detail;

                switch (exception)
                {
                    case NotFoundException: //404
                        status = HttpStatusCode.NotFound;
                        title = "Resource Not Found";
                        detail = exception.Message;
                        _logger.LogWarning("Resource not found: {ErrorMessage}", exception.Message);
                        break;

                    case ValidationException: //404
                        status = HttpStatusCode.BadRequest;
                        title = "Validation Error";
                        detail = exception.Message;
                        _logger.LogWarning("Validation error: {ErrorMessage}", exception.Message);
                        break;

                    case BusinessConflictException: //409
                        status = HttpStatusCode.Conflict;
                        title = "Business Conflict";
                        detail = exception.Message;
                        _logger.LogWarning("Business conflict: {ErrorMessage}", exception.Message);
                        break;

                    default:
                        status = HttpStatusCode.InternalServerError; //500
                        title = "Internal Server Error";
                        //hidden details in prod, visible in dev
                        detail = _environment.IsDevelopment()
                            ? exception.ToString()
                            : "An unexpected error occurred. Please try again later.";
                        _logger.LogError(exception, "Unhandled exception: {ErrorMessage}", exception.Message);
                        break;
                }

                var problemDetails = new ProblemDetails
                {
                    Status = (int)status,
                    Title = title,
                    Detail = detail,
                    Instance = context.Request.Path,
                    Extensions = { ["traceId"] = traceId }
                };

                //stack trace for dev
                if (_environment.IsDevelopment() && exception.StackTrace != null)
                {
                    problemDetails.Extensions["stackTrace"] = exception.StackTrace;
                }
                context.Response.ContentType = "application/problem+json";
                context.Response.StatusCode = (int)status;

                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = _environment.IsDevelopment()
                };
                await context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails, options));
            }
        }
    }
}
