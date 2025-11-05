using JewelryStore.OrdersService.Orders.BLL.DTOs;

namespace JewelryStore.OrdersService.Orders.BLL.Services.Interfaces
{
    public interface IProductService
    {
        Task<ProductDTO> GetProductByIdAsync(int productId);
        Task<bool> IsProductAvailableAsync(int productId);
    }
}