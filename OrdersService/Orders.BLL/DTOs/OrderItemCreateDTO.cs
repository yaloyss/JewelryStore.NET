using System.ComponentModel.DataAnnotations;

namespace Orders.BLL.DTOs
{
	public class OrderItemCreateDTO
	{
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Quantity must be greater than zero")]
        [MinLength(1, ErrorMessage = "Quantity must be greater than zero")]
        public int Quantity { get; set; }
    }
}

