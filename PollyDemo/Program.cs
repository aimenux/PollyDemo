using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PollyDemo.Examples;
using PollyDemo.Examples.CircuitBreaker;
using PollyDemo.Examples.Fallback;
using PollyDemo.Examples.Retry;
using PollyDemo.Examples.Timeout;
using PollyDemo.Examples.WaitAndRetry;

namespace PollyDemo
{
    public static class Program
    {
        public static async Task Main()
        {
            var services = new ServiceCollection();
            services.AddLogging(builder =>
            {
                builder.AddConsole(options =>
                {
                    options.DisableColors = false;
                    options.TimestampFormat = "[HH:mm:ss:fff] ";
                });
                builder.AddNonGenericLogger();
                builder.SetMinimumLevel(LogLevel.Trace);
            });

            services.AddTransient<IExample, Example1>();
            services.AddTransient<IExample, Example2>();
            services.AddTransient<IExample, Example3>();
            services.AddTransient<IExample, Example4>();
            services.AddTransient<IExample, Example5>();
            services.AddTransient<IExample, Example6>();
            services.AddTransient<IExample, Example7>();
            services.AddTransient<IExample, Example8>();
            services.AddTransient<IExample, Example9>();
            services.AddTransient<IExample, Example10>();
            services.AddTransient<IExample, Example11>();
            services.AddTransient<IExample, Example12>();

            var serviceProvider = services.BuildServiceProvider();
            var logger = serviceProvider.GetService<ILogger>();
            var examples = serviceProvider.GetServices<IExample>();

            foreach (var example in examples)
            {
                var name = example.GetType().Name;
                var description = example.Description;
                logger.LogInformation($"{name} -> {description}");

                try
                {
                    await example.RunAsync();

                    logger.LogInformation("Example '{name}' is terminated successfully", name);
                }
                catch (Exception ex)
                {
                    logger.LogCritical("An error has occured in example '{name}': {ex}", name, ex);
                }
            }

            Console.WriteLine("Press any key to exit !\n");
            Console.ReadKey();
        }
    }
}
