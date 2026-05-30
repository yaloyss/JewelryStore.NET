using System.Net;
using System.Net.Http.Json;
using Catalog.BLL.DTOs.Metal;
using Catalog.DAL.Data;
using Catalog.IntegrationTests.Infrastructure;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace Catalog.IntegrationTests.Controllers
{
    public class MetalsControllerTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private readonly CatalogDbContext _db;

        public MetalsControllerTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();

            var scope = factory.Services.CreateScope();
            _db = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
            DatabaseSeeder.Seed(_db);
        }

        // GET 

        [Fact]
        public async Task GetAllMetals_WhenDataExists_Returns200WithList()
        {
            var response = await _client.GetAsync("/api/metals");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var body = await response.Content.ReadFromJsonAsync<List<MetalDTO>>();
            body.Should().NotBeNull();
            body!.Should().HaveCountGreaterThanOrEqualTo(4); // Seeder додав 4 метали
        }


        [Fact]
        public async Task GetMetalById_ExistingId_Returns200WithCorrectData()
        {
            var response = await _client.GetAsync("/api/metals/1");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var body = await response.Content.ReadFromJsonAsync<MetalDTO>();
            body.Should().NotBeNull();
            body!.MetalId.Should().Be(1);
            body.Name.Should().Be("Gold");
            body.Color.Should().Be("Yellow");
        }

        [Fact]
        public async Task GetMetalById_NonExistingId_Returns404()
        {
            var response = await _client.GetAsync("/api/metals/99999");
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetMetalById_InvalidId_Returns400()
        {
            var response = await _client.GetAsync("/api/metals/-1");
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }


        [Fact]
        public async Task GetMetalByName_ExistingName_Returns200WithCorrectData()
        {
            var response = await _client.GetAsync("/api/metals/by-name/Silver");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var body = await response.Content.ReadFromJsonAsync<MetalDTO>();
            body.Should().NotBeNull();
            body!.Name.Should().Be("Silver");
        }

        [Fact]
        public async Task GetMetalByName_NonExistingName_Returns404()
        {
            var response = await _client.GetAsync("/api/metals/by-name/Adamantium");
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        // POST

        [Fact]
        public async Task CreateMetal_ValidDto_Returns201()
        {
            var dto = new CreateMetalDTO { Name = "Titanium", Color = "Grey" };

            var response = await _client.PostAsJsonAsync("/api/metals", dto);

            response.StatusCode.Should().Be(HttpStatusCode.Created);
            response.Headers.Location.Should().NotBeNull();

            var created = await response.Content.ReadFromJsonAsync<MetalDTO>();
            created!.Name.Should().Be("Titanium");
            created.Color.Should().Be("Grey");
        }

        [Fact]
        public async Task CreateMetal_DuplicateName_Returns409()
        {
            // "Gold" вже існує в Seeder
            var dto = new CreateMetalDTO { Name = "Gold", Color = "Yellow" };

            var response = await _client.PostAsJsonAsync("/api/metals", dto);

            response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        }

        [Fact]
        public async Task CreateMetal_CreatedMetalIsPersisted_CanBeRetrievedByName()
        {
            var dto = new CreateMetalDTO { Name = "Rhodium", Color = "White" };
            await _client.PostAsJsonAsync("/api/metals", dto);

            var getResponse = await _client.GetAsync("/api/metals/by-name/Rhodium");

            getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var fetched = await getResponse.Content.ReadFromJsonAsync<MetalDTO>();
            fetched!.Name.Should().Be("Rhodium");
        }

        // DELETE 

        [Fact]
        public async Task DeleteMetal_UnusedMetal_Returns204()
        {
            var createDto = new CreateMetalDTO { Name = "ToDelete", Color = "Black" };
            var createResponse = await _client.PostAsJsonAsync("/api/metals", createDto);
            var created = await createResponse.Content.ReadFromJsonAsync<MetalDTO>();

            var deleteResponse = await _client.DeleteAsync($"/api/metals/{created!.MetalId}");

            deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task DeleteMetal_UsedInProduct_Returns409()
        {
            var response = await _client.DeleteAsync("/api/metals/2");

            response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        }

        [Fact]
        public async Task DeleteMetal_NonExistingId_Returns404()
        {
            var response = await _client.DeleteAsync("/api/metals/99999");
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}