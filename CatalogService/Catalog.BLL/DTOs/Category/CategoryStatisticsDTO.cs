using System.ComponentModel.DataAnnotations;

namespace Catalog.BLL.DTOs.Category
{
	public class CategoryStatisticsDTO
	{
        [Required]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Category name is required")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Name must be between 1 and 100 characters")]
        public string Name { get; set; } = null!;

        public int TotalProducts { get; set; }
        public int GoldenProducts { get; set; }
        public int SilverProducts { get; set; }
    }
}

