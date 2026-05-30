using Catalog.DAL.Data;
using Catalog.DAL.Repositories;
using Catalog.DAL.Tests.Helpers;
using Catalog.DAL.UOW;
using FluentAssertions;

namespace Catalog.DAL.Tests.UOW
{
    public class UnitOfWorkTests : IDisposable
    {
        private readonly CatalogDbContext _context;
        private readonly UnitOfWork _sut;

        public UnitOfWorkTests()
        {
            _context = DbContextFactory.Create();
            _sut = new UnitOfWork(_context);
        }

        public void Dispose() => _sut.Dispose();

        // initialized repositories
        [Fact]
        public void UnitOfWork_AllRepositoriesAreNotNull()
        {
            _sut.Products.Should().NotBeNull();
            _sut.Categories.Should().NotBeNull();
            _sut.Metals.Should().NotBeNull();
            _sut.Stones.Should().NotBeNull();
            _sut.ProductStones.Should().NotBeNull();
        }

        [Fact]
        public async Task SaveChangesAsync_AfterAddingEntity_PersistsToDatabase()
        {
            var category = TestDataBuilder.CreateCategory(id: 1, name: "Rings");
            await _sut.Categories.AddAsync(category);

            var rowsAffected = await _sut.SaveChangesAsync();

            rowsAffected.Should().Be(1);
            _context.Categories.Count().Should().Be(1);
        }
        
        [Fact]
        public async Task SaveChangesAsync_ValidOperations_ChangesArePersisted()
        {
            using var context = DbContextFactory.Create();
            var repo = new ProductRepository(context);
            var product = TestDataBuilder.CreateProduct(id: 1);

            await repo.AddAsync(product);
            await context.SaveChangesAsync();

            var saved = await context.Products.FindAsync(1);
            saved.Should().NotBeNull();
            saved!.Name.Should().Be(product.Name);
        }
        
        [Fact]
        public async Task WithoutSaveChanges_ChangesAreNotPersisted()
        {
            using var context = DbContextFactory.Create();
            var repo = new ProductRepository(context);
            var product = TestDataBuilder.CreateProduct(id: 1);

            await repo.AddAsync(product);

            var result = await context.Products.FindAsync(1);
            result.Should().NotBeNull();
        }
        
        // cross-repository operation
        [Fact]
        public async Task UnitOfWork_CrossRepositoryOperation_AllEntitiesAreSavedInOneSaveChanges()
        {
            var metal = TestDataBuilder.CreateMetal(id: 1);
            var category = TestDataBuilder.CreateCategory(id: 1);
            var product = TestDataBuilder.CreateProduct(id: 1, categoryId: 1, metalId: 1);

            await _sut.Metals.AddAsync(metal);
            await _sut.Categories.AddAsync(category);
            await _sut.Products.AddAsync(product);
            await _sut.SaveChangesAsync();

            _context.Metals.Count().Should().Be(1);
            _context.Categories.Count().Should().Be(1);
            _context.Products.Count().Should().Be(1);
        }
    }
}