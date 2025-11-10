using JewelryStore.CatalogService.Catalog.DAL.Data;
using JewelryStore.CatalogService.Catalog.DAL.Repositories.Interfaces;
using JewelryStore.CatalogService.Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace JewelryStore.CatalogService.Catalog.DAL.Repositories
{
    // LINQ to Entities with many-to-many relations
    public class ProductStoneRepository : GenericRepository<ProductStone>, IProductStoneRepository
    {
        public ProductStoneRepository(CatalogDbContext context) : base(context) { }

        public async Task<IEnumerable<ProductStone>> GetProductStonesWithDetailsAsync(int productId)
        {
            return await _dbSet
                .Where(ps => ps.ProductId == productId)
                .Include(ps => ps.Product).ThenInclude(p => p.Metal)
                .Include(ps => ps.Product).ThenInclude(p => p.Category)
                .Include(ps => ps.Stone)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetProductsByStoneAsync(int stoneId)
        {
            return await _dbSet
                .Where(ps => ps.StoneId == stoneId)
                .Select(ps => ps.Product)
                .Include(p => p.Metal)
                .Include(p => p.Category)
                .Distinct()
                .ToListAsync();
        }

        public async Task<IEnumerable<Stone>> GetProductStonesAsync(int productId)
        {
            return await _dbSet
                .Where(ps => ps.ProductId == productId)
                .Select(ps => ps.Stone)
                .ToListAsync();
        }

        public async Task<bool> AddStoneToProductAsync(int productId, int stoneId)
        {
            var exists = await _dbSet.AnyAsync(ps => ps.ProductId == productId && ps.StoneId == stoneId);
            if (exists)
            {
                return false;
            }
            var productStone = new ProductStone
            {
                ProductId = productId,
                StoneId = stoneId
            };

            await _dbSet.AddAsync(productStone);
            return true;
        }

        public async Task<bool> RemoveStoneFromProductAsync(int productId, int stoneId)
        {
            var productStone = await _dbSet.FirstOrDefaultAsync(ps => ps.ProductId == productId && ps.StoneId == stoneId);
            if (productStone == null)
            {
                return false;
            }

            _dbSet.Remove(productStone);
            return true;
        }

        public async Task<IEnumerable<ProductStone>> GetProductsWithMultipleStonesAsync()
        {
            var productIdsWithMultipleStones = await _dbSet
                .GroupBy(ps => ps.ProductId)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToListAsync();

            return await _dbSet
                .Where(ps => productIdsWithMultipleStones.Contains(ps.ProductId))
                .Include(ps => ps.Product).ThenInclude(p => p.Metal)
                .Include(ps => ps.Product).ThenInclude(p => p.Category)
                .Include(ps => ps.Stone)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetProductsByStoneNamesAsync(List<string> stoneNames)
        {
            return await _dbSet
                .Where(ps => stoneNames.Contains(ps.Stone.Name))
                .Select(ps => ps.Product)
                .Include(p => p.Metal)
                .Include(p => p.Category)
                .Distinct()
                .ToListAsync();
        }
    }
}

