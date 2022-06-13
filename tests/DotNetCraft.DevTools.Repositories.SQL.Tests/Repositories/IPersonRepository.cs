using DotNetCraft.DevTools.Repositories.Abstraction;
using DotNetCraft.DevTools.Repositories.Sql;
using DotNetCraft.DevTools.Repositories.SQL.Tests.DbContexts;
using DotNetCraft.DevTools.Repositories.SQL.Tests.Entities;
using Microsoft.Extensions.Logging;

namespace DotNetCraft.DevTools.Repositories.SQL.Tests.Repositories
{
    public interface IPersonRepository : IRepository<Person, long>
    {
    }

    public class PersonRepository : GenericRepository<TestDbContext, Person, long>, IPersonRepository
    {
        public PersonRepository(TestDbContext dbContext, ILogger<BaseRepository<Person, long>> logger) : base(dbContext, logger)
        {
        }
    }
}
