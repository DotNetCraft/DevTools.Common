using System.Threading.Tasks;
using DotNetCraft.DevTools.Repositories.Abstraction;
using DotNetCraft.DevTools.Repositories.Sql;
using DotNetCraft.DevTools.Repositories.SQL.Tests.DbContexts;
using DotNetCraft.DevTools.Repositories.SQL.Tests.Entities;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace DotNetCraft.DevTools.Repositories.SQL.Tests
{
    [TestClass]
    public class GenericUnitOfWorkTests : BaseDbContextTests<TestDbContext>
    {
        [TestMethod]
        public async Task UnitOfWorkTest()
        {
            var logger = new NullLogger<GenericUnitOfWork>();
            var logger2 = new NullLogger<GenericRepository<TestDbContext, Person, long>>();
            var fakeRep = new GenericRepository<TestDbContext, Person, long>(DbContext, logger2);

            var repositoryFactory = Substitute.For<IRepositoryFactory>();
            repositoryFactory.GetRepository<Person, long>().Returns(fakeRep);

            using (var unitOfWork = new GenericUnitOfWork(repositoryFactory, DbContext, logger))
            {
                var repository = unitOfWork.GetRepository<Person, long>();

                var person = new Person
                {
                    Id = 100,
                    Name = "manual"
                };

                await unitOfWork.BeginTransactionAsync();

                await repository.InsertAsync(person);

                await unitOfWork.CommitAsync();
            }

            var rep = new GenericRepository<TestDbContext, Person, long>(DbContext, logger2);
            var res = await rep.GetAsync(100);
            Assert.AreEqual(100, res.Id);
            Assert.AreEqual("manual", res.Name);
        }
    }
}
