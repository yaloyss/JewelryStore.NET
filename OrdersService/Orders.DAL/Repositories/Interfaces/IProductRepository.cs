using Orders.Domain.Entities;

namespace Orders.DAL.Repositories.Interfaces
{
    public interface IProductRepository
    {
        Task<Product?> GetByIdAsync(int id, CancellationToken ct = default);
    }
}