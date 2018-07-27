using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Common.Foundation.Interceptors
{
    public class CachingInterceptor : IInterceptor
    {
        private readonly ILogger<CachingInterceptor> _logger;
        private readonly IDistributedCache _cache;

        public CachingInterceptor(ILogger<CachingInterceptor> logger, IDistributedCache cache)
        {
            _logger = logger;
            _cache = cache;
        }

        public void Intercept(IInvocation invocation)
        {
            if (InterceptorUtils.IsAsyncMethod(invocation.Method))
            {
                InvokeAsync(invocation);
            }
            else
            {
                InvokeSync(invocation);
            }

        }

        private void InvokeSync(IInvocation invocation)
        {
            if (invocation.Method.ReturnType == typeof(void))
            {
                invocation.Proceed();
                return;
            }

            var key = GenerateKey(invocation);
            try
            {
                var result = _cache.Get(key);

                if (result != null)
                {
                    _logger.LogInformation($"Returned from cache. Method:{invocation.Method.Name}");
                    invocation.ReturnValue = FromByteArray<object>(result);
                    return;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Unable to retrieve object value from cache. Key: {key} Exception: {ex}");
            }
            
            invocation.Proceed();

            if (!(invocation.InvocationTarget is ICacheable cachedObject))
            {
                throw new InvalidOperationException($"The object {invocation.InvocationTarget.GetType()} has not implemented IMustBeCached interface.");
            }

            try
            {
                _cache.Set(key, ToByteArray(invocation.ReturnValue), new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = cachedObject.GetCacheTimeSpan()
                });
                _logger.LogInformation($"Added the response to cache. Method: {invocation.Method.Name}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Unable to insert object value in cache. Key: {key} Exception: {ex}");
            }
        }

        private void InvokeAsync(IInvocation invocation)
        {
            if (invocation.Method.ReturnType == typeof(void))
            {
                invocation.Proceed();
                return;
            }

            var key = GenerateKey(invocation);
            try
            {
                var result = _cache.Get(key);

                if (result != null)
                {
                    _logger.LogInformation($"Returned from cache. Method:{invocation.Method.Name}");
                    invocation.ReturnValue = Task.FromResult(FromByteArray<object>(result));
                    return;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Unable to retrieve object value from cache. Key: {key} Exception: {ex}");
            }

            invocation.Proceed();

            ((Task)invocation.ReturnValue)
                .ContinueWith(task =>
                {
                    if (!(invocation.InvocationTarget is ICacheable cachedObject))
                    {
                        throw new InvalidOperationException($"The object {invocation.InvocationTarget.GetType()} has not implemented IMustBeCached interface.");
                    }

                    try
                    {
                        dynamic returnValue = invocation.ReturnValue;
                        _cache.Set(key, ToByteArray(returnValue.Result), new DistributedCacheEntryOptions
                        {
                            AbsoluteExpirationRelativeToNow = cachedObject.GetCacheTimeSpan()
                        });

                        _logger.LogInformation($"Added the response to cache. Method: {invocation.Method.Name}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Unable to insert object value in cache. Key: {key} Exception: {ex}");
                    }
                });
        }

        private static byte[] ToByteArray<T>(T obj)
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

        private static T FromByteArray<T>(byte[] data)
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

        private static string GenerateKey(IInvocation invocation)
        {
            return
                $"{invocation.Method.Name}-{string.Join(";", invocation.Arguments.Select(x => JsonConvert.SerializeObject(x).Replace("\"", "")))}";
        }
    }
}
