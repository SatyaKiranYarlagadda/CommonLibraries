using Microsoft.Extensions.Logging;
using Polly;
using Polly.Wrap;
using System;
using System.Threading.Tasks;

namespace Common.Foundation.FaultTolerance
{
    public class RetryableOperation : IRetryableOperation
    {
        private readonly ILogger<RetryableOperation> _logger;
        private readonly int _defaultRetryAttemptInSeconds = 1;
        private readonly int _exceptionsAllowedBeforeBreaking = 5;

        public RetryableOperation(ILogger<RetryableOperation> logger)
        {
            _logger = logger;
        }

        public async Task<TResult> TryExecuteAsync<TResult>(Func<Task<TResult>> actionToPerform, Func<Exception, bool> isRetryableException, int retryCount = 5, TimeSpan? retryAttempt = null, bool isLinearRetry = false)
        {
            var retryAttemptInSeconds = retryAttempt.HasValue ? retryAttempt.Value.Seconds : _defaultRetryAttemptInSeconds;
            var policies = new Policy[]
            {
                Policy.Handle<Exception>(isRetryableException)
                .WaitAndRetryAsync(
                    // number of retries
                    retryCount,
                    // backoff
                    retryTimeSpan => isLinearRetry ? TimeSpan.FromSeconds(retryAttemptInSeconds) : TimeSpan.FromSeconds(Math.Pow(2, retryAttemptInSeconds)),
                    // on retry
                    (exception, timeSpan, currentRetryCount, context) =>
                    {
                        var msg = $"Retry ({currentRetryCount} of {retryCount}) due to: {exception}.";
                        _logger.LogWarning(msg);
                        _logger.LogDebug(msg);
                    }),
                Policy.Handle<Exception>(isRetryableException)
                .CircuitBreakerAsync( 
                   // number of exceptions before breaking circuit
                   _exceptionsAllowedBeforeBreaking,
                   // time circuit opened before retry
                   TimeSpan.FromMinutes(1),
                   (exception, duration) =>
                   {
                        // on circuit opened
                        _logger.LogTrace("Circuit breaker opened");
                   },
                   () =>
                   {
                        // on circuit closed
                        _logger.LogTrace("Circuit breaker reset");
                   })
            };

            var policyWrap = PolicyWrap.WrapAsync(policies);
            return await policyWrap.ExecuteAsync<TResult>(actionToPerform);
        }

        public TResult TryExecute<TResult>(Func<TResult> actionToPerform, Func<Exception, bool> isRetryableException, int retryCount = 5, TimeSpan? retryAttempt = null, bool isLinearRetry = false)
        {
            var retryAttemptInSeconds = retryAttempt.HasValue ? retryAttempt.Value.Seconds : _defaultRetryAttemptInSeconds;
            var policies = new Policy[]
            {
                Policy.Handle<Exception>(isRetryableException)
                .WaitAndRetry(
                    // number of retries
                    retryCount,
                    // backoff
                    retryTimeSpan => isLinearRetry ? TimeSpan.FromSeconds(retryAttemptInSeconds) : TimeSpan.FromSeconds(Math.Pow(2, retryAttemptInSeconds)),
                    // on retry
                    (exception, timeSpan, currentRetryCount, context) =>
                    {
                        var msg = $"Retry ({currentRetryCount} of {retryCount}) due to: {exception}.";
                        _logger.LogWarning(msg);
                        _logger.LogDebug(msg);
                    }),
                Policy.Handle<Exception>(isRetryableException)
                .CircuitBreaker( 
                   // number of exceptions before breaking circuit
                   _exceptionsAllowedBeforeBreaking,
                   // time circuit opened before retry
                   TimeSpan.FromMinutes(1),
                   (exception, duration) =>
                   {
                        // on circuit opened
                        _logger.LogTrace("Circuit breaker opened");
                   },
                   () =>
                   {
                        // on circuit closed
                        _logger.LogTrace("Circuit breaker reset");
                   })
            };

            var policyWrap = PolicyWrap.Wrap(policies);
            return policyWrap.Execute<TResult>(actionToPerform);
        }
    }
}
