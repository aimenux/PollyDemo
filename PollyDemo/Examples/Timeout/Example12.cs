using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Timeout;

namespace PollyDemo.Examples.Timeout
{
    public class Example12 : AbstractExample
    {
        private readonly ILogger _logger;

        public Example12(ILogger logger)
        {
            _logger = logger;
        }

        public override string Description => $"Optimistic timeout after {TimeoutInSeconds} seconds";

        public int TimeoutInSeconds { get; } = 5;

        public override Task RunAsync()
        {
            Task OnTimeoutAsync(Context context, TimeSpan delay, Task task, Exception exception)
            {
                _logger.LogError("An exception '{message}' has occured after '{delay}' seconds", exception.Message, delay.TotalSeconds);
                return Task.CompletedTask;
            }

            var millisecondsDelay = TimeoutInSeconds * 1000;
            var cancellationTokenSource = new CancellationTokenSource(millisecondsDelay);
            return Policy
                .TimeoutAsync(TimeoutInSeconds, TimeoutStrategy.Optimistic, OnTimeoutAsync)
                .ExecuteAsync(() => LongOperationAsync(cancellationTokenSource.Token));
        }
    }
}
