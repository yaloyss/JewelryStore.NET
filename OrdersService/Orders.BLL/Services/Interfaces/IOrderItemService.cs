using Orders.BLL.DTOs;

namespace Orders.BLL.Services.Interfaces
{
    public interface IOrderItemService
    {
        Task<OrderItemDTO> GetOrderItemByIdAsync(int orderItemId, CancellationToken ct = default);
        Task<IEnumerable<OrderItemDTO>> GetAllOrderItemsAsync(CancellationToken ct = default);
        Task<IEnumerable<OrderItemDTO>> GetOrderItemsByOrderIdAsync(int orderId, CancellationToken ct = default);
    }
}