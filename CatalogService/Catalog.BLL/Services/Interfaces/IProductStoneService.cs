using Catalog.BLL.DTOs.Product;

namespace Catalog.BLL.Services.Interfaces
{
	public interface IProductStoneService
	{
        Task<IEnumerable<ProductDetailedInfoDTO>> GetProductsWithMultipleStonesAsync(CancellationToken cancellationToken = default);
    }
}

