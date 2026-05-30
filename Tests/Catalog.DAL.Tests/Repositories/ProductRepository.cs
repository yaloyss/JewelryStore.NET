using Catalog.DAL.Data;
using Catalog.DAL.Repositories;
using Catalog.DAL.Tests.Helpers;
using Catalog.Domain.Entities;
using FluentAssertions;

namespace Catalog.DAL.Tests.Repositories
{
    public class ProductRepositoryTests : IDisposable
    {
        private readonly CatalogDbContext _context;
        private readonly ProductRepository _sut;

        public ProductRepositoryTests()
        {
            _context = DbContextFactory.Create();
            _sut = new ProductRepository(_context);
        }

        public void Dispose() => _context.Dispose();

        [Fact]
        public async Task GetProductWithDetailsAsync_ExistingProduct_ReturnsWithNavigationProperties()
        {
            // Arrange
            var metal = TestDataBuilder.CreateMetal(id: 1);
            var category = TestDataBuilder.CreateCategory(id: 1);
            var stone = TestDataBuilder.CreateStone(id: 1);
            var product = TestDataBuilder.CreateProduct(id: 1, categoryId: 1, metalId: 1);
            var productStone = TestDataBuilder.CreateProductStone(productId: 1, stoneId: 1);

            await _context.Metals.AddAsync(metal);
            await _context.Categories.AddAsync(category);
            await _context.Stones.AddAsync(stone);
            await _context.Products.AddAsync(product);
            await _context.ProductStones.AddAsync(productStone);
            await _context.SaveChangesAsync();

            // Act
            var result = await _sut.GetProductWithDetailsAsync(1);

            // Assert
            result.Should().NotBeNull();
            result!.Metal.Should().NotBeNull();
            result.Category.Should().NotBeNull();
            result.ProductStones.Should().NotBeNull();
            result.ProductStones.Should().HaveCount(1);
        }

        [Fact]
        public async Task GetProductWithDetailsAsync_NonExistingProduct_ReturnsNull()
        {
            var result = await _sut.GetProductWithDetailsAsync(999);

            result.Should().BeNull();
        }

        [Fact]
        public async Task GetProductWithDetailsAsync_ProductWithNoStones_ReturnsProductWithEmptyStones()
        {
            var metal = TestDataBuilder.CreateMetal(id: 1);
            var category = TestDataBuilder.CreateCategory(id: 1);
            var product = TestDataBuilder.CreateProduct(id: 1, categoryId: 1, metalId: 1);

            await _context.Metals.AddAsync(metal);
            await _context.Categories.AddAsync(category);
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            var result = await _sut.GetProductWithDetailsAsync(1);

            result.Should().NotBeNull();
            result!.ProductStones.Should().NotBeNull();
            result.ProductStones.Should().BeEmpty();
        }

        [Fact]
        public async Task GetProductWithDetailsAsync_ProductWithNoMetal_ReturnsProductWithNullMetal()
        {
            // Arrange продукт без металу, наприклад намисто з перлин
            var category = TestDataBuilder.CreateCategory(id: 1);
            var product = TestDataBuilder.CreateProduct(id: 1, categoryId: 1, metalId: null);

            await _context.Categories.AddAsync(category);
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            var result = await _sut.GetProductWithDetailsAsync(1);

            result.Should().NotBeNull();
            result!.Metal.Should().BeNull();
            result.Category.Should().NotBeNull();
        }

        [Fact]
        public async Task GetProductWithDetailsAsync_ProductWithMultipleStones_ReturnsAllStones()
        {
            var category = TestDataBuilder.CreateCategory(id: 1);
            var metal = TestDataBuilder.CreateMetal(id: 1);
            var stone1 = TestDataBuilder.CreateStone(id: 1, name: "Diamond");
            var stone2 = TestDataBuilder.CreateStone(id: 2, name: "Ruby");
            var product = TestDataBuilder.CreateProduct(id: 1, categoryId: 1, metalId: 1);
            var ps1 = TestDataBuilder.CreateProductStone(productId: 1, stoneId: 1);
            var ps2 = TestDataBuilder.CreateProductStone(productId: 1, stoneId: 2);

            await _context.Categories.AddAsync(category);
            await _context.Metals.AddAsync(metal);
            await _context.Stones.AddRangeAsync(stone1, stone2);
            await _context.Products.AddAsync(product);
            await _context.ProductStones.AddRangeAsync(ps1, ps2);
            await _context.SaveChangesAsync();

            // Act
            var result = await _sut.GetProductWithDetailsAsync(1);

            result.Should().NotBeNull();
            result!.ProductStones.Should().HaveCount(2);
            result.ProductStones!.Select(ps => ps.Stone.Name) .Should().Contain(new[] { "Diamond", "Ruby" });
        }
    }
}