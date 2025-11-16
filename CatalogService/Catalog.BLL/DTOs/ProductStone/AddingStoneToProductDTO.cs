using System.ComponentModel.DataAnnotations;

namespace Catalog.BLL.DTOs.ProductStone
{
	public class AddingStoneToProductDTO
	{
        [Required]
        public int ProductId { get; set; }
        [Required]
        public int StoneId { get; set; }
    }
}

