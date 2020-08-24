using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Polly;

namespace PollyDemo.Examples.WaitAndRetry
{
    public class Example4 : AbstractExample
    {
        private readonly ILogger _logger;

        public int WaitTimeInSeconds { get; } = 1;

        public Example4(ILogger logger)
        {
            _logger = logger;
        }

        public override string Description => $"Retry forever each {WaitTimeInSeconds} seconds";

        public override Task RunAsync()
        {
            TimeSpan SleepDurationProvider(int retryCount)
            {
                _logger.LogWarning("Example '{example}' is failed, retry ({retry}) after ({seconds}) seconds", Name, retryCount, WaitTimeInSeconds);
                return TimeSpan.FromSeconds(WaitTimeInSeconds);
            }

            return Policy
                .Handle<ArgumentException>()
                .WaitAndRetryForeverAsync(SleepDurationProvider)
                .ExecuteAsync(AlwaysFailedOperationAsync);
        }
    }
}
