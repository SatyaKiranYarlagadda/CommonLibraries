using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using Microsoft.Extensions.Logging;

namespace Common.Foundation.Interceptors
{
    public class LoggingInterceptor : IInterceptor
    {
        private readonly ILogger<LoggingInterceptor> _logger;

        public LoggingInterceptor(ILogger<LoggingInterceptor> logger)
        {
            _logger = logger;
        }

        public void Intercept(IInvocation invocation)
        {
            if (InterceptorUtils.IsAsyncMethod(invocation.Method))
            {
                LogAsync(invocation);
            }
            else
            {
                LogSync(invocation);
            }

        }

        private void LogSync(IInvocation invocation)
        {
            _logger.LogInformation($"Method: {invocation.Method.Name} invoked.");
            if (invocation.Arguments != null && invocation.Arguments.Any())
            {
                if (invocation.Arguments.Length == 1)
                {
                    var request = invocation.Arguments.FirstOrDefault() ?? "null";
                    _logger.LogInformation("Args: {@Request}", request);

                }
                else
                {
                    _logger.LogDebug($"Args: {string.Join(", ", invocation.Arguments.Select(a => (a ?? "").ToString()).ToArray())}");
                }
            }

            var stopWatch = Stopwatch.StartNew();
            invocation.Proceed();
            stopWatch.Stop();
            _logger.LogInformation($"Method: {invocation.Method.Name} completed. ElapedTime: {stopWatch.ElapsedMilliseconds} ms");
            _logger.LogInformation("Result: {@Result}", invocation.ReturnValue);
        }

        private void LogAsync(IInvocation invocation)
        {
            _logger.LogInformation($"Method: {invocation.Method.Name} invoked.");
            if (invocation.Arguments != null && invocation.Arguments.Any())
            {
                if (invocation.Arguments.Length == 1)
                {
                    var request = invocation.Arguments.FirstOrDefault() ?? "null";
                    _logger.LogInformation("Args: {@Request}", request);

                }
                else
                {
                    _logger.LogDebug($"Args: {string.Join(", ", invocation.Arguments.Select(a => (a ?? "").ToString()).ToArray())}");
                }
            }

            var stopWatch = Stopwatch.StartNew();
            invocation.Proceed();

            ((Task)invocation.ReturnValue)
                .ContinueWith(task =>
                {
                    //After method execution
                    stopWatch.Stop();
                    _logger.LogInformation($"Method: {invocation.Method.Name} completed. ElapedTime: {stopWatch.ElapsedMilliseconds} ms");
                    _logger.LogInformation("Result: {@Result}", task);
                });
        }
    }
}
