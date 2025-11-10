using JewelryStore.CatalogService.Catalog.Domain.Entities;

namespace JewelryStore.CatalogService.Catalog.DAL.Repositories.Interfaces
{
    public interface IStoneRepository : IGenericRepository<Stone>
    {
        Task<Stone?> GetStoneByNameAsync(string name);
    }
}

