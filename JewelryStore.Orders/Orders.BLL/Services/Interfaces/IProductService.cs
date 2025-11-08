using JewelryStore.OrdersService.Orders.BLL.DTOs;

namespace JewelryStore.OrdersService.Orders.BLL.Services.Interfaces
{
    public interface IProductService
    {
        Task<ProductDTO> GetProductByIdAsync(int productId, CancellationToken ct = default);
        Task<bool> IsProductAvailableAsync(int productId, CancellationToken ct = default);
    }
}