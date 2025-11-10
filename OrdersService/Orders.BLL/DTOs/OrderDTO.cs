namespace Orders.BLL.DTOs
{
	public class OrderDTO
	{
        public int OrderId { get; set; }

        public int CustomerId { get; set; }

        public DateTime OrderDate { get; set; }

        public string Status { get; set; } = string.Empty;

        public List<OrderItemDTO> Items { get; set; } = new();

        public CustomerDTO Customer { get; set; }

        public decimal TotalAmount => Items.Sum(i => i.TotalPrice);
    }
}

