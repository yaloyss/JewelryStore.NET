using Catalog.Domain.Entities;

namespace Catalog.DAL.Repositories.Interfaces
{
    public interface IProductStoneRepository : IGenericRepository<ProductStone>
    {
        Task<IEnumerable<ProductStone>> GetProductStonesWithDetailsAsync(int productId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Product>> GetProductsByStoneAsync(int stoneId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Stone>> GetProductStonesAsync(int productId, CancellationToken cancellationToken = default);
        Task<bool> AddStoneToProductAsync(int productId, int stoneId, CancellationToken cancellationToken = default);
        Task<bool> RemoveStoneFromProductAsync(int productId, int stoneId, CancellationToken cancellationToken = default);
        Task<IEnumerable<ProductStone>> GetProductsWithMultipleStonesAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<Product>> GetProductsByStoneNamesAsync(List<string> stoneNames, CancellationToken cancellationToken = default);
    }
}

