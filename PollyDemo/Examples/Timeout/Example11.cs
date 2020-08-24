using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Timeout;

namespace PollyDemo.Examples.Timeout
{
    public class Example11 : AbstractExample
    {
        private readonly ILogger _logger;

        public Example11(ILogger logger)
        {
            _logger = logger;
        }

        public override string Description => $"Pessimistic timeout after {TimeoutInSeconds} seconds";

        public int TimeoutInSeconds { get; } = 5;

        public override Task RunAsync()
        {
            Task OnTimeoutAsync(Context context, TimeSpan delay, Task task, Exception exception)
            {
                _logger.LogError("An exception '{message}' has occured after '{delay}' seconds", exception.Message, delay.TotalSeconds);
                return Task.CompletedTask;
            }

            return Policy
                .TimeoutAsync(TimeoutInSeconds, TimeoutStrategy.Pessimistic, OnTimeoutAsync)
                .ExecuteAsync(LongOperationAsync);
        }
    }
}
