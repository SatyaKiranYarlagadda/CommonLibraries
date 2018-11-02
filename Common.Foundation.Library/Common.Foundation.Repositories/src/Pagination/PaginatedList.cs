using System.Collections.Generic;

namespace Common.Foundation.Repositories.Pagination
{
    public class PaginatedList<T> : IPaginatedList<T>
    {
        public int Skip { get; set; }
        public int Take { get; set; }
        public IEnumerable<T> Items { get; set; }
        public int TotalPages { get; set; }
    }
}
