using System;

namespace Common.Foundation.Caching
{
    public interface ICacheable
    {
        TimeSpan GetCacheTimeSpan();
    }
}
