using JewelryStore.OrdersService.Orders.Domain.Entities;

namespace JewelryStore.OrdersService.Orders.DAL.Repositories.Interfaces
{
    public interface IProductRepository
    {
        Task<Product?> GetByIdAsync(int id, CancellationToken ct = default);
    }
}