using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DotNetCraft.DevTools.Repositories.Abstraction;
using DotNetCraft.DevTools.Repositories.Sql;
using DotNetCraft.DevTools.Repositories.SQL.Tests.Entities;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DotNetCraft.DevTools.Repositories.SQL.Tests
{

    [TestClass]
    public class GenericRepositoryTests : BaseDbContextTests<TestDbContext>
    {
        [TestInitialize]
        public async Task Init()
        {
            var myEntitySet = DbContext.Set<Person>();
            for (int i = 1; i < 10; i++)
            {
                myEntitySet.Add(new Person { Id = i, Name = $"test {i}" });
            }

            await DbContext.SaveChangesAsync();
        }

        [TestMethod]
        public async Task GetEntityByIdTest()
        {
            var logger = new NullLogger<GenericRepository<Person, long>>();

            var rep = new GenericRepository<Person, long>(DbContext, logger);
            {
                var res = await rep.GetAsync(2);
                Assert.AreEqual(2, res.Id);
            }
        }

        [TestMethod]
        public async Task GetEntitiesTest()
        {
            var logger = new NullLogger<GenericRepository<Person, long>>();

            var rep = new GenericRepository<Person, long>(DbContext, logger);
            {
                var request = new PersonByNameSpecification("test 3");
                var res = await rep.GetBySpecificationAsync(request);
                var singleItem = res.Single();
                Assert.AreEqual(3, singleItem.Id);
                Assert.AreEqual("test 3", singleItem.Name);
            }
        }

        [TestMethod]
        public async Task InsertEntityTest()
        {
            var logger = new NullLogger<GenericRepository<Person, long>>();

            var rep = new GenericRepository<Person, long>(DbContext, logger);
            {
                var person = new Person
                {
                    Id = 100,
                    Name = "manual"
                };
                var res = await rep.InsertAsync(person);
                Assert.AreEqual(100, res.Id);

                await rep.SaveChangesAsync();
            }

            var rep2 = new GenericRepository<Person, long>(DbContext, logger);
            {
                var res = await rep2.GetAsync(100);
                Assert.AreEqual(100, res.Id);
                Assert.AreEqual("manual", res.Name);
            }
        }

        [TestMethod]
        public async Task UpdateEntityTest()
        {
            var logger = new NullLogger<GenericRepository<Person, long>>();

            var person = new Person
            {
                Id = 5,
                Name = "updated"
            };

            var rep = new GenericRepository<Person, long>(DbContext, logger);
            var res = await rep.UpdateAsync(person, CancellationToken.None);
            await rep.SaveChangesAsync();
            Assert.AreEqual(5, res.Id);
            Assert.AreEqual("updated", res.Name);

            rep = new GenericRepository<Person, long>(DbContext, logger);
            res = await rep.GetAsync(5);
            Assert.AreEqual(5, res.Id);
            Assert.AreEqual("updated", res.Name);
        }

        [TestMethod]
        [ExpectedException(typeof(EntityNotFoundException))]
        public async Task DeleteEntityByIdTest()
        {
            var logger = new NullLogger<GenericRepository<Person, long>>();

            var rep = new GenericRepository<Person, long>(DbContext, logger);
            var res = await rep.DeleteAsync(5);
            await rep.SaveChangesAsync();
            Assert.IsTrue(res);

            rep = new GenericRepository<Person, long>(DbContext, logger);
            var res2 = await rep.GetAsync(5);
            Assert.Fail("EntityNotFoundException should be raised");
        }

        [TestMethod]
        [ExpectedException(typeof(EntityNotFoundException))]
        public async Task DeleteEntityTest()
        {
            var logger = new NullLogger<GenericRepository<Person, long>>();

            var rep = new GenericRepository<Person, long>(DbContext, logger);
            var res2 = await rep.GetAsync(5);

            rep = new GenericRepository<Person, long>(DbContext, logger);
            var res = await rep.DeleteAsync(res2);
            await rep.SaveChangesAsync();
            Assert.IsTrue(res);

            rep = new GenericRepository<Person, long>(DbContext, logger);
            res2 = await rep.GetAsync(5);
            Assert.Fail("EntityNotFoundException should be raised");
        }
    }
}