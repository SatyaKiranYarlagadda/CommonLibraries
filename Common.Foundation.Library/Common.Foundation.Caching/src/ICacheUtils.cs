using System;
using System.Threading.Tasks;

namespace Common.Foundation.Caching
{
    public interface ICacheUtils
    {
        string GenerateDefaultKey<T>(T obj);

        void AddToCache<T>(string key, T value, TimeSpan? duration = null);

        T GetFromCache<T>(string key);

        void RemoveFromCache(string key);

        void RefreshCache(string key);

        Task AddToCacheAsync<T>(string key, T value, TimeSpan? duration = null);

        Task<T> GetFromCacheAsync<T>(string key);

        Task RemoveFromCacheAsync(string key);

        Task RefreshCacheAsync(string key);
    }
}
