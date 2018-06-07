using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Common.Foundation.Api.Behaviors
{
    public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;
        public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger) => _logger = logger;

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            _logger.LogInformation($"Handling {typeof(TRequest).Name}");
            _logger.LogDebug($"RequestName: {typeof(TRequest).Name} RequestObject: {JsonConvert.SerializeObject(request)}");

            var sw = Stopwatch.StartNew();

            var response = await next();

            _logger.LogInformation($"Handled {typeof(TResponse).Name}");
            _logger.LogDebug($"ResponseName: {typeof(TResponse).Name} ResponseObject: {JsonConvert.SerializeObject(response)}");

            _logger.LogInformation($"Total timespent to handle the request:{typeof(TRequest).Name} in millinseconds:{sw.ElapsedMilliseconds}");
            return response;
        }
    }
}
