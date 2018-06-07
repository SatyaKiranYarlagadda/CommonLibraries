using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Common.Foundation.Api.Http.DelegateHandlers
{
    public class ResponseTimeHandler : DelegatingHandler
    {
        private readonly ILogger<ResponseTimeHandler> _logger;

        public ResponseTimeHandler(ILogger<ResponseTimeHandler> logger)
        {
            _logger = logger;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var sw = Stopwatch.StartNew();

            _logger.LogInformation($"Starting request. Endpoint:{request.RequestUri.ToString()} and Method:{request.Method.ToString()}");

            var response = await base.SendAsync(request, cancellationToken);

            _logger.LogInformation($"Finished request in {sw.ElapsedMilliseconds}ms, Endpoint:{request.RequestUri.ToString()} and Method:{request.Method.ToString()}");

            return response;
        }
    }
}
