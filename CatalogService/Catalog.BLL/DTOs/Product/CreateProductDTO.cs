namespace Catalog.BLL.DTOs.Product
{
	public class CreateProductDTO
	{
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public decimal Weight { get; set; }
        public decimal? Size { get; set; }
        public string? Manufacturer { get; set; }
        public int? MetalId { get; set; }
        public int CategoryId { get; set; }
        public List<int> StoneIds { get; set; } = new();
    }
}

