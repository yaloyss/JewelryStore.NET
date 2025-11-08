using JewelryStore.OrdersService.Orders.BLL.DTOs;

namespace JewelryStore.OrdersService.Orders.Application.Services.Interfaces
{
    public interface IOrderService
    {
        Task<OrderDTO> CreateOrderAsync(OrderCreateDTO orderCreateDto, CancellationToken ct = default);

        Task<OrderDTO> GetOrderByIdAsync(int orderId, CancellationToken ct = default);
        Task<IEnumerable<OrderListDTO>> GetAllOrdersAsync(CancellationToken ct = default);
        Task<IEnumerable<OrderListDTO>> GetOrdersByCustomerNameAsync(string firstName, string lastName, CancellationToken ct = default);

        Task<OrderDTO> UpdateOrderStatusAsync(int orderId, OrderStatusUpdateDTO statusUpdateDto, CancellationToken ct = default);
        Task<bool> DeleteOrderAsync(int orderId, CancellationToken ct = default);
    }
}