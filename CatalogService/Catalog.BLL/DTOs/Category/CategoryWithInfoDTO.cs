using Catalog.BLL.DTOs.Product;

namespace Catalog.BLL.DTOs.Category
{
	public class CategoryWithInfoDTO
	{
        public int CategoryId { get; set; }
        public string Name { get; set; } = null!;
        public int ProductCount { get; set; }
        public List<ProductDTO> Products { get; set; } = new();
    }
}

