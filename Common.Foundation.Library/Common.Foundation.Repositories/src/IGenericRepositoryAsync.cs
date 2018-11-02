using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Common.Foundation.Repositories.Pagination;
using Microsoft.EntityFrameworkCore.Query;

namespace Common.Foundation.Repositories
{
    public interface IGenericRepositoryAsync<T> where T : class
    {
        /// <summary>
        /// Get entity/ies satisfying the perdicate.
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="orderBy">Orderby</param>
        /// <param name="include">Other conditions to include</param>
        /// <param name="disableTracking">Flag to disable database tracking</param>
        /// <returns></returns>
        Task<T> SingleAsync(Expression<Func<T, bool>> predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null,
            bool disableTracking = true,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<IEnumerable<T>> GetListAsync(Expression<Func<T, bool>> predicate,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null,
            bool disableTracking = true,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<IPaginatedList<T>> GetPaginatedListAsync(Expression<Func<T, bool>> predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null,
            int skip = 0,
            int take = 20,
            bool disableTracking = true,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<T> SearchAsync(params object[] keyValues);

        /// <summary>
        /// Add an entity to the database
        /// </summary>
        /// <param name="entity">The entity</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns></returns>
        Task AddAsync(T entity, CancellationToken cancellationToken = default(CancellationToken));

        Task AddAsync(params T[] entities);

        Task AddAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default(CancellationToken));
        
        /// <summary>
        /// Update the given entity.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        void Update(T entity);

        void Update(params T[] entities);

        void Update(IEnumerable<T> entities);

        /// <summary>
        /// Delete given entity
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task DeleteAsync(T entity, CancellationToken cancellationToken = default(CancellationToken));

        Task DeleteAsync(object id, CancellationToken cancellationToken = default(CancellationToken));

        void Delete(params T[] entities);

        void Delete(IEnumerable<T> entities);
    }
}
