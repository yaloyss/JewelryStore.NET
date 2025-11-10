using Catalog.Domain.Entities;

namespace Catalog.DAL.Repositories.Interfaces
{
    public interface ICategoryRepository : IGenericRepository<Category>
    {
        Task<Category?> GetCategoryByIdAsync(int categoryId);
        Task<IEnumerable<Product>> GetProductsForCategoryAsync(int categoryId);
        Task<int> GetProductCountByCategoryAsync(int categoryId);
        Task<Dictionary<string, int>> GetCategoryStatisticsAsync(int categoryId);
    }
}

