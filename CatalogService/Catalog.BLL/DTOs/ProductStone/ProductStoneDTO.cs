using Catalog.BLL.DTOs.Stone;
using Catalog.BLL.DTOs.Product;

namespace Catalog.BLL.DTOs.ProductStone
{
	public class ProductStoneDTO
	{
        public int ProductId { get; set; }
        public int StoneId { get; set; }
        public ProductDTO Product { get; set; } = null!;
        public StoneDTO Stone { get; set; } = null!;
    }
}

