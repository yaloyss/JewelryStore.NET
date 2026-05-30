using Catalog.DAL.Data;
using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Catalog.IntegrationTests.Infrastructure
{
    public static class DatabaseSeeder
    {
        public static void Seed(CatalogDbContext context)
        {
            // чистий стан бд
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            context.ProductStones.ExecuteDelete();
            context.Products.ExecuteDelete();
            context.Categories.ExecuteDelete();
            context.Metals.ExecuteDelete();
            context.Stones.ExecuteDelete();

            // Metals
            var goldYellow  = new Metal { MetalId = 1, Name = "Gold", Color = "Yellow"};
            var goldWhite = new Metal { MetalId = 2, Name = "Gold", Color = "White"};
            var silver = new Metal { MetalId = 3, Name = "Silver", Color = "Silver"};
            var platinum = new Metal { MetalId = 4, Name = "Platinum", Color = "White"};

            context.Metals.AddRange(goldYellow, goldWhite, silver, platinum);

            // Stones 
            var diamond = new Stone { StoneId = 1, Name = "Diamond"};
            var ruby = new Stone { StoneId = 2, Name = "Ruby"};
            var emerald = new Stone { StoneId = 3, Name = "Emerald"};
            var pearl = new Stone { StoneId = 4, Name = "Pearl"};

            context.Stones.AddRange(diamond, ruby, emerald, pearl);

            // Categories 
            var rings = new Category { CategoryId = 1, Name = "Rings"};
            var earrings = new Category { CategoryId = 2, Name = "Earrings"};
            var pendants = new Category { CategoryId = 3, Name = "Pendants"};

            context.Categories.AddRange(rings, earrings, pendants);

            // Products 
            var product1 = new Product
            {
                ProductId = 1,
                Name = "White Gold Diamond Ring",
                Price = 28000,
                Weight = 3.2m,
                Size = 16.5m,
                Manufacturer = "Ukraine",
                CategoryId = 1,
                MetalId = 2
            };

            var product2 = new Product
            {
                ProductId = 2,
                Name = "Silver Ruby Earrings",
                Price = 5500,
                Weight = 2.1m,
                Size = null,
                Manufacturer = "Ukraine",
                CategoryId = 2,
                MetalId = 3
            };

            var product3 = new Product
            {
                ProductId = 3,
                Name = "Emerald and Diamond Pendant",
                Price = 15000,
                Weight = 1.8m,
                Size = 2.5m,
                Manufacturer = "Ukraine",
                CategoryId = 3,
                MetalId = 1
            };

            context.Products.AddRange(product1, product2, product3);

            // ProductStones 
            context.ProductStones.AddRange(
                new ProductStone { ProductId = 1, StoneId = 1 },         
                new ProductStone { ProductId = 2, StoneId = 2 },         
                new ProductStone { ProductId = 3, StoneId = 1 },
                new ProductStone { ProductId = 3, StoneId = 3 }   
            );

            context.SaveChanges();
        }
    }
}