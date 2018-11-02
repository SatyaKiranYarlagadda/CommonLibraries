using System.Collections.Generic;

namespace Common.Foundation.Repositories.Pagination
{
    public interface IPaginatedList<T> 
    {
        int Skip { get; }

        int Take { get; }

        IEnumerable<T> Items { get; }

        int TotalPages { get; }
    }
}
