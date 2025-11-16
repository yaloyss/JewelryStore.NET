using System.ComponentModel.DataAnnotations;

namespace Catalog.BLL.DTOs.Product
{
	public class ProductPriceRangeDTO
    {
        [Required(ErrorMessage = "Minimum price is required")]
        [Range(0, double.MaxValue, ErrorMessage = "Minimum price cannot be negative")]
        public decimal MinPrice { get; set; }

        [Required(ErrorMessage = "Maximum price is required")]
        [Range(0, double.MaxValue, ErrorMessage = "Maximum price cannot be negative")]
        public decimal MaxPrice { get; set; }
    }
}

