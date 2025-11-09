namespace JewelryStore.CatalogService.CatalogDomain.Entities
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

        public Metal metal { get; set; } 
        public Category category { get; set; } 
        public ICollection<ProductStone>? ProductStone { get; set; } = null!;
    }
}

