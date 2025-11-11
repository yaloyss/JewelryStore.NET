using Catalog.DAL.Data;
using Catalog.DAL.Repositories.Interfaces;
using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Catalog.DAL.Repositories
{
    //explicit Loading
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(CatalogDbContext context) : base(context) { }

        public async Task<Category?> GetCategoryByIdAsync(int categoryId, CancellationToken cancellationToken = default)
        {
            var category = await _dbSet.FindAsync(new object[] { categoryId }, cancellationToken);
            if (category != null)
            {
                await _context.Entry(category)
                    .Collection(c => c.Products)
                    .LoadAsync(cancellationToken);
            }
            return category;
        }

        public async Task<IEnumerable<Product>> GetProductsForCategoryAsync(int categoryId, CancellationToken cancellationToken = default)
        {
            var category = await _dbSet.FindAsync(new object[] { categoryId }, cancellationToken);
            if (category == null)
            {
                return Enumerable.Empty<Product>();
            }

            await _context.Entry(category)
                .Collection(c => c.Products)
                .Query()
                .Include(p => p.Metal)
                .LoadAsync(cancellationToken);

            return category.Products ?? Enumerable.Empty<Product>();
        }

        public async Task<int> GetProductCountByCategoryAsync(int categoryId, CancellationToken cancellationToken = default)
        {
            var category = await _dbSet.FindAsync(new object[] { categoryId }, cancellationToken);
            if (category == null)
            {
                return 0;
            }

            return await _context.Entry(category)
                .Collection(c => c.Products)
                .Query()
                .CountAsync(cancellationToken);
        }

        public async Task<Dictionary<string, int>> GetCategoryStatisticsAsync(int categoryId, CancellationToken cancellationToken = default)
        {
            var category = await _dbSet.FindAsync(new object[] { categoryId }, cancellationToken);
            if (category == null)
            {
                return new Dictionary<string, int>();
            }

            var totalProducts = await _context.Entry(category)
                .Collection(c => c.Products)
                .Query()
                .CountAsync(cancellationToken);

            var goldenProducts = await _context.Entry(category)
                .Collection(c => c.Products)
                .Query()
                .Where(p => p.Metal.Name.Contains("Gold"))
                .CountAsync(cancellationToken);

            var silverProducts = await _context.Entry(category)
                .Collection(c => c.Products)
                .Query()
                .Where(p => p.Metal.Name.Contains("Silver"))
                .CountAsync(cancellationToken);

            return new Dictionary<string, int>
            {
                { "TotalProducts", totalProducts },
                { "GoldenProducts", goldenProducts },
                { "SilverProducts", silverProducts }
            };
        }
    }
}

