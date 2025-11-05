using System;
namespace JewelryStore.OrdersService.Orders.Domain.Exceptions
{
    public class ValidationException : Exception
    {
        public ValidationException(string message) : base(message) { }
    }
}

