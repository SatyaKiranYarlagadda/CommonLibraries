using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Common.Foundation.Repositories
{
    public class UnitOfWork<TContext> : IUnitOfWork<TContext>, IUnitOfWork
        where TContext : DbContext, IDisposable
    {
        private Dictionary<Type, object> _repositories;
        private readonly IRepositoryFactory _repositoryFactory;

        public UnitOfWork(TContext context, IRepositoryFactory repositoryFactory)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
            _repositoryFactory = repositoryFactory ?? throw new ArgumentNullException(nameof(repositoryFactory));
        }

        public IGenericRepository<TEntity> GetRepository<TEntity>() where TEntity : class
        {
            if (_repositories == null)
                _repositories = new Dictionary<Type, object>();

            var type = typeof(TEntity);

            if (_repositories.ContainsKey(type))
                return (IGenericRepository<TEntity>) _repositories[type];

            var respository = _repositoryFactory.GetRepository<TEntity>();
            _repositories[type] = respository ?? new GenericRepository<TEntity>(Context);

            return (IGenericRepository<TEntity>)_repositories[type];
        }

        public IReadRepository<TEntity> GetReadRepository<TEntity>() where TEntity : class
        {
            if (_repositories == null)
                _repositories = new Dictionary<Type, object>();

            var type = typeof(TEntity);

            if (_repositories.ContainsKey(type))
                return (IReadRepository<TEntity>) _repositories[type];

            var respository = _repositoryFactory.GetReadRepository<TEntity>();
            _repositories[type] = respository ?? new ReadRepository<TEntity>(Context);

            return (IReadRepository<TEntity>)_repositories[type];
        }

        public TContext Context { get; }

        public int SaveChanges()
        {
            return Context.SaveChanges();
        }

        public void Dispose()
        {
            Context?.Dispose();
        }
    }
}
