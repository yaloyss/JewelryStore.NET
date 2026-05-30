using Catalog.DAL.Data;
using Catalog.DAL.Repositories;
using Catalog.DAL.Tests.Helpers;
using Catalog.Domain.Entities;
using FluentAssertions;

namespace Catalog.DAL.Tests.Repositories
{
    public class GenericRepositoryTests : IDisposable
    {
        private readonly CatalogDbContext _context;
        private readonly GenericRepository<Category> _sut;

        public GenericRepositoryTests()
        {
            _context = DbContextFactory.Create();
            _sut = new GenericRepository<Category>(_context);
        }

        public void Dispose() => _context.Dispose();

        
        [Fact]
        public async Task GetByIdAsync_ExistingId_ReturnsEntity()
        {
            var category = TestDataBuilder.CreateCategory(id: 6);
            await _context.Set<Category>().AddAsync(category);
            await _context.SaveChangesAsync();

            var result = await _sut.GetByIdAsync(6);

            result.Should().NotBeNull();
            result!.CategoryId.Should().Be(6);
        }

        [Fact]
        public async Task GetByIdAsync_NonExistingId_ReturnsNull()
        {
            var result = await _sut.GetByIdAsync(999);

            result.Should().BeNull();
        }

        [Fact]
        public async Task GetAllAsync_HasRecords_ReturnsAllEntities()
        {
            var categories = TestDataBuilder.CreateCategories(3);
            await _context.Set<Category>().AddRangeAsync(categories);
            await _context.SaveChangesAsync();

            var result = await _sut.GetAllAsync();

            result.Should().HaveCount(3);
            result.Should().NotContainNulls();
        }


        [Fact]
        public async Task AddAsync_ValidEntity_EntityIsPersisted()
        {
            var category = TestDataBuilder.CreateCategory(id: 1, name: "Rings");

            await _sut.AddAsync(category);
            await _context.SaveChangesAsync();

            _context.Set<Category>().Count().Should().Be(1);
        }

        [Fact]
        public async Task Update_ExistingEntity_ChangesArePersisted()
        {
            // Arrange
            var category = TestDataBuilder.CreateCategory(id: 1, name: "Rings");
            await _context.Set<Category>().AddAsync(category);
            await _context.SaveChangesAsync();

            var existing = await _sut.GetByIdAsync(1);
            existing!.Name = "Bracelets";

            // Act
            _sut.Update(existing);
            await _context.SaveChangesAsync();

            // Assert
            var updated = await _sut.GetByIdAsync(1);
            updated!.Name.Should().Be("Bracelets");
        }
        
        [Fact]
        public async Task Delete_ExistingEntity_EntityIsRemoved()
        {
            var cat1 = TestDataBuilder.CreateCategory(id: 1, name: "Rings");
            var cat2 = TestDataBuilder.CreateCategory(id: 2, name: "Earrings");
            await _context.Set<Category>().AddRangeAsync(cat1, cat2);
            await _context.SaveChangesAsync();

            _sut.Delete(cat1);
            await _context.SaveChangesAsync();

            var all = await _sut.GetAllAsync();
            all.Should().HaveCount(1);
            var deleted = await _sut.GetByIdAsync(cat1.CategoryId);
            deleted.Should().BeNull();
        }

        [Fact]
        public async Task Delete_NonExistingEntity_ThrowsException()
        {
            var ghost = TestDataBuilder.CreateCategory(id: 99, name: "Ghost");

            var act = async () =>
            {
                _sut.Delete(ghost);
                await _context.SaveChangesAsync();
            };

            await act.Should().ThrowAsync<Exception>();
        }
    }
}