namespace Catalog.DAL.Tests.Helpers;

using Catalog.DAL.Data;
using Microsoft.EntityFrameworkCore;

public static class DbContextFactory
{
    public static CatalogDbContext Create()
    {
        var options = new DbContextOptionsBuilder<CatalogDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var context = new CatalogDbContext(options);
        return context;
    }
}