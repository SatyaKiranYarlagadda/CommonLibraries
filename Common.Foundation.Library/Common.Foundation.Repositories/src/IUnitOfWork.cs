using System;
using Microsoft.EntityFrameworkCore;

namespace Common.Foundation.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<T> GetRepository<T>() where T : class;
        IReadRepository<T> GetReadRepository<T>() where T : class;
        int SaveChanges();
    }

    public interface IUnitOfWork<TContext> : IUnitOfWork where TContext : DbContext
    {
        TContext Context { get; }
    }
}
