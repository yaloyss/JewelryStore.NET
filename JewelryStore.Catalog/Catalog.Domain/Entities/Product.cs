namespace JewelryStore.CatalogService.Catalog.Domain.Entities
{
	public class Product
	{
        public int ProductId { get; set; }

        public string Name { get; set; } = null!;

        public decimal Price { get; set; }

        public decimal Weight { get; set; }

        public decimal? Size { get; set; }

        public string? Manufacturer { get; set; }

        public int? MetalId { get; set; }

        public int CategoryId { get; set; }

        public Metal Metal { get; set; } 
        public Category Category { get; set; } 
        public ICollection<ProductStone>? ProductStones { get; set; } = null!;
    }
}

