using System;
using System.Threading.Tasks;

namespace Common.Foundation.FaultTolerance
{
    public interface IRetryableOperation
    {
        Task<TResult> TryExecuteAsync<TResult>(Func<Task<TResult>> actionToPerform, Func<Exception, bool> isRetryableException, int retryCount = 5, TimeSpan? retryAttempt = null, bool isLinearRetry=false);

        TResult TryExecute<TResult>(Func<TResult> actionToPerform, Func<Exception, bool> isRetryableException, int retryCount = 5, TimeSpan? retryAttempt = null, bool isLinearRetry = false);
    }
}
