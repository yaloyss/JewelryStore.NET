using System;
namespace JewelryStore.OrdersService.Orders.Domain.Entities
{
	public class Order
	{
        public int OrderId { get; set; }

        public int CustomerId { get; set; }

        public DateTime OrderDate { get; set; }

        public string Status { get; set; } = string.Empty;

        public Customer Customer { get; set; }
        public List<OrderItem> Items { get; set; } = new();
    }
}

