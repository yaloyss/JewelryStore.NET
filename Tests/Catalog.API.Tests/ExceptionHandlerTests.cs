using System.Net;
using System.Text.Json;
using Catalog.API.Middleware;
using Catalog.BLL.Exceptions;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;

namespace Catalog.API.Tests
{
    public class ExceptionHandlerTests
    {
        private readonly Mock<ILogger<ExceptionHandler>> _loggerMock;
        private readonly Mock<IHostEnvironment> _envMock;

        public ExceptionHandlerTests()
        {
            _loggerMock = new Mock<ILogger<ExceptionHandler>>();
            _envMock    = new Mock<IHostEnvironment>();
        }

        // створює middleware, викликає з фейковим контекстом, повертає десеріалізований problemDetails з відповіді
        private async Task<(HttpContext context, ProblemDetails? problem)> InvokeMiddlewareAsync(
            Exception exceptionToThrow,
            bool isDevelopment = false)
        {
            // dev or prod env
            _envMock.Setup(e => e.EnvironmentName) .Returns(isDevelopment ? "Development" : "Production");

            // requestDelegate кидає виняток як наступний крок пайплайну
            RequestDelegate next = _ => throw exceptionToThrow;

            var middleware = new ExceptionHandler(next, _loggerMock.Object, _envMock.Object);

            var context = new DefaultHttpContext();     // фейковий httpContext для запису відповіді
            context.Response.Body = new MemoryStream();

            await middleware.InvokeAsync(context);

            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();

            ProblemDetails? problem = null;
            if (!string.IsNullOrWhiteSpace(responseBody))
            {
                problem = JsonSerializer.Deserialize<ProblemDetails>(responseBody, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }

            return (context, problem);
        }

        
        // NotFoundException 404
        [Fact]
        public async Task InvokeAsync_NotFoundException_Returns404()
        {
            var (context, _) = await InvokeMiddlewareAsync(
                new NotFoundException("Product with ID 1 not found."));

            context.Response.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task InvokeAsync_NotFoundException_ResponseBodyHasCorrectTitle()
        {
            var (_, problem) = await InvokeMiddlewareAsync(
                new NotFoundException("Product with ID 1 not found."));

            problem.Should().NotBeNull();
            problem!.Title.Should().Be("Resource Not Found");
        }

        [Fact]
        public async Task InvokeAsync_NotFoundException_ResponseBodyHasCorrectDetail()
        {
            var exceptionMessage = "Product with ID 1 not found.";
            var (_, problem) = await InvokeMiddlewareAsync(
                new NotFoundException(exceptionMessage));

            problem!.Detail.Should().Be(exceptionMessage);
        }

        [Fact]
        public async Task InvokeAsync_NotFoundException_ContentTypeIsProblemJson()
        {
            var (context, _) = await InvokeMiddlewareAsync(
                new NotFoundException("Not found."));

            context.Response.ContentType.Should().Be("application/problem+json");
        }

        
        // ValidationException 400 Bad Request
        [Fact]
        public async Task InvokeAsync_ValidationException_Returns400()
        {
            var (context, _) = await InvokeMiddlewareAsync(
                new ValidationException("ProductId must be greater than 0."));

            context.Response.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task InvokeAsync_ValidationException_ResponseBodyHasCorrectTitle()
        {
            var (_, problem) = await InvokeMiddlewareAsync(
                new ValidationException("ProductId must be greater than 0."));

            problem!.Title.Should().Be("Validation Error");
        }

        [Fact]
        public async Task InvokeAsync_ValidationException_ResponseBodyHasCorrectDetail()
        {
            var exceptionMessage = "ProductId must be greater than 0.";
            var (_, problem) = await InvokeMiddlewareAsync(
                new ValidationException(exceptionMessage));

            problem!.Detail.Should().Be(exceptionMessage);
        }

        
        // BusinessConflictException 409 
        [Fact]
        public async Task InvokeAsync_BusinessConflictException_Returns409()
        {
            var (context, _) = await InvokeMiddlewareAsync(
                new BusinessConflictException("Stone 'Ruby' is already added to this product."));

            context.Response.StatusCode.Should().Be((int)HttpStatusCode.Conflict);
        }

        [Fact]
        public async Task InvokeAsync_BusinessConflictException_ResponseBodyHasCorrectTitle()
        {
            var (_, problem) = await InvokeMiddlewareAsync(
                new BusinessConflictException("Conflict occurred."));

            problem!.Title.Should().Be("Business Conflict");
        }

        [Fact]
        public async Task InvokeAsync_BusinessConflictException_ResponseBodyHasCorrectDetail()
        {
            var exceptionMessage = "Stone 'Ruby' is already added to this product.";
            var (_, problem) = await InvokeMiddlewareAsync(
                new BusinessConflictException(exceptionMessage));

            problem!.Detail.Should().Be(exceptionMessage);
        }


        // Unhandled Exception 500 Internal Server Error
        [Fact]
        public async Task InvokeAsync_UnhandledException_Returns500()
        {
            var (context, _) = await InvokeMiddlewareAsync(
                new InvalidOperationException("Something went wrong."));

            context.Response.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
        }

        [Fact]
        public async Task InvokeAsync_UnhandledException_ResponseBodyHasCorrectTitle()
        {
            var (_, problem) = await InvokeMiddlewareAsync(
                new InvalidOperationException("Something went wrong."));

            problem!.Title.Should().Be("Internal Server Error");
        }

        
        // prod - деталі помилки приховані
        [Fact]
        public async Task InvokeAsync_UnhandledException_InProduction_HidesDetails()
        {
            var (_, problem) = await InvokeMiddlewareAsync(
                new InvalidOperationException("Secret internal details."),
                isDevelopment: false);

            // стандартне повідомлення без деталей виключення
            problem!.Detail.Should().Be("An unexpected error occurred. Please try again later.");
            problem.Detail.Should().NotContain("Secret internal details.");
        }

        
        // dev - видимі деталі помилки 
        [Fact]
        public async Task InvokeAsync_UnhandledException_InDevelopment_ShowsDetails()
        {
            var (_, problem) = await InvokeMiddlewareAsync(
                new InvalidOperationException("Secret internal details."),
                isDevelopment: true);

            // повне повідомлення виключення
            problem!.Detail.Should().Contain("Secret internal details.");
        }


        //  якщо виняток не кидається - middleware не чіпає відповідь
        [Fact]
        public async Task InvokeAsync_NoException_PassesThroughPipeline()
        {
            _envMock.Setup(e => e.EnvironmentName).Returns("Production");

            // next не кидає виняток i pipeline проходить нормально
            RequestDelegate next = ctx =>
            {
                ctx.Response.StatusCode = 200;
                return Task.CompletedTask;
            };

            var middleware = new ExceptionHandler(next, _loggerMock.Object, _envMock.Object);
            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();

            await middleware.InvokeAsync(context);

            // Assert — статус не змінився на помилку
            context.Response.StatusCode.Should().Be(200);
        }

        // traceId та instance у відповіді
        [Fact]
        public async Task InvokeAsync_AnyException_ResponseContainsTraceId()
        {
            var (_, problem) = await InvokeMiddlewareAsync(
                new NotFoundException("Not found."));

            problem!.Extensions.Should().ContainKey("traceId");
        }
    }
}