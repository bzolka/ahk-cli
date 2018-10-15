using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AHK
{
    class Program
    {
        static void Main(string[] args)
        {
            var appConfig = getAppConfig();

            var loggerFactory = new LoggerFactory()
                        .AddConsole(LogLevel.Warning, true);

            createCliApp(appConfig, loggerFactory)
                .Execute(args);
        }

        private static Microsoft.Extensions.CommandLineUtils.CommandLineApplication createCliApp(AppConfig appConfig, ILoggerFactory loggerFactory)
        {
            var cliApp = new Microsoft.Extensions.CommandLineUtils.CommandLineApplication(throwOnUnexpectedArg: false);
            cliApp.Command("run",
                runCommandConfig => {
                    runCommandConfig.Description = "Kiertekeles futtatasa hallgatoi megoldasokon";
                    var konfigArg = runCommandConfig.Option("-k|--konfiguracio", "Futtatast leiro konfiguracios fajl", Microsoft.Extensions.CommandLineUtils.CommandOptionType.SingleValue);
                    var megoldasArg = runCommandConfig.Option("-m|--megoldas", "Hallgatoi megoldasokat tartalmazo konyvtar", Microsoft.Extensions.CommandLineUtils.CommandOptionType.SingleValue);
                    var eredmenyArg = runCommandConfig.Option("-e|--eredmeny", "Eredmenyek ebbe a konyvtarba keruljenek", Microsoft.Extensions.CommandLineUtils.CommandOptionType.SingleValue);

                    runCommandConfig.OnExecute(async () => {
                        if (!konfigArg.HasValue() || !megoldasArg.HasValue() || !eredmenyArg.HasValue())
                        {
                            runCommandConfig.ShowHelp();
                            return -1;
                        }

                        return await RunCommand.Execute(konfigArg.Value(), megoldasArg.Value(), eredmenyArg.Value(), appConfig, loggerFactory);
                    });
                },
                throwOnUnexpectedArg: false);

            cliApp.Command("clean",
                cleanCommandConfig => {
                    cleanCommandConfig.Description = "Felbemaradt futtatasok eltakaritasa";
                    cleanCommandConfig.OnExecute(() => CleanCommand.Execute(loggerFactory.CreateLogger("Cleanup")));
                },
                throwOnUnexpectedArg: false);


            cliApp.HelpOption("-?|-h|--help");
            cliApp.FullName = "AUTomatikus Hazi Kiertekelo";
            cliApp.OnExecute(() => { cliApp.ShowHelp(); return 0; });

            return cliApp;
        }

        private static AppConfig getAppConfig()
        {
            var configRoot = new ConfigurationBuilder()
                                    .AddJsonFile(@"AppConfig.json", optional: true, reloadOnChange: false)
                                    .AddEnvironmentVariables("AHK_")
                                    .Build();
            var appConfig = new AppConfig();
            configRoot.Bind(appConfig);
            return appConfig;
        }
    }
}
