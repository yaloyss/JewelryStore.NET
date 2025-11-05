using System;
using JewelryStore.OrdersService.Orders.DAL.Repositories.Interfaces;

namespace JewelryStore.OrdersService.Orders.DAL.UOW
{
    public interface IUnitOfWork : IDisposable
    {
        ICustomerRepository Customers { get; }
        IOrderRepository Orders { get; }
        IOrderItemRepository OrderItems { get; }
        IProductRepository Products { get; }

        Task BeginTransactionAsync();
        Task CommitAsync();
        Task RollbackAsync();
    }
}

