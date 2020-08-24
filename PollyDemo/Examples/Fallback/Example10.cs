using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Polly;

namespace PollyDemo.Examples.Fallback
{
    public class Example10 : AbstractExample
    {
        private readonly ILogger _logger;

        public Example10(ILogger logger)
        {
            _logger = logger;
        }

        public override string Description { get; } = "Fallback";

        public override Task RunAsync()
        {
            Task OnFallbackAsync(Exception ex)
            {
                _logger.LogWarning("An exception has occured, fallback will be called: {ex}", ex);
                return Task.CompletedTask;
            }

            Task FallbackAction(CancellationToken cancellationToken)
            {
                _logger.LogWarning("Fallback '{action}' is called", nameof(AlwaysSuccessfulOperationAsync));
                return AlwaysSuccessfulOperationAsync();
            }

            return Policy
                .Handle<ArgumentException>()
                .FallbackAsync(FallbackAction, OnFallbackAsync)
                .ExecuteAsync(FrequentlyFailedOperationAsync);
        }
    }
}
