using JewelryStore.CatalogService.Catalog.DAL.Data;
using JewelryStore.CatalogService.Catalog.DAL.Repositories.Interfaces;
using JewelryStore.CatalogService.Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace JewelryStore.CatalogService.Catalog.DAL.Repositories
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

