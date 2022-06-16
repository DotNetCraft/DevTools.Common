using System;
using System.Linq.Expressions;

namespace DotNetCraft.DevTools.Repositories.Abstraction
{
    public interface IRepositorySpecification<TEntity>
        where TEntity : class
    {
        Expression<Func<TEntity, bool>> IsSatisfy();
    }
}
