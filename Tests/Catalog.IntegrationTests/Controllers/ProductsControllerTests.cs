using System.Net;
using System.Net.Http.Json;
using Catalog.BLL.DTOs.Product;
using Catalog.BLL.DTOs.Stone;
using Catalog.DAL.Data;
using Catalog.DAL.Pagination;
using Catalog.IntegrationTests.Infrastructure;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace Catalog.IntegrationTests.Controllers
{

    public class ProductsControllerTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private readonly CatalogDbContext _db;

        public ProductsControllerTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();

            var scope = factory.Services.CreateScope();
            _db = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
            DatabaseSeeder.Seed(_db);
        }

        // GET /api/products — список з пагінацією

        [Fact]
        public async Task GetAllProducts_WhenDataExists_Returns200WithPagedResponse()
        {
            var response = await _client.GetAsync("/api/products");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var body = await response.Content.ReadFromJsonAsync<PagedResponse<ProductDTO>>();
            body.Should().NotBeNull();
            body!.Items.Should().NotBeEmpty();
            body.Items.Should().HaveCountGreaterThanOrEqualTo(3); // Seeder додав 3 продукти
            body.TotalCount.Should().BeGreaterThanOrEqualTo(3);
        }

        [Fact]
        public async Task GetAllProducts_WithCategoryFilter_Returns200OnlyMatchingProducts()
        {
            // фільтр по CategoryId=1
            var response = await _client.GetAsync("/api/products?categoryId=1");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var body = await response.Content.ReadFromJsonAsync<PagedResponse<ProductDTO>>();
            body!.Items.Should().OnlyContain(p => p.CategoryId == 1);
        }

        [Fact]
        public async Task GetAllProducts_WithPriceRangeFilter_Returns200OnlyProductsInRange()
        {
            var response = await _client.GetAsync("/api/products?minPrice=5000&maxPrice=20000");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var body = await response.Content.ReadFromJsonAsync<PagedResponse<ProductDTO>>();
            body!.Items.Should().OnlyContain(p => p.Price >= 5000 && p.Price <= 20000);
        }

        [Fact]
        public async Task GetAllProducts_MinPriceGreaterThanMaxPrice_Returns400()
        {
            // порушення бізнес-правила: minPrice > maxPrice
            var response = await _client.GetAsync("/api/products?minPrice=50000&maxPrice=1000");

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GetAllProducts_WithSearchName_Returns200OnlyMatchingProducts()
        {
            // пошук по назві 
            var response = await _client.GetAsync("/api/products?searchName=Diamond");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var body = await response.Content.ReadFromJsonAsync<PagedResponse<ProductDTO>>();
            body!.Items.Should().NotBeEmpty();
            body.Items.Should().OnlyContain(p => p.Name.Contains("Diamond", StringComparison.OrdinalIgnoreCase));
        }

        [Fact]
        public async Task GetProductById_ExistingId_Returns200WithCorrectData()
        {
            var response = await _client.GetAsync("/api/products/1");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var body = await response.Content.ReadFromJsonAsync<ProductDTO>();
            body.Should().NotBeNull();
            body!.ProductId.Should().Be(1);
            body.Name.Should().Be("White Gold Diamond Ring");
            body.Price.Should().Be(28000);
            body.CategoryId.Should().Be(1);
        }

        [Fact]
        public async Task GetProductById_NonExistingId_Returns404()
        {
            var response = await _client.GetAsync("/api/products/99999");

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetProductById_InvalidId_Returns400()
        {
            var response = await _client.GetAsync("/api/products/-1");

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GetProductWithDetails_ExistingId_Returns200WithNavigationProperties()
        {
            var response = await _client.GetAsync("/api/products/1/details");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var body = await response.Content.ReadFromJsonAsync<ProductDetailedInfoDTO>();
            body.Should().NotBeNull();
            body!.ProductId.Should().Be(1);
            body.Category.Should().NotBeNull();
            body.Category.Name.Should().Be("Rings");
            body.Metal.Should().NotBeNull();
            body.Stones.Should().NotBeEmpty();
            body.Stones.Should().HaveCount(1); // product1 має тільки Diamond
        }

        [Fact]
        public async Task GetProductWithDetails_NonExistingId_Returns404()
        {
            var response = await _client.GetAsync("/api/products/99999/details");

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        // POST

        [Fact]
        public async Task CreateProduct_ValidDto_Returns201WithLocationHeader()
        {
            // Arrange
            var dto = new CreateProductDTO
            {
                Name       = "Integration Test Ring",
                Price      = 9500,
                Weight     = 4.0m,
                Size       = 17.0m,
                CategoryId = 1,
                MetalId    = 3,
                StoneIds   = new List<int> { 2 } // ruby
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/products", dto);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            response.Headers.Location.Should().NotBeNull();

            var created = await response.Content.ReadFromJsonAsync<ProductDTO>();
            created.Should().NotBeNull();
            created!.Name.Should().Be("Integration Test Ring");
            created.Price.Should().Be(9500);
            created.ProductId.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task CreateProduct_NonExistingCategory_Returns404()
        {
            // CategoryId яка не існує
            var dto = new CreateProductDTO
            {
                Name       = "Test Product",
                Price      = 1000,
                Weight     = 2.0m,
                CategoryId = 9999,
                StoneIds   = new List<int>()
            };

            var response = await _client.PostAsJsonAsync("/api/products", dto);

            // Assert — NotFoundException → middleware → 404
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task CreateProduct_NonExistingMetal_Returns404()
        {
            var dto = new CreateProductDTO
            {
                Name       = "Test Product",
                Price      = 1000,
                Weight     = 2.0m,
                CategoryId = 1,
                MetalId    = 9999, // не існує
                StoneIds   = new List<int>()
            };

            var response = await _client.PostAsJsonAsync("/api/products", dto);

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task CreateProduct_CreatedProductIsPersisted_CanBeRetrievedByGet()
        {
            var dto = new CreateProductDTO
            {
                Name       = "Persistency Check Ring",
                Price      = 3300,
                Weight     = 3.0m,
                CategoryId = 1,
                StoneIds   = new List<int>()
            };

            var createResponse = await _client.PostAsJsonAsync("/api/products", dto);
            var created = await createResponse.Content.ReadFromJsonAsync<ProductDTO>();

            var getResponse = await _client.GetAsync($"/api/products/{created!.ProductId}");

            // продукт збережено в БД
            getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var fetched = await getResponse.Content.ReadFromJsonAsync<ProductDTO>();
            fetched!.Name.Should().Be("Persistency Check Ring");
        }

        // DELETE 

        [Fact]
        public async Task DeleteProduct_ExistingId_Returns204AndProductIsGone()
        {
            var deleteResponse = await _client.DeleteAsync("/api/products/2");

            // успішне видалення
            deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

            var getResponse = await _client.GetAsync("/api/products/2");
            getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task DeleteProduct_NonExistingId_Returns404()
        {
            var response = await _client.DeleteAsync("/api/products/99999");

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task DeleteProduct_InvalidId_Returns400()
        {
            var response = await _client.DeleteAsync("/api/products/-1");

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        // GET /api/products/{id}/stones-of-product

        [Fact]
        public async Task GetProductStones_ProductWithStone_Returns200WithStones()
        {
            var response = await _client.GetAsync("/api/products/1/stones-of-product");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var body = await response.Content.ReadFromJsonAsync<List<StoneDTO>>();
            body.Should().NotBeNull();
            body!.Should().HaveCount(1);
            body[0].Name.Should().Be("Diamond");
        }

        [Fact]
        public async Task GetProductStones_NonExistingProduct_Returns404()
        {
            var response = await _client.GetAsync("/api/products/99999/stones-of-product");

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetProductsWithMultipleStones_Returns200WithOnlyMultiStoneProducts()
        {
            var response = await _client.GetAsync("/api/products/with-multiple-stones");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var body = await response.Content.ReadFromJsonAsync<List<ProductDetailedInfoDTO>>();
            body.Should().NotBeNull();
            body!.Should().HaveCount(1);
            body[0].Name.Should().Be("Emerald and Diamond Pendant");
        }

        // POST /{productId}/stones/{stoneId}/adding-stones-to-product

        [Fact]
        public async Task AddStoneToProduct_ValidIds_Returns204AndStoneIsAdded()
        {
            var response = await _client.PostAsync(
                "/api/products/1/stones/4/adding-stones-to-product", null);

            response.StatusCode.Should().Be(HttpStatusCode.NoContent);

            // Перевіряємо що камінь дійсно додано
            var stonesResponse = await _client.GetAsync("/api/products/1/stones-of-product");
            var stones = await stonesResponse.Content.ReadFromJsonAsync<List<StoneDTO>>();
            stones!.Should().HaveCount(2);
            stones.Should().Contain(s => s.Name == "Pearl");
        }

        [Fact]
        public async Task AddStoneToProduct_AlreadyAdded_Returns409()
        {
            var response = await _client.PostAsync(
                "/api/products/1/stones/1/adding-stones-to-product", null);

            response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        }

        [Fact]
        public async Task AddStoneToProduct_NonExistingProduct_Returns404()
        {
            var response = await _client.PostAsync(
                "/api/products/99999/stones/1/adding-stones-to-product", null);

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        // DELETE /{productId}/stones/{stoneId}

        [Fact]
        public async Task RemoveStoneFromProduct_ExistingRelation_Returns204()
        {
            var response = await _client.DeleteAsync("/api/products/2/stones/2");

            response.StatusCode.Should().Be(HttpStatusCode.NoContent);

            var stonesResponse = await _client.GetAsync("/api/products/2/stones-of-product");
            var stones = await stonesResponse.Content.ReadFromJsonAsync<List<StoneDTO>>();
            stones!.Should().BeEmpty();
        }

        [Fact]
        public async Task RemoveStoneFromProduct_NonExistingRelation_Returns404()
        {
            //Pearl (stoneId=4) не належить product1
            var response = await _client.DeleteAsync("/api/products/1/stones/4");

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task RemoveStoneFromProduct_NonExistingProduct_Returns404()
        {
            var response = await _client.DeleteAsync("/api/products/99999/stones/1");
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}