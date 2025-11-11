using Catalog.Domain.Entities;

namespace Catalog.DAL.Repositories.Interfaces
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        Task<Product?> GetProductWithDetailsAsync(int productId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Product>> GetProductsByMetalAsync(int metalId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Product>> GetProductsWithPriceRangeAsync(decimal minPrice, decimal maxPrice, CancellationToken cancellationToken = default);
    }
}

