using Catalog.DAL.Data;
using Catalog.DAL.Repositories.Interfaces;
using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Catalog.DAL.Repositories
{
    //eager Loading
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        public ProductRepository(CatalogDbContext context) : base(context) { }

        public async Task<Product?> GetProductWithDetailsAsync(int productId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(p => p.Metal)
                .Include(p => p.Category)
                .Include(p => p.ProductStones).ThenInclude(ps => ps.Stone)
                .FirstOrDefaultAsync(p => p.ProductId == productId, cancellationToken);
        }

        public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(p => p.Metal)
                .Include(p => p.Category)
                .Where(p => p.CategoryId == categoryId)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Product>> GetProductsByMetalAsync(int metalId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(p => p.Metal)
                .Include(p => p.Category)
                .Where(p => p.MetalId == metalId)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Product>> GetProductsWithPriceRangeAsync(decimal minPrice, decimal maxPrice, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(p => p.Metal)
                .Include(p => p.Category)
                .Where(p => p.Price >= minPrice && p.Price <= maxPrice)
                .OrderBy(p => p.Price)
                .ToListAsync(cancellationToken);
        }
    }
}

