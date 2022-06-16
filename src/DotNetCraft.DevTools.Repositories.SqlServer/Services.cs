using System;
using DotNetCraft.DevTools.Repositories.Abstraction.Interfaces;
using DotNetCraft.DevTools.Repositories.Sql;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DotNetCraft.DevTools.Repositories.SqlServer
{
    public static class Services
    {
        public static IServiceCollection RegisterDbContext<TDbContext>(this IServiceCollection services, string connectionName, IConfiguration configuration)
            where TDbContext: DbContext
        {
            if (string.IsNullOrWhiteSpace(connectionName))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(connectionName));

            services.AddDbContext<TDbContext>(
                options => options
                    .EnableDetailedErrors()
                    .EnableSensitiveDataLogging()
                    .UseSqlServer(configuration.GetConnectionString(connectionName)));

            return services;
        }

        public static IServiceCollection RegisterRepository<TRepositoryInterface, TRepository>(this IServiceCollection services)
            where TRepositoryInterface : class
            where TRepository : class, TRepositoryInterface
        {
            services.AddScoped<TRepositoryInterface, TRepository>();
            return services;
        }

        public static IServiceCollection RegisterGenericRepository<TDbContext, TEntity, TIdentifier>(this IServiceCollection services)
            where TDbContext : DbContext
            where TEntity : class 
        {
            services.AddScoped<IRepository<TEntity, TIdentifier>, GenericRepository<TDbContext, TEntity, TIdentifier>>();
            return services;
        }
    }
}
