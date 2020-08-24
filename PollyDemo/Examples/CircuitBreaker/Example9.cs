using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.CircuitBreaker;

namespace PollyDemo.Examples.CircuitBreaker
{
    public class Example9 : AbstractExample
    {
        private readonly ILogger _logger;

        public Example9(ILogger logger)
        {
            _logger = logger;
        }

        public double FailureThreshold { get; } = 0.5f;

        public int DurationOfBreakInSeconds { get; } = 2;

        public int SamplingMinimumThroughput { get; } = 10;

        public int SamplingDurationInSeconds { get; } = 15;

        public override string Description => $"Advanced circuit breaker after {FailureThreshold * 100}% failures during {SamplingDurationInSeconds} seconds";

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
                .AdvancedCircuitBreakerAsync(FailureThreshold,
                    TimeSpan.FromSeconds(SamplingDurationInSeconds),
                    SamplingMinimumThroughput,
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
            var stopFailureAfterThreshold = SamplingMinimumThroughput + 1;
            bool FailureCondition(int currentCounter) => currentCounter < stopFailureAfterThreshold;
            return waitAndRetryPolicy
                .WrapAsync(circuitBreakerPolicy)
                .ExecuteAsync(() => ConditionalFailedOperationAsync(FailureCondition, counter++));
        }
    }
}
