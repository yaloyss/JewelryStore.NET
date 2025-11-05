using System.ComponentModel.DataAnnotations;

namespace JewelryStore.OrdersService.Orders.BLL.DTOs
{
	public class OrderCreateDTO
	{
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "Status is required")]
        [StringLength(50, ErrorMessage = "Status cannot exceed 50 characters")]
        public string Status { get; set; } = "Pending";

        [Required(ErrorMessage = "Order must contain at least one item")]
        [MinLength(1, ErrorMessage = "Order must contain at least one item")]
        public List<OrderItemCreateDTO> Items { get; set; } = new();
    }
}

