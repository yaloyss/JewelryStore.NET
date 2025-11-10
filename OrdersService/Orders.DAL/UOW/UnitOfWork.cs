using Orders.DAL.Repositories;
using Orders.DAL.Repositories.Interfaces;
using Npgsql;

namespace Orders.DAL.UOW
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly string _connectionString;
        private NpgsqlConnection? _connection;
        private NpgsqlTransaction? _transaction;

        public ICustomerRepository Customers { get; private set; }
        public IOrderRepository Orders { get; private set; }
        public IOrderItemRepository OrderItems { get; private set; }
        public IProductRepository Products { get; private set; }

        public UnitOfWork(string connectionString)
        {
            _connectionString = connectionString;
            Customers = null!;
            Orders = null!;
            OrderItems = null!;
            Products = null!;
        }

        public async Task BeginTransactionAsync()
        {
            try
            {
                _connection = new NpgsqlConnection(_connectionString);
                await _connection.OpenAsync();
                _transaction = await _connection.BeginTransactionAsync();

                Customers = new CustomerRepository(_connection, _transaction);
                Orders = new OrderRepository(_connection, _transaction);
                OrderItems = new OrderItemRepository(_connection, _transaction);
                Products = new ProductRepository(_connection, _transaction);
            }
            catch (NpgsqlException ex)
            {
                throw new Exception("Database transaction initialization failed.", ex);
            }
        }

        public async Task CommitAsync()
        {
            try
            {
                if (_transaction != null)
                {
                    await _transaction.CommitAsync();
                    await _connection!.CloseAsync();
                }
            }
            catch (NpgsqlException ex)
            {
                throw new Exception("Failed to commit database transaction.", ex);
            }
        }

        public async Task RollbackAsync()
        {
            try
            {
                if (_transaction != null)
                {
                    await _transaction.RollbackAsync();
                    await _connection!.CloseAsync();
                }
            }
            catch (NpgsqlException ex)
            {
                throw new Exception("Failed to rollback database transaction.", ex);
            }
        }

        public void Dispose()
        {
            try
            {
                _transaction?.Dispose();
                _connection?.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while disposing resources: {ex.Message}");
            }
        }
    }
}
