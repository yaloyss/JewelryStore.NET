using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Reviews.Domain.Exceptions;
using System.Net;
using System.Text.Json;

namespace Reviews.API.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
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
                _logger.LogError(ex, "Unhandled exception: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            ProblemDetails problemDetails = exception switch
            {
                //404
                NotFoundException notFoundEx =>
                    CreateProblemDetails(context, HttpStatusCode.NotFound,
                        "Resource Not Found", notFoundEx.Message
                    ),

                //400
                ValidationException validationEx =>
                    CreateProblemDetails(context, HttpStatusCode.BadRequest,
                        "Validation Error", validationEx.Message
                    ),

                //400
                DomainException domainEx =>
                    CreateProblemDetails(context, HttpStatusCode.BadRequest,
                        "Domain Error", domainEx.Message
                    ),

                //mongo write error (400)
                MongoWriteException mongoWriteEx =>
                    CreateProblemDetails(context, HttpStatusCode.BadRequest,
                        "Database Write Error", mongoWriteEx.Message
                    ),

                //mongo connection failure (503)
                MongoConnectionException mongoConnEx =>
                    CreateProblemDetails(context, HttpStatusCode.ServiceUnavailable,
                        "Database Connection Error", "Failed to connect to the database. Try again later."
                    ),

                //any mongo exception (500)
                MongoException mongoEx =>
                    CreateProblemDetails(context, HttpStatusCode.InternalServerError,
                        "Database Error", mongoEx.Message
                    ),

                //default errors (500)
                _ =>
                    CreateProblemDetails(context, HttpStatusCode.InternalServerError,
                        "Internal Server Error",
                        "An unexpected server error occurred."
                    )
            };

            context.Response.ContentType = "application/problem+json";
            context.Response.StatusCode = problemDetails.Status ?? 500;

            var json = JsonSerializer.Serialize(
                problemDetails,
                new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                }
            );

            await context.Response.WriteAsync(json);
        }

        private ProblemDetails CreateProblemDetails(HttpContext context, HttpStatusCode statusCode, string title, string detail)
        {
            var problem = new ProblemDetails
            {
                Status = (int)statusCode,
                Title = title,
                Detail = detail,
                Instance = context.Request.Path,
                Type = $"https://httpstatuses.com/{(int)statusCode}"
            };

            problem.Extensions["traceId"] = context.TraceIdentifier;
            return problem;
        }
    }
}
