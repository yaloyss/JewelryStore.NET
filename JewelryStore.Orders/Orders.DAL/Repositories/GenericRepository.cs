using Npgsql;
using Dapper;
using JewelryStore.OrdersService.Orders.DAL.Repositories.Interfaces;

namespace JewelryStore.OrdersService.Orders.DAL.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly NpgsqlConnection _connection;
        protected readonly string _tableName;
        protected readonly NpgsqlTransaction? _transaction;

        public GenericRepository(NpgsqlConnection connection, string tableName, NpgsqlTransaction? transaction = null)
        {
            _connection = connection;
            _tableName = tableName;
            _transaction = transaction;
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync(CancellationToken ct = default)
        {
            var sql = $"SELECT * FROM {_tableName}";
            var commandDefinition = new CommandDefinition(sql, transaction: _transaction, cancellationToken: ct);
            return await _connection.QueryAsync<T>(commandDefinition);
        }

        public virtual async Task<T?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var sql = $"SELECT * FROM {_tableName} WHERE {_tableName.ToLower()}id = @Id";
            var commandDefinition = new CommandDefinition(sql, new { Id = id }, transaction: _transaction, cancellationToken: ct);
            return await _connection.QueryFirstOrDefaultAsync<T>(commandDefinition);
        }

        public virtual async Task<int> CreateAsync(T entity, CancellationToken ct = default)
        {
            var properties = typeof(T).GetProperties()
                .Where(p => p.Name.ToLower() != $"{_tableName.ToLower()}id")
                .ToArray();

            var columns = string.Join(", ", properties.Select(p => p.Name.ToLower()));
            var values = string.Join(", ", properties.Select(p => "@" + p.Name));

            var sql = $"INSERT INTO {_tableName} ({columns}) VALUES ({values}) RETURNING {_tableName.ToLower()}id;";
            var commandDefinition = new CommandDefinition(sql, entity, _transaction, cancellationToken: ct);
            return await _connection.ExecuteScalarAsync<int>(commandDefinition);
        }

        public virtual async Task<bool> UpdateAsync(T entity, CancellationToken ct = default)
        {
            var properties = typeof(T).GetProperties()
                .Where(p => p.Name.ToLower() != $"{_tableName.ToLower()}id")
                .ToArray();

            var updates = string.Join(", ", properties.Select(p => $"{p.Name.ToLower()} = @{p.Name}"));
            var sql = $"UPDATE {_tableName} SET {updates} WHERE {_tableName.ToLower()}id = @{_tableName}Id";

            var commandDefinition = new CommandDefinition(sql, entity, _transaction, cancellationToken: ct);
            var affected = await _connection.ExecuteAsync(commandDefinition);
            return affected > 0;
        }

        public virtual async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            var sql = $"DELETE FROM {_tableName} WHERE {_tableName.ToLower()}id = @Id";
            var commandDefinition = new CommandDefinition(sql, new { Id = id }, _transaction, cancellationToken: ct);
            var affected = await _connection.ExecuteAsync(commandDefinition);
            return affected > 0;
        }
    }
}
