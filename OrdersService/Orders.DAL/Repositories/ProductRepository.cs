using Dapper;
using Orders.Domain.Entities;
using Orders.DAL.Repositories.Interfaces;
using Npgsql;

namespace Orders.DAL.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly NpgsqlConnection _connection;
        private readonly NpgsqlTransaction? _transaction;

        public ProductRepository(NpgsqlConnection connection, NpgsqlTransaction? transaction = null)
        {
            _connection = connection;
            _transaction = transaction;
        }

        public async Task<Product?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            string sql = "SELECT productid, name, price FROM products WHERE productid = @Id;";
            var commandDefinition = new CommandDefinition(sql, new { Id = id }, _transaction, cancellationToken: ct);
            return await _connection.QuerySingleOrDefaultAsync<Product>(commandDefinition);
        }
    }
}