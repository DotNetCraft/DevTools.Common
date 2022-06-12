using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using DotNetCraft.DevTools.Repositories.Abstraction;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DotNetCraft.DevTools.Repositories.Sql
{
    public class GenericRepository<TEntity, TId> : BaseRepository<TEntity, TId>
        where TEntity : class
    {
        private static readonly ParameterExpression _parameterExpression;
        private static readonly MemberExpression _property;

        private readonly DbContext _dbContext;
        private readonly DbSet<TEntity> _dbSet;

        static GenericRepository()
        {
            var propertyInfo = typeof(TEntity).GetProperty("Id", BindingFlags.Public | BindingFlags.Instance);
            _parameterExpression = Expression.Parameter(typeof(TEntity), "entity");
            _property = Expression.Property(_parameterExpression, propertyInfo.Name);
        }

        public GenericRepository(DbContext dbContext, ILogger<BaseRepository<TEntity, TId>> logger) : base(logger)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _dbSet = _dbContext.Set<TEntity>();
        }

        protected override async Task<TEntity> OnGetAsync(TId entityId, CancellationToken cancellationToken)
        {
            var constant = Expression.Constant(entityId);
            var expression = Expression.Equal(_property, constant);
            var lambda = Expression.Lambda<Func<TEntity, bool>>(expression, _parameterExpression);

            var result = await _dbSet.FirstOrDefaultAsync(lambda, cancellationToken);
            return result;
        }

        protected override async Task<IEnumerable<TEntity>> OnGetBySpecificationAsync(IRepositorySpecification<TEntity> request, CancellationToken cancellationToken)
        {
            var result = await _dbSet.Where(request.IsSatisfy()).ToListAsync(cancellationToken: cancellationToken);
            return result;
        }

        protected override async Task<TEntity> OnInsertAsync(TEntity entity, CancellationToken cancellationToken)
        {
            var result = await _dbSet.AddAsync(entity, cancellationToken);
            return result.Entity;
        }

        protected override async Task<TEntity> OnUpdateAsync(TEntity entity, CancellationToken cancellationToken)
        {
            if (_dbSet.Local.Count > 0)
            {
                var constant = Expression.Constant((long)5);
                var expression = Expression.Equal(_property, constant);
                var lambda = Expression.Lambda<Func<TEntity, bool>>(expression, _parameterExpression);
                
                var local = _dbSet
                    .Local
                    .FirstOrDefault(lambda.Compile());
                
                if (local != null)
                    _dbContext.Entry(local).State = EntityState.Detached;
            }

            _dbContext.Entry(entity).State = EntityState.Modified;

            return entity;
        }

        protected override async Task<bool> OnDeleteAsync(TEntity entity, CancellationToken cancellationToken)
        {
            _dbSet.Remove(entity);
            return true;
        }

        protected override async Task<bool> OnDeleteAsync(TId entityId, CancellationToken cancellationToken)
        {
            //TODO: Will do a call to SQL because entity will not be in the memory
            var entity = await _dbSet.FindAsync(new object[] { entityId }, cancellationToken);

            if (entity == null)
                return false;

            _dbSet.Remove(entity);
            return true;
        }

        protected override async Task<int> OnSaveChanges(CancellationToken cancellationToken)
        {
            var result = await _dbContext.SaveChangesAsync(cancellationToken);
            return result;
        }
    }
}
