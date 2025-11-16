using System.ComponentModel.DataAnnotations;

namespace Catalog.BLL.DTOs.Product
{
	public class UpdateProductDTO
	{
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

        public int? MetalId { get; set; }

        [Required(ErrorMessage = "Category is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Category ID must be greater than 0")]
        public int CategoryId { get; set; }

        public List<int> StoneIds { get; set; } = new();
    }
}

