using System;
using System.Threading;
using System.Threading.Tasks;

namespace DotNetCraft.DevTools.Repositories.Abstraction
{
    public interface IUnitOfWork: IDisposable
    {
        Task BeginTransactionAsync(CancellationToken cancellationToken = default);
        Task CommitAsync(CancellationToken cancellationToken = default);
        Task RollbackAsync(CancellationToken cancellationToken = default);
    }
}
