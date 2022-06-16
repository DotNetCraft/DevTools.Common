using System;
using System.Collections.Concurrent;
using DotNetCraft.DevTools.Repositories.Abstraction.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DotNetCraft.DevTools.Repositories.Abstraction
{
    public class RepositoryFactory : IRepositoryFactory
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<RepositoryFactory> _logger;
        private readonly ConcurrentDictionary<Type, Type> _mapping;

        public RepositoryFactory(IServiceProvider serviceProvider, ILogger<RepositoryFactory> logger)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapping = new ConcurrentDictionary<Type, Type>();
        }

        public TRepository CreateRepository<TRepository>()
        {
            _logger.LogTrace($"Creating a new {typeof(TRepository)} instance...");

            var type = typeof(TRepository);

            TRepository repository;
            
            if (type.IsInterface)
            {
                var instanceType = _mapping.GetOrAdd(type, t =>
                {
                    var service = _serviceProvider.GetService<TRepository>();
                    return service.GetType();
                });

                repository = (TRepository)ActivatorUtilities.CreateInstance(_serviceProvider, instanceType);
            }
            else
            {
                repository = ActivatorUtilities.CreateInstance<TRepository>(_serviceProvider);
            }


            _logger.LogTrace("Repository was created");
            return repository;
        }
    }
}
