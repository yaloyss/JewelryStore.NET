using Orders.Domain.Entities;

namespace Orders.DAL.Repositories.Interfaces
{
    public interface IOrderItemRepository 
    {
        Task<IEnumerable<OrderItem>> GetByOrderIdAsync(int orderId, CancellationToken ct = default);
    }
}