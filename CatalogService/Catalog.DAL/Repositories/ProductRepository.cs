using Ardalis.Specification.EntityFrameworkCore;
using Catalog.DAL.Data;
using Catalog.DAL.Pagination;
using Catalog.DAL.Repositories.Interfaces;
using Catalog.DAL.Sorting;
using Catalog.DAL.Specifications;
using Catalog.Domain.Entities;
using Catalog.Domain.Entities.Parameters;
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

        public async Task<PagedList<Product>> GetProductsPagedAsync(ProductParameters parameters, ISorting<Product>? sortHelper = null, CancellationToken cancellationToken = default)
        {
            var specification = new ProductWithFiltersSpecification(parameters);
            var query = _dbSet.WithSpecification(specification).ApplySorting(parameters.OrderBy, sortHelper);

            return await PagedList<Product>.ToPagedListAsync(query, parameters, cancellationToken);
        }
    }
}

