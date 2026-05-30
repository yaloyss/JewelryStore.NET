using Catalog.Domain.Entities;

namespace Catalog.DAL.Tests.Helpers
{
    public static class TestDataBuilder
    {
        // category
        public static Category CreateCategory(int id = 1, string name = "Rings") =>
            new Category { CategoryId = id, Name = name };

        public static List<Category> CreateCategories(int count = 3) =>
            Enumerable.Range(1, count)
                .Select(i => CreateCategory(id: i, name: $"Category {i}")) .ToList();

        // metal
        public static Metal CreateMetal(int id = 1, string name = "Gold", string color = "Yellow") =>
            new Metal { MetalId = id, Name = name, Color = color };

        // stone
        public static Stone CreateStone(int id = 1, string name = "Diamond") =>
            new Stone { StoneId = id, Name = name };

        // product
        public static Product CreateProduct(
            int id = 1,
            string name = "Test Ring",
            decimal price = 5000,
            decimal weight = 3.5m,
            int categoryId = 1,
            int? metalId = 1,
            decimal? size = 17.0m,
            string manufacturer = "Ukraine") =>
            new Product
            {
                ProductId = id,
                Name = name,
                Price = price,
                Weight = weight,
                CategoryId = categoryId,
                MetalId = metalId,
                Size = size,
                Manufacturer = manufacturer
            };

        public static List<Product> CreateProducts(int count = 3, int categoryId = 1, int? metalId = 1) =>
            Enumerable.Range(1, count)
                .Select(i => CreateProduct(
                    id: i,
                    name: $"Product {i}",
                    price: 1000 * i,
                    categoryId: categoryId,
                    metalId: metalId))
                .ToList();

        // product stone
        public static ProductStone CreateProductStone(int productId, int stoneId) =>
            new ProductStone { ProductId = productId, StoneId = stoneId };
    }
}