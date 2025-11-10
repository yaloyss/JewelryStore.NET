using System.ComponentModel.DataAnnotations;

namespace Orders.BLL.DTOs
{
	public class OrderStatusUpdateDTO
	{
        [Required(ErrorMessage = "Status is required")]
        [StringLength(50, ErrorMessage = "Status cannot exceed 50 characters")]
        public string Status { get; set; }
    }
}

