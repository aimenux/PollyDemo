using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Polly;

namespace PollyDemo.Examples.WaitAndRetry
{
    public class Example5 : AbstractExample
    {
        private readonly ILogger _logger;

        public int MaxRetry { get; } = 3;

        public int WaitTimeInSeconds { get; } = 1;

        public Example5(ILogger logger)
        {
            _logger = logger;
        }

        public override string Description => $"Retry each {WaitTimeInSeconds} seconds, MaxRetry is {MaxRetry} times";

        public override Task RunAsync()
        {
            TimeSpan SleepDurationProvider(int retryCount)
            {
                _logger.LogWarning("Example '{example}' is failed, retry ({retry}) after ({seconds}) seconds", Name, retryCount, WaitTimeInSeconds);
                return TimeSpan.FromSeconds(WaitTimeInSeconds);
            }

            return Policy
                .Handle<ArgumentException>()
                .WaitAndRetryAsync(MaxRetry, SleepDurationProvider)
                .ExecuteAsync(AlwaysFailedOperationAsync);
        }
    }
}
