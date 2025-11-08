using JewelryStore.OrdersService.Orders.Domain.Entities;

namespace JewelryStore.OrdersService.Orders.DAL.Repositories.Interfaces
{
    public interface IOrderItemRepository 
    {
        Task<IEnumerable<OrderItem>> GetByOrderIdAsync(int orderId, CancellationToken ct = default);
    }
}