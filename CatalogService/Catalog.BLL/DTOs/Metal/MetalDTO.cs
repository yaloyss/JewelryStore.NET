using System.ComponentModel.DataAnnotations;

namespace Catalog.BLL.DTOs.Metal
{
	public class MetalDTO
	{
        [Required]
        public int MetalId { get; set; }

        [Required(ErrorMessage = "Metal name is required")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Name must be between 1 and 100 characters")]
        public string Name { get; set; } = null!;

        [StringLength(50, ErrorMessage = "Color cannot exceed 50 characters")]
        public string? Color { get; set; }
    }
}

