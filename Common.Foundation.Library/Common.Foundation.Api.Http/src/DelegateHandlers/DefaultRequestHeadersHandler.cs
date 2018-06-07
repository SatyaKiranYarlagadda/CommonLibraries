using CorrelationId;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Common.Foundation.Api.Http.DelegateHandlers
{
    public class DefaultRequestHeadersHandler : DelegatingHandler
    {
        private readonly ICorrelationContextAccessor _correlationContextAccessor;

        public DefaultRequestHeadersHandler(ICorrelationContextAccessor correlationContextAccessor)
        {
            _correlationContextAccessor = correlationContextAccessor;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            IEnumerable<string> correlationIds = new List<string>();
            var correlationIdHeaderExists = request.Headers.TryGetValues("X-Correlation-ID", out correlationIds);

            if (!correlationIdHeaderExists || !correlationIds.Any())
            {
                request.Headers.Add("X-Correlation-ID", _correlationContextAccessor.CorrelationContext.CorrelationId);
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
