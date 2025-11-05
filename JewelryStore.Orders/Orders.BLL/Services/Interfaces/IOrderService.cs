using JewelryStore.OrdersService.Orders.BLL.DTOs;

namespace JewelryStore.OrdersService.Orders.Application.Services.Interfaces
{
    public interface IOrderService
    {
        Task<OrderDTO> CreateOrderAsync(OrderCreateDTO orderCreateDto);

        Task<OrderDTO> GetOrderByIdAsync(int orderId);
        Task<IEnumerable<OrderListDTO>> GetAllOrdersAsync();
        Task<IEnumerable<OrderListDTO>> GetOrdersByCustomerNameAsync(string firstName, string lastName);

        Task<OrderDTO> UpdateOrderStatusAsync(int orderId, OrderStatusUpdateDTO statusUpdateDto);
        Task<bool> DeleteOrderAsync(int orderId);
    }
}