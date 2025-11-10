using JewelryStore.CatalogService.Catalog.Domain.Entities;

namespace JewelryStore.CatalogService.Catalog.DAL.Repositories.Interfaces
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        Task<Product?> GetProductWithDetailsAsync(int productId);
        Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId);
        Task<IEnumerable<Product>> GetProductsByMetalAsync(int metalId);
        Task<IEnumerable<Product>> GetProductsWithPriceRangeAsync(decimal minPrice, decimal maxPrice);
    }
}

