using Catalog.DAL.Data;
using Catalog.DAL.Repositories.Interfaces;
using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Catalog.DAL.Repositories
{
    public class MetalRepository : GenericRepository<Metal>, IMetalRepository
    {
        public MetalRepository(CatalogDbContext context) : base(context) { }


        public async Task<Metal?> GetMetalByNameAsync(string name)
        {
            return await _dbSet
                .FirstOrDefaultAsync(m => m.Name == name);
        }
    }
}
