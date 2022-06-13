using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace DotNetCraft.DevTools.Repositories.Abstraction
{
    public abstract class BaseUnitOfWork: IUnitOfWork
    {
        private readonly ILogger<BaseUnitOfWork> _logger;

        protected BaseUnitOfWork(ILogger<BaseUnitOfWork> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected abstract Task OnBeginTransactionAsync(CancellationToken cancellationToken);
        protected abstract Task OnCommitAsync(CancellationToken cancellationToken);
        protected abstract Task OnRollbackAsync(CancellationToken cancellationToken);

        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogTrace("Opening a new transaction...");
                await OnBeginTransactionAsync(cancellationToken);
                _logger.LogTrace("Transaction was opened.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to open a new transaction: {ex.Message}");
                throw;
            }
        }

        public async Task CommitAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogTrace("Commiting an existing transaction...");
                await OnCommitAsync(cancellationToken);
                _logger.LogTrace("Transaction was committed.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to commit an existing transaction: {ex.Message}");
                throw;
            }
        }

        public async Task RollbackAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogTrace("Rolling back an existing transaction...");
                await OnRollbackAsync(cancellationToken);
                _logger.LogTrace("Transaction was rolled back.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to roll back an existing transaction: {ex.Message}");
                throw;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~BaseUnitOfWork()
        {
            Dispose(false);
        }
    }
}
