using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Common.Foundation.Caching
{
    public class CacheUtils : ICacheUtils
    {
        private readonly IDistributedCache _cache;
        private readonly ILogger<CacheUtils> _logger;

        private const int DefaultCacheDurationInSeconds = 300;

        public CacheUtils(IDistributedCache cache, ILogger<CacheUtils> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        public static byte[] ToByteArray<T>(T obj)
        {
            if (obj == null)
                return null;
            var bf = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        public static T FromByteArray<T>(byte[] data)
        {
            if (data == null)
                return default(T);

            var bf = new BinaryFormatter();
            using (var ms = new MemoryStream(data))
            {
                var obj = bf.Deserialize(ms);
                return (T)obj;
            }
        }

        public string GenerateDefaultKey<T>(T obj)
        {
            return obj == null ? null : $"{obj.GetType().Name}-{JsonConvert.SerializeObject(obj).Replace("\"", "")}";
        }

        public void AddToCache<T>(string key, T value, TimeSpan? duration = null)
        {
            try
            {
                if (value == null)
                    return;

                if (!duration.HasValue)
                {
                    _logger.LogInformation($"Cache duration not provided, Defaulting to duration: {DefaultCacheDurationInSeconds} seconds.");
                    duration = TimeSpan.FromSeconds(DefaultCacheDurationInSeconds);
                }

                _cache.Set(key, ToByteArray(value), new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = duration
                });

                _logger.LogDebug($"Added obj value to cache. Key: {key}");
            }
            catch (Exception ex)
            {
               _logger.LogError($"Failed to add value to cache. Key: {key} Exception:{ex}");
            }
        }

        public T GetFromCache<T>(string key)
        {
            try
            {
                if (key == null)
                {
                    _logger.LogInformation($"Provided key is null.");
                    return default(T);
                }
                    
                var value = _cache.Get(key);
                var result = FromByteArray<T>(value);

                _logger.LogDebug($"Retrieved obj value from cache. Key: {key} Value:{result}");

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to retrieve value from cache. Key: {key} Exception:{ex}");
            }
            return default(T);
        }

        public void RemoveFromCache(string key)
        {
            try
            {
                if (key == null)
                {
                    _logger.LogInformation($"Provided key is null.");
                    return;
                }

                _cache.Remove(key);
                _logger.LogDebug($"Removed key from cache. Key: {key}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to remove key from cache. Key: {key} Exception:{ex}");
            }
        }

        public void RefreshCache(string key)
        {
            try
            {
                if (key == null)
                {
                    _logger.LogInformation($"Provided key is null.");
                    return;
                }

                _cache.Refresh(key);
                _logger.LogDebug($"Refreshed key in cache. Key: {key}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to refresh key in cache. Key: {key} Exception:{ex}");
            }
        }

        public async Task AddToCacheAsync<T>(string key, T value, TimeSpan? duration = null)
        {
            try
            {
                if (value == null)
                    return;

                if (!duration.HasValue)
                {
                    _logger.LogInformation($"Cache duration not provided, Defaulting to duration: {DefaultCacheDurationInSeconds} seconds.");
                    duration = TimeSpan.FromSeconds(DefaultCacheDurationInSeconds);
                }

                await _cache.SetAsync(key, ToByteArray(value), new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = duration
                });

                _logger.LogDebug($"Added obj value to cache. Key: {key}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to add value to cache. Key: {key} Exception:{ex}");
            }
        }

        public async Task<T> GetFromCacheAsync<T>(string key)
        {
            try
            {
                if (key == null)
                {
                    _logger.LogInformation($"Provided key is null.");
                    return default(T);
                }

                var value = await _cache.GetAsync(key);
                var result = FromByteArray<T>(value);

                _logger.LogDebug($"Retrieved obj value from cache. Key: {key} Value:{result}");

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to retrieve value from cache. Key: {key} Exception:{ex}");
            }
            return default(T);
        }

        public async Task RemoveFromCacheAsync(string key)
        {
            try
            {
                if (key == null)
                {
                    _logger.LogInformation($"Provided key is null.");
                    return;
                }

                await _cache.RemoveAsync(key);
                _logger.LogDebug($"Removed key from cache. Key: {key}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to remove key from cache. Key: {key} Exception:{ex}");
            }
        }

        public async Task RefreshCacheAsync(string key)
        {
            try
            {
                if (key == null)
                {
                    _logger.LogInformation($"Provided key is null.");
                    return;
                }

                await _cache.RefreshAsync(key);
                _logger.LogDebug($"Refreshed key in cache. Key: {key}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to refresh key in cache. Key: {key} Exception:{ex}");
            }
        }
    }
}
