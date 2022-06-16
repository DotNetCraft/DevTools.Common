using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DotNetCraft.DevTools.Repositories.Abstraction.Exceptions;
using DotNetCraft.DevTools.Repositories.Abstraction.Interfaces;
using Microsoft.Extensions.Logging;

namespace DotNetCraft.DevTools.Repositories.Abstraction
{
    public abstract class BaseRepository<TEntity, TIdentifier> : IRepository<TEntity, TIdentifier>
        where TEntity : class
    {
        private readonly ILogger<BaseRepository<TEntity, TIdentifier>> _logger;
        private readonly Type _entityType;

        protected BaseRepository(ILogger<BaseRepository<TEntity, TIdentifier>> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _entityType = typeof(TEntity);
        }

        protected abstract Task<TEntity> OnGetAsync(TIdentifier entityId, CancellationToken cancellationToken);
        protected abstract Task<IEnumerable<TEntity>> OnGetBySpecificationAsync(IRepositorySpecification<TEntity> request, CancellationToken cancellationToken);
        protected abstract Task<TEntity> OnInsertAsync(TEntity entity, CancellationToken cancellationToken);
        protected abstract Task<TEntity> OnUpdateAsync(TEntity entity, CancellationToken cancellationToken);
        protected abstract Task<bool> OnDeleteAsync(TEntity entity, CancellationToken cancellationToken);
        protected abstract Task<bool> OnDeleteAsync(TIdentifier entityId, CancellationToken cancellationToken);

        public async Task<TEntity> GetAsync(TIdentifier entityId, CancellationToken cancellationToken = default)
        {
            TEntity result;
            try
            {
                _logger.LogTrace($"Retrieving {_entityType.Name} by id {entityId}...");
                result = await OnGetAsync(entityId, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to retrieve entity by id {entityId}: {ex.Message}");
                throw;
            }

            if (result == null)
            {
                var msg = $"{_entityType.Name} was not found by {entityId}.";
                _logger.LogTrace(msg);
                throw new EntityNotFoundException(msg);
            }

            _logger.LogTrace("Entity was retrieved.");
            return result;
        }

        public async Task<IEnumerable<TEntity>> GetBySpecificationAsync(IRepositorySpecification<TEntity> request, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogTrace($"Retrieving list of {_entityType.Name} by specification {request}...");
                var result = await OnGetBySpecificationAsync(request, cancellationToken);
                _logger.LogTrace("List of entities was retrieved.");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to retrieve list of entities by specification {request}: {ex.Message}");
                throw;
            }
        }

        public async Task<TEntity> InsertAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogTrace($"Inserting a new {entity}...");
                var result = await OnInsertAsync(entity, cancellationToken);
                _logger.LogTrace("Entity was inserted.");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to insert {entity}: {ex.Message}");
                throw;
            }
        }

        public async Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogTrace($"Updating {entity}...");
                var result = await OnUpdateAsync(entity, cancellationToken);
                _logger.LogTrace("Entity was updated.");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to update {entity}: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogTrace($"Deleting {entity}...");
                var result = await OnDeleteAsync(entity, cancellationToken);
                _logger.LogTrace("Entity was deleted.");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to delete {entity}: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> DeleteAsync(TIdentifier entityId, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogTrace($"Deleting {_entityType.Name} by id {entityId}...");
                var result = await OnDeleteAsync(entityId, cancellationToken);
                _logger.LogTrace("Entity was deleted.");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to delete {_entityType.Name} by id {entityId}: {ex.Message}");
                throw;
            }
        }
    }
}
