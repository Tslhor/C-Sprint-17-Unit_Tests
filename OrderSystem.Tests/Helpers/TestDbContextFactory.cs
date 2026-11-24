using Microsoft.EntityFrameworkCore;
using OrderSystem.Data;

namespace OrderSystem.Tests.Helpers;

public static class TestDbContextFactory
{
    public static ProductDbContext Create(string? databaseName = null)
    {
        var options = new DbContextOptionsBuilder<ProductDbContext>()
            .UseInMemoryDatabase(databaseName ?? Guid.NewGuid().ToString())
            .Options;

        return new ProductDbContext(options);
    }
}
