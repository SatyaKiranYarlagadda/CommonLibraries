using Microsoft.EntityFrameworkCore;

namespace Common.Foundation.Repositories
{
    public class ReadRepository<T> : BaseReadRepository<T>, IReadRepository<T> where T : class
    {
        public ReadRepository(DbContext context) : base(context)
        {
        }
    }
}
