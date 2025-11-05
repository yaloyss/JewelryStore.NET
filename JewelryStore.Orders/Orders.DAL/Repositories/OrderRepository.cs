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

        public async Task<int> CreateAsync(Order order)
        {
            string sql = @"INSERT INTO orders (customerid, status, orderdate)
                           VALUES (@CustomerId, @Status, @OrderDate)
                           RETURNING orderid;";

            return await _connection.ExecuteScalarAsync<int>(sql, order, _transaction);
        }

        public async Task<bool> UpdateAsync(Order order)
        {
            string sql = @"UPDATE orders 
                           SET customerid = @CustomerId,
                               status = @Status,
                               orderdate = @OrderDate
                           WHERE orderid = @OrderId;";

            int affected = await _connection.ExecuteAsync(sql, order, _transaction);
            return affected > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            string sql = "DELETE FROM orders WHERE orderid = @Id;";
            int affected = await _connection.ExecuteAsync(sql, new { Id = id }, _transaction);
            return affected > 0;
        }

        public async Task<Order?> GetByIdAsync(int id)
        {
            string sql = "SELECT orderid, customerid, status, orderdate FROM orders WHERE orderid = @Id;";
            return await _connection.QuerySingleOrDefaultAsync<Order>(sql, new { Id = id }, _transaction);
        }

        public async Task<IEnumerable<Order>> GetAllAsync()
        {
            string sql = "SELECT orderid, customerid, status, orderdate FROM orders ORDER BY orderdate DESC;";
            return await _connection.QueryAsync<Order>(sql, transaction: _transaction);
        }

        public async Task<IEnumerable<Order>> GetByCustomerIdAsync(int customerId)
        {
            string sql = @"SELECT 
                    o.orderid, o.customerid, o.status, o.orderdate, c.customerid, c.firstname, c.lastname, c.email, c.phonenumber
                FROM orders o
                INNER JOIN customers c ON o.customerid = c.customerid
                WHERE o.customerid = @CustomerId 
                ORDER BY o.orderdate DESC;";

            var orders = await _connection.QueryAsync<Order, Customer, Order>( sql,
                (order, customer) =>
                { order.Customer = customer;
                    return order; },
                new { CustomerId = customerId }, _transaction, splitOn: "customerid"
            );
            return orders;
        }

        public async Task<IEnumerable<Order>> GetByStatusAsync(string status)
        {
            string sql = @"SELECT 
                    o.orderid, o.customerid, o.status, o.orderdate, c.customerid, c.firstname, c.lastname, c.email, c.phonenumber
                FROM orders o
                INNER JOIN customers c ON o.customerid = c.customerid
                WHERE o.status = @Status 
                ORDER BY o.orderdate DESC;";

            var orders = await _connection.QueryAsync<Order, Customer, Order>(sql,
                (order, customer) =>
                { order.Customer = customer;
                    return order; },
                new { Status = status }, _transaction, splitOn: "customerid"
            );
            return orders;
        }
    }
}