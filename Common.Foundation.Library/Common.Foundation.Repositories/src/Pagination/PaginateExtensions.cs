using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Common.Foundation.Repositories.Pagination
{
    public static class PaginateExtensions 
    {
        public static IPaginatedList<T> Paginate<T>(this IEnumerable<T> source, int skip, int take)
        {
            if(source == null)
                return new PaginatedList<T>();

            return new PaginatedList<T>
            {
                Skip = skip,
                Take = take,
                Items = source.Skip(skip).Take(take),
                TotalPages = (int) Math.Ceiling(source.Count() / (double) take)
            };
        }

        public static IPaginatedList<T> Paginate<T>(this IQueryable<T> source, int skip, int take)
        {
            if (source == null)
                return new PaginatedList<T>();

            return new PaginatedList<T>
            {
                Skip = skip,
                Take = take,
                Items = source.Skip(skip).Take(take),
                TotalPages = (int)Math.Ceiling(source.Count() / (double)take)
            };
        }

        public static async Task<IPaginatedList<T>> PaginateAsync<T>(this IQueryable<T> source,
            int skip, int take,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (source == null)
                return new PaginatedList<T>();

            return new PaginatedList<T>
            {
                Skip = skip,
                Take = take,
                Items = await source.Skip(skip).Take(take).ToListAsync(cancellationToken),
                TotalPages = (int)Math.Ceiling(source.Count() / (double)take)
            };
        }
    }
}
