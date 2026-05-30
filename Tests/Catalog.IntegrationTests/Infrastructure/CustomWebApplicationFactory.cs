using Catalog.DAL.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Catalog.IntegrationTests.Infrastructure
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        private readonly SqliteConnection _connection = new("DataSource=:memory:");

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");

            builder.ConfigureServices(services =>
            {
                // видалення реальної реєстрації PostgreSQL DbContext
                var descriptor = services.SingleOrDefault( d => d.ServiceType == typeof(DbContextOptions<CatalogDbContext>));
                if (descriptor != null)
                    services.Remove(descriptor);

                _connection.Open();

                // реєстрування SQLite in-memory DbContext
                services.AddDbContext<CatalogDbContext>(options =>
                    options.UseSqlite(_connection) .EnableSensitiveDataLogging() .LogTo(_ => { }));

                // ініціалізація схеми БД
                var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
                db.Database.EnsureCreated();
            });

            // стандартний логер замість Serilog
            builder.ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddConsole();
                logging.SetMinimumLevel(LogLevel.Warning);
            });
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                _connection.Dispose();
            }
        }
    }
}