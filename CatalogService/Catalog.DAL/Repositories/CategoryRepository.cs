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

        public async Task<Category?> GetCategoryByIdAsync(int categoryId)
        {
            var category = await _dbSet.FindAsync(categoryId);
            if (category != null)
            {
                await _context.Entry(category)
                    .Collection(c => c.Products)
                    .LoadAsync();
            }
            return category;
        }

        public async Task<IEnumerable<Product>> GetProductsForCategoryAsync(int categoryId)
        {
            var category = await _dbSet.FindAsync(categoryId);
            if (category == null)
            {
                return Enumerable.Empty<Product>();
            }

            await _context.Entry(category)
                .Collection(c => c.Products)
                .Query()
                .Include(p => p.Metal)
                .LoadAsync();

            return category.Products ?? Enumerable.Empty<Product>();
        }

        public async Task<int> GetProductCountByCategoryAsync(int categoryId)
        {
            var category = await _dbSet.FindAsync(categoryId);
            if (category == null)
            {
                return 0;
            }

            //explicit Loading with counting
            return await _context.Entry(category)
                .Collection(c => c.Products)
                .Query()
                .CountAsync();
        }

        public async Task<Dictionary<string, int>> GetCategoryStatisticsAsync(int categoryId)
        {
            var category = await _dbSet.FindAsync(categoryId);
            if (category == null)
            {
                return new Dictionary<string, int>();
            }

            var totalProducts = await _context.Entry(category)
                .Collection(c => c.Products)
                .Query()
                .CountAsync();

            var goldenProducts = await _context.Entry(category)
                .Collection(c => c.Products)
                .Query()
                .Where(p => p.Metal.Name.Contains("Gold"))
                .CountAsync();

            var silverProducts = await _context.Entry(category)
                .Collection(c => c.Products)
                .Query()
                .Where(p => p.Metal.Name.Contains("Silver"))
                .CountAsync();

            return new Dictionary<string, int>
            {
                { "TotalProducts", totalProducts },
                { "GoldenProducts", goldenProducts },
                { "SilverProducts", silverProducts }
            };
        }
    }
}

