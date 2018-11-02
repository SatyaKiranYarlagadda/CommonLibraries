using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Common.Foundation.Api.Behaviors
{
    public class CachingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly ILogger<CachingBehavior<TRequest, TResponse>> _logger;
        private readonly ICacheUtils _cacheUtils;

        public CachingBehavior(ILogger<CachingBehavior<TRequest, TResponse>> logger, ICacheUtils cacheUtils)
        {
            _logger = logger;
            _cacheUtils = cacheUtils;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            if (!(request is ICacheable cachedRequest))
            {
                return await next();
            }

            var key = _cacheUtils.GenerateDefaultKey(request);
            try
            {
                var result = await _cacheUtils.GetFromCacheAsync<TResponse>(key);

                if (result != null)
                {
                    _logger.LogInformation($"Returned from cache. Key:{key}");

                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Unable to retrieve object value from cache. Key: {key} Exception: {ex}");
            }

            var response = await next();

            try
            {
                await _cacheUtils.AddToCacheAsync(key, response, cachedRequest.GetCacheTimeSpan());

                _logger.LogInformation($"Added the response to cache. Key: {key}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Unable to insert object value in cache. Key: {key} Exception: {ex}");
            }

            return response;
        }
    }
}
