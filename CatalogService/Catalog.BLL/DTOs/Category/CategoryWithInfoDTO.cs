using System.ComponentModel.DataAnnotations;
using Catalog.BLL.DTOs.Product;

namespace Catalog.BLL.DTOs.Category
{
	public class CategoryWithInfoDTO
	{
        [Required]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Category name is required")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Name must be between 1 and 100 characters")]
        public string Name { get; set; } = null!;

        public int ProductCount { get; set; }
        public List<ProductDTO> Products { get; set; } = new();
    }
}

