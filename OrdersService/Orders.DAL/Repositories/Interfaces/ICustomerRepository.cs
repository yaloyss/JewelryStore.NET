using Orders.Domain.Entities;

namespace Orders.DAL.Repositories.Interfaces
{
	public interface ICustomerRepository : IGenericRepository<Customer>
    {
        Task<IEnumerable<Customer>> GetByNameAsync(string? firstName, string? lastName, CancellationToken ct = default);
    }
}

