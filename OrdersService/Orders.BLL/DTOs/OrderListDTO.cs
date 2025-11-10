namespace Orders.BLL.DTOs
{
	public class OrderListDTO
	{
        public int OrderId { get; set; }

        public int CustomerId { get; set; }

        public string CustomerName { get; set; }

        public DateTime OrderDate { get; set; }

        public string Status { get; set; }

        public int ItemsCount { get; set; }

        public decimal TotalAmount { get; set; }
    }
}

