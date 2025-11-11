using Catalog.BLL.DTOs.Category;
using Catalog.BLL.DTOs.Metal;
using Catalog.BLL.DTOs.Stone;

namespace Catalog.BLL.DTOs.Product
{
	public class ProductDetailedInfoDTO
	{
        public int ProductId { get; set; }
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public decimal Weight { get; set; }
        public decimal? Size { get; set; }
        public string? Manufacturer { get; set; }
        public MetalDTO? Metal { get; set; }
        public CategoryDTO Category { get; set; } = null!;
        public List<StoneDTO> Stones { get; set; } = new();
    }
}

