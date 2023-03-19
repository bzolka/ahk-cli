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
                        .AddConsole(LogLevel.Warning, true)
                        .AddFile(
                                 pathFormat: System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData), "AHK/Log/AHK-{Date}.log"),
                                 minimumLevel: LogLevel.Information);

            createCliApp(appConfig, loggerFactory)
                .Execute(args);
        }

        private static Microsoft.Extensions.CommandLineUtils.CommandLineApplication createCliApp(AppConfig appConfig, ILoggerFactory loggerFactory)
        {
            var cliApp = new Microsoft.Extensions.CommandLineUtils.CommandLineApplication(throwOnUnexpectedArg: false);
            cliApp.HelpOption("-?|-h|--help");
            cliApp.FullName = "AUTomatikus Hazi Kiertekelo";

            var megoldasArg = cliApp.Option("-m|--megoldas", "Hallgatoi megoldasokat tartalmazo konyvtar. Alapertelmezesben az aktualis konyvtar.", Microsoft.Extensions.CommandLineUtils.CommandOptionType.SingleValue);
            var konfigArg = cliApp.Option("-k|--konfiguracio", "Futtatast leiro konfiguracios fajl. Alapertelmeyesben a megoldasok konyvtaraban keresett json fajl.", Microsoft.Extensions.CommandLineUtils.CommandOptionType.SingleValue);
            var eredmenyArg = cliApp.Option("-e|--eredmeny", "Futas eredmenyeinek helye. Alapertelmezesben az aktualis konyvtarban letrehozott uj konyvtar.", Microsoft.Extensions.CommandLineUtils.CommandOptionType.SingleValue);
            // TODO-BZ !!!
            var mellozArg = cliApp.Option("-z|--melloz", "Opcionális csv fájl útvonala, mely azon hallgatók GitHub azonosítót tartalmazza (megegyezik az alkönyvátak nevével), melyek ellen.", Microsoft.Extensions.CommandLineUtils.CommandOptionType.SingleValue);

            cliApp.OnExecute(async () => {
                return await AppRunner.Go(megoldasArg.Value(), konfigArg.Value(), eredmenyArg.Value(), mellozArg.Value(), appConfig, loggerFactory.CreateLogger("Run"));
            });

            return cliApp;
        }

        private static AppConfig getAppConfig()
        {
            var configRoot = new ConfigurationBuilder()
                                    .AddJsonFile(@"AppConfig.json", optional: true, reloadOnChange: false)
                                    .AddEnvironmentVariables("AHK_")
                                    .Build();
            return configRoot.Get<AppConfig>();
        }
    }
}
