using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Catalog.BLL.DTOs.Category;
using Catalog.DAL.Data;
using Catalog.IntegrationTests.Infrastructure;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Catalog.IntegrationTests.Middleware
{
    public class ExceptionHandlingMiddlewareTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private readonly CatalogDbContext _db;

        public ExceptionHandlingMiddlewareTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();

            var scope = factory.Services.CreateScope();
            _db = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
            DatabaseSeeder.Seed(_db);
        }

        // NotFoundException  404 Not Found

        [Fact]
        public async Task NotFoundException_Returns404WithProblemDetails()
        {
            // неіснуючий product
            var response = await _client.GetAsync("/api/products/99999");

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            response.Content.Headers.ContentType!.MediaType .Should().Be("application/problem+json");

            // структура ProblemDetails
            var body = await response.Content.ReadFromJsonAsync<ProblemDetails>();
            body.Should().NotBeNull();
            body!.Status.Should().Be(404);
            body.Title.Should().Be("Resource Not Found");
            body.Detail.Should().NotBeNullOrEmpty();
            body.Detail.Should().Contain("99999");
        }

        [Fact]
        public async Task NotFoundException_ResponseBodyDoesNotContainStackTrace()
        {
            var response = await _client.GetAsync("/api/products/99999");

            // stack trace не потрапляє клієнту
            var rawBody = await response.Content.ReadAsStringAsync();
            rawBody.Should().NotContain("at Catalog");
            rawBody.Should().NotContain("System.Exception");
        }

        // ValidationException  400 Bad Request

        [Fact]
        public async Task ValidationException_Returns400WithProblemDetails()
        {
            // у сервісі "ProductId must be greater than 0"
            var response = await _client.GetAsync("/api/products/-1");

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var body = await response.Content.ReadFromJsonAsync<ProblemDetails>();
            body.Should().NotBeNull();
            body!.Status.Should().Be(400);
            body.Title.Should().Be("Validation Error");
            body.Detail.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task ValidationException_MinPriceGreaterThanMaxPrice_Returns400WithCorrectTitle()
        {
            var response = await _client.GetAsync("/api/products?minPrice=99999&maxPrice=1");

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var body = await response.Content.ReadFromJsonAsync<ProblemDetails>();
            body!.Title.Should().Be("Validation Error");
            body.Detail.Should().Contain("MinPrice");
        }

        // BusinessConflictException 409 Conflict

        [Fact]
        public async Task BusinessConflictException_Returns409WithProblemDetails()
        {
            // Rings вже існує 
            var dto = new CreateCategoryDTO { Name = "Rings" };
            var response = await _client.PostAsJsonAsync("/api/categories", dto);

            response.StatusCode.Should().Be(HttpStatusCode.Conflict);

            var body = await response.Content.ReadFromJsonAsync<ProblemDetails>();
            body.Should().NotBeNull();
            body!.Status.Should().Be(409);
            body.Title.Should().Be("Business Conflict");
            body.Detail.Should().Contain("Rings");
        }

        // структура відповіді

        [Fact]
        public async Task ErrorResponse_AlwaysContainsRequiredProblemDetailsFields()
        {
            var response = await _client.GetAsync("/api/categories/99999");

            // ProblemDetails
            var rawBody = await response.Content.ReadAsStringAsync();
            var doc = JsonDocument.Parse(rawBody);
            var root = doc.RootElement;

            root.TryGetProperty("status",   out _).Should().BeTrue("'status' must be present");
            root.TryGetProperty("title",    out _).Should().BeTrue("'title' must be present");
            root.TryGetProperty("detail",   out _).Should().BeTrue("'detail' must be present");
            root.TryGetProperty("instance", out _).Should().BeTrue("'instance' must be present");
            root.TryGetProperty("traceId",  out _).Should().BeTrue("'traceId' extension is serialized as a top-level ProblemDetails property"); 
        }

        [Fact]
        public async Task ErrorResponse_InstanceFieldMatchesRequestPath()
        {
            var response = await _client.GetAsync("/api/metals/99999");

            // шлях запиту
            var body = await response.Content.ReadFromJsonAsync<ProblemDetails>();
            body!.Instance.Should().Contain("/api/metals/99999");
        }

        [Fact]
        public async Task SuccessResponse_IsNotProblemDetails()
        {
            var response = await _client.GetAsync("/api/products/1");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Content.Headers.ContentType!.MediaType .Should().NotBe("application/problem+json");
        }

        // 404 для неіснуючого маршруту

        [Fact]
        public async Task NonExistingRoute_Returns404()
        {
            var response = await _client.GetAsync("/api/nonexistent-resource");

            // ASP.NET Core routing повертає 404 для неіснуючих маршрутів
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}