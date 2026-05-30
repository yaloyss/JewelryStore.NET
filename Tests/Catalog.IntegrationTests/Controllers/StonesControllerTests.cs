using System.Net;
using System.Net.Http.Json;
using Catalog.BLL.DTOs.Stone;
using Catalog.DAL.Data;
using Catalog.IntegrationTests.Infrastructure;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace Catalog.IntegrationTests.Controllers
{
    public class StonesControllerTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private readonly CatalogDbContext _db;

        public StonesControllerTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();

            var scope = factory.Services.CreateScope();
            _db = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
            DatabaseSeeder.Seed(_db);
        }

        // GET

        [Fact]
        public async Task GetAllStones_WhenDataExists_Returns200WithList()
        {
            var response = await _client.GetAsync("/api/stones");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var body = await response.Content.ReadFromJsonAsync<List<StoneDTO>>();
            body.Should().NotBeNull();
            body!.Should().HaveCountGreaterThanOrEqualTo(4); // Diamond, Ruby, Emerald, Pearl
        }


        [Fact]
        public async Task GetStoneById_ExistingId_Returns200WithCorrectData()
        {
            var response = await _client.GetAsync("/api/stones/1");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var body = await response.Content.ReadFromJsonAsync<StoneDTO>();
            body.Should().NotBeNull();
            body!.StoneId.Should().Be(1);
            body.Name.Should().Be("Diamond");
        }

        [Fact]
        public async Task GetStoneById_NonExistingId_Returns404()
        {
            var response = await _client.GetAsync("/api/stones/99999");
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetStoneById_InvalidId_Returns400()
        {
            var response = await _client.GetAsync("/api/stones/0");
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }


        [Fact]
        public async Task GetStoneByName_ExistingName_Returns200WithCorrectData()
        {
            var response = await _client.GetAsync("/api/stones/by-name/Emerald");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var body = await response.Content.ReadFromJsonAsync<StoneDTO>();
            body.Should().NotBeNull();
            body!.Name.Should().Be("Emerald");
        }

        [Fact]
        public async Task GetStoneByName_NonExistingName_Returns404()
        {
            var response = await _client.GetAsync("/api/stones/by-name/Kryptonite");
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        // POST

        [Fact]
        public async Task CreateStone_ValidDto_Returns201()
        {
            var dto = new CreateStoneDTO { Name = "Opal" };

            var response = await _client.PostAsJsonAsync("/api/stones", dto);

            response.StatusCode.Should().Be(HttpStatusCode.Created);
            response.Headers.Location.Should().NotBeNull();

            var created = await response.Content.ReadFromJsonAsync<StoneDTO>();
            created!.Name.Should().Be("Opal");
            created.StoneId.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task CreateStone_DuplicateName_Returns409()
        {
            // "Diamond" вже є в Seeder
            var dto = new CreateStoneDTO { Name = "Diamond" };

            var response = await _client.PostAsJsonAsync("/api/stones", dto);

            response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        }

        [Fact]
        public async Task CreateStone_CreatedStoneIsPersisted_CanBeRetrievedByName()
        {
            var dto = new CreateStoneDTO { Name = "Moonstone" };
            await _client.PostAsJsonAsync("/api/stones", dto);

            var getResponse = await _client.GetAsync("/api/stones/by-name/Moonstone");

            getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        // DELETE 

        [Fact]
        public async Task DeleteStone_NonExistingId_Returns404()
        {
            var response = await _client.DeleteAsync("/api/stones/99999");
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}