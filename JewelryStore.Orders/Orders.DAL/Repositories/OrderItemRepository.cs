using Dapper;
using JewelryStore.OrdersService.Orders.Domain.Entities;
using JewelryStore.OrdersService.Orders.DAL.Repositories.Interfaces;
using Npgsql;

namespace JewelryStore.OrdersService.Orders.DAL.Repositories
{
    public class OrderItemRepository : IOrderItemRepository
    {
        private readonly NpgsqlConnection _connection;
        private readonly NpgsqlTransaction? _transaction;

        public OrderItemRepository(NpgsqlConnection connection, NpgsqlTransaction? transaction = null)
        {
            _connection = connection;
            _transaction = transaction;
        }

        public async Task<IEnumerable<OrderItem>> GetByOrderIdAsync(int orderId, CancellationToken ct = default)
        {
            const string sql = @" SELECT 
                    oi.orderitemid, oi.orderid, oi.productid, oi.quantity, oi.unitprice, p.productid, p.name, p.price
                FROM orderitems oi
                INNER JOIN products p ON oi.productid = p.productid
                WHERE oi.orderid = @OrderId;";

            var commandDefinition = new CommandDefinition(sql, new { OrderId = orderId }, _transaction, cancellationToken: ct);
            var orderItems = await _connection.QueryAsync<OrderItem, Product, OrderItem>(commandDefinition,
                (orderItem, product) => {orderItem.Product = product;
                    return orderItem;
                }, splitOn: "productid");
            return orderItems;
        }
    }
}