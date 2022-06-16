using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNetCraft.DevTools.Repositories.Abstraction.Interfaces;
using DotNetCraft.DevTools.Repositories.Sql;
using DotNetCraft.DevTools.Repositories.SQL.Tests.DbContexts;
using DotNetCraft.DevTools.Repositories.SQL.Tests.Entities;
using DotNetCraft.DevTools.Repositories.SQL.Tests.Repositories;
using DotNetCraft.DevTools.Repositories.SqlServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DotNetCraft.DevTools.Repositories.SQL.Tests
{
    [TestClass]
    public class ServiceProviderTests : BaseDbContextTests<TestDbContext>
    {
        [TestMethod]
        public void RepositoryRegistrationTest()
        {
            var services = new ServiceCollection();

            services.AddLogging();

            services.RegisterRepository<IPersonRepository, PersonRepository>();
            services.RegisterGenericRepository<TestDbContext, Person, long>();
            services.RegisterRepository<IRepository<Account, long>, GenericRepository<TestDbContext, Account, long>>();

            services.AddDbContext<TestDbContext>(options =>
            {
                options
                    .EnableDetailedErrors()
                    .EnableSensitiveDataLogging()
                    .UseSqlite(_connection);
            });

            var provider = services.BuildServiceProvider();

            var personRepository = provider.GetService<IPersonRepository>();
            Assert.IsNotNull(personRepository);
            var personRepository2 = provider.GetService<IRepository<Person, long>>();
            Assert.IsNotNull(personRepository2);

            var accountRepository = provider.GetService<IRepository<Account, long>>();
            Assert.IsNotNull(accountRepository);
        }
    }
}
