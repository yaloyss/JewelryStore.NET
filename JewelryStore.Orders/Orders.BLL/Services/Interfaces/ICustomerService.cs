using JewelryStore.OrdersService.Orders.BLL.DTOs;

namespace JewelryStore.OrdersService.Orders.BLL.Services.Interfaces
{
    public interface ICustomerService
    {
        Task<CustomerDTO> GetCustomerByIdAsync(int customerId, CancellationToken ct = default);
        Task<IEnumerable<CustomerDTO>> GetCustomersByNameAsync(string? firstName, string? lastName, CancellationToken ct = default);
        Task<int> CreateCustomerAsync(CustomerDTO customerDto, CancellationToken ct = default);
        Task<bool> UpdateCustomerAsync(CustomerDTO customerDto, CancellationToken ct = default);
        Task<bool> DeleteCustomerAsync(int customerId, CancellationToken ct = default);
    }
}