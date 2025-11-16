using Catalog.DAL.Configuration;
using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Catalog.DAL.Data
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
            DataSeeding(modelBuilder);
        }

        private void DataSeeding(ModelBuilder modelBuilder)
        {
            //categories
            modelBuilder.Entity<Category>().HasData(
                new Category { CategoryId = 1, Name = "Rings" },
                new Category { CategoryId = 2, Name = "Earrings" },
                new Category { CategoryId = 3, Name = "Pendants" },
                new Category { CategoryId = 4, Name = "Bracelets" },
                new Category { CategoryId = 5, Name = "Necklaces" }
            );
            //metals
            modelBuilder.Entity<Metal>().HasData(
                new Metal { MetalId = 1, Name = "Gold", Color = "Yellow" },
                new Metal { MetalId = 2, Name = "Gold", Color = "White" },
                new Metal { MetalId = 3, Name = "Gold", Color = "Rose" },
                new Metal { MetalId = 4, Name = "Silver", Color = "Silver" },
                new Metal { MetalId = 5, Name = "Platinum", Color = "White" }
            );
            //stones
            modelBuilder.Entity<Stone>().HasData(
                new Stone { StoneId = 1, Name = "Diamond" },
                new Stone { StoneId = 2, Name = "Ruby" },
                new Stone { StoneId = 3, Name = "Emerald" },
                new Stone { StoneId = 4, Name = "Moonstone" },
                new Stone { StoneId = 5, Name = "Amethyst" },
                new Stone { StoneId = 6, Name = "Garnet" },
                new Stone { StoneId = 7, Name = "Opal" },
                new Stone { StoneId = 8, Name = "Pearl" },
                new Stone { StoneId = 9, Name = "Cubic Zirconia" },
                new Stone { StoneId = 10, Name = "Onyx" },
                new Stone { StoneId = 11, Name = "Smoky Quartz" }
            );
            //products
            modelBuilder.Entity<Product>().HasData(
                new Product { ProductId = 1, Name = "White Gold Ring with Diamond", Price = 28000, Weight = 3.2m, Size = 16.5m, Manufacturer = "Ukraine", CategoryId = 1, MetalId = 2 },
                new Product { ProductId = 2, Name = "Platinum Smoky Quartz Ring", Price = 32000, Weight = 4.5m, Size = 16.5m, Manufacturer = "Ukraine", CategoryId = 1, MetalId = 5 },
                new Product { ProductId = 3, Name = "Silver Amethyst Ring", Price = 3100, Weight = 3.8m, Size = 17.5m, Manufacturer = "Ukraine", CategoryId = 1, MetalId = 4 },
                new Product { ProductId = 4, Name = "Silver Ring with Cubic Zirconia", Price = 1200, Weight = 8.5m, Size = 18, Manufacturer = "Ukraine", CategoryId = 1, MetalId = 4 },

                new Product { ProductId = 5, Name = "Diamond Stud Earrings", Price = 28000, Weight = 2.1m, Size = null, Manufacturer = "Ukraine", CategoryId = 2, MetalId = 2 },
                new Product { ProductId = 6, Name = "Emerald Earrings", Price = 43000, Weight = 6.3m, Size = null, Manufacturer = "Ukraine", CategoryId = 2, MetalId = 2 },
                new Product { ProductId = 7, Name = "Silver Hoop Earrings with Cubic Zirconia", Price = 3500, Weight = 4.2m, Size = null, Manufacturer = "Ukraine", CategoryId = 2, MetalId = 4 },
                new Product { ProductId = 8, Name = "Pearl Earrings", Price = 12000, Weight = 3.5m, Size = null, Manufacturer = "Ukraine", CategoryId = 2, MetalId = 1 },

                new Product { ProductId = 9, Name = "Cross Pendant with Onyx", Price = 6600, Weight = 1.8m, Size = 2.5m, Manufacturer = "Ukraine", CategoryId = 3, MetalId = 4 },
                new Product { ProductId = 10, Name = "Heart Pendant with Moonstone", Price = 2150, Weight = 2.3m, Size = 2.0m, Manufacturer = "Ukraine", CategoryId = 3, MetalId = 4 },
                new Product { ProductId = 11, Name = "Smoky Quartz Pendant", Price = 9900, Weight = 1.5m, Size = 1.8m, Manufacturer = "Ukraine", CategoryId = 3, MetalId = 4 },
                new Product { ProductId = 12, Name = "Garnet Pendant", Price = 2000, Weight = 1.2m, Size = 1.5m, Manufacturer = "Ukraine", CategoryId = 3, MetalId = 4 },

                new Product { ProductId = 13, Name = "Silver Cross Bracelet with Onyx", Price = 8900, Weight = 15.5m, Size = 19, Manufacturer = "Ukraine", CategoryId = 4, MetalId = 4 },
                new Product { ProductId = 14, Name = "Moonstone and Opal Bracelet", Price = 23000, Weight = 12.4m, Size = 18, Manufacturer = "Ukraine", CategoryId = 4, MetalId = 4 },
                new Product { ProductId = 15, Name = "Thin Chain Bracelet", Price = 5100, Weight = 3.1m, Size = 18, Manufacturer = "Ukraine", CategoryId = 4, MetalId = 5 },
                new Product { ProductId = 16, Name = "Pearl Bracelet", Price = 26000, Weight = 9.8m, Size = 17.5m, Manufacturer = "Ukraine", CategoryId = 4, MetalId = 2 },

                new Product { ProductId = 17, Name = "Pearl Necklace", Price = 25000, Weight = 45.0m, Size = 45, Manufacturer = "Ukraine", CategoryId = 5, MetalId = null },
                new Product { ProductId = 18, Name = "Garnet Necklace", Price = 18500, Weight = 52.0m, Size = 50, Manufacturer = "Ukraine", CategoryId = 5, MetalId = 4 },
                new Product { ProductId = 19, Name = "Diamond Necklace", Price = 51500, Weight = 38.5m, Size = 48, Manufacturer = "Ukraine", CategoryId = 5, MetalId = 4 }
            );
            modelBuilder.Entity<ProductStone>().HasData(
                new ProductStone { ProductId = 1, StoneId = 1 },
                new ProductStone { ProductId = 2, StoneId = 11 },
                new ProductStone { ProductId = 3, StoneId = 5 }, 
                new ProductStone { ProductId = 4, StoneId = 9 },
                new ProductStone { ProductId = 5, StoneId = 1 }, 
                new ProductStone { ProductId = 6, StoneId = 3 },
                new ProductStone { ProductId = 7, StoneId = 9 },
                new ProductStone { ProductId = 8, StoneId = 8 },
                new ProductStone { ProductId = 9, StoneId = 10 },
                new ProductStone { ProductId = 10, StoneId = 4 },
                new ProductStone { ProductId = 11, StoneId = 11 },
                new ProductStone { ProductId = 12, StoneId = 6 },
                new ProductStone { ProductId = 13, StoneId = 10 },
                new ProductStone { ProductId = 14, StoneId = 4 }, 
                new ProductStone { ProductId = 14, StoneId = 7 },
                new ProductStone { ProductId = 16, StoneId = 8 },
                new ProductStone { ProductId = 17, StoneId = 8 },
                new ProductStone { ProductId = 18, StoneId = 6 },
                new ProductStone { ProductId = 19, StoneId = 1 }
            );
        }
    }
}

