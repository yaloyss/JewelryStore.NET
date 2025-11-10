using JewelryStore.CatalogService.Catalog.Domain.Entities;

namespace JewelryStore.CatalogService.Catalog.DAL.Repositories.Interfaces
{
    public interface IMetalRepository : IGenericRepository<Metal>
    {
        Task<Metal?> GetMetalByNameAsync(string name);
    }
}

