using Dapper;
using Npgsql;
using Orders.DAL.Repositories.Interfaces;
using Orders.Domain.Entities;

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

        //ado.net for create, update, delete

        public async Task<int> CreateAsync(Order order, CancellationToken ct = default)
        {
            string sql = @"INSERT INTO orders (customerid, status, orderdate)
                           VALUES (@CustomerId, @Status, @OrderDate)
                           RETURNING orderid;";

            await using var command = new NpgsqlCommand(sql, _connection, _transaction);

            command.Parameters.AddWithValue("@CustomerId", order.CustomerId);
            command.Parameters.AddWithValue("@Status", order.Status);
            command.Parameters.AddWithValue("@OrderDate", order.OrderDate);

            var result = await command.ExecuteScalarAsync(ct); //returns first value (id)
            return Convert.ToInt32(result);
        }

        public async Task<bool> UpdateAsync(Order order, CancellationToken ct = default)
        {
            string sql = @"UPDATE orders 
                           SET customerid = @CustomerId,
                               status = @Status,
                               orderdate = @OrderDate
                           WHERE orderid = @OrderId;";

            await using var command = new NpgsqlCommand(sql, _connection, _transaction);

            command.Parameters.AddWithValue("@OrderId", order.OrderId);
            command.Parameters.AddWithValue("@CustomerId", order.CustomerId);
            command.Parameters.AddWithValue("@Status", order.Status);
            command.Parameters.AddWithValue("@OrderDate", order.OrderDate);

            int affected = await command.ExecuteNonQueryAsync(ct);      //doesn't return anything
            return affected > 0;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            string sql = "DELETE FROM orders WHERE orderid = @Id;";

            await using var command = new NpgsqlCommand(sql, _connection, _transaction);
            command.Parameters.AddWithValue("@Id", id);

            int affected = await command.ExecuteNonQueryAsync(ct); 
            return affected > 0;
        }

        //dapper for read operations

        public async Task<Order?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            string sql = "SELECT orderid, customerid, status, orderdate FROM orders WHERE orderid = @Id;";
            var commandDefinition = new CommandDefinition(sql, new { Id = id }, _transaction, cancellationToken: ct);
            return await _connection.QuerySingleOrDefaultAsync<Order>(commandDefinition);     //gets one row from result
        }

        public async Task<IEnumerable<Order>> GetAllAsync(CancellationToken ct = default)
        {
            string sql = "SELECT orderid, customerid, status, orderdate FROM orders ORDER BY orderdate DESC;";

            var commandDefinition = new CommandDefinition(sql, transaction: _transaction, cancellationToken: ct);
            return await _connection.QueryAsync<Order>(commandDefinition);
        }

        //dapper with multi mapping for complex queries

        public async Task<IEnumerable<Order>> GetByCustomerIdAsync(int customerId, CancellationToken ct = default)
        {
            string sql = @"SELECT 
                o.orderid, o.customerid, o.status, o.orderdate,
                c.customerid, c.firstname, c.lastname, c.email, c.phonenumber
                FROM orders o
                INNER JOIN customers c ON o.customerid = c.customerid
                WHERE o.customerid = @CustomerId 
                ORDER BY o.orderdate DESC;";

            var orders = await _connection.QueryAsync<Order, Customer, Order>(
                new CommandDefinition(sql, new { CustomerId = customerId }, _transaction, cancellationToken: ct),
                (order, customer) =>{order.Customer = customer;
                    return order; },
                splitOn: "customerid");
            return orders;
        }

        public async Task<IEnumerable<Order>> GetByStatusAsync(string status, CancellationToken ct = default)
        {
            string sql = @"SELECT 
                o.orderid, o.customerid, o.status, o.orderdate,
                c.customerid, c.firstname, c.lastname, c.email, c.phonenumber
                FROM orders o
                INNER JOIN customers c ON o.customerid = c.customerid
                WHERE o.status = @Status 
                ORDER BY o.orderdate DESC;";

            var orders = await _connection.QueryAsync<Order, Customer, Order>(
                new CommandDefinition(sql, new { Status = status }, _transaction, cancellationToken: ct),
                (order, customer) =>{order.Customer = customer;
                    return order;},
                splitOn: "customerid");
            return orders;
        }
    }
}