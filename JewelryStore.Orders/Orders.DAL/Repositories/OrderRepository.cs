using Dapper;
using JewelryStore.OrdersService.Orders.Domain.Entities;
using JewelryStore.OrdersService.Orders.DAL.Repositories.Interfaces;
using Npgsql;

namespace JewelryStore.OrdersService.Orders.DAL.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly NpgsqlConnection _connection;
        private readonly NpgsqlTransaction? _transaction;

        public OrderRepository(NpgsqlConnection connection, NpgsqlTransaction? transaction = null)
        {
            _connection = connection;
            _transaction = transaction;
        }

        public async Task<int> CreateAsync(Order order, CancellationToken ct = default)
        {
            string sql = @"INSERT INTO orders (customerid, status, orderdate)
                           VALUES (@CustomerId, @Status, @OrderDate)
                           RETURNING orderid;";

            var commandDefinition = new CommandDefinition(sql, order, _transaction, cancellationToken: ct);
            return await _connection.ExecuteScalarAsync<int>(commandDefinition);
        }

        public async Task<bool> UpdateAsync(Order order, CancellationToken ct = default)
        {
            string sql = @"UPDATE orders 
                           SET customerid = @CustomerId,
                               status = @Status,
                               orderdate = @OrderDate
                           WHERE orderid = @OrderId;";

            var commandDefinition = new CommandDefinition(sql, order, _transaction, cancellationToken: ct);
            int affected = await _connection.ExecuteAsync(commandDefinition);
            return affected > 0;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            string sql = "DELETE FROM orders WHERE orderid = @Id;";
             var commandDefinition = new CommandDefinition(sql, new { Id = id }, _transaction, cancellationToken: ct);
            int affected = await _connection.ExecuteAsync(commandDefinition);
            return affected > 0;
        }

        public async Task<Order?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            string sql = "SELECT orderid, customerid, status, orderdate FROM orders WHERE orderid = @Id;";
            var commandDefinition = new CommandDefinition(sql, new { Id = id }, _transaction, cancellationToken: ct);
            return await _connection.QuerySingleOrDefaultAsync<Order>(commandDefinition);
        }

        public async Task<IEnumerable<Order>> GetAllAsync(CancellationToken ct = default)
        {
            string sql = "SELECT orderid, customerid, status, orderdate FROM orders ORDER BY orderdate DESC;";
            var commandDefinition = new CommandDefinition(sql, transaction: _transaction, cancellationToken: ct);
            return await _connection.QueryAsync<Order>(commandDefinition);
        }

        public async Task<IEnumerable<Order>> GetByCustomerIdAsync(int customerId, CancellationToken ct = default)
        {
            string sql = @"SELECT 
                    o.orderid, o.customerid, o.status, o.orderdate, c.customerid, c.firstname, c.lastname, c.email, c.phonenumber
                FROM orders o
                INNER JOIN customers c ON o.customerid = c.customerid
                WHERE o.customerid = @CustomerId 
                ORDER BY o.orderdate DESC;";

            var commandDefinition = new CommandDefinition(sql, new { CustomerId = customerId }, _transaction, cancellationToken: ct);
            var orders = await _connection.QueryAsync<Order, Customer, Order>(commandDefinition,
                (order, customer) => {order.Customer = customer;
                    return order;
                }, splitOn: "customerid");
            return orders;
        }

        public async Task<IEnumerable<Order>> GetByStatusAsync(string status, CancellationToken ct = default)
        {
            string sql = @"SELECT 
                    o.orderid, o.customerid, o.status, o.orderdate, c.customerid, c.firstname, c.lastname, c.email, c.phonenumber
                FROM orders o
                INNER JOIN customers c ON o.customerid = c.customerid
                WHERE o.status = @Status 
                ORDER BY o.orderdate DESC;";

            var commandDefinition = new CommandDefinition(sql, new { Status = status }, _transaction, cancellationToken: ct);
            var orders = await _connection.QueryAsync<Order, Customer, Order>(commandDefinition,
                (order, customer) => {order.Customer = customer;
                    return order;
                }, splitOn: "customerid");
            return orders;
        }
    }
}