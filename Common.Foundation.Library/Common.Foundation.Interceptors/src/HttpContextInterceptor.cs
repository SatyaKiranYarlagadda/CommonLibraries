using Castle.DynamicProxy;
using CorrelationId;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;


namespace Common.Foundation.Interceptors.src
{
    public class HttpContextInterceptor : IInterceptor
    {
        private readonly ILogger<HttpContextInterceptor> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICorrelationContextAccessor _correlationContextAccessor;

        private const string CorrelationIdHeader = "X-Correlation-ID";

        public HttpContextInterceptor(ILogger<HttpContextInterceptor> logger, IHttpContextAccessor httpContextAccessor, ICorrelationContextAccessor correlationContextAccessor)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _correlationContextAccessor = correlationContextAccessor;
        }

        public void Intercept(IInvocation invocation)
        {
            var requestHeaders = _httpContextAccessor.HttpContext.Request.Headers;
            var correlationId = _correlationContextAccessor.CorrelationContext.CorrelationId;

            if (requestHeaders.ContainsKey(CorrelationIdHeader))
            {
                if (!requestHeaders[CorrelationIdHeader].Equals(correlationId))
                {
                    _httpContextAccessor.HttpContext.Request.Headers[CorrelationIdHeader] = correlationId;
                }

            }
            else
            {
                _httpContextAccessor.HttpContext.Request.Headers.Add(CorrelationIdHeader, correlationId);
            }

            _logger.LogInformation($"Setting correlationId to [{correlationId}] on the HttpContext header.");
            invocation.Proceed();
        }
    }
}
