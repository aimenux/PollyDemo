using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.CircuitBreaker;

namespace PollyDemo.Examples.CircuitBreaker
{
    public class Example8 : AbstractExample
    {
        private readonly ILogger _logger;

        public Example8(ILogger logger)
        {
            _logger = logger;
        }

        public int DurationOfBreakInSeconds { get; } = 2;

        public int ExceptionsAllowedBeforeBreaking { get; } = 5;

        public override string Description => $"Circuit breaker after {ExceptionsAllowedBeforeBreaking} exceptions during {DurationOfBreakInSeconds} seconds";

        public override Task RunAsync()
        {
            void OnReset()
            {
                _logger.LogWarning("Circuit breaker is reset on example '{example}'", Name);
            }

            void OnBreak(Exception ex, TimeSpan durationOfBreak)
            {
                _logger.LogWarning("Circuit breaker is break on example '{example}' during {duration} seconds", Name, durationOfBreak.TotalSeconds);
            }

            void OnHalfOpen()
            {
                _logger.LogWarning("Circuit breaker is half open on example '{example}'", Name);
            }

            var circuitBreakerPolicy = Policy
                .Handle<ArgumentException>()
                .CircuitBreakerAsync(ExceptionsAllowedBeforeBreaking,
                    TimeSpan.FromSeconds(DurationOfBreakInSeconds),
                    OnBreak,
                    OnReset,
                    OnHalfOpen);

            const int waitTimeInSeconds = 1;
            TimeSpan SleepDurationProvider(int retryCount)
            {
                _logger.LogWarning("Example '{example}' is failed, retry ({retry}) after ({seconds}) seconds", Name, retryCount, waitTimeInSeconds);
                return TimeSpan.FromSeconds(waitTimeInSeconds);
            }

            var waitAndRetryPolicy = Policy.Handle<ArgumentException>().Or<BrokenCircuitException>()
                .WaitAndRetryForeverAsync(SleepDurationProvider);

            var counter = 0;
            var stopFailureAfterThreshold = ExceptionsAllowedBeforeBreaking + 1;
            bool FailureCondition(int currentCounter) => currentCounter < stopFailureAfterThreshold;
            return waitAndRetryPolicy
                .WrapAsync(circuitBreakerPolicy)
                .ExecuteAsync(() => ConditionalFailedOperationAsync(FailureCondition, counter++));
        }
    }
}
