using System.Net;
using System.Net.Http.Json;
using Catalog.BLL.DTOs.Category;
using Catalog.BLL.DTOs.Product;
using Catalog.DAL.Data;
using Catalog.IntegrationTests.Infrastructure;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace Catalog.IntegrationTests.Controllers
{
    public class CategoriesControllerTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private readonly CatalogDbContext _db;

        public CategoriesControllerTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();

            var scope = factory.Services.CreateScope();
            _db = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
            DatabaseSeeder.Seed(_db);
        }

        // GET 

        [Fact]
        public async Task GetAllCategories_WhenDataExists_Returns200WithList()
        {
            var response = await _client.GetAsync("/api/categories");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var body = await response.Content.ReadFromJsonAsync<List<CategoryDTO>>();
            body.Should().NotBeNull();
            body!.Should().HaveCountGreaterThanOrEqualTo(3); // Rings, Earrings, Pendants
        }

        [Fact]
        public async Task GetCategoryById_ExistingId_Returns200WithCorrectData()
        {
            var response = await _client.GetAsync("/api/categories/1");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var body = await response.Content.ReadFromJsonAsync<CategoryDTO>();
            body.Should().NotBeNull();
            body!.CategoryId.Should().Be(1);
            body.Name.Should().Be("Rings");
        }

        [Fact]
        public async Task GetCategoryById_NonExistingId_Returns404()
        {
            var response = await _client.GetAsync("/api/categories/99999");

            //NotFoundException → middleware → 404
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetCategoryById_InvalidId_Returns400()
        {
            var response = await _client.GetAsync("/api/categories/-1");

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GetCategoryWithDetails_ExistingId_Returns200WithProducts()
        {
            var response = await _client.GetAsync("/api/categories/1/details");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var body = await response.Content.ReadFromJsonAsync<CategoryWithInfoDTO>();
            body.Should().NotBeNull();
            body!.CategoryId.Should().Be(1);
            body.Name.Should().Be("Rings");
            body.Products.Should().NotBeEmpty();
            body.ProductCount.Should().Be(body.Products.Count);
        }

        [Fact]
        public async Task GetCategoryWithDetails_NonExistingId_Returns404()
        {
            var response = await _client.GetAsync("/api/categories/99999/details");
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }


        [Fact]
        public async Task GetCategoryStatistics_ExistingId_Returns200WithStats()
        {
            // Rings (id=1) має 1 продукт з White Gold → GoldenProducts = 1
            var response = await _client.GetAsync("/api/categories/1/statistics");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var body = await response.Content.ReadFromJsonAsync<CategoryStatisticsDTO>();
            body.Should().NotBeNull();
            body!.CategoryId.Should().Be(1);
            body.TotalProducts.Should().BeGreaterThanOrEqualTo(1);
        }

        [Fact]
        public async Task GetCategoryStatistics_NonExistingId_Returns404()
        {
            var response = await _client.GetAsync("/api/categories/99999/statistics");
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetProductsForCategory_ExistingCategory_Returns200WithProducts()
        {
            var response = await _client.GetAsync("/api/categories/1/products");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var body = await response.Content.ReadFromJsonAsync<List<ProductDTO>>();
            body.Should().NotBeNull();
            body!.Should().NotBeEmpty();
            body.Should().OnlyContain(p => p.CategoryId == 1);
        }

        [Fact]
        public async Task GetProductsForCategory_NonExistingCategory_Returns404()
        {
            var response = await _client.GetAsync("/api/categories/99999/products");
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
        
        [Fact]
        public async Task GetProductCount_ExistingCategory_Returns200WithCorrectCount()
        {
            var response = await _client.GetAsync("/api/categories/2/products/count");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var count = await response.Content.ReadFromJsonAsync<int>();
            count.Should().Be(1);
        }

        // POST

        [Fact]
        public async Task CreateCategory_ValidDto_Returns201WithLocationHeader()
        {
            var dto = new CreateCategoryDTO { Name = "Bracelets" };

            var response = await _client.PostAsJsonAsync("/api/categories", dto);

            response.StatusCode.Should().Be(HttpStatusCode.Created);
            response.Headers.Location.Should().NotBeNull();

            var created = await response.Content.ReadFromJsonAsync<CategoryDTO>();
            created.Should().NotBeNull();
            created!.Name.Should().Be("Bracelets");
            created.CategoryId.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task CreateCategory_DuplicateName_Returns409()
        {
            // Rings вже є в Seeder
            var dto = new CreateCategoryDTO { Name = "Rings" };

            var response = await _client.PostAsJsonAsync("/api/categories", dto);

            response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        }

        [Fact]
        public async Task CreateCategory_CreatedCategoryIsPersisted_CanBeRetrievedByGet()
        {
            var dto = new CreateCategoryDTO { Name = "Necklaces" };

            var createResponse = await _client.PostAsJsonAsync("/api/categories", dto);
            var created = await createResponse.Content.ReadFromJsonAsync<CategoryDTO>();

            var getResponse = await _client.GetAsync($"/api/categories/{created!.CategoryId}");

            getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var fetched = await getResponse.Content.ReadFromJsonAsync<CategoryDTO>();
            fetched!.Name.Should().Be("Necklaces");
        }

        // DELETE 

        [Fact]
        public async Task DeleteCategory_EmptyCategory_Returns204AndCategoryIsGone()
        {
            // спочатку створює категорію без продуктів
            var createDto = new CreateCategoryDTO { Name = "ToDelete" };
            var createResponse = await _client.PostAsJsonAsync("/api/categories", createDto);
            var created = await createResponse.Content.ReadFromJsonAsync<CategoryDTO>();

            var deleteResponse = await _client.DeleteAsync($"/api/categories/{created!.CategoryId}");

            deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

            var getResponse = await _client.GetAsync($"/api/categories/{created.CategoryId}");
            getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task DeleteCategory_CategoryWithProducts_Returns409()
        {
            // Act — Rings (id=1) має продукти → BusinessConflictException → 409
            var response = await _client.DeleteAsync("/api/categories/1");

            response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        }

        [Fact]
        public async Task DeleteCategory_NonExistingId_Returns404()
        {
            var response = await _client.DeleteAsync("/api/categories/99999");
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}