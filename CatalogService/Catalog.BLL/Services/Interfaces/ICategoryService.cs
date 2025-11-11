using Catalog.BLL.DTOs.Category;
using Catalog.BLL.DTOs.Product;

namespace Catalog.BLL.Services.Interfaces
{
	public interface ICategoryService
	{
        Task<IEnumerable<CategoryDTO>> GetAllCategoriesAsync(CancellationToken cancellationToken = default);
        Task<CategoryDTO> GetCategoryByIdAsync(int categoryId, CancellationToken cancellationToken = default);
        Task<CategoryWithInfoDTO> GetCategoryWithDetailsAsync(int categoryId, CancellationToken cancellationToken = default);
        Task<CategoryStatisticsDTO> GetCategoryStatisticsAsync(int categoryId, CancellationToken cancellationToken = default);
        Task<CategoryDTO> CreateCategoryAsync(CreateCategoryDTO dto, CancellationToken cancellationToken = default);
        Task DeleteCategoryAsync(int categoryId, CancellationToken cancellationToken = default);
        Task<IEnumerable<ProductDTO>> GetProductsForCategoryAsync(int categoryId, CancellationToken cancellationToken = default);
        Task<int> GetProductCountByCategoryAsync(int categoryId, CancellationToken cancellationToken = default);
    }
}

