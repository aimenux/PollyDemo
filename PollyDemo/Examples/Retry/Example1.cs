using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Polly;

namespace PollyDemo.Examples.Retry
{
    public class Example1 : AbstractExample
    {
        private readonly ILogger _logger;

        public Example1(ILogger logger)
        {
            _logger = logger;
        }

        public override string Description { get; } = "Retry forever";

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
                .RetryForeverAsync(OnRetry())
                .ExecuteAsync(FrequentlyFailedOperationAsync);
        }
    }
}
