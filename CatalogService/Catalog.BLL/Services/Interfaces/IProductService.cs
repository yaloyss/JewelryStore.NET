using Catalog.BLL.DTOs.Product;
using Catalog.BLL.DTOs.Stone;
using Catalog.DAL.Pagination;
using Catalog.Domain.Entities.Parameters;

namespace Catalog.BLL.Services.Interfaces
{
	public interface IProductService
	{
        Task<ProductDTO> GetProductByIdAsync(int productId, CancellationToken cancellationToken = default);
        Task<PagedResponse<ProductDTO>> GetProductsPagedAsync(ProductParameters parameters, CancellationToken cancellationToken = default);
        Task<ProductDetailedInfoDTO> GetProductWithDetailsAsync(int productId, CancellationToken cancellationToken = default);
        Task<ProductDTO> CreateProductAsync(CreateProductDTO dto, CancellationToken cancellationToken = default);
        Task DeleteProductAsync(int productId, CancellationToken cancellationToken = default);

        Task<IEnumerable<StoneDTO>> GetProductStonesAsync(int productId, CancellationToken cancellationToken = default);
        Task<IEnumerable<ProductDTO>> GetProductsByStoneNamesAsync(List<string> stoneNames, CancellationToken cancellationToken = default);
        Task<IEnumerable<ProductDetailedInfoDTO>> GetProductsWithMultipleStonesAsync(CancellationToken cancellationToken = default);
        Task AddStoneToProductAsync(int productId, int stoneId, CancellationToken cancellationToken = default);
        Task RemoveStoneFromProductAsync(int productId, int stoneId, CancellationToken cancellationToken = default);
    }
}

