using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Common.Foundation.Repositories.Pagination;
using Microsoft.EntityFrameworkCore.Query;

namespace Common.Foundation.Repositories
{
    public interface IReadRepository<T> where T : class
    {

        IQueryable<T> Query(string sql, params object[] parameters);

        T Search(params object[] keyValues);

        T Single(Expression<Func<T, bool>> predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null,
            bool disableTracking = true);

        IEnumerable<T> GetList(Expression<Func<T, bool>> predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null,
            bool disableTracking = true);

        IPaginatedList<T> GetPaginatedList(Expression<Func<T, bool>> predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null,
            int skip = 0,
            int take = 20,
            bool disableTracking = true);
    }
}
