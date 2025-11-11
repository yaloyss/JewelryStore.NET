using Catalog.Domain.Entities;

namespace Catalog.DAL.Repositories.Interfaces
{
    public interface ICategoryRepository : IGenericRepository<Category>
    {
        Task<Category?> GetCategoryByIdAsync(int categoryId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Product>> GetProductsForCategoryAsync(int categoryId, CancellationToken cancellationToken = default);
        Task<int> GetProductCountByCategoryAsync(int categoryId, CancellationToken cancellationToken = default);
        Task<Dictionary<string, int>> GetCategoryStatisticsAsync(int categoryId, CancellationToken cancellationToken = default);
    }
}

