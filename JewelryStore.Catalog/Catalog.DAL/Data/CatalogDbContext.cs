using JewelryStore.CatalogService.Catalog.DAL.Configuration;
using JewelryStore.CatalogService.Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace JewelryStore.CatalogService.Catalog.DAL.Data
{
    public class CatalogDbContext : DbContext
    {
        public CatalogDbContext(DbContextOptions<CatalogDbContext> options)
            : base(options) { }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Metal> Metals { get; set; }
        public DbSet<Stone> Stones { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductStone> ProductStones { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new CategoryConfiguration());
            modelBuilder.ApplyConfiguration(new MetalConfiguration());
            modelBuilder.ApplyConfiguration(new StoneConfiguration());
            modelBuilder.ApplyConfiguration(new ProductConfiguration());
            modelBuilder.ApplyConfiguration(new ProductStoneConfiguration());
        }
    }
}

