using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Common.Foundation.Repositories
{
    public class UnitOfWorkAsync<TContext> : IUnitOfWorkAsync<TContext>, IUnitOfWorkAsync
        where TContext : DbContext, IDisposable
    {
        private Dictionary<Type, object> _repositories;
        private readonly IRepositoryAsyncFactory _repositoryFactory;

        public UnitOfWorkAsync(TContext context, IRepositoryAsyncFactory repositoryFactory)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
            _repositoryFactory = repositoryFactory ?? throw new ArgumentNullException(nameof(repositoryFactory));
        }

        public IGenericRepositoryAsync<TEntity> GetRepositoryAsync<TEntity>() where TEntity : class
        {
            if (_repositories == null)
                _repositories = new Dictionary<Type, object>();

            var type = typeof(TEntity);

            if (_repositories.ContainsKey(type))
                return (IGenericRepositoryAsync<TEntity>)_repositories[type];

            var respository = _repositoryFactory.GetRepositoryAsync<TEntity>();
            _repositories[type] = respository ?? new GenericRepositoryAsync<TEntity>(Context);

            return (IGenericRepositoryAsync<TEntity>)_repositories[type];
        }

        public TContext Context { get; }

        public async Task<int> SaveChangesAsync()
        {
            return await Context.SaveChangesAsync();
        }

        public void Dispose()
        {
            Context?.Dispose();
        }
    }
}
