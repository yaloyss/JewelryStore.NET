using Catalog.Domain.Entities;

namespace Catalog.DAL.Repositories.Interfaces
{
    public interface IStoneRepository : IGenericRepository<Stone>
    {
        Task<Stone?> GetStoneByNameAsync(string name, CancellationToken cancellationToken = default);
    }
}

