namespace DotNetCraft.DevTools.Repositories.Abstraction.Interfaces
{
    public interface IRepositoryFactory
    {
        TRepository CreateRepository<TRepository>();
    }
}
