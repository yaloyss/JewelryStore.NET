using System;
namespace JewelryStore.OrdersService.Orders.Domain.Exceptions
{
    public class BusinessConflictException : Exception
    {
        public BusinessConflictException(string message) : base(message) { }
    }
}

