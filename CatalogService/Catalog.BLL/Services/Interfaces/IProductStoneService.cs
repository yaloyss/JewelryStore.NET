using Catalog.BLL.DTOs.Product;
using Catalog.BLL.DTOs.ProductStone;

namespace Catalog.BLL.Services.Interfaces
{
	public interface IProductStoneService
	{
        Task<IEnumerable<ProductDTO>> GetProductsByStoneAsync(int stoneId, CancellationToken cancellationToken = default);
        Task<IEnumerable<ProductDTO>> GetProductsByStoneNamesAsync(FindProductsByStoneNamesDTO dto, CancellationToken cancellationToken = default);
        Task<IEnumerable<ProductDetailedInfoDTO>> GetProductsWithMultipleStonesAsync(CancellationToken cancellationToken = default);
    }
}

