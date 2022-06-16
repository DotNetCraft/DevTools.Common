using DotNetCraft.DevTools.Repositories.Abstraction;
using DotNetCraft.DevTools.Repositories.SQL.Tests.DbContexts;
using DotNetCraft.DevTools.Repositories.SQL.Tests.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DotNetCraft.DevTools.Repositories.SQL.Tests
{
    [TestClass]
    public class RepositoryFactoryTests
    {
        [TestMethod]
        public void DifferentRepositoriesCreationTest()
        {
            ILogger<RepositoryFactory> logger = new NullLogger<RepositoryFactory>();

            using (var connection = new SqliteConnection("DataSource=:memory:"))
            {
                connection.Open();

                var serviceCollection = new ServiceCollection();
                serviceCollection.AddLogging();
                serviceCollection.AddDbContext<TestDbContext>(builder => { builder.UseSqlite(connection); });
                serviceCollection.AddScoped<IPersonRepository, PersonRepository>();
                var serviceProvider = serviceCollection.BuildServiceProvider();

                var factory = new RepositoryFactory(serviceProvider, logger);
                var rep1 = factory.CreateRepository<IPersonRepository>();
                var rep2 = factory.CreateRepository<IPersonRepository>();

                Assert.IsNotNull(rep1);
                Assert.IsNotNull(rep2);
                Assert.AreNotEqual(rep1.GetHashCode(), rep2.GetHashCode());

                rep1 = serviceProvider.GetService<IPersonRepository>();
                rep2 = serviceProvider.GetService<IPersonRepository>();
                Assert.IsNotNull(rep1);
                Assert.IsNotNull(rep2);
                Assert.AreEqual(rep1.GetHashCode(), rep2.GetHashCode());

                connection.Close();
            }
        }
    }
}
