using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Ahk
{
    /// <summary>
    /// Not a true Startup.cs, but acts as for for all intents and purposes
    /// </summary>
    internal class Startup
    {
        public static IServiceProvider GetServiceProvider()
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            return services.BuildServiceProvider();
        }

        /// <summary>
        /// Register dependency injection here.
        /// </summary>
        private static void ConfigureServices(IServiceCollection services)
        {
            var logger = LoggerFactory.Create(
                            loggerBuilder => loggerBuilder
                                .SetMinimumLevel(LogLevel.Warning)
                                .ClearProviders()
                                .AddProvider(new AnsiConsoleLoggerProvider())
                                .AddFile(pathFormat: System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Ahk-cli/Ahk-cli-{Date}.log"), minimumLevel: LogLevel.Information))
                            .CreateLogger("Ahk-cli");
            services.AddSingleton<ILogger>(logger);

            services.AddTransient<Commands.Eval.DockerTrxEvaluateCommand>();
            services.AddTransient<Commands.Eval.DockerConsoleMessageEvaluateCommand>();
            services.AddTransient<Commands.CleanupContainersCommand>();
        }
    }
}
