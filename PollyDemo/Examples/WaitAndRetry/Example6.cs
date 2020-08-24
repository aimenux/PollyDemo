using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Polly;

namespace PollyDemo.Examples.WaitAndRetry
{
    public class Example6 : AbstractExample
    {
        private readonly ILogger _logger;

        public int MaxRetry { get; } = 3;

        public IEnumerable<TimeSpan> WaitTimeInSeconds { get; } = new[]
        {
            TimeSpan.FromSeconds(1),
            TimeSpan.FromSeconds(2),
            TimeSpan.FromSeconds(3)
        };

        public Example6(ILogger logger)
        {
            _logger = logger;
        }

        public override string Description
        {
            get
            {
                var seconds = WaitTimeInSeconds.Select(x => x.TotalSeconds);
                return $"Retry each {string.Join(",", seconds)} seconds, MaxRetry is {MaxRetry} times";
            }
        }

        public override Task RunAsync()
        {
            void OnRetry(Exception ex, TimeSpan currentWaitTime, int retryCount, Context context)
            {
                _logger.LogWarning("Example '{example}' is failed, retry ({retry}) after ({seconds}) seconds", Name, retryCount, currentWaitTime.TotalSeconds);
            }

            return Policy
                .Handle<ArgumentException>()
                .WaitAndRetryAsync(WaitTimeInSeconds, OnRetry)
                .ExecuteAsync(AlwaysFailedOperationAsync);
        }
    }
}
