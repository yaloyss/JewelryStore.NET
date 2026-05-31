namespace JewelryStore.Catalog.Client;

public interface ICatalogProductClient
{
    Task<CatalogProductDto?> GetByIdAsync(int productId, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(int productId, CancellationToken cancellationToken = default);
}
