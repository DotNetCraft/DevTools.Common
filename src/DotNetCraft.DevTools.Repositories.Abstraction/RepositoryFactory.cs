using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DotNetCraft.DevTools.Repositories.Abstraction
{
    public class RepositoryFactory : IRepositoryFactory
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<RepositoryFactory> _logger;

        public RepositoryFactory(IServiceProvider serviceProvider, ILogger<RepositoryFactory> logger)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IRepository<TEntity, TIdentifier> GetRepository<TEntity, TIdentifier>()
            where TEntity : class
        {
            _logger.LogTrace($"Creating a new {typeof(IRepository<TEntity, TIdentifier>)} instance...");
            var repository = _serviceProvider.GetService<IRepository<TEntity, TIdentifier>>();
            _logger.LogTrace("Repository was created");
            return repository;
        }
    }
}
