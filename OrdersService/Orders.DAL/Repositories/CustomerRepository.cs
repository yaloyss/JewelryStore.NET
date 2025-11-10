using Orders.Domain.Entities;
using Orders.DAL.Repositories.Interfaces;
using Npgsql;

namespace Orders.DAL.Repositories
{
    public class CustomerRepository : GenericRepository<Customer>, ICustomerRepository
    {
        private readonly NpgsqlTransaction? _transaction;

        public CustomerRepository(NpgsqlConnection connection, NpgsqlTransaction? transaction = null)
            : base(connection, "customers", transaction)
        {
            _transaction = transaction;
        }

        public override async Task<Customer?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            string sql = "SELECT customerid, firstname, lastname, email, phonenumber FROM customers WHERE customerid = @Id;";

            await using var command = new NpgsqlCommand(sql, _connection, _transaction);
            command.Parameters.AddWithValue("@Id", id);

            await using var reader = await command.ExecuteReaderAsync(ct);
            
            if (await reader.ReadAsync(ct))
            {
                return MapCustomerFromReader(reader);
            }

            return null;
        }

        public override async Task<IEnumerable<Customer>> GetAllAsync(CancellationToken ct = default)
        {
            string sql = "SELECT customerid, firstname, lastname, email, phonenumber FROM customers;";

            await using var command = new NpgsqlCommand(sql, _connection, _transaction);
            await using var reader = await command.ExecuteReaderAsync(ct);

            var customers = new List<Customer>();
            while (await reader.ReadAsync(ct))
            {
                customers.Add(MapCustomerFromReader(reader));
            }

            return customers;
        }

        public async Task<IEnumerable<Customer>> GetByNameAsync(string? firstName, string? lastName, CancellationToken ct = default)
        {
            var conditions = new List<string>();
            var command = new NpgsqlCommand { Connection = _connection, Transaction = _transaction };

            if (!string.IsNullOrWhiteSpace(firstName))
            {
                conditions.Add("LOWER(firstname) LIKE LOWER(@FirstName)");
                command.Parameters.AddWithValue("@FirstName", $"%{firstName}%");
            }

            if (!string.IsNullOrWhiteSpace(lastName))
            {
                conditions.Add("LOWER(lastname) LIKE LOWER(@LastName)");
                command.Parameters.AddWithValue("@LastName", $"%{lastName}%");
            }

            if (conditions.Count == 0)
            {
                return new List<Customer>();
            }

            string sql = $"SELECT customerid, firstname, lastname, email, phonenumber FROM customers WHERE {string.Join(" AND ", conditions)};";
            command.CommandText = sql;

            await using (command)
            {
                await using var reader = await command.ExecuteReaderAsync(ct);

                var customers = new List<Customer>();
                while (await reader.ReadAsync(ct))
                {
                    customers.Add(MapCustomerFromReader(reader));
                }

                return customers;
            }
        }

        public override async Task<int> CreateAsync(Customer customer, CancellationToken ct = default)
        {
            string sql = @"INSERT INTO customers (firstname, lastname, email, phonenumber)
                           VALUES (@FirstName, @LastName, @Email, @PhoneNumber)
                           RETURNING customerid;";

            await using var command = new NpgsqlCommand(sql, _connection, _transaction);
            
            command.Parameters.AddWithValue("@FirstName", customer.FirstName);
            command.Parameters.AddWithValue("@LastName", customer.LastName);
            command.Parameters.AddWithValue("@Email", customer.Email);
            command.Parameters.AddWithValue("@PhoneNumber", customer.PhoneNumber ?? (object)DBNull.Value);

            var result = await command.ExecuteScalarAsync(ct);
            return Convert.ToInt32(result);
        }

        public override async Task<bool> UpdateAsync(Customer customer, CancellationToken ct = default)
        {
            string sql = @"UPDATE customers 
                           SET firstname = @FirstName,
                               lastname = @LastName,
                               email = @Email,
                               phonenumber = @PhoneNumber
                           WHERE customerid = @CustomerId;";

            await using var command = new NpgsqlCommand(sql, _connection, _transaction);
            
            command.Parameters.AddWithValue("@CustomerId", customer.CustomerId);
            command.Parameters.AddWithValue("@FirstName", customer.FirstName);
            command.Parameters.AddWithValue("@LastName", customer.LastName);
            command.Parameters.AddWithValue("@Email", customer.Email);
            command.Parameters.AddWithValue("@PhoneNumber", customer.PhoneNumber ?? (object)DBNull.Value);

            int affected = await command.ExecuteNonQueryAsync(ct);
            return affected > 0;
        }

        public override async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            string sql = "DELETE FROM customers WHERE customerid = @Id;";

            await using var command = new NpgsqlCommand(sql, _connection, _transaction);
            command.Parameters.AddWithValue("@Id", id);

            int affected = await command.ExecuteNonQueryAsync(ct);
            return affected > 0;
        }

        private Customer MapCustomerFromReader(NpgsqlDataReader reader)
        {
            return new Customer
            {
                CustomerId = reader.GetInt32(reader.GetOrdinal("customerid")),
                FirstName = reader.GetString(reader.GetOrdinal("firstname")),
                LastName = reader.GetString(reader.GetOrdinal("lastname")),
                Email = reader.GetString(reader.GetOrdinal("email")),
                PhoneNumber = reader.IsDBNull(reader.GetOrdinal("phonenumber")) 
                    ? null : reader.GetString(reader.GetOrdinal("phonenumber"))
            };
        }
    }
}