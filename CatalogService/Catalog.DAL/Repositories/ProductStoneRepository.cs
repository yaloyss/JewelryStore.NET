using Catalog.DAL.Data;
using Catalog.DAL.Repositories.Interfaces;
using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Catalog.DAL.Repositories
{
    // LINQ to entities with many-to-many relations
    public class ProductStoneRepository : GenericRepository<ProductStone>, IProductStoneRepository
    {
        public ProductStoneRepository(CatalogDbContext context) : base(context) { }

        public async Task<IEnumerable<ProductStone>> GetProductStonesWithDetailsAsync(int productId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(ps => ps.ProductId == productId)
                .Include(ps => ps.Product).ThenInclude(p => p.Metal)
                .Include(ps => ps.Product).ThenInclude(p => p.Category)
                .Include(ps => ps.Stone)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Product>> GetProductsByStoneAsync(int stoneId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(ps => ps.StoneId == stoneId)
                .Select(ps => ps.Product)
                .Include(p => p.Metal)
                .Include(p => p.Category)
                .Distinct()
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Stone>> GetProductStonesAsync(int productId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(ps => ps.ProductId == productId)
                .Select(ps => ps.Stone)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> AddStoneToProductAsync(int productId, int stoneId, CancellationToken cancellationToken = default)
        {
            var exists = await _dbSet.AnyAsync(ps => ps.ProductId == productId && ps.StoneId == stoneId, cancellationToken);
            if (exists)
            {
                return false;
            }
            var productStone = new ProductStone
            {
                ProductId = productId,
                StoneId = stoneId
            };

            await _dbSet.AddAsync(productStone, cancellationToken);
            return true;
        }

        public async Task<bool> RemoveStoneFromProductAsync(int productId, int stoneId, CancellationToken cancellationToken = default)
        {
            var productStone = await _dbSet.FirstOrDefaultAsync(ps => ps.ProductId == productId && ps.StoneId == stoneId, cancellationToken);
            if (productStone == null)
            {
                return false;
            }

            _dbSet.Remove(productStone);
            return true;
        }

        public async Task<IEnumerable<ProductStone>> GetProductsWithMultipleStonesAsync(CancellationToken cancellationToken = default)
        {
            var productIdsWithMultipleStones = await _dbSet
                .GroupBy(ps => ps.ProductId)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToListAsync(cancellationToken);

            return await _dbSet
                .Where(ps => productIdsWithMultipleStones.Contains(ps.ProductId))
                .Include(ps => ps.Product).ThenInclude(p => p.Metal)
                .Include(ps => ps.Product).ThenInclude(p => p.Category)
                .Include(ps => ps.Stone)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Product>> GetProductsByStoneNamesAsync(List<string> stoneNames, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(ps => stoneNames.Contains(ps.Stone.Name))
                .Select(ps => ps.Product)
                .Include(p => p.Metal)
                .Include(p => p.Category)
                .Distinct()
                .ToListAsync(cancellationToken);
        }
    }
}
