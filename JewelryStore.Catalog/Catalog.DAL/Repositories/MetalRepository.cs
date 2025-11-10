using JewelryStore.CatalogService.Catalog.DAL.Data;
using JewelryStore.CatalogService.Catalog.DAL.Repositories.Interfaces;
using JewelryStore.CatalogService.Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace JewelryStore.CatalogService.Catalog.DAL.Repositories
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
