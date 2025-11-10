using Catalog.DAL.Data;
using Catalog.DAL.Repositories.Interfaces;
using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Catalog.DAL.Repositories
{
    public class StoneRepository : GenericRepository<Stone>, IStoneRepository
    {
        public StoneRepository(CatalogDbContext context) : base(context) { }

        public async Task<Stone?> GetStoneByNameAsync(string name)
        {
            return await _dbSet
                .FirstOrDefaultAsync(s => s.Name == name);
        }
    }
}

