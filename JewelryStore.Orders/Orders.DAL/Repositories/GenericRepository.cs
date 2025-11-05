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

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            var sql = $"SELECT * FROM {_tableName}";
            return await _connection.QueryAsync<T>(sql, transaction: _transaction);
        }

        public virtual async Task<T?> GetByIdAsync(int id)
        {
            var sql = $"SELECT * FROM {_tableName} WHERE {_tableName.ToLower()}id = @Id";
            return await _connection.QueryFirstOrDefaultAsync<T>(sql, new { Id = id }, transaction: _transaction);
        }

        public virtual async Task<int> CreateAsync(T entity)
        {
            var props = typeof(T).GetProperties()
                .Where(p => p.Name.ToLower() != $"{_tableName.ToLower()}id")
                .ToArray();

            var columns = string.Join(", ", props.Select(p => p.Name.ToLower()));
            var values = string.Join(", ", props.Select(p => "@" + p.Name));

            var sql = $"INSERT INTO {_tableName} ({columns}) VALUES ({values}) RETURNING {_tableName.ToLower()}id;";
            return await _connection.ExecuteScalarAsync<int>(sql, entity, _transaction);
        }

        public virtual async Task<bool> UpdateAsync(T entity)
        {
            var props = typeof(T).GetProperties()
                .Where(p => p.Name.ToLower() != $"{_tableName.ToLower()}id")
                .ToArray();

            var updates = string.Join(", ", props.Select(p => $"{p.Name.ToLower()} = @{p.Name}"));
            var sql = $"UPDATE {_tableName} SET {updates} WHERE {_tableName.ToLower()}id = @{_tableName}Id";

            var affected = await _connection.ExecuteAsync(sql, entity, _transaction);
            return affected > 0;
        }

        public virtual async Task<bool> DeleteAsync(int id)
        {
            var sql = $"DELETE FROM {_tableName} WHERE {_tableName.ToLower()}id = @Id";
            var affected = await _connection.ExecuteAsync(sql, new { Id = id }, _transaction);
            return affected > 0;
        }
    }
}
