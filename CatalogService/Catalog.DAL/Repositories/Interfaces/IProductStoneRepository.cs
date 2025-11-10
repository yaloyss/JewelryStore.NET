using Catalog.Domain.Entities;

namespace Catalog.DAL.Repositories.Interfaces
{
    public interface IProductStoneRepository : IGenericRepository<ProductStone>
    {
        Task<IEnumerable<ProductStone>> GetProductStonesWithDetailsAsync(int productId);
        Task<IEnumerable<Product>> GetProductsByStoneAsync(int stoneId);
        Task<IEnumerable<Stone>> GetProductStonesAsync(int productId);
        Task<bool> AddStoneToProductAsync(int productId, int stoneId);
        Task<bool> RemoveStoneFromProductAsync(int productId, int stoneId);
        Task<IEnumerable<ProductStone>> GetProductsWithMultipleStonesAsync();
        Task<IEnumerable<Product>> GetProductsByStoneNamesAsync(List<string> stoneNames);
    }
}

