using System;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace DotNetCraft.DevTools.Repositories.SQL.Tests
{
    public abstract class BaseDbContextTests<TDbContext> : IDisposable
        where TDbContext : DbContext
    {
        private const string InMemoryConnectionString = "DataSource=:memory:";
        private readonly SqliteConnection _connection;

        protected readonly TDbContext DbContext;

        protected BaseDbContextTests()
        {
            _connection = new SqliteConnection(InMemoryConnectionString);
            _connection.Open();
            var options = new DbContextOptionsBuilder<TDbContext>()
                .EnableSensitiveDataLogging()
                .UseSqlite(_connection)
                .Options;


            DbContext = (TDbContext)Activator.CreateInstance(typeof(TDbContext), new object?[] { options })! ?? throw new InvalidOperationException();
            DbContext.Database.EnsureCreated();
        }

        public void Dispose()
        {
            _connection.Close();
        }
    }
}
