using JewelryStore.OrdersService.Orders.Domain.Entities;

namespace JewelryStore.OrdersService.Orders.DAL.Repositories.Interfaces
{
	public interface ICustomerRepository : IGenericRepository<Customer>
    {
        Task<IEnumerable<Customer>> GetByNameAsync(string? firstName, string? lastName);
    }
}

