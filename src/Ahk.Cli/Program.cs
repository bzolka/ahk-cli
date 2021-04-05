using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Spectre.Console;

namespace Ahk
{
    class Program
    {
        static void Main(string[] args)
        {
            var appConfig = getAppConfig();

            var loggerFactory = LoggerFactory.Create(
                loggerBuilder => loggerBuilder
                    .AddConsole()
                    .SetMinimumLevel(LogLevel.Warning)
                    .AddFile(pathFormat: System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Ahk-cli/Ahk-cli-{Date}.log"), minimumLevel: LogLevel.Information));

            createCliApp(appConfig, loggerFactory.CreateLogger("Run"))
                .Execute(args);
        }

        private static Microsoft.Extensions.CommandLineUtils.CommandLineApplication createCliApp(AppConfig appConfig, ILogger logger)
        {
            var cliApp = new Microsoft.Extensions.CommandLineUtils.CommandLineApplication(throwOnUnexpectedArg: false);
            cliApp.HelpOption("-?|-h|--help");
            cliApp.FullName = "AUTomated homework evaluation";
            cliApp.ShowRootCommandFullNameAndVersion();

            var megoldasArg = cliApp.Option("-m|--megoldas", "Hallgatoi megoldasokat tartalmazo konyvtar. Alapertelmezesben az aktualis konyvtar.", Microsoft.Extensions.CommandLineUtils.CommandOptionType.SingleValue);
            var konfigArg = cliApp.Option("-k|--konfiguracio", "Futtatast leiro konfiguracios fajl. Alapertelmeyesben a megoldasok konyvtaraban keresett json fajl.", Microsoft.Extensions.CommandLineUtils.CommandOptionType.SingleValue);
            var eredmenyArg = cliApp.Option("-e|--eredmeny", "Futas eredmenyeinek helye. Alapertelmezesben az aktualis konyvtarban letrehozott uj konyvtar.", Microsoft.Extensions.CommandLineUtils.CommandOptionType.SingleValue);

            cliApp.OnExecute(async () =>
            {
                try
                {
                    Execution.ExecutionStatistics? execStatistics = null;

                    await AnsiConsole.Progress()
                          .Columns(new ProgressColumn[]
                          {
                              new TaskDescriptionColumn(),
                              new ProgressBarColumn(),
                              new PercentageColumn(),
                              new RemainingTimeColumn(),
                          })
                          .StartAsync(async ctx =>
                          {
                              execStatistics = await AppRunner.Go(megoldasArg.Value(), konfigArg.Value(), eredmenyArg.Value(), appConfig, new ConsoleProgress.SpectreConsoleProgress(ctx), logger);
                          });

                    if (execStatistics!.HasFailed())
                    {
                        AnsiConsole.MarkupLine($"[{Color.Yellow}]Some submissions were not evaluated:[/] [{Color.Green}]{execStatistics.ExecutedSuccessfully} completed[/] and [{Color.Red}]{execStatistics.FailedExecution} FAILED[/].");
                        return -1;
                    }
                    else
                    {
                        AnsiConsole.MarkupLine($"[{Color.Green}]Completed the evaluation of {execStatistics.ExecutedSuccessfully} submissions[/]");
                        return 0;
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Runtime error");
                    AnsiConsole.MarkupLine($"[{Color.Red}]FAILED[/]");
                    return -1;
                }
            });

            return cliApp;
        }

        private static AppConfig getAppConfig()
        {
            var configRoot = new ConfigurationBuilder()
                                    .AddJsonFile(@"AppConfig.json", optional: true, reloadOnChange: false)
                                    .AddEnvironmentVariables("Ahk_")
                                    .Build();
            return configRoot.Get<AppConfig>();
        }
    }
}
