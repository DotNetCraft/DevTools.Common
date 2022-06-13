namespace DotNetCraft.DevTools.Repositories.Abstraction
{
    public interface IRepositoryFactory
    {
        IRepository<TEntity, TIdentifier> GetRepository<TEntity, TIdentifier>() 
            where TEntity : class;
    }
}
