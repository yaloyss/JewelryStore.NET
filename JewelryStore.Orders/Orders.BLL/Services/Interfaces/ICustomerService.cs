using JewelryStore.OrdersService.Orders.BLL.DTOs;

namespace JewelryStore.OrdersService.Orders.BLL.Services.Interfaces
{
    public interface ICustomerService
    {
        Task<CustomerDTO> GetCustomerByIdAsync(int customerId);
        Task<IEnumerable<CustomerDTO>> GetCustomersByNameAsync(string? firstName, string? lastName);
        Task<int> CreateCustomerAsync(CustomerDTO customerDto);
        Task<bool> UpdateCustomerAsync(CustomerDTO customerDto);
        Task<bool> DeleteCustomerAsync(int customerId);
    }
}