using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DotNetCraft.DevTools.Repositories.Abstraction
{
    public interface IRepository<TEntity, in TIdentifier>
        where TEntity : class
    {
        Task<TEntity> GetAsync(TIdentifier entityId, CancellationToken cancellationToken = default);
        Task<IEnumerable<TEntity>> GetBySpecificationAsync(IRepositorySpecification<TEntity> request, CancellationToken cancellationToken = default);
        Task<TEntity> InsertAsync(TEntity entity, CancellationToken cancellationToken = default);
        Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(TIdentifier entityId, CancellationToken cancellationToken = default);

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
