using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Common.Foundation.Repositories
{
    public interface IUnitOfWorkAsync : IDisposable
    {
        IGenericRepositoryAsync<TEntity> GetRepositoryAsync<TEntity>() where TEntity : class;

        Task<int> SaveChangesAsync();
    }

    public interface IUnitOfWorkAsync<TContext> : IUnitOfWorkAsync where TContext : DbContext
    {
        TContext Context { get; }
    }
}
