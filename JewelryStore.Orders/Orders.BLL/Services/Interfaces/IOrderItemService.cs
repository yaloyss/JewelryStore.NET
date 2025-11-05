using JewelryStore.OrdersService.Orders.BLL.DTOs;

namespace JewelryStore.OrdersService.Orders.BLL.Services.Interfaces
{
    public interface IOrderItemService
    {
        Task<OrderItemDTO> GetOrderItemByIdAsync(int orderItemId);
        Task<IEnumerable<OrderItemDTO>> GetAllOrderItemsAsync();
        Task<IEnumerable<OrderItemDTO>> GetOrderItemsByOrderIdAsync(int orderId);
    }
}