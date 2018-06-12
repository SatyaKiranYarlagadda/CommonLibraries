using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Common.Foundation.Api.Http.DelegateHandlers
{
    public class LoggingHandler : DelegatingHandler
    {
        private readonly ILogger<LoggingHandler> _logger;

        public LoggingHandler(ILogger<LoggingHandler> logger)
        {
            _logger = logger;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            _logger.LogTrace($"Starting request. Endpoint:{request.RequestUri.ToString()}," +
                $" Method:{request.Method.ToString()}," +
                $" RequestHeaders:{GetSerializedString(request.Headers)}," +
                $" RequestBody:{await request.Content.ReadAsStringAsync()}");

            var response = await base.SendAsync(request, cancellationToken);

            _logger.LogTrace($"Completed request. Endpoint:{request.RequestUri.ToString()}," +
                $" Method:{request.Method.ToString()}," +
                $" ResponseHeaders:{GetSerializedString(response.Headers)}," +
                $" ResponseBody:{await response.Content.ReadAsStringAsync()}");

            return response;
        }

        private static string GetSerializedString(object value)
        {
            return JsonConvert.SerializeObject(value, Formatting.None,
                            new JsonSerializerSettings
                            {
                                NullValueHandling = NullValueHandling.Ignore
                            });
        }
    }
}
