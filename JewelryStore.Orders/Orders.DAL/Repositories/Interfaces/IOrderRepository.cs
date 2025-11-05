using JewelryStore.OrdersService.Orders.Domain.Entities;

namespace JewelryStore.OrdersService.Orders.DAL.Repositories.Interfaces
{
	public interface IOrderRepository : IGenericRepository<Order>
	{
        Task<IEnumerable<Order>> GetByCustomerIdAsync(int customerId);
        Task<IEnumerable<Order>> GetByStatusAsync(string status);
    }
}

