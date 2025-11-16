using System.ComponentModel.DataAnnotations;

namespace Catalog.BLL.DTOs.Stone
{
	public class StoneDTO
	{
        [Required]
        public int StoneId { get; set; }

        [Required(ErrorMessage = "Stone name is required")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Name must be between 1 and 100 characters")]
        public string Name { get; set; } = null!;
    }
}

