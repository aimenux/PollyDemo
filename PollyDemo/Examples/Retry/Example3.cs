using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Polly;

namespace PollyDemo.Examples.Retry
{
    public class Example3 : AbstractExample
    {
        private readonly ILogger _logger;

        public int MaxRetry { get; } = 3;

        public Example3(ILogger logger)
        {
            _logger = logger;
        }

        public override string Description => $"Retry {MaxRetry} times";

        public override Task RunAsync()
        {
            Action<Exception, int> OnRetry()
            {
                return (exception, retryCount) =>
                {
                    _logger.LogWarning("Example '{example}' is failed ({retry})", Name, retryCount);
                };
            }

            return Policy
                .Handle<ArgumentException>()
                .RetryAsync(MaxRetry, OnRetry())
                .ExecuteAsync(AlwaysFailedOperationAsync);
        }
    }
}
