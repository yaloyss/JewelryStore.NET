using Catalog.DAL.Data;
using Catalog.DAL.Repositories;
using Catalog.DAL.Tests.Helpers;
using FluentAssertions;

namespace Catalog.DAL.Tests.Repositories
{
    public class CategoryRepositoryTests : IDisposable
    {
        private readonly CatalogDbContext _context;
        private readonly CategoryRepository _sut;

        public CategoryRepositoryTests()
        {
            _context = DbContextFactory.Create();
            _sut = new CategoryRepository(_context);
        }

        public void Dispose() => _context.Dispose();

        [Fact]
        public async Task GetCategoryByIdAsync_ExistingId_ReturnsCategoryWithProducts()
        {
            var metal = TestDataBuilder.CreateMetal(id: 1);
            var category = TestDataBuilder.CreateCategory(id: 1, name: "Rings");
            var product = TestDataBuilder.CreateProduct(id: 1, categoryId: 1, metalId: 1);

            await _context.Metals.AddAsync(metal);
            await _context.Categories.AddAsync(category);
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            
            var result = await _sut.GetCategoryByIdAsync(1);

            result.Should().NotBeNull();
            result!.CategoryId.Should().Be(1);
            result.Products.Should().HaveCount(1);
        }

        [Fact]
        public async Task GetCategoryByIdAsync_NonExistingId_ReturnsNull()
        {
            var result = await _sut.GetCategoryByIdAsync(999);

            result.Should().BeNull();
        }

        [Fact]
        public async Task GetCategoryByIdAsync_CategoryWithNoProducts_ReturnsEmptyProductsList()
        {
            var category = TestDataBuilder.CreateCategory(id: 1, name: "Rings");
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

            var result = await _sut.GetCategoryByIdAsync(1);

            result.Should().NotBeNull();
            result!.Products.Should().BeEmpty();
        }

        [Fact]
        public async Task GetProductsForCategoryAsync_CategoryWithProducts_ReturnsProductsWithMetal()
        {
            var metal = TestDataBuilder.CreateMetal(id: 1, name: "Gold", color: "Yellow");
            var category = TestDataBuilder.CreateCategory(id: 1);
            var products = TestDataBuilder.CreateProducts(count: 2, categoryId: 1, metalId: 1);

            await _context.Metals.AddAsync(metal);
            await _context.Categories.AddAsync(category);
            await _context.Products.AddRangeAsync(products);
            await _context.SaveChangesAsync();

            var result = await _sut.GetProductsForCategoryAsync(1);

            var list = result.ToList();
            list.Should().HaveCount(2);
            list.Should().OnlyContain(p => p.Metal != null);
            list.Should().OnlyContain(p => p.Metal!.Name == "Gold");
        }

        [Fact]
        public async Task GetProductsForCategoryAsync_NonExistingCategory_ReturnsEmpty()
        {
            var result = await _sut.GetProductsForCategoryAsync(999);

            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetProductsForCategoryAsync_CategoryWithNoProducts_ReturnsEmpty()
        {
            var category = TestDataBuilder.CreateCategory(id: 1);
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

            var result = await _sut.GetProductsForCategoryAsync(1);

            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetProductCountByCategoryAsync_CategoryWithProducts_ReturnsCorrectCount()
        {
            var metal = TestDataBuilder.CreateMetal(id: 1);
            var category = TestDataBuilder.CreateCategory(id: 1);
            var products = TestDataBuilder.CreateProducts(count: 3, categoryId: 1, metalId: 1);

            await _context.Metals.AddAsync(metal);
            await _context.Categories.AddAsync(category);
            await _context.Products.AddRangeAsync(products);
            await _context.SaveChangesAsync();

            var result = await _sut.GetProductCountByCategoryAsync(1);

            result.Should().Be(3);
        }

        [Fact]
        public async Task GetProductCountByCategoryAsync_NonExistingCategory_ReturnsZero()
        {
            var result = await _sut.GetProductCountByCategoryAsync(999);

            result.Should().Be(0);
        }

        [Fact]
        public async Task GetCategoryStatisticsAsync_CategoryWithMixedMetals_ReturnsCorrectStats()
        {
            // Arrange
            var gold = TestDataBuilder.CreateMetal(id: 1, name: "Gold", color: "Yellow");
            var silver = TestDataBuilder.CreateMetal(id: 2, name: "Silver", color: "Silver");
            var platinum = TestDataBuilder.CreateMetal(id: 3, name: "Platinum", color: "White");

            var category = TestDataBuilder.CreateCategory(id: 1);

            var goldProduct1 = TestDataBuilder.CreateProduct(id: 1, categoryId: 1, metalId: 1);
            var goldProduct2 = TestDataBuilder.CreateProduct(id: 2, categoryId: 1, metalId: 1);
            var silverProduct = TestDataBuilder.CreateProduct(id: 3, categoryId: 1, metalId: 2);
            var platinumProduct = TestDataBuilder.CreateProduct(id: 4, categoryId: 1, metalId: 3);

            await _context.Metals.AddRangeAsync(gold, silver, platinum);
            await _context.Categories.AddAsync(category);
            await _context.Products.AddRangeAsync(goldProduct1, goldProduct2, silverProduct, platinumProduct);
            await _context.SaveChangesAsync();

            // Act
            var result = await _sut.GetCategoryStatisticsAsync(1);

            // Assert
            result.Should().NotBeEmpty();
            result["TotalProducts"].Should().Be(4);
            result["GoldenProducts"].Should().Be(2);
            result["SilverProducts"].Should().Be(1);
        }

        [Fact]
        public async Task GetCategoryStatisticsAsync_NonExistingCategory_ReturnsEmptyDictionary()
        {
            var result = await _sut.GetCategoryStatisticsAsync(999);

            result.Should().BeEmpty();
        }
    }
}