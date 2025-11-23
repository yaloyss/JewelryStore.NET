using Catalog.Domain.Entities;
using Catalog.Domain.Entities.Parameters;
using Catalog.DAL.Sorting;
using Catalog.DAL.Pagination;


namespace Catalog.DAL.Repositories.Interfaces
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        Task<Product?> GetProductWithDetailsAsync(int productId, CancellationToken cancellationToken = default);
        Task<PagedList<Product>> GetProductsPagedAsync(ProductParameters parameters, ISorting<Product>? sort = null, CancellationToken cancellationToken = default);
    }
}

