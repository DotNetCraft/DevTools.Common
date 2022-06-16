using System;
using System.Threading;
using System.Threading.Tasks;
using DotNetCraft.DevTools.Repositories.Abstraction;
using DotNetCraft.DevTools.Repositories.Abstraction.Exceptions;
using DotNetCraft.DevTools.Repositories.Abstraction.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

namespace DotNetCraft.DevTools.Repositories.Sql
{
    public class GenericUnitOfWork: BaseUnitOfWork
    {
        private readonly IRepositoryFactory _repositoryFactory;
        private readonly DbContext _dbContext;
        private IDbContextTransaction _dbContextTransaction;

        public GenericUnitOfWork(IRepositoryFactory repositoryFactory, DbContext dbContext, ILogger<BaseUnitOfWork> logger) : base(logger)
        {
            _repositoryFactory = repositoryFactory ?? throw new ArgumentNullException(nameof(repositoryFactory));
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        protected override async Task OnBeginTransactionAsync(CancellationToken cancellationToken)
        {
            if (_dbContextTransaction != null)
                throw new UnitOfWorkException("Transaction was already opened.");
            _dbContextTransaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        }

        protected override async Task OnCommitAsync(CancellationToken cancellationToken)
        {
            if (_dbContextTransaction == null)
                throw new UnitOfWorkException("There is no opened transaction to commit.");

            await _dbContext.SaveChangesAsync(cancellationToken);
            await _dbContextTransaction.CommitAsync(cancellationToken);
            await _dbContextTransaction.DisposeAsync();
            _dbContextTransaction = null;
        }

        protected override async Task OnRollbackAsync(CancellationToken cancellationToken)
        {
            if (_dbContextTransaction == null)
                throw new UnitOfWorkException("There is no opened transaction to rollback.");

            await _dbContextTransaction.RollbackAsync(cancellationToken);
            await _dbContextTransaction.DisposeAsync();
            _dbContextTransaction = null;
        }

        protected override void Dispose(bool disposing)
        {
            if (_dbContextTransaction != null)
            {
                _dbContextTransaction.Rollback();
                _dbContextTransaction.Dispose();
                _dbContextTransaction = null;
            }
            
            base.Dispose(disposing);
        }

        public TRepository GetRepository<TRepository>()
        {
            var repository = _repositoryFactory.CreateRepository<TRepository>();
            return repository;
        }
    }
}
