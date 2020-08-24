using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Polly;

namespace PollyDemo.Examples.WaitAndRetry
{
    public class Example7 : AbstractExample
    {
        private readonly ILogger _logger;

        public int MaxRetry { get; } = 3;

        public Example7(ILogger logger)
        {
            _logger = logger;
        }

        public override string Description => $"Retry each function-time, MaxRetry is {MaxRetry} times";

        public override Task RunAsync()
        {
            TimeSpan SleepDurationProvider(int retryCount)
            {
                var waitTimeInSeconds = Math.Pow(2, retryCount);
                _logger.LogWarning("Example '{example}' is failed, retry ({retry}) after ({seconds}) seconds", Name, retryCount, waitTimeInSeconds);
                return TimeSpan.FromSeconds(waitTimeInSeconds);
            }

            return Policy
                .Handle<ArgumentException>()
                .WaitAndRetryAsync(MaxRetry, SleepDurationProvider)
                .ExecuteAsync(AlwaysFailedOperationAsync);
        }
    }
}
