using System;
namespace JewelryStore.OrdersService.Orders.Domain.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message) { }
    }
}

