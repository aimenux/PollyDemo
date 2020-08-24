using System;
using System.Threading;
using System.Threading.Tasks;

namespace PollyDemo.Examples
{
    public abstract class AbstractExample : IExample
    {
        public const int LongOperationDefaultDuration = 30;

        public string Name => GetType().Name;

        public abstract string Description { get; }

        public abstract Task RunAsync();

        protected static Task AlwaysSuccessfulOperationAsync()
        {
            return Task.CompletedTask;
        }

        protected static Task AlwaysFailedOperationAsync()
        {
            throw new ArgumentException(nameof(AlwaysFailedOperationAsync));
        }

        protected static Task SometimesFailedOperationAsync()
        {
            var randomValue = Randomize.Get(1, 1000);

            if (randomValue % 2 == 0)
            {
                throw new ArgumentException(nameof(SometimesFailedOperationAsync));
            }

            return Task.CompletedTask;
        }

        protected static Task FrequentlyFailedOperationAsync() => FrequentlyFailedOperationAsync(50);

        protected static Task FrequentlyFailedOperationAsync(int threshold)
        {
            var randomValue = Randomize.Get(1, 1000);

            if (randomValue > threshold)
            {
                throw new ArgumentException(nameof(FrequentlyFailedOperationAsync));
            }

            return Task.CompletedTask;
        }

        protected static Task ConditionalFailedOperationAsync(Predicate<int> failureCondition, int parameter)
        {
            if (failureCondition(parameter))
            {
                throw new ArgumentException(nameof(ConditionalFailedOperationAsync));
            }

            return Task.CompletedTask;
        }

        protected static Task LongOperationAsync() => LongOperationAsync(LongOperationDefaultDuration);

        protected static Task LongOperationAsync(int durationInSeconds)
        {
            return Task.Delay(TimeSpan.FromSeconds(durationInSeconds));
        }

        protected static Task LongOperationAsync(CancellationToken cancellationToken)
        {
            return Task.Delay(TimeSpan.FromSeconds(LongOperationDefaultDuration), cancellationToken);
        }

        private static class Randomize
        {
            private static readonly Random Random = new Random(Guid.NewGuid().GetHashCode());

            public static int Get(int min, int max) => Random.Next(min, max);
        }
    }
}