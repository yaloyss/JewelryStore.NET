using Catalog.Domain.Entities;

namespace Catalog.DAL.Repositories.Interfaces
{
    public interface IMetalRepository : IGenericRepository<Metal>
    {
        Task<Metal?> GetMetalByNameAsync(string name);
    }
}

