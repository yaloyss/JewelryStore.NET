using Catalog.BLL.DTOs.Stone;
using Catalog.BLL.DTOs.Product;
using System.ComponentModel.DataAnnotations;

namespace Catalog.BLL.DTOs.ProductStone
{
	public class ProductStoneDTO
	{
        [Required]
        public int ProductId { get; set; }
        [Required]
        public int StoneId { get; set; }

        public ProductDTO Product { get; set; } = null!;
        public StoneDTO Stone { get; set; } = null!;
    }
}

