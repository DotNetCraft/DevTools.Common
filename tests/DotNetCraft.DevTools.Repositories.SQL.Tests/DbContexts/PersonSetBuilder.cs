using System.Threading.Tasks;
using DotNetCraft.DevTools.Repositories.SQL.Tests.Entities;
using Microsoft.EntityFrameworkCore;

namespace DotNetCraft.DevTools.Repositories.SQL.Tests.DbContexts
{
    internal static class PersonSetBuilder
    {
        public static async Task Build(DbContext dbContext, int persons)
        {
            var myEntitySet = dbContext.Set<Person>();
            for (int i = 1; i < 10; i++)
            {
                myEntitySet.Add(new Person { Id = i, Name = $"test {i}" });
            }

            await dbContext.SaveChangesAsync();
        }
    }
}
