using System.ComponentModel.DataAnnotations;
using Catalog.BLL.DTOs.Category;
using Catalog.BLL.DTOs.Metal;
using Catalog.BLL.DTOs.Stone;

namespace Catalog.BLL.DTOs.Product
{
	public class ProductDetailedInfoDTO
	{
        [Required]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Product name is required")]
        [StringLength(200, MinimumLength = 1, ErrorMessage = "Name must be between 1 and 200 characters")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Weight is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Weight must be greater than 0")]
        public decimal Weight { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Size must be greater than 0")]
        public decimal? Size { get; set; }

        [StringLength(100, ErrorMessage = "Manufacturer name cannot exceed 100 characters")]
        public string? Manufacturer { get; set; }

        public MetalDTO? Metal { get; set; }
        public CategoryDTO Category { get; set; } = null!;
        public List<StoneDTO> Stones { get; set; } = new();
    }
}

